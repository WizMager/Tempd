using StartGame.Utils;
using Unity.Burst;
using Unity.Entities;
using Unity.NetCode;

namespace StartGame.Systems
{
    [WorldSystemFilter(WorldSystemFilterFlags.ClientSimulation)]
    [UpdateAfter(typeof(NetworkGroupCommandBufferSystem))]
    public partial struct ClientConnectionStatusHandleSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<NetworkStreamDriver>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var events = SystemAPI.GetSingleton<NetworkStreamDriver>().ConnectionEventsForTick;

            foreach (var connectionEvent in events)
            {
                ConnectionStatusNotifier.Publish(connectionEvent.State, connectionEvent.DisconnectReason);
            }
        }
    }
}