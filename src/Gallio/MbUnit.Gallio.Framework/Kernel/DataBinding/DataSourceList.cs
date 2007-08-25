using System;
using System.Collections.Generic;
using System.Text;

namespace MbUnit.Framework.Kernel.DataBinding
{
    /// <summary>
    /// A data source list contains a list of data sources.
    /// </summary>
    public class DataSourceList : IDataSourceResolver
    {
        private readonly List<IDataSource> sources;

        /// <summary>
        /// Creates an empty list of data sources.
        /// </summary>
        public DataSourceList()
        {
            sources = new List<IDataSource>();
        }

        /// <summary>
        /// Adds a data source to the list.
        /// </summary>
        /// <param name="source">The source to add</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="source"/> is null</exception>
        public void AddDataSource(IDataSource source)
        {
            if (source == null)
                throw new ArgumentNullException("source");

            sources.Add(source);
        }

        /// <inheritdoc />
        public IDataSource ResolveDataSource(string sourceName)
        {
            return sources.Find(delegate(IDataSource source)
            {
                return source.Name == sourceName;
            });
        }
    }
}
