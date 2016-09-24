using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Task1
{
    public interface IPdfDocumentManager
    {
        void HandleNewFile(FileInfo fileInfo);
        void RenderDocument();
        void AddImageToDocument(string fileName);
        void CopyToFaultedDirectory();
        bool IsNewFileSequence(string lastFile, string newFile);
        int ParsFileNumber(string fileName);
    }
}
