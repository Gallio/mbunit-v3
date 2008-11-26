// Copyright 2005-2008 Gallio Project - http://www.gallio.org/
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
    [TestsOn(typeof(PatternTestGlobals))]
    public class PatternTestGlobalsTest
    {
        [Test]
        public void DegreeOfParallelismDefault()
        {
            PatternTestGlobals.Reset();
            Assert.AreEqual(Math.Max(2, Environment.ProcessorCount), PatternTestGlobals.DegreeOfParallelism);
        }

        [Test]
        public void DegreeOfParallelismMustBeAtLeast1()
        {
            Assert.DoesNotThrow(() => PatternTestGlobals.DegreeOfParallelism = 1);
            Assert.Throws<ArgumentOutOfRangeException>(() => PatternTestGlobals.DegreeOfParallelism = 0);
            Assert.Throws<ArgumentOutOfRangeException>(() => PatternTestGlobals.DegreeOfParallelism = -1);
            Assert.AreEqual(1, PatternTestGlobals.DegreeOfParallelism);
        }

        [FixtureTearDown]
        public void ResetGlobals()
        {
            PatternTestGlobals.Reset();
        }
    }
}
