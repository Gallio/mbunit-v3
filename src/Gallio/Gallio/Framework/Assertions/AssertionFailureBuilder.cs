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
using Gallio.Common.Collections;
using Gallio.Framework.Text;
using Gallio.Runtime.Formatting;
using Gallio.Runtime.Diagnostics;
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
        private StackTraceData stackTrace;
        private bool isStackTraceSet;
        private List<AssertionFailure.LabeledValue> labeledValues;
        private List<ExceptionData> exceptions;
        private List<AssertionFailure> innerFailures;

        /// <summary>
        /// Creates an assertion failure builder with the default formatter.
        /// </summary>
        /// <param name="description">The description of the failure</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="description"/>
        /// is null</exception>
        public AssertionFailureBuilder(string description)
            : this(description, Runtime.Formatting.Formatter.Instance)
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
        public AssertionFailureBuilder SetStackTrace(StackTraceData stackTrace)
        {
            this.stackTrace = stackTrace;
            isStackTraceSet = true;
            return this;
        }

        /// <summary>
        /// <para>
        /// Adds the raw expected value to be formatted using <see cref="Formatter" />.
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
        public AssertionFailureBuilder AddRawExpectedValue(object expectedValue)
        {
            return AddRawLabeledValue("Expected Value", expectedValue);
        }

        /// <summary>
        /// <para>
        /// Adds the raw actual value to be formatted using <see cref="Formatter" />.
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
        public AssertionFailureBuilder AddRawActualValue(object actualValue)
        {
            return AddRawLabeledValue("Actual Value", actualValue);
        }

        /// <summary>
        /// <para>
        /// Adds the raw expected and actual values to be formatted using <see cref="Formatter" />
        /// and includes formatting of their differences.
        /// </para>
        /// <para>
        /// The order in which this method is called determines the order in which the
        /// values will appear relative to other labeled values.
        /// </para>
        /// </summary>
        /// <remarks>
        /// This is a convenience method for setting a pair of labeled values called "Expected Value"
        /// and "Actual Value" with diffs.
        /// </remarks>
        /// <param name="expectedValue">The expected value</param>
        /// <param name="actualValue">The actual value</param>
        /// <returns>The builder, to allow for fluent method chaining</returns>
        public AssertionFailureBuilder AddRawExpectedAndActualValuesWithDiffs(object expectedValue, object actualValue)
        {
            return AddRawLabeledValuesWithDiffs("Expected Value", expectedValue, "Actual Value", actualValue);
        }

        /// <summary>
        /// <para>
        /// Adds two raw labeled values formatted using <see cref="Formatter" /> and includes
        /// formatting of their differences.
        /// </para>
        /// <para>
        /// The order in which this method is called determines the order in which the
        /// values will appear relative to other labeled values.
        /// </para>
        /// </summary>
        /// <param name="leftLabel">The left label</param>
        /// <param name="leftValue">The left value</param>
        /// <param name="rightLabel">The right label</param>
        /// <param name="rightValue">The right value</param>
        /// <returns>The builder, to allow for fluent method chaining</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="leftLabel"/> or
        /// <paramref name="rightLabel"/> is null</exception>
        public AssertionFailureBuilder AddRawLabeledValuesWithDiffs(
            string leftLabel, object leftValue, string rightLabel, object rightValue)
        {
            if (leftLabel == null)
                throw new ArgumentNullException("leftLabel");
            if (rightLabel == null)
                throw new ArgumentNullException("rightLabel");

            if (ReferenceEquals(leftValue, rightValue))
            {
                AddRawLabeledValue(String.Format("{0} & {1}", leftLabel, rightLabel), leftValue);
                AddLabeledValue("Remark", "Both values are the same instance.");
            }
            else
            {
                string formattedLeftValue = Formatter.Format(leftValue);
                string formattedRightValue = Formatter.Format(rightValue);

                if (formattedLeftValue == formattedRightValue)
                {
                    AddLabeledValue(String.Format("{0} & {1}", leftLabel, rightLabel), formattedLeftValue);
                    AddLabeledValue("Remark", "Both values look the same when formatted but they are distinct instances.");
                }
                else
                {
                    DiffSet diffSet = DiffSet.GetDiffSet(formattedLeftValue, formattedRightValue);
                    diffSet = diffSet.Simplify();

                    StructuredTextWriter highlightedLeftValueWriter = new StructuredTextWriter();
                    StructuredTextWriter highlightedRightValueWriter = new StructuredTextWriter();

                    diffSet.WriteTo(highlightedLeftValueWriter, DiffStyle.LeftOnly,
                        formattedLeftValue.Length <= AssertionFailure.MaxFormattedValueLength ? int.MaxValue : CompressedDiffContextLength);
                    diffSet.WriteTo(highlightedRightValueWriter, DiffStyle.RightOnly,
                        formattedRightValue.Length <= AssertionFailure.MaxFormattedValueLength ? int.MaxValue : CompressedDiffContextLength);

                    AddLabeledValue(leftLabel, highlightedLeftValueWriter.ToStructuredText());
                    AddLabeledValue(rightLabel, highlightedRightValueWriter.ToStructuredText());
                }
            }

            return this;
        }

        /// <summary>
        /// <para>
        /// Adds a raw labeled value to be formatted using <see cref="Formatter" />.
        /// </para>
        /// <para>
        /// The order in which this method is called determines the order in which this
        /// labeled value will appear relative to other labeled values.
        /// </para>
        /// </summary>
        /// <param name="label">The label</param>
        /// <param name="value">The raw unformatted value</param>
        /// <returns>The builder, to allow for fluent method chaining</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="label"/> is null</exception>
        /// <exception cref="ArgumentException">Thrown if <paramref name="label"/> is empty</exception>
        public AssertionFailureBuilder AddRawLabeledValue(string label, object value)
        {
            return AddLabeledValue(label, Formatter.Format(value));
        }

        /// <summary>
        /// <para>
        /// Adds a labeled value as plain text.
        /// </para>
        /// <para>
        /// The order in which this method is called determines the order in which this
        /// labeled value will appear relative to other labeled values.
        /// </para>
        /// </summary>
        /// <param name="label">The label</param>
        /// <param name="formattedValue">The formatted value</param>
        /// <returns>The builder, to allow for fluent method chaining</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="label"/> or <paramref name="formattedValue"/> is null</exception>
        /// <exception cref="ArgumentException">Thrown if <paramref name="label"/> is empty</exception>
        public AssertionFailureBuilder AddLabeledValue(string label, string formattedValue)
        {
            AddLabeledValueImpl(new AssertionFailure.LabeledValue(label, formattedValue));
            return this;
        }

        /// <summary>
        /// <para>
        /// Adds a labeled value as structured text.
        /// </para>
        /// <para>
        /// The order in which this method is called determines the order in which this
        /// labeled value will appear relative to other labeled values.
        /// </para>
        /// </summary>
        /// <param name="label">The label</param>
        /// <param name="formattedValue">The formatted value as structured text</param>
        /// <returns>The builder, to allow for fluent method chaining</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="label"/> or <paramref name="formattedValue"/> is null</exception>
        /// <exception cref="ArgumentException">Thrown if <paramref name="label"/> is empty</exception>
        public AssertionFailureBuilder AddLabeledValue(string label, StructuredText formattedValue)
        {
            AddLabeledValueImpl(new AssertionFailure.LabeledValue(label, formattedValue));
            return this;
        }

        /// <summary>
        /// <para>
        /// Adds a labeled value.
        /// </para>
        /// <para>
        /// The order in which this method is called determines the order in which this
        /// value will appear relative to other labeled values.
        /// </para>
        /// </summary>
        /// <param name="labeledValue">The labeled value</param>
        /// <returns>The builder, to allow for fluent method chaining</returns>
        public AssertionFailureBuilder AddLabeledValue(AssertionFailure.LabeledValue labeledValue)
        {
            AddLabeledValueImpl(labeledValue);
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
        /// Adds a nested assertion failure that contributed to the composite assertion failure
        /// described by this instance.
        /// </summary>
        /// <param name="innerFailure">The inner assertion failure to add</param>
        /// <returns>The builder, to allow for fluent method chaining</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="innerFailure"/> is null</exception>
        public AssertionFailureBuilder AddInnerFailure(AssertionFailure innerFailure)
        {
            if (innerFailure == null)
                throw new ArgumentNullException("innerFailure");

            if (innerFailures == null)
                innerFailures = new List<AssertionFailure>();
            innerFailures.Add(innerFailure);
            return this;
        }

        /// <summary>
        /// Adds an enumeration of nested assertion failures that contributed to the composite
        /// assertion failure described by this instance.
        /// </summary>
        /// <param name="innerFailures">The enumeration of inner assertion failures to add</param>
        /// <returns>The builder, to allow for fluent method chaining</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="innerFailures"/> is null or
        /// contains a null</exception>
        public AssertionFailureBuilder AddInnerFailures(IEnumerable<AssertionFailure> innerFailures)
        {
            if (innerFailures == null)
                throw new ArgumentNullException("innerFailures");

            foreach (AssertionFailure innerFailure in innerFailures)
                AddInnerFailure(innerFailure);
            return this;
        }

        /// <summary>
        /// Generates an immutable object that describes the failure.
        /// </summary>
        /// <returns>The assertion failure</returns>
        [SystemInternal]
        public AssertionFailure ToAssertionFailure()
        {
            return CreateAssertionFailure(description, message, GetStackTraceOrDefault(),
                GetLabeledValuesAsArray(), GetExceptionsAsArray(), GetInnerFailuresAsArray());
        }

        /// <summary>
        /// Creates an assertion failure object.
        /// </summary>
        /// <remarks>
        /// Subclasses may override this method to define custom extended assertion
        /// failure objects.
        /// </remarks>
        protected virtual AssertionFailure CreateAssertionFailure(string description,
            string message, StackTraceData stackTrace, IList<AssertionFailure.LabeledValue> labeledValues,
            IList<ExceptionData> exceptions, IList<AssertionFailure> innerFailures)
        {
            return new AssertionFailure(description, message, stackTrace, labeledValues, exceptions,
                innerFailures);
        }

        private void AddLabeledValueImpl(AssertionFailure.LabeledValue labeledValue)
        {
            if (labeledValues == null)
                labeledValues = new List<AssertionFailure.LabeledValue>();

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

        private AssertionFailure[] GetInnerFailuresAsArray()
        {
            return innerFailures != null ? innerFailures.ToArray() : EmptyArray<AssertionFailure>.Instance;
        }

        [SystemInternal]
        private StackTraceData GetStackTraceOrDefault()
        {
            if (isStackTraceSet)
                return stackTrace;

            return new StackTraceData(StackTraceFilter.CaptureFilteredStackTrace());
        }
    }
}