using System;
using System.Collections.Generic;
using System.Text;
using Consumer.EventHandlers;
using Events;
using Newtonsoft.Json;

namespace Consumer.EventStore
{
    /// <summary>
    /// Handler which knows what to do with an EventStoreEvent
    /// Handlers are configured through Castle.Windsor
    /// </summary>
    public interface IEventStoreEventHandler
    {
        void Handle(IEventStoreEvent @event);
    }

    internal class ItemAddedTobasketEventStoreHandler : IEventStoreEventHandler
    {
        private readonly IEventHandler<ItemAddedToBasketEvent> _eventHandler;

        /// <summary>
        /// Handles a EventStoreEvent which is equivalent to a CWTEvent - in this case ComplianceCalculatedEventV1
        /// </summary>
        /// <param name="cwtEventHandler">A handler which knows how to handle</param>
        /// <param name="jsonDeserializer"></param>
        /// <param name="eventConsumptionConfiguration"></param>
        public ItemAddedTobasketEventStoreHandler(IEventHandler<ItemAddedToBasketEvent> eventHandler)
        {
            _eventHandler = eventHandler ?? throw new ArgumentNullException(nameof(eventHandler));
        }

        public async void Handle(IEventStoreEvent eventStoreEvent)
        {
            ItemAddedToBasketEvent deserializedEvent = GetDeserializedEvent(eventStoreEvent);

            _eventHandler.HandleEvent(deserializedEvent);
        }

        private ItemAddedToBasketEvent GetDeserializedEvent(IEventStoreEvent eventStoreEvent)
        {
            EventMetaData eventMetaData = JsonConvert.DeserializeObject<EventMetaData>(eventStoreEvent.MetaData.Decode());
            ItemAddedToBasketEvent cwtEvent = JsonConvert.DeserializeObject<ItemAddedToBasketEvent>(eventStoreEvent.Data.Decode());
            cwtEvent.EventMetaData = eventMetaData;
            return cwtEvent;
        }
    }
}
