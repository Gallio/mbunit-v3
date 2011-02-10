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
using System.Text;
using Gallio.Common;
using Gallio.Common.Collections;
using Gallio.Framework;
using Gallio.Framework.Assertions;
using Gallio.Runtime.Formatting;
using MbUnit.Framework.ContractVerifiers.Core;

namespace MbUnit.Framework.ContractVerifiers
{
    /// <summary>
    /// Contract for verifying the implementation of the generic <see cref="IEquatable{T}"/> interface. 
    /// </summary>
    /// <remarks>
    /// <para>
    /// Built-in verifications:
    /// <list type="bullet">
    /// <item>
    /// <strong>ObjectEquals</strong> : The <see cref="Object.Equals(object)"/> method was overriden and 
    /// behaves correctly against the provided equivalence classes.
    /// </item>
    /// <item>
    /// <strong>ObjectGetHashCode</strong> : The <see cref="Object.GetHashCode"/> method was overriden and behaves 
    /// correctly against the provided equivalence classes.
    /// </item>
    /// <item>
    /// <strong>EquatableEquals</strong> : The <see cref="IEquatable{T}.Equals"/> method is implemented
    /// and behaves as expected agains the provided equivalence classes.
    /// </item>
    /// <item>
    /// <strong>OperatorEquals</strong> : The type has a static equality operator (==) overload which behaves
    /// correctly against the provided equivalence classes. Disable that test by setting 
    /// the <see cref="EqualityContract{TTarget}.ImplementsOperatorOverloads"/> 
    /// property to <c>false</c>.
    /// </item>
    /// <item>
    /// <strong>OperatorNotEquals</strong> : The type has a static inequality operator (!=) overload which 
    /// behaves correctly against the provided equivalence classes. Disable that test by 
    /// setting the <see cref="EqualityContract{TTarget}.ImplementsOperatorOverloads"/> 
    /// property to <c>false</c>.
    /// </item>
    /// </list>
    /// </para>
    /// </remarks>
    /// <example>
    /// The following example shows a simple class implementing the 
    /// <see cref="IEquatable{T}"/> interface, and a test fixture which uses the
    /// equality contract to test it.
    /// <code><![CDATA[
    /// public class SampleEquatable : IEquatable<SampleEquatable>
    /// {
    ///     private int value;
    /// 
    ///     public SampleEquatable(int value)
    ///     {
    ///         this.value = value;
    ///     }
    /// 
    ///     public override int GetHashCode()
    ///     {
    ///         return value.GetHashCode();
    ///     }
    /// 
    ///     public override bool Equals(object obj)
    ///     {
    ///         return Equals(obj as SampleEquatable);
    ///     }
    /// 
    ///     public bool Equals(SampleEquatable other)
    ///     {
    ///         return !Object.ReferenceEquals(other, null) 
    ///             && (value == other.value);
    ///     }
    /// 
    ///     public static bool operator ==(SampleEquatable left, SampleEquatable right)
    ///     {
    ///         return (Object.ReferenceEquals(left, null)
    ///             && Object.ReferenceEquals(right, null))
    ///             || (!Object.ReferenceEquals(left, null) 
    ///             && left.Equals(right));
    ///     }
    ///     
    ///     public static bool operator !=(SampleEquatable left, SampleEquatable right)
    ///     {
    ///         return !(left == right);
    ///     }
    /// }
    /// 
    /// public class SampleEquatableTest
    /// {
    ///     [VerifyContract]
    ///     public readonly IContract EqualityTests = new EqualityContract<SampleEquatable>
    ///     {
    ///         ImplementsOperatorOverloads = true, // Optional (default is true)
    ///         EquivalenceClasses =
    ///         {
    ///             { new SampleEquatable(1) },
    ///             { new SampleEquatable(2) },
    ///             { new SampleEquatable(3) },
    ///             { new SampleEquatable(4) },
    ///             { new SampleEquatable(5) }
    ///         }
    ///     };
    /// }
    /// ]]></code>
    /// </example>
    /// <typeparam name="TTarget">The target tested type which implements the generic <see cref="IEquatable{T}"/> interface.</typeparam>
    /// <seealso cref="VerifyContractAttribute"/>
    public class EqualityContract<TTarget> : AbstractContract
        where TTarget : IEquatable<TTarget>
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        public EqualityContract()
        {
            ImplementsOperatorOverloads = true;
            EquivalenceClasses = new EquivalenceClassCollection();
        }

