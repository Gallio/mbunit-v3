using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using MbUnit.Framework.Model;

namespace MbUnit.Core.Serialization
{
    /// <summary>
    /// Describes an entry in a metadata map in a portable manner for serialization.
    /// </summary>
    /// <seealso cref="MetadataMap"/>
    [Serializable]
    [XmlType(Namespace=SerializationUtils.XmlNamespace)]
    public class MetadataMapEntry
    {
        private string key;
        private string[] values;

        /// <summary>
        /// Gets or sets the key.  (non-null)
        /// </summary>
        [XmlAttribute("key")]
        public string Key
        {
            get { return key; }
            set { key = value; }
        }

        /// <summary>
        /// Gets or sets the array of values represented as strings.  (non-null but possibly empty)
        /// </summary>
        [XmlArray("values", IsNullable=false)]
        [XmlArrayItem("value", IsNullable=false)]
        public string[] Values
        {
            get { return values; }
            set { values = value; }
        }
    }
}
