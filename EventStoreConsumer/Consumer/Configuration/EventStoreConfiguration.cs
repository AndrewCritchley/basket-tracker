using System.Net;
using EventProcessor.Configuration;

namespace Consumer.Configuration
{
    public class EventStoreConfiguration : BaseConfiguration, IEventStoreConfiguration
    {
        public IPAddress EventStoreIpAddress => IPAddress.Parse("10.0.75.1");
        public int EventStorePort => 1113;
        public string EventStoreUsername => "admin";
        public string EventStorePassword => "changeit";
        public string EventStoreStreamToProcess => "$ce-customer";
        public string EventStoreGroup => "group";
    }
}
