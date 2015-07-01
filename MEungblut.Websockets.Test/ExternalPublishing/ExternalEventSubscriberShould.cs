namespace MEungblut.Websockets.UnitTests.ExternalPublishing
{
    using System;

    using MEungblut.Websockets.ExternalPublishing;

    using Moq;

    using NUnit.Framework;

    [TestFixture]
    public class ExternalEventSubscriberShould
    {
        private Guid eventClientId;
        private Guid eventId;
        private string eventResourceToSubscribeTo;
        private EventSubscriptionMessage eventSubscriptionMessage;
        private Guid eventSubscriptionId;
        private AlwaysNotifyingEventSubscription eventSubscription;

        private ExternalEventSubscriber externalEventSubscriber;

        private Mock<ISubscriptionRepository> repository;

        [SetUp]
        public void Setup()
        {
            this.eventClientId = Guid.NewGuid();
            this.eventId = Guid.NewGuid();
            this.eventSubscriptionId = Guid.NewGuid();

            this.eventSubscription = new AlwaysNotifyingEventSubscription(this.eventSubscriptionId, this.eventClientId);

            this.repository = new Mock<ISubscriptionRepository>();
            IEventResourceToEventSubscriptionMatcher matcher = new AlwaysNotifyingEventIdMatcher(this.eventSubscription);
            var factory = new Mock<IEventResourceToSubscriptionMatcherFactory>();
            factory.Setup(x => x.GetResourceMatchers()).Returns(new[] { matcher });
            this.externalEventSubscriber = new ExternalEventSubscriber(this.repository.Object, factory.Object);
        }

        [Test]
        public void PassEventSubscriptionToRepository()
        {
            this.eventResourceToSubscribeTo = string.Format("match", this.eventId);
            this.eventSubscriptionMessage = new EventSubscriptionMessage(this.eventClientId, this.eventResourceToSubscribeTo);

            this.externalEventSubscriber.Subscribe(this.eventSubscriptionMessage);

            this.repository.Verify(x => x.Add(It.IsAny<AlwaysNotifyingEventSubscription>()));
        }

        [Test]
        public void ReturnTheSubscription()
        {
            this.eventResourceToSubscribeTo = string.Format("match", this.eventId);
            this.eventSubscriptionMessage = new EventSubscriptionMessage(this.eventClientId, this.eventResourceToSubscribeTo);
            var returnedValue = this.externalEventSubscriber.Subscribe(this.eventSubscriptionMessage);

            Assert.AreEqual(this.eventSubscriptionId, returnedValue.Id);
        }

        [Test]
        public void ThrowCouldNotFindMAtchingSubscriptionMessageIfNoMatchCanBeFound()
        {
            this.eventResourceToSubscribeTo = string.Format("nomatch", this.eventId);
            this.eventSubscriptionMessage = new EventSubscriptionMessage(this.eventClientId, this.eventResourceToSubscribeTo);

            try
            {
                this.externalEventSubscriber.Subscribe(this.eventSubscriptionMessage);
                Assert.Fail();
            }
            catch (CouldNotFindMatchingSubscriptionForResourceException)
            {

            }
        }

        [Test]
        public void RemoveSubscriptionFromRepositoryWhenUnsubscribeCalled()
        {
            this.eventResourceToSubscribeTo = string.Format("match", this.eventId);
            this.eventSubscriptionMessage = new EventSubscriptionMessage(this.eventClientId, this.eventResourceToSubscribeTo);
            var subscription = this.externalEventSubscriber.Subscribe(this.eventSubscriptionMessage);

            this.externalEventSubscriber.Unsubscribe(subscription.Id);

            this.repository.Verify(x => x.Remove(subscription.Id));
        }

        [Test]
        public void RemoveAllSubscriberConnectionsWhenASubscriberDisconnects()
        {
            Guid subscriberId = Guid.NewGuid();
            Guid subscriptionId1 = Guid.NewGuid();
            Guid subscriptionId2 = Guid.NewGuid();

            AlwaysNotifyingEventSubscription subscription1 = new AlwaysNotifyingEventSubscription(subscriptionId1, subscriberId);
            AlwaysNotifyingEventSubscription subscription2 = new AlwaysNotifyingEventSubscription(subscriptionId2, subscriberId);

            this.repository.Setup(x => x.Get()).Returns(new[] { subscription1, subscription2 });

            this.externalEventSubscriber.UnsubscribeAll(subscriberId);

            this.repository.Verify(x => x.Remove(subscriptionId1));
            this.repository.Verify(x => x.Remove(subscriptionId2));
        }
    }

    public class AlwaysNotifyingEventIdMatcher : IEventResourceToEventSubscriptionMatcher
    {
        private AlwaysNotifyingEventSubscription alwaysNotifyingEventSubscription;

        public AlwaysNotifyingEventIdMatcher(AlwaysNotifyingEventSubscription subscriptionToReturnOnMatch)
        {
            this.alwaysNotifyingEventSubscription = subscriptionToReturnOnMatch;
        }

        public EventSubscription CreateSubscription(string resourceString, Guid subscriptionId)
        {
            if (resourceString == "match")
                return this.alwaysNotifyingEventSubscription;

            return new NoMatchingEventSubscription(subscriptionId);
        }
    }
}
