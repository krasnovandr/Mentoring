using System.Configuration;
using System.Diagnostics;
using System.IO;
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


            IocContainerFactory.Create();

            HostFactory.Run(
                hostConf => hostConf.Service<IFileMonitorService>(
                    s =>
                    {
                        s.ConstructUsing(() => new FileMonitorService(inDir, resultDir, faultDir));
                        s.WhenStarted(serv => serv.Start());
                        s.WhenStopped(serv => serv.Stop());
                    }));
        }
    }
}
