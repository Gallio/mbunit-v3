using System;
using System.Collections.Generic;
using System.Text;
using MbUnit.Framework.Kernel.Metadata;

namespace MbUnit.Framework.Kernel.DataBinding
{
    /// <summary>
    /// <para>
    /// A data row yields a collection of data factories that together produce a single
    /// combination of the values provided by that data source.  Metadata may be
    /// associated with a row to describe its contents and purpose.
    /// </para>
    /// <para>
    /// The values themselves may be generated dynamically and bound with a particular
    /// data binding context.  This allows for the creation of data rows consisting of
    /// heavy-weight objects with decommission concerns.
    /// </para>
    /// </summary>
    public interface IDataRow
    {
        /// <summary>
        /// <para>
        /// Gets the metadata associated with the data set, if any.
        /// </para>
        /// <para>
        /// For example, the metadata may contain a description that serves
        /// as documentation of the contents of the data row or of a the test
        /// scenario that is exercised by the contents of the data row.
        /// </para>
        /// </summary>
        MetadataMap Metadata { get; }

        /// <summary>
        /// Gets the factories for the values of the data row.
        /// </summary>
        IList<IDataFactory> Factories { get; }
    }
}
