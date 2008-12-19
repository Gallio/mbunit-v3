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
using System.Runtime.Serialization;
using System.Text;
using Gallio.Framework.Assertions;
using Gallio.Framework.Data;
using Gallio.Framework.Pattern;
using Gallio.Model;
using Gallio.Reflection;
using MbUnit.Framework.ContractVerifiers.Patterns;
using MbUnit.Framework.ContractVerifiers.Patterns.HasAttribute;
using MbUnit.Framework.ContractVerifiers.Patterns.HasConstructor;
using MbUnit.Framework.ContractVerifiers.Patterns.StandardExceptionConstructor;

namespace MbUnit.Framework.ContractVerifiers
{
    /// <summary>
    /// <para>
    /// Field-based contract verifier for the implementation of custom exception.
    /// </para>
    /// <para>
    /// Built-in verifications:
    /// <list type="bullet">
    /// <item>
    /// <name>HasSerializableAttribute</name>
    /// <description>The exception type has the <see cref="SerializableAttribute" /> attribute.
    /// Disable that test by settings the <see cref="VerifyExceptionContract{T}.ImplementsSerialization"/>
    /// property to <c>false</c>.</description>
    /// </item>
    /// <item>
    /// <name>HasSerializationConstructor</name>
    /// <description>The exception type has a protected serialization constructor similar to
    /// <see cref="Exception(SerializationInfo, StreamingContext)" />. Disable that test 
    /// by settings the <see cref="VerifyExceptionContract{T}.ImplementsSerialization"/>
    /// property to <c>false</c>.</description>
    /// </item>
    /// <item>
    /// <name>IsDefaultConstructorWellDefined</name>
    /// <description>The exception type has a default parameter-less constructor. When
    /// the <see cref="VerifyExceptionContract{T}.ImplementsSerialization"/> property
    /// is set to <c>true</c> as well, the method verifies that the properties of 
    /// the exception are preserved during a roundtrip serialization. Disable the test 
    /// by settings the <see cref="VerifyExceptionContract{T}.ImplementsStandardConstructors"/>
    /// property to <c>false</c>. </description>
    /// </item>
    /// <item>
    /// <name>IsMessageConstructorWellDefined</name>
    /// <description>The exception type has single argument constructor for the message. When
    /// the <see cref="VerifyExceptionContract{T}.ImplementsSerialization"/> property
    /// is set to <c>true</c> as well, the method verifies that the properties of 
    /// the exception are preserved during a roundtrip serialization. Disable the test 
    /// by settings the <see cref="VerifyExceptionContract{T}.ImplementsStandardConstructors"/>
    /// property to <c>false</c>. </description>
    /// </item>
    /// <item>
    /// <name>IsMessageAndInnerExceptionConstructorWellDefined</name>
    /// <description>The exception type has two parameters constructor for the message and an inner exception. 
    /// When the <see cref="VerifyExceptionContract{T}.ImplementsSerialization"/> property
    /// is set to <c>true</c> as well, the method verifies that the properties of 
    /// the exception are preserved during a roundtrip serialization. Disable the test 
    /// by settings the <see cref="VerifyExceptionContract{T}.ImplementsStandardConstructors"/>
    /// property to <c>false</c>. </description>
    /// </item>
    /// </list>
    /// </para>
    /// <example>
    /// The following example shows a simple custom exception class with some 
    /// basic serialization support, and a test fixture which uses the
    /// exception contract verifier to test it.
    /// <code><![CDATA[
    /// [Serializable]
    /// public class SampleException : Exception, ISerializable
    /// {
    ///     public SampleException()
    ///     {
    ///     }
    /// 
    ///     public SampleException(string message)
    ///         : base(message)
    ///     {
    ///     }
    /// 
    ///     public SampleException(string message, Exception innerException)
    ///         : base(message, innerException)
    ///     {
    ///     }
    /// 
    ///     protected SampleException(SerializationInfo info, StreamingContext context)
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
    /// public class SampleExceptionTest
    /// {
    ///     [ContractVerifier]
    ///     public readonly IContractVerifier ExceptionTests = new ExceptionContractVerifier<SampleException>()
    ///     {
    ///         ImplementsSerialization = true, // Optional (default is true)
    ///         ImplementsStandardConstructors = true // Optional (default is true)
    ///     };
    /// }
    /// ]]></code>
    /// </example>
    /// </summary>
    /// <typeparam name="TException">The target custom exception type.</typeparam>
    public class VerifyExceptionContract<TException> : AbstractContractVerifier
        where TException : Exception
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        public VerifyExceptionContract()
        {
            this.ImplementsSerialization = true;
            this.ImplementsStandardConstructors = true;
        }

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
        /// <item>The exception has a default parameter-less constructor.</item>
        /// <item>The exception has a single parameter constructor for the message.</item>
        /// <item>The exception two parameters constructor for the message and an inner exception.</item>
        /// </list>
        /// </para>
        /// </summary>
        public bool ImplementsStandardConstructors
        {
            get;
            set;
        }

        /// <inheritdoc />
        public override IEnumerable<ContractVerifierPattern> GetContractPatterns()
        {
            if (ImplementsSerialization)
            {
                // Has Serializable attribute?
                yield return new HasAttributePatternBuilder<TException, SerializableAttribute>()
                    .ToPattern();

                // Has non-public serialization constructor?
                yield return new HasConstructorPatternBuilder<TException>()
                    .SetName("Serialization")
                    .SetParameterTypes(typeof(SerializationInfo), typeof(StreamingContext))
                    .SetAccessibility(HasConstructorAccessibility.NonPublic)
                    .ToPattern();
            }

            if (ImplementsStandardConstructors)
            {
                // Is public default constructor well defined?
                yield return new StandardExceptionConstructorPatternBuilder<TException>()
                    .SetFriendlyName("Default")
                    .SetCheckForSerializationSupport(ImplementsSerialization)
                    .SetConstructorSpecifications(new ExceptionConstructorSpec())
                    .ToPattern();

                // Is public single parameter constructor (message) well defined?
                yield return new StandardExceptionConstructorPatternBuilder<TException>()
                    .SetFriendlyName("Message")
                    .SetCheckForSerializationSupport(ImplementsSerialization)
                    .SetParameterTypes(typeof(string))
                    .SetConstructorSpecifications(
                        new ExceptionConstructorSpec(null),
                        new ExceptionConstructorSpec(String.Empty),
                        new ExceptionConstructorSpec("A message"))
                    .ToPattern();

                // Is public two parameters constructor (message and inner exception) well defined?
                yield return new StandardExceptionConstructorPatternBuilder<TException>()
                    .SetFriendlyName("MessageAndInnerException")
                    .SetCheckForSerializationSupport(ImplementsSerialization)
                    .SetParameterTypes(typeof(string), typeof(Exception))
                    .SetConstructorSpecifications(
                        new ExceptionConstructorSpec(null, null),
                        new ExceptionConstructorSpec(String.Empty, null),
                        new ExceptionConstructorSpec("A message", null),
                        new ExceptionConstructorSpec(null, new Exception()),
                        new ExceptionConstructorSpec(String.Empty, new Exception()),
                        new ExceptionConstructorSpec("A message", new Exception()))
                    .ToPattern();
            }
        }
    }
}
