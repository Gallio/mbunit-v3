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

namespace Gallio.Common
{
    /// <summary>
    /// A function chain captures a sequence of actions to be performed as
    /// part of a complex multi-part process.
    /// </summary>
    /// <typeparam name="T">The function argument type.</typeparam>
    /// <typeparam name="TResult">The function result type.</typeparam>
    public class FuncChain<T, TResult>
    {
        private Func<T, TResult> func;

        /// <summary>
        /// Creates a function chain.
        /// </summary>
        /// <param name="func">The initial function.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="func"/> is null.</exception>
        public FuncChain(Func<T, TResult> func)
        {
            if (func == null)
                throw new ArgumentNullException("func");

            this.func = func;
        }

        /// <summary>
        /// Gets or sets a representation of the chain as a single function.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The function is progressively augmented as new contributions are
        /// registered <see cref="Around" />.  By default the action is whatever was
        /// passed into the constructor.
        /// </para>
        /// </remarks>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="value"/> is null.</exception>
        public Func<T, TResult> Func
        {
            get { return func; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");

                func = value;
            }
        }

        /// <summary>
        /// Registers a function decorator to perform around all other actions
        /// currently in the chain.  The contained part of the chain
        /// is passed in as a function to the decorator that the decorator
        /// can choose to run (or not) as needed.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The value of <see cref="Func" /> will be set to a new instance
        /// that performs the specified <paramref name="decorator"/> around
        /// the current <see cref="Func" />.
        /// </para>
        /// </remarks>
        /// <param name="decorator">The decorator to register.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="decorator"/> is null.</exception>
        public void Around(FuncDecorator<T, TResult> decorator)
        {
            if (decorator == null)
                throw new ArgumentNullException("decorator");

            Func<T, TResult> innerFunc = func;
            func = delegate(T obj)
            {
                return decorator(obj, innerFunc);
            };
        }
    }
}