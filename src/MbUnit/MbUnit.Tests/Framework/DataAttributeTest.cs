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
using Gallio.Common.Markup;
using Gallio.Common.Reflection;
using Gallio.Model;
using Gallio.Tests;
using MbUnit.Framework;

namespace MbUnit.Tests.Framework
{
    [TestsOn(typeof(DataAttribute))]
    [RunSample(typeof(ExpectedExceptionSample))]
    public class DataAttributeTest : BaseTestWithSampleRunner
    {
        [Test]
        [Row("ExactException", true, null)]
        [Row("SubclassException", true, null)]
        [Row("SubstringExceptionMessage", true, null)]
        [Row("WrongExceptionType", false, "Expected an exception of type 'System.InvalidOperationException' but a different exception was thrown.")]
        [Row("WrongExceptionMessage", false, "Expected an exception of type 'System.ArgumentNullException' with message substring 'message' but a different exception was thrown.")]
        [Row("NoExceptionThrown", false, "Expected an exception of type 'System.ArgumentNullException' but none was thrown.")]
        [Row("NoExceptionExpected", false, "Execute")]
        public void ExpectedExceptionOutcome(string testMethodName, bool success, string expectedLogOutput)
        {
            var run = Runner.GetPrimaryTestStepRun(CodeReference.CreateFromMember(typeof(ExpectedExceptionSample).GetMethod(testMethodName)));

            Assert.Multiple(() =>
            {
                Assert.IsNotNull(run);
                Assert.AreEqual(1, run.Children.Count);

                var childRun = run.Children[0];
                Assert.AreEqual(success ? TestOutcome.Passed : TestOutcome.Failed, childRun.Result.Outcome);

                if (expectedLogOutput != null)
                    AssertLogContains(childRun, expectedLogOutput, MarkupStreamNames.Failures);
            });
        }

        [Explicit("Sample")]
        public class ExpectedExceptionSample
        {
            [Test]
            [Row(1, ExpectedException = typeof(ArgumentNullException))]
            public void ExactException(int dummy)
            {
                throw new ArgumentNullException();
            }

            [Test]
            [Row(1, ExpectedException = typeof(ArgumentException))]
            public void SubclassException(int dummy)
            {
                throw new ArgumentNullException();
            }

            [Test]
            [Row(1, ExpectedException = typeof(ArgumentNullException), ExpectedExceptionMessage = "message")]
            public void SubstringExceptionMessage(int dummy)
            {
                throw new ArgumentNullException("the message");
            }

            [Test]
            [Row(1, ExpectedException = typeof(InvalidOperationException))]
            public void WrongExceptionType(int dummy)
            {
                throw new ArgumentNullException();
            }

            [Test]
            [Row(1, ExpectedException = typeof(ArgumentNullException), ExpectedExceptionMessage = "message")]
            public void WrongExceptionMessage(int dummy)
            {
                throw new ArgumentNullException("different");
            }

            [Test]
            [Row(1, ExpectedException = typeof(ArgumentNullException))]
            public void NoExceptionThrown(int dummy)
            {
            }

            [Test]
            [Row(1)]
            public void NoExceptionExpected(int dummy)
            {
                throw new ArgumentNullException();
            }
        }
    }
}
