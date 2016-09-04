using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.IO;
using System.Text.RegularExpressions;
using System.Timers;
using Microsoft.ServiceBus.Messaging;
using MigraDoc.DocumentObjectModel;
using MigraDoc.Rendering;
using ServiceBusClient;
using ZXing;

namespace WorkerService
{
    public class PdfDocumentManager
    {
        private Document _document;
        private readonly string _inputDirectory;
        private readonly string _resultDirectory;
        private readonly string _faultDirectory;
        private bool _sequenceFaulted;
        private readonly List<string> _sequenceFileNames;
        private DateTime _lastProcessedFileTime;
        private string _lastProcessedFile;
        private readonly BarcodeReader _barcodeReader;
        private TimeSpan _processingTimeout;
        private readonly Regex _fileMask;
        private readonly FileQueueServiceBusClient _fileQueueServiceBusClient;
        private readonly StatusQueueServiceBusClient _statusQueueServiceBusClient;
        private readonly TopicServiceBusClient _topicServiceBusClient;
        private string _sequenceDelimeter = "New Sequence";
        public WorkerServiceStates CurrentState { get; set; }

        public Timer Timer;

        public PdfDocumentManager(
            string inputDirectory,
            string resultDirectory,
            string faultDirectory)
        {
            _inputDirectory = inputDirectory;
            _resultDirectory = resultDirectory;
            _faultDirectory = faultDirectory;
            _sequenceFileNames = new List<string>();
            _sequenceFaulted = false;
            _barcodeReader = new BarcodeReader { AutoRotate = true };
            _processingTimeout = new TimeSpan(0, 0, 1, 10);
            _fileMask = new Regex(@"([A-Za-z0-9])*_\d*\.(jpg|jpeg|png|gif|bmp)");

            _fileQueueServiceBusClient = new FileQueueServiceBusClient();
            _fileQueueServiceBusClient.CreateQueue();

            _statusQueueServiceBusClient = new StatusQueueServiceBusClient();
            _statusQueueServiceBusClient.CreateQueue();


            _topicServiceBusClient = new TopicServiceBusClient();
            _topicServiceBusClient.SettingsRecievedEvent += NewSettingsMessage;
            _topicServiceBusClient.StatusRecievedEvent += SendStatus;
            _topicServiceBusClient.CreateTopics();
            _topicServiceBusClient.CreateSubscription(ConfigurationManager.AppSettings["InputDirectory"]);
            CurrentState = WorkerServiceStates.Iddle;
            Timer = new Timer(30000);
            Timer.Elapsed += timer_Elapsed;
        }

        void timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            UpdateStatus();
        }

        private void UpdateStatus()
        {
            _statusQueueServiceBusClient.Send(new WorkerServiceStatus
            {
                ClientName = ConfigurationManager.AppSettings["InputDirectory"],
                CurrentState = CurrentState,
                CurrentSettings = new WorkerServiceSettings
                {
                    BarcodeStopSequence = _sequenceDelimeter,
                    ProcessingTimeout = _processingTimeout
                }
            });
        }

        public void SendStatus(string command)
        {
            UpdateStatus();
        }

        private void NewSettingsMessage(WorkerServiceSettings serviceSettings)
        {
            _processingTimeout = serviceSettings.ProcessingTimeout;
            _sequenceDelimeter = serviceSettings.BarcodeStopSequence;
        }

        public void HandleNewFile(FileInfo fileInfo)
        {
            if (Regex.IsMatch(fileInfo.Name, _fileMask.ToString(), RegexOptions.IgnoreCase))
            {
                CurrentState = WorkerServiceStates.WaitNewFile;
                bool stopImage = false;
                try
                {
                    var bmp = (Bitmap)Image.FromFile(fileInfo.FullName);
                    var result = _barcodeReader.Decode(bmp);
                    stopImage = result != null && result.Text == _sequenceDelimeter;
                }
                catch (Exception exception)
                {
                    _sequenceFaulted = true;
                }


                if (IsNewFileSequence(_lastProcessedFile, fileInfo.Name) || stopImage)
                {
                    RenderDocument();
                    if (stopImage == false)
                    {
                        AddImageToDocument(fileInfo.FullName);
                    }

                }
                else
                {

                    AddImageToDocument(fileInfo.FullName);
                }

                _lastProcessedFile = fileInfo.Name;
                _lastProcessedFileTime = DateTime.Now;
            }
        }

        public void RenderDocument()
        {
            if (_document == null)
            {
                return;
            }

            var render = new PdfDocumentRenderer
            {
                Document = _document
            };

            if (_sequenceFaulted == false)
            {

                render.RenderDocument();
                var stream = new MemoryStream();
                render.PdfDocument.Save(stream, false);
                try
                {
                    CurrentState = WorkerServiceStates.Iddle;
                    _fileQueueServiceBusClient.SendFile(stream);
                }
                catch (Exception ex)
                {

                    throw;
                }
            }
            else
            {
                CopyToFaultedDirectory();
            }

            _sequenceFaulted = false;
            _sequenceFileNames.Clear();
            _document = null;
        }

        public void AddImageToDocument(string fileName)
        {
            if (_document == null)
            {
                _document = new Document();
            }


            if (_sequenceFaulted == false)
            {
                var section = _document.AddSection();
                var img = section.AddImage(fileName);
                img.Height = _document.DefaultPageSetup.PageHeight;
                img.Width = _document.DefaultPageSetup.PageWidth;
            }

            _sequenceFileNames.Add(Path.GetFileName(fileName));


            _lastProcessedFile = fileName;
            _lastProcessedFileTime = DateTime.Now;
        }



        private void CopyToFaultedDirectory()
        {
            foreach (var file in _sequenceFileNames)
            {
                File.Copy(Path.Combine(_inputDirectory, file), Path.Combine(_faultDirectory, file));
            }
        }

        private bool IsNewFileSequence(string lastFile, string newFile)
        {
            if (string.IsNullOrEmpty(lastFile))
            {
                return true;
            }

            var previousNumber = ParsFileNumber(lastFile);
            var newNumber = ParsFileNumber(newFile);

            if ((previousNumber + 1) != newNumber)
            {
                return true;
            }

            if (_lastProcessedFileTime.Add(_processingTimeout) < DateTime.Now)
            {
                return true;
            }

            return false;
        }

        private int ParsFileNumber(string fileName)
        {
            fileName = Path.GetFileNameWithoutExtension(fileName);

            if (string.IsNullOrEmpty(fileName))
            {
                return default(int);
            }

            int number;
            string parsedNumber = fileName.Split('_')[1];
            int.TryParse(parsedNumber, out number);

            return number;
        }
    }
}
