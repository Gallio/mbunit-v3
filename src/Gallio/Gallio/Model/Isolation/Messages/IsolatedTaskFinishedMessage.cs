using System;
using System.Collections.Generic;
using System.Text;
using Gallio.Common.Diagnostics;
using Gallio.Common.Messaging;
using Gallio.Common.Validation;

namespace Gallio.Model.Isolation.Messages
{
    /// <summary>
    /// Tells the server that the client has finished processing an isolated task.
    /// </summary>
    [Serializable]
    public class IsolatedTaskFinishedMessage : Message
    {
        /// <summary>
        /// Gets or sets the unique id of the isolated task.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the isolated task result.
        /// </summary>
        public object Result { get; set; }

        /// <summary>
        /// Gets or sets the exception data.
        /// </summary>
        public ExceptionData Exception { get; set; }

        /// <inheritdoc />
        public override void Validate()
        {
            if (Id == Guid.Empty)
                throw new ValidationException("Id should be set.");
        }
    }
}