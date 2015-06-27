namespace MEungblut.Websockets.Owin
{
    using System;
    using System.Net.WebSockets;
    using System.Text;
    using System.Threading.Tasks;

    [WebSocketRoute("/ws")]
    public class PossumWebSocketConnection : WebSocketConnection
    {
        public static event EventHandler<PossumWebSocketConnection> SocketConnected;

        public event EventHandler<PossumWebSocketConnection> SocketDisconnected;
        public event EventHandler<SocketMessage> MessageReceived;

        public PossumWebSocketConnection(OwinWebSocketManager manager)
        {
            this.Id = Guid.NewGuid();
        }

        public override void OnOpen()
        {
            base.OnOpen();

            if (SocketConnected != null)
                SocketConnected(this, this);
        }

        public override void OnClose(WebSocketCloseStatus? closeStatus, string closeStatusDescription)
        {
            if (this.SocketDisconnected != null)
                this.SocketDisconnected(this, this);

            base.OnClose(closeStatus, closeStatusDescription);
        }

        public override Task OnMessageReceived(ArraySegment<byte> message, WebSocketMessageType type)
        {
            if (this.MessageReceived != null)
                this.MessageReceived(this, new SocketMessage(this.Id, new UTF8Encoding().GetString(message.Array)));

            return base.OnMessageReceived(message, type);
        }

        public Guid Id { get; private set; }
    }
}
