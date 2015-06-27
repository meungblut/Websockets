namespace MEungblut.Websockets.UnitTests
{
    using System;
    using System.Diagnostics;
    using System.Net.WebSockets;
    using System.Threading;

    using MEungblut.Websockets.Client;
    using MEungblut.Websockets.Owin;

    using NUnit.Framework;

    public abstract class WebSocketManagerTestBase
    {
        protected abstract IWebSocketManager Manager { get; set; }
        protected abstract string WebsocketUrl { get; }

        protected abstract IClientWebSocket GetClientWebSocket();

        private ManualResetEvent _socketConnected;
        private ManualResetEvent _socketDisconnected;
        private ManualResetEvent _socketSentMessage;
        private ManualResetEvent _clientSocketReceivedMessage;

        private SocketMessage _lastSocketMessage;
        private Guid _lastConnectedGuid;

        [SetUp]
        public void Setup()
        {
            this.operatingSystem = System.Environment.OSVersion;

            this._socketConnected = new ManualResetEvent(false);
            this._socketDisconnected = new ManualResetEvent(false);
            this._socketSentMessage = new ManualResetEvent(false);
            this._clientSocketReceivedMessage = new ManualResetEvent(false);

            this.Manager.SocketConnected += this.ManagerSocketConnected;
            this.Manager.SocketDisconnected += this.ManagerSocketDisconnected;
            this.Manager.MessageReceived += this.ManagerMessageReceived;
        }

        void ManagerMessageReceived(object sender, SocketMessage e)
        {
            this._lastSocketMessage = e;
            this._socketSentMessage.Set();
        }

        void ManagerSocketDisconnected(object sender, Guid e)
        {
            this._socketDisconnected.Set();
        }

        void ManagerSocketConnected(object sender, Guid e)
        {
            this._lastConnectedGuid = e;
            this._socketConnected.Set();
        }

        [Test]
        public void RaiseConnectedEventWhenASocketConnects()
        {
            this.GuardAgainstPlatformProblems();
            this.GetConnectedWebSocket();
            bool connected = this._socketConnected.WaitOne(TimeSpan.FromSeconds(1));
            Assert.IsTrue(connected);
        }

        [Test]
        public async void RaiseDisconnectedWhenASocketCloses()
        {
            this.GuardAgainstPlatformProblems();

            var socket = this.GetConnectedWebSocket();
            this._socketConnected.WaitOne(TimeSpan.FromSeconds(1));

            await socket.CloseAsync(WebSocketCloseStatus.Empty, string.Empty, CancellationToken.None);
            bool disconnected = this._socketDisconnected.WaitOne(TimeSpan.FromSeconds(1));

            Assert.IsTrue(disconnected);
        }

        [Test]
        public void RaiseMessageReceivedWhenSocketSendsMessage()
        {
            var clientSocket = this.GetConnectedWebSocket();
            this._socketConnected.WaitOne(TimeSpan.FromSeconds(1));

            clientSocket.SendMessage("Hello");
            bool messageReceived = this._socketSentMessage.WaitOne(TimeSpan.FromSeconds(1));

            Assert.IsTrue(messageReceived);
            Assert.AreEqual("Hello", this._lastSocketMessage.Message.Substring(0, 5));
        }

        [Test]
        public void SendMessageToConnectedSocket()
        {
            this.GuardAgainstPlatformProblems();

            var clientSocket = this.GetConnectedWebSocket();
            this._socketConnected.WaitOne(TimeSpan.FromSeconds(1));

            clientSocket.MessageReceived += this.clientSocket_MessageReceived;

            this.Manager.SendMessage(this._lastConnectedGuid, "Hello");
            bool clientReceivedAMessage = this._clientSocketReceivedMessage.WaitOne(TimeSpan.FromSeconds(1));

            Assert.IsTrue(clientReceivedAMessage);
            Assert.AreEqual("Hello", this._lastMessageReceivedByClient);
        }

        [Test]
        public void BroadcastMessageToConnectedClientSockets()
        {
            this.GuardAgainstPlatformProblems();

            var clientSocket1 = this.GetConnectedWebSocket();
            this._socketConnected.WaitOne(TimeSpan.FromSeconds(1));
            clientSocket1.MessageReceived += this.clientSocket1_MessageReceived;

            this._socketConnected.Reset();

            var clientSocket2 = this.GetConnectedWebSocket();
            clientSocket2.MessageReceived += this.ClientSocket2OnMessageReceived;
            this._socketConnected.WaitOne(TimeSpan.FromSeconds(1));

            this.Manager.BroadcastToAllClients("Hi");

            WaitHandle.WaitAll(new WaitHandle[] { this._client2ReceivedMessage, this._client1ReceivedMessage });

            Assert.AreEqual("Hi", this._client1Message);
            Assert.AreEqual("Hi", this._client2Message);
        }

        readonly ManualResetEvent _client2ReceivedMessage = new ManualResetEvent(false);
        private string _client2Message;
        private void ClientSocket2OnMessageReceived(object sender, string s)
        {
            this._client2ReceivedMessage.Set();
            this._client2Message = s;
        }


        readonly ManualResetEvent _client1ReceivedMessage = new ManualResetEvent(false);
        private string _client1Message;
        void clientSocket1_MessageReceived(object sender, string e)
        {
            this._client1ReceivedMessage.Set();
            this._client1Message = e;
        }

        private string _lastMessageReceivedByClient;

        private OperatingSystem operatingSystem;

        void clientSocket_MessageReceived(object sender, string e)
        {
            this._lastMessageReceivedByClient = e;
            this._clientSocketReceivedMessage.Set();
        }

        protected IClientWebSocket GetConnectedWebSocket()
        {
            IClientWebSocket socket = this.GetClientWebSocket();
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            socket.Connect(this.WebsocketUrl);
            while (socket.State != WebSocketState.Open)
            {
                Thread.Sleep(10);
            }

            stopwatch.Stop();

            Console.WriteLine("Connection took " + stopwatch.ElapsedMilliseconds + " milliseconds");

            Assert.AreEqual(WebSocketState.Open, socket.State);
            return socket;
        }

        private void GuardAgainstPlatformProblems()
        {
            bool isRunningOnMono = (Type.GetType("Mono.Runtime") != null);

            bool isWindowsButLessThan8 = this.operatingSystem.Version.Major == 6 && this.operatingSystem.Version.Minor <= 1;

            if (this.Manager is OwinWebSocketManager)
                if (isRunningOnMono || isWindowsButLessThan8)
                    Assert.Inconclusive("Can't use owin websockets or dot net websockets on anything other than windows 8");
        }
    }
}
