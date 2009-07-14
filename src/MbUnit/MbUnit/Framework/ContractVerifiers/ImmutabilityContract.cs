// Copyright 2005-2009 Gallio Project - http://www.gallio.org/
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
using Gallio.Common.Collections;
using Gallio.Framework.Assertions;
using MbUnit.Framework.ContractVerifiers.Core;
using System.Collections.ObjectModel;

namespace MbUnit.Framework.ContractVerifiers
{
    /// <summary>
    /// Contract for verifying the implementation of an immutable type.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Built-in verifications:
    /// <list type="bullet">
    /// <item>
    /// <strong>AreReadOnlyFields</strong> : All the public and non-public instance fields are marked
    /// as read only. The evaluation is made recursively on the field types too.
    /// </item>
    /// <item>
    /// <strong>HasNoPublicPropertySetter</strong> : The type does not have any public property setter. 
    /// The evaluation is made recursively on the property types too.
    /// </item>
    /// </list>
    /// </para>
    /// </remarks>
    /// <example>
    /// The following example shows a simple immutable class with all the
    /// instance fields marked as read only, and a test fixture which invokes the
    /// immutability contract to test the class.
    /// <code><![CDATA[
    /// public class SampleImmutable
    /// {
    ///     private readonly int number;
    ///     private readonly string text;
    /// 
    ///     public SampleImmutable(int number, string text)
    ///     {
    ///         this.number = number;
    ///         this.text = text;
    ///     }
    /// }
    ///
    /// public class SampleImmutableTest
    /// {
    ///     [VerifyContract]
    ///     public readonly IContract ImmutabilityTests = new ImmutabilityContract<SampleImmutable>();
    /// }
    /// ]]></code>
    /// </example>
    /// <typeparam name="TTarget">The target immutable type.</typeparam>
    /// <seealso cref="VerifyContractAttribute"/>
    public class ImmutabilityContract<TTarget> : AbstractContract
    {
        private const BindingFlags FieldBindingFlags = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public;

        /// <summary>
        /// Default constructor.
        /// </summary>
        public ImmutabilityContract()
        {
        }

        /// <inheritdoc />
        protected override IEnumerable<Test> GetContractVerificationTests()
        {
            // Are all child fields marked as read only?
            yield return CreateImmutabilityTest("AreAllFieldsReadOnly");
        }

        private Test CreateImmutabilityTest(string name)
        {
            return new TestCase(name, () =>
            {
                var visitedTypes = new ImmutableTypeCollection();
                Assert.Multiple(() => VerifyMemberTypes(typeof(TTarget), visitedTypes, Context));
            });
        }

        private static void VerifyMemberTypes(Type type, ICollection<Type> visitedTypes, ContractVerificationContext context)
        {
            if (!visitedTypes.Contains(type))
            {
                visitedTypes.Add(type);

                foreach (var fieldInfo in type.GetFields(FieldBindingFlags))
                {
                    AssertionHelper.Explain(() =>
                        Assert.IsTrue(fieldInfo.IsInitOnly),
                        innerFailures => new AssertionFailureBuilder("Expected the field to be marked as read only since it is part of an immutable type.")
                            .AddRawLabeledValue("Declaring Type", fieldInfo.DeclaringType)
                            .AddRawLabeledValue("Field Type", fieldInfo.FieldType)
                            .AddRawLabeledValue("Field Name", fieldInfo.Name)
                            .SetStackTrace(context.GetStackTraceData())
                            .AddInnerFailures(innerFailures)
                            .ToAssertionFailure());

                    VerifyMemberTypes(fieldInfo.FieldType, visitedTypes, context);
                }
            }
        }
    }
}



