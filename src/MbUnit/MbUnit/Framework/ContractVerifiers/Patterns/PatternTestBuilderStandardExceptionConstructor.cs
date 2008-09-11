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
using Gallio.Framework.Assertions;
using System.Reflection;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace MbUnit.Framework.ContractVerifiers.Patterns
{
    /// <summary>
    /// Builder of pattern test for the contract verifiers.
    /// It generates a test which verifies that the target exception type has
    /// the specified constructor. The constructor must initialize the exception 
    /// properties as expected (Message, InnerException) and optionnaly preserve
    /// those properties during a roundtrip serialization.
    /// </summary>
    public class PatternTestBuilderStandardExceptionConstructor : PatternTestBuilder
    {
        private string friendlyName;
        private bool checkForSerializationSupport;
        private Type[] parameters;
        private ExceptionConstructorSpec[] specs;

        /// <summary>
        /// Constructs a pattern test builder.
        /// It generates a test which verifies that the target exception type has
        /// the specified constructor. The constructor must initialize the exception 
        /// properties as expected (Message, InnerException) and optionnaly preserve
        /// those properties during a roundtrip serialization.
        /// </summary>
        /// <param name="targetType">The target type.</param>
        /// <param name="checkForSerializationSupport">Determines whether the
        /// exception properties are preserved during a serialization roundtrip.</param>
        /// <param name="friendlyName">A friendly name for the constructor.</param>
        /// <param name="parameters">The parameter types for the constructor</param>
        /// <param name="specs">Specifications for instantiating sample exception instances.</param>
        public PatternTestBuilderStandardExceptionConstructor(Type targetType,
            bool checkForSerializationSupport, string friendlyName, Type[] parameters,
            ExceptionConstructorSpec[] specs)
            : base(targetType)
        {
            this.friendlyName = friendlyName;
            this.checkForSerializationSupport = checkForSerializationSupport;
            this.parameters = parameters;
            this.specs = specs;
        }

        /// <inheritdoc />
        protected override string Name
        {
            get
            {
                return "Is" + friendlyName + "ConstructorWellDefined";
            }
        }

        /// <inheritdoc />
        protected override void Run(PatternTestInstanceState state)
        {
            ConstructorInfo ctor = GetConstructorInfo();

            foreach (ExceptionConstructorSpec spec in specs)
            {
                Exception instance = spec.GetInstance(ctor);

                Assert.Multiple(() =>
                {
                    AssertionHelper.Verify(() =>
                    {
                        if (Object.ReferenceEquals(spec.InnerException, instance.InnerException))
                            return null;

                        return new AssertionFailureBuilder("The inner exception should be referentially identical to the exception provided in the constructor.")
                            .AddRawLabeledValue("Exception Type", TargetType)
                            .AddRawLabeledValue("Actual Inner Exception", instance.InnerException)
                            .AddRawLabeledValue("Expected Inner Exception", spec.InnerException)
                            .ToAssertionFailure();
                    });

                    if (spec.Message == null)
                    {
                        AssertionHelper.Verify(() =>
                        {
                            if (instance.Message.Contains(TargetType.FullName))
                                return null;

                            return new AssertionFailureBuilder("The exception message should to contain the exception type name.")
                                .AddRawLabeledValue("Exception Type", TargetType)
                                .AddLabeledValue("Actual Message", instance.Message)
                                .ToAssertionFailure();
                        });
                    }
                    else
                    {
                        AssertionHelper.Verify(() =>
                        {
                            if (spec.Message == instance.Message)
                                return null;

                            return new AssertionFailureBuilder("Expected the exception message to be equal to a specific text.")
                                .AddRawLabeledValue("Exception Type", TargetType)
                                .AddLabeledValue("Actual Message", instance.Message)
                                .AddLabeledValue("Expected Message", spec.Message)
                                .ToAssertionFailure();
                        });
                    }

                    if (checkForSerializationSupport)
                        AssertMessageAndInnerExceptionPreservedByRoundTripSerialization(instance);
                });
            }
        }

        private ConstructorInfo GetConstructorInfo()
        {
            ConstructorInfo ctor = TargetType.GetConstructor(parameters);

            AssertionHelper.Verify(() =>
            {
                if (ctor != null)
                    return null;

                return new AssertionFailureBuilder("Expected the exception type to have a public constructor.")
                    .AddRawLabeledValue("Exception Type", TargetType)
                    .AddLabeledValue("Expected Signature", GetConstructorSignature(parameters))
                    .ToAssertionFailure();
            });

            return ctor;
        }

        /// <summary>
        /// Verifies that the <see cref="Exception.Message" /> and 
        /// <see cref="Exception.InnerException" />
        /// properties are preserved by round-trip serialization.
        /// </summary>
        /// <param name="instance">The exception instance.</param>
        private void AssertMessageAndInnerExceptionPreservedByRoundTripSerialization(Exception instance)
        {
            Exception result = RoundTripSerialize(instance);

            AssertionHelper.Verify(() =>
            {
                if (result.Message == instance.Message)
                    return null;

                return new AssertionFailureBuilder("Expected the exception message to be preserved by round-trip serialization.")
                    .AddRawLabeledValue("Exception Type", TargetType)
                    .AddLabeledValue("Expected Message", instance.Message)
                    .AddLabeledValue("Actual Message ", result.Message)
                    .ToAssertionFailure();
            });

            if (instance.InnerException == null)
            {
                AssertionHelper.Verify(() =>
                {
                    if (result.InnerException == null)
                        return null;

                    return new AssertionFailureBuilder("The inner exception should be preserved by round-trip serialization.")
                        .AddRawLabeledValue("Exception Type", TargetType)
                        .AddRawLabeledValue("Actual Inner Exception", instance.InnerException)
                        .AddRawLabeledValue("Expected Inner Exception", result.InnerException)
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
                        .AddRawLabeledValue("Exception Type", TargetType)
                        .AddRawLabeledValue("Actual Inner Exception", instance.InnerException)
                        .AddRawLabeledValue("Expected Inner Exception", result.InnerException)
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
