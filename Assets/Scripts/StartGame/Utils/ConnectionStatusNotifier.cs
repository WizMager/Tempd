using System;
using Unity.NetCode;

namespace StartGame.Utils
{
    public static class ConnectionStatusNotifier
    {
        public static Action<ConnectionState.State, NetworkStreamDisconnectReason> OnConnectionStatusChanged;
        
        public static void Publish(ConnectionState.State state, NetworkStreamDisconnectReason reason)
        {
            OnConnectionStatusChanged?.Invoke(state, reason);
        }
    }
}