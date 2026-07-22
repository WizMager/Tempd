using StartGame.Components;
using Unity.Burst;
using Unity.Entities;
using Unity.NetCode;
using UnityEngine.SceneManagement;

namespace StartGame.Systems
{
    [WorldSystemFilter(WorldSystemFilterFlags.ClientSimulation)]
    public partial struct StartGameSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<EndSimulationEntityCommandBufferSystem.Singleton>();
            state.RequireForUpdate<StartGameRpc>();
        }
        
        public void OnUpdate(ref SystemState state)
        {
            var ecb = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>().CreateCommandBuffer(state.WorldUnmanaged);
            
            foreach (var (_, _, entity) in SystemAPI.Query<RefRO<StartGameRpc>, RefRO<ReceiveRpcCommandRequest>>().WithEntityAccess())
            {
                SceneManager.LoadScene("Game");
                
                ecb.DestroyEntity(entity);
            }
        }
    }
}