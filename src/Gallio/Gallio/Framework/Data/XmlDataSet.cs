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
using System.Collections.Generic;
using System.Xml.XPath;
using Gallio.Model;

namespace Gallio.Framework.Data
{
    /// <summary>
    /// <para>
    /// An XML data set selects nodes from an XML document using XPath
    /// expressions.  The selected nodes are returned as <see cref="XPathNavigator" /> objects.
    /// </para>
    /// <para>
    /// Two XPath expressions are used.
    /// <list type="bullet">
    /// <item>
    /// <term>Item Path</term>
    /// <description>An XPath expression that selects a set of nodes that are used to
    /// uniquely identify records.  For example, the item path might be used to select
    /// the containing element of each Book element in an XML document of Books.
    /// The item path is specified in the constructor.</description>
    /// </item>
    /// <item>
    /// <term>Binding Path</term>
    /// <description>An XPath expression that selects a node relative to the item path
    /// that contains a particular data value of interest.  For example, the binding path
    /// might be used to select the Author attribute of a Book element in an XML document of Books.
    /// The binding path is specified by the <see cref="DataBinding" />.</description>
    /// </item>
    /// </list>
    /// </para>
    /// </summary>
    public class XmlDataSet : BaseDataSet
    {
        private readonly Func<IXPathNavigable> documentProvider;
        private readonly string itemPath;
        private readonly bool isDynamic;

        /// <summary>
        /// Creates an XML data set.
        /// </summary>
        /// <param name="documentProvider">A delegate that produces the XML document on demand</param>
        /// <param name="itemPath">The XPath expression used to select items within the document</param>
        /// <param name="isDynamic">True if the data set should be considered dynamic</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="documentProvider"/> or <paramref name="itemPath"/> is null</exception>
        public XmlDataSet(Func<IXPathNavigable> documentProvider, string itemPath, bool isDynamic)
        {
            if (documentProvider == null)
                throw new ArgumentNullException("documentProvider");
            if (itemPath == null)
                throw new ArgumentNullException("itemPath");

            this.documentProvider = documentProvider;
            this.itemPath = itemPath;
            this.isDynamic = isDynamic;
        }

        /// <inheritdoc />
        public override int ColumnCount
        {
            get { return 0; }
        }

        /// <inheritdoc />
        protected override bool CanBindImpl(DataBinding binding)
        {
            string bindingPath = binding.Path;
            if (bindingPath == null)
                return false;

            string fullPath = itemPath + "/" + bindingPath;
            try
            {
                IXPathNavigable document = documentProvider();
                return document.CreateNavigator().Select(fullPath).MoveNext();
            }
            catch (XPathException)
            {
                // Return false if the combined path is not a valid XPath expression.
                return false;
            }
        }

        /// <inheritdoc />
        protected override IEnumerable<IDataItem> GetItemsImpl(ICollection<DataBinding> bindings, bool includeDynamicItems)
        {
            if (!isDynamic || includeDynamicItems)
            {
                IXPathNavigable document = documentProvider();

                foreach (XPathNavigator navigator in document.CreateNavigator().Select(itemPath))
                    yield return new Item(navigator, isDynamic);
            }
        }

        private sealed class Item : SimpleDataItem
        {
            private readonly XPathNavigator navigator;

            public Item(XPathNavigator navigator, bool isDynamic)
                : base(null, isDynamic)
            {
                this.navigator = navigator;
            }

            /// <inheritdoc />
            public override IEnumerable<DataBinding> GetBindingsForInformalDescription()
            {
                yield return new DataBinding(null, ".");
            }

            protected override object GetValueImpl(DataBinding binding)
            {
                if (binding.Path == null)
                    throw new DataBindingException("A valid XPath expression is required as the binding path.", binding);

                try
                {
                    return navigator.SelectSingleNode(binding.Path);
                }
                catch (XPathException ex)
                {
                    throw new DataBindingException("A valid XPath expression is required as the binding path.", binding, ex);
                }
            }
        }
    }
}
