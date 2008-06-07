// Copyright 2005-2008 Gallio Project - http://www.gallio.org/
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

namespace Gallio.Framework.Formatting
{
    /// <summary>
    /// Provides services for formatting objects for display.
    /// </summary>
    public interface IFormatter
    {
        /// <summary>
        /// Formats an object to a string for display.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The formatted string may incorporate a variety of presentation techniques
        /// intended to clearly identify formatted values of different types.
        /// For example, strings might appear quoted, objects might be described
        /// using <see cref="Object.ToString" /> or in terms of their properties,
        /// collections might be listed as sequences of formatted elements.
        /// </para>
        /// <para>
        /// The resulting string should not be expected to be machine-readable.
        /// </para>
        /// <para>
        /// This method should never return an empty string or throw an exception.
        /// If the value cannot be formatted property then a placeholder should
        /// be used instead, such as the value's type.
        /// </para>
        /// <para>
        /// Likewise this method should never throw an exception.  If the object cannot be formatted
        /// cleanly, then an error message should be produced instead.
        /// </para>
        /// <para>
        /// These constraints are intended to ensure that the formatter provides
        /// a robust service to all clients.
        /// </para>
        /// </remarks>
        /// <param name="obj">The object to format, may be null</param>
        /// <returns>The formatted string, never null and never an empty string</returns>
        string Format(object obj);
    }
}