        /// <summary>
        /// Determines whether the verifier will evaluate the presence and the 
        /// behavior of the equality and the inequality operator overloads.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The default value is <c>true</c>.
        /// </para>
        /// <para>
        /// Built-in verifications:
        /// <list type="bullet">
        /// <item>The type has a static equality operator (==) overload which 
        /// behaves correctly against the provided equivalence classes.</item>
        /// <item>The type has a static inequality operator (!=) overload which 
        /// behaves correctly against the provided equivalence classes.</item>
        /// <item></item>
        /// </list>
        /// </para>
        /// </remarks>
        public bool ImplementsOperatorOverloads
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the collection of equivalence classes of instances
        /// to feed the contract verifier.
        /// </summary>
        public EquivalenceClassCollection EquivalenceClasses
        {
            get;
            set;
        }

        /// <inheritdoc />
        protected override IEnumerable<Test> GetContractVerificationTests()
        {
            const BindingFlags bindingCommon = BindingFlags.InvokeMethod | BindingFlags.Public;
            const BindingFlags bindingPublicInstance = bindingCommon | BindingFlags.Instance;
            const BindingFlags bindingAllInstance = bindingPublicInstance | BindingFlags.NonPublic;
            const BindingFlags bindingAllStatic = bindingCommon | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.FlattenHierarchy;

            // Is Object equality method OK?
            yield return CreateEqualityTest(new EqualitySpecifications
            {
                Name = "ObjectEquals",
                MethodFriendlyName = "bool Equals(Object)",
                ExpectedEquality = true,
                GetWorkingType = o => o.GetType(),
                GetEqualityMethod = t => t.GetMethod("Equals", bindingPublicInstance, null, new[] { typeof(object) }, null),
                IsEqualityMethodRequired = o => true,
                IsSecondArgumentCompatible = o => true,
            });

            // Is Object hash code calculcation well implemented?
            yield return CreateHashCodeTest();

            // Is strongly typed IEquatable equality method OK?
            Type[] equatableTypes = GetAllEquatableInterfaces();

            foreach (Type type in equatableTypes)
            {
                var typeCopy = type;
                yield return CreateEqualityTest(new EqualitySpecifications
                {
                    Name = "EquatableEquals" + ((equatableTypes.Length == 1) ? String.Empty : "_" + typeCopy.Name),
                    MethodFriendlyName = String.Format("bool Equals({0})", typeCopy.Name),
                    ExpectedEquality = true,
                    GetWorkingType = obj => GetEquatableInterface(obj, typeCopy),
                    GetEqualityMethod = t => t.GetMethod("Equals", bindingAllInstance, null, new[] { typeCopy }, null),
                    IsEqualityMethodRequired = o => typeCopy.IsAssignableFrom(o.GetType()),
                    IsSecondArgumentCompatible = o => typeCopy.IsAssignableFrom(o.GetType()),
                });
            }

            if (ImplementsOperatorOverloads)
            {
                // Is equality operator overload OK?
                yield return CreateEqualityTest(new EqualitySpecifications
                {
                    Name = "OperatorEquals",
                    MethodFriendlyName = String.Format("static bool operator ==({0}, {0})", typeof(TTarget).Name),
                    ExpectedEquality = true,
                    GetWorkingType = o => o.GetType(),
                    GetEqualityMethod = t => t.GetMethod("op_Equality", bindingAllStatic, null, new[] { typeof(TTarget), typeof(TTarget) }, null),
                    IsEqualityMethodRequired = IsTargetTypeExactly,
                    IsSecondArgumentCompatible = IsTargetTypeOrDerived,
                });

                // Is inequality operator overload OK?
                yield return CreateEqualityTest(new EqualitySpecifications
                {
                    Name = "OperatorNotEquals",
                    MethodFriendlyName = String.Format("static bool operator !=({0}, {0})", typeof(TTarget).Name),
                    ExpectedEquality = false,
                    GetWorkingType = o => o.GetType(),
                    GetEqualityMethod = t => t.GetMethod("op_Inequality", bindingAllStatic, null, new[] { typeof(TTarget), typeof(TTarget) }, null),
                    IsEqualityMethodRequired = IsTargetTypeExactly,
                    IsSecondArgumentCompatible = IsTargetTypeOrDerived,
                });
            }
        }

