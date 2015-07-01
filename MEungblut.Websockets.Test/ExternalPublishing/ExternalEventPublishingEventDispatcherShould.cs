namespace MEungblut.Websockets.UnitTests.ExternalPublishing
{
    using MEungblut.Websockets.ExternalPublishing;
    using MEungblut.Websockets.UnitTests;

    using Moq;

    using NUnit.Framework;

    [TestFixture]
    public class ExternalEventPublishingEventDispatcherShould
    {
        [Test]
        public void PassRaisedEventsToExternalPublisher()
        {
            Mock<IExternalEventPublisher> publisher = new Mock<IExternalEventPublisher>();
            var eventHandler = new ExternalEventPublishingEventDispatcher(publisher.Object);

            DummyEvent eventToDispatch = new DummyEvent();
            eventHandler.Notify(eventToDispatch);

            publisher.Verify(x => x.Publish(eventToDispatch));
        }
    }
}
