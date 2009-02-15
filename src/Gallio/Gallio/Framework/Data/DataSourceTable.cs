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

namespace Gallio.Framework.Data
{
    /// <summary>
    /// A data source table manages a collection of named data sources.
    /// </summary>
    public class DataSourceTable : IDataSourceResolver
    {
        private Dictionary<string, DataSource> sources;

        /// <inheritdoc />
        public DataSource DefineDataSource(string name)
        {
            if (name == null)
                throw new ArgumentNullException("name");

            DataSource source;

            if (sources != null)
            {
                if (sources.TryGetValue(name, out source))
                    return source;
            }
            else
            {
                sources = new Dictionary<string, DataSource>();
            }

            source = new DataSource(name);
            sources.Add(name, source);
            return source;
        }

        /// <inheritdoc />
        public DataSource ResolveDataSource(string name)
        {
            if (name == null)
                throw new ArgumentNullException("name");

            DataSource source;
            if (sources != null && sources.TryGetValue(name, out source))
                return source;

            return null;
        }
    }
}
