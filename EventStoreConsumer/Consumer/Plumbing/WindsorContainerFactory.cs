using System;
using Castle.Facilities.TypedFactory;
using Castle.MicroKernel.Resolvers.SpecializedResolvers;
using Castle.Windsor;
using Castle.Windsor.Installer;
using ConfigurationInstaller = Consumer.Plumbing.Installers.ConfigurationInstaller;

namespace Consumer.Plumbing
{
    internal static class WindsorContainerFactory
    {
        public static IWindsorContainer Container { get; set; }

        internal static IWindsorContainer Create()
        {
            try
            {
                Container = new WindsorContainer();
                Container.Kernel.Resolver.AddSubResolver(new ArrayResolver(Container.Kernel));
                Container.AddFacility<TypedFactoryFacility>();

                Container.Install(new ConfigurationInstaller());

                return Container;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
