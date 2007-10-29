// Copyright 2007 MbUnit Project - http://www.mbunit.com/
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
using System.Text;

namespace Gallio.Model.Data
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
