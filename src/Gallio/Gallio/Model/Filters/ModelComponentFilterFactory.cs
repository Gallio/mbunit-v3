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
using Gallio.Model;

namespace Gallio.Model.Filters
{
    /// <summary>
    /// Builds filters for <see cref="ITestComponent"/> objects.
    /// </summary>
    /// <remarks>
    /// Recognizes the following filter keys:
    /// <list type="bullet">
    /// <item>Id: Filter by id</item>
    /// <item>Name: Filter by name</item>
    /// <item>Assembly: Filter by assembly name</item>
    /// <item>Namespace: Filter by namespace name</item>
    /// <item>Type: Filter by type name, including inherited types</item>
    /// <item>ExactType: Filter by exact type name, excluding inherited types</item>
    /// <item>Member: Filter by member name</item>
    /// <item>*: All other names are assumed to correspond to metadata keys</item>
    /// </list>
    /// </remarks>
    /// <typeparam name="T">The <see cref="ITestComponent"/> subtype</typeparam>
    public class ModelComponentFilterFactory<T> : IFilterFactory<T>
        where T : ITestComponent
    {
        /// <inheritdoc />
        public Filter<T> CreateFilter(string key, Filter<string> valueFilter)
        {
            switch (key)
            {
                case "Id":
                    return new IdFilter<T>(valueFilter);
                case "Name":
                    return new NameFilter<T>(valueFilter);
                case "Assembly":
                    return new AssemblyFilter<T>(valueFilter);
                case "Namespace":
                    return new NamespaceFilter<T>(valueFilter);
                case "Type":
                    return new TypeFilter<T>(valueFilter, true);
                case "ExactType":
                    return new TypeFilter<T>(valueFilter, false);
                case "Member":
                    return new MemberFilter<T>(valueFilter);
                default:
                    return new MetadataFilter<T>(key, valueFilter);
            }
        }
    }
}
