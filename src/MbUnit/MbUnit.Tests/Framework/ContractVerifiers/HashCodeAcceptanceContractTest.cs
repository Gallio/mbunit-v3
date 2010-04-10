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
        [Row("Test1", "CollisionProbabilityTest", TestStatus.Failed)]
        [Row("Test2", "CollisionProbabilityTest", TestStatus.Passed)]
        public void VerifySampleAccessorContract(string groupName, string testMethodName, TestStatus expectedTestStatus)
        {
            VerifySampleContract(groupName, typeof(SampleTest), testMethodName, expectedTestStatus);
        }

        [Explicit]
        internal class SampleTest
        {
            [VerifyContract]
            public readonly IContract Test1 = new HashCodeAcceptanceContract<Sample>
            {
                CollisionProbabilityLimit = 0.1,
                DistinctInstances = GetSampleInstances(1, 1, 2, 3)
            };

            [VerifyContract]
            public readonly IContract Test2 = new HashCodeAcceptanceContract<Sample>
            {
                CollisionProbabilityLimit = 0.2,
                DistinctInstances = GetSampleInstances(1, 1, 2, 3)
            };

            private static IEnumerable<Sample> GetSampleInstances(params int[] hashes)
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

        #region Experimentations

        [Explicit]
        internal class ExperimentTest
        {
            [VerifyContract]
            public readonly IContract HashCodeAcceptanceTests = new HashCodeAcceptanceContract<Experiment>
            {
                CollisionProbabilityLimit = 0.01,
                DistinctInstances = GetDistinctInstances()
            };

            private static IEnumerable<Experiment> GetDistinctInstances()
            {
                for (int i = 0; i < 1000; i++)
                    for (int j = 0; j < 1000; j++)
                        yield return new Experiment(i, j);
            }

            internal class Experiment
            {
                private readonly int id1;
                private readonly int id2;

                public Experiment(int id1, int id2)
                {
                    this.id1 = id1;
                    this.id2 = id2;
                }

                public override int GetHashCode()
                {
                    unchecked
                    {
                        int hash = 17;
                        hash = hash * 23 + id1.GetHashCode();
                        hash = hash * 23 + id2.GetHashCode();
                        return hash;
                    }
                }
            }
        }

        #endregion
    }
}
