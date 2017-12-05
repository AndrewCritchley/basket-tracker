using Common;

namespace Events
{
    public class ItemAddedToBasketEvent  : IEventMetaDataEvent
    {
        public string ItemName { get; set; }
        public int CustomerId { get; set; }
        public int CustomEventId { get; set; }
        public IEventStoreMetaData ActualEventMetaData { get; }
        public bool IsSystemEvent { get; }
        public byte[] Data { get; }
        public byte[] MetaData { get; }
        public IEventStoreMetaData ConsumedEventMetaData { get; }
        public bool EventDataAvailable { get; }
        public EventMetaData EventMetaData { get; set; }
    }

    public interface IEventMetaDataEvent
    {
        EventMetaData EventMetaData { get; set; }
    }
}
