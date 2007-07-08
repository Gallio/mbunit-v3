using System;
using MbUnit.Core.Metadata;
using MbUnit.Core.Model;
using MbUnit.Framework.Core.Attributes;

namespace MbUnit.Framework
{
    /// <summary>
    /// Associates the name of the type under test with a test fixture, test method,
    /// test parameter or other test component.  The type under test helps to describe
    /// which type is primarily being exercised by the test so that we can quickly
    /// identify which tests to run after making changes to a given type.
    /// </summary>
    /// <remarks>
    /// This attribute can be repeated multiple times if there are multiple types.
    /// </remarks>
    public class TestsOnAttribute : MetadataPatternAttribute
    {
        private string typeName;

        /// <summary>
        /// Associates the type under test with the test component annotated by this attribute.
        /// </summary>
        /// <param name="type">The type under test</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="type"/> is null</exception>
        public TestsOnAttribute(Type type)
        {
            if (type == null)
                throw new ArgumentNullException("type");

            this.typeName = type.FullName;
        }

        /// <summary>
        /// Associates the fully-qualified name of the type under test with the test component annotated by this attribute.
        /// </summary>
        /// <param name="typeName">The fully-qualified name of the type under test</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="typeName"/> is null</exception>
        public TestsOnAttribute(string typeName)
        {
            if (typeName == null)
                throw new ArgumentNullException("typeName");

            this.typeName = typeName;
        }

        /// <summary>
        /// Gets or sets the fully-qualified name of the type under test.
        /// </summary>
        public string TypeName
        {
            get { return typeName; }
        }

        /// <inheritdoc />
        public override void Apply(TestTemplateTreeBuilder builder, ITestComponent component)
        {
            component.Metadata.Entries.Add(MetadataConstants.TestsOnKey, typeName);
        }
    }
}