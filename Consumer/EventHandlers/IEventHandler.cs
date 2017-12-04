namespace Consumer.EventHandlers
{
    public interface IEventHandler<TEvent>
    {
        void HandleEvent(TEvent @event);
    }
}
