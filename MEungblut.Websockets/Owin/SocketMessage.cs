namespace MEungblut.Websockets.Owin
{
    using System;

    public class SocketMessage
    {
        public SocketMessage(Guid socketId, string message)
        {
            this.SocketId = socketId;
            this.Message = message;
        }
        public Guid SocketId { get; private set; }

        public string Message { get; private set; }
    }
}