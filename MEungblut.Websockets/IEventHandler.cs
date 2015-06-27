namespace MEungblut.Websockets
{
    public interface IEventHandler<T> where T : IDomainEvent
    {
        void Notify(T domainEvent);
    }
}