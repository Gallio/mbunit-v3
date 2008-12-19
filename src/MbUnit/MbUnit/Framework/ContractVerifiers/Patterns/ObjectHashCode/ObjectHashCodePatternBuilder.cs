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
using System.Reflection;

namespace MbUnit.Framework.ContractVerifiers.Patterns.ObjectHashCode
{
    /// <summary>
    /// Builder for the test pattern <see cref="ObjectHashCodePattern{T}"/>
    /// </summary>
    /// <typeparam name="TTarget">The target equatable type.</typeparam>
    internal class ObjectHashCodePatternBuilder<TTarget> : ContractVerifierPatternBuilder
        where TTarget : IEquatable<TTarget>
    {
        private PropertyInfo equivalenceClassSource;

        /// <summary>
        /// Sets the source of equivalence classes.
        /// </summary>
        /// <param name="equivalenceClassSource">Information about the contract verifier
        /// property providing a collection of equivalence classes.</param>
        /// <returns>A reference to the builder itself.</returns>
        internal ObjectHashCodePatternBuilder<TTarget> SetEquivalenceClassSource(PropertyInfo equivalenceClassSource)
        {
            if (equivalenceClassSource == null)
            {
                throw new ArgumentNullException("source");
            }

            this.equivalenceClassSource = equivalenceClassSource;
            return this;
        }

        /// <inheritdoc />
        public override ContractVerifierPattern ToPattern()
        {
            if (equivalenceClassSource == null)
            {
                throw new InvalidOperationException("The source of equivalence classes must be specified.");
            }

            return new ObjectHashCodePattern<TTarget>(
                new ObjectHashCodePatternSettings(equivalenceClassSource));
        }
    }
}