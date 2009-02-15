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
using Gallio.Framework.Data;

namespace Gallio.Framework.Pattern
{
    /// <summary>
    /// <para>
    /// A pattern test data context introduces a scope for data source declarations.
    /// </para>
    /// <para>
    /// Each <see cref="PatternTest" /> or <see cref="PatternTestParameter" /> has a <see cref="PatternTestDataContext" />.
    /// However, data contexts may also be nested.
    /// </para>
    /// <para>
    /// For example, the constructor of a test class typically augments its containing test with
    /// additional test parameters that represent its own constructor parameters.  If data binding
    /// attributes are applied to the constructor, they should operate within the scope of that
    /// constructor only.  To achieve this effect, the constructor declares a new <see cref="PatternTestDataContext" />
    /// within which its test parameters will be created.
    /// </para>
    /// </summary>
    public class PatternTestDataContext : IDataSourceResolver
    {
        private readonly PatternTestDataContext parent;
        private int implicitDataBindingIndexOffset;

        private DataSourceTable dataSourceTable;

        /// <summary>
        /// Creates a new data context.
        /// </summary>
        /// <param name="parent">The containing data context, or null if none</param>
        public PatternTestDataContext(PatternTestDataContext parent)
        {
            this.parent = parent;
        }

        /// <summary>
        /// Gets the containing data context, or null if none.
        /// </summary>
        public PatternTestDataContext Parent
        {
            get { return parent; }
        }

        /// <summary>
        /// <para>
        /// Gets or sets the offset to add to a test parameter's implicit data binding index to map it
        /// into the containing data context.
        /// </para>
        /// <para>
        /// This property is used to determine the data binding index of a test parameter that
        /// has not been explicitly bound.  The offsets are summed cumulatively to produce an implicit
        /// data binding index while traversing the chain of containing data contexts while locating the
        /// first anonymous data source.
        /// </para>
        /// <para>
        /// For example, suppose <c>Y</c> is the second parameter of a test method.  If <c>Y</c> does not
        /// have an explicit data binding, we will apply implicit data binding rules as follows.
        /// <list type="bullet">
        /// <item>If <c>Y</c>'s data context contains an anonymous data source, then the implicit data binding index will be 0.</item>
        /// <item>Otherwise, if <c>Y</c>'s containing data context contains an anonymous data source, then the implicit data
        /// binding index will equal the offset specified in <c>Y</c>'s data context: 1 (since it is the second parameter).</item>
        /// <item>Otherwise, we continue searching containing data contexts and summing their offsets until we find an
        /// anonymous data source.  If none is found, then data binding will fail.</item>
        /// </list>
        /// </para>
        /// </summary>
        public int ImplicitDataBindingIndexOffset
        {
            get { return implicitDataBindingIndexOffset; }
            set { implicitDataBindingIndexOffset = value; }
        }

        /// <summary>
        /// Defines a new data source within this data context if one does not exist.
        /// Otherwise returns the existing one.
        /// </summary>
        /// <param name="name">The data source name</param>
        /// <returns>The defined data source</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="name"/> is null</exception>
        public DataSource DefineDataSource(string name)
        {
            if (dataSourceTable == null)
                dataSourceTable = new DataSourceTable();

            return dataSourceTable.DefineDataSource(name);
        }

        /// <inheritdoc />
        public DataSource ResolveDataSource(string name)
        {
            if (dataSourceTable != null)
            {
                DataSource source = dataSourceTable.ResolveDataSource(name);
                if (source != null)
                    return source;
            }

            if (parent != null)
                return parent.ResolveDataSource(name);
            return null;
        }

        /// <summary>
        /// Returns the index that should be used to implicitly bind to the nearest
        /// anonymous data source that can be found.  The index is computed as the
        /// sum of the <see cref="ImplicitDataBindingIndexOffset" /> of each data
        /// context traversed to find the data source excluding the data context
        /// that actually has the data source.
        /// </summary>
        /// <returns>The implicit data binding index, or null if no anonymous data sources were found</returns>
        /// <seealso cref="ImplicitDataBindingIndexOffset"/>
        public int? ResolveImplicitDataBindingIndex()
        {
            int index = 0;

            for (PatternTestDataContext currentContext = this; currentContext != null; currentContext = currentContext.parent)
            {
                if (currentContext.dataSourceTable != null
                    && currentContext.dataSourceTable.ResolveDataSource("") != null)
                    return index;

                index += currentContext.implicitDataBindingIndexOffset;
            }

            return null;
        }

        /// <summary>
        /// Creates a child data context.
        /// </summary>
        /// <returns>A handle for the child context</returns>
        public virtual PatternTestDataContext CreateChild()
        {
            return new PatternTestDataContext(this);
        }
    }
}
