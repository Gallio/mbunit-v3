using System;
using System.Collections.Generic;
using System.Text;
using Gallio.Framework.Data;

namespace Gallio.Framework.Pattern
{
    /// <summary>
    /// A test data context builder applies contributions to a test data context under construction.
    /// </summary>
    public interface ITestDataContextBuilder
    {
        /// <summary>
        /// Defines a new data source within this data context if one does not exist.
        /// Otherwise returns the existing one.
        /// </summary>
        /// <param name="name">The data source name</param>
        /// <returns>The defined data source</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="name"/> is null</exception>
        DataSource DefineDataSource(string name);

        /// <summary>
        /// Creates a child data context.
        /// </summary>
        /// <returns>The builder for the child context</returns>
        ITestDataContextBuilder CreateChild();

        /// <summary>
        /// <para>
        /// Gets or sets the offset to add to a test parameter's implicit data binding index to map it
        /// into the containing data context.
        /// </para>
        /// <para>
        /// This property is used to determine the data binding index of a test parameter that
        /// has not been explicitly bound.  The offsets are summed cumulatively to produce an implicit
        /// data binding index while traversing the chain of containing data contexts while locating the
        /// first anonymous data source.
        /// </para>
        /// <para>
        /// For example, suppose <c>Y</c> is the second parameter of a test method.  If <c>Y</c> does not
        /// have an explicit data binding, we will apply implicit data binding rules as follows.
        /// <list type="bullet">
        /// <item>If <c>Y</c>'s data context contains an anonymous data source, then the implicit data binding index will be 0.</item>
        /// <item>Otherwise, if <c>Y</c>'s containing data context contains an anonymous data source, then the implicit data
        /// binding index will equal the offset specified in <c>Y</c>'s data context: 1 (since it is the second parameter).</item>
        /// <item>Otherwise, we continue searching containing data contexts and summing their offsets until we find an
        /// anonymous data source.  If none is found, then data binding will fail.</item>
        /// </list>
        /// </para>
        /// </summary>
        int ImplicitDataBindingIndexOffset { get; set; }

        /// <summary>
        /// Gets the underlying pattern test data context.
        /// </summary>
        /// <returns>The underlying pattern test data context</returns>
        PatternTestDataContext ToPatternTestDataContext();
    }
}
