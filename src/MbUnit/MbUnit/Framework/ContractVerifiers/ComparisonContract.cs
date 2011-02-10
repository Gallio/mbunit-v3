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
using Gallio.Common;
using Gallio.Framework.Assertions;
using MbUnit.Framework.ContractVerifiers.Core;

namespace MbUnit.Framework.ContractVerifiers
{
    /// <summary>
    /// Contract for verifying the implementation of the generic <see cref="IComparable{T}"/> interface. 
    /// </summary>
    /// <remarks>
    /// <para>
    /// Built-in verifications:
    /// <list type="bullet">
    /// <item>
    /// <strong>ComparableCompareTo</strong> : The type implements the method <see cref="IComparable{T}.CompareTo"/>.
    /// The method behaves as expected agains the provided equivalence classes.
    /// </item>
    /// <item>
    /// <strong>OperatorGreaterThan</strong> : The type has a static "Greater Than" operator overload which behaves
    /// correctly against the provided equivalence classes. Disable that test by setting 
    /// the <see cref="ComparisonContract{TTarget}.ImplementsOperatorOverloads"/> property to <c>false</c>.
    /// </item>
    /// <item>
    /// <strong>OperatorGreaterThanOrEqual</strong> : The type has a static "Greater Than Or Equal" operator 
    /// overload which behaves correctly against the provided equivalence classes. Disable that test by setting 
    /// the <see cref="ComparisonContract{TTarget}.ImplementsOperatorOverloads"/> property to <c>false</c>.
    /// </item>
    /// <item>
    /// <strong>OperatorLessThan</strong> : The type has a static "Less Than" operator overload which behaves
    /// correctly against the provided equivalence classes. Disable that test by setting 
    /// the <see cref="ComparisonContract{TTarget}.ImplementsOperatorOverloads"/> property to <c>false</c>.
    /// </item>
    /// <item>
    /// <strong>OperatorLessThanOrEqual</strong> : The type has a static "Less Than Or Equal" operator 
    /// overload which behaves correctly against the provided equivalence classes. Disable that test by setting 
    /// the <see cref="ComparisonContract{TTarget}.ImplementsOperatorOverloads"/> property to <c>false</c>.
    /// </item>
    /// </list>
    /// </para>
    /// </remarks>
    /// <example>
    /// The following example shows a simple class implementing the 
    /// <see cref="IComparable{T}"/> interface, and a test fixture which uses the
    /// comparison contract to test it.
    /// <code><![CDATA[
    /// public class SampleComparable : IComparable<SampleComparable>
    /// {
    ///     private int value;
    /// 
    ///     public SampleComparable(int value)
    ///     {
    ///         this.value = value;
    ///     }
    /// 
    ///     public int CompareTo(SampleComparable other)
    ///     {
    ///         return Object.ReferenceEquals(other, null) 
    ///             ? Int32.MaxValue 
    ///             : value.CompareTo(other.value);
    ///     }
    /// 
    ///     public static bool operator >=(SampleComparable left, SampleComparable right)
    ///     {
    ///         return (Object.ReferenceEquals(left, null) 
    ///             && Object.ReferenceEquals(right, null)) 
    ///             || (!Object.ReferenceEquals(left, null) 
    ///             && (left.CompareTo(right) >= 0));
    ///     }
    /// 
    ///     public static bool operator <=(SampleComparable left, SampleComparable right)
    ///     {
    ///         return Object.ReferenceEquals(left, null) 
    ///             || (left.CompareTo(right) <= 0);
    ///     }
    /// 
    ///     public static bool operator >(SampleComparable left, SampleComparable right)
    ///     {
    ///         return !Object.ReferenceEquals(left, null) 
    ///             && (left.CompareTo(right) > 0);
    ///     }
    /// 
    ///     public static bool operator <(SampleComparable left, SampleComparable right)
    ///     {
    ///         return (!Object.ReferenceEquals(left, null) 
    ///             || !Object.ReferenceEquals(right, null)) 
    ///             && (Object.ReferenceEquals(left, null) 
    ///             || (left.CompareTo(right) < 0));
    ///     }
    /// 
    /// public class SampleComparableTest
    /// {
    ///     [VerifyContract]
    ///     public readonly IContract EqualityTests = new ComparisonContract<SampleComparable>
    ///     {
    ///         ImplementsOperatorOverloads = true, // Optional (default is true)
    ///         EquivalenceClasses =
    ///         {
    ///             { new SampleComparable(1) },
    ///             { new SampleComparable(2) },
    ///             { new SampleComparable(3) },
    ///             { new SampleComparable(4) },
    ///             { new SampleComparable(5) }
    ///         }
    ///     };
    /// }
    /// ]]></code>
    /// </example>
    /// <typeparam name="TTarget">The type of the object to evaluate.</typeparam>
    /// <seealso cref="VerifyContractAttribute"/>
    public class ComparisonContract<TTarget> : AbstractContract
        where TTarget : IComparable<TTarget>
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        public ComparisonContract()
        {
            this.ImplementsOperatorOverloads = true;
            this.EquivalenceClasses = new EquivalenceClassCollection();
        }

