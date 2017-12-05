using System;
using System.Net;
using System.Threading.Tasks;
using Consumer.Configuration;
using Consumer.Plumbing.Factories;
using EventStore.ClientAPI;
using EventStore.ClientAPI.SystemData;

namespace Consumer.EventStore
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
            Console.WriteLine("Handling event");
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
            _eventStoreConnection.Dispose();
        }

        public void Dispose()
        {
            ReleaseUnmanagedResources();
            GC.SuppressFinalize(this);
        }
    }
}
