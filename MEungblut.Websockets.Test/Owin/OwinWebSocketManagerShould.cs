namespace MEungblut.Websockets.UnitTests.Owin
{
    using System;

    using global::Owin;

    using MEungblut.Websockets.Owin;

    using Microsoft.Owin.Hosting;

    using NUnit.Framework;

    [TestFixture]
    public abstract class OwinWebSocketManagerTestShould : WebSocketManagerTestBase
    {
        private IDisposable _app;
        private IWebSocketConnectionFactory _connectionFactory;

        void Configure(IAppBuilder builder)
        {
            this.Manager = new OwinWebSocketManager();
            this._connectionFactory = new PossumWebSocketConnectionFactory((OwinWebSocketManager)this.Manager);
            builder.MapWebSocketRoute("/ws", this._connectionFactory);
        }

        [TestFixtureSetUp]
        public void TestFixtureSetup()
        {
            this.GuardAgainstPlatformProblems();
            string uri = "http://+:4567";
            this._app = WebApp.Start(uri, this.Configure);
        }

        private void GuardAgainstPlatformProblems()
        {
            var operatingSystem = System.Environment.OSVersion;

            bool isRunningOnMono = (Type.GetType("Mono.Runtime") != null);

            bool isWindowsButLessThan8 = operatingSystem.Version.Major == 6 && operatingSystem.Version.Minor <= 1;

            if (isRunningOnMono || isWindowsButLessThan8)
                Assert.Ignore("Can't use owin websockets or dot net websockets on anything other than windows 8");
        }

        [TestFixtureTearDown]
        public void TestTearDown()
        {
            this._app.Dispose();
        }

        protected override IWebSocketManager Manager { get; set; }

        protected override string WebsocketUrl
        {
            get { return "ws://127.0.0.1:4567/ws"; }
        }
    }
}
