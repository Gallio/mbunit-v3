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
using System.Reflection;
using System.Text;
using Gallio;

namespace MbUnit.Framework.ContractVerifiers
{
    /// <summary>
    /// <para>
    /// Verifies the equality contract for an equatable type.
    /// </para>
    /// <para>
    /// Built-in verifications:
    /// <list type="bullet">
    /// <item>Equality and difference according to <see cref="Object.Equals(object)" /></item>
    /// <item>Equality and difference according to <see cref="IEquatable{T}.Equals(T)" /></item>
    /// <item>Equality and difference according to the '==' and '!=' operators</item>
    /// <item>Equal objects have the same hashcode according to <see cref="Object.GetHashCode()" /></item>
    /// </list>
    /// </para>
    /// </summary>
    public abstract class EqualityContractVerifier<T> : ContractVerifier<T>, IDistinctInstanceProvider<T>
        where T : IEquatable<T>
    {
        /// <inheritdoc />
        public abstract IEnumerable<T> GetDistinctInstances();

        /// <summary>
        /// Verifies <see cref="Object.Equals(object)" />.
        /// </summary>
        [Test]
        public void ObjectEquals()
        {
            VerifyEqualityContract(delegate(T a, T b) { return a.Equals((object)b); });
        }

        /// <summary>
        /// Verifies <see cref="Object.GetHashCode()" />.
        /// </summary>
        [Test]
        public void ObjectGetHashCode()
        {
            VerifyHashCodeContract(delegate(T value) { return value.GetHashCode(); });
        }

        /// <summary>
        /// Verifies <see cref="IEquatable{T}.Equals(T)" />.
        /// </summary>
        [Test]
        public void EquatableEquals()
        {
            VerifyEqualityContract(delegate(T a, T b) { return a.Equals(b); });
        }

        /// <summary>
        /// Verifies the "==" operator.
        /// </summary>
        [Test]
        public void OperatorEquals()
        {
            MethodInfo @operator = typeof(T).GetMethod("op_Equality", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static,
                null, new Type[] { typeof(T), typeof(T) }, null);

            VerifyEqualityContract(delegate(T a, T b)
            {
                return (bool) @operator.Invoke(null, new object[] { a, b });
            });
        }

        /// <summary>
        /// Verifies the "!=" operator.
        /// </summary>
        [Test]
        public void OperatorNotEquals()
        {
            MethodInfo @operator = typeof(T).GetMethod("op_Inequality", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static,
                null, new Type[] { typeof(T), typeof(T) }, null);

            VerifyEqualityContract(delegate(T a, T b)
            {
                return ! (bool)@operator.Invoke(null, new object[] { a, b });
            });
        }

        /// <summary>
        /// Ensures that an equality operations is correctly implemented.
        /// </summary>
        /// <param name="equals">The equality operation</param>
        protected void VerifyEqualityContract(Func<T, T, bool> equals)
        {
            int i = 0;
            foreach (T a in GetDistinctInstances())
            {
                Assert.IsTrue(equals(a, a), "Equality operator should consider '{0}' equal to itself.", a);

                int j = 0;
                foreach (T b in GetDistinctInstances())
                {
                    if (i == j)
                    {
                        Assert.IsTrue(equals(a, b), "Equality operator should consider '{0}' equal to '{1}'.", a, b);
                        Assert.IsTrue(equals(b, a), "Equality operator should condider '{0}' equal to '{1}'.", b, a);
                    }
                    else
                    {
                        Assert.IsFalse(equals(a, b), "Equality operator should consider '{0}' not equal to '{1}'.", a, b);
                        Assert.IsFalse(equals(b, a), "Equality operator should consider '{0}' not equal to '{1}'.", b, a);
                    }

                    j += 1;
                }

                i += 1;
            }
        }

        /// <summary>
        /// Ensures that a hash code operation is correctly implemented.
        /// </summary>
        /// <param name="getHashCode">The hash code operation</param>
        protected void VerifyHashCodeContract(Func<T, int> getHashCode)
        {
            InterimAssert.WithPairs(GetDistinctInstances(), GetDistinctInstances(), delegate(T a, T b)
            {
                Assert.AreEqual(getHashCode(a), getHashCode(b), "Hash code should return same value for '{0}' and '{1}'.", a, b);
            });
        }
    }
}
