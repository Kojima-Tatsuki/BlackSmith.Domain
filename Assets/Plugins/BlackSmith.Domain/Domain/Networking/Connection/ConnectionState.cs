namespace BlackSmith.Domain.Networking.Connection
{
    public class ConnectionState
    {
        
    }

    public enum ConnectionStateType
    {
        Offline,
        ClientConnecting,
        ClientConnected,
        ClientReconnecting,
        StartingHost,
        Hosting,
    }
}