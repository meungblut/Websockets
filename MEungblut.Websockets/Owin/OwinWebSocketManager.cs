namespace MEungblut.Websockets.Owin
{
    using System;
    using System.Collections.Concurrent;
    using System.Text;

    public class OwinWebSocketManager : IWebSocketManager
    {
        private readonly ConcurrentDictionary<Guid, OwinWebSocketConnection> _connectedSocketSessions;

        public Guid Id { get; private set; }

        public event EventHandler<Guid> SocketConnected;
        public event EventHandler<Guid> SocketDisconnected;
        public event EventHandler<SocketMessage> MessageReceived;

        public OwinWebSocketManager()
        {
            this.Id = Guid.NewGuid();
            this._connectedSocketSessions = new ConcurrentDictionary<Guid, OwinWebSocketConnection>();
            OwinWebSocketConnection.SocketConnected += this.OwinWebSocketConnection_SocketConnected;
        }

        void OwinWebSocketConnection_SocketConnected(object sender, OwinWebSocketConnection e)
        {
            if (this.SocketConnected != null)
                this.SocketConnected(this, e.Id);

            e.SocketDisconnected += this.e_SocketDisconnected;
            e.MessageReceived += this.e_MessageReceived;

            this._connectedSocketSessions.AddOrUpdate(e.Id, e, (key, value) => e);
        }

        void e_MessageReceived(object sender, SocketMessage e)
        {
            if (this.MessageReceived != null)
                this.MessageReceived(this, e);
        }

        void e_SocketDisconnected(object sender, OwinWebSocketConnection e)
        {
            if (this.SocketDisconnected != null)
                this.SocketDisconnected(this, e.Id);

            e.SocketDisconnected -= this.e_SocketDisconnected;
            e.MessageReceived -= this.e_MessageReceived;
        }
        
        public void SendMessage(Guid socketIdentifier, string message)
        {
            OwinWebSocketConnection connection;
            if(! this._connectedSocketSessions.TryGetValue(socketIdentifier, out connection))
                throw new ArgumentException("I don't know about a socket with id " + socketIdentifier);

            connection.SendText(new UTF8Encoding().GetBytes(message), true);
        }

        public void BroadcastToAllClients(string message)
        {
            foreach (var key in this._connectedSocketSessions.Keys)
            {
                this.SendMessage(key, message);
            }
        }

        public void Dispose()
        {
        }
    }
}
