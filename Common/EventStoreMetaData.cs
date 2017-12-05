using System;
using System.Text;
using EventStore.ClientAPI;

namespace Common
{
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
