using StartGame.Components;
using StartGame.Utils;
using Unity.Entities;
using Unity.NetCode;

namespace StartGame.Systems
{
    [WorldSystemFilter(WorldSystemFilterFlags.ClientSimulation)]
    [UpdateAfter(typeof(NetworkGroupCommandBufferSystem))]
    public partial struct ClientConnectionStatusHandleSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<NetworkStreamDriver>();
        }

        public void OnUpdate(ref SystemState state)
        {
            var events = SystemAPI.GetSingleton<NetworkStreamDriver>().ConnectionEventsForTick;

            foreach (var connectionEvent in events)
            {
                ConnectionStatusNotifier.Publish(connectionEvent.State, connectionEvent.DisconnectReason);

                if (connectionEvent.State == ConnectionState.State.Disconnected)
                {
                    RequestLeaveFromLocalWorld();
                }
            }
        }

        private static void RequestLeaveFromLocalWorld()
        {
            foreach (var world in World.All)
            {
                if (world is not { IsCreated: true, Name: "LocalWorld" })
                {
                    continue;
                }

                var entityManager = world.EntityManager;
                using var leaveQuery = entityManager.CreateEntityQuery(typeof(LeaveRequestComponent));
                if (!leaveQuery.IsEmpty)
                {
                    return;
                }

                var leaveEntity = entityManager.CreateEntity();
                entityManager.AddComponentData(leaveEntity, new LeaveRequestComponent
                {
                    DisconnectInProgress = true
                });
                return;
            }
        }
    }
}