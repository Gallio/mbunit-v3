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
using System.Text;
using Gallio.Framework.Pattern;
using Gallio.Reflection;
using System.Reflection;
using System.Runtime.Serialization;
using Gallio.Model;
using Gallio.Collections;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using Gallio.Framework.Data;
using Gallio.Framework.Assertions;

namespace MbUnit.Framework.ContractVerifiers
{
    /// <summary>
    /// <para>
    /// Attribute for a test fixture that verifies the implementation 
    /// contract of a custom exception type, derived from <see cref="Exception" />. 
    /// </para>
    /// <para>
    /// By default, the verifier will check for the serialization support 
    /// and the implementation of the 3 recommended constructors. Use the named 
    /// parameters <see cref="VerifyExceptionContractAttribute.ImplementsSerialization" />
    /// and <see cref="VerifyExceptionContractAttribute.ImplementsStandardConstructors" />
    /// to modify that behavior.
    /// </para>
    /// <example>
    /// <para>
    /// The following example declares a simple custom exception, 
    /// and tests it using the exception contract verifier.
    /// <code><![CDATA[
    /// [Serializable]
    /// public class MyException : Exception, ISerializable
    /// {
    ///     public MyException()
    ///     {
    ///     }
    ///
    ///     public MyException(string message)
    ///         : base(message)
    ///     {
    ///     }
    ///
    ///     public MyException(string message, Exception innerException)
    ///         : base(message, innerException)
    ///     {
    ///     }
    ///
    ///     protected MyException(SerializationInfo info, StreamingContext context)
    ///         : base(info, context)
    ///     {
    ///     }
    ///
    ///     public override void GetObjectData(SerializationInfo info, StreamingContext context)
    ///     {
    ///         base.GetObjectData(info, context);
    ///     }
    /// }
    /// 
    /// [VerifyExceptionContract(typeof(MyException)]
    /// public class MyExceptionTest
    /// {
    /// }
    /// ]]></code>
    /// </para>
    /// </example>
    /// </summary>
    [CLSCompliant(false)]
    [AttributeUsage(PatternAttributeTargets.TestType, AllowMultiple = false, Inherited = true)]
    public class VerifyExceptionContractAttribute : VerifyContractAttribute 
    {
        /// <summary>
        /// <para>
        /// Determines whether the verifier will check for the serialization support. 
        /// The default value is 'true'.
        /// </para>
        /// <para>
        /// Built-in verifications:
        /// <list type="bullet">
        /// <item>The exception implements the <see cref="ISerializable" /> interface.</item>
        /// <item>The exception has the <see cref="SerializableAttribute" /> attribute.</item>
        /// <item>The exception type has a protected serialization constructor similar to
        /// <see cref="Exception(SerializationInfo, StreamingContext)" />.</item>
        /// </list>
        /// </para>
        /// </summary>
        public bool ImplementsSerialization
        {
            get;
            set;
        }

        /// <summary>
        /// <para>
        /// Determines whether the verifier will check for the presence of
        /// the recommended standard constructors. The default value is 'true'.
        /// </para>
        /// <para>
        /// Built-in verifications:
        /// <list type="bullet">
        /// <item>The exception has a default argument-less constructor.</item>
        /// <item>The exception has a single argument constructor for the message.</item>
        /// <item>The exception two arguments constructor for the message and an inner exception.</item>
        /// </list>
        /// </para>
        /// </summary>
        public bool ImplementsStandardConstructors
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the type of the custom exception to verify. The type must derived
        /// from the <see cref="Exception"/> base class.
        /// </summary>
        public Type ExceptionType
        {
            get;
            private set;
        }

        /// <summary>
        /// <para>
        /// Attribute for a test fixture that verifies the implementation 
        /// contract of a custom exception type derived 
        /// from <see cref="Exception" />. 
        /// </para>
        /// <para>
        /// By default, the verifier will check for the serialization support 
        /// and the implementation of the 3 recommended constructors. Use the named 
        /// parameters <see cref="VerifyExceptionContractAttribute.ImplementsSerialization" />
        /// and <see cref="VerifyExceptionContractAttribute.ImplementsStandardConstructors" />
        /// to modify this behavior.
        /// </para>
        /// </summary>
        /// <param name="exceptionType">The custom exception type to verify. 
        /// It must derive from <see cref="Exception" />.</param>
        public VerifyExceptionContractAttribute(Type exceptionType)
            : base("ExceptionContract")
        {
            this.ImplementsSerialization = true;
            this.ImplementsStandardConstructors = true;
            this.ExceptionType = exceptionType;
        }

