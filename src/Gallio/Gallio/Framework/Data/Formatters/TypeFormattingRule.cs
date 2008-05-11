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

namespace Gallio.Framework.Data.Formatters
{
    /// <summary>
    /// <para>
    /// A formatting rule for <see cref="Type" />.
    /// </para>
    /// <para>
    /// Formats values like: System.String, MyType+Nested, System.Int32[]
    /// </para>
    /// </summary>
    public sealed class TypeFormattingRule : IFormattingRule
    {
        /// <inheritdoc />
        public int? GetPriority(Type type)
        {
            if (typeof(Type).IsAssignableFrom(type))
                return FormattingRulePriority.Best;
            return null;
        }

        /// <inheritdoc />
        public string Format(object obj, IFormatter formatter)
        {
            Type value = (Type)obj;
            return value.ToString();
        }
    }
}