using System.Net;

namespace EventProcessor.Configuration
{
    public interface IEventStoreConfiguration
    {
        IPAddress EventStoreIpAddress { get; }
        int EventStorePort { get; }

        string EventStoreUsername { get; }
        string EventStorePassword { get; }
        string EventStoreStreamToProcess { get; }
        string EventStoreGroup { get; }
    }
}
