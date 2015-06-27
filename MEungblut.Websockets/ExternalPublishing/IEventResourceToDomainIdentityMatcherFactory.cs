namespace MEungblut.Websockets.ExternalPublishing
{
    using System.Collections.Generic;

    public interface IEventResourceToSubscriptionMatcherFactory
    {
        IEnumerable<IEventResourceToEventSubscriptionMatcher> GetResourceMatchers();
    }
}