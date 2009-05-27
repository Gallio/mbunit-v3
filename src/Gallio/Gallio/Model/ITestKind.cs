using System;
using System.Collections.Generic;
using System.Text;
using Gallio.Runtime.Extensibility;

namespace Gallio.Model
{
    /// <summary>
    /// Describes a test kind.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The test kind is ignored by the test runner but it allows tests to be classified
    /// so that a user interface can provide appropriate decorations and other affordances
    /// for any test kinds that it recognizes.
    /// </para>
    /// <para>
    /// The test kind is associated with a test by setting the test's <see cref="MetadataKeys.TestKind"/>
    /// metadata to the id of the test kind component.
    /// </para>
    /// </remarks>
    [Traits(typeof(TestKindTraits))]
    public interface ITestKind
    {
    }
}
