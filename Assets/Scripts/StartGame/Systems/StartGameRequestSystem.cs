using StartGame.Components;
using Unity.Burst;
using Unity.Entities;
using Unity.NetCode;

namespace StartGame.Systems
{
    [WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
    public partial struct StartGameRequestSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<EndSimulationEntityCommandBufferSystem.Singleton>();
            state.RequireForUpdate<StartGameRequestRpc>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var ecb = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>().CreateCommandBuffer(state.WorldUnmanaged);
            
            foreach (var (_, _, entity) in SystemAPI.Query<RefRO<StartGameRequestRpc>, RefRO<ReceiveRpcCommandRequest>>().WithEntityAccess())
            {
                var isAllReady = true;
                
                foreach (var lobbyPlayer in SystemAPI.Query<RefRO<LobbyPlayerComponent>>())
                {
                    if (lobbyPlayer.ValueRO.IsReady) 
                        continue;
                    
                    isAllReady = false;
                    
                    break;
                }

                if (isAllReady)
                {
                    var startNotifyEntity = ecb.CreateEntity();
                    ecb.AddComponent<StartGameRpc>(startNotifyEntity);
                    ecb.AddComponent<SendRpcCommandRequest>(startNotifyEntity);
                }
                
                ecb.DestroyEntity(entity);
            }
        }
    }
}