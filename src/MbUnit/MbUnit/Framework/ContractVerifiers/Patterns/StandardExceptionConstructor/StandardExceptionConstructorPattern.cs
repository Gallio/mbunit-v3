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

namespace MbUnit.Framework.ContractVerifiers.Patterns.StandardExceptionConstructor
{
    /// <summary>
    /// Test pattern for contract verifiers related to exceptions.
    /// It verifies that the target exception type has the specified standard constructor,
    /// and that it behaves as expected.
    /// </summary>
    /// <typeparam name="TException">The target custom exception type.</typeparam>
    internal class StandardExceptionConstructorPattern<TException> : ContractVerifierPattern
        where TException : Exception
    {
        private StandardExceptionConstructorPatternSettings settings;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="settings">Settings.</param>
        internal StandardExceptionConstructorPattern(StandardExceptionConstructorPatternSettings settings)
        {
            if (settings == null)
            {
                throw new ArgumentNullException("settings");
            }

            this.settings = settings;
        }

        /// <inheritdoc />
        protected override string Name
        {
            get
            {
                return settings.FriendlyName;
            }
        }

        /// <inheritdoc />
        protected internal override void Run(IContractVerifierPatternInstanceState state)
        {
            ConstructorInfo ctor = GetConstructorInfo();

            foreach (var spec in settings.ConstructorSpecifications)
            {
                Exception instance = spec.GetInstance(ctor);

                Assert.Multiple(() =>
                {
                    AssertionHelper.Verify(() =>
                    {
                        if (Object.ReferenceEquals(spec.InnerException, instance.InnerException))
                            return null;

                        return new AssertionFailureBuilder("The inner exception should be referentially identical to the exception provided in the constructor.")
                            .AddRawLabeledValue("Exception Type", typeof(TException))
                            .AddRawLabeledValue("Actual Inner Exception", instance.InnerException)
                            .AddRawLabeledValue("Expected Inner Exception", spec.InnerException)
                            .ToAssertionFailure();
                    });

                    if (spec.Message == null)
                    {
                        AssertionHelper.Verify(() =>
                        {
                            if (instance.Message.Contains(typeof(TException).FullName))
                                return null;

                            return new AssertionFailureBuilder("The exception message should to contain the exception type name.")
                                .AddRawLabeledValue("Exception Type", typeof(TException))
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
                                .AddRawLabeledValue("Exception Type", typeof(TException))
                                .AddLabeledValue("Actual Message", instance.Message)
                                .AddLabeledValue("Expected Message", spec.Message)
                                .ToAssertionFailure();
                        });
                    }

                    if (settings.CheckForSerializationSupport)
                    {
                        AssertMessageAndInnerExceptionPreservedByRoundTripSerialization(instance);
                    }
                });
            }
        }

        private ConstructorInfo GetConstructorInfo()
        {
            Type[] types = new List<Type>(settings.ParameterTypes).ToArray();
            ConstructorInfo ctor = typeof(TException).GetConstructor(types);

            AssertionHelper.Verify(() =>
            {
                if (ctor != null)
                    return null;

                return new AssertionFailureBuilder("Expected the exception type to have a public constructor.")
                    .AddRawLabeledValue("Exception Type", typeof(TException))
                    .AddLabeledValue("Expected Signature", GetConstructorSignature(settings.ParameterTypes))
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
                    .AddRawLabeledValue("Exception Type", typeof(TException))
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
                        .AddRawLabeledValue("Exception Type", typeof(TException))
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
                        .AddRawLabeledValue("Exception Type", typeof(TException))
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
            using (var stream = new MemoryStream())
            {
                var formatter = new BinaryFormatter();
                formatter.Serialize(stream, instance);
                stream.Position = 0;
                return (Exception)formatter.Deserialize(stream);
            }
        }
    }
}
