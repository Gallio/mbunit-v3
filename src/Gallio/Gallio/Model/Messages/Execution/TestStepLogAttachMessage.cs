using System;
using System.Collections.Generic;
using System.Text;
using Gallio.Common.Markup;
using Gallio.Common.Validation;
using Gallio.Model.Schema;
using Gallio.Common.Messaging;

namespace Gallio.Model.Messages.Execution
{
    /// <summary>
    /// Notifies that an attachment has been added to a test step log.
    /// </summary>
    [Serializable]
    public class TestStepLogAttachMessage : Message
    {
        /// <summary>
        /// Gets or sets the id of the test step, not null.
        /// </summary>
        public string StepId { get; set; }
        
        /// <summary>
        /// Gets or sets the attachment, not null.
        /// </summary>
        public Attachment Attachment { get; set; }

        /// <inheritdoc />
        public override void Validate()
        {
            ValidationUtils.ValidateNotNull("stepId", StepId);
            ValidationUtils.ValidateNotNull("attachment", Attachment);
        }
    }
}