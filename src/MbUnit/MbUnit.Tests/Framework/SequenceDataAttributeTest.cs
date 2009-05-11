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
    [TestsOn(typeof(SequenceDataAttribute))]
    [RunSample(typeof(SequenceDataSample))]
    public class SequenceDataAttributeTest : BaseTestWithSampleRunner
    {
        [Test]
        [Row("SingleDoubleSequence", new[] { "[1]", "[1.25]", "[1.5]", "[1.75]", "[2]" })]
        [Row("SingleInt32Sequence", new[] { "[10]", "[20]", "[30]" })]
        [Row("SingleDecimalSequence", new[] { "[1]", "[2]", "[3]", "[4]" })]
        [Row("TwoCombinatorialSequences", new[] 
        {   "[1,8]", 
            "[2,8]", 
            "[3,8]", 
            "[4,8]",
            "[1,9]", 
            "[2,9]", 
            "[3,9]", 
            "[4,9]" })]
        public void EnumData(string testMethod, string[] expectedTestLogOutput)
        {
            var run = Runner.GetPrimaryTestStepRun(CodeReference.CreateFromMember(typeof(SequenceDataSample).GetMethod(testMethod)));
            Assert.AreElementsEqualIgnoringOrder(expectedTestLogOutput, 
                run.Children.Select(x => x.TestLog.GetStream(MarkupStreamNames.Default).ToString()),
                (x, y) => y.Contains(x));
        }

        [TestFixture, Explicit("Sample")]
        public class SequenceDataSample
        {
            [Test]
            public void SingleDoubleSequence([SequenceData(1, 0.25, 5)] double value)
            {
                TestLog.WriteLine("[{0}]", value);
            }

            [Test]
            public void SingleInt32Sequence([SequenceData(10, 10, 3)] int value)
            {
                TestLog.WriteLine("[{0}]", value);
            }

            [Test]
            public void SingleDecimalSequence([SequenceData(1, 1, 4)] Decimal value)
            {
                TestLog.WriteLine("[{0}]", value);
            }

            [Test]
            public void TwoCombinatorialSequences(
                [SequenceData(1, 1, 4)] double value1,
                [SequenceData(8, 1, 2)] double value2)
            {
                TestLog.WriteLine("[{0},{1}]", value1, value2);
            }
        }
    }
}
