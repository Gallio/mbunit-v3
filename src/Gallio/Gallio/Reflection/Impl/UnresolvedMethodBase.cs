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
using System.Globalization;
using System.Reflection;
using Gallio.Collections;

/*
 * This compilation unit contains MethodBase overrides that must be duplicated for each
 * of the unresolved reflection types because C# does not support multiple inheritance.
 */

namespace Gallio.Reflection.Impl
{
    internal static class UnresolvedMethodBase
    {
        public static bool ContainsGenericParameters(IFunctionInfo adapter)
        {
            return adapter.GenericParameters.Count != 0;
        }

        public static Type[] GetGenericArguments(IFunctionInfo adapter)
        {
            return
                GenericUtils.ConvertAllToArray<IGenericParameterInfo, Type>(adapter.GenericParameters,
                    delegate(IGenericParameterInfo parameter) { return parameter.Resolve(false); });
        }

        public static ParameterInfo[] GetParameters(IFunctionInfo adapter)
        {
            return
                GenericUtils.ConvertAllToArray<IParameterInfo, ParameterInfo>(adapter.Parameters,
                    delegate(IParameterInfo parameter) { return parameter.Resolve(false); });
        }

        public static bool IsGenericMethod(IFunctionInfo adapter)
        {
            return ContainsGenericParameters(adapter) || adapter.DeclaringType.GenericParameters.Count != 0;
        }
    }

    public partial class UnresolvedConstructorInfo
    {
        /// <inheritdoc />
        public override MethodAttributes Attributes
        {
            get { return adapter.MethodAttributes; }
        }

        /// <inheritdoc />
        public override CallingConventions CallingConvention
        {
            get { return CallingConventions.Any; }
        }

        /// <inheritdoc />
        public override bool ContainsGenericParameters
        {
            get { return UnresolvedMethodBase.ContainsGenericParameters(adapter); }
        }

        /// <inheritdoc />
        public override bool IsGenericMethod
        {
            get { return UnresolvedMethodBase.IsGenericMethod(adapter); }
        }

        /// <inheritdoc />
        public override bool IsGenericMethodDefinition
        {
            get { return adapter.IsGenericMethodDefinition; }
        }

        /// <inheritdoc />
        public override RuntimeMethodHandle MethodHandle
        {
            get { throw new NotSupportedException(); }
        }

        /// <inheritdoc />
        public override Type[] GetGenericArguments()
        {
            return UnresolvedMethodBase.GetGenericArguments(adapter);
        }

        /// <inheritdoc />
        public override MethodBody GetMethodBody()
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc />
        public override MethodImplAttributes GetMethodImplementationFlags()
        {
            return MethodImplAttributes.ForwardRef;
        }

        /// <inheritdoc />
        public override ParameterInfo[] GetParameters()
        {
            return UnresolvedMethodBase.GetParameters(adapter);
        }

        /// <inheritdoc />
        public override object Invoke(object obj, BindingFlags invokeAttr, Binder binder, object[] parameters,
            CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }

    public partial class UnresolvedMethodInfo
    {
        /// <inheritdoc />
        public override MethodAttributes Attributes
        {
            get { return adapter.MethodAttributes; }
        }

        /// <inheritdoc />
        public override CallingConventions CallingConvention
        {
            get { return CallingConventions.Any; }
        }

        /// <inheritdoc />
        public override bool ContainsGenericParameters
        {
            get { return UnresolvedMethodBase.ContainsGenericParameters(adapter); }
        }

        /// <inheritdoc />
        public override bool IsGenericMethod
        {
            get { return UnresolvedMethodBase.IsGenericMethod(adapter); }
        }

        /// <inheritdoc />
        public override bool IsGenericMethodDefinition
        {
            get { return adapter.IsGenericMethodDefinition; }
        }

        /// <inheritdoc />
        public override RuntimeMethodHandle MethodHandle
        {
            get { throw new NotSupportedException(); }
        }

        /// <inheritdoc />
        public override Type[] GetGenericArguments()
        {
            return UnresolvedMethodBase.GetGenericArguments(adapter);
        }

        /// <inheritdoc />
        public override MethodBody GetMethodBody()
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc />
        public override MethodImplAttributes GetMethodImplementationFlags()
        {
            return MethodImplAttributes.ForwardRef;
        }

        /// <inheritdoc />
        public override ParameterInfo[] GetParameters()
        {
            return UnresolvedMethodBase.GetParameters(adapter);
        }

        /// <inheritdoc />
        public override object Invoke(object obj, BindingFlags invokeAttr, Binder binder, object[] parameters,
            CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}