using System;
using System.Collections.Generic;
using System.Text;
using MbUnit.Core.Serialization;

namespace MbUnit.Core.Model
{
    /// <summary>
    /// The root interface of the test object model.
    /// All components have a name for presentation, metadata for
    /// annotations, and a code reference to its point of definition. 
    /// </summary>
    public interface ITestComponent
    {
        /// <summary>
        /// Gets or sets the stable unique identifier of the component.
        /// </summary>
        /// <remarks>
        /// The identifier must be unique across all components
        /// within a given test project.  It should also be stable so that the
        /// identifier remains valid across recompilations and code changes that
        /// do not alter the underlying declarations (insofar as is possible).
        /// The identifier is used to refer to test components remotely from
        /// a test runner and persistently in test projects and reports.
        /// Normally the identifier is assigned automatically by the <see cref="TestTemplateTreeBuilder" />.
        /// </remarks>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="value"/> is null</exception>
        string Id { get; set; }

        /// <summary>
        /// Gets or sets the name of the component.
        /// </summary>
        /// <remarks>
        /// The name does not need to be globally unique.
        /// </remarks>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="value"/> is null</exception>
        string Name { get; set; }

        /// <summary>
        /// Gets the metadata of the component.
        /// </summary>
        MetadataMap Metadata { get; }

        /// <summary>
        /// Gets or sets a reference to the point of definition of this test
        /// component in the code.
        /// </summary>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="value"/> is null</exception>
        CodeReference CodeReference { get; set; }

        /// <summary>
        /// Gets a serializable description of the component.
        /// </summary>
        /// <returns>The component info</returns>
        TestComponentInfo ToInfo();
    }
}
