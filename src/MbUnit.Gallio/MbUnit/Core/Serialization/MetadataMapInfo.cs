using System;
using System.Xml.Serialization;
using MbUnit.Core.Model;

namespace MbUnit.Core.Serialization
{
    /// <summary>
    /// Describes a metadata map in a portable manner for serialization.
    /// </summary>
    /// <seealso cref="MetadataMap"/>
    [Serializable]
    [XmlType(Namespace=SerializationUtils.XmlNamespace)]
    public class MetadataMapInfo
    {
        private MetadataMapEntry[] entries;

        /// <summary>
        /// Gets or sets the array of metadata map entries.  (non-null but possibly empty)
        /// </summary>
        [XmlArray("entries", IsNullable=false)]
        [XmlArrayItem("entry", IsNullable=false)]
        public MetadataMapEntry[] Entries
        {
            get { return entries; }
            set { entries = value; }
        }
    }
}