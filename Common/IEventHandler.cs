namespace Common
{
    public interface IEventHandler<TEvent>
    {
        void HandleEvent(TEvent @event);
    }
}
