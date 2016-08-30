using System;
using System.IO;
using System.Threading;
using Microsoft.ServiceBus.Messaging;

namespace WorkerService
{
    public class FileMonitorService
    {
        private readonly FileSystemWatcher _watcher;
        private readonly string _inputDirectory;
        private readonly string _resultDirectory;
        private readonly string _faultDirectory;
        private readonly PdfDocumentManager _pfPdfDocumentManager;
        private readonly ManualResetEvent _stopWorkEvent;


        public FileMonitorService(string inputDirectory, string resultDirectory, string faultDirectory)
        {
            _inputDirectory = inputDirectory;
            _resultDirectory = resultDirectory;
            _faultDirectory = faultDirectory;

            if (!Directory.Exists(inputDirectory))
                Directory.CreateDirectory(inputDirectory);

            if (!Directory.Exists(resultDirectory))
                Directory.CreateDirectory(resultDirectory);


            if (!Directory.Exists(_faultDirectory))
                Directory.CreateDirectory(_faultDirectory);

            _watcher = new FileSystemWatcher(inputDirectory);
            _watcher.Created += Watcher_Created;

            _stopWorkEvent = new ManualResetEvent(false);
            _pfPdfDocumentManager = new PdfDocumentManager(inputDirectory, resultDirectory, faultDirectory);
        }


        private void InitialProcessing()
        {
            //foreach (var file in Directory.EnumerateFiles(_inputDirectory))
            //{
            //    FileProcessing(file);
            //}
        }

        private void Watcher_Created(object sender, FileSystemEventArgs e)
        {
       
            FileProcessing(e.FullPath);
        }

        private void FileProcessing(string file)
        {
            var fileInfo = new FileInfo(file);

            if (_stopWorkEvent.WaitOne(TimeSpan.Zero))
            {
                _pfPdfDocumentManager.RenderDocument();
                return;
            }

            if (TryOpen(file, 3))
                _pfPdfDocumentManager.HandleNewFile(fileInfo);
        }

        public void Start()
        {
            InitialProcessing();
            _watcher.EnableRaisingEvents = true;
        }

        public void Stop()
        {
            _watcher.EnableRaisingEvents = false;

            _stopWorkEvent.Set();
            _pfPdfDocumentManager.RenderDocument();
        }

        private bool TryOpen(string fileName, int tryCount)
        {
            for (int i = 0; i < tryCount; i++)
            {
                try
                {
                    var file = File.Open(fileName, FileMode.Open, FileAccess.Read, FileShare.None);
                    file.Close();

                    return true;
                }
                catch (IOException)
                {
                    Thread.Sleep(5000);
                }
            }

            return false;
        }
    }
}
