using System;
using System.Collections.Generic;
using System.Text;
using Gallio.Common.Reflection;
using Gallio.Common.Validation;
using Gallio.Model.Schema;
using Gallio.Common.Messaging;

namespace Gallio.Model.Messages.Exploration
{
    /// <summary>
    /// Notifies that a test has been discovered by the test exploration process.
    /// </summary>
    [Serializable]
    public class TestDiscoveredMessage : Message
    {
        /// <summary>
        /// Gets or sets the id of the parent test, or null if the test is the root.
        /// </summary>
        public string ParentTestId { get; set; }

        /// <summary>
        /// Gets or sets information about the test that was discovered, not null.
        /// </summary>
        public TestData Test { get; set; }

        /// <inheritdoc />
        public override void Validate()
        {
            ValidationUtils.ValidateNotNull("test", Test);
        }
    }
}