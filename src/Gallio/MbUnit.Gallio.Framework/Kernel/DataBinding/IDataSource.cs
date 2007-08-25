using System;
using System.Collections.Generic;
using System.Text;
using MbUnit.Framework.Kernel.Metadata;

namespace MbUnit.Framework.Kernel.DataBinding
{
    /// <summary>
    /// A data source provides data for bindings.
    /// </summary>
    public interface IDataSource
    {
        /// <summary>
        /// Gets the name of the data source.
        /// </summary>
        string Name { get; }

        IEnumerable<IDataRow> Bind(DataBinding[] bindings, IDataBinder binder, IDataSourceResolver resolver);
    }
}
