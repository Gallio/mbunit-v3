using System;
using System.Collections;
using System.Reflection;
using Gallio;
using Gallio.Framework.Pattern;
using Gallio.Reflection;

namespace MbUnit.Framework.ContractVerifiers
{
    /// <summary>
    /// <para>
    /// Attribute for test fixtures that verify the implementation 
    /// contract of a type implementing the generic <see cref="IComparable{T}"/> interface. 
    /// </para>
    /// <para>
    /// The test fixture must implement the <see cref="IEquivalenceClassProvider{T}"/> interface 
    /// which provides a set of equivalence classes of distinct object instances to be used by
    /// the contract verifier.
    /// </para>
    /// <para>
    /// By default, the verifier will evaluate the behavior of the 
    /// <see cref="IComparable{T}.CompareTo"/> method. It will verify as well the good 
    /// implementation of the four comparison operator overloads (Greater Than, Less Than, 
    /// Greater Than Or Equal, and Less Than Or Equal). Use the named parameters 
    /// <see cref="VerifyEqualityContractAttribute.ImplementsOperatorOverloads"/>
    /// to disable that verification.
    /// </para>
    /// <example>
    /// <para>
    /// The following example declares a simple comparable "Foo" class and tests it using the comparison
    /// contract verifier. The Foo class has a contructor which takes one single Int32 argument, which
    /// is used internally by the class for the implementation of the comparison contract.
    /// <code><![CDATA[
    /// public class Foo : IComparable<Foo>
    /// {
    ///     private int value;
    ///     
    ///     public Foo(int value)
    ///     {
    ///         this.value = value;
    ///     }
    /// 
    ///     public int CompareTo(Foo other) 
    ///     { 
    ///         return (other == null) ? Int32.MaxValue : value.CompareTo(other.value);
    ///     }
    /// 
    ///     public static bool operator >=(Foo left, Foo right)
    ///     {
    ///         return ((left == null) && (right == null)) || ((left != null) && (left.CompareTo(right) >= 0));
    ///     }
    ///
    ///     public static bool operator <=(Foo left, Foo right)
    ///     {
    ///         return (left == null) || (left.CompareTo(right) <= 0);
    ///     }
    ///
    ///     public static bool operator >(Foo left, Foo right)
    ///     {
    ///         return (left != null) && (left.CompareTo(right) > 0);
    ///     }
    ///
    ///     public static bool operator <(Foo left, Foo right)
    ///     {
    ///         return ((left != null) || (right != null)) && ((left == null) || (left.CompareTo(right) < 0));
    ///     }
    /// }
    /// 
    /// [VerifyComparisonContract(typeof(Foo), ImplementsOperatorOverloads = true)]
    /// public class FooTest : IEquivalenceClassProvider<Foo>
    /// {
    ///     public EquivalenceClassCollection<Foo> GetEquivalenceClasses()
    ///     {
    ///         return EquivalenceClassCollection<Foo>.FromDistinctInstances(
    ///             null,
    ///             new Foo(1),
    ///             new Foo(2),
    ///             new Foo(5),
    ///             new Foo(36));
    ///     }
    /// }
    /// ]]></code>
    /// For nullable comparable types, such as the "Foo" class in the example, 
    /// consider as a good practice to include a null reference as the unique element 
    /// of your first equivalence class. Indeed, by definition, any object compares 
    /// greater than null reference, and two null references compare equal to each other.
    /// </para>
    /// </example>
    /// </summary>
    [CLSCompliant(false)]
    [AttributeUsage(PatternAttributeTargets.TestType, AllowMultiple = false, Inherited = true)]
    public class VerifyComparisonContractAttribute : VerifyContractAttribute
    {
        /// <summary>
        /// <para>
        /// Determines whether the verifier will evaluate the presence and the 
        /// behavior of the four comparison operator overloads.
        /// </para>
        /// Built-in verifications:
        /// <list type="bullet">
        /// <item>The type has a static "greater than" operator overload which 
        /// behaves correctly against the provided equivalence classes.</item>
        /// <item>The type has a static "less than" operator overload which 
        /// behaves correctly against the provided equivalence classes.</item>
        /// <item>The type has a static "greater than or equal" operator overload which 
        /// behaves correctly against the provided equivalence classes.</item>
        /// <item>The type has a static "less than or equal" operator overload which 
        /// behaves correctly against the provided equivalence classes.</item>
        /// </list>
        /// </summary>
        public bool ImplementsOperatorOverloads
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the type of the object to verify. The type must implement
        /// the generic <see cref="IComparable{T}"/> interface.
        /// </summary>
        public Type Type
        {
            get;
            private set;
        }

