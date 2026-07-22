using StartGame.Components;
using StartGame.Utils;
using Unity.Entities;
using Unity.NetCode;

namespace StartGame.Systems
{
    [WorldSystemFilter(WorldSystemFilterFlags.ClientSimulation)]
    public partial struct StartGameSystem : ISystem
    {
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
                NetworkSceneLoader.LoadGameSubScene(state.World);
                SceneFlowHelper.LoadGameAdditive();
                SceneFlowHelper.UnloadMenuIfLoaded();
                ecb.DestroyEntity(entity);
            }
        }
    }
}
