namespace MEungblut.Websockets.ExternalPublishing
{
    using System;

    public abstract class EventSubscription
    {
        public EventSubscription(Guid subscriberId)
        {
            this.SubscriberId = subscriberId;
            this.Id = Guid.NewGuid();
        }

        public abstract bool NotifyFor(IDomainEvent eventRaised);

        public Guid SubscriberId { get; protected set; }

        public Guid Id { get; protected set; }
    }
}
