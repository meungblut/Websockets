namespace MEungblut.Websockets.UnitTests.ExternalPublishing
{
    using System;

    using MEungblut.Websockets.ExternalPublishing;

    public class AlwaysNotifyingEventSubscription : EventSubscription
    {
        public AlwaysNotifyingEventSubscription(Guid subscriptionId, Guid subscriberId) : base(subscriberId)
        {
            this.Id = subscriptionId;
            this.SubscriberId = subscriberId;
        }

        public override bool NotifyFor(object eventRaised)
        {
            return true;
        }
    }
}
