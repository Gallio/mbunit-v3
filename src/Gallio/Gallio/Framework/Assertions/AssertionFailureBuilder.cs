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
using Gallio.Collections;
using Gallio.Framework.Assertions;
using Gallio.Framework.Text;
using Gallio.Framework.Formatting;
using Gallio.Model.Diagnostics;
using Gallio.Model.Logging;

namespace Gallio.Framework.Assertions
{
    /// <summary>
    /// <para>
    /// An assertion failure builder is used to construct a complete description
    /// of an assertion failure that has occurred.  It takes into account information about
    /// the expected and actual value, as well as additional labeled values and exceptions.
    /// </para>
    /// </summary>
    public class AssertionFailureBuilder
    {
        internal static readonly int CompressedDiffContextLength = 50;

        private readonly string description;
        private readonly IFormatter formatter;

        private string message;
        private string stackTrace;
        private bool isStackTraceSet;
        private List<AssertionFailure.LabeledValue> labeledValues;
        private List<ExceptionData> exceptions;

        /// <summary>
        /// Creates an assertion failure builder with the default formatter.
        /// </summary>
        /// <param name="description">The description of the failure</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="description"/>
        /// is null</exception>
        public AssertionFailureBuilder(string description)
            : this(description, Gallio.Framework.Formatting.Formatter.Instance)
        {
        }

        /// <summary>
        /// Creates an assertion failure builder with the specified formatter.
        /// </summary>
        /// <param name="description">The description of the failure</param>
        /// <param name="formatter">The formatter to use</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="description"/> or
        /// <paramref name="formatter"/>  is null</exception>
        public AssertionFailureBuilder(string description, IFormatter formatter)
        {
            if (description == null)
                throw new ArgumentNullException("description");
            if (formatter == null)
                throw new ArgumentNullException("formatter");

            this.description = description;
            this.formatter = formatter;
        }

        /// <summary>
        /// Gets the formatted used by the builder.
        /// </summary>
        public IFormatter Formatter
        {
            get { return formatter; }
        }

        /// <summary>
        /// Sets the user-supplied assertion failure message.
        /// </summary>
        /// <param name="message">The user-supplied assertion failure message, or null if none</param>
        /// <returns>The builder, to allow for fluent method chaining</returns>
        public AssertionFailureBuilder SetMessage(string message)
        {
            this.message = message;
            return this;
        }

        /// <summary>
        /// Sets the user-supplied assertion failure message to a formatted value.
        /// </summary>
        /// <param name="messageFormat">The user-supplied assertion failure message format
        /// string, or null if none</param>
        /// <param name="messageArgs">The format arguments, or null or empty if none</param>
        /// <returns>The builder, to allow for fluent method chaining</returns>
        public AssertionFailureBuilder SetMessage(string messageFormat, params object[] messageArgs)
        {
            message = messageFormat != null && messageArgs != null
                ? String.Format(messageFormat, messageArgs)
                : messageFormat;
            return this;
        }

        /// <summary>
        /// Sets the stack trace.
        /// </summary>
        /// <param name="stackTrace">The stack trace, or null if none</param>
        /// <returns>The builder, to allow for fluent method chaining</returns>
        public AssertionFailureBuilder SetStackTrace(string stackTrace)
        {
            this.stackTrace = stackTrace;
            isStackTraceSet = true;
            return this;
        }

        /// <summary>
        /// <para>
        /// Sets the raw expected value to be formatted using <see cref="Formatter" />.
        /// </para>
        /// <para>
        /// The order in which this method is called determines the order in which this
        /// value will appear relative to other labeled values.
        /// </para>
        /// </summary>
        /// <remarks>
        /// This is a convenience method for setting a labeled value called "Expected Value".
        /// </remarks>
        /// <param name="expectedValue">The expected value</param>
        /// <returns>The builder, to allow for fluent method chaining</returns>
        public AssertionFailureBuilder SetRawExpectedValue(object expectedValue)
        {
            return SetRawLabeledValue("Expected Value", expectedValue);
        }

        /// <summary>
        /// <para>
        /// Sets the raw actual value to be formatted using <see cref="Formatter" />.
        /// </para>
        /// <para>
        /// The order in which this method is called determines the order in which this
        /// value will appear relative to other labeled values.
        /// </para>
        /// </summary>
        /// <remarks>
        /// This is a convenience method for setting a labeled value called "Actual Value".
        /// </remarks>
        /// <param name="actualValue">The actual value</param>
        /// <returns>The builder, to allow for fluent method chaining</returns>
        public AssertionFailureBuilder SetRawActualValue(object actualValue)
        {
            return SetRawLabeledValue("Actual Value", actualValue);
        }

