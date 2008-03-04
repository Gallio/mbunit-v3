// Copyright 2005-2008 Gallio Project - http://www.gallio.org/
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
    /// A registered service resolver looks up <see cref="IRegisteredComponent"/>
    /// components by name.
    /// </summary>
    public interface IRegisteredComponentResolver<T>
        where T : class, IRegisteredComponent
    {
        /// <summary>
        /// Gets the names of all registered components.
        /// </summary>
        /// <returns>The list of registered component names</returns>
        IList<string> GetNames();

        /// <summary>
        /// Resolves a registered component by name.
        /// </summary>
        /// <param name="name">The name of the registered component, matched case-insensitively</param>
        /// <returns>The test runner factory, or null if none exist with the specified name</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="name"/> is null</exception>
        T Resolve(string name);
    }
}
