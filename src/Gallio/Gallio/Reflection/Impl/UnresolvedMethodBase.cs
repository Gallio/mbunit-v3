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