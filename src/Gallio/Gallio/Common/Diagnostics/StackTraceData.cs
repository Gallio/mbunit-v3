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
using System.Globalization;
using System.Text.RegularExpressions;
using Gallio.Common.Markup;
using Gallio.Common.Normalization;
using Gallio.Common.Reflection;

namespace Gallio.Common.Diagnostics
{
    /// <summary>
    /// Describes a stack trace in a serializable form.
    /// </summary>
    [Serializable]
    public sealed class StackTraceData : IMarkupStreamWritable, INormalizable<StackTraceData>
    {
        private static readonly Regex StackFrameRegex = new Regex(@"(?<prefix>\) in )(?<path>\S.*):line (?<line>[0-9]+)",
            RegexOptions.Multiline | RegexOptions.Compiled | RegexOptions.CultureInvariant);

        private readonly string stackTrace;

        /// <summary>
        /// Creates a stack trace data object from a string.
        /// </summary>
        /// <param name="stackTrace">The stack trace.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="stackTrace"/> is null.</exception>
        public StackTraceData(string stackTrace)
        {
            if (stackTrace == null)
                throw new ArgumentNullException("stackTrace");

            this.stackTrace = stackTrace.EndsWith("\n") ? stackTrace.Substring(0, stackTrace.Length - 1) : stackTrace;
        }

        /// <summary>
        /// Creates a stack trace data object that points to the specified code element.
        /// </summary>
        /// <param name="codeElement">The code element.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="codeElement"/> is null.</exception>
        public StackTraceData(ICodeElementInfo codeElement)
        {
            if (codeElement == null)
                throw new ArgumentNullException("codeElement");

            var codeLocation = codeElement.GetCodeLocation();
            var codeReference = codeElement.CodeReference;
            this.stackTrace = String.Format("   at {0}\n   at {1}.{2}() in {3}:line {4}",
                codeReference.MemberName, codeReference.NamespaceName, codeReference.TypeName, codeLocation.Path, codeLocation.Line);
        }

        /// <summary>
        /// Returns true if the stack trace data is empty.
        /// </summary>
        public bool IsEmpty
        {
            get { return stackTrace.Length == 0; }
        }

        /// <inheritdoc />
        public StackTraceData Normalize()
        {
            string normalizedStackTrace = NormalizationUtils.NormalizeXmlText(stackTrace);

            if (ReferenceEquals(stackTrace, normalizedStackTrace))
                return this;

            return new StackTraceData(normalizedStackTrace);
        }

        /// <summary>
        /// Formats the stack trace to a string similar to the one that the .Net framework
        /// would ordinarily construct.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The exception will not be terminated by a new line.
        /// </para>
        /// </remarks>
        /// <returns>The formatted stack trace.</returns>
        public override string ToString()
        {
            return stackTrace;
        }

        /// <summary>
        /// Writes the stack trace in a structured format with markers to distinguish its component elements.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The stack trace will not be terminated by a new line.
        /// </para>
        /// </remarks>
        /// <param name="writer">The log stream writer.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="writer"/> is null.</exception>
        public void WriteTo(MarkupStreamWriter writer)
        {
            if (writer == null)
                throw new ArgumentNullException("writer");

            if (IsEmpty)
                return;

            using (writer.BeginMarker(Marker.StackTrace))
            {
                int pos = 0;
                foreach (Match match in StackFrameRegex.Matches(stackTrace))
                {
                    if (match.Index != pos)
                        writer.Write(stackTrace.Substring(pos, match.Index - pos));

                    string prefix = match.Groups["prefix"].Value;
                    writer.Write(prefix);

                    string path = match.Groups["path"].Value;
                    int line;
                    int.TryParse(match.Groups["line"].Value, NumberStyles.None, CultureInfo.InvariantCulture, out line);

                    using (writer.BeginMarker(Marker.CodeLocation(new CodeLocation(path, line, 0))))
                        writer.Write(match.Value.Substring(prefix.Length));

                    pos = match.Index + match.Length;
                }

                if (pos < stackTrace.Length)
                    writer.Write(stackTrace.Substring(pos));
            }
        }
    }
}
