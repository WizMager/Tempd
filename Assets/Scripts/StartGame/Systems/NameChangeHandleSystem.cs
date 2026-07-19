using StartGame.Components;
using Unity.Burst;
using Unity.Entities;
using Unity.NetCode;

namespace StartGame.Systems
{
    [WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
    public partial struct NameChangeHandleSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<EndSimulationEntityCommandBufferSystem.Singleton>();
            state.RequireForUpdate<ChangeNameRpc>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var ecb = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>().CreateCommandBuffer(state.WorldUnmanaged);
            
            foreach (var (changeName, rpcCommandRequest, entity) in SystemAPI.Query<RefRO<ChangeNameRpc>, RefRO<ReceiveRpcCommandRequest>>().WithEntityAccess())
            {
                var connection = rpcCommandRequest.ValueRO.SourceConnection;
                var id = SystemAPI.GetComponent<NetworkId>(connection).Value;
                var name = changeName.ValueRO.Name;

                foreach (var lobbyPlayer in SystemAPI.Query<RefRW<LobbyPlayerComponent>>())
                {
                    if (lobbyPlayer.ValueRO.NetworkId != id)
                        continue;

                    lobbyPlayer.ValueRW.Name = name;
                }
                
                ecb.DestroyEntity(entity);
            }
        }
    }
}