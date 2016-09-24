using System.Configuration;
using System.Diagnostics;
using System.IO;
using Castle.DynamicProxy;
using LoggingLib;
using Topshelf;

namespace Task1
{
    static class Program
    {
        static void Main(string[] args)
        {
            var currentDir = Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName);
            var inDir = Path.Combine(currentDir, ConfigurationManager.AppSettings["InputDirectory"]);
            var resultDir = Path.Combine(currentDir, ConfigurationManager.AppSettings["ResultDirectory"]);
            var faultDir = Path.Combine(currentDir, ConfigurationManager.AppSettings["FaultDirectory"]);


            var generator = new ProxyGenerator();
            var test =
                generator.CreateInterfaceProxyWithTarget<IFileMonitorService>(
                    new FileMonitorService(inDir, resultDir, faultDir), new LoggingInterceptor());

         

            HostFactory.Run(
                hostConf => hostConf.Service<IFileMonitorService>(
                    s =>
                    {
                        s.ConstructUsing(() => test);
                        s.WhenStarted(serv => serv.Start());
                        s.WhenStopped(serv => serv.Stop());
                    }));
        }
    }
}
