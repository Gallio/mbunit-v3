// Copyright 2008 MbUnit Project - http://www.mbunit.com/
// Portions Copyright 2000-2004 Jonathan De Halleux, Jamie Cansdale
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
using System.Collections.Generic;
using Gallio.Framework.Data.Conversions;

namespace Gallio.Framework.Data.Binders
{
    /// <summary>
    /// <para>
    /// A <see cref="DataBindingContext" /> tracks a list of <see cref="IDataSet"/>s and
    /// <see cref="DataBinding"/>s that are used to produce <see cref="DataBindingItem"/>s.
    /// </para>
    /// </summary>
    /// <remarks>
    /// <para>
    /// In typical usage, a <see cref="IDataBinder" /> registers interest in obtaining
    /// data from a particular <see cref="IDataSet" /> by providing a corresponding <see cref="DataBinding" />.
    /// The <see cref="DataBindingContext" /> then adds the requested <see cref="IDataSet" />
    /// to its <see cref="DataSets" /> list and returns a <see cref="IDataBindingAccessor" />.
    /// When <see cref="GetItems" /> is called, the <see cref="DataBindingContext" /> produces
    /// an enumeration of <see cref="DataBindingItem" />.  Data-bound values can then
    /// be obtained from the <see cref="DataBindingItem" /> via the <see cref="IDataBindingAccessor.GetValue" />
    /// method of each accessor.
    /// </para>
    /// </remarks>
    /// <seealso cref="IDataBinder"/>
    public sealed class DataBindingContext 
    {
        private readonly IConverter converter;
        private readonly JoinedDataSet joinedDataSet;
        private readonly List<DataBinding> translatedBindings;

        /// <summary>
        /// Creates a data binding context.
        /// </summary>
        /// <param name="converter">The converter</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="converter"/> is null</exception>
        public DataBindingContext(IConverter converter)
        {
            if (converter == null)
                throw new ArgumentNullException("converter");

            this.converter = converter;

            joinedDataSet = new JoinedDataSet();
            translatedBindings = new List<DataBinding>();
        }

        /// <summary>
        /// Gets the converter service.
        /// </summary>
        public IConverter Converter
        {
            get { return converter; }
        }

        /// <summary>
        /// <para>
        /// Gets the immutable list of data sets to be enumerated during data binding.
        /// </para>
        /// </summary>
        /// <remarks>
        /// These are the <see cref="IDataSet"/>s from which the raw data for binding will
        /// be retrieved.
        /// </remarks>
        public IList<IDataSet> DataSets
        {
            get { return joinedDataSet.DataSets; }
        }

        /// <summary>
        /// Gets or sets the <see cref="IJoinStrategy"/> to use for combining the
        /// <see cref="DataSets" /> together.
        /// By default the strategy is <see cref="CombinatorialJoinStrategy"/>.
        /// </summary>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="value"/> is null</exception>
        public IJoinStrategy Strategy
        {
            get { return joinedDataSet.Strategy; }
            set { joinedDataSet.Strategy = value; }
        }

        /// <summary>
        /// Registers a data binding for a given data set and adds the data set
        /// to the list of data sets to be enumerated during data binding.
        /// Returns a <see cref="IDataBindingAccessor" /> that may be used to 
        /// retrieve the values associated with the binding.
        /// </summary>
        /// <remarks>
        /// A <see cref="IDataBinder" /> uses this method to register interest
        /// in querying a particular <see cref="IDataSet" />.  It must do so
        /// before <see cref="GetItems" /> is called to ensure that the <see cref="IDataSet"/>
        /// is included in the enumeration.
        /// </remarks>
        /// <param name="dataSet">The data set</param>
        /// <param name="binding">The data binding</param>
        /// <returns>An accessor that returns the values associated with the binding
        /// during data binding operations converted to the requested type</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="dataSet"/> or <paramref name="binding"/> is null</exception>
        public IDataBindingAccessor RegisterBinding(IDataSet dataSet, DataBinding binding)
        {
            if (dataSet == null)
                throw new ArgumentNullException("dataSet");
            if (binding == null)
                throw new ArgumentNullException("binding");

            if (! joinedDataSet.DataSets.Contains(dataSet))
                joinedDataSet.AddDataSet(dataSet);

            DataBinding translatedBinding = joinedDataSet.TranslateBinding(dataSet, binding);
            translatedBindings.Add(translatedBinding);

            return new ConvertedDataBindingAccessor(converter, translatedBinding);
        }

        /// <summary>
        /// <para>
        /// Gets an enumeration of <see cref="DataBindingItem"/>s.
        /// </para>
        /// <para>
        /// The contents of each item may be inspected using a <see cref="IDataBindingAccessor" />
        /// as returned by <see cref="RegisterBinding" />.  When the client is finished with
        /// an item, it should dispose it by calling the <see cref="IDisposable.Dispose" />
        /// method of the <see cref="DataBindingItem" />.
        /// </para>
        /// </summary>
        /// <remarks>
        /// <para>
        /// A client typically registers its <see cref="IDataBinder"/>s with the
        /// <see cref="DataBindingContext"/> then enters a loop to enumerate the items.
        /// For each item, the client will call <see cref="IDataBindingAccessor.GetValue" />
        /// to retrieve the values of interest.  When it is finished with an item, it
        /// disposes it and proceeds on to the next one or stops.  In this manner, a
        /// client may apply multiple <see cref="IDataBinder"/>s within the same
        /// <see cref="DataBindingContext"/>.
        /// </para>
        /// <para>
        /// If no data sets have been registered, this method returns exactly one
        /// data item with a single <see cref="NullDataRow" />.
        /// </para>
        /// </remarks>
        /// <param name="includeDynamicRows">If true, includes rows that may be dynamically
        /// generated in the result set.  Otherwise excludes such rows and only returns
        /// those that are statically known a priori.</param>
        /// <returns>The enumeration of data binding items</returns>
        public IEnumerable<DataBindingItem> GetItems(bool includeDynamicRows)
        {
            if (DataSets.Count == 0)
            {
                yield return new DataBindingItem(NullDataRow.Instance);
            }
            else
            {
                foreach (IDataRow row in joinedDataSet.GetRows(translatedBindings, includeDynamicRows))
                    yield return new DataBindingItem(row);
            }
        }
    }
}
