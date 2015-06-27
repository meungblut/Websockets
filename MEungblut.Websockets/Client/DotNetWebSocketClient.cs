namespace MEungblut.Websockets.Client
{
    using System;
    using System.Net.WebSockets;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;

    public class DotNetWebSocketClient : IClientWebSocket
    {
        private const int ReceiveChunkSize = 1024;
        private const int SendChunkSize = 1024;
        private readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
        private readonly CancellationToken _cancellationToken;
        private readonly ClientWebSocket socket;

        public event EventHandler SocketDisconnected;
        public event EventHandler<string> MessageReceived;

        public DotNetWebSocketClient()
        {
            this.socket = new ClientWebSocket();
        }

        public void Connect(string uri)
        {
            this.socket.ConnectAsync(new Uri(uri), CancellationToken.None).Wait();
        }

        public WebSocketState State
        {
            get
            {
                return this.socket.State;
            }
        }

        public Task CloseAsync(WebSocketCloseStatus webSocketCloseStatus, string statusDescription, CancellationToken cancellationToken)
        {
            return this.socket.CloseAsync(webSocketCloseStatus, statusDescription, cancellationToken);
        }

        public Task SendAsync(
            ArraySegment<byte> bytes,
            WebSocketMessageType webSocketMessageType,
            bool endOfMessage,
            CancellationToken cancellationToken)
        {
            return this.socket.SendAsync(bytes, webSocketMessageType, endOfMessage, cancellationToken);
        }

        public void StartListening()
        {
            Task.Factory.StartNew(this.StartListen);
        }

        public void SendMessage(string message)
        {
            this.SendMessageAsync(message);
        }

        private async void SendMessageAsync(string message)
        {
            if (this.socket.State != WebSocketState.Open)
            {
                throw new Exception("Connection is not open.");
            }

            var messageBuffer = Encoding.UTF8.GetBytes(message);
            var messagesCount = (int)Math.Ceiling((double)messageBuffer.Length / SendChunkSize);

            for (var i = 0; i < messagesCount; i++)
            {
                var offset = (SendChunkSize * i);
                var count = SendChunkSize;
                var lastMessage = ((i + 1) == messagesCount);

                if ((count * (i + 1)) > messageBuffer.Length)
                {
                    count = messageBuffer.Length - offset;
                }

                await this.socket.SendAsync(new ArraySegment<byte>(messageBuffer, offset, count), WebSocketMessageType.Text, lastMessage, this._cancellationToken);
            }
        } 

        private async void StartListen()
        {
            var buffer = new byte[ReceiveChunkSize];

            try
            {
                while (this.socket.State == WebSocketState.Open)
                {
                    var stringResult = new StringBuilder();

                    WebSocketReceiveResult result;
                    do
                    {
                        result = await this.socket.ReceiveAsync(new ArraySegment<byte>(buffer), this._cancellationToken);

                        if (result.MessageType == WebSocketMessageType.Close)
                        {
                            await
                            this.socket.CloseAsync(WebSocketCloseStatus.NormalClosure, string.Empty, CancellationToken.None);
                            if (this.SocketDisconnected != null)
                                this.SocketDisconnected(this, new EventArgs());
                        }
                        else
                        {
                            var str = Encoding.UTF8.GetString(buffer, 0, result.Count);
                            stringResult.Append(str);
                        }

                    } while (!result.EndOfMessage);

                    if (this.MessageReceived != null)
                        this.MessageReceived(this, stringResult.ToString());
                }
            }
            catch (Exception)
            {
                if (this.SocketDisconnected != null)
                    this.SocketDisconnected(this, new EventArgs());
            }
            finally
            {
                this.socket.Dispose();
            }
        }
    }
}
