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

namespace MbUnit.Tests.Framework
{
    [TestsOn(typeof(RandomStringsAttribute))]
    [RunSample(typeof(RandomStringsSample))]
    public class RandomStringsAttributeTest : BaseTestWithSampleRunner
    {
        [Test]
        [Row("Single", "[A-D]{3}[0-9]{3}", 100)]
        public void GenerateRandomValues(string testMethod, string expectedMatch, int expectedCount)
        {
            var run = Runner.GetPrimaryTestStepRun(CodeReference.CreateFromMember(typeof(RandomStringsSample).GetMethod(testMethod)));
            string[] lines = run.Children.Select(x => x.TestLog.GetStream(MarkupStreamNames.Default).ToString()).ToArray();
            Assert.AreEqual(expectedCount, lines.Length);

            foreach(string line in lines)
            {
                Assert.IsTrue(Regex.Match(line, String.Format(@"\[{0}\]", expectedMatch)).Success);
            }
        }

        [TestFixture, Explicit("Sample")]
        public class RandomStringsSample
        {
            [Test]
            public void Single([RandomStrings(Pattern = @"[A-D]{3}[0-9]{3}", Count = 100)] string text)
            {
                TestLog.WriteLine("[{0}]", text);
            }
        }
    }
}
