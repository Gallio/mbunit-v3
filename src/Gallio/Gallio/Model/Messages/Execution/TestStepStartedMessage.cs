using System;
using System.Collections.Generic;
using System.Text;
using Gallio.Common.Reflection;
using Gallio.Common.Validation;
using Gallio.Model.Schema;
using Gallio.Common.Messaging;

namespace Gallio.Model.Messages.Execution
{
    /// <summary>
    /// Notifies that a test step has started execution and provides its definition.
    /// </summary>
    [Serializable]
    public class TestStepStartedMessage : Message
    {
        [NonSerialized]
        private ICodeElementInfo codeElement;

        /// <summary>
        /// Gets or sets information about the test step that is about to start, not null.
        /// </summary>
        public TestStepData Step { get; set; }
        
        /// <summary>
        /// Gets or sets the code element associated with the test step, or null if none.
        /// </summary>
        public ICodeElementInfo CodeElement
        {
            get { return codeElement; }
            set { codeElement = value; }
        }

        /// <inheritdoc />
        public override void Validate()
        {
            ValidationUtils.ValidateNotNull("step", Step);
        }
    }
}