namespace MEungblut.Websockets.UnitTests
{
    using System;

    public class DummyEvent : IDomainEvent
    {
        public DummyEvent()
        {
            this.Id = Guid.NewGuid().ToString();
        }

        public string Id { get; set; }
    }
}