using StartGame.Components;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.NetCode;

namespace StartGame.Systems
{
    [WorldSystemFilter(WorldSystemFilterFlags.LocalSimulation)]
    public partial struct JoinRequestHandleSystem : ISystem
    {
        private EntityQuery _joinRequestQuery;
        
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<EndSimulationEntityCommandBufferSystem.Singleton>();
            state.RequireForUpdate<JoinRequestComponent>();
            
            _joinRequestQuery = SystemAPI.QueryBuilder().WithAll<JoinRequestComponent>().Build();
        }

        public void OnUpdate(ref SystemState state)
        {
            var ecb = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>().CreateCommandBuffer(state.WorldUnmanaged);
            foreach (var requestEntity in _joinRequestQuery.ToEntityArray(Allocator.Temp))
            {
                var clientWorld = ClientServerBootstrap.CreateClientWorld("ClientWorld");
                //add logic for connect or unlock join popup
                ecb.DestroyEntity(requestEntity);
            }
        }
    }
}