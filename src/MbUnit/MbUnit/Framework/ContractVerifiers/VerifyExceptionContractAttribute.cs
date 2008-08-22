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
    /// to modify this behavior.
    /// </para>
    /// <example>
    /// <para>
    /// The following example declares a simple custom exception named "MyException", 
    /// and tests it using the exception contract verifier.
    /// <code><![CDATA[
    /// [Serializable]
    /// public class MyException : Exception, ISerializable
    /// {
    ///     public SerializedExceptionSample()
    ///     {
    ///     }
    ///
    ///     public SerializedExceptionSample(string message)
    ///         : base(message)
    ///     {
    ///     }
    ///
    ///     public SerializedExceptionSample(string message, Exception innerException)
    ///         : base(message, innerException)
    ///     {
    ///     }
    ///
    ///     protected SerializedExceptionSample(SerializationInfo info, StreamingContext context)
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
    /// [VerifyExceptionContract(typeof(MyException),
    ///     ImplementsSerialization = false,
    ///     ImplementsStandardConstructors = true)]
    /// private class MyExceptionTest
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
        /// <param name="exceptionType">The custom exception type to verify. It must derive from <see cref="Exception" />.</param>
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
        /// the 'Serializable' attribute.
        /// </summary>
        /// <param name="scope"></param>
        private void AddHasSerializableAttributeTest(PatternEvaluationScope scope)
        {
            AddContractTest(
                scope, 
                "HasSerializableAttribute",
                "Verify that the type '" + ExceptionType.FullName + "' has the [Serializable] attribute.",
                state =>
                {
                    Assert.IsTrue(ExceptionType.IsDefined(typeof(SerializableAttribute), false),
                        "Type '{0}' should have the [Serializable] attribute.", ExceptionType.FullName);
                });
        }

        /// <summary>
        /// Adds a child test which verifies that the exception type has
        /// a non-public serializable constructor with the signature 
        /// '.ctor(SerializationInfo, StreamingContext)'.
        /// </summary>
        /// <param name="scope"></param>
        private void AddHasSerializationConstructorTest(PatternEvaluationScope scope)
        {
            AddContractTest(
                scope,
                "HasSerializationConstructor",
                "Verify that the type '" + ExceptionType.FullName + "' has a non-public serializable constructor with signature '.ctor(SerializationInfo, StreamingContext)'.",
                state =>
                {
                    ConstructorInfo ctor = ExceptionType.GetConstructor(BindingFlags.NonPublic | BindingFlags.Instance,
                        null, new Type[] { typeof(SerializationInfo), typeof(StreamingContext) }, null);
                    Assert.IsNotNull(ctor, "Type '{0}' should have a non-public serializable constructor with signature '.ctor(SerializationInfo, StreamingContext)'", ExceptionType.FullName);
                });
        }

        /// <summary>
        /// Adds a child test which verifies that the exception type has
        /// a valid default constructor.
        /// </summary>
        /// <param name="scope"></param>
        private void AddIsDefaultConstructorWellDefinedTest(PatternEvaluationScope scope)
        {
            AddContractTest(
                scope,
                "IsDefaultConstructorWellDefined",
                "Verify that the type '" + ExceptionType.FullName + "' has a default constructor.",
                state =>
                {
                    ConstructorInfo ctor = ExceptionType.GetConstructor(EmptyArray<Type>.Instance);
                    Assert.IsNotNull(ctor, "Type '{0}' should have should have a default constructor.", ExceptionType.FullName);
                    Exception instance = (Exception)ctor.Invoke(null);
                    Assert.IsTrue(instance.Message.Contains(ExceptionType.FullName), "The message text must contain the exception type.");
                    Assert.IsNull(instance.InnerException, "The inner exception should be null.");

                    if (ImplementsSerialization)
                        AssertMessageAndInnerExceptionPreservedByRoundTripSerialization(instance);
                });
        }

        /// <summary>
        /// Adds a child test which verifies that the exception type has
        /// a single argument constructor with the signature '.ctor(string)'.
        /// </summary>
        /// <param name="scope"></param>
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
                        Assert.IsNotNull(ctor, "Type '{0}' should have should have a single argument constructor with signature '.ctor(string)'.", ExceptionType.FullName);
                        Exception instance = (Exception)ctor.Invoke(new object[] { message });
                        Assert.IsNull(instance.InnerException, "The inner exception should be null.");

                        if (message == null)
                            Assert.IsTrue(instance.Message.Contains(ExceptionType.FullName), "The message text must contain the exception type.");
                        else
                            Assert.AreEqual(message, instance.Message, "The message text should be equal to the provided message.");

                        if (ImplementsSerialization)
                            AssertMessageAndInnerExceptionPreservedByRoundTripSerialization(instance);
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
        /// <param name="scope"></param>
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
                            Assert.IsNotNull(ctor, "Type '{0}' should have should have a two arguments constructor with signature '.ctor(string, Exception)'.", ExceptionType.FullName);
                            Exception instance = (Exception)ctor.Invoke(new object[] { message, innerException });
                            Assert.AreSame(innerException, instance.InnerException, "The inner exception should be equal to the provided exception.");

                            if (message == null)
                                Assert.IsTrue(instance.Message.Contains(ExceptionType.FullName), "The message text must contain the exception type.");
                            else
                                Assert.AreEqual(message, instance.Message, "The message text should be equal to the provided message.");

                            if (ImplementsSerialization)
                                AssertMessageAndInnerExceptionPreservedByRoundTripSerialization(instance);
                        }
                });
        }

        /// <summary>
        /// Verifies that the <see cref="Exception.Message" /> and <see cref="Exception.InnerException" />
        /// properties are preserved by round-trip serialization.
        /// </summary>
        /// <param name="instance">The exception instance.</param>
        protected static void AssertMessageAndInnerExceptionPreservedByRoundTripSerialization(Exception instance)
        {
            Exception result = RoundTripSerialize(instance);
            Assert.AreEqual(result.Message, instance.Message, "The exception message should be preserved by round-trip serialization.");

            if (instance.InnerException == null)
                Assert.IsNull(result.InnerException, "The inner exception should be preserved by round-trip serialization.");
            else
                Assert.IsInstanceOfType(instance.InnerException.GetType(), result.InnerException, "The inner exception should be preserved by round-trip serialization.");
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
