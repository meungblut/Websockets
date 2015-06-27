namespace MEungblut.Websockets.ExternalPublishing
{
    using System;

    public interface IExternalEventSubscription
    {
        EventSubscription Subscribe(EventSubscriptionMessage subscription); 

        void Unsubscribe(Guid subscriptionId);

        void UnsubscribeAll(Guid subscriberId); 
    }
}
