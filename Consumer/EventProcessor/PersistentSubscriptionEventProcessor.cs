//using System;
//using System.Reflection;
//using System.Threading;
//using System.Threading.Tasks;
//using EventStore.ClientAPI;
//using EventStore.ClientAPI.Exceptions;

//namespace Consumer.EventProcessor
//{
//    class PersistentSubscriptionEventProcessor
//    {
//        private static readonly ILog _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

//        private readonly IEventStoreConnectionManager _connectionManager;
//        private readonly IEventAppearedAction _eventAppearedAction;
//        private readonly IPausableServiceActions _serviceActions;
//        private readonly IExecutionTimerFactory _executionTimerFactory;
//        private readonly IErroredEventHandler _erroredEventHandler;
//        private readonly IEventStoreEventFactory _eventStoreEventFactory;
//        private readonly IPersistentSubscriptionConfiguration _persistentSubscriptionConfiguration;
//        private readonly IServiceUnhealthyAction _serviceUnhealthyAction;

//        private EventStorePersistentSubscriptionBase _subscription;
//        private bool _active; //Marker use the trap subscription cancellations that fail.
//        private bool _isHandlingEvent;

//        public PersistentSubscriptionEventProcessor(
//            IEventStoreConnectionManager connectionManager,
//            IEventAppearedAction eventAppearedAction,
//            IPausableServiceActions serviceActions,
//            IExecutionTimerFactory executionTimerFactory,
//            IErroredEventHandler erroredEventHandler,
//            IEventStoreEventFactory eventStoreEventFactory,
//            IPersistentSubscriptionConfiguration persistentSubscriptionConfiguration,
//            IServiceUnhealthyAction serviceUnhealthyAction)
//        {
//            _connectionManager = connectionManager ?? throw new ArgumentNullException(nameof(connectionManager));
//            _eventAppearedAction = eventAppearedAction ?? throw new ArgumentNullException(nameof(eventAppearedAction));
//            _serviceActions = serviceActions ?? throw new ArgumentNullException(nameof(serviceActions));
//            _executionTimerFactory = executionTimerFactory ?? throw new ArgumentException(nameof(executionTimerFactory));
//            _erroredEventHandler = erroredEventHandler ?? throw new ArgumentNullException(nameof(erroredEventHandler));
//            _eventStoreEventFactory = eventStoreEventFactory ?? throw new ArgumentNullException(nameof(eventStoreEventFactory));
//            _persistentSubscriptionConfiguration = persistentSubscriptionConfiguration ?? throw new ArgumentNullException(nameof(persistentSubscriptionConfiguration));
//            _serviceUnhealthyAction = serviceUnhealthyAction ?? throw new ArgumentNullException(nameof(serviceUnhealthyAction));
//        }

//        public void ProcessStream(string eventStoreStream)
//        {
//            _active = true;

//            var subscriptionGroupName = _persistentSubscriptionConfiguration.SubscriptionGroupName;

//            try
//            {
//                BeginSubscription(eventStoreStream, subscriptionGroupName);
//            }
//            catch (Exception ex)
//            {
//                _logger.Error($"Error subscribing to persistent subscription for stream {eventStoreStream} and group {subscriptionGroupName}", ex);
//            }
//        }

//        public void Stop()
//        {
//            try
//            {
//                _active = false; //in case the subscription doesn't die
//                while (_isHandlingEvent)
//                {
//                    _logger.Info("Waiting for service to finish handling event");
//                    Thread.Sleep(1000);
//                }
//                _subscription?.Stop(_persistentSubscriptionConfiguration.SubscriptionStopTimeout);
//            }
//            catch (Exception e)
//            {
//                _logger.Error("Exception caught stopping subscription", e);
//            }
//        }

//        private void BeginSubscription(string eventStoreStream, string subscriptionGroupName)
//        {
//            _subscription = _connectionManager.GetConnectedConnection()
//                .ConnectToPersistentSubscription(eventStoreStream, subscriptionGroupName, EventAppeared, (subscription, dropReason, ex) =>
//                {
//                    if (dropReason != SubscriptionDropReason.UserInitiated)
//                    {
//                        _logger.Warn($"Persistent subscription has been dropped to stream [{eventStoreStream}] and group [{subscriptionGroupName}] with reason: {dropReason}", ex);

