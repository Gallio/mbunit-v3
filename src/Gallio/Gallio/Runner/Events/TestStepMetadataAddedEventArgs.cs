using System;
using Gallio.Model;
using Gallio.Model.Serialization;
using Gallio.Runner.Reports;

namespace Gallio.Runner.Events
{
    /// <summary>
    /// Arguments for an event raised to indicate that a test step dynamically added metadata to itself.
    /// </summary>
    public sealed class TestStepMetadataAddedEventArgs : TestStepEventArgs
    {
        private readonly string metadataKey;
        private readonly string metadataValue;

        /// <summary>
        /// Initializes the event arguments.
        /// </summary>
        /// <param name="report">The report</param>
        /// <param name="test">The test data</param>
        /// <param name="testStepRun">The test step run</param>
        /// <param name="metadataKey">The metadata key</param>
        /// <param name="metadataValue">The metadata value</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="report>"/>, <paramref name="test"/>
        /// <paramref name="testStepRun"/>, <paramref name="metadataKey"/> or <paramref name="metadataValue" /> is null</exception>
        public TestStepMetadataAddedEventArgs(Report report, TestData test, TestStepRun testStepRun, string metadataKey, string metadataValue)
            : base(report, test, testStepRun)
        {
            if (metadataKey == null)
                throw new ArgumentNullException("metadataKey");
            if (metadataValue == null)
                throw new ArgumentNullException("metadataValue");

            this.metadataKey = metadataKey;
            this.metadataValue = metadataValue;
        }

        /// <summary>
        /// Gets the metadata key.
        /// </summary>
        /// <seealso cref="MetadataKeys"/>
        public string MetadataKey
        {
            get { return metadataKey; }
        }

        /// <summary>
        /// Gets the metadata value.
        /// </summary>
        public string MetadataValue
        {
            get { return metadataValue; }
        }
    }
}