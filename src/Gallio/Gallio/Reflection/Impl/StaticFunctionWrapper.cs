// Copyright 2008 MbUnit Project - http://www.mbunit.com/
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
using System.Reflection;
using Gallio.Collections;
using Gallio.Utilities;
using System.Text;

namespace Gallio.Reflection.Impl
{
    /// <summary>
    /// A <see cref="StaticReflectionPolicy"/> function wrapper.
    /// </summary>
    public abstract class StaticFunctionWrapper : StaticMemberWrapper, IFunctionInfo
    {
        private readonly Memoizer<MethodAttributes> methodAttributesMemoizer = new Memoizer<MethodAttributes>();
        private readonly Memoizer<IList<IParameterInfo>> parametersMemoizer = new Memoizer<IList<IParameterInfo>>();

        /// <summary>
        /// Creates a wrapper.
        /// </summary>
        /// <param name="policy">The reflection policy</param>
        /// <param name="handle">The underlying reflection object</param>
        /// <param name="declaringType">The declaring type</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="policy"/>, <paramref name="handle"/>,
        /// or <paramref name="declaringType"/> is null</exception>
        public StaticFunctionWrapper(StaticReflectionPolicy policy, object handle, StaticDeclaredTypeWrapper declaringType)
            : base(policy, handle, declaringType)
        {
            if (declaringType == null)
                throw new ArgumentNullException("declaringType");
        }

        /// <inheritdoc />
        public MethodAttributes MethodAttributes
        {
            get
            {
                return methodAttributesMemoizer.Memoize(delegate
                {
                    return Policy.GetFunctionAttributes(this);
                });
            }
        }

        /// <inheritdoc />
        public bool IsAbstract
        {
            get { return (MethodAttributes & MethodAttributes.Abstract) != 0; }
        }

        /// <inheritdoc />
        public bool IsFinal
        {
            get { return (MethodAttributes & MethodAttributes.Final) != 0; }
        }

        /// <inheritdoc />
        public bool IsStatic
        {
            get { return (MethodAttributes & MethodAttributes.Static) != 0; }
        }

        /// <inheritdoc />
        public bool IsVirtual
        {
            get { return (MethodAttributes & MethodAttributes.Virtual) != 0; }
        }

        /// <inheritdoc />
        public bool IsAssembly
        {
            get { return (MethodAttributes & MethodAttributes.MemberAccessMask) == MethodAttributes.Assembly; }
        }

        /// <inheritdoc />
        public bool IsFamily
        {
            get { return (MethodAttributes & MethodAttributes.MemberAccessMask) == MethodAttributes.Family; }
        }

        /// <inheritdoc />
        public bool IsFamilyAndAssembly
        {
            get { return (MethodAttributes & MethodAttributes.MemberAccessMask) == MethodAttributes.FamANDAssem; }
        }

        /// <inheritdoc />
        public bool IsFamilyOrAssembly
        {
            get { return (MethodAttributes & MethodAttributes.MemberAccessMask) == MethodAttributes.FamORAssem; }
        }

        /// <inheritdoc />
        public bool IsPrivate
        {
            get { return (MethodAttributes & MethodAttributes.MemberAccessMask) == MethodAttributes.Private; }
        }

        /// <inheritdoc />
        public bool IsPublic
        {
            get { return (MethodAttributes & MethodAttributes.MemberAccessMask) == MethodAttributes.Public; }
        }

        /// <inheritdoc />
        public bool IsHideBySig
        {
            get { return (MethodAttributes & MethodAttributes.HideBySig) != 0; }
        }

        /// <inheritdoc />
        public IList<IParameterInfo> Parameters
        {
            get
            {
                return parametersMemoizer.Memoize(delegate
                {
                    return new CovariantList<StaticParameterWrapper, IParameterInfo>(Policy.GetFunctionParameters(this)); 
                });
            }
        }

        /// <inheritdoc />
        public bool Equals(IFunctionInfo other)
        {
            return Equals((object)other);
        }

        /// <summary>
        /// Implementation of <see cref="IFunctionInfo.Resolve" />
        /// </summary>
        protected abstract MethodBase ResolveMethodBase(bool throwOnError);

        /// <inheritdoc />
        protected override MemberInfo ResolveMemberInfo(bool throwOnError)
        {
            return ResolveMethodBase(throwOnError);
        }

        MethodBase IFunctionInfo.Resolve(bool throwOnError)
        {
            return ResolveMethodBase(throwOnError);
        }

        internal string ToString(ITypeInfo returnType, IList<ITypeInfo> genericArguments)
        {
            StringBuilder sig = new StringBuilder();

            sig.Append(GetTypeNameForSignature(returnType));
            sig.Append(' ');
            sig.Append(Name);
            AppendGenericArgumentListToSignature(sig, genericArguments);
            sig.Append('(');
            AppendParameterListToSignature(sig, Parameters);
            sig.Append(')');

            return sig.ToString();
        }
    }
}
