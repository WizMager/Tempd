using Unity.Collections;
using Unity.NetCode;

namespace StartGame.Components
{
    public struct ChangeNameRpc : IRpcCommand
    {
        public FixedString32Bytes Name;
    }
}