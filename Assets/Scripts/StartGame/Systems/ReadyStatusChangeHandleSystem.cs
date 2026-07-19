using StartGame.Components;
using Unity.Burst;
using Unity.Entities;
using Unity.NetCode;

namespace StartGame.Systems
{
    [WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
    public partial struct ReadyStatusChangeHandleSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<EndSimulationEntityCommandBufferSystem.Singleton>();
            state.RequireForUpdate<ReadyStatusChangeRpc>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var ecb = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>().CreateCommandBuffer(state.WorldUnmanaged);
            
            foreach (var (rpcCommand, entity) in SystemAPI.Query<RefRO<ReceiveRpcCommandRequest>>().WithAll<ReadyStatusChangeRpc>().WithEntityAccess())
            {
                var id = SystemAPI.GetComponent<NetworkId>(rpcCommand.ValueRO.SourceConnection);
                
                foreach (var lobbyPlayer in SystemAPI.Query<RefRW<LobbyPlayerComponent>>())
                {
                    if (lobbyPlayer.ValueRO.NetworkId != id.Value)
                        continue;

                    lobbyPlayer.ValueRW.IsReady = !lobbyPlayer.ValueRO.IsReady;
                }
                
                ecb.DestroyEntity(entity);
            }
        }
    }
}