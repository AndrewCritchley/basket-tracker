using System;
using System.Threading.Tasks;
using Common;
using Events;
using Newtonsoft.Json;

namespace EventHandlers
{
    internal class ItemAddedTobasketEventStoreHandler : IEventStoreEventHandler
    {
        private readonly IEventHandler<ItemAddedToBasketEvent> _eventHandler;
        public ItemAddedTobasketEventStoreHandler(IEventHandler<ItemAddedToBasketEvent> eventHandler)
        {
            _eventHandler = eventHandler ?? throw new ArgumentNullException(nameof(eventHandler));
        }

        public async Task<EventProcessingState> HandleAsync(IEventStoreEvent eventStoreEvent)
        {
            ItemAddedToBasketEvent deserializedEvent = GetDeserializedEvent(eventStoreEvent);

            return await _eventHandler.HandleEventAsync(deserializedEvent);
        }

        private ItemAddedToBasketEvent GetDeserializedEvent(IEventStoreEvent eventStoreEvent)
        {
            EventMetaData eventMetaData = JsonConvert.DeserializeObject<EventMetaData>(eventStoreEvent.MetaData.Decode());
            ItemAddedToBasketEvent eventObject = JsonConvert.DeserializeObject<ItemAddedToBasketEvent>(eventStoreEvent.Data.Decode());
            eventObject.EventMetaData = eventMetaData;
            return eventObject;
        }
    }
}
