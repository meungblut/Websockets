namespace MEungblut.Websockets.UnitTests.ExternalPublishing.Protocol
{
    using MEungblut.Websockets.ExternalPublishing.Protocol;
    using MEungblut.Websockets.UnitTests;

    using NUnit.Framework;

    [TestFixture]
    public class WebSocketSerialiserShould
    {
        [Test]
        public void SerialiseDummyEventToExpectedFormat()
        {
            var dummyEvent = this.GetDummyEvent();
            string expectedMessage =
@"content-type:application/vnd.meungblut.websockets.dummyevent+json

{""id"":""6a8d7bcb-4d16-4b22-af52-60be3860ab86"",""someData"":""some data""}";
            var serialiser = new WebSocketDataSerialisation();
            var serialisedMessage = serialiser.GetString(dummyEvent);
            Assert.AreEqual(expectedMessage, serialisedMessage);
        }

        [Test]
        public void DeserialiseDummyEvent()
        {
            string messageToDeserialise =
@"content-type:application/vnd.meungblut.websockets.dummyevent+json

{""id"":""6a8d7bcb-4d16-4b22-af52-60be3860ab86"",""someData"":""some data""}";
            var serialiser = new WebSocketDataSerialisation();

            WebSocketDataSerialisation.AddTypeToDeserialise("content-type:application/vnd.meungblut.websockets.dummyevent+json", typeof(DummyEvent));

            var deserialisedObject = (DummyEvent)serialiser.GetObject(messageToDeserialise);

            Assert.AreEqual("6a8d7bcb-4d16-4b22-af52-60be3860ab86", deserialisedObject.Id);
            Assert.AreEqual("some data", deserialisedObject.SomeData);
        }

        private DummyEvent GetDummyEvent()
        {
            var dummyEvent = new DummyEvent();
            dummyEvent.SomeData = "some data";
            dummyEvent.Id = "6a8d7bcb-4d16-4b22-af52-60be3860ab86";
            return dummyEvent;
        }
    }
}
