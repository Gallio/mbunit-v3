// Copyright 2008 MbUnit Project - http://www.mbunit.com/
// Portions Copyright 2000-2004 Jonathan De Halleux, Jamie Cansdale
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

namespace Gallio.Concurrency
{
    /// <summary>
    /// Holds the result of executing a task.
    /// </summary>
    [Serializable]
    public sealed class TaskResult
    {
        private readonly object value;
        private readonly Exception exception;

        private TaskResult(object value, Exception exception)
        {
            this.value = value;
            this.exception = exception;
        }

        /// <summary>
        /// Creates a task result that contains the specified value yielded by the task
        /// when it terminated.
        /// </summary>
        /// <param name="value">The value</param>
        public static TaskResult CreateFromValue(object value)
        {
            return new TaskResult(value, null);
        }

        /// <summary>
        /// Creates a task result that contains the specified exception that was encountered
        /// by the task and caused it to terminate.
        /// </summary>
        /// <param name="exception">The exception</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="exception"/> is null</exception>
        public static TaskResult CreateFromException(Exception exception)
        {
            if (exception == null)
                throw new ArgumentNullException("exception");

            return new TaskResult(null, exception);
        }

        /// <summary>
        /// Gets the value yielded by the task when it terminated, or null if an exception was thrown.
        /// </summary>
        public object Value
        {
            get { return value; }
        }

        /// <summary>
        /// Gets the exception that was encountered by the task and caused it to terminated, or null
        /// if no exception was thrown.
        /// </summary>
        public Exception Exception
        {
            get { return exception; }
        }
    }
}
