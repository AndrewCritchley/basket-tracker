using System;
using Consumer.EventHandlers;
using Events;
using Newtonsoft.Json;

namespace Consumer.EventStore
{
    internal class ItemRemovedFrombasketEventStoreHandler : IEventStoreEventHandler
    {

        private readonly IEventHandler<ItemRemovedFromBasketEvent> _eventHandler;

        /// <summary>
        /// Handles a EventStoreEvent which is equivalent to a CWTEvent - in this case ComplianceCalculatedEventV1
        /// </summary>
        /// <param name="cwtEventHandler">A handler which knows how to handle</param>
        /// <param name="jsonDeserializer"></param>
        /// <param name="eventConsumptionConfiguration"></param>
        public ItemRemovedFrombasketEventStoreHandler(IEventHandler<ItemRemovedFromBasketEvent> eventHandler)
        {
            _eventHandler = eventHandler ?? throw new ArgumentNullException(nameof(eventHandler));
        }

        public async void Handle(IEventStoreEvent eventStoreEvent)
        {
            ItemRemovedFromBasketEvent deserializedEvent = GetDeserializedEvent(eventStoreEvent);

            _eventHandler.HandleEvent(deserializedEvent);
        }

        private ItemRemovedFromBasketEvent GetDeserializedEvent(IEventStoreEvent eventStoreEvent)
        {
            EventMetaData eventMetaData = JsonConvert.DeserializeObject<EventMetaData>(eventStoreEvent.MetaData.Decode());
            var cwtEvent = JsonConvert.DeserializeObject<ItemRemovedFromBasketEvent>(eventStoreEvent.Data.Decode());
            cwtEvent.EventMetaData = eventMetaData;
            return cwtEvent;
        }
    }
}
