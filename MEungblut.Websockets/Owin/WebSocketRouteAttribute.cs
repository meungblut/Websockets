namespace MEungblut.Websockets.Owin
{
    using System;

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class WebSocketRouteAttribute : Attribute
    {
        public string Route { get; set; }

        public WebSocketRouteAttribute(string route)
        {
            this.Route = route;
        }
    }
}
