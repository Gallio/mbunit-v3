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

namespace MbUnit.Framework.ContractVerifiers.Patterns.HasAttribute
{
    /// <summary>
    /// Builder for the test pattern <see cref="HasAttributePattern{TTarget, TAttribute}"/>
    /// </summary>
    /// <typeparam name="TTarget">The target type to test.</typeparam>
    /// <typeparam name="TAttribute">The expected attribute type to find.</typeparam>
    internal class HasAttributePatternBuilder<TTarget, TAttribute> : ContractVerifierPatternBuilder
        where TTarget : class
        where TAttribute : Attribute
    {
        /// <inheritdoc />
        public override ContractVerifierPattern ToPattern()
        {
            return new HasAttributePattern<TTarget, TAttribute>(
                new HasAttributePatternSettings());
        }
    }
}