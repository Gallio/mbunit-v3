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

namespace MbUnit.Framework.ContractVerifiers.Patterns.ObjectHashCode
{
    /// <summary>
    /// Builder for the test pattern <see cref="ObjectHashCodePattern"/>
    /// </summary>
    internal class ObjectHashCodePatternBuilder : ContractVerifierPatternBuilder
    {
        private Type targetType;

        /// <summary>
        /// Sets the target evaluated type.
        /// </summary>
        /// <param name="targetType">The target evaluated type.</param>
        /// <returns>A reference to the builder itself.</returns>
        internal ObjectHashCodePatternBuilder SetTargetType(Type targetType)
        {
            if (targetType == null)
            {
                throw new ArgumentNullException("targetType");
            }

            this.targetType = targetType;
            return this;
        }

        /// <inheritdoc />
        public override ContractVerifierPattern ToPattern()
        {
            if (targetType == null)
            {
                throw new InvalidOperationException("The evaluated target type must be specified.");
            }

            return new ObjectHashCodePattern(
                new ObjectHashCodePatternSettings(targetType));
        }
    }
}