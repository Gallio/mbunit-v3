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

namespace Gallio.Hosting
{
    /// <summary>
    /// A default implementation of <see cref="IRegisteredComponentResolver{T}" />
    /// based on <see cref="IRuntime" />.
    /// </summary>
    public class RuntimeRegisteredComponentResolver<T> : IRegisteredComponentResolver<T>
        where T : class, IRegisteredComponent
    {
        private readonly IRuntime runtime;

        /// <summary>
        /// Creates a test runner manager.
        /// </summary>
        /// <param name="runtime">The runtime</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="runtime"/> is null</exception>
        public RuntimeRegisteredComponentResolver(IRuntime runtime)
        {
            if (runtime == null)
                throw new ArgumentNullException("runtime");

            this.runtime = runtime;
        }

        /// <inheritdoc />
        public IList<string> GetNames()
        {
            List<string> names = new List<string>();
            foreach (T component in runtime.ResolveAll<T>())
                names.Add(component.Name);

            return names;
        }

        /// <inheritdoc />
        public T Resolve(string name)
        {
            if (name == null)
                throw new ArgumentNullException("name");

            foreach (T component in runtime.ResolveAll<T>())
            {
                if (String.Equals(name, component.Name, StringComparison.CurrentCultureIgnoreCase))
                    return component;
            }

            return null;
        }
    }
}
