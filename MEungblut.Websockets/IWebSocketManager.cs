namespace MEungblut.Websockets
{
    using System;

    using MEungblut.Websockets.Owin;

    public interface IWebSocketManager : IDisposable
    {
        Guid Id { get; }
        event EventHandler<Guid> SocketConnected;
        event EventHandler<Guid> SocketDisconnected;
        event EventHandler<SocketMessage> MessageReceived;

        void SendMessage(Guid socketIdentifier, string message);
        void BroadcastToAllClients(string message);
    }
} 