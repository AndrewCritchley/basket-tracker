using System;
using Common;
using EventHandlers;
using Events;
using Persistence;
using Persistence.Model;

namespace EventHandlers
{
    public class ItemRemovedFromBasketEventHandler : IEventHandler<ItemRemovedFromBasketEvent>
    {
        public void HandleEvent(ItemRemovedFromBasketEvent @event)
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
                Console.WriteLine($"Item {@event.ItemName} was removed from the basket");
            }
        }
    }
}
