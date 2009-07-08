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
using System.Transactions;
using Gallio.Framework;
using Gallio.Common.Reflection;
using Gallio.Runner.Reports;
using Gallio.Tests;
using MbUnit.Framework;
using System.Linq;
using Gallio.Common.Markup;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Threading;
using Gallio.Model;

namespace MbUnit.Tests.Framework
{
    [TestsOn(typeof(MultipleCultureAttribute))]
    [RunSample(typeof(MultipleCultureSample))]
    public class MultipleCultureAttributeTest : BaseTestWithSampleRunner
    {
        [Test]
        public void RunMultipleCulture()
        {
            var run = Runner.GetPrimaryTestStepRun(CodeReference.CreateFromMember(typeof(MultipleCultureSample).GetMethod("TestMethod")));
            string[] cultures = run.Children.Select(x => x.TestLog.GetStream(MarkupStreamNames.Default).ToString()).ToArray();
            Assert.AreElementsEqualIgnoringOrder(new[] { "en-US", "en-GB", "fr-FR" }, cultures, (x, y) => y.StartsWith(x));
        }

        [Test]
        public void RunInvalidCulture()
        {
            var run = Runner.GetPrimaryTestStepRun(CodeReference.CreateFromMember(typeof(MultipleCultureSample).GetMethod("InvalidCulture")));
            Assert.AreEqual(TestStatus.Failed, run.Result.Outcome.Status);
        }

        [TestFixture, Explicit("Sample")]
        public class MultipleCultureSample
        {
            [Test]
            [MultipleCulture("en-US", "en-GB", "fr-FR")]
            public void TestMethod()
            {
                TestLog.WriteLine(Thread.CurrentThread.CurrentCulture.Name);
            }

            [Test]
            [MultipleCulture("kl-KL")] // Klingon culture not supported ;)
            public void InvalidCulture()
            {
            }
        }
    }
}
