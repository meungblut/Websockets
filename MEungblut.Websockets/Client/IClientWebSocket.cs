namespace MEungblut.Websockets.Client
{
    using System;
    using System.Net.WebSockets;
    using System.Threading;
    using System.Threading.Tasks;

    public interface IClientWebSocket
    {
        event EventHandler SocketDisconnected;
        event EventHandler<string> MessageReceived;

        void Connect(string uri);

        WebSocketState State { get; }

        Task CloseAsync(WebSocketCloseStatus webSocketCloseStatus, string statusDescription, CancellationToken cancellationToken);

        Task SendAsync(ArraySegment<byte> bytes, WebSocketMessageType webSocketMessageType, bool endOfMessage, CancellationToken cancellationToken);

        void StartListening();

        void SendMessage(string message);
    }
}
