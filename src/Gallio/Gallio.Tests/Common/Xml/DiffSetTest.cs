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

using Gallio.Model.Schema;
using Gallio.Common.Reflection;
using Gallio.Runner.Reports.Schema;
using Gallio.Tests;
using MbUnit.Framework;
using Gallio.Common.Xml;
using System;

namespace Gallio.Tests.Common.Xml
{
    [TestFixture]
    [TestsOn(typeof(DiffSet))]
    public class DiffSetTest
    {
        [Test]
        public void Empty()
        {
            var diffSet = DiffSet.Empty;
            Assert.IsTrue(diffSet.IsEmpty);
            Assert.IsEmpty(diffSet);
        }

        [Test]
        [ExpectedArgumentNullException]
        public void Constructs_with_null_initializer_should_throw_exception()
        {
            new DiffSet(null);
        }

        [Test]
        public void Constructs_ok()
        {
            var diff1 = new Diff("Path1", "Message1", "Expected1", "Actual1");
            var diff2 = new Diff("Path2", "Message2", "Expected2", "Actual2");
            var diff3 = new Diff("Path3", "Message3", "Expected3", "Actual3");
            var diffSet = new DiffSet(new[] { diff1, diff2, diff3 });
            Assert.IsFalse(diffSet.IsEmpty);
            Assert.AreElementsEqual(new[] { diff1, diff2, diff3 }, diffSet);
        }
    }
}
