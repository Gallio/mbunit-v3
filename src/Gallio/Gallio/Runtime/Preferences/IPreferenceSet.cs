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
