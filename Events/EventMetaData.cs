using System;
using System.Text;

namespace Events
{
    /// <summary>
    /// Changes to this class should be additive not subtractive as this is a key class
    /// used to decide how to deserialize the data
    /// </summary>
    public class EventMetaData
    {
        /// <summary>
        /// Unique event id for this event.
        /// If using EventStore pass this to the EventData object
        /// </summary>
        public Guid EventId { get; set; }
        /// <summary>
        /// The event id that caused this event.  This causationId could be used for idompotency
        /// </summary>
        public Guid? CausationId { get; set; }
        /// <summary>
        /// The id to correlate events with
        /// This would most likely be the SDCS TransactionId to see the flow
        /// </summary>
        public Guid? CorrelationId { get; set; }
        /// <summary>
        /// Description of the event.
        /// </summary>
        public string EventType { get; set; }
        /// <summary>
        /// The time this event occured
        /// </summary>
        public DateTime EventOccuredAtUtc { get; set; }
        /// <summary>
        /// The event type version
        /// </summary>
        public int EventTypeVersion { get; set; }
        /// <summary>
        /// The number of the event
        /// </summary>
        public long EventNumber { get; set; }
    }

    public static class ByteArrayExtensions
    {
        public static string Decode(this byte[] ba)
        {
            return Encoding.UTF8.GetString(ba);
        }
    }
}
