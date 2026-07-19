using StartGame.Components;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.NetCode;

namespace StartGame.Systems
{
    [WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
    public partial struct PlayerConnectionHandleSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<LobbyPlayerPrefabComponent>();
            state.RequireForUpdate<EndSimulationEntityCommandBufferSystem.Singleton>();
            state.RequireForUpdate<NetworkStreamDriver>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var events = SystemAPI.GetSingleton<NetworkStreamDriver>().ConnectionEventsForTick;
            var ecb = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>().CreateCommandBuffer(state.WorldUnmanaged);

            foreach (var tickEvent in events)
            {
                var networkId = tickEvent.Id.Value;
                
                switch (tickEvent.State)
                {
                    case ConnectionState.State.Disconnected:
                        foreach (var (lobbyPlayer, entity) in SystemAPI.Query<RefRO<LobbyPlayerComponent>>().WithEntityAccess())
                        {
                            if (lobbyPlayer.ValueRO.NetworkId != networkId)
                                continue;
                            
                            ecb.DestroyEntity(entity);
                        }
                        continue;
                    case ConnectionState.State.Connected:
                        var lobbyPlayerPrefab = SystemAPI.GetSingleton<LobbyPlayerPrefabComponent>();
                        var lobbyPlayerEntity = ecb.Instantiate(lobbyPlayerPrefab.Value);
                        FixedString32Bytes name = $"Player{networkId}";
                        ecb.SetComponent(lobbyPlayerEntity, new LobbyPlayerComponent
                        {
                            NetworkId = networkId,
                            Name = name,
                            IsReady = false
                        });
                        continue;
                    default:
                        continue;
                }
            }
        }
    }
}