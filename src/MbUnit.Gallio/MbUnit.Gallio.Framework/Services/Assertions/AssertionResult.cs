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
using System.Xml.Serialization;
using MbUnit.Framework.Services.Assertions;
using MbUnit.Framework.Utilities;

namespace MbUnit.Framework.Services.Assertions
{
    /// <summary>
    /// Describes the result of having verified an assertion.
    /// </summary>
    [Serializable]
    [XmlType]
    [XmlRoot("assertionResult")]
    public sealed class AssertionResult
    {
        /// <summary>
        /// Creates an uninitialized assertion result.
        /// </summary>
        public AssertionResult()
        {
        }

        /// <summary>
        /// Creates an initialized assertion result.
        /// </summary>
        /// <param name="resultType">The assertion result type</param>
        /// <param name="resultMessage">The assertion failure message, never null</param>
        /// <param name="resultException">The assertion failure exception or null if none</param>
        /// <param name="innerResults">The inner assertion results or null or an empty array if none</param>
        public AssertionResult(AssertionResultType resultType, string resultMessage, Exception resultException, AssertionResult[] innerResults)
        {
            if (!AssertionUtils.IsValid(resultType))
                throw new ArgumentOutOfRangeException("resultType", resultType, "The value is not a valid AssertionResultType constant.");
            if (resultMessage == null)
                throw new ArgumentNullException("resultMessage");

            this.resultType = resultType;
            this.resultMessage = resultMessage;
            this.resultException = resultException;

            if (innerResults != null && innerResults.Length != 0)
                this.innerResults = innerResults;
        }

        /// <summary>
        /// Creates a successful assertion result.
        /// </summary>
        /// <returns>The successful assertion result</returns>
        public static AssertionResult CreateSuccessResult()
        {
            return CreateSuccessResult(null);
        }

        /// <summary>
        /// Creates a successful assertion result with optional inner results.
        /// </summary>
        /// <param name="innerResults">The inner assertion results or null or an empty array if none</param>
        /// <returns>The successful assertion result</returns>
        public static AssertionResult CreateSuccessResult(AssertionResult[] innerResults)
        {
            return new AssertionResult(AssertionResultType.Success, null, null, innerResults);
        }

        /// <summary>
        /// Creates a failure result.
        /// </summary>
        /// <param name="failureMessage">The failure message, must not be null</param>
        /// <returns>The failure assertion result</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="failureMessage"/> is null</exception>
        public static AssertionResult CreateFailureResult(string failureMessage)
        {
            return CreateFailureResult(failureMessage, null, null);
        }

        /// <summary>
        /// Creates a failure result with an optional exception.
        /// </summary>
        /// <param name="failureMessage">The failure message, must not be null</param>
        /// <param name="failureException">The failure exception or null if none</param>
        /// <returns>The failure assertion result</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="failureMessage"/> is null</exception>
        public static AssertionResult CreateFailureResult(string failureMessage, Exception failureException)
        {
            return CreateFailureResult(failureMessage, failureException, null);
        }

        /// <summary>
        /// Creates a failure result with an optional exception and inner results.
        /// </summary>
        /// <param name="failureMessage">The failure message, must not be null</param>
        /// <param name="failureException">The failure exception or null if none</param>
        /// <param name="innerResults">The inner assertion results or null or an empty array if none</param>
        /// <returns>The failure assertion result</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="failureMessage"/> is null</exception>
        public static AssertionResult CreateFailureResult(string failureMessage, Exception failureException,
                                                          AssertionResult[] innerResults)
        {
            if (failureMessage == null)
                throw new ArgumentNullException("failureMessage");

            return new AssertionResult(AssertionResultType.Failure, failureMessage, failureException, innerResults);
        }

        /// <summary>
        /// Gets or sets the assertion associated with this assertion result.
        /// The value 
        /// </summary>
        [XmlElement("assertion", IsNullable=false)]
        public Assertion Assertion
        {
            get { return assertion; }
            set { assertion = value; }
        }
        private Assertion assertion;

        /// <summary>
        /// Gets or sets the array of assertion results included in the production of
        /// this one.  Zero or more inner assertion results may appear when multiple
        /// assertions are compounded into one logical unit.
        /// Returns an empty array when there are no inner assertions.
        /// </summary>
        [XmlArray("innerResults", IsNullable = false)]
        [XmlArrayItem("result", IsNullable = false)]
        public AssertionResult[] InnerResults
        {
            get
            {
                return innerResults;
            }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");

                innerResults = value;
            }
        }
        private AssertionResult[] innerResults = EmptyArray<AssertionResult>.Instance;

        /// <summary>
        /// Gets or sets the assertion result type which describes the outcome of
        /// having evaluated the assertion to produce this result.
        /// </summary>
        [XmlAttribute("resultType")]
        public AssertionResultType ResultType
        {
            get { return resultType; }
            set
            {
                if (!AssertionUtils.IsValid(value))
                    throw new ArgumentOutOfRangeException("value", value, "The value is not a valid AssertionResultType constant.");

                resultType = value;
            }
        }
        private AssertionResultType resultType = AssertionResultType.Success;

        /// <summary>
        /// Gets or sets the assertion result message that explains the cause of the result.
        /// May be blank but never null.
        /// </summary>
        [XmlElement("resultMessage", IsNullable = false)]
        public string ResultMessage
        {
            get { return resultMessage; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");

                resultMessage = value;
            }
        }
        private string resultMessage = "";

        /// <summary>
        /// Gets or sets the exception associated with the assertion result (generally an error).
        /// May be null if none.
        /// </summary>
        [XmlElement("resultException", IsNullable=true)]
        public Exception ResultException
        {
            get { return resultException; }
            set { resultException = value; }
        }
        private Exception resultException;

        /// <summary>
        /// Returns true if the <see cref="ResultType" /> is <see cref="AssertionResultType.Success" />,
        /// <see cref="AssertionResultType.Inconclusive" />, <see cref="AssertionResultType.Ignore" />,
        /// or <see cref="AssertionResultType.Skip" />.
        /// </summary>
        [XmlIgnore]
        public bool IsOk
        {
            get { return AssertionUtils.IsOk(resultType); }
        }
    }
}