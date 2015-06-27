namespace MEungblut.Websockets.ExternalPublishing
{
    using System;
    using System.Collections.Generic;

    public interface ISubscriptionRepository
    {
        void Add(EventSubscription subscription);

        void Remove(Guid subscriptionId);

        IEnumerable<EventSubscription> Get();
    }
}