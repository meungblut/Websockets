namespace MEungblut.Websockets.UnitTests.SuperWebSocket
{
    using MEungblut.Websockets.Client;

    public class SuperWebsocketServerWithDotNetClient : SuperWebsocketManagerShould
    {
        protected override IClientWebSocket GetClientWebSocket()
        {
            return new WebSocket4NetSocketClient();
        }
    }
}
