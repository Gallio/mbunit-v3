using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using Gallio.Utilities;

namespace Gallio.Icarus
{
    [Serializable]
    [XmlRoot("settings", Namespace = XmlSerializationUtils.GallioNamespace)]
    [XmlType(Namespace = XmlSerializationUtils.GallioNamespace)]
    public class UserOptions
    {
        [XmlElement("treeViewCategory")]
        public string TreeViewCategory
        {
            get;
            set;
        }

        [XmlArray("collapsedNodes", IsNullable = false)]
        [XmlArrayItem("collapsedNode", typeof(string), IsNullable = false)]
        public List<string> CollapsedNodes
        {
            get;
            set;
        }
    }
}
