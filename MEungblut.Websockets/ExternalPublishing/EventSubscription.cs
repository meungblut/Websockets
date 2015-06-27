namespace MEungblut.Websockets.ExternalPublishing
{
    using System;

    public class EventSubscriptionMessage
    {
        public Guid Subscriber { get; private set; }

        public string EventResource { get; private set; }

        public EventSubscriptionMessage(Guid subscriber, string eventResource)
        {
            this.EventResource = eventResource;
            this.Subscriber = subscriber;
        }
    }
}