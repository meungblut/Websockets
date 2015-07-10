namespace MEungblut.Websockets.ExternalPublishing
{
    public interface IExternalEventPublisher
    {
        void Publish(object eventToPublish);
        void Broadcast(object eventToPublish);
    }
}