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
using Gallio.Common.Collections;
using Gallio.Framework.Assertions;
using MbUnit.Framework.ContractVerifiers.Core;

namespace MbUnit.Framework.ContractVerifiers
{
    /// <summary>
    /// Contract for verifying the implementation of public type accessors, usually the getter and the setter of a particular property.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Built-in verifications:
    /// <list type="bullet">
    /// <item>
    /// <strong>SetValidValues</strong> : The setter accepts the values specified in the <see cref="ValidValues"/> 
    /// contract property as valid assignment values,
    /// and the getter returns equal values (object equality) when called afterwards.
    /// </item>
    /// <item>
    /// <strong>SetInvalidValues</strong> : The setter rejects the values specified in the <see cref="InvalidValues"/> 
    /// contract property by throwing the appropriate exception. The test is not run when the <see cref="InvalidValues"/> 
    /// contract property is left empty.
    /// </item>
    /// <item>
    /// <strong>SetNullValue</strong> : The setter rejects or accepts a null reference value according to the state of the 
    /// <see cref="AcceptNullValue"/> contract property. If set to true, the setter is expected to accept a null reference 
    /// as a valid value, and the getter to return a null reference as well. If set to false, the setter is expected to reject 
    /// a null reference assignment by throwing a <see cref="ArgumentNullException"/> exception. If the type handled by 
    /// the tested accessors is not nullable (a value type by example), the test is not run.
    /// </item>
    /// </list>
    /// </para>
    /// </remarks>
    /// <example>
    /// The following examples shows a simple class that contains a property, and a test fixture which declares an accessor
    /// contract to verify the expected behavior of the subject property.
    /// <code><![CDATA[
    /// public class Foo
    /// {
    ///     private string name;
    ///     
    ///     public string Name
    ///     {
    ///         get
    ///         {
    ///             return name;
    ///         }
    ///         
    ///         set
    ///         {
    ///             if (value == null)
    ///             {
    ///                 throw new ArgumentNullException("value");
    ///             }
    ///         
    ///             if (value.Length == 0)
    ///             {
    ///                 throw new ArgumentException("Cannot be an empty string.", "value");
    ///             }
    /// 
    ///             name = value;
    ///         }
    ///     }
    /// }
    /// 
    /// [TestFixture]
    /// public class FooTest
    /// {
    ///     [VerifyContract]
    ///     public readonly IContract NameAccessorTests = new AccessorContract<Foo, string>
    ///     {
    ///         PropertyName = "Name",
    ///         AcceptNullValue = false,
    ///         ValidValues = { "Value1", "Value2" },
    ///         InvalidValues =
    ///         {
    ///             { typeof(ArgumentException), String.Empty }
    ///         }
    ///     };
    /// }
    /// ]]></code>
    /// </example>
    /// <typeparam name="TTarget">The type of the tested object which contain the accessors.</typeparam>
    /// <typeparam name="TValue">The type of the value handled by the accessors.</typeparam>
    /// <seealso cref="VerifyContractAttribute"/>
    public class AccessorContract<TTarget, TValue> : AbstractContract
    {
        /// <summary>
        /// Provides a default instance of the tested type.
        /// </summary>
        /// <remarks>
        /// <para>
        /// By default, the contract verifier attempts to invoke the default constructor to get an valid instance. 
        /// Overwrite the default provider if the type has no default constructor, or if you want to use a particular instance.
        /// </para>
        /// </remarks>
        /// <example>
        /// <code><![CDATA[
        /// [VerifyContract]
        /// public readonly IContract AccessorTest = new AccessorContract<Foo, int>
        /// {
        ///     DefaultInstance = () => new Foo("Hello")
        ///     // Other initialization stuff...
        /// };
        /// ]]></code>
        /// </example>
        public Func<TTarget> DefaultInstance
        {
            get;
            set;
        }

        /// <summary>
        /// Specifies an explicit way to set a value to the tested accessors.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Identify the tested accessors by using one of the following methods:
        /// <list type="bullet">
        /// <item>
        /// <strong>Explicitly invoke the getter and the setter.</strong> : Specify how to invoke to getter and 
        /// the setter by providing appropriate  delegates to the <see cref="Getter"/> and <see cref="Setter"/> 
        /// contract properties. The <see cref="PropertyName"/> contract property must be left unitialized (null).
        /// </item>
        /// <item>
        /// <strong>Specify the name of a property.</strong> : Set the name of the tested property by feeding the 
        /// <see cref="PropertyName"/> contract property with a valid name. The explicit <see cref="Getter"/> and 
        /// <see cref="Setter"/> contract properties must then be left uninitialized (null).
        /// </item>
        /// </list>
        /// </para>
        /// </remarks>
        /// <example>
        /// The following example shows how to specify explicitely a getter and a setter.
        /// <code><![CDATA[
        /// public class Foo
        /// {
        ///     public string Name
        ///     {
        ///         get;
        ///         set;
        ///     }
        /// }
        /// 
        /// [TestFixture]
        /// public class FooTest
        /// {
        ///     [VerifyContract]
        ///     public readonly IContract NameAccessorTests = new AccessorContract<Foo, string>
        ///     {
        ///         Setter = (target, value) => target.Name = value,
        ///         Getter = (target) => target.Name,
        ///         ValidValues = { "Value1", "Value2" },
        ///     };
        /// }
        /// ]]></code>
        /// </example>
        /// <seealso cref="Getter"/>
        /// <seealso cref="PropertyName"/>
        public Action<TTarget, TValue> Setter
        {
            get;
            set;
        }

