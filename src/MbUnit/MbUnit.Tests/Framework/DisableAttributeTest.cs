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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gallio.Common.Reflection;
using Gallio.Model;
using Gallio.Runner.Reports.Schema;
using Gallio.Tests;
using MbUnit.Framework;

namespace MbUnit.Tests.Framework
{
    [TestFixture]
    [TestsOn(typeof(DisableAttribute))]
    [RunSample(typeof(Sample))]
    public class DisableAttributeTest : BaseTestWithSampleRunner
    {
        [Test]
        [Row(typeof(Sample), "ControlTest", true)]
        [Row(typeof(Sample.Abstract), "BaseTest", false)]
        [Row(typeof(Sample.Concrete), "BaseTest", true)]
        [Row(typeof(Sample.Concrete), "RunningTest", true)]
        [Row(typeof(Sample.Concrete), "DisableTest", false)]
        public void Validate(Type type, string name, bool shouldRun)
        {
            TestStepRun run = Runner.GetPrimaryTestStepRun(CodeReference.CreateFromMember(type.GetMethod(name)));

            if (shouldRun)
            {
                Assert.IsNotNull(run);
                Assert.AreEqual(TestStatus.Passed, run.Result.Outcome.Status);
            }
            else
            {
                Assert.IsNull(run);
            }
        }

        [TestFixture, Explicit]
        public class Sample
        {
            [Test]
            public void ControlTest()
            {
            }

            [TestFixture, Disable]
            public class Abstract
            {
                [Test]
                public void BaseTest()
                {
                }
            }

            [TestFixture]
            public class Concrete : Abstract
            {
                [Test]
                public void RunningTest()
                {
                }

                [Test, Disable]
                public void DisableTest()
                {
                }
            }
        }
    }
}
