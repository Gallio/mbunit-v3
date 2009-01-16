using System;
using System.Collections.Generic;
using System.Text;
using Gallio.Framework.Data;

namespace Gallio.Framework.Pattern
{
    /// <summary>
    /// A test builder applies contributions to a test under construction.
    /// </summary>
    public interface ITestParameterBuilder : ITestComponentBuilder
    {
        /// <summary>
        /// Gets the set of actions that describe the behavior of the test parameter.
        /// </summary>
        PatternTestParameterActions TestParameterActions { get; }

        /// <summary>
        /// Gets or sets the data binder for the parameter.
        /// </summary>
        /// <remarks>
        /// The default value is a <see cref="ScalarDataBinder" /> bound to the anonymous
        /// data source using a <see cref="DataBinding"/> whose path is the name of this parameter and whose
        /// index is the implicit index computed by the pararameter's data context.
        /// </remarks>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="value"/> is null</exception>
        IDataBinder Binder { get; set; }

        /// <summary>
        /// Gets the underlying test parameter.
        /// </summary>
        /// <returns>The underlying test parameter</returns>
        PatternTestParameter ToTestParameter();
    }
}
