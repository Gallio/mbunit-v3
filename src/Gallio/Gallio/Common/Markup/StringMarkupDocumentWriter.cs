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
using System.IO;

namespace Gallio.Common.Markup
{
    /// <summary>
    /// <para>
    /// An implementation of <see cref="MarkupDocumentWriter" /> that writes its output to a string.
    /// </para>
    /// </summary>
    public class StringMarkupDocumentWriter : TextualMarkupDocumentWriter
    {
        /// <summary>
        /// Creates a log stream writer for the <see cref="MarkupStreamNames.Default" /> stream.
        /// </summary>
        /// <param name="verbose">If true, prints detailed information about the location of
        /// attachments, sections, and markers, otherwise discards these formatting details
        /// and prints section headers as text on their own line</param>
        public StringMarkupDocumentWriter(bool verbose)
            : base(CreateStringWriter(), verbose)
        {
        }

        /// <summary>
        /// Gets the formatted log contents as a string.
        /// </summary>
        /// <returns>The contents as a string</returns>
        public override string ToString()
        {
            return Writer.ToString();
        }

        private static TextWriter CreateStringWriter()
        {
            StringWriter writer = new StringWriter();
            writer.NewLine = "\n";
            return writer;
        }
    }
}
