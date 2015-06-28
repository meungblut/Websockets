namespace MEungblut.Websockets.Owin
{
    public class OwinWebSocketConnectionFactory : IWebSocketConnectionFactory
    {
        private OwinWebSocketManager _manager;

        public OwinWebSocketConnectionFactory(OwinWebSocketManager manager)
        {
            this._manager = manager;
        }

        public WebSocketConnection GetWebSocketConnection()
        {
            return new OwinWebSocketConnection(this._manager);
        }
    }
}
