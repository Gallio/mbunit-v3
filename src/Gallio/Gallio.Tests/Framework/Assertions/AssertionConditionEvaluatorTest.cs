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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gallio.Framework;
using Gallio.Framework.Assertions;
using Gallio.Runtime.Formatting;
using MbUnit.Framework;

namespace Gallio.Tests.Framework.Assertions
{
    [TestsOn(typeof(AssertionConditionEvaluator))]
    public class AssertionConditionEvaluatorTest
    {
        [Test, ExpectedArgumentNullException]
        public void EvalThrowsIfConditionIsNull()
        {
            AssertionConditionEvaluator.Eval(null, true, "abc");
        }

        [Test]
        [Row(true), Row(false)]
        public void ReturnsNullIfTheConditionEvaluatesAsExpected(bool expectedResult)
        {
            Assert.IsNull(AssertionConditionEvaluator.Eval(() => expectedResult, expectedResult, null));
        }

        [Test]
        [Row(true), Row(false), MultipleAsserts]
        public void FailureDescribesExpectedResultAndConditionAndParameters(bool expectedResult)
        {
            AssertionFailure failure = AssertionConditionEvaluator.Eval(() => !expectedResult, expectedResult, null);
            Assert.IsNotNull(failure);
            Assert.AreEqual(String.Format("Expected the condition to evaluate to {0}.", expectedResult.ToString().ToLowerInvariant()), failure.Description);
            Assert.AreElementsEqual(new[] {
                new AssertionFailure.LabeledValue("Condition", "! expectedResult"),
                new AssertionFailure.LabeledValue("expectedResult", Formatter.Instance.Format(expectedResult)),
            }, failure.LabeledValues);
        }

        [Test]
        public void FailureDescribesException()
        {
            object x = null;
            AssertionFailure failure = AssertionConditionEvaluator.Eval(() => x.Equals(null), true, null);
            Assert.IsNotNull(failure);
            Assert.AreEqual("Expected the condition to evaluate to true but it threw an exception.", failure.Description);
            Assert.AreElementsEqual(new[] {
                new AssertionFailure.LabeledValue("Condition", "x.Equals(null)"),
                new AssertionFailure.LabeledValue("x", "null"),
            }, failure.LabeledValues);
            Assert.AreEqual(1, failure.Exceptions.Count);
            Assert.Contains(failure.Exceptions[0].ToString(), "NullReferenceException");
        }

        [Test]
        public void FailureIncludesStackTrace()
        {
            AssertionFailure failure = AssertionConditionEvaluator.Eval(() => true, false, null);
            Assert.IsNotNull(failure);
            Assert.Contains(failure.StackTrace.ToString(), "FailureIncludesStackTrace");
        }

        [Test]
        public void FailureIncludesOptionalMessageWhenProvided()
        {
            AssertionFailure failure = AssertionConditionEvaluator.Eval(() => true, false, "Expected {0}", "true");
            Assert.IsNotNull(failure);
            Assert.AreEqual("Expected true", failure.Message);
        }

        [Test]
        public void FailureExcludesOptionalMessageWhenOmitted()
        {
            AssertionFailure failure = AssertionConditionEvaluator.Eval(() => true, false, null);
            Assert.IsNotNull(failure);
            Assert.IsNull(failure.Message);
        }

        [Test]
        public void FailureMessageIgnoresValueOfTopLevelNotExpression()
        {
            int x = 42;
            AssertionFailure failure = AssertionConditionEvaluator.Eval(() => ! (x.ToString() == "42"), true, null);
            Assert.IsNotNull(failure);

            Assert.AreElementsEqual(new[] {
                new AssertionFailure.LabeledValue("Condition", "! (x.ToString() == \"42\")"),
                new AssertionFailure.LabeledValue("x.ToString()", "\"42\""),
                new AssertionFailure.LabeledValue("x", "42"),
            }, failure.LabeledValues);
        }

        [Test]
        public void FailureMessageIgnoresRepeatedOccurrencesOfAVariable()
        {
            int x = 42;
            AssertionFailure failure = AssertionConditionEvaluator.Eval(() => x * 2 == x + 1, true, null);
            Assert.IsNotNull(failure);

            Assert.AreElementsEqual(new[] {
                new AssertionFailure.LabeledValue("Condition", "x * 2 == x + 1"),
                new AssertionFailure.LabeledValue("x * 2", "84"),
                new AssertionFailure.LabeledValue("x + 1", "43"),
                new AssertionFailure.LabeledValue("x", "42")
            }, failure.LabeledValues);
        }
    }
}
