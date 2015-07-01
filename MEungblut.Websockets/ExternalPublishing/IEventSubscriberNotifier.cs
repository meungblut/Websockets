namespace MEungblut.Websockets.ExternalPublishing
{
    using System;

    public interface IEventSubscriberNotifier
    {
        void Notify(Guid subscriberId, object eventToPublish);
    }
}