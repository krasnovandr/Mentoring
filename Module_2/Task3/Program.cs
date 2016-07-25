using System.IO;
using System.Xml.Xsl;

namespace Task3
{
    public static class Program
    {
        static void Main()
        {
            var xsl = new XslCompiledTransform();
            var arguments = new XsltArgumentList();
            var settings = new XsltSettings { EnableScript = true };
            arguments.AddExtensionObject("urn:Helpers", new Helpers());

            xsl.Load("../../HTML_Transformator.xslt", settings, null);
            var fs = new FileStream("../../Report.html", FileMode.Create);
            xsl.Transform("../../../books.xml", arguments, fs);
        }
    }
}
