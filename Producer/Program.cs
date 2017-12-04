using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
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

            using (var connection = EventStoreConnection.Create(connectionSettingsBuilder, new IPEndPoint(IPAddress.Parse("10.0.75.1"), 1113)))
            {
                connection.ConnectAsync().Wait();
                while (true)
                {
                    Task.Delay(new TimeSpan(0, 0, 1)).Wait();

                    var commitMetadata = new Dictionary<string, object> { { "CommitId", Guid.NewGuid() } };
                    var events = new[]
                    {
                        new ItemAddedToBasketEvent()
                        {
                            ItemName =  "Chocolate",
                            ItemId = random.Next(0, int.MaxValue),
                            CustomEventId = rollingNumber++
                        }
                    };

                    var eventData = events.Select(e => ToEventData(Guid.NewGuid(), e, commitMetadata)).ToList();
                    var f = connection.AppendToStreamAsync($"customer-{random.Next(0, int.MaxValue)}", ExpectedVersion.Any, eventData).Result;
                }
            }
        }

        private static EventData ToEventData(Guid eventId, object e, IDictionary<string, object> metadata)
        {
            var encodedEvent = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(e));
            var encodedMetadata = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(metadata));
            var typeName = e.GetType().Name;

            return new EventData(eventId, typeName, true, encodedEvent, encodedMetadata);
        }
    }
}