        private Test CreateHashCodeTest()
        {
            return new TestCase("HashCode", () => Assert.Multiple(() =>
            {
                foreach (InstancePair pair in GetAllPairsInSameClasses())
                {
                    if (pair.AreEquivalent && IsTargetTypeOrDerived(pair.First) && IsTargetTypeOrDerived(pair.Second))
                    {
                        VerifyHashCodeOfEquivalentInstances(pair.First, pair.Second);
                    }
                }
            }));
        }

        private void VerifyHashCodeOfEquivalentInstances(object first, object second)
        {
            int firstHashCode = first.GetHashCode();
            int secondHashCode = second.GetHashCode();

            AssertionHelper.Explain(() =>
                Assert.AreEqual(firstHashCode, secondHashCode),
                innerFailures => new AssertionFailureBuilder(
                    "Expected the hash codes of two values within the same equivalence class to be equal.")
                    .AddRawLabeledValue("First Value", first)
                    .AddRawLabeledValue("First Hash Code", firstHashCode)
                    .AddRawLabeledValue("Second Value", second)
                    .AddRawLabeledValue("Second Hash Code", secondHashCode)
                    .SetStackTrace(Context.GetStackTraceData())
                    .AddInnerFailures(innerFailures)
                    .ToAssertionFailure());
        }

        private struct EqualitySpecifications
        {
            public string Name;
            public string MethodFriendlyName;
            public Func<object, Type> GetWorkingType;
            public Func<Type, MethodInfo> GetEqualityMethod;
            public Func<object, bool> IsEqualityMethodRequired;
            public bool ExpectedEquality;
            public Func<object, bool> IsSecondArgumentCompatible;
        }

        private Test CreateEqualityTest(EqualitySpecifications spec)
        {
            return new TestCase(spec.Name, () => Assert.Multiple(() =>
            {
                foreach (InstancePair pair in GetAllPairs())
                {
                    if (IsTargetTypeOrDerived(pair.First))
                    {
                        Type workingType = spec.GetWorkingType(pair.First);

                        if (workingType != null)
                        {
                            MethodInfo methodInfo = spec.GetEqualityMethod(workingType);

                            if (MethodExists(methodInfo, spec.MethodFriendlyName, spec.IsEqualityMethodRequired(pair.First)))
                            {
                                BinaryCompareDelegate comparer = GetBinaryComparer(methodInfo);

                                if (pair.IsNewFirst)
                                {
                                    if (methodInfo.IsStatic && IsReferenceType(pair.First) && IsReferenceType(pair.Second))
                                        VerifyEquality(spec, comparer, methodInfo, new InstancePair(null, null, true));

                                    if (IsReferenceType(pair.Second))
                                        VerifyEquality(spec, comparer, methodInfo, new InstancePair(pair.First, null, false));
                                }

                                if (spec.IsSecondArgumentCompatible(pair.Second))
                                {
                                    VerifyEquality(spec, comparer, methodInfo, pair);

                                    if (methodInfo.IsStatic && IsReferenceType(pair.First))
                                        VerifyEquality(spec, comparer, methodInfo, new InstancePair(null, pair.Second, false));
                                }
                            }
                        }
                    }
                }
            }));
        }

