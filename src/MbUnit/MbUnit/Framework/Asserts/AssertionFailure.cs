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
using System.Collections.ObjectModel;
using System.IO;
using Gallio.Model.Diagnostics;
using Gallio.Model.Logging;
using Gallio.Utilities;

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
    public class AssertionFailure : ITestLogStreamWritable
    {
        private const int MaxPaddedLabelLength = 16;

        /// <summary>
        /// Gets the maximum length of label that can be presented before truncation occurs.
        /// </summary>
        /// <value>100</value>
        public static readonly int MaxLabelLengthBeforeTruncation = 100;

        /// <summary>
        /// Gets the maximum length of formatted value that can be presented before truncation occurs.
        /// </summary>
        /// <value>2000</value>
        public static readonly int MaxFormattedValueLength = 2000;

        private readonly string description;
        private readonly string message;
        private readonly string stackTrace;
        private readonly IList<LabeledValue> labeledValues;
        private readonly IList<ExceptionData> exceptions;

        /// <summary>
        /// Creates an assertion failure object.
        /// </summary>
        protected internal AssertionFailure(string description, string message, string stackTrace,
            IList<LabeledValue> labeledValues, IList<ExceptionData> exceptions)
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
        public IList<LabeledValue> LabeledValues
        {
            get { return new ReadOnlyCollection<LabeledValue>(labeledValues); }
        }

        /// <summary>
        /// Gets information about the exceptions.
        /// </summary>
        public IList<ExceptionData> Exceptions
        {
            get { return new ReadOnlyCollection<ExceptionData>(exceptions); }
        }

        /// <summary>
        /// Writes the assertion failure to a test log stream.
        /// </summary>
        /// <param name="writer">The test log stream</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="writer"/> is null</exception>
        public virtual void WriteTo(TestLogStreamWriter writer)
        {
            if (writer == null)
                throw new ArgumentNullException("writer");

            using (writer.BeginMarker(Marker.AssertionFailure))
            {
                using (writer.BeginSection(description))
                {
                    WriteDetails(writer);
                }
            }
        }

        /// <summary>
        /// Formats the failure as a string.
        /// </summary>
        /// <returns>The formatted string</returns>
        public override string ToString()
        {
            StringTestLogWriter writer = new StringTestLogWriter(false);
            WriteTo(writer.Default);
            return writer.ToString();
        }

        /// <summary>
        /// Writes the details about the assertion failure to the structured text writer.
        /// </summary>
        /// <param name="writer">The structured text writer, not null</param>
        protected virtual void WriteDetails(TestLogStreamWriter writer)
        {
            if (!string.IsNullOrEmpty(message))
                writer.WriteLine(message);

            if (labeledValues.Count != 0)
            {
                writer.WriteLine();

                using (writer.BeginMarker(Marker.Monospace))
                {
                    int paddedLength = ComputePaddedLabelLength();
                    foreach (LabeledValue labeledValue in labeledValues)
                    {
                        WriteLabel(writer, labeledValue.Label);
                        WritePaddingSpaces(writer, paddedLength - labeledValue.Label.Length);
                        writer.Write(@" : ");
                        WriteFormattedValue(writer, labeledValue.FormattedValue);
                        writer.WriteLine();
                    }
                }
            }

            if (exceptions.Count != 0)
            {
                foreach (ExceptionData exception in exceptions)
                {
                    writer.WriteLine();
                    writer.WriteException(exception);
                    writer.WriteLine();
                }
            }

            if (!string.IsNullOrEmpty(stackTrace))
            {
                writer.WriteLine();

                using (writer.BeginMarker(Marker.StackTrace))
                {
                    if (stackTrace.EndsWith("\n"))
                        writer.Write(stackTrace);
                    else
                        writer.WriteLine(stackTrace);
                }
            }
        }

        private int ComputePaddedLabelLength()
        {
            int maxLabelLength = 0;
            foreach (LabeledValue labeledValue in labeledValues)
                if (labeledValue.Label.Length <= MaxPaddedLabelLength)
                    maxLabelLength = Math.Max(maxLabelLength, labeledValue.Label.Length);
            return maxLabelLength;
        }

        private static void WritePaddingSpaces(TextWriter writer, int count)
        {
            while (count-- > 0)
                writer.Write(' ');
        }

        private static void WriteLabel(TestLogStreamWriter writer, string label)
        {
            WriteTruncated(writer, new StructuredText(label), MaxLabelLengthBeforeTruncation);
        }

        private static void WriteFormattedValue(TestLogStreamWriter writer, StructuredText formattedValue)
        {
            WriteTruncated(writer, formattedValue, MaxFormattedValueLength);
        }

        private static void WriteTruncated(TestLogStreamWriter writer, StructuredText text, int maxLength)
        {
            if (text.TruncatedWriteTo(writer, maxLength))
                writer.WriteEllipsis();
        }

        /// <summary>
        /// <para>
        /// A labeled value describes a named assertion parameter.
        /// </para>
        /// <para>
        /// The label indicates the purpose of the value, such as "Expected Value".
        /// The value itself should be formatted to emphasize structural characteristics.
        /// </para>
        /// <para>
        /// For additional emphasis, such as for comparison purposes (ie. diffs), the value may
        /// be formatted as structured text to include highlights and other markup.
        /// </para>
        /// </summary>
        [Serializable]
        public struct LabeledValue
        {
            private readonly string label;
            private readonly StructuredText formattedValue;

            /// <summary>
            /// Creates a labeled value with plain text.
            /// </summary>
            /// <param name="label">The label</param>
            /// <param name="formattedValue">The formatted value as plain text</param>
            /// <exception cref="ArgumentNullException">Thrown if <paramref name="label"/> or
            /// <paramref name="formattedValue"/> is null</exception>
            /// <exception cref="ArgumentException">Thrown if <paramref name="label"/> is empty</exception>
            public LabeledValue(string label, string formattedValue)
                : this(label, new StructuredText(formattedValue))
            {
            }

            /// <summary>
            /// Creates a labeled value with structured text.
            /// </summary>
            /// <param name="label">The label</param>
            /// <param name="formattedValue">The formatted value as structured text</param>
            /// <exception cref="ArgumentNullException">Thrown if <paramref name="label"/> or
            /// <paramref name="formattedValue"/> is null</exception>
            /// <exception cref="ArgumentException">Thrown if <paramref name="label"/> is empty</exception>
            public LabeledValue(string label, StructuredText formattedValue)
            {
                if (label == null)
                    throw new ArgumentNullException("label");
                if (label.Length == 0)
                    throw new ArgumentException("The label must not be empty.", "label");
                if (formattedValue == null)
                    throw new ArgumentNullException("formattedValue");

                this.label = label;
                this.formattedValue = formattedValue;
            }

            /// <summary>
            /// Gets the label.
            /// </summary>
            public string Label
            {
                get { return label; }
            }

            /// <summary>
            /// Gets the formatted value as structured text.
            /// </summary>
            public StructuredText FormattedValue
            {
                get { return formattedValue; }
            }

            /// <inheritdoc />
            public override string ToString()
            {
                return label + @": " + formattedValue;
            }
        }
    }
}
