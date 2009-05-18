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
    [TestsOn(typeof(SequentiaNumbersAttribute))]
    [RunSample(typeof(SequentiaNumbersSample))]
    public class SequentiaNumbersAttributeTest : BaseTestWithSampleRunner
    {
        [Test]
        [Row("SingleStartStepCountSequence", new[] { 1, 1.25, 1.5, 1.75, 2 })]
        [Row("SingleStartEndCountSequence", new[] { 1, 1.25, 1.5, 1.75, 2 })]
        [Row("SingleStartEndStepSequence", new[] { 1, 1.25, 1.5, 1.75, 2 })]
        public void GenerateSequence(string testMethod, decimal[] expectedOutput)
        {
            var run = Runner.GetPrimaryTestStepRun(CodeReference.CreateFromMember(typeof(SequentiaNumbersSample).GetMethod(testMethod)));
            string[] lines = run.Children.Select(x => x.TestLog.GetStream(MarkupStreamNames.Default).ToString()).ToArray();
            Assert.AreElementsEqual(expectedOutput, lines.Select(x => Decimal.Parse(Regex.Match(x, @"\[(?<value>-?(\d*\.)?\d+)\]").Groups["value"].Value)));
        }

        [TestFixture, Explicit("Sample")]
        public class SequentiaNumbersSample
        {
            [Test]
            public void SingleStartStepCountSequence([SequentiaNumbers(Start = 1, Step = 0.25, Count = 5)] decimal value)
            {
                TestLog.WriteLine("[{0}]", value);
            }

            [Test]
            public void SingleStartEndCountSequence([SequentiaNumbers(Start = 1, End = 2, Count = 5)] decimal value)
            {
                TestLog.WriteLine("[{0}]", value);
            }

            [Test]
            public void SingleStartEndStepSequence([SequentiaNumbers(Start = 1, End = 2, Step = 0.25)] decimal value)
            {
                TestLog.WriteLine("[{0}]", value);
            }
        }
    }
}
