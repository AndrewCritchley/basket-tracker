using System;
using Consumer.Plumbing;
using EventProcessor.Configuration;
using EventProcessor.EventStore;
using ServiceApi;

namespace Consumer
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("****************** STARTING Consumer ******************");

            var windsorContainer = WindsorContainerFactory.Create();

            var eventStoreConfiguration = windsorContainer.Resolve<IEventStoreConfiguration>();
            var eventConsumer = windsorContainer.Resolve<IEventConsumer>();

            eventConsumer.SubscribeToStream(eventStoreConfiguration.EventStoreStreamToProcess, eventStoreConfiguration.EventStoreGroup).Wait();

            using (var serviceApi = ServiceApiHostBuilder.BuildWebHost(args))
            {
                serviceApi.Start();

                Console.WriteLine("Waiting for events. press enter to exit");
                Console.ReadLine();
            }
        }
    }
}
