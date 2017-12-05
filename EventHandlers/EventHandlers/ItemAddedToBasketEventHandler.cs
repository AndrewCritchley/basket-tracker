using Common;
using Events;
using Persistence;
using Persistence.Model;

namespace EventHandlers
{
    public class ItemAddedToBasketEventHandler : IEventHandler<ItemAddedToBasketEvent>
    {
        public void HandleEvent(ItemAddedToBasketEvent @event)
        {
            using (var context = new BasketStateContext())
            {
                context.Database.EnsureCreated();
                context.BasketItems.Add(new BasketItem()
                {
                    ItemName = @event.ItemName,
                    CustomerId = @event.CustomerId,
                });
                context.SaveChanges();
            }
        }
    }
}
