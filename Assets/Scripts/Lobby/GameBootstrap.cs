using Unity.NetCode;

namespace Lobby
{
    public class GameBootstrap : ClientServerBootstrap
    {
        public override bool Initialize(string defaultWorldName)
        {
            AutoConnectPort = 0;
            CreateLocalWorld(defaultWorldName);
            
            return true;
        }
    }
}