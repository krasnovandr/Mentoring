using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;

namespace Task1
{
    public class FileMonitorService
    {
        private FileSystemWatcher watcher;
        private string _inputDirectory;
        private readonly string _resultDirectory;

        //private Thread workThread;

        private ManualResetEvent stopWorkEvent;
        private AutoResetEvent newFileEvent;
        private Regex fileMask;
        private string lastProcessedFile;
        private DateTime lastProcessedFileTime;
        private TimeSpan processingTimeout;

        public FileMonitorService(string inputDirectory, string resultDirectory)
        {
            this._inputDirectory = inputDirectory;
            this._resultDirectory = resultDirectory;

            if (!Directory.Exists(inputDirectory))
                Directory.CreateDirectory(inputDirectory);

            if (!Directory.Exists(resultDirectory))
                Directory.CreateDirectory(resultDirectory);

            watcher = new FileSystemWatcher(inputDirectory);
            watcher.Created += Watcher_Created;

            //workThread = new Thread(WorkProcedure);
            stopWorkEvent = new ManualResetEvent(false);
            newFileEvent = new AutoResetEvent(false);
            fileMask = new Regex(@"([A-Za-z0-9])*_\d*.(jpg|jpeg|png|gif|bmp)");
            lastProcessedFile = string.Empty;
            lastProcessedFileTime = new DateTime();
            processingTimeout = new TimeSpan(0, 0, 0, 10);
        }

        //private void WorkProcedure(object obj)
        //{
        //    do
        //    {
        //        foreach (var file in Directory.EnumerateFiles(inputDirectory))
        //        {
        //            if (stopWorkEvent.WaitOne(TimeSpan.Zero))
        //                return;

        //            var outputFilePath = Path.Combine(resultDirectory, Path.GetFileName(file));

        //            if (TryOpen(file, 3))
        //            {
        //                if (!string.IsNullOrEmpty(Path.GetFileName(file)) &&
        //                    Regex.IsMatch(file, fileMask.ToString(), RegexOptions.IgnoreCase))
        //                {


        //                    if (false)
        //                    {
        //                        PdfDocumentManager.CreateDocument();
        //                    }

        //                    File.Copy(file, outputFilePath);
        //                    PdfDocumentManager.CreateDocument();
        //                    PdfDocumentManager.AddImageToDocument(file);
        //                    File.Delete(file);
        //                }
        //            }
        //        }
        //    } while (WaitHandle.WaitAny(new WaitHandle[] { stopWorkEvent, newFileEvent }, 1000) !=0 );
        //}

        private void Watcher_Created(object sender, FileSystemEventArgs e)
        {
            FileInfo file = new FileInfo(e.FullPath);


            if (!string.IsNullOrEmpty(Path.GetFileName(file.Name)) &&
                Regex.IsMatch(file.Name, fileMask.ToString(), RegexOptions.IgnoreCase))
            {
                //todo determine is it new sequence

                if (IsNewFileSequence(lastProcessedFile, file.Name))
                {
                    PdfDocumentManager.RenderDocument(_resultDirectory);
                    PdfDocumentManager.AddImageToDocument(file.FullName);
                }
                else
                {
                    PdfDocumentManager.AddImageToDocument(file.FullName);
                }
            }

            lastProcessedFile = file.Name;
            lastProcessedFileTime = DateTime.Now;
            //newFileEvent.Set();
        }

        private bool IsNewFileSequence(string lastFile, string newFile)
        {
            //is first file
            if (string.IsNullOrEmpty(lastFile))
            {
                return true;
            }

            var previousNumber = ParsFileNumber(lastFile);
            var newNumber = ParsFileNumber(newFile);

            if ((previousNumber + 1) != newNumber )
            {
                return true;
            }

            if (lastProcessedFileTime.Add(processingTimeout) < DateTime.Now)
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

        public void Start()
        {
            //workThread.Start();
            watcher.EnableRaisingEvents = true;
        }

        public void Stop()
        {
            watcher.EnableRaisingEvents = false;
            PdfDocumentManager.RenderDocument(_resultDirectory);
            //stopWorkEvent.Set();
            //workThread.Join();
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
