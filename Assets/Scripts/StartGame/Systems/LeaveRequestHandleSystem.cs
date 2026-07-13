using StartGame.Components;
using Unity.Collections;
using Unity.Entities;
using Unity.NetCode;

namespace StartGame.Systems
{
    [WorldSystemFilter(WorldSystemFilterFlags.LocalSimulation)]
    public partial struct LeaveRequestHandleSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<EndSimulationEntityCommandBufferSystem.Singleton>();
            state.RequireForUpdate<LeaveRequestComponent>();
        }

        public void OnUpdate(ref SystemState state)
        {
            var ecb = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>().CreateCommandBuffer(state.WorldUnmanaged);

            foreach (var (request, entity) in SystemAPI.Query<RefRW<LeaveRequestComponent>>().WithEntityAccess())
            {
                if (!request.ValueRO.DisconnectInProgress)
                {
                    SendDisconnects();
                    request.ValueRW.DisconnectInProgress = true;
                }

                if (HasNetworkWorlds())
                {
                    if (ClientWorldHasConnections())
                    {
                        continue;
                    }

                    DisposeNetworkWorlds();
                }

                RestoreLocalWorldAsDefault();
                ecb.DestroyEntity(entity);
            }
        }

        private static void SendDisconnects()
        {
            var clientWorld = ClientServerBootstrap.ClientWorld;
            if (clientWorld is { IsCreated: true })
            {
                RequestDisconnect(clientWorld.EntityManager);
            }

            var serverWorld = ClientServerBootstrap.ServerWorld;
            if (serverWorld is { IsCreated: true })
            {
                var serverEntityManager = serverWorld.EntityManager;
                using var connectionQuery = serverEntityManager.CreateEntityQuery(ComponentType.ReadOnly<NetworkStreamConnection>(), ComponentType.ReadOnly<NetworkId>());
                var connectionEntities = connectionQuery.ToEntityArray(Allocator.Temp);

                foreach (var entity in connectionEntities)
                {
                    if (!serverEntityManager.HasComponent<NetworkStreamRequestDisconnect>(entity))
                    {
                        serverEntityManager.AddComponent<NetworkStreamRequestDisconnect>(entity);
                    }
                }

                connectionEntities.Dispose();
            }
        }

        private static void RequestDisconnect(EntityManager entityManager)
        {
            using var connectionQuery = entityManager.CreateEntityQuery(typeof(NetworkStreamConnection));
            var connectionEntities = connectionQuery.ToEntityArray(Allocator.Temp);

            foreach (var entity in connectionEntities)
            {
                if (!entityManager.HasComponent<NetworkStreamRequestDisconnect>(entity))
                {
                    entityManager.AddComponent<NetworkStreamRequestDisconnect>(entity);
                }
            }

            connectionEntities.Dispose();
        }

        private static bool HasNetworkWorlds()
        {
            var clientWorld = ClientServerBootstrap.ClientWorld;
            if (clientWorld is { IsCreated: true })
            {
                return true;
            }

            var serverWorld = ClientServerBootstrap.ServerWorld;
            return serverWorld is { IsCreated: true };
        }

        private static bool ClientWorldHasConnections()
        {
            var clientWorld = ClientServerBootstrap.ClientWorld;
            if (clientWorld is not { IsCreated: true })
            {
                return false;
            }

            using var connectionQuery = clientWorld.EntityManager.CreateEntityQuery(typeof(NetworkStreamConnection));
            return connectionQuery.CalculateEntityCount() > 0;
        }

        private static void DisposeNetworkWorlds()
        {
            var clientWorld = ClientServerBootstrap.ClientWorld;
            if (clientWorld is { IsCreated: true })
            {
                clientWorld.Dispose();
            }

            var serverWorld = ClientServerBootstrap.ServerWorld;
            if (serverWorld?.IsCreated == true)
            {
                serverWorld.Dispose();
            }
        }

        private static void RestoreLocalWorldAsDefault()
        {
            foreach (var world in World.All)
            {
                if (world is not { IsCreated: true, Name: "LocalWorld" })
                {
                    continue;
                }

                World.DefaultGameObjectInjectionWorld = world;
                return;
            }
        }
    }
}