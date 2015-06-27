namespace MEungblut.Websockets.Owin
{
    using System.Collections.Generic;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;

    using Microsoft.Owin;

    public class WebSocketConnectionMiddleware : OwinMiddleware 
    {
        private readonly Regex matchPattern;
        private readonly IWebSocketConnectionFactory serviceLocator;

        public WebSocketConnectionMiddleware(OwinMiddleware next, IWebSocketConnectionFactory locator)
            : base(next)
        {
            this.serviceLocator = locator;
        }

        public WebSocketConnectionMiddleware(OwinMiddleware next, IWebSocketConnectionFactory locator, Regex matchPattern)
            : this(next, locator)
        {
            this.matchPattern = matchPattern;
        }

        public override Task Invoke(IOwinContext context)
        {
            var matches = new Dictionary<string, string>();

            if (this.matchPattern != null)
            {
                var match = this.matchPattern.Match(context.Request.Path.Value);
                if (!match.Success)
                    return this.Next.Invoke(context);

                for (var i = 1; i <= match.Groups.Count; i++)
                {
                    var name = this.matchPattern.GroupNameFromNumber(i);
                    var value = match.Groups[i];
                    matches.Add(name, value.Value);
                }
            }

            var socketConnection = this.serviceLocator.GetWebSocketConnection();

            socketConnection.AcceptSocket(context, matches);

            return Task.FromResult<object>(null);
        }
    }

    public interface IWebSocketConnectionFactory
    {
        WebSocketConnection GetWebSocketConnection();
    }
}
