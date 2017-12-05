using System;
using System.Threading.Tasks;
using Common;
using Events;
using Newtonsoft.Json;

namespace EventHandlers
{
    internal class ItemRemovedFrombasketEventStoreHandler : IEventStoreEventHandler
    {
        private readonly IEventHandler<ItemRemovedFromBasketEvent> _eventHandler;

        public ItemRemovedFrombasketEventStoreHandler(IEventHandler<ItemRemovedFromBasketEvent> eventHandler)
        {
            _eventHandler = eventHandler ?? throw new ArgumentNullException(nameof(eventHandler));
        }

        public async Task<EventProcessingState> HandleAsync(IEventStoreEvent eventStoreEvent)
        {
            ItemRemovedFromBasketEvent deserializedEvent = GetDeserializedEvent(eventStoreEvent);
            return await _eventHandler.HandleEventAsync(deserializedEvent);
        }

        private ItemRemovedFromBasketEvent GetDeserializedEvent(IEventStoreEvent eventStoreEvent)
        {
            EventMetaData eventMetaData = JsonConvert.DeserializeObject<EventMetaData>(eventStoreEvent.MetaData.Decode());
            var eventObject = JsonConvert.DeserializeObject<ItemRemovedFromBasketEvent>(eventStoreEvent.Data.Decode());
            eventObject.EventMetaData = eventMetaData;
            return eventObject;
        }
    }
}
