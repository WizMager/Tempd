using Unity.Collections;
using Unity.Entities;
using Unity.NetCode;

namespace StartGame.Components
{
    public struct LobbyPlayerComponent : IComponentData
    {
        public int NetworkId;
        [GhostField] public FixedString32Bytes Name;
        [GhostField] public bool IsReady;
    }
}