        /// <summary>
        /// <para>
        /// Attribute for test fixtures that verify the implementation 
        /// contract of a type implementing the generic <see cref="IComparable{T}"/> interface. 
        /// </para>
        /// <para>
        /// The test fixture must implement the <see cref="IEquivalenceClassProvider{T}"/> interface 
        /// which provides a set of equivalence classes of object instances to be used by
        /// the contract verifier.
        /// </para>
        /// <para>
        /// By default, the verifier will evaluated the behavior of the 
        /// <see cref="IComparable{T}.CompareTo"/> method. It will verify as well the good 
        /// implementation of the four comparison operator overloads (greater than, less than, 
        /// greater than or equal, and less than or equal). Use the named parameters 
        /// <see cref="VerifyEqualityContractAttribute.ImplementsOperatorOverloads"/>
        /// to disable that verification.
        /// </para>
        /// </summary>
        /// <param name="type">the type of the object to verify. The type must implement
        /// the generic <see cref="IComparable{T}"/> interface.</param>
        public VerifyComparisonContractAttribute(Type type)
            : base("ComparisonContract")
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }
            
            this.ImplementsOperatorOverloads = true;
            this.Type = type;
        }

        /// <inheritdoc />
        protected override void Validate(PatternEvaluationScope scope, ICodeElementInfo codeElement)
        {
            base.Validate(scope, codeElement);

            if (GetIComparableInterface() == null)
                ThrowUsageErrorException("The specified type must implements the generic System.IComparable interface.");
        }

        /// <inheritdoc />
        protected override void AddContractTests(PatternEvaluationScope scope)
        {
            AddComparableCompareToTest(scope);

            if (ImplementsOperatorOverloads)
            {
                AddOperatorGreaterThanTest(scope);
                AddOperatorLessThanTest(scope);
                AddOperatorGreaterThanOrEqualTest(scope);
                AddOperatorLessThanOrEqualTest(scope);
            }
        }

        /// <summary>
        /// Verifies the implementation and the behavior of <see cref="IComparable{T}.CompareTo" />.
        /// </summary>
        /// <param name="scope">The pattern evaluation scope.</param>
        private void AddComparableCompareToTest(PatternEvaluationScope scope)
        {
            AddContractTest(
                scope,
                "ComparableCompareTo",
                "Verify the implementation of 'IComparable<T>.CompareTo()' on the type '" + Type.FullName + "'.",
                state =>
                {
                    MethodInfo compares = GetIComparableInterface().GetMethod("CompareTo",
                        BindingFlags.InvokeMethod | BindingFlags.Public | BindingFlags.Instance,
                        null, new Type[] { Type }, null);
                    Assert.IsNotNull(compares, "The type '" + Type.FullName + "' should implement the method 'bool CompareTo(" + Type.Name + ")'.");

                    VerifyComparisonContract<int>(state.FixtureType, state.FixtureInstance, false,
                        (a, b) =>
                        {
                            return Math.Sign((int)compares.Invoke(a, new object[] { b }));
                        },
                        (i, j) =>
                        {
                            return Math.Sign(i.CompareTo(j));
                        },
                        r =>
                        {
                            if (r == 0)
                            {
                                return "zero";
                            }
                            else if (r > 0)
                            {
                                return "a positive result";
                            }
                            else
                            {
                                return "a negative result";
                            }
                        });
                });
        }

        /// <summary>
        /// Verifies the implementation and the behavior of the static "greater than" operator overload.
        /// </summary>
        /// <param name="scope">The pattern evaluation scope.</param>
        private void AddOperatorGreaterThanTest(PatternEvaluationScope scope)
        {
            AddContractTest(
                scope,
                "OperatorGreaterThan",
                "Verify the implementation of the 'Greater Than' operator (>) overload on the type '" + Type.FullName + "'.",
                state =>
                {
                    MethodInfo @operator = Type.GetMethod("op_GreaterThan",
                        BindingFlags.InvokeMethod | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static,
                        null, new Type[] { Type, Type }, null);
                    Assert.IsNotNull(@operator, "The type '" + Type.FullName + "' should implement the static 'Greater Than' operator (>) overload.");

                    VerifyComparisonContract<bool>(state.FixtureType, state.FixtureInstance, true,
                        (a, b) =>
                        {
                            return (bool)@operator.Invoke(null, new object[] { a, b });
                        },
                        (i, j) =>
                        {
                            return (i > j);
                        },
                        r => r.ToString());
                });
        }

        /// <summary>
        /// Verifies the implementation and the behavior of the static "greater than or equal" operator overload.
        /// </summary>
        /// <param name="scope">The pattern evaluation scope.</param>
        private void AddOperatorGreaterThanOrEqualTest(PatternEvaluationScope scope)
        {
            AddContractTest(
                scope,
                "OperatorGreaterThanOrEqual",
                "Verify the implementation of the 'Greater Than Or Equal' operator (>=) overload on the type '" + Type.FullName + "'.",
                state =>
                {
                    MethodInfo @operator = Type.GetMethod("op_GreaterThanOrEqual",
                        BindingFlags.InvokeMethod | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static,
                        null, new Type[] { Type, Type }, null);
                    Assert.IsNotNull(@operator, "The type '" + Type.FullName + "' should implement the static 'Greater Than Or Equal' operator (>=) overload.");

                    VerifyComparisonContract<bool>(state.FixtureType, state.FixtureInstance, true,
                        (a, b) =>
                        {
                            return (bool)@operator.Invoke(null, new object[] { a, b });
                        },
                        (i, j) =>
                        {
                            return (i >= j);
                        },
                        r => r.ToString());
                });
        }

        /// <summary>
        /// Verifies the implementation and the behavior of the static "Less Than" operator overload.
        /// </summary>
        /// <param name="scope">The pattern evaluation scope.</param>
        private void AddOperatorLessThanTest(PatternEvaluationScope scope)
        {
            AddContractTest(
                scope,
                "OperatorLessThan",
                "Verify the implementation of the 'Less Than' operator (<) overload on the type '" + Type.FullName + "'.",
                state =>
                {
                    MethodInfo @operator = Type.GetMethod("op_LessThan",
                        BindingFlags.InvokeMethod | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static,
                        null, new Type[] { Type, Type }, null);
                    Assert.IsNotNull(@operator, "The type '" + Type.FullName + "' should implement the static 'Less Than' operator (<) overload.");

                    VerifyComparisonContract<bool>(state.FixtureType, state.FixtureInstance, true,
                        (a, b) =>
                        {
                            return (bool)@operator.Invoke(null, new object[] { a, b });
                        },
                        (i, j) =>
                        {
                            return (i < j);
                        },
                        r => r.ToString());
                });
        }

        /// <summary>
        /// Verifies the implementation and the behavior of the static "Less Than Or Equal" operator overload.
        /// </summary>
        /// <param name="scope">The pattern evaluation scope.</param>
        private void AddOperatorLessThanOrEqualTest(PatternEvaluationScope scope)
        {
            AddContractTest(
                scope,
                "OperatorLessThanOrEqual",
                "Verify the implementation of the 'Less Than Or Equal' operator (<=) overload on the type '" + Type.FullName + "'.",
                state =>
                {
                    MethodInfo @operator = Type.GetMethod("op_LessThanOrEqual",
                        BindingFlags.InvokeMethod | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static,
                        null, new Type[] { Type, Type }, null);
                    Assert.IsNotNull(@operator, "The type '" + Type.FullName + "' should implement the static 'Less Than Or Equal' operator (<=) overload.");

                    VerifyComparisonContract<bool>(state.FixtureType, state.FixtureInstance, true,
                        (a, b) =>
                        {
                            return (bool)@operator.Invoke(null, new object[] { a, b });
                        },
                        (i, j) =>
                        {
                            return (i <= j);
                        },
                        r => r.ToString());
                });
        }


        /// <summary>
        /// Casts the instance of the test fixture into a provider of equivalence classes, 
        /// then returns the resulting collection as an enumeration.
        /// </summary>
        /// <param name="fixtureType">The type of the fixture.</param>
        /// <param name="fixtureInstance">The fixture instance.</param>
        /// <returns></returns>
        protected IEnumerable GetEquivalentClasses(Type fixtureType, object fixtureInstance)
        {
            Type interfaceType = GetIEquivalenceClassProviderInterface(fixtureType);
            Assert.IsNotNull(interfaceType, "The equality contract verifier for the test fixture '{0}' must implement the interface 'IEquivalentClassProvider'.");
            return (IEnumerable)interfaceType.InvokeMember("GetEquivalenceClasses",
                BindingFlags.Public | BindingFlags.Instance | BindingFlags.InvokeMethod,
                null, fixtureInstance, null);
        }

        /// <summary>
        /// Verifies that the comparison operation gives the expected result
        /// for all the possible combinations between the objects found
        /// in all the available equivalence classes.
        /// </summary>
        /// <typeparam name="U">The type of the result returned by the comparison operation (usually Int32 or Boolean).</typeparam>
        /// <param name="fixtureType">The type of the test fixture.</param>
        /// <param name="fixtureInstance">The instance of test fixture.</param>
        /// <param name="isStaticMethodInvoked">Indicates whether the comparison method is based on the invocation of a static method (true) or an instance method (false).</param>
        /// <param name="compares">The comparison operation.</param>
        /// <param name="refers">The reference operation which provides the expected result.</param>
        /// <param name="formatsExpectedResult">Formats the expected result as a displayable text.</param>
        protected void VerifyComparisonContract<U>(Type fixtureType, object fixtureInstance, bool isStaticMethodInvoked, 
            Func<object, object, U> compares, Func<int, int, U> refers, Func<U, string> formatsExpectedResult)
        {
            int i = 0;

            foreach (object a in GetEquivalentClasses(fixtureType, fixtureInstance))
            {
                int j = 0;

                foreach (object b in GetEquivalentClasses(fixtureType, fixtureInstance))
                {
                    CompareEquivalentInstances<U>((IEnumerable)a, (IEnumerable)b,
                        isStaticMethodInvoked, refers(i, j), compares, formatsExpectedResult);
                    j++;
                }

                i++;
            }
        }

        /// <summary>
        /// Verifies that the comparison operation gives the expected result for
        /// all the possible combinations of objects found in the two specified
        /// equivalence classes.
        /// </summary>
        /// <typeparam name="U">The type of the result returned by the comparison operation (usually Int32 or Boolean).</typeparam>
        /// <param name="a">The first equivalence class.</param>
        /// <param name="b">The second equivalence class.</param>
        /// <param name="isStaticMethodInvoked">Indicates whether the comparison method is based on the invocation of a static method (true) or an instance method (false).</param>
        /// <param name="expectedResult">The expected result of the comparison.</param>
        /// <param name="compares">The comparison operation.</param>
        /// <param name="formatsExpectedResult">Formats the expected result as a displayable text.</param>
        protected void CompareEquivalentInstances<U>(IEnumerable a, IEnumerable b, bool isStaticMethodInvoked,
            U expectedResult, Func<object, object, U> compares, Func<U, string> formatsExpectedResult)
        {
            foreach (object x in a)
            {
                if (isStaticMethodInvoked || (x != null))
                {
                    foreach (object y in b)
                    {
                        Assert.AreEqual(expectedResult, compares(x, y),
                            "The comparison between '{0}' and '{1}' should give {2}.", 
                            x, y, formatsExpectedResult(expectedResult));
                    }
                }
            }
        }

        private Type GetIComparableInterface()
        {
            return GetInterface(Type, typeof(IComparable<>).MakeGenericType(Type));
        }

        private Type GetIEquivalenceClassProviderInterface(Type fixtureType)
        {
            return GetInterface(fixtureType, typeof(IEquivalenceClassProvider<>).MakeGenericType(Type));
        }
    }
}
