using System;
using System.Linq;
using System.Reflection;
using Castle.Facilities.TypedFactory;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using Consumer.Configuration;
using Consumer.DynamicLoader;
using Consumer.EventStore;
using Consumer.Plumbing.Factories;
using log4net;

namespace Consumer.Plumbing.Installers
{
    public class EventHandlerInstaller : IWindsorInstaller
    {
        private static readonly ILog _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(Component.For<IEventStoreEventHandlerFactory>()
                .AsFactory(c => c.SelectedWith(new DefaultEventStoreEventHandlerSelectorFactory()))
                .LifestyleSingleton());

            container.Register(Component.For<IEventStoreConfiguration>()
                .ImplementedBy<EventStoreConfiguration>().LifestyleSingleton());

            container.Register(Component.For<IEventConsumer>()
                .ImplementedBy<PersistentSubscriptionEventConsumer>().LifestyleSingleton());

            AddHandlersToContainer(container);
        }

        public void AddHandlersToContainer(IWindsorContainer container)
        {
            _logger.Debug("Adding handlers via dynamic loading");
            var asl = new AssemblyLoader();

            var assembliesToLoad = new[] { @"D:\EventHandlers\EventHandlers.dll" };

            foreach (var assemblyPath in assembliesToLoad)
            {
                _logger.Debug($"Loading assembly '{assemblyPath}'");
                var asm = asl.LoadFromAssemblyPath(assemblyPath);

                var installers = asm.GetTypes().Where(e => typeof(IWindsorInstaller).IsAssignableFrom(e));
                _logger.Info($"Found {installers.Count()} installers ({string.Join(",", installers.Select(e => e.Name))})");

                foreach (var installerType in installers)
                {
                    var installerInstance = (IWindsorInstaller)Activator.CreateInstance(installerType);
                    container.Install(installerInstance);
                    _logger.Info($"Installing {installerType.FullName} from '{assemblyPath}'");
                }
            }
        }
    }
}
