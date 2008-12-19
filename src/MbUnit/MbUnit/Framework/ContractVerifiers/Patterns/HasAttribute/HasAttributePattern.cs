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

namespace MbUnit.Framework.ContractVerifiers.Patterns.HasAttribute
{
    /// <summary>
    /// General purpose test pattern for contract verifiers.
    /// It verifies that the target evaluated type has the specified attribute.
    /// </summary>
    /// <typeparam name="TTarget">The target type to test.</typeparam>
    /// <typeparam name="TAttribute">The expected attribute type to find.</typeparam>
    internal class HasAttributePattern<TTarget, TAttribute> : ContractVerifierPattern
        where TTarget : class
        where TAttribute : Attribute
    {
        private HasAttributePatternSettings settings;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="settings">Settings.</param>
        internal HasAttributePattern(HasAttributePatternSettings settings)
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
                return "Has" + typeof(TAttribute).Name;
            }
        }

        /// <inheritdoc />
        protected internal override void Run(IContractVerifierPatternInstanceState state)
        {
            AssertionHelper.Verify(() =>
            {
                if (typeof(TTarget).IsDefined(typeof(TAttribute), false))
                    return null;

                return new AssertionFailureBuilder("Expected the exception type to be annotated by a particular attribute.")
                    .AddRawLabeledValue("Exception Type", typeof(TTarget))
                    .AddRawLabeledValue("Expected Attribute", typeof(TAttribute))
                    .ToAssertionFailure();
            });
        }
    }
}
