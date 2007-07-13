using System;
using System.Xml.Serialization;
using MbUnit.Framework.Model;

namespace MbUnit.Core.Serialization
{
    /// <summary>
    /// Describes a test parameter in a portable manner for serialization.
    /// </summary>
    /// <seealso cref="ITestParameter"/>
    [Serializable]
    [XmlType(Namespace=SerializationUtils.XmlNamespace)]
    public class TestParameterInfo : TestComponentInfo
    {
        private string typeName;
        private int index;

        /// <summary>
        /// Creates an empty object.
        /// </summary>
        public TestParameterInfo()
        {
        }

        /// <summary>
        /// Creates an serializable description of a model object.
        /// </summary>
        /// <param name="obj">The model object</param>
        public TestParameterInfo(ITestParameter obj)
            : base(obj)
        {
            typeName = obj.Type.FullName;
            index = obj.Index;
        }

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