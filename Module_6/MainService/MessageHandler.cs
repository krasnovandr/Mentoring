using System;
using System.IO;
using System.Threading;
using ServiceBusClient;

namespace MainService
{
    public class MessageHandler
    {
        private Thread workThread;

        private readonly StatusQueueServiceBusClient _statusQueueServiceBusClient;
        private readonly FileQueueServiceBusClient _fileQueueServiceBusClient;
        private readonly TopicServiceBusClient _topicServiceBusClient;
        private readonly ManualResetEvent _stopWorkEvent;
        private readonly FileSystemWatcher _watcher;

        public MessageHandler()
        {
            workThread = new Thread(WorkProcedure);
            _statusQueueServiceBusClient = new StatusQueueServiceBusClient();
            _statusQueueServiceBusClient.CreateQueue();
            _statusQueueServiceBusClient.SettingsRecievedEvent += _statusQueueServiceBusClient_SettingsRecievedEvent;


            _fileQueueServiceBusClient = new FileQueueServiceBusClient();
            _fileQueueServiceBusClient.CreateQueue();

            _topicServiceBusClient = new TopicServiceBusClient();
            _topicServiceBusClient.CreateTopics();
            _stopWorkEvent = new ManualResetEvent(false);

            var file = Path.GetFullPath(ServiceSettings.SettingsFilePath);
            var directoryName = Path.GetDirectoryName(file);
            var fileName = Path.GetFileName(file);
            _watcher = new FileSystemWatcher();
            _watcher.Path = directoryName;
            //_watcher.Filter = "Settings.xml";
            _watcher.Changed += WatcherOnChanged;

        }

        private void WatcherOnChanged(object sender, FileSystemEventArgs fileSystemEventArgs)
        {
            SynchroniseSettings();
        }

        private void SynchroniseSettings()
        {
            var settings = ServiceSettings.ReadSettings();
            _topicServiceBusClient.SendSettings(settings);
            _topicServiceBusClient.GetWorkerServiceStatus();
        }

        void _statusQueueServiceBusClient_SettingsRecievedEvent(WorkerServiceStatus serviceStatus)
        {
            File.WriteAllText(serviceStatus.ClientName + ".txt",
                  serviceStatus.CurrentState.ToString() + Environment.NewLine +
                  serviceStatus.CurrentSettings.BarcodeStopSequence + Environment.NewLine +
                  serviceStatus.CurrentSettings.ProcessingTimeout);
        }



        private void WorkProcedure()
        {
            do
            {
                if (_stopWorkEvent.WaitOne(TimeSpan.Zero))
                {
                    return;
                }

                var stream = _fileQueueServiceBusClient.CheckAndReceiveFile();
                if (stream != null)
                {
                    SaveRecievedFile(stream);
                }
                Thread.Sleep(5000);
            }
            while (true);
        }

        private void SaveRecievedFile(MemoryStream largeMessage)
        {
            var documentName = string.Format("result_{0}.pdf", DateTime.Now.ToString("yyyyMMdd_hh_mm_ssfff"));

            var fileOut = new FileStream(documentName, FileMode.Create);
            largeMessage.CopyTo(fileOut);
            fileOut.Close();
        }


        public void Start()
        {
            _watcher.EnableRaisingEvents = true;
            SynchroniseSettings();
            workThread.Start();
        }


        public void Stop()
        {
            _watcher.EnableRaisingEvents = false;
            _stopWorkEvent.Set();
            workThread.Join();
        }
    }

}
