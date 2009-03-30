using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.XPath;

namespace Gallio.Utilities
{
    /// <summary>
    /// Provides utilities for manipulating Xml.
    /// </summary>
    public static class XmlUtils
    {
        /// <summary>
        /// Produces an XPathDocument given an action applied to an XmlWriter.
        /// </summary>
        /// <param name="writeAction">The action to perform</param>
        /// <returns>The XPathDocument</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="writeAction"/> is null</exception>
        public static XPathDocument WriteToXPathDocument(Action<XmlWriter> writeAction)
        {
            if (writeAction == null)
                throw new ArgumentNullException("writeAction");

            XmlWriterSettings xmlWriterSettings = new XmlWriterSettings();
            xmlWriterSettings.CheckCharacters = false;
            xmlWriterSettings.Encoding = Encoding.UTF8;
            xmlWriterSettings.Indent = false;
            xmlWriterSettings.CloseOutput = false;

            MemoryStream stream = new MemoryStream();

            using (XmlWriter xmlWriter = XmlWriter.Create(stream, xmlWriterSettings))
                writeAction(xmlWriter);

            XmlReaderSettings xmlReaderSettings = new XmlReaderSettings();
            xmlReaderSettings.CheckCharacters = false;
            xmlReaderSettings.ValidationType = ValidationType.None;
            xmlReaderSettings.CloseInput = true;

            stream.Position = 0;

            using (XmlReader xmlReader = XmlReader.Create(stream, xmlReaderSettings))
                return new XPathDocument(xmlReader);
        }
    }
}