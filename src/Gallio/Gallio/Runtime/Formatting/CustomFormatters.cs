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
using Gallio.Common;
using Gallio.Runtime.Conversions;

namespace Gallio.Runtime.Formatting
{
    /// <summary>
    /// Extensibility point for object formatting managed by <see cref="RuleBasedFormatter"/>.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Use the static methods <see cref="Register"/> and <see cref="Unregister"/> to add and remove 
    /// custom type formatters.
    /// </para>
    /// </remarks>
    /// <example>
    /// The following example registers a custom formatter for a <c>Foo</c> object.
    /// <code><![CDATA[
    /// public interface IFoo
    /// {
    ///     int Value
    ///     {
    ///         get;
    ///     }
    /// }
    /// 
    /// CustomFormatters.Register<Foo>(x => String.Format("Foo = {0}", x.Value));
    /// ]]></code>
    /// </example>
    public static class CustomFormatters
    {
        private static readonly IDictionary<Type, Data> Formatters = new Dictionary<Type, Data>();
        private static readonly object SyncRoot = new object();

        /// <summary>
        /// Registers a custom formatter that represents an object in a readable text format.
        /// </summary>
        /// <param name="type">The type of the object to format.</param>
        /// <param name="formatter">A delegate that performs the formatting operation.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="type"/> or <paramref name="formatter"/> is null.</exception>
        /// <exception cref="InvalidOperationException">Thrown if a custom formatter for the specified type was already registered.</exception>
        public static void Register(Type type, FormattingFunc formatter)
        {
            if (type == null)
                throw new ArgumentNullException("type");
            if (formatter == null)
                throw new ArgumentNullException("formatter");

            lock (SyncRoot)
            {
                Data data;

                if (Formatters.TryGetValue(type, out data))
                {
                    data.Count++;
                }
                else
                {
                    Formatters[type] = new Data(formatter);
                }
            }
        }

        /// <summary>
        /// Registers a strongly-tped custom formatter for the specified types.
        /// </summary>
        /// <typeparam name="T">The type of the object to format.</typeparam>
        /// <param name="formatter">A delegate that performs the formatting operation.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="formatter"/> is null.</exception>
        /// <exception cref="InvalidOperationException">Thrown if a custom formatter for the specified type was already registered.</exception>
        public static void Register<T>(FormattingFunc<T> formatter)
        {
            if (formatter == null)
                throw new ArgumentNullException("formatter");

            Register(typeof(T), x => formatter((T)x));
        }

        /// <summary>
        /// Unregisters the custom formatter for the specified type.
        /// </summary>
        /// <remarks>
        /// <para>
        /// If no custom formatter exists for the specified type,
        /// the method has no effect and no exception is thrown.
        /// </para>
        /// </remarks>
        /// <param name="type">The searched type.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="type"/> is null.</exception>
        public static void Unregister(Type type)
        {
            if (type == null)
                throw new ArgumentNullException("type");

            lock (SyncRoot)
            {
                Data data;

                if (Formatters.TryGetValue(type, out data))
                {
                    if (data.Count > 0)
                    {
                        data.Count--;
                    }
                    else
                    {
                        Formatters.Remove(type);
                    }
                }
            }
        }

        /// <summary>
        /// Unregisters the custom formatter for the specified type.
        /// </summary>
        /// <remarks>
        /// <para>
        /// If no custom converter exists for the specified types,
        /// the method has no effect and no exception is thrown.
        /// </para>
        /// </remarks>
        /// <typeparam name="T">The searched type.</typeparam>
        public static void Unregister<T>()
        {
            Unregister(typeof(T));
        }

        // Returns the formatter for the searched type, or null if none was registered.
        internal static FormattingFunc Find(Type type)
        {
            if (type == null)
                throw new ArgumentNullException("type");

            lock (SyncRoot)
            {
                Data data;
                return Formatters.TryGetValue(type, out data) ? data.Formatter : null;
            }
        }

        // Removes all the registered custom formatters.
        internal static void UnregisterAll()
        {
            lock (SyncRoot)
            {
                Formatters.Clear();
            }
        }

        private sealed class Data
        {
            private readonly FormattingFunc formatter;

            public FormattingFunc Formatter
            {
                get
                {
                    return formatter;
                }
            }

            public int Count
            {
                get;
                set;
            }

            public Data(FormattingFunc formatter)
            {
                this.formatter = formatter;
            }
        }
    }
}
