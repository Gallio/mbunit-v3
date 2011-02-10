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
using Gallio.Runner.Reports.Schema;
using System.Globalization;

namespace MbUnit.Tests.Framework
{
    [TestsOn(typeof(MultipleCultureAttribute))]
    [RunSample(typeof(MultipleCultureSample))]
    [RunSample(typeof(InvalidMultipleCultureSample))]
    [RunSample(typeof(MultipleCultureWithThreadsSample))]
    public class MultipleCultureAttributeTest : BaseTestWithSampleRunner
    {
        [Test]
        public void RunMultipleCulture()
        {
            var run = Runner.GetPrimaryTestStepRun(typeof(MultipleCultureSample), "Test");
            var cultures = GetLogs(run.Children);
            Assert.AreElementsEqualIgnoringOrder(new[] { "en-US", "en-GB", "fr-FR" }, cultures, (x, y) => y.StartsWith(x));
        }

        [Test]
        public void RunInvalidCulture()
        {
            var run = Runner.GetPrimaryTestStepRun(typeof(InvalidMultipleCultureSample), "Test");
            Assert.AreEqual(TestStatus.Failed, run.Result.Outcome.Status);
        }

        [Test]
        public void RunMultipleCultureWithThreads([Column("Test1", "Test2")] string methodName) // Issue 572 (http://code.google.com/p/mb-unit/issues/detail?id=572)
        {
            var runs = Runner.GetTestStepRuns(typeof(MultipleCultureWithThreadsSample), methodName);
            var cultures = GetLogs(runs).Where(x => Regex.IsMatch(x, @"^\w{2}-\w{2}"));
            Assert.AreElementsEqualIgnoringOrder(new[] { "en-US", "en-US", "fr-FR", "fr-FR" }, cultures, (x, y) => y.StartsWith(x));
        }

        [TestFixture, Explicit("Sample")]
        public class MultipleCultureSample
        {
            [Test]
            [MultipleCulture("en-US", "en-GB", "fr-FR")]
            public void Test()
            {
                TestLog.WriteLine(Thread.CurrentThread.CurrentCulture.Name);
            }
        }

        [TestFixture, Explicit("Sample")]
        public class InvalidMultipleCultureSample
        {
            [Test]
            [MultipleCulture("kl-KL")] // Klingon culture is not supported yet :)
            public void Test()
            {
            }
        }

        [TestFixture, Explicit("Sample")]
        public class MultipleCultureWithThreadsSample
        {
            [Test]
            [ThreadedRepeat(2, Order = 1)]
            [MultipleCulture("en-US", "fr-FR", Order = 2)]
            public void Test1()
            {
                TestLog.WriteLine(Thread.CurrentThread.CurrentCulture.Name);
            }

            [Test]
            [MultipleCulture("en-US", "fr-FR", Order = 1)]
            [ThreadedRepeat(2, Order = 2)]
            public void Test2()
            {
                TestLog.WriteLine(Thread.CurrentThread.CurrentCulture.Name);
            }
        }
    }
}
