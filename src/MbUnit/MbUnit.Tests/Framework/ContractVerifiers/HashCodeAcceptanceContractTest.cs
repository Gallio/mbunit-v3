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
using System.Runtime.Serialization;
using Gallio.Model;
using Gallio.Runner.Reports;
using Gallio.Tests;
using Gallio.Tests.Integration;
using MbUnit.Framework;
using MbUnit.Framework.ContractVerifiers;

namespace MbUnit.Tests.Framework.ContractVerifiers
{
    [RunSample(typeof(SampleTest))]
    public class HashCodeAcceptanceContractTest : AbstractContractTest
    {
        [Test]
        [Row("CollisionTestFail", "CollisionProbabilityTest", TestStatus.Failed)]
        [Row("CollisionTestFail", "UniformDistributionTest", TestStatus.Passed)]
        [Row("CollisionTestPass", "CollisionProbabilityTest", TestStatus.Passed)]
        [Row("CollisionTestPass", "UniformDistributionTest", TestStatus.Passed)]
        [Row("DistributionTestFail", "CollisionProbabilityTest", TestStatus.Passed)]
        [Row("DistributionTestFail", "UniformDistributionTest", TestStatus.Failed)]
        [Row("DistributionTestPass", "CollisionProbabilityTest", TestStatus.Passed)]
        [Row("DistributionTestPass", "UniformDistributionTest", TestStatus.Passed)]
        public void VerifySampleAccessorContract(string groupName, string testMethodName, TestStatus expectedTestStatus)
        {
            VerifySampleContract(groupName, typeof(SampleTest), testMethodName, expectedTestStatus);
        }

        [Explicit]
        internal class SampleTest
        {
            [VerifyContract]
            public readonly IContract CollisionTestFail = new HashCodeAcceptanceContract<Sample>
            {
                CollisionProbabilityLimit = 0.1,
                UniformDistributionSignificanceLevel = 0.05,
                DistinctInstances = GetSampleInstancesFromValues(1, 1, 2, 3)
            };

            [VerifyContract]
            public readonly IContract CollisionTestPass = new HashCodeAcceptanceContract<Sample>
            {
                CollisionProbabilityLimit = 0.2,
                UniformDistributionSignificanceLevel = 0.05,
                DistinctInstances = GetSampleInstancesFromValues(1, 1, 2, 3)
            };

            [VerifyContract]
            public readonly IContract DistributionTestPass = new HashCodeAcceptanceContract<Sample>
            {
                CollisionProbabilityLimit = 0.2,
                UniformDistributionSignificanceLevel = 0.055,
                DistinctInstances = GetSampleInstancesFromFrequencies(17, 8, 8, 6, 10, 16, 15, 19)
            };

            [VerifyContract]
            public readonly IContract DistributionTestFail = new HashCodeAcceptanceContract<Sample>
            {
                CollisionProbabilityLimit = 0.2,
                UniformDistributionSignificanceLevel = 0.057,
                DistinctInstances = GetSampleInstancesFromFrequencies(17, 8, 8, 6, 10, 16, 15, 19)
            };

            private static IEnumerable<Sample> GetSampleInstancesFromFrequencies(params int[] frequencies)
            {
                for (int i = 0; i < frequencies.Length; i++)
                {
                    for (int j = 0; j < frequencies[i]; j++)
                    {
                        yield return new Sample(i + 1);
                    }
                }
            }

            private static IEnumerable<Sample> GetSampleInstancesFromValues(params int[] hashes)
            {
                for (int i = 0; i < hashes.Length; i++)
                {
                    yield return new Sample(hashes[i]);
                }
            }
        }

        internal class Sample
        {
            private readonly int value;

            public Sample(int value)
            {
                this.value = value;
            }

            public override int GetHashCode()
            {
                return value;
            }
        }
    }
}
