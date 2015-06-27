namespace MEungblut.Websockets.UnitTests
{
    using System;

    using MEungblut.Websockets.Owin;

    using Moq;

    using NUnit.Framework;

    using Possum.Infrastructure.Logging;

    [TestFixture]
    public class LoggingWebSocketManagerDecoratorShould
    {
        private Mock<IWebSocketManager> socketManager;
        private Mock<ILoggingFrameworkAdapter> loggingAdapter;
        private LoggingWebSocketManagerDecorator loggingWebSocketManagerDecorator;

        [SetUp]
        public void Setup()
        {
            this.socketManager = new Mock<IWebSocketManager>();
            this.loggingAdapter = new Mock<ILoggingFrameworkAdapter>();
            this.loggingWebSocketManagerDecorator = new LoggingWebSocketManagerDecorator(this.socketManager.Object, this.loggingAdapter.Object);
        }

        [Test]
        public void LogSocketConnections()
        {
            Guid connectionId = Guid.NewGuid();
            this.socketManager.Raise(x => x.SocketConnected += null, this.socketManager, connectionId);

            this.loggingAdapter.Verify(x => x.LogDebug("Websocket connected", connectionId));
        }

        [Test]
        public void LogSocketDisconnections()
        {
            Guid connectionId = Guid.NewGuid();
            this.socketManager.Raise(x => x.SocketDisconnected += null, this.socketManager, connectionId);

            this.loggingAdapter.Verify(x => x.LogDebug("Websocket disconnected", connectionId));
        }

        [Test]
        public void LogSocketMessages()
        {
            SocketMessage message = new SocketMessage(Guid.NewGuid(), "Message");
            this.socketManager.Raise(x => x.MessageReceived += null, this.socketManager, message);

            this.loggingAdapter.Verify(x => x.LogDebug("Websocket message received", message));
        }

        [Test]
        public void LogSentMessages()
        {
            Guid identifier = Guid.NewGuid();
            string message = "a message";
            this.loggingWebSocketManagerDecorator.SendMessage(identifier, message);

            this.loggingAdapter.Verify(x => x.LogDebug("Sending message to socket", identifier, message));
        }

        [Test]
        public void LogBroadcastMessages()
        {
            string message = "a message";
            this.loggingWebSocketManagerDecorator.BroadcastToAllClients(message);

            this.loggingAdapter.Verify(x => x.LogDebug("Message broadcast", message));
        }

        [Test]
        public void CallDisposeOnDecoratedManager()
        {
            this.loggingWebSocketManagerDecorator.Dispose();

            this.socketManager.Verify(x => x.Dispose());
        }

        [Test]
        public void CallSendOnDecoratedManager()
        {
            Guid identifier = Guid.NewGuid();
            string message = "a message";
            this.loggingWebSocketManagerDecorator.SendMessage(identifier, message);

            this.socketManager.Verify(x => x.SendMessage(identifier, message));
        }

        [Test]
        public void CallBroadcastOnDecoratedManager()
        {
            Guid identifier = Guid.NewGuid();
            string message = "a message";
            this.loggingWebSocketManagerDecorator.BroadcastToAllClients(message);

            this.socketManager.Verify(x => x.BroadcastToAllClients(message));
        }

        [Test]
        public void LogExceptionOnSend()
        {
            var identifier = Guid.NewGuid();
            var message = "message";
            var exceptionToThrow = new Exception();
            this.socketManager.Setup(x => x.SendMessage(identifier, message)).Throws(exceptionToThrow);

            try
            {
                this.loggingWebSocketManagerDecorator.SendMessage(identifier, message);
            }
            catch (Exception exception) { }

            this.loggingAdapter.Verify(x => x.LogException(exceptionToThrow));
        }

        [Test]
        public void RaiseMessageEventFromDecoratedManager()
        {
            this.loggingWebSocketManagerDecorator.MessageReceived += this.LoggingWebSocketManagerDecoratorOnMessageReceived;

            this.socketManager.Raise(x => x.MessageReceived += null, this.socketManager, new SocketMessage(new Guid(), string.Empty));

            Assert.IsTrue(this.messageReceivedEventRaised);
        }

        [Test]
        public void LogExceptionsEncounteredInMessageReceivedEventhanders()
        {
            this.loggingWebSocketManagerDecorator.MessageReceived += this.ExceptionThrowingMessageReceivedHandler;
            this.socketManager.Raise(x => x.MessageReceived += null, this.socketManager, new SocketMessage(new Guid(), string.Empty));
            this.loggingAdapter.Verify(x => x.LogException(It.IsAny<Exception>()));
        }

        [Test]
        public void RaiseConnectedEventFromDecoratedManager()
        {
            this.loggingWebSocketManagerDecorator.SocketConnected += this.LoggingWebSocketManagerDecoratorOnSocketConnectedOrDisconnected;

            this.socketManager.Raise(x => x.SocketConnected += null, this.socketManager, new Guid());

            Assert.IsTrue(this.messageConnectedOrDisconnectedEventRaised);
        }

        [Test]
        public void LogExceptionsEncounteredInConnectedEventhanders()
        {
            this.loggingWebSocketManagerDecorator.SocketConnected += this.ExceptionThrowingConnectedOrDisconnectedHandler;
            this.socketManager.Raise(x => x.SocketConnected += null, this.socketManager, new Guid());
            this.loggingAdapter.Verify(x => x.LogException(It.IsAny<Exception>()));
        }

        [Test]
        public void LogExceptionsEncounteredInDisconnectedEventhanders()
        {
            this.loggingWebSocketManagerDecorator.SocketDisconnected += this.ExceptionThrowingConnectedOrDisconnectedHandler;
            this.socketManager.Raise(x => x.SocketDisconnected += null, this.socketManager, new Guid());
            this.loggingAdapter.Verify(x => x.LogException(It.IsAny<Exception>()));
        }

        [Test]
        public void RaiseDisconnectedEventFromDecoratedManager()
        {
            this.loggingWebSocketManagerDecorator.SocketDisconnected += this.LoggingWebSocketManagerDecoratorOnSocketConnectedOrDisconnected;

            this.socketManager.Raise(x => x.SocketDisconnected += null, this.socketManager, new Guid());

            Assert.IsTrue(this.messageConnectedOrDisconnectedEventRaised);
        }

        private bool messageConnectedOrDisconnectedEventRaised;

        private void LoggingWebSocketManagerDecoratorOnSocketConnectedOrDisconnected(object sender, Guid guid)
        {
            this.messageConnectedOrDisconnectedEventRaised = true;
        }

        private void ExceptionThrowingMessageReceivedHandler(object sender, SocketMessage socketMessage)
        {
            throw new Exception();
        }

        private void ExceptionThrowingConnectedOrDisconnectedHandler(object sender, Guid connectedGuid)
        {
            throw new Exception();
        }

        private bool messageReceivedEventRaised;
        private void LoggingWebSocketManagerDecoratorOnMessageReceived(object sender, SocketMessage socketMessage)
        {
            this.messageReceivedEventRaised = true;
        }
    }
}
