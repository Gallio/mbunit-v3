using System;

namespace MbUnit.Core.Metadata
{
    /// <summary>
    /// Specifies the kind of a test template.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The template kind is not significant to the MbUnit test runner.  Instead
    /// it provides a loose classification that may be used to provide appropriate
    /// decorations when presenting the template in a user interface.
    /// </para>
    /// <para>
    /// If none of the built-in kinds are appropriate, you may use the
    /// <see cref="Custom" /> kind or invent one of your own to present
    /// in the user interface (albeit perhaps without special affordances.)
    /// </para>
    /// </remarks>
    /// <seealso cref="MetadataConstants.TemplateKindKey"/>
    public static class TemplateKind
    {
        /// <summary>
        /// The template is the root of the template tree.
        /// </summary>
        public const string Root = "Root";

        /// <summary>
        /// The template encloses all contributions offered by a given test framework.
        /// </summary>
        public const string Framework = "Framework";

        /// <summary>
        /// The template describes a grouping of templates for descriptive purposes.
        /// </summary>
        public const string Group = "Group";

        /// <summary>
        /// The template describes a test suite.
        /// </summary>
        public const string Suite = "Suite";

        /// <summary>
        /// The template describes a test fixture.
        /// </summary>
        public const string Fixture = "Fixture";

        /// <summary>
        /// The template describes a test.
        /// </summary>
        public const string Test = "Test";

        /// <summary>
        /// The template is of some other unspecified kind.
        /// </summary>
        /// <remarks>
        /// If none of the built-in kinds are appropriate, you may use the
        /// <see cref="Custom" /> kind or invent one of your own to present
        /// in the user interface (albeit perhaps without special affordances.)
        /// </remarks>
        public const string Custom = "Custom";
    }
}
