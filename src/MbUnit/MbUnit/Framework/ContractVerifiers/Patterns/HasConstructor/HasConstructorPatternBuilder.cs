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

namespace MbUnit.Framework.ContractVerifiers.Patterns.HasConstructor
{
    /// <summary>
    /// Builder for the test pattern <see cref="HasConstructorPattern{TTarget}"/>
    /// </summary>
    /// <typeparam name="TTarget"></typeparam>
    internal class HasConstructorPatternBuilder<TTarget> : ContractVerifierPatternBuilder
    {
        private HasConstructorAccessibility accessibility = HasConstructorAccessibility.Public;
        private string name = String.Empty;
        private List<Type> parameterTypes = new List<Type>();

        /// <summary>
        /// Sets the accessibility of the searched constructor.
        /// If not specified, the default value is <see cref="HasConstructorAccessibility.Public"/>
        /// </summary>
        /// <param name="accessibility">The accessibility of searched constructor.</param>
        /// <returns>A reference to the builder itself.</returns>
        internal HasConstructorPatternBuilder<TTarget> SetAccessibility(HasConstructorAccessibility accessibility)
        {
            this.accessibility = accessibility;
            return this;
        }

        /// <summary>
        /// Sets the friendly name used to build the full test pattern name.
        /// </summary>
        /// <param name="name">A friendly name.</param>
        /// <returns>A reference to the builder itself.</returns>
        internal HasConstructorPatternBuilder<TTarget> SetName(string name)
        {
            if (name == null)
            {
                throw new ArgumentNullException("friendlyName");
            }

            this.name = name;
            return this;
        }

        /// <summary>
        /// Sets the types of the constructor parameters.
        /// </summary>
        /// <param name="parameterTypes">An array of types definining the types
        /// of the constructor parameters.</param>
        /// <returns>A reference to the builder itself.</returns>
        internal HasConstructorPatternBuilder<TTarget> SetParameterTypes(params Type[] parameterTypes)
        {
            if (parameterTypes == null)
            {
                throw new ArgumentNullException("parameterTypes");
            }

            this.parameterTypes = new List<Type>(parameterTypes);
            return this;
        }

        /// <inheritdoc />
        public override ContractVerifierPattern ToPattern()
        {
            return new HasConstructorPattern<TTarget>(
                new HasConstructorPatternSettings(accessibility, name, parameterTypes));
        }
    }
}