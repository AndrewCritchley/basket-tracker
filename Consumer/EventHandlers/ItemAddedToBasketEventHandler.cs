using System;
using Events;

namespace Consumer.EventHandlers
{
    public class ItemAddedToBasketEventHandler : IEventHandler<ItemAddedToBasketEvent>
    {
        public void HandleEvent(ItemAddedToBasketEvent @event)
        {
            Console.WriteLine($"Item {@event.ItemName} was added to the basket");
        }
    }
}
