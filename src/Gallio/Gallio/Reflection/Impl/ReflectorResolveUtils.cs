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
        /// <summary>
        /// Resolves a reflected type to its native <see cref="Type" /> object.
        /// </summary>
        /// <param name="type">The reflected type</param>
        /// <returns>The resolved <see cref="Type" />.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="type"/>
        /// is null</exception>
        /// <exception cref="CodeElementResolveException">Thrown if <paramref name="type"/>
        /// could not be resolved</exception>
        public static Type ResolveType(ITypeInfo type)
        {
            if (type == null)
                throw new ArgumentNullException("type");

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
                    throw new NotImplementedException("Resolving generic parameters not implemented yet.");
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

        /// <summary>
        /// Resolves a reflected field to its native <see cref="FieldInfo" /> object.
        /// </summary>
        /// <param name="field">The reflected type</param>
        /// <returns>The resolved <see cref="FieldInfo" />.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="field"/>
        /// is null</exception>
        /// <exception cref="CodeElementResolveException">Thrown if <paramref name="field"/>
        /// could not be resolved</exception>
        public static FieldInfo ResolveField(IFieldInfo field)
        {
            if (field == null)
                throw new ArgumentNullException("field");

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

        /// <summary>
        /// Resolves a reflected property to its native <see cref="PropertyInfo" /> object.
        /// </summary>
        /// <param name="property">The reflected property</param>
        /// <returns>The resolved <see cref="PropertyInfo" />.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="property"/>
        /// is null</exception>
        /// <exception cref="CodeElementResolveException">Thrown if <paramref name="property"/>
        /// could not be resolved</exception>
        public static PropertyInfo ResolveProperty(IPropertyInfo property)
        {
            if (property == null)
                throw new ArgumentNullException("property");

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

        /// <summary>
        /// Resolves a reflected event to its native <see cref="EventInfo" /> object.
        /// </summary>
        /// <param name="event">The reflected event</param>
        /// <returns>The resolved <see cref="EventInfo" />.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="event"/>
        /// is null</exception>
        /// <exception cref="CodeElementResolveException">Thrown if <paramref name="event"/>
        /// could not be resolved</exception>
        public static EventInfo ResolveEvent(IEventInfo @event)
        {
            if (@event == null)
                throw new ArgumentNullException("event");

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

        /// <summary>
        /// Resolves a reflected constructor to its native <see cref="ConstructorInfo" /> object.
        /// </summary>
        /// <param name="constructor">The reflected constructor</param>
        /// <returns>The resolved <see cref="ConstructorInfo" />.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="constructor"/>
        /// is null</exception>
        /// <exception cref="CodeElementResolveException">Thrown if <paramref name="constructor"/>
        /// could not be resolved</exception>
        public static ConstructorInfo ResolveConstructor(IConstructorInfo constructor)
        {
            if (constructor == null)
                throw new ArgumentNullException("constructor");

            try
            {
                Type resolvedType = constructor.DeclaringType.Resolve();
                Type[] resolvedParameterTypes = ResolveParameterTypes(constructor);
                ConstructorInfo resolvedConstructor = resolvedType.GetConstructor(
                    BindingFlags.Public | BindingFlags.NonPublic
                    | (constructor.IsStatic ? BindingFlags.Static : BindingFlags.Instance),
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

        /// <summary>
        /// Resolves a reflected method to its native <see cref="MethodInfo" /> object.
        /// </summary>
        /// <param name="method">The reflected method</param>
        /// <returns>The resolved <see cref="MethodInfo" />.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="method"/>
        /// is null</exception>
        /// <exception cref="CodeElementResolveException">Thrown if <paramref name="method"/>
        /// could not be resolved</exception>
        public static MethodInfo ResolveMethod(IMethodInfo method)
        {
            if (method == null)
                throw new ArgumentNullException("method");

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

        /// <summary>
        /// Resolves a reflected parameter to its native <see cref="ParameterInfo" /> object.
        /// </summary>
        /// <param name="parameter">The reflected parameter</param>
        /// <returns>The resolved <see cref="ParameterInfo" />.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="parameter"/>
        /// is null</exception>
        /// <exception cref="CodeElementResolveException">Thrown if <paramref name="parameter"/>
        /// could not be resolved</exception>
        public static ParameterInfo ResolveParameter(IParameterInfo parameter)
        {
            if (parameter == null)
                throw new ArgumentNullException("parameter");

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
            return GenericUtils.ConvertAllToArray<IParameterInfo, Type>(function.Parameters, delegate(IParameterInfo parameter)
            {
                return parameter.ValueType.Resolve();
            });
        }
    }
}