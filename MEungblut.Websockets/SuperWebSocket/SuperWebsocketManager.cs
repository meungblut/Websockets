namespace MEungblut.Websockets.SuperWebSocket
{
    using System;
    using System.Collections.Concurrent;

    using global::SuperWebSocket;

    using MEungblut.Websockets.Owin;

    public class SuperWebsocketManager : IWebSocketManager
    {
        readonly WebSocketServer _appServer = new WebSocketServer();

        public Guid Id { get; private set; }

        public event EventHandler<Guid> SocketConnected;
        public event EventHandler<Guid> SocketDisconnected;
        public event EventHandler<SocketMessage> MessageReceived;
        private readonly ConcurrentDictionary<string, WebSocketSession> _connectedSocketSessions;

        private IWebsocketConfiguration websocketConfiguration;

        public SuperWebsocketManager(IWebsocketConfiguration configuration)
        {
            this.websocketConfiguration = configuration;
            this.Id = Guid.NewGuid();

            this._connectedSocketSessions = new ConcurrentDictionary<string, WebSocketSession>();

            this._appServer.Setup(configuration.Port);

            if (!this._appServer.Start())
                throw new ArgumentException("Couldn't start service");

            this._appServer.NewMessageReceived += this.AppServerNewMessageReceived;
            this._appServer.NewSessionConnected += this.AppServerNewSessionConnected;
            this._appServer.SessionClosed += this.AppServerSessionClosed;
        }

        void AppServerSessionClosed(WebSocketSession session, SuperSocket.SocketBase.CloseReason value)
        {
            if (this.SocketDisconnected != null)
                this.SocketDisconnected(this, Guid.Parse(session.SessionID));
        }

        void AppServerNewSessionConnected(WebSocketSession session)
        {
            this._connectedSocketSessions.AddOrUpdate(session.SessionID, session, (key, value) => session);

            if (this.SocketConnected != null)
                this.SocketConnected(this, Guid.Parse(session.SessionID));
        }

        void AppServerNewMessageReceived(WebSocketSession session, string value)
        {
            if (this.MessageReceived != null)
                this.MessageReceived(this, new SocketMessage(Guid.Parse(session.SessionID), value));
        }

        public void SendMessage(Guid socketIdentifier, string message)
        {
            WebSocketSession connection;
            if (!this._connectedSocketSessions.TryGetValue(socketIdentifier.ToString(), out connection))
                throw new ArgumentException("I don't know about a socket with id " + socketIdentifier);

            connection.Send(message);
        }

        public void BroadcastToAllClients(string message)
        {
            foreach (var key in this._connectedSocketSessions.Keys)
            {
                this.SendMessage(Guid.Parse(key), message);
            }
        }

        public void Dispose()
        {
            this._appServer.Dispose();
        }
    }
}
