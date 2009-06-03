// Copyright 2005-2009 Gallio Project - http://www.gallio.org/
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
using Gallio.Common;

namespace Gallio.Common.Reflection.Impl
{
    /// <summary>
    /// A <see cref="StaticReflectionPolicy"/> wrapper.
    /// </summary>
    /// <remarks>
    /// <para>
    /// A wrapper holds an underlying reflection object.  Its behavior is
    /// derived from by primitive operations on the <see cref="Handle" /> defined by the
    /// particular <see cref="Policy"/> implementation that is in use.
    /// </para>
    /// </remarks>
    public abstract class StaticWrapper : IEquatable<StaticWrapper>
    {
        private Memoizer<int> hashCodeMemoizer = new Memoizer<int>();

        private readonly StaticReflectionPolicy policy;
        private readonly object handle;

        /// <summary>
        /// Creates a wrapper.
        /// </summary>
        /// <param name="policy">The reflection policy.</param>
        /// <param name="handle">The underlying reflection object.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="policy"/> or <paramref name="handle"/> is null.</exception>
        public StaticWrapper(StaticReflectionPolicy policy, object handle)
        {
            if (policy == null)
                throw new ArgumentNullException("policy");
            if (handle == null)
                throw new ArgumentNullException("handle");

            this.policy = policy;
            this.handle = handle;
        }

        /// <summary>
        /// Gets the reflection policy.
        /// </summary>
        public StaticReflectionPolicy Policy
        {
            get { return policy; }
        }

        /// <summary>
        /// Gets the underlying reflection object.
        /// </summary>
        public object Handle
        {
            get { return handle; }
        }

        /// <inhertdoc />
        public bool Equals(StaticWrapper other)
        {
            return EqualsByHandle(other);
        }

        /// <inhertdoc />
        public override bool Equals(object obj)
        {
            return Equals(obj as StaticWrapper);
        }

        /// <summary>
        /// Compares the policy and handle of this wrapper with those of another
        /// using <see cref="StaticReflectionPolicy.Equals" />.
        /// </summary>
        /// <param name="other">The other wrapper.</param>
        protected bool EqualsByHandle(StaticWrapper other)
        {
            return other != null
                && policy == other.policy
                && policy.Equals(this, other);
        }

        /// <inhertdoc />
        public override int GetHashCode()
        {
            return hashCodeMemoizer.Memoize(() => policy.GetHashCode(this));
        }
    }
}
