using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using Gallio.Collections;

namespace Gallio.Schema.Plugins
{
    /// <summary>
    /// Represents a table of key and value pairs as a sequence of Xml elements whose local name
    /// represents the key and whose text body contains the value.
    /// </summary>
    [Serializable]
    [XmlType(Namespace = SchemaConstants.XmlNamespace)]
    public sealed class KeyValueTable
    {
        private PropertySet propertySet;

        /// <summary>
        /// Creates an empty key value table.
        /// </summary>
        public KeyValueTable()
        {
        }

        /// <summary>
        /// Gets or sets the Xml elements that describe the property entries
        /// during serialization.
        /// </summary>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="value"/> is null</exception>
        [XmlAnyElement]
        public XmlElement[] Entries
        {
            get
            {
                XmlDocument doc = new XmlDocument();
                return GenericUtils.ConvertAllToArray(PropertySet, entry =>
                {
                    XmlElement element = doc.CreateElement(entry.Key, SchemaConstants.XmlNamespace);
                    element.InnerText = entry.Value;
                    return element;
                });
            }
            set
            {
                if (value == null || Array.IndexOf(value, null) >= 0)
                    throw new ArgumentNullException("value");

                PropertySet.Clear();
                foreach (XmlElement element in value)
                {
                    PropertySet.Add(element.LocalName, element.InnerText ?? "");
                }
            }
        }

        /// <summary>
        /// Gets or sets a mutable view of the properties as a property set.
        /// </summary>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="value"/> is null</exception>
        [XmlIgnore]
        public PropertySet PropertySet
        {
            get
            {
                if (propertySet == null)
                    propertySet = new PropertySet();
                return propertySet;
            }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");
                propertySet = value;
            }
        }
    }
}