        /// <summary>
        /// <para>
        /// Sets the raw expected and actual values to be formatted using <see cref="Formatter" />
        /// and includes formatting of their differences.
        /// </para>
        /// <para>
        /// The order in which this method is called determines the order in which the
        /// values will appear relative to other labeled values.
        /// </para>
        /// </summary>
        /// <remarks>
        /// This is a convenience method for setting a pair of labeled values called "Expected Value"
        /// and "Actual Value" and including formatting of differences produced by <see cref="DiffSet"/>.
        /// </remarks>
        /// <param name="expectedValue">The expected value</param>
        /// <param name="actualValue">The actual value</param>
        /// <returns>The builder, to allow for fluent method chaining</returns>
        public AssertionFailureBuilder SetRawExpectedAndActualValueWithDiffs(object expectedValue, object actualValue)
        {
            if (ReferenceEquals(expectedValue, actualValue))
            {
                SetRawLabeledValue("Expected & Actual Value", expectedValue);
                SetLabeledValue("Remark", "The expected and actual values are the same instance.");
            }
            else
            {
                string formattedExpectedValue = Formatter.Format(expectedValue);
                string formattedActualValue = Formatter.Format(actualValue);

                if (formattedExpectedValue == formattedActualValue)
                {
                    SetLabeledValue("Expected & Actual Value", formattedExpectedValue);
                    SetLabeledValue("Remark", "The expected and actual values are distinct instances but their formatted representations look the same.");
                }
                else
                {
                    DiffSet diffSet = DiffSet.GetDiffSet(formattedExpectedValue, formattedActualValue);

                    StructuredTextWriter highlightedExpectedValueWriter = new StructuredTextWriter();
                    StructuredTextWriter highlightedActualValueWriter = new StructuredTextWriter();

                    diffSet.WriteTo(highlightedExpectedValueWriter, DiffStyle.LeftOnly,
                        formattedExpectedValue.Length <= AssertionFailure.MaxFormattedValueLength ? int.MaxValue : CompressedDiffContextLength);
                    diffSet.WriteTo(highlightedActualValueWriter, DiffStyle.RightOnly,
                        formattedActualValue.Length <= AssertionFailure.MaxFormattedValueLength ? int.MaxValue : CompressedDiffContextLength);
 
                    SetLabeledValue("Expected Value", highlightedExpectedValueWriter.ToStructuredText());
                    SetLabeledValue("Actual Value", highlightedActualValueWriter.ToStructuredText());
                }
            }

            return this;
        }

        /// <summary>
        /// <para>
        /// Sets a raw labeled value to be formatted using <see cref="Formatter" />.
        /// </para>
        /// <para>
        /// The order in which this method is called determines the order in which this
        /// labeled value will appear relative to other labeled values.
        /// </para>
        /// <para>
        /// If a value is already associated with the specified label, replaces its value and
        /// repositions it at the end of the list.
        /// </para>
        /// </summary>
        /// <param name="label">The label</param>
        /// <param name="value">The raw unformatted value</param>
        /// <returns>The builder, to allow for fluent method chaining</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="label"/> is null</exception>
        /// <exception cref="ArgumentException">Thrown if <paramref name="label"/> is empty</exception>
        public AssertionFailureBuilder SetRawLabeledValue(string label, object value)
        {
            return SetLabeledValue(label, Formatter.Format(value));
        }

        /// <summary>
        /// <para>
        /// Sets a labeled value as plain text.
        /// </para>
        /// <para>
        /// The order in which this method is called determines the order in which this
        /// labeled value will appear relative to other labeled values.
        /// </para>
        /// <para>
        /// If a value is already associated with the specified label, replaces its value and
        /// repositions it at the end of the list.
        /// </para>
        /// </summary>
        /// <param name="label">The label</param>
        /// <param name="formattedValue">The formatted value</param>
        /// <returns>The builder, to allow for fluent method chaining</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="label"/> or <paramref name="formattedValue"/> is null</exception>
        /// <exception cref="ArgumentException">Thrown if <paramref name="label"/> is empty</exception>
        public AssertionFailureBuilder SetLabeledValue(string label, string formattedValue)
        {
            SetLabeledValueImpl(new AssertionFailure.LabeledValue(label, formattedValue));
            return this;
        }

