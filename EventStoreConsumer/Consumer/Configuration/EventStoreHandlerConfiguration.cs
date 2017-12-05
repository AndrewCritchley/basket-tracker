using EventProcessor.Configuration;

namespace Consumer.Configuration
{
    public class EventStoreHandlerConfiguration : BaseConfiguration, IEventStoreHandlerConfiguration
    {
        public string AssemblyPath =>
            @"D:\Github\EventStoreDotnetCoreConsumer\BasketTrackerEventHandlers\EventHandlers\bin\Debug\netstandard2.0\EventHandlers.dll";
    }
}
