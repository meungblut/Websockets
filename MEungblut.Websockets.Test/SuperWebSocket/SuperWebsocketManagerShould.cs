namespace MEungblut.Websockets.UnitTests.SuperWebSocket
{
    using MEungblut.Websockets.SuperWebSocket;

    using NUnit.Framework;

    [TestFixture]
    public abstract class SuperWebsocketManagerShould : WebSocketManagerTestBase
    {

        [TestFixtureSetUp]
        public void TestFixtureSetup()
        {
            IWebsocketConfiguration config = new WebSocketConfigDouble(1236);
            this.Manager = new SuperWebsocketManager(config);
        }

        [TestFixtureTearDown]
        public void TearDown()
        {
            this.Manager.Dispose();
        }

        protected override IWebSocketManager Manager { get; set; }

        protected override string WebsocketUrl
        {
            get { return "ws://127.0.0.1:1236/"; }
        }
    }
}
