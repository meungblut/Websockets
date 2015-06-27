namespace MEungblut.Websockets.Config
{
    using System;
    using System.Reflection;

    public class AppConfigSocketConfiguration : IWebsocketConfiguration
    {
        public AppConfigSocketConfiguration()
        {
            try
            {
                this.Port =
                    int.Parse(System.Configuration.ConfigurationManager.AppSettings["Websocket:PortNumberToListenOn"]);
            }
            catch (Exception exception)
            {
                throw new CouldNotReadPortNumberException("Executing in " + Assembly.GetExecutingAssembly().Location, exception);
            }

            this.Server = System.Configuration.ConfigurationManager.AppSettings["Websocket:Server"];
        }

        public int Port { get; private set; }

        public string Server { get; private set; }
    }
}
