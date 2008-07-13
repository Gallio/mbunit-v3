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
using Gallio.Framework.Formatting;
using Gallio.Framework.Utilities;

namespace MbUnit.Framework
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
        private readonly string description;

        private string message;
        private string stackTrace;
        private bool isStackTraceSet;
        private List<KeyValuePair<string, string>> labeledValues;
        private List<string> exceptions;

        /// <summary>
        /// Creates an assertion failure builder.
        /// </summary>
        /// <param name="description">The description of the failure</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="description"/>
        /// is null</exception>
        public AssertionFailureBuilder(string description)
        {
            if (description == null)
                throw new ArgumentNullException("description");

            this.description = description;
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
        /// Sets the expected value.
        /// This is a convenience method for setting a labeled value called "Expected Value".
        /// </para>
        /// <para>
        /// The order in which this method is called determines the order in which this
        /// value will appear relative to other labeled values.
        /// </para>
        /// </summary>
        /// <param name="value">The expected value</param>
        /// <returns>The builder, to allow for fluent method chaining</returns>
        public AssertionFailureBuilder SetExpectedValue(object value)
        {
            SetLabeledValueImpl("Expected Value", value);
            return this;
        }

        /// <summary>
        /// <para>
        /// Sets the actual value.
        /// This is a convenience method for setting a labeled value called "Actual Value".
        /// </para>
        /// <para>
        /// The order in which this method is called determines the order in which this
        /// value will appear relative to other labeled values.
        /// </para>
        /// </summary>
        /// <param name="value">The actual value</param>
        /// <returns>The builder, to allow for fluent method chaining</returns>
        public AssertionFailureBuilder SetActualValue(object value)
        {
            SetLabeledValueImpl("Actual Value", value);
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
        /// <param name="label">The label</param>
        /// <param name="value">The value</param>
        /// <returns>The builder, to allow for fluent method chaining</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="label"/> is null</exception>
        /// <exception cref="ArgumentException">Thrown if <paramref name="label"/> is empty</exception>
        public AssertionFailureBuilder SetLabeledValue(string label, object value)
        {
            SetLabeledValueImpl(label, value);
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

            if (exceptions == null)
                exceptions = new List<string>();
            exceptions.Add(StackTraceFilter.FilterExceptionToString(ex));
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
            string message, string stackTrace, KeyValuePair<string, string>[] labeledValues,
            string[] exceptions)
        {
            return new AssertionFailure(description, message, stackTrace, labeledValues, exceptions);
        }

        private void SetLabeledValueImpl(string label, object value)
        {
            if (label == null)
                throw new ArgumentNullException("label");
            if (label.Length == 0)
                throw new ArgumentException("The label must not be empty.", "label");

            if (labeledValues == null)
                labeledValues = new List<KeyValuePair<string, string>>();

            string formattedValue = Formatter.Instance.Format(value);

            for (int i = 0; i < labeledValues.Count; i++)
            {
                if (labeledValues[i].Key == label)
                {
                    labeledValues.RemoveAt(i);
                    break;
                }
            }

            labeledValues.Add(new KeyValuePair<string,string>(label, formattedValue));
        }

        private KeyValuePair<string, string>[] GetLabeledValuesAsArray()
        {
            return labeledValues != null ? labeledValues.ToArray() : EmptyArray<KeyValuePair<string, string>>.Instance;
        }

        private string[] GetExceptionsAsArray()
        {
            return exceptions != null ? exceptions.ToArray() : EmptyArray<string>.Instance;
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
