namespace MEungblut.Websockets.ExternalPublishing
{
    public interface IExternalEventPublisher
    {
        void Publish(IDomainEvent eventToPublish);
    }
}