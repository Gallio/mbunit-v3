using System;
using System.Collections.Generic;
using System.Text;
using Gallio.Common.Messaging;
using Gallio.Common.Validation;

namespace Gallio.Model.Isolation.Messages
{
    /// <summary>
    /// Tells the client to run an isolated task.
    /// </summary>
    [Serializable]
    public class RunIsolatedTaskMessage : Message
    {
        /// <summary>
        /// Gets or sets the unique id of the isolated task.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the type of isolated task.
        /// </summary>
        public Type IsolatedTaskType { get; set; }

        /// <summary>
        /// Gets or sets the isolated task arguments.
        /// </summary>
        public object[] Arguments { get; set; }

        /// <inheritdoc />
        public override void Validate()
        {
            if (Id == Guid.Empty)
                throw new ValidationException("Id should be set.");
            ValidationUtils.ValidateNotNull("isolatedTaskType", IsolatedTaskType);
        }
    }
}