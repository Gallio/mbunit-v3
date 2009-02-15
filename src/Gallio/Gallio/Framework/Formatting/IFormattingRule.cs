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

namespace Gallio.Framework.Formatting
{
    /// <summary>
    /// <para>
    /// A formatting rule encapsulates an algorithm for formatting values of particular
    /// types to strings for presentation.
    /// </para>
    /// </summary>
    /// <remarks>
    /// This type is not intended to be used directly by clients.  Instead refer to
    /// <see cref="IFormatter" /> for a simpler abstraction that wraps <see cref="IFormattingRule" />.
    /// </remarks>
    public interface IFormattingRule
    {
        /// <summary>
        /// Gets the formatting rule's priority for object of the specified type.
        /// Rules with higher priority values take precedence over rules with lower priority values.
        /// </summary>
        /// <remarks>
        /// A typical 
        /// </remarks>
        /// <param name="type">The type of object, never null</param>
        /// <returns>The priority of this rule, or null if the rule does not support formatting the specified object type</returns>
        /// <seealso cref="FormattingRulePriority"/> for priority suggestions.
        int? GetPriority(Type type);

        /// <summary>
        /// Formats the specified object.
        /// </summary>
        /// <remarks>
        /// Exceptions thrown by this method will not be reported to clients.
        /// Instead the formatter will substitute an appropriate placeholder string instead.
        /// </remarks>
        /// <param name="obj">The object to format, never null</param>
        /// <param name="formatter">The formatter to use for recursive formatting, never null</param>
        /// <returns>The formatted string representation of the object or null if the object
        /// could not be formatted to produce a non-empty string</returns>
        string Format(object obj, IFormatter formatter);
    }
}
