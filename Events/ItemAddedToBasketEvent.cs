using System;

namespace Events
{
    public class ItemAddedToBasketEvent
    {
        public string ItemName { get; set; }
        public int ItemId { get; set; }
        public int CustomEventId { get; set; }
    }
}
