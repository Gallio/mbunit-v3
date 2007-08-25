using System;
using System.Collections.Generic;
using System.Text;

namespace MbUnit.Framework.Kernel.DataBinding
{
    /// <summary>
    /// A data source resolver locates data sources by name.
    /// </summary>
    public interface IDataSourceResolver
    {
        /// <summary>
        /// Locates a data source by its name.
        /// </summary>
        /// <param name="sourceName">The name of the data source</param>
        /// <returns>The data source, or null if not found</returns>
        IDataSource ResolveDataSource(string sourceName);
    }
}
