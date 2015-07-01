namespace MEungblut.Websockets.UnitTests.ExternalPublishing
{
    using System;

    using MEungblut.Websockets.ExternalPublishing;

    class NeverNotifyingEventSubscription : EventSubscription
    {
        public NeverNotifyingEventSubscription(Guid subscriberId) : base(subscriberId)
        {
            this.Id = Guid.NewGuid();
        }

        public override bool NotifyFor(object eventRaised)
        {
            return false;
        }
    }
}
