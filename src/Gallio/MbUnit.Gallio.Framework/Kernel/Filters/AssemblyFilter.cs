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
using System.Reflection;
using MbUnit.Framework.Kernel.Model;

namespace MbUnit.Framework.Kernel.Filters
{
    /// <summary>
    /// A filter that matches objects whose <see cref="IModelComponent.CodeReference" />
    /// contains the specified assembly.
    /// </summary>
    [Serializable]
    public class AssemblyFilter<T> : Filter<T> where T : IModelComponent
    {
        private string assemblyName;

        /// <summary>
        /// Creates an assembly filter.
        /// </summary>
        /// <param name="assemblyName">The type name specified in one of the following ways.
        /// In each case, the assembly name must exactly match the value obtained via reflection
        /// on the assembly.
        /// <list type="bullet">
        /// <item>Full name as returned by <see cref="Assembly.FullName" /></item>
        /// <item>Simple name (aka. partial name) as returned by <see cref="AssemblyName.Name" /></item>
        /// </list>
        /// </param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="assemblyName"/> is null</exception>
        public AssemblyFilter(string assemblyName)
        {
            if (assemblyName == null)
                throw new ArgumentNullException("assemblyName");

            this.assemblyName = assemblyName;
        }

        /// <inheritdoc />
        public override bool IsMatch(T value)
        {
            string assemblyName = value.CodeReference.AssemblyName;
            if (assemblyName == null)
                return false;

            return this.assemblyName == assemblyName
                || this.assemblyName == GetSimpleName(assemblyName);
        }

        private string GetSimpleName(string assemblyName)
        {
            AssemblyName name = new AssemblyName(assemblyName);
            return name.Name;
        }
    }
}