        /// <summary>
        /// Determines whether the verifier will evaluate the presence and the 
        /// behavior of the 4 operator overloads "Greater Than", "Greater Than
        /// Or Equal", "Less Than", and "Less Than Or Equal".
        /// </summary>
        /// <remarks>
        /// <para>
        /// The default value is <c>true</c>.
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
            const BindingFlags bindingAllInstance = bindingCommon | BindingFlags.NonPublic | BindingFlags.Instance;
            const BindingFlags bindingAllStatic = bindingCommon | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.FlattenHierarchy;
            Type[] comparableTypes = GetAllComparableInterfaces();

            foreach (Type type in comparableTypes)
            {
                var typeCopy = type;
                yield return CreateComparisonTest<int>(new ComparisonSpecifications<int>
                {
                    Name = "ComparableCompareTo" + ((comparableTypes.Length == 1) ? String.Empty : "_" + typeCopy.Name),
                    MethodFriendlyName = String.Format("int CompareTo({0})", typeCopy.Name),
                    GetWorkingType = obj => GetComparableInterface(obj, typeCopy),
                    GetComparisonMethod = t => t.GetMethod("CompareTo", bindingAllInstance, null, new[] { typeCopy }, null),
                    IsComparisonMethodRequired = o => typeCopy.IsAssignableFrom(o.GetType()),
                    AdjustExpectedEquivalence = i => i,
                    FormatResult = i => (i < 0) ? "negative" : ((i > 0) ? "positive" : "zero"),
                    IsSecondArgumentCompatible = o => typeCopy.IsAssignableFrom(o.GetType()),
                });
            }

