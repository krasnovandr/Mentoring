using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text.RegularExpressions;
using LoggingLib;
using MigraDoc.DocumentObjectModel;
using MigraDoc.Rendering;
using ZXing;

namespace Task1
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
        private readonly TimeSpan _processingTimeout;
        private readonly Regex _fileMask;

        public PdfDocumentManager(string inputDirectory, string resultDirectory, string faultDirectory)
        {
            _inputDirectory = inputDirectory;
            _resultDirectory = resultDirectory;
            _faultDirectory = faultDirectory;
            _sequenceFileNames = new List<string>();
            _sequenceFaulted = false;
            _barcodeReader = new BarcodeReader { AutoRotate = true };
            _processingTimeout = new TimeSpan(0, 0, 1, 10);
            _fileMask = new Regex(@"([A-Za-z0-9])*_\d*\.(jpg|jpeg|png|gif|bmp)");
        }

        [LoggerAspect]
        public void HandleNewFile(FileInfo fileInfo)
        {
            if (Regex.IsMatch(fileInfo.Name, _fileMask.ToString(), RegexOptions.IgnoreCase))
            {
                bool stopImage = false;
                try
                {
                    var bmp = (Bitmap)Image.FromFile(fileInfo.FullName);
                    var result = _barcodeReader.Decode(bmp);
                    stopImage = result != null && result.Text == "New Sequence";
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
        [LoggerAspect]
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
                var documentName = string.Format("result_{0}.pdf", DateTime.Now.ToString("yyyyMMdd_hh_mm_ssfff"));
                var resultpath = Path.Combine(_resultDirectory, documentName);
                render.Save(resultpath);
            }
            else
            {
                CopyToFaultedDirectory();
            }

            _sequenceFaulted = false;
            _sequenceFileNames.Clear();
            _document = null;
        }
        [LoggerAspect]
        private void AddImageToDocument(string fileName)
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


        [LoggerAspect]
        private void CopyToFaultedDirectory()
        {
            foreach (var file in _sequenceFileNames)
            {
                File.Copy(Path.Combine(_inputDirectory, file), Path.Combine(_faultDirectory, file));
            }
        }

        [LoggerAspect]
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
        [LoggerAspect]
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
