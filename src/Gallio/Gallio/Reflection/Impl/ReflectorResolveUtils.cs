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
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Gallio.Collections;
using Gallio.Reflection;

namespace Gallio.Reflection.Impl
{
    /// <summary>
    /// <para>
    /// Provides helpers for resolving abstract reflection objects to obtain
    /// native ones based on the structural properties of the reflected
    /// code elements.
    /// </para>
    /// <para>
    /// This class is intended to assist with the implementation of new
    /// reflection policies.  It should not be used directly by clients of the
    /// reflection API.
    /// </para>
    /// </summary>
    public class ReflectorResolveUtils
    {
        private static readonly LazyCache<string, Assembly> resolvedAssemblyCache
            = new LazyCache<string, Assembly>(PopulateResolvedAssembly);
        private static readonly LazyCache<string, Assembly> resolvedAssemblyCacheWithFallbackOnPartialName
            = new LazyCache<string, Assembly>(PopulateResolvedAssemblyWithFallbackOnPartialName);

        /// <summary>
        /// Resolves a reflected assembly to its native <see cref="Assembly" /> object.
        /// </summary>
        /// <param name="assembly">The reflected assembly</param>
        /// <param name="fallbackOnPartialName">If true, allows the assembly to be resolved
        /// by partial name if no match could be found by fullname</param>
        /// <param name="throwOnError">If true, throws an exception if resolution fails,
        /// otherwise returns null</param>
        /// <returns>The resolved <see cref="Assembly" />.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="assembly"/>
        /// is null</exception>
        /// <exception cref="ReflectionWrapperResolveException">Thrown if <paramref name="assembly"/>
        /// could not be resolved</exception>
        public static Assembly ResolveAssembly(IAssemblyInfo assembly, bool fallbackOnPartialName, bool throwOnError)
        {
            if (assembly == null)
                throw new ArgumentNullException("assembly");

            string fullName = assembly.FullName;
            Assembly resolvedAssembly = fallbackOnPartialName
                ? resolvedAssemblyCacheWithFallbackOnPartialName[fullName]
                : resolvedAssemblyCache[fullName];

            if (throwOnError && resolvedAssembly == null)
                throw new ReflectionWrapperResolveException(assembly);

            return resolvedAssembly;
        }

        private static Assembly PopulateResolvedAssembly(string fullName)
        {
            try
            {
                return Assembly.Load(fullName);
            }
            catch (FileNotFoundException)
            {
            }
            catch (BadImageFormatException)
            {
            }

            return null;
        }

        private static Assembly PopulateResolvedAssemblyWithFallbackOnPartialName(string fullName)
        {
            Assembly resolvedAssembly = resolvedAssemblyCache[fullName];
            if (resolvedAssembly == null)
            {
                string partialName = new AssemblyName(fullName).Name;
                if (fullName != partialName)
                {
                    try
                    {
                        resolvedAssembly = Assembly.Load(partialName);
                    }
                    catch (FileNotFoundException)
                    {
                    }
                    catch (BadImageFormatException)
                    {
                    }
                }
            }

            return resolvedAssembly;
        }

