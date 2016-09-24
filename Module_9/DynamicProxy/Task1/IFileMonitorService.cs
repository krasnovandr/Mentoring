using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Task1
{
    public interface IFileMonitorService
    {
        void InitialProcessing();
        void Watcher_Created(object sender, FileSystemEventArgs e);
        void FileProcessing(string file);
        void Start();
        void Stop();
        bool TryOpen(string fileName, int tryCount);
    }
}
