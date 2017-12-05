using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using Common;
using Events;

namespace EventHandlers
{
    public class EventHandlerInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(Component.For<IEventHandler<ItemAddedToBasketEvent>>()
                .ImplementedBy<ItemAddedToBasketEventHandler>().LifestyleSingleton());

            container.Register(Component.For<IEventHandler<ItemRemovedFromBasketEvent>>()
                .ImplementedBy<ItemRemovedFromBasketEventHandler>().LifestyleSingleton());

            container.Register(Component.For<IEventStoreEventHandler>()
                .ImplementedBy<ItemAddedTobasketEventStoreHandler>().LifestyleSingleton()
                .Named(new EventHandlerNameGenerator().BuildHandlerName(EventTypes.ITEM_ADDED_TO_BASKET, 1))
                .IsDefault());

            container.Register(Component.For<IEventStoreEventHandler>()
                .ImplementedBy<ItemAddedTobasketEventStoreHandler>().LifestyleSingleton()
                .Named(new EventHandlerNameGenerator().BuildHandlerName(EventTypes.ITEM_REMOVED_FROM_BASKET, 1))
                .IsDefault());
        }
    }
}
