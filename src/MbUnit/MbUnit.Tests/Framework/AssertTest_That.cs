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

using Gallio.Framework.Assertions;
using MbUnit.Framework;
using NHamcrest.Core;

namespace MbUnit.Tests.Framework
{
    [TestsOn(typeof(Assert))]
    public class AssertTest_That : BaseAssertTest
    {
        [Test]
        public void No_errors_if_matcher_matches()
        {
            var failures = Capture(() => Assert.That(true, Is.True()));

            Assert.IsEmpty(failures);
        }

        [Test]
        public void Error_is_described_if_matcher_fails()
        {
            var matcher = Is.True();
            const bool item = false;

            var failures = Capture(() => Assert.That(item, matcher, "this is a message: {0}", "arg1"));

            Assert.AreEqual(1, failures.Length);
            var assertionFailure = failures[0];
            Assert.AreEqual("Expected True", assertionFailure.Description);
            Assert.AreEqual("this is a message: arg1", assertionFailure.Message);
            Assert.IsTrue(assertionFailure.LabeledValues.Contains(new AssertionFailure.LabeledValue("Expected", "True")));
            Assert.IsTrue(assertionFailure.LabeledValues.Contains(new AssertionFailure.LabeledValue("But", "was False")));
        }
    }
}
