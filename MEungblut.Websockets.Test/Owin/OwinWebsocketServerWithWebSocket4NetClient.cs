namespace MEungblut.Websockets.UnitTests.Owin
{
    using MEungblut.Websockets.Client;

    public class OwinWebsocketServerWithWebSocket4NetClient : OwinWebSocketManagerTestShould
    {
        protected override IClientWebSocket GetClientWebSocket()
        {
            return new WebSocket4NetSocketClient();
        }
    }
}
