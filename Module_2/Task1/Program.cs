using System;
using System.IO;
using System.Xml;
using System.Xml.Schema;

namespace Task1
{
    static class Program
    {
        static void Main(string[] args)
        {
            var settings = new XmlReaderSettings();
            settings.Schemas.Add("http://library.by/catalog", "../../BooksSchema.xsd");
            settings.ValidationEventHandler += settings_ValidationEventHandler;
         
            settings.ValidationFlags = settings.ValidationFlags | XmlSchemaValidationFlags.ReportValidationWarnings;
            settings.ValidationType = ValidationType.Schema;
            
            File.WriteAllText("ErrorLog.txt", String.Empty);
            XmlReader reader = XmlReader.Create("../../../books.xml", settings);

            Console.WriteLine("Validation of correct transfer document:");
            while (reader.Read()) ;

            Console.WriteLine("Start validation of incorrect document all errors will be writed to ErrorLog file");
            XmlReader readerWithErrors = XmlReader.Create("../../booksWithError.xml", settings);

            while (readerWithErrors.Read()) ;
            Console.ReadKey();
        }

        private static void settings_ValidationEventHandler(object sender, ValidationEventArgs e)
        {
            var errorMessage = string.Format("[{0}:{1}] {2}", 
                e.Exception.LineNumber, e.Exception.LinePosition, e.Message);
            errorMessage += Environment.NewLine;
            File.AppendAllText("ErrorLog.txt", errorMessage);
        }
    }
}
