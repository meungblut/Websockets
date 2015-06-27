namespace MEungblut.Websockets.Client
{
    using System;
    using System.Net.WebSockets;
    using System.Threading;
    using System.Threading.Tasks;

    using WebSocket4Net;

    using WebSocketState = System.Net.WebSockets.WebSocketState;

    public class WebSocket4NetSocketClient : IClientWebSocket
    {
        private WebSocket4Net.WebSocket socket;

        void SocketClosed(object sender, EventArgs e)
        {
            if (this.SocketDisconnected != null) this.SocketDisconnected(this, new EventArgs());
        }

        void SocketMessageReceived(object sender, MessageReceivedEventArgs e)
        {
            if (this.MessageReceived != null) this.MessageReceived(this, e.Message);
        }

        public event EventHandler SocketDisconnected;

        public event EventHandler<string> MessageReceived;

        public void Connect(string uri)
        {
            this.socket = new WebSocket4Net.WebSocket(uri);
            this.socket.MessageReceived += this.SocketMessageReceived;
            this.socket.Closed += this.SocketClosed;
            this.socket.Open();
        }

        public WebSocketState State
        {
            get
            {
                return this.GetState();
            }
        }

        private WebSocketState GetState()
        {
            switch (this.socket.State)
            {
                case WebSocket4Net.WebSocketState.Closed:
                    return WebSocketState.Closed;
                case WebSocket4Net.WebSocketState.Closing:
                    return WebSocketState.CloseReceived;
                case WebSocket4Net.WebSocketState.Connecting:
                    return WebSocketState.Connecting;
                case WebSocket4Net.WebSocketState.None:
                    return WebSocketState.None;
                case WebSocket4Net.WebSocketState.Open:
                    return WebSocketState.Open;
                default:
                    throw new ArgumentException("Unexpected socket state " + this.socket.State);
            }
        }

        public Task CloseAsync(
            WebSocketCloseStatus webSocketCloseStatus,
            string statusDescription,
            CancellationToken cancellationToken)
        {
            return Task.Run(() => this.socket.Close(), cancellationToken);
        }

        public Task SendAsync(
            ArraySegment<byte> bytes,
            WebSocketMessageType webSocketMessageType,
            bool endOfMessage,
            CancellationToken cancellationToken)
        {
            return Task.Run(() => this.socket.Send(new[] { bytes }), cancellationToken);
        }

        public void StartListening()
        {
            //nothing - framework does it for you...
        }

        public void SendMessage(string message)
        {
            this.socket.Send(message);
        }
    }
}
