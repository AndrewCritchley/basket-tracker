using System;
using Events;

namespace Consumer.EventHandlers
{
    public class ItemRemovedFromBasketEventHandler : IEventHandler<ItemRemovedFromBasketEvent>
    {
        public void HandleEvent(ItemRemovedFromBasketEvent @event)
        {
            Console.WriteLine($"Item {@event.ItemName} was removed from the basket");
        }
    }
}
