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
using System.IO;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using Gallio.Collections;

namespace MbUnit.Framework.ContractVerifiers
{
    /// <summary>
    /// <para>
    /// Verifies the equality contract for an exception type.
    /// </para>
    /// <para>
    /// Built-in verifications:
    /// <list type="bullet">
    /// <item>The exception has the <see cref="SerializableAttribute" /> attribute.</item>
    /// <item>The exception type has a protected serialization constructor similar to
    /// <see cref="Exception(SerializationInfo, StreamingContext)" />.</item>
    /// <item>If the exception type has standard constructors similar to <see cref="Exception()" />,
    /// <see cref="Exception(string)" /> or <see cref="Exception(string, Exception)" />, then
    /// verifies that they are well defined.  If any of these constructions are not intended
    /// to be defined, override the appropriate test method to disable it.</item>
    /// </list>
    /// </para>
    /// </summary>
    [CLSCompliant(false)]
    public abstract class ExceptionContractVerifier<T> : ContractVerifier<T>
        where T : Exception
    {
        /// <summary>
        /// Ensures that the exception has a <see cref="SerializableAttribute" /> attribute.
        /// </summary>
        [Test]
        public void HasSerializableAttribute()
        {
            Assert.IsTrue(typeof(T).IsDefined(typeof(SerializableAttribute), false),
                "Type '{0}' should have the [Serializable] attribute.", typeof(T));
        }

        /// <summary>
        /// Ensures that the exception has a non-public serialization constructor.
        /// </summary>
        [Test]
        public void HasSerializationConstructor()
        {
            ConstructorInfo ctor = typeof(T).GetConstructor(BindingFlags.NonPublic | BindingFlags.Instance,
                null, new Type[] { typeof(SerializationInfo), typeof(StreamingContext) }, null);
            Assert.IsNotNull(ctor, "Type '{0}' should have a non-public serializable constructor with signature '.ctor(SerializationInfo, StreamingContext)'", typeof(T));
        }

        /// <summary>
        /// Ensures that the exception has a well-defined default constructor.
        /// </summary>
        /// <remarks>
        /// Override to disable this test.
        /// </remarks>
        [Test]
        [DependsOn("HasSerializableAttribute")]
        [DependsOn("HasSerializationConstructor")]
        public virtual void DefaultConstructorIsWellDefined()
        {
            T instance = CreateInstanceUsingDefaultConstructor();
            Assert.IsNotNull(instance, "Type '{0}' should have a default constructor.  If you do not wish it to have one, then override this test to disable it.", typeof(T));

            Assert.IsTrue(instance.Message.Contains(typeof(T).FullName), "The message text contain the exception type.");
            Assert.IsNull(instance.InnerException, "The inner exception should be null.");

            AssertMessageAndInnerExceptionPreservedByRoundTripSerialization(instance);
        }

        /// <summary>
        /// Ensures that the exception has a well-defined single argument message constructor.
        /// </summary>
        /// <remarks>
        /// Override to disable this test.
        /// </remarks>
        [Test]
        [DependsOn("HasSerializableAttribute")]
        [DependsOn("HasSerializationConstructor")]
        public virtual void StandardMessageConstructorIsWellDefined(
            [Column(null, "", "A message.")] string message)
        {
            T instance = CreateInstanceUsingStandardMessageConstructor(message);
            Assert.IsNotNull(instance, "Type '{0}' should have a single argument constructor with signature '.ctor(string)'.  If you do not wish it to have one, then override this test to disable it.", typeof(T));

            if (message == null)
                Assert.IsTrue(instance.Message.Contains(typeof(T).FullName), "The message text contain the exception type.");
            else
                Assert.AreEqual(message, instance.Message, "The message text should be equal to the provided message.");

            Assert.IsNull(instance.InnerException, "The inner exception should be null.");

            AssertMessageAndInnerExceptionPreservedByRoundTripSerialization(instance);
        }

        /// <summary>
        /// Ensures that the exception has a well-defined two argument message and inner exception constructor.
        /// </summary>
        /// <remarks>
        /// Override to disable this test.
        /// </remarks>
        [Test]
        [DependsOn("HasSerializableAttribute")]
        [DependsOn("HasSerializationConstructor")]
        public virtual void StandardMessageAndInnerExceptionConstructorIsWellDefined(
            [Column(null, "", "A message.")] string message,
            [Column(false, true)] bool hasInnerException)
        {
            Exception innerException = hasInnerException ? new Exception("Test."): null;
            T instance = CreateInstanceUsingStandardMessageAndInnerExceptionConstructor(message, innerException);
            Assert.IsNotNull(instance, "Type '{0}' should have a two argument constructor with signature '.ctor(string, Exception)'.  If you do not wish it to have one, then override this test to disable it.", typeof(T));

            if (message == null)
                Assert.IsTrue(instance.Message.Contains(typeof(T).FullName), "The message text contain the exception type.");
            else
                Assert.AreEqual(message, instance.Message, "The message text should be equal to the provided message.");

            Assert.AreSame(innerException, instance.InnerException, "The inner exception should be equal to the provided exception.");

            AssertMessageAndInnerExceptionPreservedByRoundTripSerialization(instance);
        }

        /// <summary>
        /// Verifies that the <see cref="Exception.Message" /> and <see cref="Exception.InnerException" />
        /// properties are preserved by round-trip serialization.
        /// </summary>
        /// <param name="instance">The instance</param>
        protected static void AssertMessageAndInnerExceptionPreservedByRoundTripSerialization(T instance)
        {
            T result = RoundTripSerialize(instance);
            Assert.AreEqual(result.Message, instance.Message, "The exception message should be preserved by round-trip serialization.");

            if (instance.InnerException == null)
                Assert.IsNull(result.InnerException, "The inner exception should be preserved by round-trip serialization.");
            else
                Assert.IsInstanceOfType(instance.InnerException.GetType(), result.InnerException, "The inner exception should be preserved by round-trip serialization.");
        }

        /// <summary>
        /// Performs round-trip serialization of the exception and returns the result.
        /// </summary>
        /// <param name="instance">The instance</param>
        /// <returns>The instance produced after serialization and deserialization</returns>
        protected static T RoundTripSerialize(T instance)
        {
            using (Stream stream = new MemoryStream())
            {
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Serialize(stream, instance);
                stream.Position = 0;
                return (T) formatter.Deserialize(stream);
            }
        }

        /// <summary>
        /// Creates an exception instance using the default constructor.
        /// Returns null if not supported.
        /// </summary>
        /// <returns>The instance, or null if not supported</returns>
        protected static T CreateInstanceUsingDefaultConstructor()
        {
            ConstructorInfo ctor = typeof(T).GetConstructor(EmptyArray<Type>.Instance);
            return ctor != null ? (T) ctor.Invoke(null) : null;
        }

        /// <summary>
        /// Creates an exception instance using the standard 1 argument constructor with a message string.
        /// Returns null if not supported.
        /// </summary>
        /// <param name="message">The message text</param>
        /// <returns>The instance, or null if not supported</returns>
        protected static T CreateInstanceUsingStandardMessageConstructor(string message)
        {
            ConstructorInfo ctor = typeof(T).GetConstructor(new Type[] { typeof(string) });
            return ctor != null ? (T)ctor.Invoke(new object[] { message }) : null;
        }

        /// <summary>
        /// Creates an exception instance using the standard 2 argument constructor with a message string
        /// and inner exception.  Returns null if not supported.
        /// </summary>
        /// <param name="message">The message text</param>
        /// <param name="innerException">The inner exception</param>
        /// <returns>The instance, or null if not supported</returns>
        protected static T CreateInstanceUsingStandardMessageAndInnerExceptionConstructor(string message, Exception innerException)
        {
            ConstructorInfo ctor = typeof(T).GetConstructor(new Type[] { typeof(string), typeof(Exception) });
            return ctor != null ? (T)ctor.Invoke(new object[] { message, innerException }) : null;
        }
    }
}
