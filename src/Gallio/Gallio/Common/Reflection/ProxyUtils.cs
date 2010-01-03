using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;

namespace Gallio.Common.Reflection
{
    /// <summary>
    /// Utilities for generating dynamic proxies and adapters using code generation techniques.
    /// </summary>
    public static class ProxyUtils
    {
        /// <summary>
        /// Coerces a source delegate to a target delegate type by casting the source delegate's
        /// parameters individually to the target delegate type's parameter types.
        /// </summary>
        /// <param name="targetDelegateType">The target delegate type.</param>
        /// <param name="sourceDelegate">The source delegate, or null.</param>
        /// <returns>The target delegate, or null if the source delegate was null.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="targetDelegateType"/>
        /// is null.</exception>
        /// <exception cref="ArgumentException">Thrown if <paramref name="targetDelegateType"/>
        /// is not a delegate type.</exception>
        /// <exception cref="InvalidOperationException">Thrown if the source and target delegate types are
        /// incompatible such as if they have different numbers of parameters or a different
        /// composition of out/ref parameters.</exception>
        public static Delegate CoerceDelegate(Type targetDelegateType, Delegate sourceDelegate)
        {
            if (targetDelegateType == null)
                throw new ArgumentNullException("targetDelegateType");
            if (sourceDelegate == null)
                return null;
            if (!typeof(Delegate).IsAssignableFrom(targetDelegateType))
                throw new ArgumentException("Target type must be a Delegate type.", "targetDelegateType");

            Type sourceDelegateType = sourceDelegate.GetType();
            if (targetDelegateType.IsAssignableFrom(sourceDelegateType))
                return sourceDelegate;

            MethodInfo targetInvokeMethod, sourceInvokeMethod;
            Type targetReturnType, sourceReturnType;
            ParameterInfo[] targetParameters, sourceParameters;
            GetDelegateTypeInfo(targetDelegateType, out targetInvokeMethod, out targetReturnType, out targetParameters);
            GetDelegateTypeInfo(sourceDelegateType, out sourceInvokeMethod, out sourceReturnType, out sourceParameters);

            if (!IsDelegateReturnTypeCompatible(targetReturnType, sourceReturnType))
                throw new InvalidOperationException("The delegate signatures cannot be coerced because they have incompatible return types.");
            if (!IsDelegateParameterListCompatible(targetParameters, sourceParameters))
                throw new InvalidOperationException("The delegate signatures cannot be coerced because they have incompatible parameters.");

            // Recreating the method each time is not very efficient but this function is not intended
            // to be used very often.
            // TODO: Determine whether it would really be worth caching the method.
            Type[] invokerParameterTypes = new Type[targetParameters.Length + 1];
            invokerParameterTypes[0] = sourceDelegateType;
            for (int i = 0; i < targetParameters.Length; i++)
                invokerParameterTypes[i + 1] = targetParameters[i].ParameterType;

            DynamicMethod method = new DynamicMethod("DelegateInvoker", targetReturnType, invokerParameterTypes, true);
            method.DefineParameter(0, ParameterAttributes.None, "__sourceDelegate");
            for (int i = 0; i < targetParameters.Length; i++)
                method.DefineParameter(i + 1, targetParameters[i].Attributes, targetParameters[i].Name);

            ILGenerator gen = method.GetILGenerator();

            // Load the source delegate onto the top of the evaluation stack.
            gen.Emit(OpCodes.Ldarg_0);

            // Load the arguments onto the evaluation stack.
            LocalBuilder[] locals = null;
            for (int i = 0; i < targetParameters.Length; i++)
            {
                if (IsOutputParameter(sourceParameters[i]))
                {
                    if (locals == null)
                        locals = new LocalBuilder[targetParameters.Length];

                    locals[i] = gen.DeclareLocal(sourceParameters[i].ParameterType.GetElementType());

                    if (IsInputParameter(sourceParameters[i]))
                    {
                        gen.Emit(OpCodes.Ldarg, i + 1);
                        EmitLoadIndirect(gen, targetParameters[i].ParameterType.GetElementType());
                        EmitConversion(gen, sourceParameters[i].ParameterType.GetElementType(), targetParameters[i].ParameterType.GetElementType());
                        gen.Emit(OpCodes.Stloc, locals[i].LocalIndex);
                    }

                    gen.Emit(OpCodes.Ldloca, locals[i].LocalIndex);
                }
                else
                {
                    gen.Emit(OpCodes.Ldarg, i + 1);
                    EmitConversion(gen, sourceParameters[i].ParameterType, targetParameters[i].ParameterType);
                }
            }

            // Call the source delegate's Invoke method.
            gen.EmitCall(OpCodes.Callvirt, sourceInvokeMethod, null);

            // Store the resulting output parameters.
            for (int i = 0; i < targetParameters.Length; i++)
            {
                if (IsOutputParameter(sourceParameters[i]))
                {
                    gen.Emit(OpCodes.Ldarg, i + 1);
                    gen.Emit(OpCodes.Ldloc, locals[i].LocalIndex);
                    EmitConversion(gen, targetParameters[i].ParameterType.GetElementType(), sourceParameters[i].ParameterType.GetElementType());
                    EmitStoreIndirect(gen, targetParameters[i].ParameterType.GetElementType());
                }
            }

            // Return the result, if any.
            if (targetReturnType != null)
                EmitConversion(gen, targetReturnType, sourceReturnType);
            gen.Emit(OpCodes.Ret);

            return method.CreateDelegate(targetDelegateType, sourceDelegate);
        }

        private static void GetDelegateTypeInfo(Type delegateType,
            out MethodInfo invokeMethod, out Type returnType, out ParameterInfo[] parameters)
        {
            invokeMethod = delegateType.GetMethod("Invoke");

            returnType = invokeMethod.ReturnType;
            if (returnType == typeof(void))
                returnType = null;

            parameters = invokeMethod.GetParameters();
        }

