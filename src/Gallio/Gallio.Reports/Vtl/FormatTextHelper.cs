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
using NVelocity;
using NVelocity.App;
using Gallio.Runner.Reports;
using System.Text.RegularExpressions;
using Gallio.Model;
using Gallio.Runner.Reports.Schema;
using Gallio.Common.Collections;
using Gallio.Common.Markup.Tags;
using Gallio.Common.Markup;

namespace Gallio.Reports.Vtl
{
    /// <summary>
    /// Provides helper methods to ease text formating from VTL template engine.
    /// </summary>
    internal class FormatTextHelper
    {
        /// <summary>
        /// Normalizes the end of lines for text-based formats.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Replaces single LF characters by CR/LF pairs.
        /// </para>
        /// </remarks>
        /// <param name="text">The text to be normalized.</param>
        /// <returns>The normalized text.</returns>
        public string NormalizeEndOfLinesText(string text)
        {
            return text.Replace("\n", "\r\n");
        }

        /// <summary>
        /// Removes characters from the specified text.
        /// </summary>
        /// <param name="text">The text to process.</param>
        /// <param name="chars">The characters to remove from the string.</param>
        /// <returns>The processed text.</returns>
        public string RemoveChars(string text, string chars)
        {
            var builder = new StringBuilder(text);

            foreach (char @char in chars)
                builder.Replace(@char.ToString(), String.Empty);

            return builder.ToString();
        }
    }
}
