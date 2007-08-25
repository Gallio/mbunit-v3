using System;
using System.Collections.Generic;
using System.Text;
using MbUnit.Framework.Kernel.Conversions;

namespace MbUnit.Framework.Kernel.DataBinding
{
    /// <summary>
    /// A data binder provides services for performing data binding.
    /// </summary>
    public interface IDataBinder
    {
        /// <summary>
        /// Gets the data binder's converter.
        /// </summary>
        IConverter Converter { get; }

        /// <summary>
        /// Creates a data binding context.
        /// </summary>
        /// <returns>The context</returns>
        IDataBindingContext CreateContext();

        /// <summary>
        /// Obtains an enumeration of data rows that satisfy the request.
        /// </summary>
        /// <param name="set">The data binding request</param>
        /// <param name="resolver">The data source resolver</param>
        /// <returns>The enumeration of data rows</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="set"/>
        /// or <paramref name="resolver"/> is null</exception>
        IEnumerable<IDataRow> Bind(DataBindingSet set, IDataSourceResolver resolver);
    }
}
