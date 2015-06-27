namespace MEungblut.Websockets.Owin
{
    using System;
    using System.Net.WebSockets;
    using System.Threading;

    internal class SendContext
    {
        public ArraySegment<byte> Buffer;
        public bool EndOfMessage;
        public WebSocketMessageType Type;
        public CancellationToken CancelToken;

        public SendContext(ArraySegment<byte> buffer, bool endOfMessage, WebSocketMessageType type, CancellationToken cancelToken)
        {
            this.Buffer = buffer;
            this.EndOfMessage = endOfMessage;
            this.Type = type;
            this.CancelToken = cancelToken;
        }
    }
}