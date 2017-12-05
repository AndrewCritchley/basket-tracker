using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using Microsoft.Extensions.DependencyModel;

namespace Consumer.DynamicLoader
{
    public class AssemblyLoader : AssemblyLoadContext
    {
        protected override Assembly Load(AssemblyName assemblyName)
        {
            //var dependencies = DependencyContext.Default;

            //var res = dependencies.CompileLibraries.Where(d => d.Name.Contains(assemblyName.Name)).ToList();
            //var assembly = Assembly.Load(new AssemblyName(res.First().Name));
            //return assembly;

            return Load(assemblyName.Name);
        }

        public static Assembly Load(string assemblyName)
        {
            var assemblyFullPath =
                $@"D:\Github\EventStoreDotnetCoreConsumer\BasketTrackerEventHandlers\EventHandlers\bin\Debug\netstandard2.0\{
                        assemblyName
                    }.dll";

            var fileNameWithOutExtension = Path.GetFileNameWithoutExtension(assemblyFullPath);

            var inCompileLibraries = DependencyContext.Default.CompileLibraries.Any(l => l.Name.Equals(fileNameWithOutExtension, StringComparison.OrdinalIgnoreCase));
            var inRuntimeLibraries = DependencyContext.Default.RuntimeLibraries.Any(l => l.Name.Equals(fileNameWithOutExtension, StringComparison.OrdinalIgnoreCase));

            try
            {
                var assembly = (inCompileLibraries || inRuntimeLibraries)
                    ? Assembly.Load(new AssemblyName(fileNameWithOutExtension))
                    : AssemblyLoadContext.Default.LoadFromAssemblyPath(assemblyFullPath);

                return assembly;
            }
            catch
            {
                return null;
            }
        }
        
    }
}
