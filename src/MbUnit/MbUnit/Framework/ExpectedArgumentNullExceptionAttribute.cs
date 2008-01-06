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

namespace MbUnit.Framework
{
    /// <summary>
    /// Declares that the associated test method is expected to throw an <see cref="ArgumentNullException" />.
    /// The expected contents of the exception message may optionally be specified.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class ExpectedArgumentNullExceptionAttribute : ExpectedExceptionAttribute
    {
        /// <summary>
        /// Declares that the associated test method is expected to throw an <see cref="ArgumentOutOfRangeException" />.
        /// </summary>
        public ExpectedArgumentNullExceptionAttribute()
            : base(typeof(ArgumentNullException))
        {
        }

        /// <summary>
        /// Declares that the associated test method is expected to throw an <see cref="ArgumentOutOfRangeException" />.
        /// The expected contents of the exception message may also optionally be specified.
        /// </summary>
        /// <param name="message">The expected exception message, or null if not specified</param>
        public ExpectedArgumentNullExceptionAttribute(string message)
            : base(typeof(ArgumentException), message)
        {
        }
    }
}
