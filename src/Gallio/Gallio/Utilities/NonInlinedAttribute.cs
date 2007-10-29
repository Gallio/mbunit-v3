// Copyright 2007 MbUnit Project - http://www.mbunit.com/
// Portions Copyright 2000-2004 Jonathan De Halleux, Jamie Cansdale
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
using System.Diagnostics;
using System.Security;
using System.Security.Permissions;
using System.Text;

namespace Gallio.Utilities
{
    /// <summary>
    /// <para>
    /// This attribute prevents the compiler from inlining the associated method.
    /// </para>
    /// <para>
    /// When optimizations are enabled (as is typically the case in a Release build)
    /// simple method bodies may be inlined.  Normally that is harmless however the
    /// inlining operation causes the stack trace to be incomplete.  That may be
    /// significant when the <see cref="StackTrace" /> object is used to obtain
    /// a reference to the calling method, for example.
    /// </para>
    /// </summary>
    /// <todo author="jeff">
    /// There must be a better way to do this!  Like a CompilerServices attribute?
    /// </todo>
    [Serializable]
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Constructor, AllowMultiple = false, Inherited = false)]
    public class NonInlinedAttribute : CodeAccessSecurityAttribute
    {
        /// <summary>
        /// Indicates that the associated method must not be inlined.
        /// </summary>
        /// <param name="action">Must be <see cref="SecurityAction.Demand"/>.
        /// Unfortunately this is a limitation of the compiler's special
        /// support for securty attributes so we're leaking implementation details here.</param>
        public NonInlinedAttribute(SecurityAction action)
            : base(action)
        {
        }

        /// <inheritdoc />
        public override IPermission CreatePermission()
        {
            return new SecurityPermission(SecurityPermissionFlag.NoFlags);
        }
    }
}
