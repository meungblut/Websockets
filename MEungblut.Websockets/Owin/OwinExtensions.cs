namespace MEungblut.Websockets.Owin
{
    using System.Collections.Generic;

    using global::Owin;

    public static class OwinExtension
    {
        public static void MapWebSocketRoute(this IAppBuilder app, string route, IWebSocketConnectionFactory websocketFactory)
        {
            app.Map(route, config => config.Use<WebSocketConnectionMiddleware>(websocketFactory));
        }

        /// <summary>
        /// Maps a URI pattern to a web socket consumer using a Regex pattern mach on the URI
        /// </summary>
        /// <typeparam name="T">Type of WebSocketHubConnection</typeparam>
        /// <param name="app">Owin app</param>        /// 
        /// <param name="regexPatternMatch">Regex pattern of the URI to match.  Capture groups will be sent to the hub on the Arguments property</param>
        /// <param name="serviceLocator">Service locator to use for getting instances of T</param>
        //public static void MapWebSocketPattern<T>(this IAppBuilder app, string regexPatternMatch, IServiceLocator serviceLocator = null)
        //    where T : WebSocketConnection
        //{
        //    app.Use<WebSocketConnectionMiddleware>(serviceLocator, new Regex(regexPatternMatch, RegexOptions.Compiled | RegexOptions.IgnoreCase));
        //}

        /// <summary>
        /// Maps a static URI route to the web socket connection using the WebSocketRouteAttribute
        /// </summary>
        /// <typeparam name="T">Type of WebSocketHubConnection</typeparam>
        /// <param name="app">Owin App</param>
        /// <param name="serviceLocator">Service locator to use for getting instances of T</param>
        //public static void MapWebSocketRoute(this IAppBuilder app, IServiceLocator serviceLocator = null)
        //{
        //    var routeAttributes = typeof(T).GetCustomAttributes(typeof(WebSocketRouteAttribute), true);

        //    if (routeAttributes.Length == 0)
        //        throw new InvalidOperationException(typeof(T).Name + " type must have attribute of WebSocketRouteAttribute for mapping");

        //    foreach (var routeAttribute in routeAttributes.Cast<WebSocketRouteAttribute>())
        //    {
        //        app.Map(routeAttribute.Route, config => config.Use<WebSocketConnectionMiddleware>(serviceLocator));
        //    }
        //}

        internal static T Get<T>(this IDictionary<string, object> dictionary, string key)
        {
            object item;
            if (dictionary.TryGetValue(key, out item))
            {
                return (T)item;
            }

            return default(T);
        }
    }

    public interface IWebSocketFactory
    {
        IWebSocket GetWebSocket();
    }
}