        private void VerifyEquality(EqualitySpecifications spec, BinaryCompareDelegate comparer, MethodInfo methodInfo, InstancePair pair)
        {
            bool actual = comparer(pair.First, pair.Second);
            bool expected = !(pair.AreEquivalent ^ spec.ExpectedEquality);

            AssertionHelper.Explain(() =>
                Assert.AreEqual(expected, actual),
                innerFailures => new AssertionFailureBuilder(
                    "The equality comparison between left and right values did not produce the expected result.")
                    .AddRawLabeledValue("Left Value", pair.First)
                    .AddRawLabeledValue("Right Value", pair.Second)
                    .AddRawLabeledValue("Expected Result", expected)
                    .AddRawLabeledValue("Actual Result", actual)
                    .SetStackTrace(Context.GetStackTraceData())
                    .AddInnerFailures(innerFailures)
                    .ToAssertionFailure());
        }

        private delegate bool BinaryCompareDelegate(object first, object second);

        private static BinaryCompareDelegate GetBinaryComparer(MethodInfo methodInfo)
        {
            return (first, second) => methodInfo.IsStatic
                ? (bool)methodInfo.Invoke(null, new[] { first, second })
                : (bool)methodInfo.Invoke(first, new[] { second });
        }

        #region Reflection Helpers

        private struct InstancePair
        {
            private readonly object first;
            private readonly object second;
            private readonly bool areEquivalent;
            private readonly bool isNewFirst;

            public object First { get { return first; } }
            public object Second { get { return second; } }
            public bool AreEquivalent { get { return areEquivalent; } }
            public bool IsNewFirst { get { return isNewFirst; } }

            public InstancePair(object first, object second, bool areEquivalent)
                : this(first, second, areEquivalent, false)
            { 
            }

            public InstancePair(object first, object second, bool areEquivalent, bool isNewFirst)
            {
                this.first = first;
                this.second = second;
                this.areEquivalent = areEquivalent;
                this.isNewFirst = isNewFirst;
            }
        }

        private Type[] GetAllEquatableInterfaces()
        {
            var types = new List<Type>();

            foreach (object instance in GetAllInstances())
            {
                foreach (Type interfaceType in instance.GetType().FindInterfaces(Module.FilterTypeName, typeof(IEquatable<>).Name))
                {
                    Type type = interfaceType.GetGenericArguments()[0];

                    if (!types.Contains(type))
                        types.Add(type);
                }
            }

            return types.ToArray();
        }

        private IEnumerable<object> GetAllInstances()
        {
            foreach (var @class in EquivalenceClasses)
            {
                foreach (object instance in @class)
                {
                    yield return instance;
                }
            }
        }

        private IEnumerable<InstancePair> GetAllPairsInSameClasses()
        {
            foreach (var @class in EquivalenceClasses)
            {
                bool isNewFirst = true;

                foreach (object first in @class)
                {
                    foreach (object second in @class)
                    {
                        yield return new InstancePair(first, second, true, isNewFirst);
                        isNewFirst = false;
                    }
                }
            }
        }

        private IEnumerable<InstancePair> GetAllPairs()
        {
            for (int i = 0; i < EquivalenceClasses.Count; i++)
            {
                bool isNewFirst = true;

                foreach (object first in EquivalenceClasses[i])
                {
                    for (int j = 0; j < EquivalenceClasses.Count; j++)
                    {
                        foreach (object second in EquivalenceClasses[j])
                        {
                            yield return new InstancePair(first, second, i == j, isNewFirst);
                            isNewFirst = false;
                        }
                    }
                }
            }
        }

        private static Type GetEquatableInterface(object instance, Type equatableType)
        {
            return GetInterface(instance.GetType(), typeof(IEquatable<>).MakeGenericType(equatableType));
        }

        private static bool IsTargetTypeOrDerived(object obj)
        {
            return typeof(TTarget).IsAssignableFrom(obj.GetType());
        }

        private static bool IsTargetTypeExactly(object obj)
        {
            return typeof(TTarget) == obj.GetType();
        }

        private static bool IsValueType(object obj)
        {
            return obj.GetType().IsValueType;
        }

        private static bool IsReferenceType(object obj)
        {
            return !obj.GetType().IsValueType;
        }

        #endregion
    }
}
