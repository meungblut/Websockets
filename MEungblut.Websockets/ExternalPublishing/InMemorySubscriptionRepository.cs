namespace MEungblut.Websockets.ExternalPublishing
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;

    public class InMemorySubscriptionRepository : ISubscriptionRepository
    {
        private readonly ConcurrentDictionary<Guid, EventSubscription> subscribers;

        public InMemorySubscriptionRepository()
        {
            this.subscribers = new ConcurrentDictionary<Guid, EventSubscription>();
        }

        public void Add(EventSubscription subscription)
        {
            this.subscribers.AddOrUpdate(subscription.SubscriberId, subscription, (key, value) => subscription);
        }

        public void Remove(Guid subscriptionId)
        {
            EventSubscription sub;
            this.subscribers.TryRemove(subscriptionId, out sub);
        }

        public IEnumerable<EventSubscription> Get()
        {
            return this.subscribers.Values;
        } 
    }
}
