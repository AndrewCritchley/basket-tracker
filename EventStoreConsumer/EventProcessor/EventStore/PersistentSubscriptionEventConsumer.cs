using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using EventProcessor.Configuration;
using EventProcessor.Plumbing.Factories;
using EventStore.ClientAPI;
using EventStore.ClientAPI.SystemData;

namespace EventProcessor.EventStore
{
    public interface IEventConsumer
    {
        Task SubscribeToStream(string streamName, string groupName);
    }

    public class PersistentSubscriptionEventConsumer : IEventConsumer, IDisposable
    {
        private readonly IEventStoreConfiguration _eventStoreConfiguration;
        private readonly IEventStoreEventHandlerFactory _eventHandlerFactory;
        private readonly DefaultEventStoreEventFactory _eventStoreEventFactory;
        private readonly IEventStoreConnection _eventStoreConnection;

        /// <summary>
        /// This is just temporary - for benchmarking as I go along
        /// </summary>
        private List<DateTime> _last10EventProcessTimes = new List<DateTime>();

        public PersistentSubscriptionEventConsumer(IEventStoreConfiguration eventStoreConfiguration,
            IEventStoreEventHandlerFactory eventHandlerFactory)
        {
            _eventStoreConfiguration = eventStoreConfiguration;
            _eventHandlerFactory = eventHandlerFactory;
            _eventStoreEventFactory = new DefaultEventStoreEventFactory();

            var connectionSettingsBuilder = ConnectionSettings
                .Create()
                .SetDefaultUserCredentials(new UserCredentials(_eventStoreConfiguration.EventStoreUsername,
                    _eventStoreConfiguration.EventStorePassword));

            _eventStoreConnection = EventStoreConnection.Create(connectionSettingsBuilder,
                new IPEndPoint(_eventStoreConfiguration.EventStoreIpAddress, _eventStoreConfiguration.EventStorePort));
        }

        public async Task SubscribeToStream(string streamName, string groupName)
        {
            await _eventStoreConnection.ConnectAsync();

            // Normally the creating of the subscription group is not done in your general executable code. 
            // Instead it is normally done as a step during an install or as an admin task when setting 
            // things up. You should assume the subscription exists in your code.
            //     await CreateSubscription(connection, streamName, groupName);

           await _eventStoreConnection.ConnectToPersistentSubscriptionAsync(streamName, groupName, EventAppeared);

            Console.WriteLine("Subscribed to stream");
        }

        private async Task EventAppeared(EventStorePersistentSubscriptionBase _, ResolvedEvent e)
        {
            var strParts = new Dictionary<string, string>()
            {
                {"EventType", e.Event.EventType},
                {"EventId", e.OriginalEventNumber.ToString()},
                { "Stream", e.OriginalStreamId }
            };

            _last10EventProcessTimes.Add(DateTime.Now);

            if (_last10EventProcessTimes.Count % 1000 == 0)
            {
                var last10 = _last10EventProcessTimes.OrderByDescending(ee => ee).Take(1000).ToList();
                var first = last10.Min();
                var last = last10.Max();

                var timeForLast10 = last - first;
                Console.WriteLine($"{timeForLast10.TotalSeconds}s per 1000 events");

                _last10EventProcessTimes = new List<DateTime>();
            }

       //     Console.WriteLine(String.Join(", ", strParts.Select(s => $"{s.Key} = {s.Value}")));
            var @event = _eventStoreEventFactory.CreateFrom(e);
            var handler = _eventHandlerFactory.GetHandlerForEvent(@event);
            await handler.HandleAsync(@event);
        }

        private async Task CreateSubscription(IEventStoreConnection connection, string streamName, string groupName)
        {
            Console.WriteLine($"Creating persistent subscription to {streamName} for group {groupName}");
            PersistentSubscriptionSettings settings = PersistentSubscriptionSettings.Create()
                .ResolveLinkTos()
                .StartFromBeginning();

            try
            {
                var userCredentials = new UserCredentials(_eventStoreConfiguration.EventStoreUsername,
                    _eventStoreConfiguration.EventStorePassword);

                await connection.CreatePersistentSubscriptionAsync(streamName, groupName, settings, userCredentials);

                Console.WriteLine($"Created new persistent subscription to {streamName} for group {groupName}");
            }
            catch (AggregateException ex)
            {
                if (ex.InnerException.GetType() != typeof(InvalidOperationException)
                    && ex.InnerException?.Message !=
                    $"Subscription group {groupName} on stream {streamName} already exists")
                {
                    throw;
                }
                else
                {
                    Console.WriteLine($"Persistent subscription to {streamName} for group {groupName} already existed");
                }
            }
            catch (InvalidOperationException ex)
            {
                if (ex.Message !=
                    $"Subscription group {groupName} on stream {streamName} already exists")
                {
                    throw;
                }
                else
                {
                    Console.WriteLine($"Persistent subscription to {streamName} for group {groupName} already existed");
                }
            }
        }

        private void ReleaseUnmanagedResources()
        {
            Console.WriteLine("DISPOSING!!");
            _eventStoreConnection.Dispose();
        }

        public void Dispose()
        {
            ReleaseUnmanagedResources();
            GC.SuppressFinalize(this);
        }
    }
}
