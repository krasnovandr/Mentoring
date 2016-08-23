using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MigraDoc.DocumentObjectModel;
using MigraDoc.Rendering;
using Topshelf;

namespace Task1
{
    public class FileMonitorService
    {
        private FileSystemWatcher watcher;
        private string inDir;
        private string outDir;

        private Thread workThread;

        private ManualResetEvent stopWorkEvent;
        private AutoResetEvent newFileEvent;

        public FileMonitorService(string inDir, string outDir)
        {
            this.inDir = inDir;
            this.outDir = outDir;

            if (!Directory.Exists(inDir))
                Directory.CreateDirectory(inDir);

            if (!Directory.Exists(outDir))
                Directory.CreateDirectory(outDir);

            //watcher = new FileSystemWatcher(inDir);
            //watcher.Created += Watcher_Created;

            workThread = new Thread(WorkProcedure);
            stopWorkEvent = new ManualResetEvent(false);
            newFileEvent = new AutoResetEvent(false);
        }

        private void WorkProcedure(object obj)
        {
            do
            {
                foreach (var file in Directory.EnumerateFiles(inDir))
                {
                    if (stopWorkEvent.WaitOne(TimeSpan.Zero))
                        return;

                    var inFile = file;
                    var outFile = Path.Combine(outDir, Path.GetFileName(file));

                    if (TryOpen(inFile, 3))
                        if (!string.IsNullOrEmpty(Path.GetFileName(file)))
                        {
                            var document = new Document();
                            var section = document.AddSection();
                            var img = section.AddImage(file);
                            img.Height = document.DefaultPageSetup.PageHeight;
                            img.Width = document.DefaultPageSetup.PageWidth;

                            section.AddPageBreak();

                            var render = new PdfDocumentRenderer();
                            render.Document = document;

                            render.RenderDocument();
                            render.Save("result.pdf");
                            File.Delete(file);
                        }
                    
                }
                Thread.Sleep(1000);
            } while (true);
        }

        //private void Watcher_Created(object sender, FileSystemEventArgs e)
        //{
        //    newFileEvent.Set();
        //}

        public void Start()
        {
            workThread.Start();
            //watcher.EnableRaisingEvents = true;
        }

        public void Stop()
        {
            //watcher.EnableRaisingEvents = false;
            stopWorkEvent.Set();
            workThread.Join();
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
