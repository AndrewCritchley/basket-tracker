using System;
using Events;

namespace Consumer.Plumbing
{
    /// <summary>
    /// There is a convention that event store handlers will be named in the format specified below.
    /// Handlers should be registered with Castle Windsor using a named instance which reflects this
    /// </summary>
    internal class EventHandlerNameGenerator
    {
        internal string BuildHandlerName(EventMetaData eventMetaData)
        {
            return BuildHandlerName(eventMetaData.EventType, eventMetaData.EventTypeVersion);
        }

        internal string BuildHandlerName(string eventType, int version)
        {
            if (eventType == null) throw new ArgumentNullException(nameof(eventType));
            if (string.IsNullOrWhiteSpace(eventType)) throw new ArgumentException("eventType must be specified", nameof(eventType));
            if (version == 0) throw new ArgumentException("0 is not a valid version", nameof(version));

            return $"{eventType}:{version}";
        }
    }
}
