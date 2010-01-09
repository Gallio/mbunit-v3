// Copyright 2005-2010 Gallio Project - http://www.gallio.org/
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
using System.Globalization;
using System.Reflection;
using Gallio.Common.Collections;
using System.Security;

/*
 * This compilation unit contains MethodBase overrides that must be duplicated for each
 * of the unresolved reflection types because C# does not support multiple inheritance.
 */

#if DOTNET40
namespace Gallio.Common.Reflection.Impl.DotNet40
#else
namespace Gallio.Common.Reflection.Impl.DotNet20
#endif
{
    internal static class UnresolvedMethodBase
    {
        public static ParameterInfo[] ResolveParameters(IList<IParameterInfo> parameters)
        {
            return GenericCollectionUtils.ConvertAllToArray<IParameterInfo, ParameterInfo>(parameters,
                delegate(IParameterInfo parameter) { return parameter.Resolve(false); });
        }
    }

    internal partial class UnresolvedConstructorInfo
    {
        public override MethodAttributes Attributes
        {
            get { return adapter.MethodAttributes; }
        }

        public override CallingConventions CallingConvention
        {
            get { return CallingConventions.Any; }
        }

        public override bool ContainsGenericParameters
        {
            get { return false; }
        }

        public override bool IsGenericMethod
        {
            get { return false; }
        }

        public override bool IsGenericMethodDefinition
        {
            get { return false; }
        }

        public override RuntimeMethodHandle MethodHandle
        {
            get { throw new NotSupportedException("Cannot get method handle of unresolved constructor."); }
        }

        public override Type[] GetGenericArguments()
        {
            return Type.EmptyTypes;
        }

#if DOTNET40
        [SecuritySafeCritical]
#endif
        public override MethodBody GetMethodBody()
        {
            throw new NotSupportedException("Cannot get method body of unresolved constructor.");
        }

        public override MethodImplAttributes GetMethodImplementationFlags()
        {
            return MethodImplAttributes.ForwardRef;
        }

        public override ParameterInfo[] GetParameters()
        {
            return UnresolvedMethodBase.ResolveParameters(adapter.Parameters);
        }

        public override object Invoke(object obj, BindingFlags invokeAttr, Binder binder, object[] parameters,
            CultureInfo culture)
        {
            throw new NotSupportedException("Cannot invoke unresolved constructor.");
        }

        #region .Net 4.0 Only
#if DOTNET40
        public override bool IsSecurityCritical
        {
            get { return false; }
        }

        public override bool IsSecuritySafeCritical
        {
            get { return false; }
        }

        public override bool IsSecurityTransparent
        {
            get { return false; }
        }
#endif
        #endregion
    }

    internal partial class UnresolvedMethodInfo
    {
        public override MethodAttributes Attributes
        {
            get { return adapter.MethodAttributes; }
        }

        public override CallingConventions CallingConvention
        {
            get { return CallingConventions.Any; }
        }

        public override bool ContainsGenericParameters
        {
            get { return adapter.ContainsGenericParameters; }
        }

        public override bool IsGenericMethod
        {
            get { return adapter.IsGenericMethod; }
        }

        public override bool IsGenericMethodDefinition
        {
            get { return adapter.IsGenericMethodDefinition; }
        }

        public override RuntimeMethodHandle MethodHandle
        {
            get { throw new NotSupportedException("Cannot get method handle of unresolved method."); }
        }

        public override Type[] GetGenericArguments()
        {
            return GenericCollectionUtils.ConvertAllToArray<ITypeInfo, Type>(adapter.GenericArguments,
                delegate(ITypeInfo parameter) { return parameter.Resolve(false); });
        }

#if DOTNET40
        [SecuritySafeCritical]
#endif
        public override MethodBody GetMethodBody()
        {
            throw new NotSupportedException("Cannot get method body of unresolved method.");
        }

        public override MethodImplAttributes GetMethodImplementationFlags()
        {
            return MethodImplAttributes.ForwardRef;
        }

        public override ParameterInfo[] GetParameters()
        {
            return UnresolvedMethodBase.ResolveParameters(adapter.Parameters);
        }

        public override object Invoke(object obj, BindingFlags invokeAttr, Binder binder, object[] parameters,
            CultureInfo culture)
        {
            throw new NotSupportedException("Cannot invoke unresolved method.");
        }

        #region .Net 4.0 Only
#if DOTNET40
        public override bool IsSecurityCritical
        {
            get { return false; }
        }

        public override bool IsSecuritySafeCritical
        {
            get { return false; }
        }

        public override bool IsSecurityTransparent
        {
            get { return false; }
        }
#endif
        #endregion
    }
}