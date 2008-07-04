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
    public sealed class AssertionFailureBuilder
    {
        private const int ExpectedValueOrder = 0;
        private const int ActualValueOrder = 1;
        private const int FirstLabelOrder = 100;

        private readonly string description;

        private string message;
        private string stackTrace;
        private bool isStackTraceSet;
        private SortedList<int, KeyValuePair<string, string>> labeledValues;
        private List<string> exceptions;
        private int nextLabelOrder = FirstLabelOrder;

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
            message = messageFormat != null
                ? String.Format(messageFormat, messageArgs)
                : null;
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
        /// </para>
        /// </summary>
        /// <param name="value">The expected value</param>
        /// <returns>The builder, to allow for fluent method chaining</returns>
        public AssertionFailureBuilder SetExpectedValue(object value)
        {
            SetLabeledValue("Expected Value", value, ExpectedValueOrder);
            return this;
        }

        /// <summary>
        /// <para>
        /// Sets the actual value.
        /// </para>
        /// </summary>
        /// <param name="value">The actual value</param>
        /// <returns>The builder, to allow for fluent method chaining</returns>
        public AssertionFailureBuilder SetActualValue(object value)
        {
            SetLabeledValue("Actual Value", value, ActualValueOrder);
            return this;
        }

        /// <summary>
        /// <para>
        /// Sets a labeled value.
        /// </para>
        /// <para>
        /// If a value is already associated with the specified label, replaces it.
        /// </para>
        /// </summary>
        /// <param name="label">The label</param>
        /// <param name="value">The value</param>
        /// <returns>The builder, to allow for fluent method chaining</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="label"/> is null</exception>
        /// <exception cref="ArgumentException">Thrown if <paramref name="label"/> is empty</exception>
        public AssertionFailureBuilder SetLabeledValue(string label, object value)
        {
            SetLabeledValue(label, value, nextLabelOrder++);
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
            return new AssertionFailure(description, message, GetStackTraceOrDefault(),
                GetLabeledValuesAsArray(), GetExceptionsAsArray());
        }

        private void SetLabeledValue(string label, object value, int sortOrder)
        {
            if (label == null)
                throw new ArgumentNullException("label");
            if (label.Length == 0)
                throw new ArgumentException("The label must not be empty.", "label");

            if (labeledValues == null)
                labeledValues = new SortedList<int, KeyValuePair<string, string>>();

            string formattedValue = Formatter.Instance.Format(value);

            int i = 0;
            foreach (KeyValuePair<string, string> pair in labeledValues.Values)
            {
                if (pair.Key == label)
                {
                    labeledValues.RemoveAt(i);
                    break;
                }
            }

            labeledValues.Add(sortOrder, new KeyValuePair<string,string>(label, formattedValue));
        }

        private KeyValuePair<string, string>[] GetLabeledValuesAsArray()
        {
            return labeledValues != null ? GenericUtils.ToArray(labeledValues.Values) : EmptyArray<KeyValuePair<string, string>>.Instance;
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
