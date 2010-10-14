// Copyright 2005-2010 Gallio Project - http://www.gallio.org/
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
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Gallio.Common.Security
{
    /// <summary>
    /// Calculates a 32-bit hash code based on the FNV (Fowler-Noll-Vo) algorithm.
    /// (http://en.wikipedia.org/wiki/Fowler_Noll_Vo_hash)
    /// </summary>
    public class FnvHasher
    {
        private const int Initial = -2128831035;
        private const int Vector = 16777619;
        private int value;

        /// <summary>
        /// Returns the final hash code.
        /// </summary>
        /// <returns>The resulting 32-bit hash code.</returns>
        public virtual int ToValue()
        {
            return value;
        }

        /// <summary>
        /// Default constructor.
        /// </summary>
        public FnvHasher()
        {
            value = Initial;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="salt">Salt value.</param>
        public FnvHasher(int salt)
            : this()
        {
            Combine(salt);
        }

        /// <summary>
        /// Adds the hash code of the specified object and combines it with the previous values.
        /// </summary>
        /// <param name="obj">The object to add.</param>
        /// <returns>The builder itself for chaining syntax.</returns>
        public FnvHasher Add(object obj)
        {
            Combine(obj);
            return this;
        }

        /// <summary>
        /// Adds the hash code of the specified objects and combines them with the previous values.
        /// </summary>
        /// <param name="objects">An enumeration of objects to add.</param>
        /// <returns>The builder itself for chaining syntax.</returns>
        public FnvHasher Add(IEnumerable objects)
        {
            if (!ReferenceEquals(null, objects))
            {
                foreach (object obj in objects)
                    Combine(obj);
            }

            return this;
        }

        private void Combine(object obj)
        {
            value = (value * Vector) ^ (obj ?? 0).GetHashCode();
        }
    }
}