        /// <inheritdoc />
        protected override void Validate(PatternEvaluationScope scope, ICodeElementInfo codeElement)
        {
            base.Validate(scope, codeElement);

            if (ExceptionType == null)
                ThrowUsageErrorException("The specified exception type cannot be a null reference.");

            if (!ExceptionType.IsSubclassOf(typeof(Exception)))
                ThrowUsageErrorException("The specified exception type must derive from System.Exception.");
        }

        /// <inheritdoc />
        protected override void AddContractTests(PatternEvaluationScope scope)
        {
            if (ImplementsSerialization)
            {
                AddHasSerializableAttributeTest(scope);
                AddHasSerializationConstructorTest(scope);
            }

            if (ImplementsStandardConstructors)
            {
                AddIsDefaultConstructorWellDefinedTest(scope);
                AddIsStandardMessageConstructorWellDefinedTest(scope);
                AddIsStandardMessageAndInnerExceptionConstructorWellDefinedTest(scope);
            }
        }

        /// <summary>
        /// Adds a child test which verifies that the exception type has
        /// the <see cref="SerializableAttribute" /> attribute.
        /// </summary>
        /// <param name="scope">The pattern evaluation scope</param>
        private void AddHasSerializableAttributeTest(PatternEvaluationScope scope)
        {
            AddContractTest(
                scope, 
                "HasSerializableAttribute",
                "Verify that the type '" + ExceptionType.FullName + "' has the [Serializable] attribute.",
                state => 
                {
                    AssertionHelper.Verify(() =>
                    {
                        if (ExceptionType.IsDefined(typeof(SerializableAttribute), false))
                            return null;

                        return new AssertionFailureBuilder("Expected the exception type to be annotated by a particular attribute.")
                            .SetRawLabeledValue("Exception Type", ExceptionType)
                            .SetRawLabeledValue("Expected Attribute", typeof(SerializableAttribute))
                            .ToAssertionFailure();
                    });
                });
        }

        /// <summary>
        /// Adds a child test which verifies that the exception type has
        /// a non-public serializable constructor with the signature 
        /// '.ctor(SerializationInfo, StreamingContext)'.
        /// </summary>
        /// <param name="scope">The pattern evaluation scope</param>
        private void AddHasSerializationConstructorTest(PatternEvaluationScope scope)
        {
            AddContractTest(
                scope,
                "HasSerializationConstructor",
                "Verify that the type '" + ExceptionType.FullName + "' has a non-public serializable constructor with signature '.ctor(SerializationInfo, StreamingContext)'.",
                state =>
                {
                    AssertionHelper.Verify(() =>
                    {
                        if (ExceptionType.GetConstructor(
                            BindingFlags.NonPublic | BindingFlags.Instance, null,
                            new Type[] { typeof(SerializationInfo), typeof(StreamingContext) }, null) != null)
                            return null;

                        return new AssertionFailureBuilder("Expected the exception type to have non-public constructor dedicated to serialization.")
                            .SetRawLabeledValue("Exception Type", ExceptionType)
                            .SetLabeledValue("Constructor Signature", ".ctor(SerializationInfo, StreamingContext)")
                            .ToAssertionFailure();
                    });
                });
        }

        /// <summary>
        /// Adds a child test which verifies that the exception type has
        /// a valid default constructor.
        /// </summary>
        /// <param name="scope">The pattern evaluation scope</param>
        private void AddIsDefaultConstructorWellDefinedTest(PatternEvaluationScope scope)
        {
            AddContractTest(
                scope,
                "IsDefaultConstructorWellDefined",
                "Verify that the type '" + ExceptionType.FullName + "' has a default constructor.",
                state =>
                {
                    ConstructorInfo ctor = ExceptionType.GetConstructor(EmptyArray<Type>.Instance);

                    AssertionHelper.Verify(() =>
                    {
                        if (ctor != null)
                            return null;

                        return new AssertionFailureBuilder("The exception type should have a default parameter-less constructor.")
                            .SetRawLabeledValue("Exception Type", ExceptionType)
                            .SetLabeledValue("Expected Constructor Signature", ".ctor()")
                            .ToAssertionFailure();
                    });

                    Exception instance = (Exception)ctor.Invoke(null);

                    Assert.Multiple(() =>
                    {
                        AssertionHelper.Verify(() =>
                        {
                            if (instance.Message.Contains(ExceptionType.FullName))
                                return null;

                            return new AssertionFailureBuilder("The exception message should contain the exception type name.")
                                .SetRawLabeledValue("Exception Type", ExceptionType)
                                .SetLabeledValue("Actual Message", instance.Message)
                                .ToAssertionFailure();
                        });

                        AssertionHelper.Verify(() =>
                        {
                            if (instance.InnerException == null)
                                return null;

                            return new AssertionFailureBuilder("The inner exception should be null.")
                                .SetRawLabeledValue("Exception Type", ExceptionType)
                                .ToAssertionFailure();
                        });

                        if (ImplementsSerialization)
                            AssertMessageAndInnerExceptionPreservedByRoundTripSerialization(instance);
                    });
                });
        }

