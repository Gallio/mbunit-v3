using System;
using MbUnit.Framework.Kernel.Attributes;
using MbUnit.Framework.Kernel.Model;

namespace MbUnit.Framework
{
    /// <summary>
    /// Associates a category name with a test fixture, test method, test parameter
    /// or other test component.  The category name can be used to classify tests
    /// and build test suites of related tests.
    /// </summary>
    public class CategoryAttribute : MetadataPatternAttribute
    {
        private readonly string categoryName;

        /// <summary>
        /// Associates a cateogry name with the test component annotated by this attribute.
        /// </summary>
        /// <param name="categoryName">The category name to associate</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="categoryName"/> is null</exception>
        public CategoryAttribute(string categoryName)
        {
            if (categoryName == null)
                throw new ArgumentNullException(@"categoryName");

            this.categoryName = categoryName;
        }

        /// <summary>
        /// Gets or sets the category name.
        /// </summary>
        public string CategoryName
        {
            get { return categoryName; }
        }

        /// <inheritdoc />
        public override void Apply(TemplateTreeBuilder builder, ITemplateComponent component)
        {
            component.Metadata.Entries.Add(MetadataKeys.CategoryName, categoryName);
        }
    }
}