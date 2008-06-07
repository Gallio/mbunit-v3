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
using System.Collections;

namespace Gallio.Framework.Formatting
{
    /// <summary>
    /// <para>
    /// A formatting rule for <see cref="DictionaryEntry" />.
    /// </para>
    /// <para>
    /// Formats values as "\"key\": \"value\"".
    /// </para>
    /// </summary>
    public sealed class DictionaryEntryFormattingRule : IFormattingRule
    {
        /// <inheritdoc />
        public int? GetPriority(Type type)
        {
            if (type == typeof(DictionaryEntry))
                return FormattingRulePriority.Best;
            return null;
        }

        /// <inheritdoc />
        public string Format(object obj, IFormatter formatter)
        {
            DictionaryEntry entry = (DictionaryEntry)obj;
            return string.Concat(formatter.Format(entry.Key), ": ", formatter.Format(entry.Value));
        }
    }
}