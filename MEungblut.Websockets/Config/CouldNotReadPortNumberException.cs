namespace MEungblut.Websockets.Config
{
    using System;

    public class CouldNotReadPortNumberException : Exception
    {
        public CouldNotReadPortNumberException(string message) : base(message)
        {
        }

        public CouldNotReadPortNumberException(string message, Exception exception)
            : base(message, exception)
        {
        }
    }
}
