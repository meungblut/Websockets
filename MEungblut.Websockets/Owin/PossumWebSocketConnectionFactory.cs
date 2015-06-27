namespace MEungblut.Websockets.Owin
{
    public class PossumWebSocketConnectionFactory : IWebSocketConnectionFactory
    {
        private OwinWebSocketManager _manager;

        public PossumWebSocketConnectionFactory(OwinWebSocketManager manager)
        {
            this._manager = manager;
        }

        public WebSocketConnection GetWebSocketConnection()
        {
            return new PossumWebSocketConnection(this._manager);
        }
    }
}
