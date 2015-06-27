namespace MEungblut.Websockets.UnitTests.Config
{
    using MEungblut.Websockets.Config;

    using NUnit.Framework;

    [TestFixture]
    public class AppConfigSocketConfigurationShould
    {
        [SetUp]
        public void SetAppConfiguration()
        {
            System.Configuration.ConfigurationManager.AppSettings["Websocket:PortNumberToListenOn"] = "1234";
            System.Configuration.ConfigurationManager.AppSettings["Websocket:Server"] = "localhost";
        }

        [Test]
        public void ReadPortNumberFromAppsettings()
        {

            IWebsocketConfiguration config = new AppConfigSocketConfiguration();

            Assert.AreEqual(1234, config.Port);
        }

        [Test]
        public void ReadServerFromAppSettings()
        {
            IWebsocketConfiguration config = new AppConfigSocketConfiguration();

            Assert.AreEqual("localhost", config.Server);
        }

        [Test]
        public void ThrowCouldNotReadPortNumberExceptionWhenPortNumberIsNotParsable()
        {
            System.Configuration.ConfigurationManager.AppSettings["Websocket:PortNumberToListenOn"] = "123d4";

            Assert.Throws<CouldNotReadPortNumberException>(() => new AppConfigSocketConfiguration());
        }
    }
}
