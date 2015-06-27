namespace MEungblut.Websockets.Config
{
    public class HardcodedWebsocketConfiguration : IWebsocketConfiguration
    {
        public int Port
        {
            get
            {
                return 7865;
            }
        }

        public string Server
        {
            get
            {
                return "127.0.0.1";
            }
        }
    }
}
