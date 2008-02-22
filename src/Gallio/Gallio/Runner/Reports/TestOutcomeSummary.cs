using System;
using System.Xml.Serialization;
using Gallio.Model;
using Gallio.Model.Serialization;

namespace Gallio.Runner.Reports
{
    /// <summary>
    /// Describes the number of test cases with a particular <see cref="TestOutcome"/>.
    /// </summary>
    /// <seealso cref="TestOutcome"/>
    [Serializable]
    [XmlType(Namespace = SerializationUtils.XmlNamespace)]
    public sealed class TestOutcomeSummary
    {
        private TestOutcome outcome;
        private int count;

        /// <summary>
        /// Gets or sets the outcome.
        /// </summary>
        [XmlElement("outcome", IsNullable=false)]
        public TestOutcome Outcome
        {
            get { return outcome; }
            set { outcome = value; }
        }

        /// <summary>
        /// Gets or sets the number of test cases with the specified outcome.
        /// </summary>
        [XmlAttribute("count")]
        public int Count
        {
            get { return count; }
            set { count = value; }
        }
    }
}
