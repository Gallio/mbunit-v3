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
using System.Collections;

namespace MbUnit.Framework.ContractVerifiers.Patterns
{
    /// <summary>
    /// Builder of pattern test for the contract verifiers.
    /// The generated test verifies that all the equal object instances
    /// located in the same equivalence class have the same hash code.
    /// </summary>
    public class PatternTestBuilderGetHashCode : PatternTestBuilder
    {
        /// <summary>
        /// Constructs a pattern test builder.
        /// The resulting test verifies that the target type has
        /// the specified attribute.
        /// </summary>
        /// <param name="targetType">The target type.</param>
        public PatternTestBuilderGetHashCode(Type targetType)
            : base(targetType)
        {
        }

        /// <inheritdoc />
        protected override string Name
        {
            get
            {
                return "ObjectGetHashCode";
            }
        }

        /// <inheritdoc />
        protected override void Run(PatternTestInstanceState state)
        {
            IEnumerator enumerator1 = GetEquivalentClasses(state.FixtureType, state.FixtureInstance).GetEnumerator();
            IEnumerator enumerator2 = GetEquivalentClasses(state.FixtureType, state.FixtureInstance).GetEnumerator();

            Assert.Multiple(() =>
            {
                while (enumerator1.MoveNext() && enumerator2.MoveNext())
                {
                    foreach (object x in (IEnumerable)enumerator1.Current)
                    foreach (object y in (IEnumerable)enumerator2.Current)
                    {
                        AssertionHelper.Verify(() =>
                        {
                            if (x.GetHashCode() == y.GetHashCode())
                                return null;

                            return new AssertionFailureBuilder("The hash codes returned by two equal instances together should be identical.")
                                .AddRawLabeledValue("First object instance", x)
                                .AddRawLabeledValue("First hash code", x.GetHashCode())
                                .AddRawLabeledValue("Second object instance", y)
                                .AddRawLabeledValue("Second hash code", y.GetHashCode())
                                .ToAssertionFailure();
                        });
                    }
                }
            });
        }

    }
}
