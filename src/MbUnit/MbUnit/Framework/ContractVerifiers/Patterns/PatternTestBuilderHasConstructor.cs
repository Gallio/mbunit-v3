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

namespace MbUnit.Framework.ContractVerifiers.Patterns
{
    /// <summary>
    /// Builder of pattern test for the contract verifiers.
    /// It generates a test that verifies that the target type has
    /// the specified constructor.
    /// </summary>
    public class PatternTestBuilderHasConstructor : PatternTestBuilder
    {
        private string constructorFriendlyName;
        private bool isPublic;
        private Type[] parameters;

        /// <summary>
        /// Constructs a pattern test builder.
        /// The resulting test verifies that the target type has
        /// the specified constructor.
        /// </summary>
        /// <param name="targetType">The target type.</param>
        /// <param name="constructorFriendlyName">A friendly name to designate the constructor.</param>
        /// <param name="isPublic">Determines whether the constructor must be public or not.</param>
        /// <param name="parameters">The types of the parameters taken by the constructor.
        /// Pass an empty array for searching a parameter-less constructor.</param>
        public PatternTestBuilderHasConstructor(Type targetType, 
            string constructorFriendlyName, bool isPublic, params Type[] parameters)
            : base(targetType)
        {
            if (constructorFriendlyName == null)
            {
                throw new ArgumentNullException("constructorName");
            }

            if (parameters == null)
            {
                throw new ArgumentNullException("parameters");
            }

            this.constructorFriendlyName = constructorFriendlyName;
            this.isPublic = isPublic;
            this.parameters = parameters;
        }

        /// <inheritdoc />
        protected override string Name
        {
            get
            {
                return "Has" + constructorFriendlyName + "Constructor";
            }
        }

        /// <inheritdoc />
        protected override void Run(PatternTestInstanceState state)
        {
            AssertionHelper.Verify(() =>
            {
                if (TargetType.GetConstructor(BindingFlags.Instance |
                    (isPublic ? BindingFlags.Public : BindingFlags.NonPublic),
                    null, parameters, null) != null)
                    return null;

                return new AssertionFailureBuilder("Expected the type to have a " + (isPublic ? String.Empty : "non-") + "public constructor.")
                    .AddRawLabeledValue("Type", TargetType)
                    .AddLabeledValue("Expected Signature", GetConstructorSignature(parameters))
                    .ToAssertionFailure();
            });
        }
    }
}
