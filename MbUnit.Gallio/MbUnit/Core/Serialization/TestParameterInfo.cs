using System;
using System.Xml.Serialization;
using MbUnit.Core.Model;

namespace MbUnit.Core.Serialization
{
    /// <summary>
    /// Describes a test parameter in a portable manner for serialization.
    /// </summary>
    [Serializable]
    [XmlType(Namespace=SerializationUtils.XmlNamespace)]
    public class TestParameterInfo : TestComponentInfo
    {
        private string typeName;
        private int index;

        /// <summary>
        /// Gets or sets the fully-qualified type name of the parameter's value type.  (non-null)
        /// </summary>
        /// <seealso cref="ITestParameter.Type"/>
        [XmlAttribute("type")]
        public string TypeName
        {
            get { return typeName; }
            set { typeName = value; }
        }

        /// <summary>
        /// Gets or sets the index of the parameter.
        /// </summary>
        /// <seealso cref="ITestParameter.Index"/>
        [XmlAttribute("index")]
        public int Index
        {
            get { return index; }
            set { index = value; }
        }
    }
}