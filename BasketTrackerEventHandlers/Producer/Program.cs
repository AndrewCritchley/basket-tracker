using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using Bogus;
using Common;
using Events;
using EventStore.ClientAPI;
using EventStore.ClientAPI.SystemData;
using Newtonsoft.Json;

namespace Producer
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("****************** STARTING PRODUCER ******************");
            var random = new Random();
            var rollingNumber = 1;

            var connectionSettingsBuilder = ConnectionSettings
                .Create()
                .SetDefaultUserCredentials(new UserCredentials("admin", "changeit"));

            var dictionary = new Dictionary<int, List<string>>();

            using (var connection = EventStoreConnection.Create(connectionSettingsBuilder, new IPEndPoint(IPAddress.Parse("10.0.75.1"), 1113)))
            {
                connection.ConnectAsync().Wait();
                while (true)
                {
                    var customerId = random.Next(0, int.MaxValue);
                    if (random.Next(0, 10) < 5 || !dictionary.Any())
                    {
                        var faker = new Faker<ItemAddedToBasketEvent>()
                            .RuleFor(e => e.ItemName, opt => opt.Lorem.Word())
                            .RuleFor(e => e.CustomerId, opt => customerId)
                            .RuleFor(e => e.CustomEventId, () => rollingNumber++);

                        var events = new[] { faker.Generate() };

                        if (!dictionary.ContainsKey(customerId))
                            dictionary.Add(customerId, new List<string>());

                        dictionary[customerId].Add(events.First().ItemName);

                        var eventData = events.Select(e => ToEventData(Guid.NewGuid(), e, EventTypes.ITEM_ADDED_TO_BASKET)).ToList();
                        var f = connection.AppendToStreamAsync($"customer-{random.Next(0, int.MaxValue)}",
                            ExpectedVersion.Any, eventData).Result;
                    }
                    else
                    {
                        var randomCustomer = dictionary.OrderBy(e => Guid.NewGuid()).First();
                        var randomItem = randomCustomer.Value.OrderBy(e => Guid.NewGuid()).First();
                        randomCustomer.Value.Remove(randomItem);

                        if (!randomCustomer.Value.Any())
                        {
                            dictionary.Remove(randomCustomer.Key);
                        }

                        var faker = new Faker<ItemRemovedFromBasketEvent>()
                            .RuleFor(e => e.ItemName, opt => randomItem)
                            .RuleFor(e => e.CustomerId, opt => randomCustomer.Key)
                            .RuleFor(e => e.CustomEventId, () => rollingNumber++);

                        var events = new[] { faker.Generate() };

                        var eventData = events.Select(e => ToEventData(Guid.NewGuid(), e, EventTypes.ITEM_REMOVED_FROM_BASKET)).ToList();
                        var f = connection.AppendToStreamAsync($"customer-{random.Next(0, int.MaxValue)}",
                            ExpectedVersion.Any, eventData).Result;
                    }

                    Console.WriteLine("Wrote item to event stream" + random.Next(0, int.MaxValue));

                    //    Task.Delay(new TimeSpan(0, 0, 0, 0, 200)).Wait();

                }
            }
        }

        private static EventData ToEventData(Guid eventId, object e, string eventType)
        {
            var metaData = new EventMetaData()
            {
                EventTypeVersion = 1,
                EventType = eventType
            };

            var encodedEvent = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(e));
            var encodedMetadata = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(metaData));
            var typeName = e.GetType().Name;

            return new EventData(eventId, typeName, true, encodedEvent, encodedMetadata);
        }
    }
}
