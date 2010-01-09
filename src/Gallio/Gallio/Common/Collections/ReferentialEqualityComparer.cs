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
using System.Collections.Generic;
using System.Text;

namespace Gallio.Common.Collections
{
    /// <summary>
    /// An equality comparer that compares values by reference.
    /// </summary>
    /// <typeparam name="T">The type of values to compare, must be reference types.</typeparam>
    public sealed class ReferentialEqualityComparer<T> : IEqualityComparer<T>
        where T : class
    {
        private static ReferentialEqualityComparer<T> instance = new ReferentialEqualityComparer<T>();

        private ReferentialEqualityComparer()
        {
        }

        /// <summary>
        /// Gets the singleton instance of the comparer.
        /// </summary>
        public static ReferentialEqualityComparer<T> Instance
        {
            get { return instance; }
        }

        /// <inheritdoc />
        public bool Equals(T x, T y)
        {
            return ReferenceEquals(x, y);
        }

        /// <inheritdoc />
        public int GetHashCode(T obj)
        {
            return obj != null ? obj.GetHashCode() : 0;
        }
    }
}
