using System;
using System.Collections.Generic;
using System.Text;
using MbUnit.Framework.Kernel.Metadata;

namespace MbUnit.Framework.Kernel.DataBinding
{
    /// <summary>
    /// An array data row stores its factories as an array and allows
    /// its metadata to be updated.
    /// </summary>
    public class ArrayDataRow : IDataRow
    {
        private readonly MetadataMap metadata;
        private readonly IDataFactory[] factories;

        /// <summary>
        /// Creates an array data row.
        /// </summary>
        /// <param name="factories">The row's data factories</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="factories"/> is null</exception>
        public ArrayDataRow(IDataFactory[] factories)
        {
            if (factories == null)
                throw new ArgumentNullException("factories");

            this.factories = factories;
            metadata = new MetadataMap();
        }

        /// <inheritdoc />
        public MetadataMap Metadata
        {
            get { return metadata; }
        }

        /// <inheritdoc />
        public IList<IDataFactory> Factories
        {
            get { return factories; }
        }
    }
}