        /// <summary>
        /// Specifies an explicit way to get a value from the tested accessors.
        /// </summary>
        /// <seealso cref="Setter"/>
        /// <seealso cref="PropertyName"/>
        public Accessor<TTarget, TValue> Getter
        {
            get;
            set;
        }

        /// <summary>
        /// Specifies the name of tested property.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Identify the tested accessors with one of the following methods:
        /// <list type="bullet">
        /// <item>
        /// <strong>Explicitly invoke the getter and the setter.</strong> : Specify how to invoke to getter and 
        /// the setter by providing appropriate delegates to the <see cref="Getter"/> and <see cref="Setter"/> 
        /// contract properties. The <see cref="PropertyName"/> contract property must be left unitialized (null).
        /// </item>
        /// <item>
        /// <strong>Specify the name of a property.</strong> : Set the name of the tested property by feeding the 
        /// <see cref="PropertyName"/> contract property with a valid name. The explicit <see cref="Getter"/> and 
        /// <see cref="Setter"/> contract properties must then be left uninitialized (null).
        /// </item>
        /// </list>
        /// </para>
        /// </remarks>
        /// <example>
        /// The following example shows how to specify the name of the property.
        /// <code><![CDATA[
        /// public class Foo
        /// {
        ///     public string Name
        ///     {
        ///         get;
        ///         set;
        ///     }
        /// }
        /// 
        /// [TestFixture]
        /// public class FooTest
        /// {
        ///     [VerifyContract]
        ///     public readonly IContract NameAccessorTests = new AccessorContract<Foo, string>
        ///     {
        ///         PropertyName = "Name",
        ///         ValidValues = { "Value1", "Value2" },
        ///     };
        /// }
        /// ]]></code>
        /// </example>
        /// <seealso cref="Getter"/>
        /// <seealso cref="Setter"/>
        public string PropertyName
        {
            get;
            set;
        }

        /// <summary>
        /// Determines whether the tested accessors are expected to accept a null reference as a valid input.
        /// </summary>
        /// <remarks>
        /// <para>
        /// If set to <c>true</c>, the setter is expected to accept a null reference as a valid value, and the getter
        /// to return a null reference as well. If set to <c>false</c>, the setter is expected to reject a null reference assignment by
        /// throwing a <see cref="ArgumentNullException"/> exception. If the type handled by the tested accessors is not nullable 
        /// (a value type by example), the contract property is ignored.
        /// </para>
        /// <para>
        /// The use of that contract property is optional, and its default state is <c>true</c>.
        /// </para>
        /// </remarks>
        public bool AcceptNullValue
        {
            get;
            set;
        }

        /// <summary>
        /// Defines a collection of distinct object instances that are expected to be inconditionally accepted as
        /// valid input by the tested setter. Feeding that contract property with at least one value is mandatory.
        /// </summary>
        /// <example>
        /// The following example shows how to specify some valid values for the contract verifier:
        /// <code><![CDATA[
        /// [TestFixture]
        /// public class FooTest
        /// {
        ///     [VerifyContract]
        ///     public readonly IContract MyPropertyAccessorTests = new AccessorContract<Foo, int>
        ///     {
        ///         PropertyName = "MyProperty",
        ///         ValidValues = { 123, 456, 789 },
        ///     };
        /// }
        /// ]]></code>
        /// </example>
        /// <seealso cref="DistinctInstanceCollection{T}"/>
        public DistinctInstanceCollection<TValue> ValidValues
        {
            get;
            set;
        }

