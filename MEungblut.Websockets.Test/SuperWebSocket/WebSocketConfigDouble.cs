namespace MEungblut.Websockets.UnitTests.SuperWebSocket
{
    class WebSocketConfigDouble : IWebsocketConfiguration
    {
        public WebSocketConfigDouble(int port)
        {
            this.Port = port;
        }

        public int Port { get; private set; }

        public string Server { get; set; }
    }
}
