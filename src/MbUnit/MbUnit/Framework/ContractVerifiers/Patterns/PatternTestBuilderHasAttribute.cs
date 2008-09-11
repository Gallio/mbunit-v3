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

namespace MbUnit.Framework.ContractVerifiers.Patterns
{
    /// <summary>
    /// Builder of pattern test for the contract verifiers.
    /// It generates a test that verifies that the target type has
    /// the specified attribute.
    /// </summary>
    /// <typeparam name="T">The searched attibute type.</typeparam>
    public class PatternTestBuilderHasAttribute<T> : PatternTestBuilder where T : Attribute
    {
        /// <summary>
        /// Constructs a pattern test builder.
        /// The resulting test verifies that the target type has
        /// the specified attribute.
        /// </summary>
        /// <param name="targetType">The target type.</param>
        public PatternTestBuilderHasAttribute(Type targetType)
            : base(targetType)
        {
        }

        /// <inheritdoc />
        protected override string Name
        {
            get
            {
                return "Has" + typeof(T).Name;
            }
        }

        /// <inheritdoc />
        protected override void Run(PatternTestInstanceState state)
        {
            AssertionHelper.Verify(() =>
            {
                if (TargetType.IsDefined(typeof(T), false))
                    return null;

                return new AssertionFailureBuilder("Expected the exception type to be annotated by a particular attribute.")
                    .AddRawLabeledValue("Exception Type", TargetType)
                    .AddRawLabeledValue("Expected Attribute", typeof(T))
                    .ToAssertionFailure();
            });
        }
    }
}
