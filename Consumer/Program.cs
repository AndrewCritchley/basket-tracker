using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using Events;
using EventStore.ClientAPI;
using EventStore.ClientAPI.SystemData;
using Newtonsoft.Json;

namespace Consumer
{
    class Program
    {
        public static string GROUP = "group";
        public static string STREAM = "$ce-customer";

        static void Main(string[] args)
        {
            Console.WriteLine("****************** STARTING Consumer ******************");

            var connectionSettingsBuilder = ConnectionSettings
                .Create()
                .SetDefaultUserCredentials(new UserCredentials("admin", "changeit"));

            var consumerNumber = new Random().Next(100, 999);

            using (var connection = EventStoreConnection.Create(connectionSettingsBuilder, new IPEndPoint(IPAddress.Parse("10.0.75.1"), 1113)))
            {
                connection.ConnectAsync().Wait();

                //Normally the creating of the subscription group is not done in your general executable code. 
                //Instead it is normally done as a step during an install or as an admin task when setting 
                //things up. You should assume the subscription exists in your code.
                CreateSubscription(connection);

                connection.ConnectToPersistentSubscription(STREAM, GROUP, (_, x) =>
                {
                    var data = Encoding.ASCII.GetString(x.Event.Data);
                    var itemAddedToBasket = JsonConvert.DeserializeObject<ItemAddedToBasketEvent>(data);
                    Console.WriteLine($"[{consumerNumber}] received event ID {itemAddedToBasket.CustomEventId}");
                });

                Console.WriteLine("waiting for events. press enter to exit");
                Console.ReadLine();
            }
        }

        private static void CreateSubscription(IEventStoreConnection conn)
        {
            PersistentSubscriptionSettings settings = PersistentSubscriptionSettings.Create()
        .ResolveLinkTos()
                .StartFromCurrent();

            try
            {
                conn.CreatePersistentSubscriptionAsync(STREAM, GROUP, settings, new UserCredentials("admin", "changeit")).Wait();
            }
            catch (AggregateException ex)
            {
                if (ex.InnerException.GetType() != typeof(InvalidOperationException)
                    && ex.InnerException?.Message != $"Subscription group {GROUP} on stream {STREAM} already exists")
                {
                    throw;
                }
            }
        }
    }
}
