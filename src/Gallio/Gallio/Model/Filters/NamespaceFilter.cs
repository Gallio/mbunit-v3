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
using Gallio.Model;

namespace Gallio.Model.Filters
{
    /// <summary>
    /// A filter that matches objects whose <see cref="IModelComponent.CodeReference" />
    /// contains the specified namespace.
    /// </summary>
    [Serializable]
    public class NamespaceFilter<T> : Filter<T> where T : IModelComponent
    {
        private string namespaceName;

        /// <summary>
        /// Creates a namespace filter.
        /// </summary>
        /// <param name="namespaceName">The namespace name that must exactly match the
        /// value obtained via reflection on types</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="namespaceName"/> is null</exception>
        public NamespaceFilter(string namespaceName)
        {
            if (namespaceName == null)
                throw new ArgumentNullException("namespaceName");

            this.namespaceName = namespaceName;
        }

        /// <inheritdoc />
        public override bool IsMatch(T value)
        {
            return namespaceName == value.CodeReference.NamespaceName;
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return " Namespace(" + namespaceName + ") ";
        }
    }
}