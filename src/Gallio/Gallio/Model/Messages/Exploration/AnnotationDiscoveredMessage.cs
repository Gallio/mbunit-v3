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
    /// Notifies that an annotation has been discovered by the test exploration process.
    /// </summary>
    [Serializable]
    public class AnnotationDiscoveredMessage : Message
    {
        /// <summary>
        /// Gets or sets the annotation, not null.
        /// </summary>
        public AnnotationData Annotation { get; set; }
        
        /// <inheritdoc />
        public override void Validate()
        {
            ValidationUtils.ValidateNotNull("annotation", Annotation);
        }
    }
}