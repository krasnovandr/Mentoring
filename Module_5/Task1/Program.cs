using System.Diagnostics;
using System.IO;
using Topshelf;

namespace Task1
{
    class Program
    {
        static void Main(string[] args)
        {

            var currentDir = Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName);
            var inDir = Path.Combine(currentDir, "in");
            var resultDir = Path.Combine(currentDir, "Result");

            HostFactory.Run(
                hostConf => hostConf.Service<FileMonitorService>(
                    s =>
                    {
                        s.ConstructUsing(() => new FileMonitorService(inDir, resultDir));
                        s.WhenStarted(serv => serv.Start());
                        s.WhenStopped(serv => serv.Stop());
                    }));
            //.UseNLog(logFactory));
        }
    }
}
