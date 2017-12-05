using Common;

namespace Events
{
    public class ItemRemovedFromBasketEvent : IEventMetaDataEvent
    {
        public string ItemName { get; set; }
        public int CustomerId { get; set; }
        public int CustomEventId { get; set; }
        public EventMetaData EventMetaData { get; set; }
    }
}
