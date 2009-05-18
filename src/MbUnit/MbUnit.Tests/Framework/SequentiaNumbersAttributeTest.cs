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

namespace MbUnit.Tests.Framework
{
    [TestsOn(typeof(SequentiaNumbersAttribute))]
    [RunSample(typeof(SequentiaNumbersSample))]
    public class SequentiaNumbersAttributeTest : BaseTestWithSampleRunner
    {
        [Test]
        [Row("SingleStartStepCountSequence", new[] { "[1]", "[1.25]", "[1.5]", "[1.75]", "[2]" })]
        [Row("SingleStartStopCountSequence", new[] { "[1]", "[1.25]", "[1.5]", "[1.75]", "[2]" })]
        [Row("SingleStartStopStepSequence", new[] { "[1]", "[1.25]", "[1.5]", "[1.75]", "[2]" })]
        [Row("TwoCombinatorialSequences", new[] 
        {   "[1,8]", 
            "[1,9]", 
            "[2,8]", 
            "[2,9]", 
            "[3,8]", 
            "[3,9]", 
            "[4,8]",
            "[4,9]" })]
        public void EnumData(string testMethod, string[] expectedTestLogOutput)
        {
            var run = Runner.GetPrimaryTestStepRun(CodeReference.CreateFromMember(typeof(SequentiaNumbersSample).GetMethod(testMethod)));
            Assert.AreElementsEqual(expectedTestLogOutput, run.Children.Select(x => x.TestLog.GetStream(MarkupStreamNames.Default).ToString()), (x, y) => y.Contains(x));
        }

        [TestFixture, Explicit("Sample")]
        public class SequentiaNumbersSample
        {
            [Test]
            public void SingleStartStepCountSequence([SequentiaNumbers(Start = 1, Step = 0.25, Count = 5)] double value)
            {
                TestLog.WriteLine("[{0}]", value);
            }

            [Test]
            public void SingleStartStopCountSequence([SequentiaNumbers(Start = 1, Stop = 2, Count = 5)] double value)
            {
                TestLog.WriteLine("[{0}]", value);
            }

            [Test]
            public void SingleStartStopStepSequence([SequentiaNumbers(Start = 1, Stop = 2, Step = 0.25)] double value)
            {
                TestLog.WriteLine("[{0}]", value);
            }

            [Test]
            public void TwoCombinatorialSequences(
                [SequentiaNumbers(Start = 1, Step = 1, Count = 4)] Decimal value1,
                [SequentiaNumbers(Start = 8, Stop = 9, Count = 2)] short value2)
            {
                TestLog.WriteLine("[{0},{1}]", value1, value2);
            }
        }
    }
}