        /// <summary>
        /// Defines a collection of incompetence classes which identify object instances that are expected to be not accepted
        /// by the tested setter. Each incompetence class contains a collection of distinct object instance associated 
        /// with an expected exception type.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Specifying incompetent values is entirely optional. By default, the entire collection is empty; which causes the 
        /// test 'SetInvalidValues' to not be run.
        /// </para>
        /// </remarks>
        /// <example>
        /// The following example shows how to specify some incompetent values for the contract verifier. The example
        /// assumes that the hypothetical 'Foo.MyProperty' property accepts any integer between 0 and 999, except 666
        /// which is expected to throw a <see cref="ArgumentException"/> exception.
        /// <code><![CDATA[
        /// [TestFixture]
        /// public class FooTest
        /// {
        ///     [VerifyContract]
        ///     public readonly IContract MyPropertyAccessorTests = new AccessorContract<Foo, int>
        ///     {
        ///         PropertyName = "MyProperty",
        ///         ValidValues = { 123, 456, 789 },
        ///         IncompetentValues =
        ///         {
        ///             { typeof(ArgumentOutOfRangeException), -100, -999, 1000, 99999 },
        ///             { typeof(ArgumentException), 666 }
        ///         }
        ///     };
        /// }
        /// ]]></code>
        /// </example>
        /// <seealso cref="InvalidValuesClassCollection{T}"/>
        /// <seealso cref="InvalidValuesClass{T}"/>
        public InvalidValuesClassCollection<TValue> InvalidValues
        {
            get;
            set;
        }

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Use the named parameters <see cref="DefaultInstance"/>, <see cref="Getter"/>, 
        /// <see cref="Setter"/>, <see cref="PropertyName"/>, <see cref="ValidValues"/>, 
        /// and <see cref="InvalidValues"/> to specify some options.
        /// </para>
        /// </remarks>
        public AccessorContract()
        {
            this.AcceptNullValue = true;
            this.ValidValues = new DistinctInstanceCollection<TValue>();
            this.DefaultInstance = () => Activator.CreateInstance<TTarget>();
            this.InvalidValues = new InvalidValuesClassCollection<TValue>();
        }

        private void AssertValid()
        {
            Assert.Multiple(() =>
            {
                if (PropertyName == null)
                {
                    AssertionHelper.Explain(() =>
                        {
                            Assert.IsNotNull(Setter);
                            Assert.IsNotNull(Getter);
                        },
                        innerFailures => new AssertionFailureBuilder("Expected the 'Setter' and the 'Getter' properties to be not " +
                            "null when the name of the tested property is not specified. Please specify a valid action that assigns " +
                            "the given value to the target object and a valid function that returns the current value from the target " +
                            "object, or specify the name of the tested property.")
                            .AddInnerFailures(innerFailures)
                            .SetStackTrace(Context.GetStackTraceData())
                            .ToAssertionFailure());
                }
                else
                {
                    AssertionHelper.Explain(() =>
                        {
                            Assert.IsNull(Setter);
                            Assert.IsNull(Getter);
                        },
                        innerFailures => new AssertionFailureBuilder("Expected the 'Setter' and the 'Getter' properties to be " +
                            "null when the name of the tested property is specified.")
                            .AddInnerFailures(innerFailures)
                            .SetStackTrace(Context.GetStackTraceData())
                            .ToAssertionFailure());

                    AssertionHelper.Explain(() =>
                        Assert.IsNotNull(GetProperty()),
                        innerFailures => new AssertionFailureBuilder("Expected the tested property to be found. " +
                            "Only instance public (or internal) properties can be tested.")
                            .AddRawLabeledValue("Tested Type", typeof(TTarget))
                            .AddLabeledValue("Searched Property Name", PropertyName)
                            .AddInnerFailures(innerFailures)
                            .SetStackTrace(Context.GetStackTraceData())
                            .ToAssertionFailure());


                }

                AssertionHelper.Explain(() =>
                    Assert.IsNotEmpty(ValidValues),
                      innerFailures => new AssertionFailureBuilder("Expected collection of valid value instances to be not empty.")
                        .SetMessage("Please feed the collection of valid values with '{0}' instances.", typeof(TValue))
                        .AddInnerFailures(innerFailures)
                        .SetStackTrace(Context.GetStackTraceData())
                        .ToAssertionFailure());
            });
        }

        /// <inheritdoc />
        protected override IEnumerable<Test> GetContractVerificationTests()
        {
            yield return CreateSetValidValuesTest("SetValidValues");

            if (InvalidValues.Count > 0)
            {
                yield return CreateSetInvalidValuesTest("SetInvalidValues");
            }

            if (!typeof(TValue).IsValueType)
            {
                yield return CreateSetNullValueTest("SetNullValue");
            }
        }

        private Test CreateSetValidValuesTest(string name)
        {
            return new TestCase(name, () =>
            {
                AssertValid();
                var target = GetSafeDefaultInstance();

                Assert.Multiple(() =>
                {
                    foreach (TValue value in ValidValues)
                    {
                        AssertValueRoundtrip(target, value);
                    }
                });
            });
        }

