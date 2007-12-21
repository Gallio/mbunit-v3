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
using System.Reflection;
using Gallio.Collections;
using Gallio.Model.Reflection;

namespace Gallio.ReSharperRunner.Reflection
{
    /// <summary>
    /// Abstract base class for ReSharper reflector types.
    /// Provides a means of controlling how and when assembly loading takes
    /// place.
    /// </summary>
    internal abstract class BaseReSharperReflector : IReflectionPolicy
    {
        private readonly IAssemblyResolver assemblyResolver;

        public BaseReSharperReflector(IAssemblyResolver assemblyResolver)
        {
            if (assemblyResolver == null)
                throw new ArgumentNullException("assemblyResolver");

            this.assemblyResolver = assemblyResolver;
        }

        protected Assembly ResolveAssembly(IAssemblyInfo assembly)
        {
            try
            {
                Assembly resolvedAssembly = assemblyResolver.ResolveAssembly(assembly);
                return resolvedAssembly;
            }
            catch (Exception ex)
            {
                throw new CodeElementResolveException(assembly, ex);
            }
        }

        /// <inheritdoc />
        public abstract IAssemblyInfo LoadAssembly(AssemblyName assemblyName);

        protected static void AddFlagIfTrue(ref TypeAttributes flags, TypeAttributes flagToAdd, bool condition)
        {
            if (condition)
                flags |= flagToAdd;
        }

        protected static void AddFlagIfTrue(ref MethodAttributes flags, MethodAttributes flagToAdd, bool condition)
        {
            if (condition)
                flags |= flagToAdd;
        }

        protected static void AddFlagIfTrue(ref FieldAttributes flags, FieldAttributes flagToAdd, bool condition)
        {
            if (condition)
                flags |= flagToAdd;
        }

        protected static void AddFlagIfTrue(ref PropertyAttributes flags, PropertyAttributes flagToAdd, bool condition)
        {
            if (condition)
                flags |= flagToAdd;
        }

        protected static void AddFlagIfTrue(ref ParameterAttributes flags, ParameterAttributes flagToAdd, bool condition)
        {
            if (condition)
                flags |= flagToAdd;
        }

        protected static TypeCode GetTypeCode(ITypeInfo type)
        {
            if (type == null)
                return TypeCode.Empty;

            switch (type.FullName)
            {
                case "System.Boolean":
                    return TypeCode.Boolean;
                case "System.Byte":
                    return TypeCode.Byte;
                case "System.Char":
                    return TypeCode.Char;
                case "System.DateTime":
                    return TypeCode.DateTime;
                case "System.DBNull":
                    return TypeCode.DBNull;
                case "System.Decimal":
                    return TypeCode.Decimal;
                case "System.Double":
                    return TypeCode.Double;
                case "System.Int16":
                    return TypeCode.Int16;
                case "System.Int32":
                    return TypeCode.Int32;
                case "System.Int64":
                    return TypeCode.Int64;
                case "System.SByte":
                    return TypeCode.SByte;
                case "System.Single":
                    return TypeCode.Single;
                case "System.String":
                    return TypeCode.String;
                case "System.UInt16":
                    return TypeCode.UInt16;
                case "System.UInt32":
                    return TypeCode.UInt32;
                case "System.UInt64":
                    return TypeCode.UInt64;
                default:
                    return TypeCode.Object;
            }
        }

        protected static object GetDefaultValue(ITypeInfo type)
        {
            switch (type.TypeCode)
            {
                case TypeCode.Boolean:
                    return default(Boolean);
                case TypeCode.Byte:
                    return default(Byte);
                case TypeCode.Char:
                    return default(Char);
                case TypeCode.DateTime:
                    return default(DateTime);
                case TypeCode.DBNull:
                    return default(DBNull);
                case TypeCode.Decimal:
                    return default(Decimal);
                case TypeCode.Double:
                    return default(Double);
                case TypeCode.Empty:
                    return null;
                case TypeCode.Int16:
                    return default(Int16);
                case TypeCode.Int32:
                    return default(Int32);
                case TypeCode.Int64:
                    return default(Int64);
                case TypeCode.Object:
                    return default(Object);
                case TypeCode.SByte:
                    return default(SByte);
                case TypeCode.Single:
                    return default(Single);
                case TypeCode.String:
                    return default(String);
                case TypeCode.UInt16:
                    return default(UInt16);
                case TypeCode.UInt32:
                    return default(UInt32);
                case TypeCode.UInt64:
                    return default(UInt64);
                default:
                    throw new NotSupportedException("TypeCode not supported.");
            }
        }

        protected static bool IsAttributeField(IFieldInfo field)
        {
            return !field.IsLiteral && !field.IsInitOnly && !field.IsStatic;
        }

        protected static bool IsAttributeProperty(IPropertyInfo property)
        {
            IMethodInfo getMethod = property.GetGetMethod();
            IMethodInfo setMethod = property.GetSetMethod();
            return getMethod != null && setMethod != null
                && getMethod.IsPublic && ! getMethod.IsAbstract && ! getMethod.IsStatic
                && setMethod.IsPublic && ! setMethod.IsAbstract && ! setMethod.IsStatic;
        }

