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
using System.Collections.Generic;
using System.IO;
using System.Text;
using Gallio.Framework;

namespace MbUnit.Framework
{
    /// <summary>
    /// <para>
    /// An assertion failure is an immutable description of a failed assertion and its
    /// cause.  It takes into account information about labeled values such as the expected
    /// and actual value.
    /// </para>
    /// <para>
    /// Use an <see cref="AssertionFailureBuilder" /> to generate an instance of this type.
    /// </para>
    /// </summary>
    [Serializable]
    public sealed class AssertionFailure
    {
        private readonly string description;
        private readonly string message;
        private readonly string stackTrace;
        private readonly KeyValuePair<string, string>[] labeledValues;
        private readonly string[] exceptions;

        internal AssertionFailure(string description, string message, string stackTrace,
            KeyValuePair<string, string>[] labeledValues, string[] exceptions)
        {
            this.description = description;
            this.message = message;
            this.stackTrace = stackTrace;
            this.labeledValues = labeledValues;
            this.exceptions = exceptions;
        }

        /// <summary>
        /// Gets the description of the assertion failure.
        /// </summary>
        public string Description
        {
            get { return description; }
        }

        /// <summary>
        /// Gets the user-supplied message about the assertion failure,
        /// or null if none.
        /// </summary>
        public string Message
        {
            get { return message; }
        }

        /// <summary>
        /// Get the stack track of the failure, or null if none.
        /// </summary>
        public string StackTrace
        {
            get { return stackTrace; }
        }

        /// <summary>
        /// Gets formatted representations of labeled values as key/value pairs.
        /// </summary>
        public IList<KeyValuePair<string, string>> LabeledValues
        {
            get { return Array.AsReadOnly(labeledValues); }
        }

        /// <summary>
        /// Gets formatted representations of exceptions.
        /// </summary>
        public IList<string> Exceptions
        {
            get { return Array.AsReadOnly(exceptions); }
        }

        /// <summary>
        /// Logs the assertion failure.
        /// </summary>
        /// <param name="logWriter">The log writer</param>
        public void Log(LogWriter logWriter)
        {
            using (logWriter.Failures.BeginSection(description))
            {
                WriteDetails(logWriter.Failures);
            }
        }

        /// <summary>
        /// Formats the failure as a string.
        /// </summary>
        /// <returns>The formatted string</returns>
        public override string ToString()
        {
            StringWriter writer = new StringWriter();
            writer.WriteLine(description);
            WriteDetails(writer);
            return writer.ToString();
        }

        private void WriteDetails(TextWriter writer)
        {
            if (!string.IsNullOrEmpty(message))
                writer.WriteLine(message);

            int maxLabelLength = ComputeMaxLabelLength();
            foreach (KeyValuePair<string, string> pair in labeledValues)
            {
                writer.Write("* ");
                writer.Write(pair.Key);
                WriteSpaces(writer, maxLabelLength - pair.Key.Length);
                writer.Write(": ");
                writer.WriteLine(pair.Value);
            }

            if (!string.IsNullOrEmpty(stackTrace))
            {
                writer.WriteLine();
                WriteWithNoExcessNewLine(writer, stackTrace);
            }

            if (exceptions.Length != 0)
            {
                foreach (string exception in exceptions)
                {
                    writer.WriteLine();
                    WriteWithNoExcessNewLine(writer, exception);
                }
            }
        }

        private void WriteWithNoExcessNewLine(TextWriter writer, string text)
        {
            writer.Write(text);

            if (!stackTrace.EndsWith("\n"))
                writer.WriteLine();
        }

        private int ComputeMaxLabelLength()
        {
            int maxLabelLength = 0;
            foreach (KeyValuePair<string, string> pair in labeledValues)
                maxLabelLength = Math.Max(maxLabelLength, pair.Key.Length);
            return maxLabelLength;
        }

        private static void WriteSpaces(TextWriter writer, int count)
        {
            while (count-- > 0)
                writer.Write(' ');
        }
    }
}
