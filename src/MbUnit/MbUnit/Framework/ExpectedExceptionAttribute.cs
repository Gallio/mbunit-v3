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
using Gallio.Model;
using Gallio.Common.Reflection;
using Gallio.Framework.Pattern;

namespace MbUnit.Framework
{
    /// <summary>
    /// Declares that the associated test is expected to throw an exception of
    /// a particular type.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The expected contents of the exception message may optionally be specified.
    /// </para>
    /// </remarks>
    [AttributeUsage(PatternAttributeTargets.Test, AllowMultiple = false, Inherited = true)]
    public class ExpectedExceptionAttribute : TestMethodDecoratorPatternAttribute
    {
        private readonly Type exceptionType;
        private string message;
        private Type innerExceptionType;

        /// <summary>
        /// Declares that the associated test is expected to throw an exception of
        /// a particular type.
        /// </summary>
        /// <param name="exceptionType">The expected exception type.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="exceptionType"/> is null.</exception>
        public ExpectedExceptionAttribute(Type exceptionType)
            : this(exceptionType, null)
        {
        }

        /// <summary>
        /// Declares that the associated test is expected to throw an exception of
        /// a particular type.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The expected contents of the exception message may also optionally be specified.
        /// </para>
        /// </remarks>
        /// <param name="exceptionType">The expected exception type.</param>
        /// <param name="message">The expected exception message, or null if not specified.
        /// May be a substring of the actual exception message.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="exceptionType"/> is null.</exception>
        public ExpectedExceptionAttribute(Type exceptionType, string message)
            : this(exceptionType, message, null)
        {
        }

        /// <summary>
        /// Declares that the associated test is expected to throw an exception of
        /// a particular type.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The expected contents of the exception message may also optionally be specified.
        /// </para>
        /// </remarks>
        /// <param name="exceptionType">The expected exception type.</param>
        /// <param name="message">The expected exception message, or null if not specified.
        /// May be a substring of the actual exception message.</param>
        /// <param name="innerExceptionType">The expected inner exception type, or null if not specified.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="exceptionType"/> is null.</exception>
        public ExpectedExceptionAttribute(Type exceptionType, string message, Type innerExceptionType)
        {
            if (exceptionType == null)
                throw new ArgumentNullException(@"exceptionType");

            this.exceptionType = exceptionType;
            this.message = message;
            this.innerExceptionType = innerExceptionType;
        }

        /// <summary>
        /// Gets the expected exception type.
        /// </summary>
        public Type ExceptionType
        {
            get { return exceptionType; }
        }

        /// <summary>
        /// Gets or sets the expected exception message, or null if none specified.
        /// May be a substring of the actual exception message.
        /// </summary>
        public string Message
        {
            get { return message; }
            set { message = value; }
        }

        /// <summary>
        /// Gets or sets expected inner exception type.
        /// </summary>
        public Type InnerExceptionType
        {
            get { return innerExceptionType; }
            set { innerExceptionType = value; }
        }

        /// <inheritdoc />
        protected override void DecorateMethodTest(IPatternScope methodScope, IMethodInfo method)
        {
            methodScope.TestBuilder.AddMetadata(MetadataKeys.ExpectedException, exceptionType.FullName);

            if (message != null)
                methodScope.TestBuilder.AddMetadata(MetadataKeys.ExpectedExceptionMessage, message);

            if (innerExceptionType != null)
                methodScope.TestBuilder.AddMetadata(MetadataKeys.ExpectedInnerException, innerExceptionType.FullName);
        }
    }
}
