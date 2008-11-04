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

namespace MbUnit.Framework.ContractVerifiers.Patterns.Equality
{
    /// <summary>
    /// Builder for the test pattern <see cref="EqualityPattern"/>
    /// </summary>
    internal class EqualityPatternBuilder : ContractVerifierPatternBuilder
    {
        private Type targetType;
        private MethodInfo equalityMethodInfo;
        private string signatureDescription;
        private bool inequality;
        private string name;

        /// <summary>
        /// Sets the target evaluated type.
        /// </summary>
        /// <param name="targetType">The target evaluated type.</param>
        /// <returns>A reference to the builder itself.</returns>
        internal EqualityPatternBuilder SetTargetType(Type targetType)
        {
            if (targetType == null)
            {
                throw new ArgumentNullException("targetType");
            }

            this.targetType = targetType;
            return this;
        }
       
        /// <summary>
        /// Sets the reflection descriptor for the equality method.
        /// </summary>
        /// <param name="equalityMethodInfo">The reflection descriptor
        /// for the equality method.</param>
        /// <returns>A reference to the builder itself.</returns>
        internal EqualityPatternBuilder SetEqualityMethodInfo(MethodInfo equalityMethodInfo)
        {
            this.equalityMethodInfo = equalityMethodInfo;
            return this;
        }

        /// <summary>
        /// Sets a friendly displayable text explaining the signature of 
        /// the equality method.
        /// </summary>
        /// <param name="signatureDescription">A friendly signature text.</param>
        /// <returns>A reference to the builder itself.</returns>
        internal EqualityPatternBuilder SetSignatureDescription(string signatureDescription)
        {
            if (signatureDescription == null)
            {
                throw new ArgumentNullException("signatureDescription");
            }

            this.signatureDescription = signatureDescription;
            return this;
        }

        /// <summary>
        /// Sets a friendly name for the test pattern
        /// </summary>
        /// <param name="name">A friendly signature text.</param>
        /// <returns>A reference to the builder itself.</returns>
        internal EqualityPatternBuilder SetName(string name)
        {
            if (name == null)
            {
                throw new ArgumentNullException("name");
            }

            this.name = name;
            return this;
        }

        /// <summary>
        /// Indicates whether the method represents an inequality operation.
        /// If not specified, the default value is false.
        /// </summary>
        /// <param name="inequality">True if the method represents an inequality operation; otherwise, false.</param>
        /// <returns>A reference to the builder itself.</returns>
        internal EqualityPatternBuilder SetInequality(bool inequality)
        {
            this.inequality = inequality;
            return this;
        }

        /// <inheritdoc />
        public override ContractVerifierPattern ToPattern()
        {
            if (targetType == null)
            {
                throw new InvalidOperationException("The evaluated target type must be specified.");
            }

            if (name == null)
            {
                throw new InvalidOperationException("A friendly name for the test pattern must be specified.");
            }

            if (signatureDescription == null)
            {
                throw new InvalidOperationException("A signature description must be specified.");
            }

            return new EqualityPattern(new EqualityPatternSettings(
                targetType, equalityMethodInfo, signatureDescription, inequality, name));
        }
    }
}