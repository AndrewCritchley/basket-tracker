using System;
using System.Text;
using EventStore.ClientAPI;

namespace Consumer.EventStore
{
    /// <summary>
    /// Abstraction / wrapper for  the EventStore ResolvedEvent class which makes it easier to test with
    /// </summary>
    public interface IEventStoreEvent
    {
        IEventStoreMetaData ActualEventMetaData { get; }
        /// <summary>
        /// Indicates whether or not this is a system event from Event Store
        /// </summary>
        /// <remarks>System event types start with '$' and we are not interested in handling these</remarks>
        bool IsSystemEvent { get; }
        /// <summary>
        /// The actual event data
        /// </summary>
        byte[] Data { get; }
        /// <summary>
        /// The actual event meta data
        /// </summary>
        byte[] MetaData { get; }
        /// <summary>
        /// The event that was received from event store
        /// </summary>
        ResolvedEvent ResolvedEvent { get; }
        /// <summary>
        /// Information on the stream the event was consumed from.
        /// Where a stream being consumed is an actual stream, the values will be the same as the Actualxxxxxx named properties
        /// Where a stream being consumed is a projection, this will be the meta data from the projected stream and will differ from the Actualxxxxx properties
        /// </summary>
        IEventStoreMetaData ConsumedEventMetaData { get; }
        /// <summary>
        /// Whether the event is available.  If projected stream and the original event has been deleted this will be set as false
        /// </summary>
        bool EventDataAvailable { get; }
    }

    /// <summary>
    /// Abstraction / wrapper for  the EventStore ResolvedEvent class which makes it easier to test with
    /// </summary>
    public class EventStoreEvent : IEventStoreEvent
    {
        private RecordedEvent RecordedEvent => ResolvedEvent.Event ?? ResolvedEvent.OriginalEvent;

        public bool IsSystemEvent => string.IsNullOrWhiteSpace(RecordedEvent.EventType) || RecordedEvent.EventType.StartsWith("$");
        public byte[] Data => ResolvedEvent.Event.Data;
        public byte[] MetaData => ResolvedEvent.Event.Metadata;
        public ResolvedEvent ResolvedEvent { get; }
        public IEventStoreMetaData ActualEventMetaData { get; }
        public IEventStoreMetaData ConsumedEventMetaData { get; }
        public bool EventDataAvailable { get; }

        public EventStoreEvent(ResolvedEvent resolvedEvent)
        {
            if (resolvedEvent.OriginalEvent == null) throw new ArgumentNullException(nameof(resolvedEvent.OriginalEvent));

            ResolvedEvent = resolvedEvent;
            ConsumedEventMetaData = new EventStoreMetaData(resolvedEvent.OriginalEvent);

            if (ResolvedEvent.Event == null)
            {
                EventDataAvailable = false;
              //  ActualEventMetaData = new NullEventStoreMetaData();
            }
            else
            {
                ActualEventMetaData = new EventStoreMetaData(resolvedEvent.Event);
                EventDataAvailable = true;
            }
        }

        /// <summary>
        /// Outputs meta data references only (no real data because it might be sensitive and ToString is likely to be used for logging purposes!)
        /// Uses ResolvedEvent rather than properties as nullable check needed for unit test scenarios
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine($"Actual: {ActualEventMetaData}");
            builder.AppendLine($"Consumed: {ConsumedEventMetaData}");
            return builder.ToString();
        }
    }

    public interface IEventStoreMetaData
    {
        long EventNumber { get; }
        string EventType { get; }
        string EventStreamId { get; }
        Guid EventId { get; }
    }

    public class EventStoreMetaData : IEventStoreMetaData
    {
        private readonly RecordedEvent _event;

        public EventStoreMetaData(RecordedEvent @event)
        {
            _event = @event;
        }

        public long EventNumber => _event.EventNumber;

        public string EventType => _event.EventType;

        public string EventStreamId => _event.EventStreamId;

        public Guid EventId => _event.EventId;

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine($"EventNumber: {_event?.EventNumber}");
            builder.AppendLine($"EventType: {_event?.EventType}");
            builder.AppendLine($"EventStreamId: {_event?.EventStreamId}");
            builder.AppendLine($"EventId: {_event?.EventId}");
            return builder.ToString();
        }
    }
}