        /// <summary>
        /// Adds a child test which verifies that the exception type has
        /// a single argument constructor with the signature '.ctor(string)'.
        /// </summary>
        /// <param name="scope">The pattern evaluation scope</param>
        private void AddIsStandardMessageConstructorWellDefinedTest(PatternEvaluationScope scope)
        {
            PatternTest test = AddContractTest(
                scope,
                "IsStandardMessageConstructorWellDefined",
                "Verify that the type '" + ExceptionType.FullName + "' has a single argument constructor with signature '.ctor(string)'.",
                state =>
                {
                    // TODO: replace that ugly loop by a data-driven test.
                    foreach (string message in new string[] { null, "", "A message." })
                    {
                        ConstructorInfo ctor = ExceptionType.GetConstructor(new Type[] { typeof(string) });

                        AssertionHelper.Verify(() =>
                        {
                            if (ctor != null)
                                return null;
                            
                            return new AssertionFailureBuilder("The exception type should have a single parameter constructor.")
                                .SetRawLabeledValue("Exception Type", ExceptionType)
                                .SetLabeledValue("Expected Constructor Signature", ".ctor(string message)")
                                .ToAssertionFailure();
                        });

                        Exception instance = (Exception)ctor.Invoke(new object[] { message });

                        Assert.Multiple(() =>
                        {
                            AssertionHelper.Verify(() =>
                            {
                                if (instance.InnerException == null)
                                    return null;

                                return new AssertionFailureBuilder("The inner exception should be null.")
                                    .SetRawLabeledValue("Exception Type", ExceptionType)
                                    .ToAssertionFailure();
                            });

                            if (message == null)
                            {
                                AssertionHelper.Verify(() =>
                                {
                                    if (instance.Message.Contains(ExceptionType.FullName))
                                        return null;

                                    return new AssertionFailureBuilder("The exception message should to contain the exception type name.")
                                        .SetRawLabeledValue("Exception Type", ExceptionType)
                                        .SetLabeledValue("Actual Message", instance.Message)
                                        .ToAssertionFailure();
                                });
                            }
                            else
                            {
                                AssertionHelper.Verify(() =>
                                {
                                    if (message == instance.Message)
                                        return null;

                                    return new AssertionFailureBuilder("Expected the exception message to be equal to a specific text.")
                                        .SetRawLabeledValue("Exception Type", ExceptionType)
                                        .SetLabeledValue("Actual Message", instance.Message)
                                        .SetLabeledValue("Expected Message", message)
                                        .ToAssertionFailure();
                                });
                            }

                            if (ImplementsSerialization)
                                AssertMessageAndInnerExceptionPreservedByRoundTripSerialization(instance);
                        });
                    }
                });

            // A data-driven should be declare like that:
            // But so far, it does not work due to the non-null constraint on
            // the 'ISlotType' argument passed to the constructor of PatternTestParameter.

            //PatternTestDataContext context = test.DataContext.CreateChild();
            //DataSource dataSource = context.DefineDataSource("MyDataSource"); 
            //dataSource.AddDataSet(new ValueSequenceDataSet(new object[] { "A message.", "Another message." }, null, false)); 
            //test.AddParameter(new PatternTestParameter("message", null, context));
        }

