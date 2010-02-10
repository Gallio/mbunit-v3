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
using System.Runtime.Serialization;
using Gallio.Common.Diagnostics;
using Gallio.Framework.Assertions;

namespace Gallio.Framework.Assertions
{
    /// <summary>
    /// An exception type that wraps a <see cref="AssertionFailure" />.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This exception is used in two ways:
    /// <list type="bullet">
    /// <item>The exception may be "silent" when it is used to cause a test to terminate due to
    /// an assertion failure that has already been logged or otherwise conveyed to the user.</item>
    /// <item>Otherwise the exception will be reported the user in the typical manner.</item>
    /// </list>
    /// </para>
    /// </remarks>
    [Serializable]
    public class AssertionFailureException : AssertionException
    {
        private const string AssertionFailureKey = "AssertionFailure";
        private readonly AssertionFailure failure;

        /// <summary>
        /// Creates an exception for a given assertion failure.
        /// </summary>
        /// <param name="failure">The assertion failure.</param>
        /// <param name="silent">True if the assertion failure exception should not be logged because
        /// the information it contains has already been conveyed to the user.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="failure"/> is null.</exception>
        public AssertionFailureException(AssertionFailure failure, bool silent)
        {
            if (failure == null)
                throw new ArgumentNullException("failure");

            this.failure = failure;

            if (! silent)
                HasNonDefaultMessage = true;
        }

        /// <summary>
        /// Creates an exception from serialization info.
        /// </summary>
        /// <param name="info">The serialization info.</param>
        /// <param name="context">The streaming context.</param>
        protected AssertionFailureException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            failure = (AssertionFailure) info.GetValue(AssertionFailureKey, typeof(AssertionFailure));
        }

        /// <summary>
        /// Gets the associated assertion failure, never null.
        /// </summary>
        [SystemInternal]
        public AssertionFailure Failure
        {
            get { return failure; }
        }

        /// <summary>
        /// Returns <c>true</c> if the assertion failure exception should not be logged because
        /// the information it contains has already been conveyed to the user.
        /// </summary>
        [SystemInternal]
        public bool IsSilent
        {
            get { return !HasNonDefaultMessage; }
        }

        /// <inheritdoc />
        public override string Message
        {
            get { return Failure.ToString(); }
        }

        /// <inheritdoc />
        [SystemInternal]
        public override bool ExcludeStackTrace
        {
            get { return IsSilent; }
        }

        /// <inheritdoc />
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);

            info.AddValue(AssertionFailureKey, failure);
        }
    }
}