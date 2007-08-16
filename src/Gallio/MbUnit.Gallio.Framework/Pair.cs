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
using System.Collections.Generic;
using System.Text;

namespace MbUnit.Framework
{
    /// <summary>
    /// An immutable structure that contains two values.
    /// </summary>
    /// <typeparam name="TFirst"></typeparam>
    /// <typeparam name="TSecond"></typeparam>
    public struct Pair<TFirst, TSecond>
    {
        private TFirst first;
        private TSecond second;

        /// <summary>
        /// Creates a pair.
        /// </summary>
        /// <param name="first">The first value</param>
        /// <param name="second">The second value</param>
        public Pair(TFirst first, TSecond second)
        {
            this.first = first;
            this.second = second;
        }

        /// <summary>
        /// Gets the first value.
        /// </summary>
        public TFirst First
        {
            get { return first; }
        }

        /// <summary>
        /// Gets the second value.
        /// </summary>
        public TSecond Second
        {
            get { return second; }
        }
    }
}
