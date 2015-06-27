namespace MEungblut.Websockets.UnitTests
{
    using System;

    using MEungblut.Websockets.ExternalPublishing;

    public class DummyEventSubscription : EventSubscription
    {
        public DummyEventSubscription(Guid subscriptionId, Guid subscriberId) : base(subscriberId)
        {
            this.Id = subscriptionId;
        }

        public override bool NotifyFor(IDomainEvent eventRaised)
        {
            return true;
        }
    }
}