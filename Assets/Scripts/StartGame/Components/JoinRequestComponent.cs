using Unity.Collections;
using Unity.Entities;

namespace StartGame.Components
{
    public struct JoinRequestComponent : IComponentData
    {
        public FixedString32Bytes EnteredIpAddress;
    }
}