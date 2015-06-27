namespace MEungblut.Websockets.ExternalPublishing.Protocol
{
    using System;

    public class UnknownMediaTypeForDeserialisationException : Exception
    {
        public UnknownMediaTypeForDeserialisationException(string mediaType) : base(mediaType)
        {
            
        }
    }
}
