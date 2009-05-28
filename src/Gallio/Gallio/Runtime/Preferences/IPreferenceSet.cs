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
using System.Collections.Generic;
using System.Text;
using Gallio.Common.Concurrency;

namespace Gallio.Runtime.Preferences
{
    /// <summary>
    /// Represents a set of related preference settings.
    /// </summary>
    /// <seealso cref="IPreferenceStore"/>
    /// <seealso cref="IPreferenceManager"/>
    public interface IPreferenceSet
    {
        /// <summary>
        /// Obtains a reader for the preference set.
        /// </summary>
        /// <param name="readAction">The read action which is provided with the reader</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="readAction"/> is null</exception>
        void Read(ReadAction<IPreferenceSetReader> readAction);

        /// <summary>
        /// Obtains a reader for the preference set.
        /// </summary>
        /// <param name="readFunc">The read function which is provided with the reader</param>
        /// <returns>The result of the read function</returns>
        /// <typeparam name="TResult">The read function result type</typeparam>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="readFunc"/> is null</exception>
        TResult Read<TResult>(ReadFunc<IPreferenceSetReader, TResult> readFunc);

        /// <summary>
        /// Obtains a writer for the preference set.
        /// </summary>
        /// <param name="writeAction">The write action which is provided with the reader</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="writeAction"/> is null</exception>
        void Write(WriteAction<IPreferenceSetWriter> writeAction);

        /// <summary>
        /// Obtains a writer for the preference set.
        /// </summary>
        /// <param name="writeFunc">The write function which is provided with the reader</param>
        /// <returns>The result of the write function</returns>
        /// <typeparam name="TResult">The write function result type</typeparam>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="writeFunc"/> is null</exception>
        TResult Write<TResult>(WriteFunc<IPreferenceSetWriter, TResult> writeFunc);
    }
}
