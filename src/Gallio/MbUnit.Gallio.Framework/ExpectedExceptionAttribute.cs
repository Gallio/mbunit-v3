// Copyright 2007 MbUnit Project - http://www.mbunit.com/
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
using MbUnit.Framework.Kernel.Attributes;
using MbUnit.Framework.Kernel.Model;

namespace MbUnit.Framework
{
    /// <summary>
    /// Declares that the associated test method is expected to throw an exception of
    /// a particular type.  The expected contents of the exception message may optionally
    /// be specified.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class ExpectedExceptionAttribute : MethodDecoratorPatternAttribute
    {
        private readonly Type exceptionType;
        private string message;

        /// <summary>
        /// Declares that the associated test method is expected to throw an exception of
        /// a particular type.
        /// </summary>
        /// <param name="exceptionType">The expected exception type</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="exceptionType"/> is null</exception>
        public ExpectedExceptionAttribute(Type exceptionType)
            : this(exceptionType, null)
        {
        }

        /// <summary>
        /// Declares that the associated test method is expected to throw an exception of
        /// a particular type.  The expected contents of the exception message may also
        /// optionally be specified.
        /// </summary>
        /// <param name="exceptionType">The expected exception type</param>
        /// <param name="message">The expected exception message, or null if not specified</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="exceptionType"/> is null</exception>
        public ExpectedExceptionAttribute(Type exceptionType, string message)
        {
            if (exceptionType == null)
                throw new ArgumentNullException(@"exceptionType");

            this.exceptionType = exceptionType;
            this.message = message;
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
        /// </summary>
        public string Message
        {
            get { return message; }
            set { message = value; }
        }

        /// <inheritdoc />
        public override void Apply(TemplateTreeBuilder builder, MbUnitMethodTemplate methodTemplate)
        {
            methodTemplate.ProcessTestChain.After(delegate(MbUnitTest test)
            {
                test.ExecuteChain.Around(delegate(MbUnitTestState state, Action<MbUnitTestState> innerAction)
                {
                    InterimAssert.Throws(exceptionType, delegate
                    {
                        innerAction(state);
                    });
                });
            });

            base.Apply(builder, methodTemplate);
        }
    }
}
