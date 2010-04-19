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
using System.Reflection;
using System.Threading;
using Gallio.Common;
using Gallio.Common.Collections;
using Gallio.Framework;
using Gallio.Framework.Assertions;
using MbUnit.Framework.ContractVerifiers.Core;

namespace MbUnit.Framework.ContractVerifiers
{
    /// <summary>
    /// Contract for verifying the efficiency of the hash code generation algorithm for a given type.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Built-in verifications:
    /// <list type="bullet">
    /// <item>
    /// <strong>CollisionProbabilityTest</strong> : The probability of hash code collision for the specified set of instances
    /// is not greater than the specified limit.
    /// </item>
    /// </list>
    /// </para>
    /// </remarks>
    /// <example>
    /// The following examples shows a simple class that overrides the <see cref="Object.GetHashCode"/> method, and
    /// the test which verifies that the algorithm is resistant to hash code collision against the specified set of
    /// distinct values.
    /// <code><![CDATA[
    /// public class Foo
    /// {
    ///     private readonly int key1;
    ///     private readonly int key2;
    ///     
    ///     public Foo(int key1, int key2)
    ///     {
    ///         this.name = key1;
    ///         this.key2 = key2;
    ///     }
    /// 
    ///     public override int GetHashCode()
    ///     {
    ///         int hash = 17;
    ///         hash = hash * 23 + id1.GetHashCode();
    ///         hash = hash * 23 + id2.GetHashCode();
    ///         return hash;
    ///     }
    /// }
    /// 
    /// [TestFixture]
    /// public class FooTest
    /// {
    ///     [VerifyContract]
    ///     public readonly IContract HashCodeAcceptanceTests = new HashCodeAcceptanceContract<Foo>
    ///     {
    ///         CollisionProbabilityLimit = CollisionProbability.Low,
    ///         UniformDistributionSignificanceLevel = UniformDistributionQuality.Low,
    ///         DistinctInstances = GetDistinctInstances();
    ///     };
    /// 
    ///     private static IEnumerable<Foo> GetDistinctInstances()
    ///     {
    ///         for(int i=0; i<1000; i++)
    ///             for(int j=0; j<1000; j++)
    ///                 yield return new Foo(i, j);
    ///     }
    /// }
    /// ]]></code>
    /// </example>
    /// <typeparam name="T">The type of the object for which the hash code generation must be evaluated.</typeparam>
    public class HashCodeAcceptanceContract<T> : AbstractContract
    {
        private double collisionProbabilityLimit = CollisionProbability.Low;
        private double uniformDistributionQuality = ContractVerifiers.UniformDistributionQuality.Good;

        /// <summary>
        /// Gets or sets the maximum collision probability tolerated.
        /// </summary>
        /// <remarks>
        /// <para>
        /// If the actual probability calculated from the specified distinct instances is greater
        /// than this limit, the test will fail.
        /// </para>
        /// <para>
        /// The default is 0.01 (1%).
        /// </para>
        /// </remarks>
        public double CollisionProbabilityLimit
        {
            get
            {
                return collisionProbabilityLimit;
            }

            set
            {
                if (value < 0 || value > 1)
                    throw new ArgumentOutOfRangeException("value", "The probability must be comprised between 0 and 1.");

                collisionProbabilityLimit = value;
            }
        }

        /// <summary>
        /// Gets or sets the required significance level to determine whether the distribution is uniform.
        /// </summary>
        /// <remarks>
        /// <para>
        /// If the actual probability calculated from the specified distinct instances is greater than 
        /// the specified value, the test fails.
        /// </para>
        /// <para>
        /// The default is 0.05 (5%).
        /// </para>
        /// </remarks>
        public double UniformDistributionQuality
        {
            get
            {
                return uniformDistributionQuality;
            }

            set
            {
                if (value < 0 || value > 1)
                    throw new ArgumentOutOfRangeException("value", "The uniform distribution significance level must be between 0 and 1.");

                uniformDistributionQuality = value;
            }
        }

        /// <summary>
        /// Gets or sets an enumeration of distinct instances to extract the hash code from.
        /// </summary>
        public IEnumerable<T> DistinctInstances
        {
            get;
            set;
        }

