using System.Threading.Tasks;

namespace Common
{
    public interface IEventStoreEventHandler
    {
        Task<EventProcessingState> HandleAsync(IEventStoreEvent @event);
    }
}
