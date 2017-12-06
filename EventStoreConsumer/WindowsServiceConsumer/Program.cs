using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using ServiceApi;

namespace WindowsServiceConsumer
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {
            using (var serviceApi = ServiceApiHostBuilder.BuildWebHost(null))
            {
                serviceApi.Start();

                Console.WriteLine("Waiting for events. press enter to exit");
                Console.ReadLine();
                while (true)
                {
                    //HACK; System.ReadLine() doesn't have execution here.
                }
            }

            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[]
            {
                new Service1()
            };
            ServiceBase.Run(ServicesToRun);
        }
    }
}
