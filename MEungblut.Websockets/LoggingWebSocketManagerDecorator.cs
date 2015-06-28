namespace MEungblut.Websockets
{
    using System;

    using MEungblut.Websockets.Owin;

    public class LoggingWebSocketManagerDecorator : IWebSocketManager
    {
        private readonly ILoggingFrameworkAdapter loggingFrameworkAdapter;
        private readonly IWebSocketManager webSocketManager;

        public LoggingWebSocketManagerDecorator(IWebSocketManager decoratedSocketManager, ILoggingFrameworkAdapter loggingFrameworkAdapter)
        {
            this.webSocketManager = decoratedSocketManager;
            this.loggingFrameworkAdapter = loggingFrameworkAdapter;

            this.webSocketManager.SocketConnected += this.WebSocketManagerOnSocketConnected;
            this.webSocketManager.SocketDisconnected += this.WebSocketManagerSocketDisconnected;
            this.webSocketManager.MessageReceived += this.WebSocketManagerMessageReceived;
        }

        void WebSocketManagerMessageReceived(object sender, SocketMessage e)
        {
            this.loggingFrameworkAdapter.LogDebug("Websocket message received", e);

            try
            {
                this.decoratedMessageReceivedEvent(sender, e);
            }
            catch (Exception exception)
            {
                this.loggingFrameworkAdapter.LogException(exception);
            }
        }

        void WebSocketManagerSocketDisconnected(object sender, Guid e)
        {
            this.loggingFrameworkAdapter.LogDebug("Websocket disconnected", e);

            try
            {
                if (this.decoratedSocketDisconnectedEvent != null)
                    this.decoratedSocketDisconnectedEvent(sender, e);
            }
            catch (Exception exception)
            {
                this.loggingFrameworkAdapter.LogException(exception);
            }
        }

        private void WebSocketManagerOnSocketConnected(object sender, Guid guid)
        {
            this.loggingFrameworkAdapter.LogDebug("Websocket connected", guid);

            try
            {
                if (this.decoratedSocketConnectedEvent != null)
                    this.decoratedSocketConnectedEvent(sender, guid);
            }
            catch (Exception exception)
            {
                this.loggingFrameworkAdapter.LogException(exception);
            }
        }

        private EventHandler<Guid> decoratedSocketConnectedEvent;

        public Guid Id
        {
            get
            {
                return this.webSocketManager.Id;
            }
        }

        public event EventHandler<Guid> SocketConnected
        {
            add
            {
                this.decoratedSocketConnectedEvent += value;
            }
            remove
            {
                this.decoratedSocketConnectedEvent -= value;
            }
        }

        private EventHandler<Guid> decoratedSocketDisconnectedEvent;


        public event EventHandler<Guid> SocketDisconnected
        {
            add
            {
                this.decoratedSocketDisconnectedEvent += value;
            }
            remove
            {
                this.decoratedSocketDisconnectedEvent -= value;
            }
        }

        private EventHandler<SocketMessage> decoratedMessageReceivedEvent;

        public event EventHandler<SocketMessage> MessageReceived
        {
            add
            {
                this.decoratedMessageReceivedEvent += value;
            }
            remove
            {
                this.decoratedMessageReceivedEvent -= value;
            }
        }

        public void SendMessage(Guid socketIdentifier, string message)
        {
            this.loggingFrameworkAdapter.LogDebug("Sending message to socket", socketIdentifier, message);

            try
            {
                this.webSocketManager.SendMessage(socketIdentifier, message);
            }
            catch (Exception exception)
            {
                this.loggingFrameworkAdapter.LogException(exception);
                throw;
            }
        }

        public void BroadcastToAllClients(string message)
        {
            this.loggingFrameworkAdapter.LogDebug("Message broadcast", message);
            this.webSocketManager.BroadcastToAllClients(message);
        }

        public void Dispose()
        {
            this.webSocketManager.SocketConnected -= this.WebSocketManagerOnSocketConnected;
            this.webSocketManager.SocketDisconnected -= this.WebSocketManagerSocketDisconnected;
            this.webSocketManager.MessageReceived -= this.WebSocketManagerMessageReceived;
            this.webSocketManager.Dispose();
        }
    }
}