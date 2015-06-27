namespace MEungblut.Websockets.ExternalPublishing
{
    using System;

    public interface IEventResourceToEventSubscriptionMatcher
    {
        EventSubscription CreateSubscription(string resourceString, Guid subscriptionId); 
    }
}