        /// <summary>
        /// Constructs a contract verifier to evaluate the efficiency of the hash code generation algorithm for a given type.
        /// </summary>
        public HashCodeAcceptanceContract()
        {
            DistinctInstances = EmptyArray<T>.Instance;
        }

        /// <inheritdoc />
        protected override IEnumerable<Test> GetContractVerificationTests()
        {
            var store = new HashStore(GenericCollectionUtils.Select(DistinctInstances, o => o.GetHashCode()));
            yield return CreateCollisionProbabilityTest(store);
            yield return CreateUniformDistributionTest(store);
        }

        private Test CreateCollisionProbabilityTest(HashStore store)
        {
            return new TestCase("CollisionProbabilityTest", () => AssertionHelper.Verify(() =>
            {
                double probability = store.GetCollisionProbability();
                TestLog.WriteLine("Statistical Population = {0}", store.Count);
                TestLog.WriteLine("Actual Collision Probability = {0}", probability);

                if (probability <= collisionProbabilityLimit)
                    return null;

                return new AssertionFailureBuilder("Expected the collision probability to be less than the specified limit.")
                    .AddRawExpectedValue(collisionProbabilityLimit)
                    .AddRawActualValue(probability)
                    .SetStackTrace(Context.GetStackTraceData())
                    .ToAssertionFailure();
            }));
        }

        private Test CreateUniformDistributionTest(HashStore store)
        {
            return new TestCase("UniformDistributionTest", () => AssertionHelper.Verify(() =>
            {
                var result = store.GetChiSquareGoodnessToFit();
                TestLog.WriteLine("Statistical Population = {0}", store.Count);
                TestLog.WriteLine("Chi square value = {0}", result.ChiSquareValue);
                TestLog.WriteLine("Degrees of freedom = {0}", result.DegreesOfFreedom);
                TestLog.WriteLine("Two-tailed P value = {0}", result.TwoTailedPValue);

                if (1 - result.TwoTailedPValue <= uniformDistributionQuality)
                    return null;

                return new AssertionFailureBuilder("Expected the statement probability to be greater than the specified significance level.")
                    .AddRawExpectedValue(uniformDistributionQuality)
                    .AddRawActualValue(1 - result.TwoTailedPValue)
                    .SetStackTrace(Context.GetStackTraceData())
                    .ToAssertionFailure();
            }));
        }
    }

    /// <summary>
    /// A list of common limits for the collision probability.
    /// </summary>
    /// <seealso cref="HashCodeAcceptanceContract{T}.CollisionProbabilityLimit"/>
    public static class CollisionProbability
    {
        /// <summary>
        /// No collision allowed (0%)
        /// </summary>
        public static readonly double Perfect = 0.0;

        /// <summary>
        /// Represents a very low probability of collision (less than 1%) 
        /// </summary>
        public static readonly double VeryLow = 0.01;

        /// <summary>
        /// Represents a low probability of collision (less than 5%) 
        /// </summary>
        public static readonly double Low = 0.05;

        /// <summary>
        /// Represents a reasonable probability of collision (less than 10%) 
        /// </summary>
        public static readonly double Medium = 0.1;

        /// <summary>
        /// Represents a mediocre probability of collision (less than 30%) 
        /// </summary>
        public static readonly double High = 0.3;
    }

    /// <summary>
    /// A list of common limits for the uniform distribution significance level.
    /// </summary>
    /// <seealso cref="HashCodeAcceptanceContract{T}.UniformDistributionQuality"/>
    public static class UniformDistributionQuality
    {
        /// <summary>
        /// Nearly uniform (less than 1% of deviation probability)
        /// </summary>
        public static readonly double Excellent = 0.01;

        /// <summary>
        /// Characterizes a roughly uniform distribution (less than 5% of deviation probability)
        /// </summary>
        public static readonly double Good = 0.05;

        /// <summary>
        /// Characterizes a reasonably uniform distribution (less than 10% of deviation probability)
        /// </summary>
        public static readonly double Fair = 0.1;

        /// <summary>
        /// Characterizes a poor uniform distribution (less than 30% of deviation probability) 
        /// </summary>
        public static readonly double Mediocre = 0.3;
    }
}



