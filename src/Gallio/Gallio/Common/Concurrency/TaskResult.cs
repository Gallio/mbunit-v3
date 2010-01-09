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

namespace Gallio.Common.Concurrency
{
    /// <summary>
    /// Holds the value or exception produced by a task.
    /// </summary>
    [Serializable]
    public abstract class TaskResult
    {
        private readonly Exception exception;

        internal TaskResult(Exception exception)
        {
            this.exception = exception;
        }

        /// <summary>
        /// Returns true if the task ran to completion and returned a value,
        /// or false if an exception was thrown.
        /// </summary>
        public bool HasValue
        {
            get { return exception == null; }
        }

        /// <summary>
        /// Gets the value that was returned by the task.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown if <see cref="HasValue" /> is false.</exception>
        public object Value
        {
            get
            {
                if (!HasValue)
                    throw new InvalidOperationException("The value is not available because the task threw an exception.");
                return GetValueAsObject();
            }
        }

        /// <summary>
        /// Gets the exception that was thrown by the task.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown if <see cref="HasValue" /> is true.</exception>
        public Exception Exception
        {
            get
            {
                if (HasValue)
                    throw new InvalidOperationException("The exception is not available because the task returned a value.");
                return exception;
            }
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return (exception ?? GetValueAsObject() ?? string.Empty).ToString();
        }

        internal abstract object GetValueAsObject();
    }

    /// <summary>
    /// Holds the value or exception produced by a task.
    /// </summary>
    [Serializable]
    public sealed class TaskResult<T> : TaskResult
    {
        private readonly T value;

        private TaskResult(T value, Exception exception)
            : base(exception)
        {
            this.value = value;
        }

        /// <summary>
        /// Creates a result object containing the value returned by the task.
        /// </summary>
        /// <returns>The new task result.</returns>
        /// <param name="value">The value.</param>
        public static TaskResult<T> CreateFromValue(T value)
        {
            return new TaskResult<T>(value, null);
        }

        /// <summary>
        /// Creates a result object containing the exception thrown by the task.
        /// </summary>
        /// <param name="exception">The exception.</param>
        /// <returns>The new task result.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="exception"/> is null.</exception>
        public static TaskResult<T> CreateFromException(Exception exception)
        {
            if (exception == null)
                throw new ArgumentNullException("exception");

            return new TaskResult<T>(default(T), exception);
        }

        /// <summary>
        /// Gets the value that was returned by the task.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown if <see cref="TaskResult.HasValue" /> is false.</exception>
        new public T Value
        {
            get
            {
                if (!HasValue)
                    throw new InvalidOperationException("The value is not available because the task threw an exception.");
                return value;
            }
        }

        internal override object GetValueAsObject()
        {
            return value;
        }
    }
}
