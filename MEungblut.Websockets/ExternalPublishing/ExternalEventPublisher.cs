namespace MEungblut.Websockets.ExternalPublishing
{
    public class ExternalEventPublisher : IExternalEventPublisher
    {
        private readonly IEventSubscriberNotifier subscriberCommunicator;
        private readonly ISubscriptionRepository subscriptionRepository;

        public ExternalEventPublisher(IEventSubscriberNotifier communicator, ISubscriptionRepository repository)
        {
            this.subscriptionRepository = repository;
            this.subscriberCommunicator = communicator;
        }

        public void Publish(object eventToPublish)
        {
            foreach (var eventSubscription in this.subscriptionRepository.Get())
            {
                if (eventSubscription.NotifyFor(eventToPublish))
                    this.subscriberCommunicator.Notify(eventSubscription.SubscriberId, eventToPublish);
            }
        }

        public void Broadcast(object eventToPublish)
        {
            foreach (var eventSubscription in this.subscriptionRepository.Get())
            {
                this.subscriberCommunicator.Notify(eventSubscription.SubscriberId, eventToPublish);
            }
        }
    }
}