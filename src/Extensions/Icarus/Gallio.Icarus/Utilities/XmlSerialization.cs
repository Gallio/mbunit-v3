using System;
using System.Collections.Generic;
using System.Text;
using Gallio.Utilities;

namespace Gallio.Icarus.Utilities
{
    public class XmlSerialization : IXmlSerialization
    {
        public void SaveToXml<T>(T root, string filename)
        {
            XmlSerializationUtils.SaveToXml<T>(root, filename);
        }

        public T LoadFromXml<T>(string filename)
        {
            return XmlSerializationUtils.LoadFromXml<T>(filename);
        }
    }
}
