using System;
using System.Collections.Generic;
using System.Text;
using Gallio.Common.Validation;
using Gallio.Model.Schema;
using Gallio.Common.Messaging;

namespace Gallio.Model.Messages.Execution
{
    /// <summary>
    /// Notifies that a test step has changed lifecycle phase.
    /// </summary>
    [Serializable]
    public class TestStepLifecyclePhaseChangedMessage : Message
    {
        /// <summary>
        /// Gets or sets the id of the test step, not null.
        /// </summary>
        public string StepId { get; set; }
        
        /// <summary>
        /// Gets or sets the lifecycle phase name, not null.
        /// </summary>
        /// <seealso cref="LifecyclePhases"/>
        public string LifecyclePhase { get; set; }

        /// <inheritdoc />
        public override void Validate()
        {
            ValidationUtils.ValidateNotNull("stepId", StepId);
            ValidationUtils.ValidateNotNull("lifecyclePhase", LifecyclePhase);
        }
    }
}