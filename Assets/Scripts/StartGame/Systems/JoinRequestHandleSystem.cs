using StartGame.Components;
using Unity.Burst;
using Unity.Entities;
using Unity.NetCode;
using Unity.Networking.Transport;

namespace StartGame.Systems
{
    [WorldSystemFilter(WorldSystemFilterFlags.LocalSimulation)]
    public partial struct JoinRequestHandleSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<EndSimulationEntityCommandBufferSystem.Singleton>();
            state.RequireForUpdate<JoinRequestComponent>();
        }

        public void OnUpdate(ref SystemState state)
        {
            foreach (var (requestComponent, entity) in SystemAPI.Query<JoinRequestComponent>().WithEntityAccess())
            {
                var clientWorld = ClientServerBootstrap.CreateClientWorld("ClientWorld");
                var networkEndpoint = NetworkEndpoint.Parse(requestComponent.EnteredIpAddress.ToString(), 7979);
                var requestConnectEntity = clientWorld.EntityManager.CreateEntity();
                clientWorld.EntityManager.AddComponentData(requestConnectEntity, new NetworkStreamRequestConnect
                {
                    Endpoint = networkEndpoint
                });
                
                state.EntityManager.DestroyEntity(entity);
            }
        }
    }
}