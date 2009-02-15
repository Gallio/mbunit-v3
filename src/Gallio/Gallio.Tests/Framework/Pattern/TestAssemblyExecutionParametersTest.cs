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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gallio.Framework.Pattern;
using MbUnit.Framework;

namespace Gallio.Tests.Framework.Pattern
{
    [TestsOn(typeof(TestAssemblyExecutionParameters))]
    public class TestAssemblyExecutionParametersTest
    {
        [Test]
        public void DegreeOfParallelismDefault()
        {
            TestAssemblyExecutionParameters.Reset();
            Assert.AreEqual(Math.Max(2, Environment.ProcessorCount), TestAssemblyExecutionParameters.DegreeOfParallelism);
        }

        [Test]
        public void DegreeOfParallelismMustBeAtLeast1()
        {
            Assert.DoesNotThrow(() => TestAssemblyExecutionParameters.DegreeOfParallelism = 1);
            Assert.AreEqual(TestAssemblyExecutionParameters.DegreeOfParallelism, 1);

            Assert.Throws<ArgumentOutOfRangeException>(() => TestAssemblyExecutionParameters.DegreeOfParallelism = 0);
            Assert.AreEqual(1, TestAssemblyExecutionParameters.DegreeOfParallelism);

            Assert.Throws<ArgumentOutOfRangeException>(() => TestAssemblyExecutionParameters.DegreeOfParallelism = -1);
            Assert.AreEqual(1, TestAssemblyExecutionParameters.DegreeOfParallelism);
        }

        [Test]
        public void DefaultTestCaseTimeout()
        {
            Assert.AreEqual(TimeSpan.FromMinutes(10), TestAssemblyExecutionParameters.DefaultTestCaseTimeout);
        }

        [Test]
        public void DefaultTestCaseTimeoutMustBeNullOrNonNegative()
        {
            Assert.DoesNotThrow(() => TestAssemblyExecutionParameters.DefaultTestCaseTimeout = null);
            Assert.IsNull(TestAssemblyExecutionParameters.DefaultTestCaseTimeout);

            Assert.DoesNotThrow(() => TestAssemblyExecutionParameters.DefaultTestCaseTimeout = TimeSpan.FromMinutes(4));
            Assert.AreEqual(TimeSpan.FromMinutes(4), TestAssemblyExecutionParameters.DefaultTestCaseTimeout);

            Assert.Throws<ArgumentOutOfRangeException>(() => TestAssemblyExecutionParameters.DefaultTestCaseTimeout = TimeSpan.FromMinutes(-1));
            Assert.AreEqual(TimeSpan.FromMinutes(4), TestAssemblyExecutionParameters.DefaultTestCaseTimeout);
        }

        [FixtureTearDown]
        public void ResetGlobals()
        {
            TestAssemblyExecutionParameters.Reset();
        }
    }
}
