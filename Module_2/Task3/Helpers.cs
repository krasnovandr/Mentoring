using System.Linq;
using System.Xml;
using System.Xml.XPath;

namespace Task3
{
    public class Helpers
    {
        public XPathNodeIterator Distinct(XPathNodeIterator iterator)
        {
            var stringsToReturn = iterator.OfType<XPathNavigator>().Select(m => m.Value);
            string[] items = stringsToReturn.Distinct().ToArray();

            var doc = new XmlDocument();
            doc.AppendChild(doc.CreateElement("root"));
            using (XmlWriter writer = doc.DocumentElement.CreateNavigator().AppendChild())
            {
                foreach (string item in items)
                {
                    writer.WriteElementString("item", item);
                }
            }
            return doc.DocumentElement.CreateNavigator().Select("item");
        }
    }
}
