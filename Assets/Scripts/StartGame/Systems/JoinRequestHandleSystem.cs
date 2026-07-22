using StartGame.Components;
using StartGame.Utils;
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
            var ecb = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>().CreateCommandBuffer(state.WorldUnmanaged);
            
            foreach (var (requestComponent, entity) in SystemAPI.Query<RefRO<JoinRequestComponent>>().WithEntityAccess())
            {
                var clientWorld = ClientServerBootstrap.CreateClientWorld("ClientWorld");
                NetworkSceneLoader.LoadNetworkBootSubScene(clientWorld);

                var connectEndpoint = NetworkEndpoint.Parse(requestComponent.ValueRO.EnteredIpAddress.ToString(), 7979);
                var requestConnectEntity = clientWorld.EntityManager.CreateEntity();
                clientWorld.EntityManager.AddComponentData(requestConnectEntity, new NetworkStreamRequestConnect
                {
                    Endpoint = connectEndpoint
                });
                
                ecb.DestroyEntity(entity);
            }
        }
    }
}