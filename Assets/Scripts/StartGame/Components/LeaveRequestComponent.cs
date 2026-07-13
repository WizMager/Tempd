using Unity.Entities;

namespace StartGame.Components
{
    public struct LeaveRequestComponent : IComponentData
    {
        public bool DisconnectInProgress;
    }
}