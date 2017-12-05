using System.Threading.Tasks;

namespace Common
{
    public interface IEventHandler<TEvent>
    {
        Task<EventProcessingState> HandleEventAsync(TEvent @event);
    }
}
