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
using Gallio.Collections;

namespace Gallio.Data
{
    /// <summary>
    /// <para>
    /// A joined data set is an aggregate data set that joins rows from each of zero or more
    /// other data sets according to a <see cref="IJoinStrategy"/>.
    /// </para>
    /// <para>
    /// A joined data set supports queries with two kinds of <see cref="DataBinding"/>:
    /// <list type="bullet">
    /// <item>A translated binding produced by <see cref="TranslateBinding" /> is scoped
    /// to a particular <see cref="IDataSet" />.  When a query occurs using a translated
    /// binding, only that <see cref="IDataSet" /> and its <see cref="IDataRow" />
    /// components are consulted.</item>
    /// <item>Any other binding is treated as if it referred to the joined <see cref="IDataRow" />
    /// including all of the contributions of all data sets.  The joined <see cref="IDataRow" />
    /// is conceptually laid out such that the columns of first <see cref="IDataSet"/> appear
    /// first followed by those of successive <see cref="IDataSet"/>s in order.  To maintain
    /// this illustion, the <see cref="DataBinding.Index" /> component of the binding is adjusted
    /// internally before passing any queries on to the <see cref="IDataSet"/>s.  Thereafter
    /// a binding will be consumed by the first <see cref="IDataSet"/> that returns <c>true</c>
    /// from its <see cref="IDataSet.CanBind" /> method after index-adjustment.
    /// </item>
    /// </list>
    /// </para>
    /// </summary>
    public class JoinedDataSet : AggregateDataSet
    {
        private readonly Dictionary<IDataSet, DataSetInfo> lookupTable = new Dictionary<IDataSet, DataSetInfo>();

        private IJoinStrategy strategy = CombinatorialJoinStrategy.Instance;
        private int columnCount;

        /// <summary>
        /// Gets or sets the <see cref="IJoinStrategy"/>.
        /// By default the strategy is <see cref="CombinatorialJoinStrategy"/>.
        /// </summary>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="value"/> is null</exception>
        public IJoinStrategy Strategy
        {
            get { return strategy; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");
                strategy = value;
            }
        }

        /// <inheritdoc />
        public override int ColumnCount
        {
            get { return columnCount; }
        }

        /// <inheritdoc />
        public override void AddDataSet(IDataSet dataSet)
        {
            base.AddDataSet(dataSet);

            lookupTable.Add(dataSet, new DataSetInfo(this, lookupTable.Count, columnCount));
            columnCount += dataSet.ColumnCount;
        }

        /// <summary>
        /// <para>
        /// Translates a binding into one that expresses a query that is scoped over
        /// a particular data set that is associated with this interface.
        /// </para>
        /// <para>
        /// If the binding contains an index parameter, the translated binding will contain
        /// an index that is offset based on the position of the contents of the data set
        /// within the rows of the joined aggregate.
        /// </para>
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <param name="dataSet">The data set</param>
        /// <param name="binding">The binding</param>
        /// <returns>The translated binding</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="dataSet"/>
        /// or <paramref name="binding"/> is null</exception>
        /// <exception cref="ArgumentException">Thrown if <paramref name="dataSet"/>
        /// is not a member of this instance</exception>
        public DataBinding TranslateBinding(IDataSet dataSet, DataBinding binding)
        {
            if (dataSet == null)
                throw new ArgumentNullException("dataSet");
            if (binding == null)
                throw new ArgumentNullException("binding");

            DataSetInfo dataSetInfo;
            if (! lookupTable.TryGetValue(dataSet, out dataSetInfo))
                throw new ArgumentException("The specified data set is not a member of this joined data set.", "dataSet");

            int? externalIndex = binding.Index;
            if (externalIndex.HasValue)
                externalIndex = externalIndex.Value + dataSetInfo.ColumnOffset;

            return new ResolvedBinding(dataSetInfo, binding, externalIndex);
        }

        /// <inheritdoc />
        protected override bool CanBindInternal(DataBinding binding)
        {
            return ResolveBinding(binding) != null;
        }

        /// <inheritdoc />
        protected override IEnumerable<IDataRow> GetRowsInternal(ICollection<DataBinding> bindings)
        {
            IDataProvider[] providers = GenericUtils.ToArray(DataSets);
            int providerCount = providers.Length;

            List<DataBinding>[] bindingsPerProvider = new List<DataBinding>[providerCount];
            for (int i = 0; i < providerCount; i++)
                bindingsPerProvider[i] = new List<DataBinding>();

            foreach (DataBinding binding in bindings)
            {
                ResolvedBinding resolvedBinding = ResolveBinding(binding);
                if (resolvedBinding != null)
                    bindingsPerProvider[resolvedBinding.DataSetInfo.DataSetIndex].Add(resolvedBinding.Inner);
            }

            foreach (IList<IDataRow> rowList in strategy.Join(providers, bindingsPerProvider))
                yield return new JoinedDataRow(this, rowList);
        }

