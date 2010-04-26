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
using System.Linq;
using Gallio.Common.Reflection;
using Gallio.Common.Text;
using MbUnit.Framework;
using MbUnit.Framework.ContractVerifiers;

namespace Gallio.Tests.Common.Text
{
    [TestsOn(typeof(Diff))]
    public class DiffTest
    {
        [VerifyContract]
        public readonly IContract EqualityTests = new EqualityContract<Diff>
        {
            EquivalenceClasses =
            {
                { new Diff(DiffKind.Change, new Range(0, 10), new Range(0, 10)) },
                { new Diff(DiffKind.NoChange, new Range(0, 10), new Range(0, 10)) },
                { new Diff(DiffKind.Change, new Range(0, 9), new Range(0, 10)) },
                { new Diff(DiffKind.Change, new Range(0, 9), new Range(0, 9)) },
            }
        };

        [VerifyContract]
        public readonly IContract HashCodeAcceptanceTests = new HashCodeAcceptanceContract<Diff>
        {
            CollisionProbabilityLimit = CollisionProbability.VeryLow,
            UniformDistributionQuality = UniformDistributionQuality.Excellent,
            DistinctInstances = DataGenerators.Join(
                Enum.GetValues(typeof(DiffKind)).Cast<DiffKind>(),
                DataGenerators.Join(
                    DataGenerators.Random.Numbers(10, 0, 100),
                    DataGenerators.Random.Numbers(10, 0, 100))
                    .Select(x => new Range(x.First, x.Second)),
                DataGenerators.Join(
                    DataGenerators.Random.Numbers(10, 0, 100),
                    DataGenerators.Random.Numbers(10, 0, 100))
                    .Select(x => new Range(x.First, x.Second)))
                .Select(x => new Diff(x.First, x.Second, x.Third))
        };

        [Test]
        public void ConstructorInitializesProperties()
        {
            var diff = new Diff(DiffKind.Change, new Range(1, 3), new Range(2, 4));
            Assert.AreEqual(DiffKind.Change, diff.Kind);
            Assert.AreEqual(new Range(1, 3), diff.LeftRange);
            Assert.AreEqual(new Range(2, 4), diff.RightRange);
        }

        [Test]
        public void ToStringFormatting()
        {
            Assert.AreEqual("Change: [1 .. 3), [2 .. 6)", new Diff(DiffKind.Change, new Range(1, 2), new Range(2, 4)).ToString());
        }
    }
}
