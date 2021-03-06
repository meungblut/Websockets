﻿namespace MEungblut.Websockets.ExternalPublishing
{
    using System;

    public class NoMatchingEventSubscription : EventSubscription
    {
        public NoMatchingEventSubscription(Guid subscriberId)
            : base(subscriberId)
        {
        }

        public override bool NotifyFor(object eventRaised)
        {
            throw new NotImplementedException("Null subscription shouldn't be called");
        }
    }
}
