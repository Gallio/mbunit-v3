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
using Gallio.Reflection;
using Gallio.Runner.Reports;
using Gallio.Tests;
using MbUnit.Framework;
using System.Linq;
using Gallio.Model.Logging;

namespace MbUnit.Tests.Framework
{
    [TestsOn(typeof(RandomNumberDataAttribute))]
    //[RunSample(typeof(RandomNumberDataSample))]
    public class RandomNumberDataAttributeTest : BaseTestWithSampleRunner
    {
        //[Test]
        //[Row("SingleDoubleSequenceWithDefaultStep", 0, 10)]
        //public void EnumData(string testMethod, string[] expectedTestLogOutput)
        //{
        //    var run = Runner.GetPrimaryTestStepRun(CodeReference.CreateFromMember(typeof(RandomNumberDataSample).GetMethod(testMethod)));
        //    Assert.AreElementsEqualIgnoringOrder(expectedTestLogOutput, 
        //        run.Children.Select(x => x.TestLog.GetStream(TestLogStreamNames.Default).ToString()),
        //        (x, y) => y.Contains(x));
        //}

        //[TestFixture, Explicit("Sample")]
        //public class RandomNumberDataSample
        //{
        //    [Test]
        //    public void SingleDoubleSequenceWithDefaultStep([RandomNumberData(0, 10)] double value)
        //    {
        //        TestLog.WriteLine("[{0}]", value);
        //    }
            
        //}

    }
}
