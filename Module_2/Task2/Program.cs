using System.Xml.Xsl;

namespace Task2
{
    static class Program
    {
        static void Main()
        {
            //This url https://validator.w3.org/feed/check.cgi was used for Rss validation
            var xsl = new XslCompiledTransform();
            var settings = new XsltSettings { EnableScript = true };
            xsl.Load("../../RSS_Transformator.xslt", settings, null);

            xsl.Transform("../../../books.xml", "../../RSS.xml");
        }
    }
}
