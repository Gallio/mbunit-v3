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
using MbUnit.Framework;
using MbUnit.Framework.ContractVerifiers.Core;
using Gallio.Tests;
using Gallio.Model;
using Rhino.Mocks;
using Gallio.Common.Reflection;

namespace MbUnit.Tests.Framework.ContractVerifiers.Core
{
    [TestFixture]
    public class HashStoreTest
    {
        [Test]
        public void AddHashCodes()
        {
            var map = new HashStore(new[] { 123, 456, 123, 456, 789, 123 });
            Assert.AreEqual(6, map.Count);
            Assert.AreEqual(3, map[123]);
            Assert.AreEqual(2, map[456]);
            Assert.AreEqual(1, map[789]);
            Assert.AreEqual(0, map[666]);
        }

        [Test]
        [Row(new[] { 1, 1, 2, 3 }, 0.1667)]
        [Row(new[] { 1, 1, 2, 2 }, 0.3333)]
        public void CalculateCollisionProbability(int[] hashes, double expectedProbability)
        {
            var map = new HashStore(hashes);
            double actualProbability = map.GetCollisionProbability();
            Assert.AreApproximatelyEqual(expectedProbability, actualProbability, 0.0001);
        }
    }
}