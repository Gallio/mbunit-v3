using System;
using System.Xml.Serialization;
using MbUnit.Core.Model;

namespace MbUnit.Core.Serialization
{
    /// <summary>
    /// Describes a test parameter set in a portable manner for serialization.
    /// </summary>
    [Serializable]
    [XmlType(Namespace=SerializationUtils.XmlNamespace)]
    public class TestParameterSetInfo : TestComponentInfo
    {
        private TestParameterInfo[] parameters;

        /// <summary>
        /// Gets or sets the test parameters.  (non-null but possibly empty)
        /// </summary>
        /// <seealso cref="ITestParameterSet.Parameters"/>
        [XmlArray("parameters", IsNullable=false)]
        [XmlArrayItem("parameter", IsNullable=false)]
        public TestParameterInfo[] Parameters
        {
            get { return parameters; }
            set { parameters = value; }
        }
    }
}