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
using Gallio.Model;
using Gallio.Common.Reflection;

namespace Gallio.Model.Filters
{
    /// <summary>
    /// A filter that matches objects whose <see cref="ITestComponent.CodeElement" />
    /// matches the specified namespace name.
    /// </summary>
    [Serializable]
    public class NamespaceFilter<T> : PropertyFilter<T> where T : ITestComponent
    {
        /// <summary>
        /// Creates a namespace filter.
        /// </summary>
        /// <param name="namespaceNameFilter">A filter for the namespace name
        /// obtained via reflection on types</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="namespaceNameFilter"/> is null</exception>
        public NamespaceFilter(Filter<string> namespaceNameFilter)
            : base(namespaceNameFilter)
        {
        }

        /// <inheritdoc />
        public override string Key
        {
            get { return @"Namespace"; }
        }

        /// <inheritdoc />
        public override bool IsMatch(T value)
        {
            INamespaceInfo @namespace = ReflectionUtils.GetNamespace(value.CodeElement);
            if (@namespace == null)
                return false;

            return ValueFilter.IsMatch(@namespace.Name);
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return @"Namespace(" + ValueFilter + @")";
        }
    }
}