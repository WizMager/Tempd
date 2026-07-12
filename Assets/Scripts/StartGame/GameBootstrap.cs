using Unity.NetCode;

namespace StartGame
{
    public class GameBootstrap : ClientServerBootstrap
    {
        public override bool Initialize(string defaultWorldName)
        {
            AutoConnectPort = 0;
            CreateLocalWorld("LocalWorld");
            
            return true;
        }
    }
}