using System;
using System.Linq;
using System.Threading.Tasks;
using Common;
using Events;
using Persistence;

namespace EventHandlers
{
    public class ItemRemovedFromBasketEventHandler : IEventHandler<ItemRemovedFromBasketEvent>
    {
        public async Task<EventProcessingState> HandleEventAsync(ItemRemovedFromBasketEvent @event)
        {
            using (var context = new BasketStateContext())
            {
                context.Database.EnsureCreated();
                var item = context.BasketItems.Single(e =>
                    e.ItemName == @event.ItemName && e.CustomerId == @event.CustomerId);


                context.BasketItems.Remove(item);

                await context.SaveChangesAsync();
                Console.WriteLine($"Item {@event.ItemName} was removed from the basket");

                return EventProcessingState.Success;
            }
        }
    }
}