        private Test CreateSetNullValueTest(string name)
        {
            return new TestCase(name, () =>
            {
                AssertValid();
                var target = GetSafeDefaultInstance();

                if (AcceptNullValue)
                {
                    AssertValueRoundtrip(target, default(TValue));
                }
                else
                {
                    AssertionHelper.Explain(() =>
                        Assert.Throws<ArgumentNullException>(() => SetValue(target, default(TValue))),
                        innerFailures => new AssertionFailureBuilder(
                        "Expected the tested 'Setter' to throw a specific exception while invoked with a null value.")
                        .AddRawLabeledValue("Expected Exception", typeof(ArgumentNullException))
                        .AddInnerFailures(innerFailures)
                        .SetStackTrace(Context.GetStackTraceData())
                        .ToAssertionFailure());
                }
            });
        }

        private Test CreateSetInvalidValuesTest(string name)
        {
            return new TestCase(name, () =>
            {
                AssertValid();
                var target = GetSafeDefaultInstance();

                Assert.Multiple(() =>
                {
                    foreach (var incompetenceClass in InvalidValues)
                    {
                        foreach (var incompetentValue in incompetenceClass)
                        {
                            AssertionHelper.Explain(() =>
                                Assert.Throws(incompetenceClass.ExpectedExceptionType, () => SetValue(target, incompetentValue)),
                                innerFailures => new AssertionFailureBuilder(
                                "Expected the 'Setter' to throw a specific exception while invoked with an invalid value.")
                                .AddRawLabeledValue("Invalid Value", incompetentValue)
                                .AddRawLabeledValue("Expected Exception", incompetenceClass.ExpectedExceptionType)
                                .AddInnerFailures(innerFailures)
                                .SetStackTrace(Context.GetStackTraceData())
                                .ToAssertionFailure());
                        }
                    }
                });
            });
        }

        private void SetValue(TTarget target, TValue value)
        {
            if (PropertyName == null)
            {
                Setter(target, value);
            }
            else
            {
                try
                {
                    GetProperty().SetValue(target, value, null);
                }
                catch (TargetInvocationException exception)
                {
                    throw exception.InnerException;
                }
            }
        }

        private TValue GetValue(TTarget target)
        {
            if (PropertyName == null)
            {
                return Getter(target);
            }
            else
            {
                try
                {
                    return (TValue)GetProperty().GetValue(target, null);
                }
                catch (TargetInvocationException exception)
                {
                    throw exception.InnerException;
                }
            }
        }

        private PropertyInfo GetProperty()
        {
            return typeof(TTarget).GetProperty(PropertyName, BindingFlags.Public | BindingFlags.Instance);
        }

        private void AssertValueRoundtrip(TTarget target, TValue value)
        {
            AssertionHelper.Explain(() =>
                Assert.DoesNotThrow(() => SetValue(target, value)),
                innerFailures => new AssertionFailureBuilder(
                "Expected the tested 'Setter' to not throw any exception while invoked with a specific value.")
                .AddRawActualValue(value)
                .AddInnerFailures(innerFailures)
                .SetStackTrace(Context.GetStackTraceData())
                .ToAssertionFailure());
                        
            TValue actual = default(TValue);

            AssertionHelper.Explain(() =>
                Assert.DoesNotThrow(() => actual = GetValue(target)),
                innerFailures => new AssertionFailureBuilder(
                "Expected the tested 'Getter' to not throw any exception while invoked to get the actual value.")
                .AddInnerFailures(innerFailures)
                .SetStackTrace(Context.GetStackTraceData())
                .ToAssertionFailure());

            AssertionHelper.Explain(() =>
                Assert.AreEqual(value, actual),
                innerFailures => new AssertionFailureBuilder(
                "Expected the 'Getter' to provide an actual value equal to the value previously assigned with the 'Setter'.")
                .AddRawExpectedValue(value)
                .AddRawActualValue(actual)
                .AddInnerFailures(innerFailures)
                .SetStackTrace(Context.GetStackTraceData())
                .ToAssertionFailure());
        }

        private TTarget GetSafeDefaultInstance()
        {
            TTarget target = default(TTarget);

            AssertionHelper.Explain(() =>
                Assert.DoesNotThrow(() => target = DefaultInstance()),
                innerFailures => new AssertionFailureBuilder(
                "Cannot instantiate a default instance of the tested type.")
                .SetMessage("Please feed the contract property 'DefaultInstance' with a valid instance of type '{0}'.", typeof(TTarget))
                .AddInnerFailures(innerFailures)
                .SetStackTrace(Context.GetStackTraceData())
                .ToAssertionFailure());

            return target;
        }
    }
}



