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
            var lobbyPlayerPrefab = SystemAPI.GetSingleton<LobbyPlayerPrefabComponent>().Value;

            var disconnectedIds = new NativeHashSet<int>(8, Allocator.Temp);

            foreach (var tickEvent in events)
            {
                if (tickEvent.State != ConnectionState.State.Disconnected)
                    continue;

                var networkId = tickEvent.Id.Value;
                disconnectedIds.Add(networkId);

                foreach (var (lobbyPlayer, entity) in SystemAPI.Query<RefRO<LobbyPlayerComponent>>().WithEntityAccess())
                {
                    if (lobbyPlayer.ValueRO.NetworkId != networkId)
                        continue;

                    ecb.DestroyEntity(entity);
                }
            }

            foreach (var networkId in SystemAPI.Query<RefRO<NetworkId>>())
            {
                var id = networkId.ValueRO.Value;
                if (disconnectedIds.Contains(id))
                    continue;

                var alreadyHasPlayer = false;
                
                foreach (var lobbyPlayer in SystemAPI.Query<RefRO<LobbyPlayerComponent>>())
                {
                    if (lobbyPlayer.ValueRO.NetworkId != id)
                        continue;

                    alreadyHasPlayer = true;
                    break;
                }

                if (alreadyHasPlayer)
                    continue;

                var lobbyPlayerEntity = ecb.Instantiate(lobbyPlayerPrefab);
                FixedString32Bytes name = $"Player{id}";
                ecb.SetComponent(lobbyPlayerEntity, new LobbyPlayerComponent
                {
                    NetworkId = id,
                    Name = name,
                    IsReady = false
                });
            }

            disconnectedIds.Dispose();
        }
    }
}