        /// <summary>
        /// Adds a child test which verifies that the exception type has
        /// a two arguments constructor with signature '.ctor(string, Exception)'.
        /// </summary>
        /// <param name="scope">The pattern evaluation scope</param>
        private void AddIsStandardMessageAndInnerExceptionConstructorWellDefinedTest(PatternEvaluationScope scope)
        {
            AddContractTest(
                scope,
                "IsStandardMessageAndInnerExceptionConstructorWellDefined",
                "Verify that the type '" + ExceptionType.FullName + "' has a two arguments constructor with signature '.ctor(string, Exception)'.",
                state =>
                {
                    // TODO: replace that awful double-loop by a data-driven test.
                    foreach (string message in new string[] { null, "", "A message." })
                        foreach (bool hasInnerException in new bool[] { true, false })
                        {
                            Exception innerException = hasInnerException ? new Exception("Test.") : null;
                            ConstructorInfo ctor = ExceptionType.GetConstructor(new Type[] { typeof(string), typeof(Exception) });

                            AssertionHelper.Verify(() =>
                            {
                                if (ctor != null)
                                    return null;

                                return new AssertionFailureBuilder("The exception type should have a two parameters constructor.")
                                    .SetRawLabeledValue("Exception Type", ExceptionType)
                                    .SetLabeledValue("Expected Constructor Signature", ".ctor(string message, Exception innerException)")
                                    .ToAssertionFailure();
                            });

                            Exception instance = (Exception)ctor.Invoke(new object[] { message, innerException });

                            Assert.Multiple(() =>
                            {
                                AssertionHelper.Verify(() =>
                                {
                                    if (Object.ReferenceEquals(innerException, instance.InnerException))
                                        return null;

                                    return new AssertionFailureBuilder("The inner exception should be referentially identical to the exception provided in the constructor.")
                                        .SetRawLabeledValue("Exception Type", ExceptionType)
                                        .SetRawLabeledValue("Actual Inner Exception", instance.InnerException)
                                        .SetRawLabeledValue("Expected Inner Exception", innerException)
                                        .ToAssertionFailure();
                                });

                                if (message == null)
                                {
                                    AssertionHelper.Verify(() =>
                                    {
                                        if (instance.Message.Contains(ExceptionType.FullName))
                                            return null;

                                        return new AssertionFailureBuilder("The exception message should to contain the exception type name.")
                                            .SetRawLabeledValue("Exception Type", ExceptionType)
                                            .SetLabeledValue("Actual Message", instance.Message)
                                            .ToAssertionFailure();
                                    });
                                }
                                else
                                {
                                    AssertionHelper.Verify(() =>
                                    {
                                        if (message == instance.Message)
                                            return null;

                                        return new AssertionFailureBuilder("Expected the exception message to be equal to a specific text.")
                                            .SetRawLabeledValue("Exception Type", ExceptionType)
                                            .SetLabeledValue("Actual Message", instance.Message)
                                            .SetLabeledValue("Expected Message", message)
                                            .ToAssertionFailure();
                                    });
                                }

                                if (ImplementsSerialization)
                                    AssertMessageAndInnerExceptionPreservedByRoundTripSerialization(instance);
                            });
                        }
                });
        }

        /// <summary>
        /// Verifies that the <see cref="Exception.Message" /> and 
        /// <see cref="Exception.InnerException" />
        /// properties are preserved by round-trip serialization.
        /// </summary>
        /// <param name="instance">The exception instance.</param>
        protected void AssertMessageAndInnerExceptionPreservedByRoundTripSerialization(Exception instance)
        {
            Exception result = RoundTripSerialize(instance);

            AssertionHelper.Verify(() =>
            {
                if (result.Message == instance.Message)
                    return null;

                return new AssertionFailureBuilder("Expected the exception message to be preserved by round-trip serialization.")
                    .SetRawLabeledValue("Exception Type", ExceptionType)
                    .SetLabeledValue("Expected Message", instance.Message)
                    .SetLabeledValue("Actual Message ", result.Message)
                    .ToAssertionFailure();
            });

            if (instance.InnerException == null)
            {
                AssertionHelper.Verify(() =>
                {
                    if (result.InnerException == null)
                        return null;

                    return new AssertionFailureBuilder("The inner exception should be preserved by round-trip serialization.")
                        .SetRawLabeledValue("Exception Type", ExceptionType)
                        .SetRawLabeledValue("Actual Inner Exception", instance.InnerException)
                        .SetRawLabeledValue("Expected Inner Exception", result.InnerException)
                        .ToAssertionFailure();
                });
            }
            else
            {
                AssertionHelper.Verify(() =>
                {
                    if ((result.InnerException.GetType() == instance.InnerException.GetType()) &&
                        (result.InnerException.Message == instance.InnerException.Message))
                        return null;

                    return new AssertionFailureBuilder("The inner exception should be preserved by round-trip serialization.")
                        .SetRawLabeledValue("Exception Type", ExceptionType)
                        .SetRawLabeledValue("Actual Inner Exception", instance.InnerException)
                        .SetRawLabeledValue("Expected Inner Exception", result.InnerException)
                        .ToAssertionFailure();
                });
            }
        }

        /// <summary>
        /// Performs round-trip serialization of the exception and returns the result.
        /// </summary>
        /// <param name="instance">The exception instance.</param>
        /// <returns>The instance produced after serialization and deserialization</returns>
        protected static Exception RoundTripSerialize(Exception instance)
        {
            using (Stream stream = new MemoryStream())
            {
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Serialize(stream, instance);
                stream.Position = 0;
                return (Exception)formatter.Deserialize(stream);
            }
        }
    }
}
