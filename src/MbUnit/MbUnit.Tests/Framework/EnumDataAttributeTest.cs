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
using Gallio.Model.Logging;

namespace MbUnit.Tests.Framework
{
    [TestsOn(typeof(EnumDataAttribute))]
    [RunSample(typeof(EnumDataSample))]
    public class EnumDataAttributeTest : BaseTestWithSampleRunner
    {
        [Test]
        [Row("SingleEnum", new[] 
        {   "[Vanilla]", 
            "[Strawberry]", 
            "[Chocolate]",
            "[Pistachio]"})]
        [Row("SingleEnumWithOneExclusion", new[] 
        {   "[Vanilla]", 
            "[Strawberry]", 
            "[Chocolate]"})]
        [Row("SingleEnumWithSeveralExclusions", new[] 
        {   "[Vanilla]", 
            "[Chocolate]"})]
        [Row("TwoCombinatorialEnums", new[] 
        {   "[Vanilla,Liliput]", 
            "[Strawberry,Liliput]", 
            "[Chocolate,Liliput]", 
            "[Pistachio,Liliput]", 
            "[Vanilla,Normal]", 
            "[Strawberry,Normal]", 
            "[Chocolate,Normal]",
            "[Pistachio,Normal]",
            "[Vanilla,Gargantua]", 
            "[Strawberry,Gargantua]", 
            "[Chocolate,Gargantua]",
            "[Pistachio,Gargantua]"})]
        public void EnumData(string testMethod, string[] expectedTestLogOutput)
        {
            var run = Runner.GetPrimaryTestStepRun(CodeReference.CreateFromMember(typeof(EnumDataSample).GetMethod(testMethod)));
            Assert.AreElementsEqualIgnoringOrder(expectedTestLogOutput, 
                run.Children.Select(x => x.TestLog.GetStream(TestLogStreamNames.Default).ToString()),
                (x, y) => y.Contains(x));
        }

        public enum Flavor
        {
            Vanilla,
            Strawberry,
            Chocolate,
            Pistachio
        }

        public enum Size
        {
            Liliput,
            Normal,
            Gargantua
        }

        [TestFixture, Explicit("Sample")]
        public class EnumDataSample
        {
            [Test]
            public void SingleEnum([EnumData(typeof(Flavor))] Flavor flavor)
            {
                TestLog.WriteLine("[{0}]", flavor);
            }

            [Test]
            public void SingleEnumWithOneExclusion([EnumData(typeof(Flavor), Exclude = Flavor.Pistachio)] Flavor flavor)
            {
                TestLog.WriteLine("[{0}]", flavor);
            }

            [Test]
            public void SingleEnumWithSeveralExclusions([EnumData(typeof(Flavor), ExcludeArray = new object[] { Flavor.Pistachio, Flavor.Strawberry })] Flavor flavor)
            {
                TestLog.WriteLine("[{0}]", flavor);
            }

            [Test]
            public void TwoCombinatorialEnums(
                [EnumData(typeof(Flavor))] Flavor flavor,
                [EnumData(typeof(Size))] Size size)
            {
                TestLog.WriteLine("[{0},{1}]", flavor, size);
            }
        }
    }
}
