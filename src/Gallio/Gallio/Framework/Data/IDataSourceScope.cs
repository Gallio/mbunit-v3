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

namespace Gallio.Framework.Data
{
    /// <summary>
    /// A data source scope provides a context in which data sources can be
    /// defined or resolves.
    /// </summary>
    public interface IDataSourceScope : IDataSourceResolver
    {
        /// <summary>
        /// Defines a new data source within this scope if one does not exist.
        /// Otherwise returns the existing one.
        /// </summary>
        /// <param name="name">The data source name</param>
        /// <returns>The defined data source</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="name"/> is null</exception>
        DataSource DefineDataSource(string name);
    }
}
