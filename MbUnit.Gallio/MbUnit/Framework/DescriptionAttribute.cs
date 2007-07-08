using System;
using MbUnit.Core.Metadata;
using MbUnit.Core.Model;
using MbUnit.Framework.Core.Attributes;

namespace MbUnit.Framework
{
    /// <summary>
    /// Associates a description with a test fixture, test method, test parameter
    /// or other test component.  The description provides useful documentation to
    /// users when browsing the tests.
    /// </summary>
    public class DescriptionAttribute : MetadataPatternAttribute
    {
        private string description;

        /// <summary>
        /// Associates a description with the test component annotated by this attribute.
        /// </summary>
        /// <param name="description">The description to associate</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="description"/> is null</exception>
        public DescriptionAttribute(string description)
        {
            if (description == null)
                throw new ArgumentNullException("description");

            this.description = description;
        }

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        public string Description
        {
            get { return description; }
        }

        /// <inheritdoc />
        public override void Apply(TestTemplateTreeBuilder builder, ITestComponent component)
        {
            component.Metadata.Entries.Add(MetadataConstants.DescriptionKey, description);
        }
    }
}
