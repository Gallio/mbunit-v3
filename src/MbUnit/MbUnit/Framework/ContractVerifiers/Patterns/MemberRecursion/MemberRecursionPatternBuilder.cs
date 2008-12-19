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
using Gallio.Framework.Assertions;
using Gallio;
using System.Collections.Generic;

namespace MbUnit.Framework.ContractVerifiers.Patterns.MemberRecursion
{
    /// <summary>
    /// Builder for the test pattern <see cref="MemberRecursionPattern{TTarget, TMemberInfo}"/>
    /// </summary>
    /// <typeparam name="TTarget">The target type to test.</typeparam>
    /// <typeparam name="TMemberInfo">The member info type (FieldInfo, PropertyInfo, MethodInfo, etc.)</typeparam>
    internal class MemberRecursionPatternBuilder<TTarget, TMemberInfo> : ContractVerifierPatternBuilder
        where TMemberInfo : MemberInfo
    {
        private string name;
        private BindingFlags bindingFlags;
        private Func<TMemberInfo, AssertionFailure> assertionFunc;

        /// <summary>
        /// Sets the friendly name used to build the full test pattern name.
        /// </summary>
        /// <param name="name">A friendly name.</param>
        /// <returns>A reference to the builder itself.</returns>
        internal MemberRecursionPatternBuilder<TTarget, TMemberInfo> SetName(string name)
        {
            if (name == null)
            {
                throw new ArgumentNullException("name");
            }

            this.name = name;
            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="bindingFlags"></param>
        /// <returns></returns>
        internal MemberRecursionPatternBuilder<TTarget, TMemberInfo> SetBindingFlags(BindingFlags bindingFlags)
        {
            this.bindingFlags = bindingFlags;
            return this;
        }

        /// <summary>
        /// Sets the assertion function to run recursively over each member.
        /// </summary>
        /// <param name="assertionFunc">A function returning an assertion failure in case of
        /// a failing test; otherwise a null reference.</param>
        /// <returns>A reference to the builder itself.</returns>
        internal MemberRecursionPatternBuilder<TTarget, TMemberInfo> SetAssertionFunction(Func<TMemberInfo, AssertionFailure> assertionFunc)
        {
            if (assertionFunc == null)
            {
                throw new ArgumentNullException("assertionFunc");
            }

            this.assertionFunc = assertionFunc;
            return this;
        }

        /// <inheritdoc />
        public override ContractVerifierPattern ToPattern()
        {
            return new MemberRecursionPattern<TTarget, TMemberInfo>(
                new MemberRecursionPatternSettings<TMemberInfo>(name, bindingFlags, assertionFunc));
        }
    }
}