        private static bool IsDelegateReturnTypeCompatible(Type targetReturnType, Type sourceReturnType)
        {
            if (targetReturnType == null && sourceReturnType == null)
                return true;
            if (targetReturnType == null || sourceReturnType == null)
                return false;
            return IsAssignableWithCast(targetReturnType, sourceReturnType);
        }

        private static bool IsDelegateParameterListCompatible(ParameterInfo[] targetParameters, ParameterInfo[] sourceParameters)
        {
            if (targetParameters.Length != sourceParameters.Length)
                return false;

            for (int i = 0; i < targetParameters.Length; i++)
            {
                if (IsInputParameter(targetParameters[i]) != IsInputParameter(sourceParameters[i])
                    || IsOutputParameter(targetParameters[i]) != IsOutputParameter(sourceParameters[i]))
                    return false;

                if (IsInputParameter(targetParameters[i])
                    && !IsAssignableWithCast(targetParameters[i].ParameterType, sourceParameters[i].ParameterType))
                    return false;

                if (IsOutputParameter(targetParameters[i])
                    && !IsAssignableWithCast(sourceParameters[i].ParameterType, targetParameters[i].ParameterType))
                    return false;
            }

            return true;
        }

        private static bool IsInputParameter(ParameterInfo parameter)
        {
            return parameter.IsIn || !parameter.IsOut;
        }

        private static bool IsOutputParameter(ParameterInfo parameter)
        {
            return parameter.ParameterType.IsByRef;
        }

        private static bool IsAssignableWithCast(Type targetType, Type sourceType)
        {
            // TODO: Should check whether it's possible to assign a value of a source type
            // to the target type even if it might require a downcast.  For example,
            // Object is possibly assignable to Int32 but Int32 is definitely not
            // assignable to KeyValuePair.
            return true;
        }

        private static void EmitLoadIndirect(ILGenerator gen, Type type)
        {
            switch (Type.GetTypeCode(type))
            {
                case TypeCode.Empty:
                case TypeCode.Object:
                case TypeCode.DBNull:
                case TypeCode.Decimal:
                case TypeCode.DateTime:
                case TypeCode.String:
                    if (type.IsValueType)
                        gen.Emit(OpCodes.Ldobj, type);
                    else
                        gen.Emit(OpCodes.Ldind_Ref);
                    break;
                case TypeCode.Boolean:
                case TypeCode.Int32:
                case TypeCode.UInt32:
                    gen.Emit(OpCodes.Ldind_I4);
                    break;
                case TypeCode.Char:
                case TypeCode.Int16:
                case TypeCode.UInt16:
                    gen.Emit(OpCodes.Ldind_I2);
                    break;
                case TypeCode.SByte:
                case TypeCode.Byte:
                    gen.Emit(OpCodes.Ldind_I1);
                    break;
                case TypeCode.Int64:
                case TypeCode.UInt64:
                    gen.Emit(OpCodes.Ldind_I8);
                    break;
                case TypeCode.Single:
                    gen.Emit(OpCodes.Ldind_R4);
                    break;
                case TypeCode.Double:
                    gen.Emit(OpCodes.Ldind_R8);
                    break;
                default:
                    throw new NotSupportedException();
            }
        }

        private static void EmitStoreIndirect(ILGenerator gen, Type type)
        {
            switch (Type.GetTypeCode(type))
            {
                case TypeCode.Empty:
                case TypeCode.Object:
                case TypeCode.DBNull:
                case TypeCode.Decimal:
                case TypeCode.DateTime:
                case TypeCode.String:
                    if (type.IsValueType)
                        gen.Emit(OpCodes.Stobj, type);
                    else
                        gen.Emit(OpCodes.Stind_Ref);
                    break;
                case TypeCode.Boolean:
                case TypeCode.Int32:
                case TypeCode.UInt32:
                    gen.Emit(OpCodes.Stind_I4);
                    break;
                case TypeCode.Char:
                case TypeCode.Int16:
                case TypeCode.UInt16:
                    gen.Emit(OpCodes.Stind_I2);
                    break;
                case TypeCode.SByte:
                case TypeCode.Byte:
                    gen.Emit(OpCodes.Stind_I1);
                    break;
                case TypeCode.Int64:
                case TypeCode.UInt64:
                    gen.Emit(OpCodes.Stind_I8);
                    break;
                case TypeCode.Single:
                    gen.Emit(OpCodes.Stind_R4);
                    break;
                case TypeCode.Double:
                    gen.Emit(OpCodes.Stind_R8);
                    break;
                default:
                    throw new NotSupportedException();
            }
        }

        private static void EmitConversion(ILGenerator gen, Type targetType, Type sourceType)
        {
            if (targetType.IsValueType)
            {
                if (sourceType.IsValueType)
                {
                    if (sourceType != targetType)
                        throw new NotSupportedException("Conversions between different values types not supported.");
                }
                else
                {
                    if (sourceType != typeof(object))
                        throw new NotSupportedException("Conversions from reference types other than Object to values types not supported.");

                    gen.Emit(OpCodes.Unbox, targetType);
                    gen.Emit(OpCodes.Ldobj, targetType);
                }
            }
            else
            {
                if (sourceType.IsValueType)
                {
                    if (targetType != typeof(object))
                        throw new NotSupportedException("Conversions from value types to reference types other than Object not supported.");

                    gen.Emit(OpCodes.Box, sourceType);
                }
                else
                {
                    if (sourceType != targetType)
                        gen.Emit(OpCodes.Castclass, targetType);
                }
            }
        }
    }
}
