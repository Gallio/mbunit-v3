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
using System.Reflection;

namespace Gallio.Common.Reflection.Impl
{
    /// <summary>
    /// Represents a <see cref="MethodInfo" /> whose native definition could not be resolved
    /// so we fall back on the <see cref="IMethodInfo"/> wrapper.
    /// </summary>
    public sealed partial class UnresolvedMethodInfo : MethodInfo, IUnresolvedCodeElement
    {
        private readonly IMethodInfo adapter;

        /// <summary>
        /// Creates a reflection object backed by the specified adapter.
        /// </summary>
        /// <param name="adapter">The adapter</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="adapter"/> is null</exception>
        public UnresolvedMethodInfo(IMethodInfo adapter)
        {
            if (adapter == null)
                throw new ArgumentNullException("adapter");

            this.adapter = adapter;
        }

        /// <summary>
        /// Gets the underlying reflection adapter.
        /// </summary>
        public IMethodInfo Adapter
        {
            get { return adapter; }
        }

        ICodeElementInfo IUnresolvedCodeElement.Adapter
        {
            get { return adapter; }
        }

        /// <inheritdoc />
        public override MemberTypes MemberType
        {
            get { return MemberTypes.Method; }
        }

        /// <inheritdoc />
        public override ParameterInfo ReturnParameter
        {
            get { return adapter.ReturnParameter.Resolve(false); }
        }

        /// <inheritdoc />
        public override Type ReturnType
        {
            get { return adapter.ReturnType.Resolve(false); }
        }

        /// <inheritdoc />
        public override ICustomAttributeProvider ReturnTypeCustomAttributes
        {
            get { return ReturnParameter; }
        }

        /// <inheritdoc />
        public override MethodInfo GetBaseDefinition()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public override MethodInfo GetGenericMethodDefinition()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public override MethodInfo MakeGenericMethod(params Type[] typeArguments)
        {
            throw new NotImplementedException();
        }
    }
}