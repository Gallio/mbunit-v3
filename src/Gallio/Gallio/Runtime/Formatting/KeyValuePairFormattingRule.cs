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
using System.Reflection;

namespace Gallio.Runtime.Formatting
{
    /// <summary>
    /// <para>
    /// A formatting rule for <see cref="KeyValuePair{TKey,TValue}" />.
    /// </para>
    /// <para>
    /// Formats values as "\"key\": \"value\"".
    /// </para>
    /// </summary>
    public sealed class KeyValuePairFormattingRule : IFormattingRule
    {
        /// <inheritdoc />
        public int? GetPriority(Type type)
        {
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(KeyValuePair<,>))
                return FormattingRulePriority.Best;
            return null;
        }

        /// <inheritdoc />
        public string Format(object obj, IFormatter formatter)
        {
            Type type = obj.GetType();
            PropertyInfo keyProperty = type.GetProperty("Key");
            PropertyInfo valueProperty = type.GetProperty("Value");

            object key = keyProperty.GetValue(obj, null);
            object value = valueProperty.GetValue(obj, null);

            return string.Concat(formatter.Format(key), ": ", formatter.Format(value));
        }
    }
}