        /// <summary>
        /// Resolves a reflected type to its native <see cref="Type" /> object.
        /// </summary>
        /// <param name="type">The reflected type</param>
        /// <param name="methodContext">The method that is currently in scope, or null if none.
        /// This parameter is used when resolving types that are part of the signature
        /// of a generic method so that generic method arguments can be handled correctly.</param>
        /// <param name="throwOnError">If true, throws an exception if resolution fails,
        /// otherwise returns an <see cref="UnresolvedType" /></param>
        /// <returns>The resolved <see cref="Type" />.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="type"/>
        /// is null</exception>
        /// <exception cref="ReflectionWrapperResolveException">Thrown if <paramref name="type"/>
        /// could not be resolved</exception>
        public static Type ResolveType(IResolvableTypeInfo type, MethodInfo methodContext, bool throwOnError)
        {
            if (type == null)
                throw new ArgumentNullException("type");

            try
            {
                ITypeInfo elementType = type.ElementType;
                if (elementType != null)
                {
                    Type resolvedElementType = ResolveTypeWithMethodContext(elementType, methodContext);

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

                if (type.IsGenericParameter)
                {
                    Type resolvedType = ResolveGenericParameter((IGenericParameterInfo)type, methodContext);
                    if (resolvedType != null)
                        return resolvedType;
                }
                else
                {
                    ITypeInfo simpleType = type.GenericTypeDefinition ?? type;

                    Assembly resolvedAssembly = simpleType.Assembly.Resolve(throwOnError);
                    if (resolvedAssembly != null)
                    {
                        Type resolvedType = resolvedAssembly.GetType(simpleType.FullName);
                        if (resolvedType != null)
                        {
                            if (type.IsGenericType && !type.IsGenericTypeDefinition)
                            {
                                Type[] resolvedTypeArguments = ResolveTypesWithMethodContext(type.GenericArguments, methodContext);
                                resolvedType = resolvedType.MakeGenericType(resolvedTypeArguments);
                            }

                            return resolvedType;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                if (throwOnError)
                    throw new ReflectionWrapperResolveException(type, ex);
            }

            if (throwOnError)
                throw new ReflectionWrapperResolveException(type);

            return new UnresolvedType(type);
        }

        private static Type ResolveGenericParameter(IGenericParameterInfo genericParameter, MethodInfo methodContext)
        {
            IMethodInfo declaringMethod = genericParameter.DeclaringMethod;
            if (declaringMethod != null)
            {
                if (methodContext == null)
                    methodContext = declaringMethod.Resolve(true);

                return methodContext.GetGenericArguments()[genericParameter.Position];
            }

            return genericParameter.DeclaringType.Resolve(true).GetGenericArguments()[genericParameter.Position];
        }

        /// <summary>
        /// Resolves a reflected field to its native <see cref="FieldInfo" /> object.
        /// </summary>
        /// <param name="field">The reflected type</param>
        /// <param name="throwOnError">If true, throws an exception if resolution fails,
        /// otherwise returns an <see cref="UnresolvedFieldInfo" /></param>
        /// <returns>The resolved <see cref="FieldInfo" />.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="field"/>
        /// is null</exception>
        /// <exception cref="ReflectionWrapperResolveException">Thrown if <paramref name="field"/>
        /// could not be resolved</exception>
        public static FieldInfo ResolveField(IFieldInfo field, bool throwOnError)
        {
            if (field == null)
                throw new ArgumentNullException("field");

            try
            {
                Type resolvedType = field.DeclaringType.Resolve(throwOnError);
                if (!(resolvedType is UnresolvedType))
                {
                    FieldInfo resolvedField = resolvedType.GetField(field.Name, BindingFlags.Public | BindingFlags.NonPublic
                        | BindingFlags.Instance | BindingFlags.Static);

                    if (resolvedField != null)
                        return resolvedField;
                }
            }
            catch (Exception ex)
            {
                if (throwOnError)
                    throw new ReflectionWrapperResolveException(field, ex);
            }

            if (throwOnError)
                throw new ReflectionWrapperResolveException(field);

            return new UnresolvedFieldInfo(field);
        }

        /// <summary>
        /// Resolves a reflected property to its native <see cref="PropertyInfo" /> object.
        /// </summary>
        /// <param name="property">The reflected property</param>
        /// <param name="throwOnError">If true, throws an exception if resolution fails,
        /// otherwise returns an <see cref="UnresolvedPropertyInfo" /></param>
        /// <returns>The resolved <see cref="PropertyInfo" />.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="property"/>
        /// is null</exception>
        /// <exception cref="ReflectionWrapperResolveException">Thrown if <paramref name="property"/>
        /// could not be resolved</exception>
        public static PropertyInfo ResolveProperty(IPropertyInfo property, bool throwOnError)
        {
            if (property == null)
                throw new ArgumentNullException("property");

            try
            {
                Type resolvedType = property.DeclaringType.Resolve(throwOnError);
                if (!(resolvedType is UnresolvedType))
                {
                    Type returnType = property.ValueType.Resolve(true);
                    Type[] parameterTypes = ResolveParameterTypes(property.IndexParameters);

                    PropertyInfo resolvedProperty =
                        resolvedType.GetProperty(property.Name, BindingFlags.Public | BindingFlags.NonPublic
                            | BindingFlags.Instance | BindingFlags.Static, null, returnType, parameterTypes, null);

                    if (resolvedProperty != null)
                        return resolvedProperty;
                }
            }
            catch (Exception ex)
            {
                if (throwOnError)
                    throw new ReflectionWrapperResolveException(property, ex);
            }

            if (throwOnError)
                throw new ReflectionWrapperResolveException(property);

            return new UnresolvedPropertyInfo(property);
        }

        /// <summary>
        /// Resolves a reflected event to its native <see cref="EventInfo" /> object.
        /// </summary>
        /// <param name="event">The reflected event</param>
        /// <param name="throwOnError">If true, throws an exception if resolution fails,
        /// otherwise returns an <see cref="UnresolvedEventInfo" /></param>
        /// <returns>The resolved <see cref="EventInfo" />.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="event"/>
        /// is null</exception>
        /// <exception cref="ReflectionWrapperResolveException">Thrown if <paramref name="event"/>
        /// could not be resolved</exception>
        public static EventInfo ResolveEvent(IEventInfo @event, bool throwOnError)
        {
            if (@event == null)
                throw new ArgumentNullException("event");

            try
            {
                Type resolvedType = @event.DeclaringType.Resolve(throwOnError);
                if (!(resolvedType is UnresolvedType))
                {
                    EventInfo resolvedEvent =
                        resolvedType.GetEvent(@event.Name, BindingFlags.Public | BindingFlags.NonPublic
                            | BindingFlags.Instance | BindingFlags.Static);

                    if (resolvedEvent != null)
                        return resolvedEvent;
                }
            }
            catch (Exception ex)
            {
                if (throwOnError)
                    throw new ReflectionWrapperResolveException(@event, ex);
            }

            if (throwOnError)
                throw new ReflectionWrapperResolveException(@event);

            return new UnresolvedEventInfo(@event);
        }

        /// <summary>
        /// Resolves a reflected constructor to its native <see cref="ConstructorInfo" /> object.
        /// </summary>
        /// <param name="constructor">The reflected constructor</param>
        /// <param name="throwOnError">If true, throws an exception if resolution fails,
        /// otherwise returns an <see cref="UnresolvedConstructorInfo" /></param>
        /// <returns>The resolved <see cref="ConstructorInfo" />.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="constructor"/>
        /// is null</exception>
        /// <exception cref="ReflectionWrapperResolveException">Thrown if <paramref name="constructor"/>
        /// could not be resolved</exception>
        public static ConstructorInfo ResolveConstructor(IConstructorInfo constructor, bool throwOnError)
        {
            if (constructor == null)
                throw new ArgumentNullException("constructor");

            try
            {
                Type resolvedType = constructor.DeclaringType.Resolve(throwOnError);
                if (!(resolvedType is UnresolvedType))
                {
                    BindingFlags bindingFlags =
                        (constructor.IsPublic ? BindingFlags.Public : BindingFlags.NonPublic)
                        | (constructor.IsStatic ? BindingFlags.Static : BindingFlags.Instance);

                    Type[] resolvedParameterTypes = ResolveParameterTypes(constructor.Parameters);
                    ConstructorInfo resolvedConstructor = resolvedType.GetConstructor(
                        bindingFlags, null, resolvedParameterTypes, null);

                    if (resolvedConstructor != null)
                        return resolvedConstructor;
                }
            }
            catch (Exception ex)
            {
                if (throwOnError)
                    throw new ReflectionWrapperResolveException(constructor, ex);
            }

            if (throwOnError)
                throw new ReflectionWrapperResolveException(constructor);

            return new UnresolvedConstructorInfo(constructor);
        }

        /// <summary>
        /// Resolves a reflected method to its native <see cref="MethodInfo" /> object.
        /// </summary>
        /// <param name="method">The reflected method</param>
        /// <param name="throwOnError">If true, throws an exception if resolution fails,
        /// otherwise returns an <see cref="UnresolvedMethodInfo" /></param>
        /// <returns>The resolved <see cref="MethodInfo" />.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="method"/>
        /// is null</exception>
        /// <exception cref="ReflectionWrapperResolveException">Thrown if <paramref name="method"/>
        /// could not be resolved</exception>
        public static MethodInfo ResolveMethod(IMethodInfo method, bool throwOnError)
        {
            if (method == null)
                throw new ArgumentNullException("method");

            try
            {
                Type resolvedType = method.DeclaringType.Resolve(throwOnError);
                if (!(resolvedType is UnresolvedType))
                {
                    BindingFlags bindingFlags = BindingFlags.DeclaredOnly
                        | (method.IsPublic ? BindingFlags.Public : BindingFlags.NonPublic)
                        | (method.IsStatic ? BindingFlags.Static : BindingFlags.Instance);

                    string methodName = method.Name;

                    MethodInfo resolvedMethod = method.IsGenericMethod
                        ? ResolveGenericMethod(resolvedType, methodName, bindingFlags, method.GenericMethodDefinition.Parameters, method.GenericArguments)
                        : ResolveNonGenericMethod(resolvedType, methodName, bindingFlags, method.Parameters);

                    if (resolvedMethod != null)
                        return resolvedMethod;
                }
            }
            catch (Exception ex)
            {
                if (throwOnError)
                    throw new ReflectionWrapperResolveException(method, ex);
            }

            if (throwOnError)
                throw new ReflectionWrapperResolveException(method);

            return new UnresolvedMethodInfo(method);
        }

        private static MethodInfo ResolveNonGenericMethod(Type resolvedType, string methodName, BindingFlags bindingFlags,
            ICollection<IParameterInfo> parameters)
        {
            Type[] resolvedParameterTypes = ResolveParameterTypes(parameters);
            return resolvedType.GetMethod(methodName, bindingFlags, null, resolvedParameterTypes, null);
        }

        private static MethodInfo ResolveGenericMethod(Type resolvedType, string methodName, BindingFlags bindingFlags,
            IList<IParameterInfo> parameters, ICollection<ITypeInfo> genericArguments)
        {
            foreach (MethodInfo method in resolvedType.GetMethods(bindingFlags))
            {
                if (method.Name != methodName
                    || ! method.IsGenericMethod
                    || method.GetGenericArguments().Length != genericArguments.Count)
                    continue;

                if (HasSameParameters(method, parameters))
                {
                    Type[] resolvedTypeArguments = ResolveTypesWithMethodContext(genericArguments, method);
                    return method.MakeGenericMethod(resolvedTypeArguments);
                }
            }

            return null;
        }

        private static bool HasSameParameters(MethodInfo method, IList<IParameterInfo> parameters)
        {
            ParameterInfo[] methodParameters = method.GetParameters();
            if (methodParameters.Length != parameters.Count)
                return false;

            for (int i = 0; i < methodParameters.Length; i++)
            {
                ITypeInfo valueType = parameters[i].ValueType;
                try
                {
                    Type resolvedValueType = ResolveTypeWithMethodContext(valueType, method);
                    if (! resolvedValueType.Equals(methodParameters[i].ParameterType))
                        return false;
                }
                catch (ReflectionWrapperResolveException)
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Resolves a reflected parameter to its native <see cref="ParameterInfo" /> object.
        /// </summary>
        /// <param name="parameter">The reflected parameter</param>
        /// <param name="throwOnError">If true, throws an exception if resolution fails,
        /// otherwise returns an <see cref="UnresolvedParameterInfo" /></param>
        /// <returns>The resolved <see cref="ParameterInfo" />.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="parameter"/>
        /// is null</exception>
        /// <exception cref="ReflectionWrapperResolveException">Thrown if <paramref name="parameter"/>
        /// could not be resolved</exception>
        public static ParameterInfo ResolveParameter(IParameterInfo parameter, bool throwOnError)
        {
            if (parameter == null)
                throw new ArgumentNullException("parameter");

            try
            {
                MemberInfo resolvedMember = parameter.Member.Resolve(throwOnError);
                if (!(resolvedMember is IUnresolvedCodeElement))
                {
                    int parameterIndex = parameter.Position;
                    ParameterInfo[] resolvedParameters;

                    MethodBase resolvedMethod = resolvedMember as MethodBase;
                    if (resolvedMethod != null)
                    {
                        if (parameterIndex == -1)
                            return ((MethodInfo)resolvedMethod).ReturnParameter;

                        resolvedParameters = resolvedMethod.GetParameters();
                    }
                    else
                    {
                        PropertyInfo resolvedProperty = (PropertyInfo)resolvedMember;
                        resolvedParameters = resolvedProperty.GetIndexParameters();
                    }

                    if (parameterIndex < resolvedParameters.Length)
                        return resolvedParameters[parameterIndex];
                }
            }
            catch (Exception ex)
            {
                if (throwOnError)
                    throw new ReflectionWrapperResolveException(parameter, ex);
            }

            if (throwOnError)
                throw new ReflectionWrapperResolveException(parameter);

            return new UnresolvedParameterInfo(parameter);
        }

        private static Type[] ResolveParameterTypes(ICollection<IParameterInfo> parameters)
        {
            return GenericUtils.ConvertAllToArray<IParameterInfo, Type>(parameters, delegate(IParameterInfo parameter)
            {
                return parameter.ValueType.Resolve(true);
            });
        }

        private static Type[] ResolveTypesWithMethodContext(ICollection<ITypeInfo> types, MethodInfo methodContext)
        {
            return GenericUtils.ConvertAllToArray<ITypeInfo, Type>(types, delegate(ITypeInfo argument)
            {
                return ResolveTypeWithMethodContext(argument, methodContext);
            });
        }

        private static Type ResolveTypeWithMethodContext(ITypeInfo type, MethodInfo methodContext)
        {
            IResolvableTypeInfo resolvableType = type as IResolvableTypeInfo;
            return resolvableType != null ? resolvableType.Resolve(methodContext, true) : type.Resolve(true);
        }
    }
}