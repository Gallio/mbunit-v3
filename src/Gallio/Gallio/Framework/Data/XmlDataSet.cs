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
using System.Xml.XPath;

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
    /// <term>Row Path</term>
    /// <description>An XPath expression that selects a set of nodes that are used to
    /// uniquely identify records.  For example, the row path might be used to select
    /// the containing element of each Book element in an XML document of Books.
    /// The row path is specified in the constructor.</description>
    /// </item>
    /// <item>
    /// <term>Binding Path</term>
    /// <description>An XPath expression that selects a node relative to the row path
    /// that contains a particular data value of interest.  For example, the binding path
    /// might be used to select the Author attribute of a Book element in an XML document of Books.
    /// The binding path is specified by the <see cref="DataBinding" />.</description>
    /// </item>
    /// </list>
    /// </para>
    /// </summary>
    public class XmlDataSet : BaseDataSet
    {
        private readonly IXPathNavigable document;
        private readonly string rowPath;
        private readonly bool isDynamic;

        /// <summary>
        /// Creates an XML data set.
        /// </summary>
        /// <param name="document">The input XML document</param>
        /// <param name="rowPath">The XPath expression used to select rows within the document</param>
        /// <param name="isDynamic">True if the data set should be considered dynamic</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="document"/> or <paramref name="rowPath"/> is null</exception>
        public XmlDataSet(IXPathNavigable document, string rowPath, bool isDynamic)
        {
            if (document == null)
                throw new ArgumentNullException("document");
            if (rowPath == null)
                throw new ArgumentNullException("rowPath");

            this.document = document;
            this.rowPath = rowPath;
            this.isDynamic = isDynamic;
        }

        /// <inheritdoc />
        public override bool IsDynamic
        {
            get { return isDynamic; }
        }

        /// <inheritdoc />
        public override int ColumnCount
        {
            get { return 0; }
        }

        /// <inheritdoc />
        protected override bool CanBindInternal(DataBinding binding)
        {
            string bindingPath = binding.Path;
            if (bindingPath == null)
                return false;

            string fullPath = rowPath + "/" + bindingPath;
            try
            {
                return document.CreateNavigator().Select(fullPath).MoveNext();
            }
            catch (XPathException)
            {
                // Return false if the combined path is not a valid XPath expression.
                return false;
            }
        }

        /// <inheritdoc />
        protected override IEnumerable<IDataRow> GetRowsInternal(ICollection<DataBinding> bindings)
        {
            foreach (XPathNavigator row in document.CreateNavigator().Select(rowPath))
                yield return new XmlDataRow(row);
        }

        private sealed class XmlDataRow : BaseDataRow
        {
            private readonly XPathNavigator row;

            public XmlDataRow(XPathNavigator row)
                : base(null)
            {
                this.row = row;
            }

            protected override object GetValueInternal(DataBinding binding)
            {
                if (binding.Path == null)
                    throw new DataBindingException("A valid XPath expression is required as the binding path.");

                try
                {
                    return row.SelectSingleNode(binding.Path);
                }
                catch (XPathException ex)
                {
                    throw new DataBindingException("A valid XPath expression is required as the binding path.", ex);
                }
            }
        }
    }
}
