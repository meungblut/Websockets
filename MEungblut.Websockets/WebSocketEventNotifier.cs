namespace MEungblut.Websockets
{
    using System;

    using MEungblut.Websockets.ExternalPublishing;
    using MEungblut.Websockets.ExternalPublishing.Protocol;
    using MEungblut.Websockets.Owin;

    public class WebSocketEventNotifier : IEventSubscriberNotifier
    {
        private readonly IWebSocketManager webSocketManager;

        private readonly IExternalEventSubscription externalEventSubscription;

        private readonly WebSocketDataSerialisation serialiser;

        public WebSocketEventNotifier(IWebSocketManager manager, IExternalEventSubscription externalEventSubscription)
        {
            this.serialiser = new WebSocketDataSerialisation();
            this.externalEventSubscription = externalEventSubscription;
            this.webSocketManager = manager;
            this.webSocketManager.SocketDisconnected += this.WebSocketManagerOnSocketDisconnected;
            this.webSocketManager.MessageReceived += this.WebSocketManagerOnMessageReceived;
        }

        private void WebSocketManagerOnMessageReceived(object sender, SocketMessage socketMessage)
        {
            string resource =
                socketMessage.Message.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries)[1];

            var subscription =
                this.externalEventSubscription.Subscribe(new EventSubscriptionMessage(socketMessage.SocketId, resource));

            var seralisedString = this.serialiser.GetString(subscription);
            this.webSocketManager.SendMessage(socketMessage.SocketId, seralisedString);
        }

        private void WebSocketManagerOnSocketDisconnected(object sender, Guid guid)
        {
            this.externalEventSubscription.UnsubscribeAll(guid);
        }

        public void Notify(Guid subscriberId, IDomainEvent eventToPublish)
        {
            var serialisedMessage = this.serialiser.GetString(eventToPublish);
            this.webSocketManager.SendMessage(subscriberId, serialisedMessage);
        }
    }
}
