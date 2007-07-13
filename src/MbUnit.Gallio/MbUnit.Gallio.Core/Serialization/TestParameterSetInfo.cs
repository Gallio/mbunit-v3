using System;
using System.Xml.Serialization;
using MbUnit.Framework.Model;
using MbUnit.Framework.Utilities;

namespace MbUnit.Core.Serialization
{
    /// <summary>
    /// Describes a test parameter set in a portable manner for serialization.
    /// </summary>
    /// <seealso cref="ITestParameterSet"/>
    [Serializable]
    [XmlType(Namespace=SerializationUtils.XmlNamespace)]
    public class TestParameterSetInfo : TestComponentInfo
    {
        private TestParameterInfo[] parameters;

        /// <summary>
        /// Creates an empty object.
        /// </summary>
        public TestParameterSetInfo()
        {
        }

        /// <summary>
        /// Creates an serializable description of a model object.
        /// </summary>
        /// <param name="obj">The model object</param>
        public TestParameterSetInfo(ITestParameterSet obj)
            : base(obj)
        {
            parameters = ListUtils.ConvertAllToArray<ITestParameter, TestParameterInfo>(obj.Parameters,
                delegate(ITestParameter parameter)
                {
                    return new TestParameterInfo(parameter);
                });
        }

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