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
using System.Linq;
using System.Text;

namespace Gallio.Runtime.Formatting
{
    /// <summary>
    /// Extensions methods for formatting.
    /// </summary>
    public static class FormatterExtensions
    {
        /// <summary>
        /// Formats an object using the default <see cref="IFormatter" />.
        /// </summary>
        /// <param name="obj">The object to format</param>
        /// <returns>The formatted object</returns>
        public static string Format(this object obj)
        {
            return Format(obj, null);
        }

        /// <summary>
        /// Formats an object using the specified <see cref="IFormatter" />.
        /// </summary>
        /// <param name="obj">The object to format</param>
        /// <param name="formatter">The formatter to use, or null for the default</param>
        /// <returns>The formatted object</returns>
        public static string Format(this object obj, IFormatter formatter)
        {
            return (formatter ?? Formatter.Instance).Format(obj);
        }
    }
}
