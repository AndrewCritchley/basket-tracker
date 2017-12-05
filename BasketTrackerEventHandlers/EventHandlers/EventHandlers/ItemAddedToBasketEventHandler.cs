using System.Threading.Tasks;
using Common;
using Events;
using Persistence;
using Persistence.Model;

namespace EventHandlers
{
    public class ItemAddedToBasketEventHandler : IEventHandler<ItemAddedToBasketEvent>
    {
        public async Task<EventProcessingState> HandleEventAsync(ItemAddedToBasketEvent @event)
        {
            using (var context = new BasketStateContext())
            {
                context.Database.EnsureCreated();
                context.BasketItems.Add(new BasketItem()
                {
                    ItemName = @event.ItemName,
                    CustomerId = @event.CustomerId,
                });

                await context.SaveChangesAsync();

                return EventProcessingState.Success;
            }
        }
    }
}
