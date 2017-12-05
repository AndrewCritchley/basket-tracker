using Common;
using EventStore.ClientAPI;

namespace Consumer.EventStore
{
    public interface IEventStoreEventFactory
    {
        /// <summary>
        /// Creates a wrapped IEventStoreEvent from the resolved event received from the EventStore API Client
        /// </summary>
        /// <param name="resolvedEvent"></param>
        /// <returns></returns>
        IEventStoreEvent CreateFrom(ResolvedEvent resolvedEvent);
    }

    public class DefaultEventStoreEventFactory : IEventStoreEventFactory
    {
        public IEventStoreEvent CreateFrom(ResolvedEvent resolvedEvent)
        {
            return new EventStoreEvent(resolvedEvent);
        }
    }
}
