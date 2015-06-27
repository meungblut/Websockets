namespace MEungblut.Websockets.Owin
{
    using System;
    using System.Collections.Generic;
    using System.Net.WebSockets;
    using System.Threading;
    using System.Threading.Tasks;

    using WebSocketSendAsync = System.Func
        <System.ArraySegment<byte> /* data */,
            int /* messageType */,
            bool /* endOfMessage */,
            System.Threading.CancellationToken /* cancel */,
            System.Threading.Tasks.Task
        >;

    using WebSocketReceiveAsync = System.Func
        <System.ArraySegment<byte> /* data */,
            System.Threading.CancellationToken /* cancel */, System.Threading.Tasks.Task<System.Tuple<int, bool, int>>>;

    using WebSocketCloseAsync = System.Func
        <
            int /* closeStatus */,
            string /* closeDescription */,
            System.Threading.CancellationToken /* cancel */,
            System.Threading.Tasks.Task
        >;

    internal class OwinWebSocket : IWebSocket
    {
        internal const int TEXT_OP = 0x1;
        internal const int BINARY_OP = 0x2;
        internal const int CLOSE_OP = 0x8;

        private readonly WebSocketSendAsync mSendAsync;
        private readonly WebSocketReceiveAsync mReceiveAsync;
        private readonly WebSocketCloseAsync mCloseAsync;
        private readonly TaskQueue mSendQueue;

        public TaskQueue SendQueue { get { return this.mSendQueue; } }

        public WebSocketCloseStatus? CloseStatus { get { return null; } }

        public string CloseStatusDescription { get { return null; } }

        public OwinWebSocket(IDictionary<string, object> owinEnvironment)
        {
            this.mSendAsync = (WebSocketSendAsync)owinEnvironment["websocket.SendAsync"];
            this.mReceiveAsync = (WebSocketReceiveAsync)owinEnvironment["websocket.ReceiveAsync"];
            this.mCloseAsync = (WebSocketCloseAsync)owinEnvironment["websocket.CloseAsync"];
            this.mSendQueue = new TaskQueue();
        }

        public Task SendText(ArraySegment<byte> data, bool endOfMessage, CancellationToken cancelToken)
        {
            return this.Send(data, WebSocketMessageType.Text, endOfMessage, cancelToken);
        }

        public Task SendBinary(ArraySegment<byte> data, bool endOfMessage, CancellationToken cancelToken)
        {
            return this.Send(data, WebSocketMessageType.Binary, endOfMessage, cancelToken);
        }

        public Task Send(ArraySegment<byte> data, WebSocketMessageType messageType, bool endOfMessage, CancellationToken cancelToken)
        {
            var sendContext = new SendContext(data, endOfMessage, messageType, cancelToken);

            return this.mSendQueue.Enqueue(
                async s =>
                {
                    await this.mSendAsync(s.Buffer, MessageTypeEnumToOpCode(s.Type), s.EndOfMessage, s.CancelToken);
                },
                sendContext);
        }

        public Task Close(WebSocketCloseStatus closeStatus, string closeDescription, CancellationToken cancelToken)
        {
            return this.mCloseAsync((int)closeStatus, closeDescription, cancelToken);
        }

        public async Task<Tuple<ArraySegment<byte>, WebSocketMessageType>> ReceiveMessage(byte[] buffer, CancellationToken cancelToken)
        {
            var count = 0;
            Tuple<int, bool, int> result;
            do
            {
                var segment = new ArraySegment<byte>(buffer, count, buffer.Length - count);
                result = await this.mReceiveAsync(segment, cancelToken);

                count += result.Item3;
            }
            while (!result.Item2);

            return new Tuple<ArraySegment<byte>, WebSocketMessageType>(new ArraySegment<byte>(buffer, 0, count), MessageTypeOpCodeToEnum(result.Item1));
        }

        private static WebSocketMessageType MessageTypeOpCodeToEnum(int messageType)
        {
            switch (messageType)
            {
                case TEXT_OP:
                    return WebSocketMessageType.Text;
                case BINARY_OP:
                    return WebSocketMessageType.Binary;
                case CLOSE_OP:
                    return WebSocketMessageType.Close;
                default:
                    throw new ArgumentOutOfRangeException("messageType", messageType, String.Empty);
            }
        }

        private static int MessageTypeEnumToOpCode(WebSocketMessageType webSocketMessageType)
        {
            switch (webSocketMessageType)
            {
                case WebSocketMessageType.Text:
                    return TEXT_OP;
                case WebSocketMessageType.Binary:
                    return BINARY_OP;
                case WebSocketMessageType.Close:
                    return CLOSE_OP;
                default:
                    throw new ArgumentOutOfRangeException("webSocketMessageType", webSocketMessageType, String.Empty);
            }
        }
    }
}
