using System;
using Consumer.Configuration;
using Consumer.EventStore;
using Consumer.Plumbing;

namespace Consumer
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("****************** STARTING Consumer ******************");

            var windsorContainer = WindsorContainerFactory.Create();

            var eventConsumer = windsorContainer.Resolve<IEventConsumer>();
            var eventStoreConfiguration = windsorContainer.Resolve<IEventStoreConfiguration>();

            eventConsumer.SubscribeToStream(eventStoreConfiguration.EventStoreStreamToProcess, eventStoreConfiguration.EventStoreGroup).Wait();

            Console.WriteLine("waiting for events. press enter to exit");
            Console.ReadLine();
        }
    }
}
