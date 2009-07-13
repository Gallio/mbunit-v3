using System;
using System.Collections.Generic;
using System.Text;
using Gallio.Common.Validation;
using Gallio.Model.Schema;
using Gallio.Common.Messaging;

namespace Gallio.Model.Messages.Execution
{
    /// <summary>
    /// Notifies that a test step has dynamically added metadata to itself.
    /// </summary>
    [Serializable]
    public class TestStepMetadataAddedMessage : Message
    {
        /// <summary>
        /// Gets or sets the id of the test step, not null.
        /// </summary>
        public string StepId { get; set; }
        
        /// <summary>
        /// Gets or sets the metadata key, not null.
        /// </summary>
        /// <seealso cref="MetadataKeys"/>
        public string MetadataKey { get; set; }

        /// <summary>
        /// Gets or sets the metadata value, not null.
        /// </summary>
        public string MetadataValue { get; set; }

        /// <inheritdoc />
        public override void Validate()
        {
            ValidationUtils.ValidateNotNull("stepId", StepId);
            ValidationUtils.ValidateNotNull("metadataKey", MetadataKey);
            ValidationUtils.ValidateNotNull("metadataValue", MetadataValue);
        }
    }
}