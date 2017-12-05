using System;
using System.Collections.Generic;
using System.Text;

namespace Consumer.Configuration
{
    public interface IEventStoreHandlerConfiguration
    {
        string AssemblyPath { get; }    
    }

    public class EventStoreHandlerConfiguration : BaseConfiguration, IEventStoreHandlerConfiguration
    {
        public string AssemblyPath =>
            @"D:\Github\EventStoreDotnetCoreConsumer\BasketTrackerEventHandlers\EventHandlers\bin\Debug\netcoreapp2.0\EventHandlers.dll";
    }
}