            if (ImplementsOperatorOverloads)
            {
                // Is "Greater Than" operator overload implementation OK?
                yield return CreateComparisonTest<bool>(new ComparisonSpecifications<bool>
                {
                    Name = "OperatorGreaterThan",
                    MethodFriendlyName = String.Format("static bool operator >({0}, {0})", typeof(TTarget).Name),
                    GetWorkingType = o => o.GetType(),
                    GetComparisonMethod = t => t.GetMethod("op_GreaterThan", bindingAllStatic, null, new[] { typeof(TTarget), typeof(TTarget) }, null),
                    IsComparisonMethodRequired = IsTargetTypeExactly,
                    AdjustExpectedEquivalence = i => i > 0,
                    FormatResult = b => b.ToString(),
                    IsSecondArgumentCompatible = IsTargetTypeOrDerived,
                });

                // Is "Greater Than Or Equal" operator overload implementation OK?
                yield return CreateComparisonTest<bool>(new ComparisonSpecifications<bool>
                {
                    Name = "OperatorGreaterThanOrEqual",
                    MethodFriendlyName = String.Format("static bool operator >=({0}, {0})", typeof(TTarget).Name),
                    GetWorkingType = o => o.GetType(),
                    GetComparisonMethod = t => t.GetMethod("op_GreaterThanOrEqual", bindingAllStatic, null, new[] { typeof(TTarget), typeof(TTarget) }, null),
                    IsComparisonMethodRequired = IsTargetTypeExactly,
                    AdjustExpectedEquivalence = i => i >= 0,
                    FormatResult = b => b.ToString(),
                    IsSecondArgumentCompatible = IsTargetTypeOrDerived,
                });

                // Is "Less Than" operator overload implementation OK?
                yield return CreateComparisonTest<bool>(new ComparisonSpecifications<bool>
                {
                    Name = "OperatorLessThan",
                    MethodFriendlyName = String.Format("static bool operator <({0}, {0})", typeof(TTarget).Name),
                    GetWorkingType = o => o.GetType(),
                    GetComparisonMethod = t => t.GetMethod("op_LessThan", bindingAllStatic, null, new[] { typeof(TTarget), typeof(TTarget) }, null),
                    IsComparisonMethodRequired = IsTargetTypeExactly,
                    AdjustExpectedEquivalence = i => i < 0,
                    FormatResult = b => b.ToString(),
                    IsSecondArgumentCompatible = IsTargetTypeOrDerived,
                });

                // Is "Less Than Or Equal" operator overload implementation OK?
                yield return CreateComparisonTest<bool>(new ComparisonSpecifications<bool>
                {
                    Name = "OperatorLessThanOrEqual",
                    MethodFriendlyName = String.Format("static bool operator <=({0}, {0})", typeof(TTarget).Name),
                    GetWorkingType = o => o.GetType(),
                    GetComparisonMethod = t => t.GetMethod("op_LessThanOrEqual", bindingAllStatic, null, new[] { typeof(TTarget), typeof(TTarget) }, null),
                    IsComparisonMethodRequired = IsTargetTypeExactly,
                    AdjustExpectedEquivalence = i => i <= 0,
                    FormatResult = b => b.ToString(),
                    IsSecondArgumentCompatible = IsTargetTypeOrDerived,
                });
            }
        }

        private struct ComparisonSpecifications<TResult>
        {
            public string Name;
            public string MethodFriendlyName;
            public Func<object, Type> GetWorkingType;
            public Func<Type, MethodInfo> GetComparisonMethod;
            public Func<object, bool> IsComparisonMethodRequired;
            public Func<int, TResult> AdjustExpectedEquivalence;
            public Func<TResult, string> FormatResult;
            public Func<object, bool> IsSecondArgumentCompatible;
        }

        private Test CreateComparisonTest<TResult>(ComparisonSpecifications<TResult> spec)
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
                            MethodInfo methodInfo = spec.GetComparisonMethod(workingType);

                            if (MethodExists(methodInfo, spec.MethodFriendlyName, spec.IsComparisonMethodRequired(pair.First)))
                            {
                                ComparerDelegate<TResult> comparer = GetBinaryComparer<TResult>(methodInfo);

                                if (pair.IsNewFirst)
                                {
                                    if (methodInfo.IsStatic && IsReferenceType(pair.First) && IsReferenceType(pair.Second))
                                        VerifyComparison<TResult>(spec, comparer, methodInfo, new InstancePair(null, null, 0));

                                    if (IsReferenceType(pair.Second))
                                        VerifyComparison<TResult>(spec, comparer, methodInfo, new InstancePair(pair.First, null, 1));
                                }

                                if (spec.IsSecondArgumentCompatible(pair.Second))
                                {
                                    VerifyComparison<TResult>(spec, comparer, methodInfo, pair);

                                    if (methodInfo.IsStatic && IsReferenceType(pair.First))
                                        VerifyComparison<TResult>(spec, comparer, methodInfo, new InstancePair(null, pair.Second, -1));
                                }
                            }
                        }
                    }
                }
            }));
        }

        private void VerifyComparison<TResult>(ComparisonSpecifications<TResult> spec, ComparerDelegate<TResult> comparer, MethodInfo methodInfo, InstancePair pair)
        {
            string actual = spec.FormatResult(comparer(pair.First, pair.Second));
            string expected = spec.FormatResult(spec.AdjustExpectedEquivalence(pair.Equivalence));

            AssertionHelper.Explain(() =>
                Assert.AreEqual(expected, actual),
                innerFailures => new AssertionFailureBuilder("The comparison between left and right values did not produce the expected result.")
                    .AddRawLabeledValue("Left Value", pair.First)
                    .AddRawLabeledValue("Right Value", pair.Second)
                    .AddRawLabeledValue("Expected Result", expected)
                    .AddRawLabeledValue("Actual Result", actual)
                    .SetStackTrace(Context.GetStackTraceData())
                    .AddInnerFailures(innerFailures)
                    .ToAssertionFailure());
        }

        private delegate TResult ComparerDelegate<TResult>(object first, object second);

        private static ComparerDelegate<TResult> GetBinaryComparer<TResult>(MethodInfo methodInfo)
        {
            return (first, second) => methodInfo.IsStatic
                ? (TResult)methodInfo.Invoke(null, new object[] { first, second })
                : (TResult)methodInfo.Invoke(first, new object[] { second });
        }

        #region Reflection helpers

        private struct InstancePair
        {
            private readonly object first;
            private readonly object second;
            private readonly int equivalence;
            private readonly bool isNewFirst;

            public object First { get { return first; } }
            public object Second { get { return second; } }
            public int Equivalence { get { return equivalence; } }
            public bool IsNewFirst { get { return isNewFirst; } }

            public InstancePair(object first, object second, int equivalence)
                : this(first, second, equivalence, false)
            {
            }

            public InstancePair(object first, object second, int equivalence, bool isNewFirst)
            {
                this.first = first;
                this.second = second;
                this.equivalence = equivalence;
                this.isNewFirst = isNewFirst;
            }
        }

        private Type[] GetAllComparableInterfaces()
        {
            var types = new List<Type>();

            foreach (object instance in GetAllInstances())
            {
                foreach (Type interfaceType in instance.GetType().FindInterfaces(Module.FilterTypeName, typeof(IComparable<>).Name))
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
                            yield return new InstancePair(first, second, i.CompareTo(j), isNewFirst);
                            isNewFirst = false;
                        }
                    }
                }
            }
        }

        private static Type GetComparableInterface(object instance, Type equatableType)
        {
            return GetInterface(instance.GetType(), typeof(IComparable<>).MakeGenericType(equatableType));
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
