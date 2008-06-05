// Copyright 2005-2008 Gallio Project - http://www.gallio.org/
// Portions Copyright 2000-2004 Jonathan de Halleux
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System;
using System.Collections;
using System.Collections.Generic;
using Gallio;

namespace Gallio.Framework.Data
{
    /// <summary>
    /// <para>
    /// A factory data set generates data rows by invoking a factory delegate
    /// and interpreting its output in accordance with the factory kind.
    /// </para>
    /// <para>
    /// Multiple different interpretations are supported.  Refer to the documentation
    /// of the <see cref="FactoryKind" /> enumeration for more details about each kind.
    /// </para>
    /// </summary>
    public class FactoryDataSet : BaseDataSet
    {
        private readonly Func<IEnumerable> factory;
        private readonly FactoryKind factoryKind;
        private readonly int columnCount;

        /// <summary>
        /// Creates a factory data set.
        /// </summary>
        /// <param name="factory">The factory delegate</param>
        /// <param name="factoryKind">The kind of factory</param>
        /// <param name="columnCount">The number of columns in the data rows produced
        /// by the factory or 0 if unknown</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="factory"/> is null</exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="factoryKind"/>
        /// is invalid or if <paramref name="columnCount"/> is less than 0</exception>
        public FactoryDataSet(Func<IEnumerable> factory, FactoryKind factoryKind, int columnCount)
        {
            if (factory == null)
                throw new ArgumentNullException("factory");
            if (! Enum.IsDefined(typeof(FactoryKind), factoryKind))
                throw new ArgumentOutOfRangeException("factoryKind", factoryKind, "Invalid factory kind.");
            if (columnCount < 0)
                throw new ArgumentOutOfRangeException("columnCount", columnCount, "Column count must be non-negative.");

            this.factory = factory;
            this.factoryKind = factoryKind;
            this.columnCount = columnCount;
        }

        /// <inheritdoc />
        public override int ColumnCount
        {
            get { return columnCount; }
        }

        /// <inheritdoc />
        protected override bool CanBindImpl(DataBinding binding)
        {
            return true;
        }

        /// <inheritdoc />
        protected override IEnumerable<IDataRow> GetRowsImpl(ICollection<DataBinding> bindings, bool includeDynamicRows)
        {
            if (!includeDynamicRows)
                yield break;

            IEnumerable enumeration = factory();
            foreach (object element in enumeration)
                foreach (IDataRow dataRow in ProcessElement(element, bindings, factoryKind))
                    yield return dataRow;
        }

        private static IEnumerable<IDataRow> ProcessElement(object element, ICollection<DataBinding> bindings, FactoryKind factoryKind)
        {
            switch (factoryKind)
            {
                case FactoryKind.DataRow:
                    return ProcessDataRow(element);

                case FactoryKind.DataSet:
                    return ProcessDataSet(element, bindings);

                case FactoryKind.ObjectArray:
                    return ProcessObjectArray(element);

                case FactoryKind.Object:
                    return ProcessObject(element);

                case FactoryKind.Auto:
                    return ProcessAuto(element, bindings);

                default:
                    throw new NotSupportedException("Unreachable code.");
            }
        }

        private static IEnumerable<IDataRow> ProcessDataRow(object value)
        {
            IDataRow dataRow = value as IDataRow;
            if (dataRow == null)
                throw new DataBindingException("Expected the factory to produce a data row.");

            return new IDataRow[] { dataRow };
        }

        private static IEnumerable<IDataRow> ProcessDataSet(object value, ICollection<DataBinding> bindings)
        {
            IDataSet dataSet = value as IDataSet;
            if (dataSet == null)
                throw new DataBindingException("Expected the factory to produce a data set.");

            return dataSet.GetRows(bindings, true);
        }

        private static IEnumerable<IDataRow> ProcessObjectArray(object value)
        {
            object[] objectArray = value as object[];
            if (objectArray == null)
                throw new DataBindingException("Expected the factory to produce an object array.");

            return new IDataRow[] { new ListDataRow<object>(objectArray, null, true) };
        }

        private static IEnumerable<IDataRow> ProcessObject(object value)
        {
            return new IDataRow[] { new ScalarDataRow<object>(value, null, true) };
        }

        private static IEnumerable<IDataRow> ProcessAuto(object value, ICollection<DataBinding> bindings)
        {
            if (value != null)
            {
                if (value is IDataSet)
                    return ProcessDataSet(value, bindings);

                if (value is IDataRow)
                    return ProcessDataRow(value);

                if (value is object[])
                    return ProcessObjectArray(value);
            }

            return ProcessObject(value);
        }
    }
}
