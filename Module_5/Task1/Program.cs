using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Topshelf;

namespace Task1
{
    class Program
    {
        static void Main(string[] args)
        {

            var currentDir = Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName);
            var inDir = Path.Combine(currentDir, "in");
            var outDir = Path.Combine(currentDir, "out");

            HostFactory.Run(
                hostConf => hostConf.Service<FileMonitorService>(
                    s =>
                    {
                        s.ConstructUsing(() => new FileMonitorService(inDir, outDir));
                        s.WhenStarted(serv => serv.Start());
                        s.WhenStopped(serv => serv.Stop());
                    }));
            //.UseNLog(logFactory));
        }
    }
}