        protected static Type ResolveType(ITypeInfo type)
        {
            try
            {
                ITypeInfo elementType = type.ElementType;
                if (elementType != null)
                {
                    Type resolvedElementType = type.ElementType.Resolve();

                    if (type.IsArray)
                    {
                        int rank = type.ArrayRank;
                        if (rank == 1)
                            return resolvedElementType.MakeArrayType();
                        else
                            return resolvedElementType.MakeArrayType(rank);
                    }
                    else if (type.IsByRef)
                    {
                        return resolvedElementType.MakeByRefType();
                    }
                    else if (type.IsPointer)
                    {
                        return resolvedElementType.MakePointerType();
                    }
                }
                else if (type.IsGenericParameter)
                {
                    throw new NotImplementedException("Resolving generic parameters not supported yet.");
                }

                Assembly resolvedAssembly = type.Assembly.Resolve();
                Type resolvedType = resolvedAssembly.GetType(type.FullName);
                if (resolvedType != null)
                    return resolvedType;
            }
            catch (Exception ex)
            {
                throw new CodeElementResolveException(type, ex);
            }

            throw new CodeElementResolveException(type);
        }

        protected static FieldInfo ResolveField(IFieldInfo field)
        {
            try
            {
                Type resolvedType = field.DeclaringType.Resolve();
                FieldInfo resolvedField = resolvedType.GetField(field.Name, BindingFlags.Public | BindingFlags.NonPublic
                    | BindingFlags.Instance | BindingFlags.Static);

                if (resolvedField != null)
                    return resolvedField;
            }
            catch (Exception ex)
            {
                throw new CodeElementResolveException(field, ex);
            }

            throw new CodeElementResolveException(field);
        }

        protected static PropertyInfo ResolveProperty(IPropertyInfo property)
        {
            try
            {
                Type resolvedType = property.DeclaringType.Resolve();
                PropertyInfo resolvedProperty =
                    resolvedType.GetProperty(property.Name, BindingFlags.Public | BindingFlags.NonPublic
                        | BindingFlags.Instance | BindingFlags.Static);

                if (resolvedProperty != null)
                    return resolvedProperty;
            }
            catch (Exception ex)
            {
                throw new CodeElementResolveException(property, ex);
            }

            throw new CodeElementResolveException(property);
        }

        protected static EventInfo ResolveEvent(IEventInfo @event)
        {
            try
            {
                Type resolvedType = @event.DeclaringType.Resolve();
                EventInfo resolvedEvent =
                    resolvedType.GetEvent(@event.Name, BindingFlags.Public | BindingFlags.NonPublic
                        | BindingFlags.Instance | BindingFlags.Static);

                if (resolvedEvent != null)
                    return resolvedEvent;
            }
            catch (Exception ex)
            {
                throw new CodeElementResolveException(@event, ex);
            }

            throw new CodeElementResolveException(@event);
        }

        protected static ConstructorInfo ResolveConstructor(IConstructorInfo constructor)
        {
            try
            {
                Type resolvedType = constructor.DeclaringType.Resolve();
                Type[] resolvedParameterTypes = ResolveParameterTypes(constructor);
                ConstructorInfo resolvedConstructor = resolvedType.GetConstructor(
                    BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static,
                    null, resolvedParameterTypes, null);

                if (resolvedConstructor != null)
                    return resolvedConstructor;
            }
            catch (Exception ex)
            {
                throw new CodeElementResolveException(constructor, ex);
            }

            throw new CodeElementResolveException(constructor);
        }

        protected static MethodInfo ResolveMethod(IMethodInfo method)
        {
            try
            {
                Type resolvedType = method.DeclaringType.Resolve();
                Type[] resolvedParameterTypes = ResolveParameterTypes(method);
                MethodInfo resolvedMethod = resolvedType.GetMethod(method.Name,
                    BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static,
                    null, resolvedParameterTypes, null);

                if (resolvedMethod != null)
                {
                    if (resolvedMethod.IsGenericMethodDefinition)
                        throw new NotImplementedException();
                    //    resolvedMethod = resolvedMethod.MakeGenericMethod(ResolveGenericParameterTypes(method));

                    return resolvedMethod;
                }
            }
            catch (Exception ex)
            {
                throw new CodeElementResolveException(method, ex);
            }

            throw new CodeElementResolveException(method);
        }

        protected static ParameterInfo ResolveParameter(IParameterInfo parameter)
        {
            try
            {
                MethodBase resolvedMethod = (MethodBase)parameter.Member.Resolve();
                ParameterInfo[] resolvedParameters = resolvedMethod.GetParameters();

                int parameterIndex = parameter.Position;
                if (parameterIndex < resolvedParameters.Length)
                    return resolvedParameters[parameterIndex];
            }
            catch (Exception ex)
            {
                throw new CodeElementResolveException(parameter, ex);
            }

            throw new CodeElementResolveException(parameter);
        }

        private static Type[] ResolveParameterTypes(IFunctionInfo function)
        {
            return GenericUtils.ConvertAllToArray<IParameterInfo, Type>(function.GetParameters(), delegate(IParameterInfo parameter)
            {
                return parameter.ValueType.Resolve();
            });
        }
    }
}
