namespace MEungblut.Websockets.ExternalPublishing
{
    using System;

    public class ExternalEventSubscriber : IExternalEventSubscription
    {
        private readonly ISubscriptionRepository subscriptionRepository;

        private readonly IEventResourceToSubscriptionMatcherFactory resourceToDomainIdentityMatcherFactory;

        public ExternalEventSubscriber(ISubscriptionRepository repository, IEventResourceToSubscriptionMatcherFactory eventResourceToDomainIdentityMatcherFactory)
        {
            this.resourceToDomainIdentityMatcherFactory = eventResourceToDomainIdentityMatcherFactory;
            this.subscriptionRepository = repository;
        } 
         
        public EventSubscription Subscribe(EventSubscriptionMessage subscriptionMessage)
        { 
            foreach (var resourceMatcher in this.resourceToDomainIdentityMatcherFactory.GetResourceMatchers())
            {
                var subscription = resourceMatcher.CreateSubscription(
                    subscriptionMessage.EventResource,
                    subscriptionMessage.Subscriber);

                if (!(subscription is NoMatchingEventSubscription))
                {
                    this.subscriptionRepository.Add(subscription);
                    return subscription;
                }
            }

            throw new CouldNotFindMatchingSubscriptionForResourceException();
        }

        public void Unsubscribe(Guid subscriptionId)
        {
            this.subscriptionRepository.Remove(subscriptionId);
        }

        public void UnsubscribeAll(Guid subscriberId)
        {
            foreach (var subscription in this.subscriptionRepository.Get())
            {
                if (subscription.SubscriberId == subscriberId)
                    this.subscriptionRepository.Remove(subscription.Id);
            }
        }
    }
}