        private ResolvedBinding ResolveBinding(DataBinding binding)
        {
            ResolvedBinding resolvedBinding = binding as ResolvedBinding;

            if (resolvedBinding != null && resolvedBinding.DataSetInfo.IsOwnedBy(this))
                return resolvedBinding;

            int? externalIndex = binding.Index;
            if (!externalIndex.HasValue)
            {
                foreach (IDataSet dataSet in DataSets)
                    if (dataSet.CanBind(binding))
                        return new ResolvedBinding(lookupTable[dataSet], binding, null);
            }
            else
            {
                DataBinding nullIndexBinding = null;

                int currentOffset = 0;
                foreach (IDataSet dataSet in DataSets)
                {
                    int currentColumnCount = dataSet.ColumnCount;

                    DataBinding currentBinding;
                    if (currentOffset == 0)
                    {
                        currentBinding = binding;
                    }
                    else
                    {
                        int internalIndex = externalIndex.Value - currentOffset;
                        if (internalIndex < 0 || internalIndex >= currentColumnCount)
                        {
                            if (nullIndexBinding == null)
                                nullIndexBinding = binding.ReplaceIndex(null);
                            currentBinding = nullIndexBinding;
                        }
                        else
                        {
                            currentBinding = binding.ReplaceIndex(internalIndex);
                        }
                    }

                    if (dataSet.CanBind(currentBinding))
                        return new ResolvedBinding(lookupTable[dataSet], currentBinding, binding.Index);

                    currentOffset += currentColumnCount;
                }
            }

            return null;
        }

        private sealed class JoinedDataRow : IDataRow
        {
            private readonly JoinedDataSet owner;
            private readonly IList<IDataRow> rowList;

            public JoinedDataRow(JoinedDataSet owner, IList<IDataRow> rowList)
            {
                this.owner = owner;
                this.rowList = rowList;
            }

            public IEnumerable<KeyValuePair<string, string>> GetMetadata()
            {
                foreach (IDataRow row in rowList)
                    foreach (KeyValuePair<string, string> entry in row.GetMetadata())
                        yield return entry;
            }

            public object GetValue(DataBinding binding)
            {
                if (binding == null)
                    throw new ArgumentNullException("binding");

                ResolvedBinding resolvedBinding = owner.ResolveBinding(binding);
                if (resolvedBinding == null)
                    throw new DataBindingException("Could not determine the underlying data set that supports the binding.");

                return rowList[resolvedBinding.DataSetInfo.DataSetIndex].GetValue(resolvedBinding.Inner);
            }

            public void Dispose()
            {
                foreach (IDataRow row in rowList)
                    row.Dispose();
            }
        }

        private sealed class DataSetInfo
        {
            private readonly JoinedDataSet owner;
            private readonly int dataSetIndex;
            private readonly int columnOffset;

            public DataSetInfo(JoinedDataSet owner, int dataSetIndex, int columnOffset)
            {
                this.owner = owner;
                this.dataSetIndex = dataSetIndex;
                this.columnOffset = columnOffset;
            }

            public int ColumnOffset
            {
                get { return columnOffset; }
            }

            public int DataSetIndex
            {
                get { return dataSetIndex; }
            }

            public bool IsOwnedBy(JoinedDataSet owner)
            {
                return ReferenceEquals(this.owner, owner);
            }
        }

        private sealed class ResolvedBinding : DataBinding
        {
            private readonly DataSetInfo dataSetInfo;
            private readonly DataBinding inner;
            private readonly int? externalIndex;

            public ResolvedBinding(DataSetInfo dataSetInfo, DataBinding inner, int? externalIndex)
            {
                this.dataSetInfo = dataSetInfo;
                this.inner = inner;
                this.externalIndex = externalIndex;
            }

            public DataSetInfo DataSetInfo
            {
                get { return dataSetInfo; }
            }

            public DataBinding Inner
            {
                get { return inner; }
            }

            public override string Path
            {
                get { return inner.Path; }
            }

            public override int? Index
            {
                get { return externalIndex; }
            }

            public override Type ValueType
            {
                get { return inner.ValueType; }
            }

            public override DataBinding ReplaceIndex(int? index)
            {
                return new ResolvedBinding(dataSetInfo, inner, index);
            }
        }
    }
}