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
using MbUnit.Framework.ContractVerifiers.Patterns.MemberRecursion;

namespace MbUnit.Framework.ContractVerifiers
{
    /// <summary>
    /// <para>
    /// Field-based contract verifier for type immutability.
    /// </para>
    /// <para>
    /// Built-in verifications:
    /// <list type="bullet">
    /// <item>
    /// <term>AreReadOnlyFields</term>
    /// <description>All the public and non-public instance fields are marked
    /// as read only. The evaluation is made recursively on the field types too.</description>
    /// </item>
    /// <item>
    /// <term>HasNoPublicPropertySetter</term>
    /// <description>The type does not have any public property setter.
    /// The evaluation is made recursively on the property types too.</description>
    /// </item>
    /// </list>
    /// </para>
    /// <example>
    /// The following example shows a simple immutable class with all the
    /// instance fields marked as read only, and a test fixture which invokes the
    /// immutability contract verifier to test the class.
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
    ///     [ContractVerifier]
    ///     public readonly IContractVerifier ImmutabilityTests = new ImmutabilityContractVerifier<SampleImmutable>();
    /// }
    /// ]]></code>
    /// </example>
    /// </summary>
    /// <typeparam name="TTarget">The target immutable type.</typeparam>
    public class VerifyImmutabilityContract<TTarget> : AbstractContractVerifier
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        public VerifyImmutabilityContract()
        {
        }

        /// <inheritdoc />
        public override IEnumerable<ContractVerifierPattern> GetContractPatterns()
        {
            // Are all child fields marked as read only?
            yield return new MemberRecursionPatternBuilder<TTarget, FieldInfo>()
                .SetName("AreReadOnlyFields")
                .SetBindingFlags(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)
                .SetAssertionFunction(fieldInfo =>
                {
                    if (fieldInfo.IsInitOnly)
                        return null;

                    return new AssertionFailureBuilder("Expected the field to be marked as read only.")
                        .AddRawLabeledValue("Declaring Type", fieldInfo.DeclaringType)
                        .AddRawLabeledValue("Field Type", fieldInfo.FieldType)
                        .AddRawLabeledValue("Field Name", fieldInfo.Name)
                        .ToAssertionFailure();
                })
                .ToPattern();
        }
    }
}



