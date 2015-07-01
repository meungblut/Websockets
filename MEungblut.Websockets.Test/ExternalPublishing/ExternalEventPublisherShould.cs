namespace MEungblut.Websockets.UnitTests.ExternalPublishing
{
    using System;

    using MEungblut.Websockets.ExternalPublishing;
    using MEungblut.Websockets.UnitTests;

    using Moq;

    using NUnit.Framework;

    [TestFixture]
    public class ExternalEventPublisherShould
    {
        private Mock<IEventSubscriberNotifier> communicator;
        private Mock<ISubscriptionRepository> subscriptionRepository;

        private ExternalEventPublisher externalEventPublisher;

        private EventSubscription subscription;

        private Guid subscriberId;

        [SetUp]
        public void Setup()
        {
            this.communicator = new Mock<IEventSubscriberNotifier>();
            this.subscriptionRepository = new Mock<ISubscriptionRepository>();
            this.externalEventPublisher = new ExternalEventPublisher(this.communicator.Object, this.subscriptionRepository.Object);

            this.subscriberId = Guid.NewGuid();
        }

        [Test]
        public void NotifyAllSubscribersForAllEvents()
        {
            this.subscription = new AlwaysNotifyingEventSubscription(Guid.NewGuid(), this.subscriberId);
            this.subscriptionRepository.Setup(x => x.Get()).Returns(new[] { this.subscription });

            var eventToPublish = new DummyEvent();
            this.externalEventPublisher.Publish(eventToPublish);

            this.communicator.Verify(x => x.Notify(this.subscriberId, eventToPublish));
        }
        
        [Test]
        public void NotNotifySubscriberIfSubscriptionIsNotSetUpToNotify()
        {
            this.subscription = new NeverNotifyingEventSubscription(this.subscriberId);
            this.subscriptionRepository.Setup(x => x.Get()).Returns(new[] { this.subscription });

            var eventToPublish = new DummyEvent();
            this.externalEventPublisher.Publish(eventToPublish);

            this.communicator.Verify(x => x.Notify(this.subscriberId, eventToPublish), Times.Never);
        }

    }
}
