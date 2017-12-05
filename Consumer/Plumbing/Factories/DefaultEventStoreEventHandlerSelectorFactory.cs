using System.Reflection;
using Castle.Facilities.TypedFactory;
using Common;
using Events;
using Newtonsoft.Json;

namespace Consumer.Plumbing.Factories
{
    /// <summary>
    /// Source and release IEventStoreEventHandlers on demand
    /// </summary>
    public interface IEventStoreEventHandlerFactory
    {
        IEventStoreEventHandler GetHandlerForEvent(IEventStoreEvent @event);
        /// <summary>
        /// IMPORTANT: Must be called when a IEventStoreEventHandler has been finished with in order to release
        /// resources.  This includes when exception occur - wrap Get / Use / Release in try / catch / finally
        /// </summary>
        /// <param name="handler"></param>
        void Release(IEventStoreEventHandlerFactory handler);
    }

    /// <summary>
    /// Class used by Castle.Windsor to select an appropriate handler for an event store event
    /// </summary>
    public class DefaultEventStoreEventHandlerSelectorFactory : DefaultTypedFactoryComponentSelector
    {
        private readonly EventHandlerNameGenerator _handlerNameGenerator;

        public DefaultEventStoreEventHandlerSelectorFactory()
        {
            _handlerNameGenerator = new EventHandlerNameGenerator();
        }

        protected override string GetComponentName(MethodInfo method, object[] arguments)
        {
            if (method.Name == "GetHandlerForEvent" && arguments.Length == 1 && arguments[0] is IEventStoreEvent)
            {
                IEventStoreEvent eventStoreEvent = arguments[0] as IEventStoreEvent;
                string metaDataJsonString = eventStoreEvent.MetaData.Decode();
                var eventMetaData = JsonConvert.DeserializeObject<EventMetaData>(metaDataJsonString);
                var handlerRegisteredName = _handlerNameGenerator.BuildHandlerName(eventMetaData);
                return handlerRegisteredName;
            }
            return base.GetComponentName(method, arguments);
        }
    }
}
