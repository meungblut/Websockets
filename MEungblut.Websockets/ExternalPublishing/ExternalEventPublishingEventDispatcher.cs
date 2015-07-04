namespace MEungblut.Websockets.ExternalPublishing
{
    public class ExternalEventPublishingEventDispatcher : IEventHandler<IDomainEvent>
    {
        private readonly IExternalEventPublisher eventPublisher;

        public ExternalEventPublishingEventDispatcher(IExternalEventPublisher externalEventPublisher)
        {
            this.eventPublisher = externalEventPublisher;
        }

        public void Notify(IDomainEvent domainEvent)
        {
            this.eventPublisher.Publish(domainEvent); 
        }
    }
} 