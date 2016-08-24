using System;
using System.IO;
using MigraDoc.DocumentObjectModel;
using MigraDoc.Rendering;

namespace Task1
{
    public static class PdfDocumentManager
    {
        private static Document _document;

        public static void CreateDocument()
        {
            _document = new Document();
        }

        public static void AddImageToDocument(string fileName)
        {
            if (_document == null)
            {
                CreateDocument();
            }

            var section = _document.AddSection();
            var img = section.AddImage(fileName);
            img.Height = _document.DefaultPageSetup.PageHeight;
            img.Width = _document.DefaultPageSetup.PageWidth;

            //section.AddPageBreak();
        }

        public static void RenderDocument(string resultDirectory)
        {
            if (_document == null)
            {
                return;
            }

            var render = new PdfDocumentRenderer
            {
                Document = _document
            };

            render.RenderDocument();
            var documentName = string.Format("result_{0}.pdf", DateTime.Now.ToString("yyyyMMdd_hhmm"));
            var resultpath = Path.Combine(resultDirectory, documentName);
            render.Save(resultpath);

            _document = null;
        }
    }
}
