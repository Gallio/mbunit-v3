using System;
using System.Xml.Serialization;
using MbUnit.Framework.Model;

namespace MbUnit.Core.Serialization
{
    /// <summary>
    /// Describes a test in a portable manner for serialization.
    /// </summary>
    /// <seealso cref="ITest"/>
    [Serializable]
    [XmlType(Namespace=SerializationUtils.XmlNamespace)]
    public class TestInfo : TestComponentInfo
    {
        /// <summary>
        /// Creates an empty object.
        /// </summary>
        public TestInfo()
        {
        }

        /// <summary>
        /// Creates an serializable description of a model object.
        /// </summary>
        /// <param name="obj">The model object</param>
        public TestInfo(ITest obj)
            : base(obj)
        {
        }
    }
}