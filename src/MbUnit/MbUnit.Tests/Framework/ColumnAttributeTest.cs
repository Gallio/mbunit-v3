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

namespace MbUnit.Tests.Framework
{
    [TestsOn(typeof(ColumnAttribute))]
    [RunSample(typeof(ColumnSample))]
    public class ColumnAttributeTest : BaseTestWithSampleRunner
    {
        [Test]
        [Row("SingleColumn", new[] 
        {   "[123]", 
            "[456]", 
            "[789]" })]
        [Row("TwoCombinatorialColumns", new[] 
        {   "[123,True]", 
            "[456,True]", 
            "[789,True]", 
            "[123,False]", 
            "[456,False]", 
            "[789,False]" })]
        [Row("ThreeCombinatorialColumns", new[] 
        {   "[123,True,One]", 
            "[456,True,One]", 
            "[789,True,One]", 
            "[123,False,One]", 
            "[456,False,One]", 
            "[789,False,One]",
            "[123,True,Two]", 
            "[456,True,Two]", 
            "[789,True,Two]", 
            "[123,False,Two]", 
            "[456,False,Two]", 
            "[789,False,Two]",
            "[123,True,Three]", 
            "[456,True,Three]", 
            "[789,True,Three]", 
            "[123,False,Three]", 
            "[456,False,Three]", 
            "[789,False,Three]" })]
        public void Column(string testMethod, string[] expectedTestLogOutput)
        {
            var run = Runner.GetPrimaryTestStepRun(CodeReference.CreateFromMember(typeof(ColumnSample).GetMethod(testMethod)));
            Assert.AreElementsEqualIgnoringOrder(expectedTestLogOutput, 
                run.Children.Select(x => x.TestLog.GetStream(MarkupStreamNames.Default).ToString()),
                (x, y) => y.Contains(x));
        }

        [TestFixture, Explicit("Sample")]
        public class ColumnSample
        {
            [Test]
            public void SingleColumn([Column(123, 456, 789)] int value)
            {
                TestLog.WriteLine("[{0}]", value);
            }

            [Test]
            public void TwoCombinatorialColumns(
                [Column(123, 456, 789)] int value, 
                [Column(true, false)] bool flag)
            {
                TestLog.WriteLine("[{0},{1}]", value, flag);
            }

            [Test]
            public void ThreeCombinatorialColumns(
                [Column(123, 456, 789)] int value, 
                [Column(true, false)] bool flag,
                [Column("One", "Two", "Three")] string text)
            {
                TestLog.WriteLine("[{0},{1},{2}]", value, flag, text);
            }
        }
    }
}
