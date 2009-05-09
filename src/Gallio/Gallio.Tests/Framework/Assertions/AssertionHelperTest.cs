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
using Gallio.Framework;
using Gallio.Framework.Assertions;
using Gallio.Common.Diagnostics;
using Gallio.Common.Markup;
using MbUnit.Framework;

namespace Gallio.Tests.Framework.Assertions
{
    [TestsOn(typeof(AssertionHelper))]
    public class AssertionHelperTest
    {
        [Test]
        [ExpectedArgumentNullException]
        public void Explain_should_throw_exception_when_action_argument_is_null()
        {
            AssertionHelper.Explain(null, 
                innerFailures =>
                    new AssertionFailureBuilder("Boom!")
                        .AddInnerFailures(innerFailures)
                        .ToAssertionFailure());
        }

        [Test]
        [ExpectedArgumentNullException]
        public void Explain_should_throw_exception_when_explanation_argument_is_null()
        {
            AssertionHelper.Explain(() => { }, null);
        }

        [Test]
        public void Explain_should_decorate_an_inner_assertion_failure()
        {
            AssertionFailure[] failures = AssertionHelper.Eval(() =>
            {
                AssertionHelper.Explain(() =>
                {
                    Assert.Fail("Inner reason");
                
                }, innerFailures =>
                    new AssertionFailureBuilder("Outer reason")
                        .AddInnerFailures(innerFailures)
                        .ToAssertionFailure());
            });

            Assert.AreEqual(1, failures.Length);
            Assert.AreEqual("Outer reason", failures[0].Description);
            Assert.AreEqual(1, failures[0].InnerFailures.Count);
            Assert.AreEqual("Inner reason", failures[0].InnerFailures[0].Message);
        }

        [Test]
        public void Explain_should_return_empty_result_when_no_failures_have_occured()
        {
            AssertionFailure[] failures = AssertionHelper.Eval(() =>
            {
                AssertionHelper.Explain(() =>
                {
                    // No failing assertion.
                
                }, innerFailures =>
                    new AssertionFailureBuilder("Ugh?")
                        .AddInnerFailures(innerFailures)
                        .ToAssertionFailure());
            });

            Assert.IsEmpty(failures);
        }
    }
}