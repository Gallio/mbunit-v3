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

namespace Gallio.Collections
{
    /// <summary>
    /// A strongly-typed key to be used together with an associative array to help the
    /// compiler perform better type checking of the value associated with the key.
    /// </summary>
    /// <typeparam name="TValue">The type of value associated with the key</typeparam>
    public struct Key<TValue>
    {
        private readonly string name;

        /// <summary>
        /// Creates a new key.
        /// </summary>
        /// <param name="name">The unique name of the key</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="name"/> is null</exception>
        public Key(string name)
        {
            if (name == null)
                throw new ArgumentNullException("name");

            this.name = name;
        }

        /// <summary>
        /// Gets the unique name of the key.
        /// </summary>
        public string Name
        {
            get { return name; }
        }
    }
}
