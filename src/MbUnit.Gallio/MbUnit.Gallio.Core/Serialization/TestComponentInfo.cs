using System;
using System.Xml.Serialization;
using MbUnit.Framework.Kernel.Model;

namespace MbUnit.Core.Serialization
{
    /// <summary>
    /// Describes a test model component in a portable manner for serialization.
    /// </summary>
    /// <seealso cref="ITestComponent"/>
    [Serializable]
    [XmlType(Namespace = SerializationUtils.XmlNamespace)]
    public class TestComponentInfo : ModelComponentInfo
    {
        /// <summary>
        /// Creates an empty object.
        /// </summary>
        public TestComponentInfo()
        {
        }

        /// <summary>
        /// Creates an serializable description of a model object.
        /// </summary>
        /// <param name="obj">The model object</param>
        public TestComponentInfo(ITestComponent obj) : base(obj)
        {
        }
    }
}