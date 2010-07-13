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

namespace Gallio.Common
{
    /// <summary>
    /// Lazy computation type.
    /// </summary>
    /// <typeparam name="T">The encapsulated type.</typeparam>
    public class Lazy<T>
    {
        private readonly Func<T> func;
        private T result;
        private bool hasValue;
        
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="func">A function which computes the value on first request.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="func"/> is null.</exception>
        public Lazy(Func<T> func)
        {
            if (func == null)
                throw new ArgumentNullException("func");
        
            this.func = func;
            hasValue = false;
        }

        /// <summary>
        /// Gets the value.
        /// </summary>
        public T Value
        {
            get
            {
                if (!hasValue)
                {
                    result = func();
                    hasValue = true;
                }

                return result;
            }
        }
    }
}
