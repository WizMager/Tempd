using StartGame.Components;
using StartGame.Utils;
using Unity.Entities;
using Unity.NetCode;
using Unity.Networking.Transport;
using UnityEngine;

namespace StartGame.Systems
{
    [WorldSystemFilter(WorldSystemFilterFlags.LocalSimulation)]
    public partial struct HostRequestHandleSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<EndSimulationEntityCommandBufferSystem.Singleton>();
            state.RequireForUpdate<HostRequestComponent>();
        }

        public void OnUpdate(ref SystemState state)
        {
            var ecb = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>().CreateCommandBuffer(state.WorldUnmanaged);
            
            foreach (var (_, entity) in SystemAPI.Query<HostRequestComponent>().WithEntityAccess())
            {
                var serverWorld = ClientServerBootstrap.CreateServerWorld("ServerWorld");
                var clientWorld = ClientServerBootstrap.CreateClientWorld("ClientWorld");
                
                NetworkSceneLoader.LoadNetworkBootSubScene(serverWorld);
                NetworkSceneLoader.LoadNetworkBootSubScene(clientWorld);
                
                var listenEndpoint = NetworkEndpoint.Parse("0.0.0.0", 7979);
                var listenRequest = serverWorld.EntityManager.CreateEntity();
                serverWorld.EntityManager.AddComponentData(listenRequest, new NetworkStreamRequestListen
                {
                    Endpoint = listenEndpoint
                });
                
                var connectEndpoint = NetworkEndpoint.Parse("127.0.0.1", 7979);
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