using System;
using System.Collections.Generic;
using System.Text;

namespace MbUnit.Core.Metadata
{
    /// <summary>
    /// Provides common metadata constants.
    /// </summary>
    public static class MetadataConstants
    {
        /// <summary>
        /// The metadata key for the author's email.
        /// The associated value should be the author's email as a string.
        /// </summary>
        public const string AuthorEmailKey = "AuthorEmail";

        /// <summary>
        /// The metadata key for the author's name.
        /// The associated value should be the author's name as a string.
        /// </summary>
        public const string AuthorNameKey = "AuthorName";

        /// <summary>
        /// The metadata key for the name of a category to which a test belongs.
        /// The associated value should be the category name as a string.
        /// </summary>
        public const string CategoryNameKey = "CategoryName";

        /// <summary>
        /// The metadata key for the description of a test component.
        /// The associated value should be the description as a string.
        /// </summary>
        public const string DescriptionKey = "Description";

        /// <summary>
        /// The metadata key used to describe the kind of a template.
        /// The associated value should be one of the <see cref="TemplateKind" /> string constants.
        /// </summary>
        public const string TemplateKindKey = "TemplateKind";

        /// <summary>
        /// The metadata key for the name of the type being tested.
        /// The associated value should be the fully qualified name of the type as a string.
        /// </summary>
        public const string TestsOnKey = "TestsOn";
    }
}
