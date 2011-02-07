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
    /// Provides helper methods to ease HTML formating from VTL template engine.
    /// </summary>
    internal class FormatHtmlHelper
    {
        /// <summary>
        /// Normalizes the end of lines for HTML-based formats.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Replaces LF and CR/LF characters by a HTML breaking line tag.
        /// </para>
        /// </remarks>
        /// <param name="text">The text to be normalized.</param>
        /// <returns>The normalized text.</returns>
        public string NormalizeEndOfLines(string text)
        {
            return text
                .Replace("\r\n", "<br>")
                .Replace("\n", "<br>");
        }

        /// <summary>
        /// Inserts HTML break word tags where it is necessary.
        /// </summary>
        /// <param name="text">The text to process.</param>
        /// <returns>The processed text.</returns>
        public string BreakWord(string text)
        {
            var output = new StringBuilder();

            foreach (char @char in text)
            {
                switch (@char)
                {
                    // Natural word breaks. Always replace spaces by non-breaking spaces followed by word-breaks to ensure that
                    // text can reflow without actually consuming the space.  Without this detail it can happen that spaces that 
                    // are supposed to be highligted (perhaps as part of a marker for a diff) will instead vanish when the text 
                    // reflow occurs, giving a false impression of the content.
                    case ' ':
                        output.Append("&nbsp;<wbr/>");
                        break;

                    // Characters to break before.
                    case '_':
                    case '/':
                    case ';':
                    case ':':
                    case '.':
                    case '\\':
                    case '(':
                    case '{':
                    case '[':
                        output.Append("<wbr/>");
                        output.Append(@char);
                        break;

                    // Characters to break after.
                    case '>':
                    case ')':
                    case ']':
                    case '}':
                        output.Append(@char);
                        output.Append("<wbr/>");
                        break;

                    default:
                        output.Append(@char);
                        break;
                }
            }

            return output.ToString();
        }

        /// <summary>
        /// Transforms a file path to an URI.
        /// </summary>
        /// <param name="path">The path to transform.</param>
        /// <returns>The resulting URI.</returns>
        public string PathToUri(string path)
        { 
            return path
                .Replace('\\', '/')
                .Replace("%", "%25")
                .Replace(" ", "%20");
        }

        /// <summary>
        /// Generates a unique id (GUID).
        /// </summary>
        /// <returns>A unique id.</returns>
        public string GenerateId()
        {
            return Guid.NewGuid().ToString("N");
        }

        /// <summary>
        /// Removes new lines and unecessary spaces to make the XML file smaller.
        /// </summary>
        /// <param name="xml"></param>
        public static string Flatten(string xml)
        {
            return Regex.Replace(xml, @"\r?\n\s*", String.Empty);
        }
    }
}
