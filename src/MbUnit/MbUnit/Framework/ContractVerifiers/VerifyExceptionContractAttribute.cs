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
using MbUnit.Framework.ContractVerifiers.Patterns;

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
            : base("ExceptionContract", exceptionType)
        {
            this.ImplementsSerialization = true;
            this.ImplementsStandardConstructors = true;
        }

        /// <inheritdoc />
        protected override void Validate(PatternEvaluationScope scope, ICodeElementInfo codeElement)
        {
            base.Validate(scope, codeElement);

            if (TargetType == null)
                ThrowUsageErrorException("The specified exception type cannot be a null reference.");

            if (!TargetType.IsSubclassOf(typeof(Exception)))
                ThrowUsageErrorException("The specified exception type must derive from System.Exception.");
        }

        /// <inheritdoc />
        protected override IEnumerable<PatternTestBuilder> GetPatternTestBuilders()
        {
            if (ImplementsSerialization)
            {
                // Has Serializable attribute?
                yield return new PatternTestBuilderHasAttribute<SerializableAttribute>(
                    TargetType);
                
                // Has non-public serialization constructor?
                yield return new PatternTestBuilderHasConstructor(
                    TargetType, 
                    "Serialization", 
                    false, 
                    typeof(SerializationInfo), typeof(StreamingContext));
            }

            if (ImplementsStandardConstructors)
            {
                // Is public default constructor well defined?
                yield return new PatternTestBuilderStandardExceptionConstructor(
                    TargetType, 
                    ImplementsSerialization,
                    "Default",
                    EmptyArray<Type>.Instance,
                    new ExceptionConstructorSpec[] 
                    {
                        new ExceptionConstructorSpec()
                    });

                // Is public single parameter constructor (message) well defined?
                yield return new PatternTestBuilderStandardExceptionConstructor(
                    TargetType,
                    ImplementsSerialization,
                    "Message",
                    new Type[] { typeof(string) },
                    new ExceptionConstructorSpec[] 
                    {
                        new ExceptionConstructorSpec(null),
                        new ExceptionConstructorSpec(String.Empty),
                        new ExceptionConstructorSpec("A message")
                    });

                // Is public two parameters constructor (message and inner exception) well defined?
                yield return new PatternTestBuilderStandardExceptionConstructor(
                    TargetType,
                    ImplementsSerialization,
                    "MessageAndInnerException",
                    new Type[] { typeof(string), typeof(Exception) },
                    new ExceptionConstructorSpec[] 
                    {
                        new ExceptionConstructorSpec(null, null),
                        new ExceptionConstructorSpec(String.Empty, null),
                        new ExceptionConstructorSpec("A message", null),
                        new ExceptionConstructorSpec(null, new Exception()),
                        new ExceptionConstructorSpec(String.Empty, new Exception()),
                        new ExceptionConstructorSpec("A message", new Exception())
                    });
            }
        }
    }
}