        /// <summary>
        /// <para>
        /// Sets a labeled value as structured text.
        /// </para>
        /// <para>
        /// The order in which this method is called determines the order in which this
        /// labeled value will appear relative to other labeled values.
        /// </para>
        /// <para>
        /// If a value is already associated with the specified label, replaces its value and
        /// repositions it at the end of the list.
        /// </para>
        /// </summary>
        /// <param name="label">The label</param>
        /// <param name="formattedValue">The formatted value as structured text</param>
        /// <returns>The builder, to allow for fluent method chaining</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="label"/> or <paramref name="formattedValue"/> is null</exception>
        /// <exception cref="ArgumentException">Thrown if <paramref name="label"/> is empty</exception>
        public AssertionFailureBuilder SetLabeledValue(string label, StructuredText formattedValue)
        {
            SetLabeledValueImpl(new AssertionFailure.LabeledValue(label, formattedValue));
            return this;
        }

        /// <summary>
        /// <para>
        /// Sets a labeled value.
        /// </para>
        /// <para>
        /// The order in which this method is called determines the order in which this
        /// value will appear relative to other labeled values.
        /// </para>
        /// <para>
        /// If a value is already associated with the specified label, replaces and
        /// repositions it at the end of the list.
        /// </para>
        /// </summary>
        /// <param name="labeledValue">The labeled value</param>
        /// <returns>The builder, to allow for fluent method chaining</returns>
        public AssertionFailureBuilder SetLabeledValue(AssertionFailure.LabeledValue labeledValue)
        {
            SetLabeledValueImpl(labeledValue);
            return this;
        }

        /// <summary>
        /// Adds an exception to the assertion failure.
        /// </summary>
        /// <param name="ex">The exception to add</param>
        /// <returns>The builder, to allow for fluent method chaining</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="ex"/> is null</exception>
        public AssertionFailureBuilder AddException(Exception ex)
        {
            if (ex == null)
                throw new ArgumentNullException("ex");

            return AddException(new ExceptionData(ex));
        }

        /// <summary>
        /// Adds an exception to the assertion failure.
        /// </summary>
        /// <param name="ex">The exception data to add</param>
        /// <returns>The builder, to allow for fluent method chaining</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="ex"/> is null</exception>
        public AssertionFailureBuilder AddException(ExceptionData ex)
        {
            if (ex == null)
                throw new ArgumentNullException("ex");

            if (exceptions == null)
                exceptions = new List<ExceptionData>();
            exceptions.Add(StackTraceFilter.FilterException(ex));
            return this;
        }

        /// <summary>
        /// Generates an immutable object that describes the failure.
        /// </summary>
        /// <returns>The assertion failure</returns>
        [TestFrameworkInternal]
        public AssertionFailure ToAssertionFailure()
        {
            return CreateAssertionFailure(description, message, GetStackTraceOrDefault(),
                GetLabeledValuesAsArray(), GetExceptionsAsArray());
        }

        /// <summary>
        /// Creates an assertion failure object.
        /// </summary>
        /// <remarks>
        /// Subclasses may override this method to define custom extended assertion
        /// failure objects.
        /// </remarks>
        protected virtual AssertionFailure CreateAssertionFailure(string description,
            string message, string stackTrace, IList<AssertionFailure.LabeledValue> labeledValues,
            IList<ExceptionData> exceptions)
        {
            return new AssertionFailure(description, message, stackTrace, labeledValues, exceptions);
        }

        private void SetLabeledValueImpl(AssertionFailure.LabeledValue labeledValue)
        {
            if (labeledValues == null)
                labeledValues = new List<AssertionFailure.LabeledValue>();

            for (int i = 0; i < labeledValues.Count; i++)
            {
                if (labeledValues[i].Label == labeledValue.Label)
                {
                    labeledValues.RemoveAt(i);
                    break;
                }
            }

            labeledValues.Add(labeledValue);
        }

        private AssertionFailure.LabeledValue[] GetLabeledValuesAsArray()
        {
            return labeledValues != null ? labeledValues.ToArray() : EmptyArray<AssertionFailure.LabeledValue>.Instance;
        }

        private ExceptionData[] GetExceptionsAsArray()
        {
            return exceptions != null ? exceptions.ToArray() : EmptyArray<ExceptionData>.Instance;
        }

        [TestFrameworkInternal]
        private string GetStackTraceOrDefault()
        {
            if (isStackTraceSet)
                return stackTrace;

            return StackTraceFilter.CaptureFilteredStackTrace();
        }
    }
}