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
using Gallio.Framework.Pattern;

namespace MbUnit.Framework
{
    /// <summary>
    /// Declares that the associated test is expected to throw an <see cref="ArgumentException" />.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The expected contents of the exception message may optionally be specified.
    /// </para>
    /// </remarks>
    [AttributeUsage(PatternAttributeTargets.Test, AllowMultiple = false, Inherited = true)]
    public class ExpectedArgumentExceptionAttribute : ExpectedExceptionAttribute
    {
        /// <summary>
        /// Declares that the associated test is expected to throw an <see cref="ArgumentOutOfRangeException" />.
        /// </summary>
        public ExpectedArgumentExceptionAttribute()
            : base(typeof(ArgumentException))
        {
        }

        /// <summary>
        /// Declares that the associated test is expected to throw an <see cref="ArgumentOutOfRangeException" />.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The expected contents of the exception message may also optionally be specified.
        /// </para>
        /// </remarks>
        /// <param name="message">The expected exception message, or null if not specified.</param>
        public ExpectedArgumentExceptionAttribute(string message)
            : base(typeof(ArgumentException), message)
        {
        }
    }
}