//                        _active = false;
//                        _serviceActions.Restart();
//                    }
//                });
//        }

//        private void EventAppeared(EventStorePersistentSubscriptionBase subscription, ResolvedEvent resolvedEvent)
//        {
//            // Guard against slowly dying subscription
//            if (!_active) return;

//            using (_executionTimerFactory.New("EventProcessor.EventAppeared"))
//            {
//                if (_logger.IsDebugEnabled) _logger.Debug($"Event appeared");
//                IEventStoreEvent eventStoreEvent = null;
//                try
//                {
//                    _isHandlingEvent = true;
//                    eventStoreEvent = _eventStoreEventFactory.CreateFrom(resolvedEvent);
//                    if (!eventStoreEvent.IsSystemEvent && !eventStoreEvent.EventDataAvailable)
//                    {
//                        HandleEventStoreErrorAsync(eventStoreEvent, "EventData not available.  Has data that has been projected had it's original source removed?").GetAwaiter().GetResult();
//                    }

//                    if (!eventStoreEvent.IsSystemEvent)
//                    {
//                        _eventAppearedAction.EventAppearedAsync(subscription, eventStoreEvent).GetAwaiter().GetResult();
//                    }
//                    else
//                    {
//                        _logger.Warn("System event received");
//                    }

//                    subscription.Acknowledge(resolvedEvent);
//                }
//                catch (AggregateException ae)
//                {

//                    HandleException(subscription, resolvedEvent, eventStoreEvent, ae);
//                }
//                catch (Exception e)
//                {
//                    HandleException(subscription, resolvedEvent, eventStoreEvent, e);
//                }
//                finally
//                {
//                    _isHandlingEvent = false;
//                }
//            }
//        }

//        private void HandleException(EventStorePersistentSubscriptionBase subscription, ResolvedEvent resolvedEvent, IEventStoreEvent eventStoreEvent, Exception e)
//        {
//            _logger.Fatal($"Fatal error processing event [{eventStoreEvent}]", e);
//            subscription.Fail(resolvedEvent, _persistentSubscriptionConfiguration.NakEventAction, e.Message);

//            //Have to do this here because _serviceActions.Stop() will call the stop method which is relying on the processor to finish handling event which is still true and will not get to true.
//            _isHandlingEvent = false;
//            _serviceActions.Stop();
//        }

//        private void HandleServiceUnhealthException()
//        {
//            //Have to do this here because _serviceUnhealthyAction is likely to call the restart method which is relying on the processor to finish handling event which is still true and will not get to true.
//            _isHandlingEvent = false;
//            _serviceUnhealthyAction.RunAsync().Wait();
//        }

//        private async Task HandleEventStoreErrorAsync(IEventStoreEvent @event, string message)
//        {
//            try
//            {
//                await _erroredEventHandler.HandleAsync(@event, message);
//            }
//            catch (Exception e2)
//            {
//                //Fatal exception - error should have been handled
//                _logger.Fatal($"Exception throw whilst handling event error.  Error handlers should not throw exceptions.", e2);
//                _serviceActions.Stop();
//            }
//        }

//        public void Dispose()
//        {
//            Dispose(true);
//            GC.SuppressFinalize(this);
//        }

//        ~PersistentSubscriptionEventProcessor()
//        {
//            Dispose(false);
//        }

//        private void Dispose(bool disposing)
//        {
//            if (disposing)
//            {
//                if (_subscription != null)
//                {
//                    try
//                    {
//                        _subscription.Stop(_persistentSubscriptionConfiguration.SubscriptionStopTimeout);
//                    }
//                    catch (TimeoutException ex)
//                    {
//                        _logger.Warn("Stopping subscription timed out.", ex);
//                    }
//                    catch (ConnectionClosedException ex)
//                    {
//                        _logger.Warn("Connection was already closed when attempting to stop subscription.", ex);
//                    }
//                    finally
//                    {
//                        _logger.Info("Stopped old subscription");
//                    }
//                }
//                _subscription = null;
//            }
//        }
//    }
//}
