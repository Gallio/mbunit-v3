using System;
using Gallio.Framework.Pattern;
using Gallio.Model;
using NBehave.Core;

namespace NBehave.Spec.Framework
{
    /// <summary>
    /// Specifies a description of a concern that is to be exercised by a particular context
    /// or specification.  The concern is presented as metadata in reports.
    /// </summary>
    [AttributeUsage(PatternAttributeTargets.TestComponent, AllowMultiple=true, Inherited=true)]
    public class ConceringAttribute : MetadataPatternAttribute
    {
        private readonly string concern;

        /// <summary>
        /// Associates a concern with a context or specification as metadata.
        /// </summary>
        /// <param name="concern">The description of the concern</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="concern"/> is null</exception>
        public ConceringAttribute(string concern)
        {
            if (concern == null)
                throw new ArgumentNullException("concern");

            this.concern = concern;
        }

        /// <summary>
        /// Gets the description of the concern.
        /// </summary>
        public string Concern
        {
            get { return concern; }
        }

        /// <inheritdoc />
        protected override void Apply(MetadataMap metadata)
        {
            metadata.Add(NBehaveMetadataKeys.Concern, concern);
        }
    }
}
