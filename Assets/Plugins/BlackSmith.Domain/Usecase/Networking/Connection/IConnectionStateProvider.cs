using BlackSmith.Domain.Networking.Connection;

namespace BlackSmith.Usecase.Interface.Networking.Connection
{
    public interface IConnectionStateProvider
    {
        ConnectionState GetConnectionState();
    }

    public interface IConnectionStateHandler
    {
        void ChangeConnectionState(ConnectionState nextState);
    }
}