using System;
using System.Xml.Serialization;
using MbUnit.Core.Model;

namespace MbUnit.Core.Serialization
{
    /// <summary>
    /// Describes a test template in a portable manner for serialization.
    /// </summary>
    /// <seealso cref="ITestTemplate"/>
    [Serializable]
    [XmlType(Namespace=SerializationUtils.XmlNamespace)]
    public class TestTemplateInfo : TestComponentInfo
    {
        private TestTemplateInfo[] children;
        private TestParameterSetInfo[] parameterSets;

        /// <summary>
        /// Gets or sets the children.  (non-null but possibly empty)
        /// </summary>
        /// <seealso cref="ITestTemplate.Children"/>
        [XmlArray("children", IsNullable=false)]
        [XmlArrayItem("child", IsNullable=false)]
        public TestTemplateInfo[] Children
        {
            get { return children; }
            set { children = value; }
        }

        /// <summary>
        /// Gets or sets the parameter sets.  (non-null but possibly empty)
        /// </summary>
        /// <seealso cref="ITestTemplate.ParameterSets"/>
        [XmlArray("parameterSets", IsNullable = false)]
        [XmlArrayItem("parameterSet", IsNullable = false)]
        public TestParameterSetInfo[] ParameterSets
        {
            get { return parameterSets; }
            set { parameterSets = value; }
        }
    }
}