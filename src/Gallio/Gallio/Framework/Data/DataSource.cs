// Copyright 2005-2009 Gallio Project - http://www.gallio.org/
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
using System.Collections.Generic;
using Gallio.Collections;
using Gallio.Model;

namespace Gallio.Framework.Data
{
    /// <summary>
    /// An data source object provides a simple way of aggregating data sets
    /// together.  It also provides a simple translation mechanism for mapping
    /// binding paths to binding indexes which is useful for providing named
    /// aliases for columns in indexed data sets.
    /// </summary>
    public class DataSource : MergedDataSet
    {
        private readonly string name;
        private Dictionary<string, int?> indexAliases;

        /// <summary>
        /// Creates a data source with a given name.
        /// </summary>
        /// <param name="name">The name of the data source, or an empty string if it is anonymous</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="name"/> is null</exception>
        public DataSource(string name)
        {
            if (name == null)
                throw new ArgumentNullException("name");

            this.name = name;
        }

        /// <summary>
        /// Gets the name of the data source, or an empty string if it is anonymous.
        /// </summary>
        public string Name
        {
            get { return name; }
        }

        /// <summary>
        /// Adds an alias for a binding path to map it to the specified index.
        /// </summary>
        /// <param name="path">The binding path to match in a case-insensitive manner</param>
        /// <param name="index">The associated index to use instead</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="path"/> is null</exception>
        public void AddIndexAlias(string path, int? index)
        {
            if (path == null)
                throw new ArgumentNullException("path");

            if (indexAliases == null)
                indexAliases = new Dictionary<string, int?>(StringComparer.InvariantCultureIgnoreCase);

            indexAliases.Add(path, index);
        }

        /// <inheritdoc />
        protected override bool CanBindImpl(DataBinding binding)
        {
            if (indexAliases != null)
                binding = TranslateBinding(binding);

            return base.CanBindImpl(binding);
        }

        /// <inheritdoc />
        protected override IEnumerable<IDataItem> GetItemsImpl(ICollection<DataBinding> bindings,
            bool includeDynamicItems)
        {
            if (indexAliases != null)
                return GetItemsImplTranslated(bindings, includeDynamicItems);
            else
                return GetItemsImplBase(bindings, includeDynamicItems);
        }

        private IEnumerable<IDataItem> GetItemsImplBase(ICollection<DataBinding> bindings,
            bool includeDynamicItems)
        {
            return base.GetItemsImpl(bindings, includeDynamicItems);
        }

        private IEnumerable<IDataItem> GetItemsImplTranslated(ICollection<DataBinding> bindings,
            bool includeDynamicItems)
        {
            DataBinding[] translatedBindings = GenericUtils.ConvertAllToArray<DataBinding, DataBinding>(bindings, TranslateBinding);

            foreach (IDataItem item in GetItemsImplBase(translatedBindings, includeDynamicItems))
                yield return new TranslatedDataItem(this, item);
        }

        private DataBinding TranslateBinding(DataBinding binding)
        {
            string path = binding.Path;

            int? index;
            if (path != null && indexAliases.TryGetValue(path, out index))
                return binding.ReplaceIndex(index);

            return binding;
        }

        private DataBinding ReverseTranslateBinding(DataBinding binding)
        {
            string alias = FindAliasForIndex(binding.Index);
            return alias != null ? new DataBinding(binding.Index, alias) : binding;
        }

        private string FindAliasForIndex(int? index)
        {
            if (index.HasValue)
            {
                foreach (KeyValuePair<string, int?> indexAlias in indexAliases)
                    if (indexAlias.Value == index)
                        return indexAlias.Key;
            }

            return null;
        }

        private sealed class TranslatedDataItem : BaseDataItem
        {
            private readonly DataSource source;
            private readonly IDataItem item;

            public TranslatedDataItem(DataSource source, IDataItem item)
            {
                this.source = source;
                this.item = item;
            }

            public override bool IsDynamic
            {
                get { return item.IsDynamic; }
            }

            public override IEnumerable<DataBinding> GetBindingsForInformalDescription()
            {
                foreach (DataBinding dataBinding in item.GetBindingsForInformalDescription())
                    yield return source.ReverseTranslateBinding(dataBinding);
            }

            protected override void PopulateMetadataImpl(MetadataMap map)
            {
                item.PopulateMetadata(map);
            }

            protected override object GetValueImpl(DataBinding binding)
            {
                if (binding == null)
                    throw new ArgumentNullException("binding");

                return item.GetValue(source.TranslateBinding(binding));
            }
        }
    }
}
