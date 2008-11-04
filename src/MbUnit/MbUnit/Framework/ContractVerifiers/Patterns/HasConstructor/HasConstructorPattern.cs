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

namespace MbUnit.Framework.ContractVerifiers.Patterns.HasConstructor
{
    /// <summary>
    /// General purpose test pattern for contract verifiers.
    /// It verifies that the target evaluated type has the specified attribute.
    /// </summary>
    internal class HasConstructorPattern : ContractVerifierPattern
    {
        private HasConstructorPatternSettings settings;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="settings">Settings.</param>
        internal HasConstructorPattern(HasConstructorPatternSettings settings)
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
                return settings.Name;
            }
        }

        /// <inheritdoc />
        protected internal override void Run(IContractVerifierPatternInstanceState state)
        {
            AssertionHelper.Verify(() =>
            {
                if (settings.TargetType.GetConstructor(BindingFlags.Instance | 
                    settings.Accessibility.BindingFlags, null, 
                    new List<Type>(settings.ParameterTypes).ToArray(), null) != null)
                    return null;

                return new AssertionFailureBuilder("Expected the type to have a " + settings.Accessibility.Name + " constructor.")
                    .AddRawLabeledValue("Type", settings.TargetType)
                    .AddLabeledValue("Expected Signature", GetConstructorSignature())
                    .ToAssertionFailure();
            });
        }

        private string GetConstructorSignature()
        {
            StringBuilder output = new StringBuilder(".ctor(");
            bool first = true;

            foreach (var type in settings.ParameterTypes)
            {
                if (!first)
                {
                    output.Append(", ");
                }

                output.Append(type.Name);
                first = false;
            }

            return output.Append(")").ToString();
        }
    }
}
