// Copyright 2005-2008 Gallio Project - http://www.gallio.org/
// Portions Copyright 2000-2004 Jonathan De Halleux, Jamie Cansdale
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
using System.Globalization;

namespace Gallio.Framework.Data.Formatters
{
    /// <summary>
    /// <para>
    /// A formatting rule for <see cref="float" />.
    /// </para>
    /// <para>
    /// Formats values like: "5.6f".
    /// </para>
    /// </summary>
    public sealed class SingleFormattingRule : IFormattingRule
    {
        /// <inheritdoc />
        public int? GetPriority(Type type)
        {
            if (type == typeof(float))
                return FormattingRulePriority.Best;
            return null;
        }

        /// <inheritdoc />
        public string Format(object obj, IFormatter formatter)
        {
            float value = (float)obj;
            return value.ToString(CultureInfo.InvariantCulture) + @"f";
        }
    }
}