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
using Gallio;
using Gallio.Framework.Assertions;
using System.Reflection;

namespace MbUnit.Framework.ContractVerifiers.Patterns.MemberRecursion
{
    /// <summary>
    /// Data container which exposes necessary data required to
    /// run the test pattern <see cref="MemberRecursionPattern{TTarget, TMemberInfo}"/>.
    /// </summary>
    /// <typeparam name="TMemberInfo"></typeparam>
    internal class MemberRecursionPatternSettings<TMemberInfo>
        where TMemberInfo : MemberInfo
    {
        /// <summary>
        /// Gets a friendly name used to build the full pattern test name.
        /// </summary>
        public string Name
        {
            get;
            private set;
        }

        /// <summary>
        /// 
        /// </summary>
        public BindingFlags BindingFlags
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the assertion function to run recursively over each member.
        /// </summary>
        public Func<TMemberInfo, AssertionFailure> AssertionFunc
        {
            get;
            private set;
        }


        /// <summary>
        /// Constructs the data container which exposes necessary data required to
        /// run the test pattern <see cref="MemberRecursionPattern{TTarget, TMemberInfo}"/>.
        /// </summary>
        /// <param name="name">A friendly name used to build the full pattern test name.</param>
        /// <param name="bindingFlags"></param>
        /// <param name="assertionFunc">The assertion function to run recursively over each member.</param>
        public MemberRecursionPatternSettings(string name, BindingFlags bindingFlags,
            Func<TMemberInfo, AssertionFailure> assertionFunc)
        {
            if (name == null)
            {
                throw new ArgumentNullException("friendlyName");
            }

            if (assertionFunc == null)
            {
                throw new ArgumentNullException("assertionFunc");
            }

            this.Name = name;
            this.BindingFlags = bindingFlags;
            this.AssertionFunc = assertionFunc;
        }
    }
}
