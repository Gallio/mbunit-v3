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
            var store = new HashStore(new[] { 123, 456, 123, 456, 789, 123 });
            Assert.AreEqual(6, store.StatisticalPopulation);
            Assert.AreEqual(3, store[123]);
            Assert.AreEqual(2, store[456]);
            Assert.AreEqual(1, store[789]);
            Assert.AreEqual(0, store[666]);
        }

        [Test]
        [Row(0.1667, new[] { 1, 1, 2, 3 })]
        [Row(0.3333, new[] { 1, 1, 2, 2 })]
        public void CalculateCollisionProbability(double expectedProbability, IEnumerable<int> hashes)
        {
            var store = new HashStore(hashes);
            double actualProbability = store.CollisionProbability;
            Assert.AreApproximatelyEqual(expectedProbability, actualProbability, 0.0001);
        }

        [Test]
        public void CalculateCollisionProbabilityWithHighLoad()
        {
            var store = new HashStore(GenerateHashLoad(Enumerable.Range(0, 10000), 5000));
            double actualProbability = store.CollisionProbability;
            Assert.AreApproximatelyEqual(9.998E-5, actualProbability, 1E-5);
        }

        private static IEnumerable<int> GenerateHashLoad(IEnumerable<int> hashes, int repeat)
        {
            foreach (int hash in hashes)
                for (int i = 0; i < repeat; i++)
                    yield return hash;
        }

        [Test]
        public void DistributionDeviationProbability()
        {
            var store = new HashStore(new[] { 7, 7, 7, 8, 8, 8, 8, 9, 9, 6, 6, 6, 5, 5, 5, 5, 5, 5 });
            double actual = store.UniformDistributionDeviationProbability;
            Assert.AreApproximatelyEqual(0.365284, actual, 0.000001);
        }
    }
}