using System;
using System.Xml;

namespace Gallio.VisualStudio.Tip
{
    internal static class XmlPersistenceUtils
    {
        private const string GallioNamespace = "http://www.gallio.org/";

        public static void SaveToAttribute(XmlElement parent, string name, string value)
        {
            parent.SetAttribute(name, GallioNamespace, value);
        }

        public static void SaveToElement(XmlElement parent, string name, string value)
        {
            XmlElement element = parent.OwnerDocument.CreateElement(name, GallioNamespace);
            element.InnerText = value;
        }

        public static string LoadFromAttribute(XmlElement parent, string name)
        {
            return parent.GetAttribute(name, GallioNamespace);
        }

        public static string LoadFromElement(XmlElement parent, string name)
        {
            XmlNodeList list = parent.GetElementsByTagName(name, GallioNamespace);
            if (list.Count == 0)
                return null;

            XmlElement element = (XmlElement) list.Item(0);
            return element.InnerText;
        }
    }
}
