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

namespace Gallio.Reflection
{
    /// <summary>
    /// A slot binder binds values to slots to create new objects and invoke methods.
    /// </summary>
    public static class SlotBinder
    {
        /// <summary>
        /// Creates an object given values for its slots.
        /// </summary>
        /// <param name="type">The type of object to create</param>
        /// <param name="slotValues">The slot values</param>
        /// <returns>An object factory</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="type"/> or
        /// <paramref name="slotValues"/> is null</exception>
        /// <exception cref="ArgumentException">Thrown if <paramref name="type"/>
        /// is not a concrete class or if the <paramref name="slotValues" /> are invalid</exception>
        public static object CreateInstance(ITypeInfo type, IEnumerable<KeyValuePair<ISlotInfo, object>> slotValues)
        {
            if (type == null)
                throw new ArgumentNullException("type");
            if (slotValues == null)
                throw new ArgumentNullException("slotValues");

            Type resolvedType = type.Resolve(true);
            if (resolvedType.IsInterface || resolvedType.IsAbstract || resolvedType.HasElementType || resolvedType.IsGenericParameter)
                throw new ArgumentException("The type must be a concrete class.", "type");

            List<KeyValuePair<Type, object>> genericParameterBindings = new List<KeyValuePair<Type, object>>();
            List<KeyValuePair<ParameterInfo, object>> constructorParameterBindings = new List<KeyValuePair<ParameterInfo, object>>();
            List<KeyValuePair<FieldInfo, object>> fieldBindings = new List<KeyValuePair<FieldInfo, object>>();
            List<KeyValuePair<PropertyInfo, object>> propertyBindings = new List<KeyValuePair<PropertyInfo, object>>();

            ConstructorInfo constructor = null;
            foreach (KeyValuePair<ISlotInfo, object> slotValue in slotValues)
            {
                ISlotInfo slot = slotValue.Key;
                if (slot == null)
                    throw new ArgumentNullException("slotValues", "The slots in the slot values list must not be null.");

                object value = slotValue.Value;

                switch (slot.Kind)
                {
                    case CodeElementKind.GenericParameter:
                        Type genericParameter = ((IGenericParameterInfo)slot).Resolve(true);
                        EnsureSlotBelongsToClass(resolvedType, genericParameter.DeclaringType, slot);

                        genericParameterBindings.Add(new KeyValuePair<Type, object>(genericParameter, value));
                        break;

                    case CodeElementKind.Parameter:
                        ParameterInfo constructorParameter = ((IParameterInfo)slot).Resolve(true);
                        EnsureSlotBelongsToClass(resolvedType, constructorParameter.Member.DeclaringType, slot);

                        ConstructorInfo possibleConstructor = constructorParameter.Member as ConstructorInfo;
                        if (possibleConstructor == null)
                            throw new ArgumentException(String.Format("The parameter slot '{0}' is not a constructor parameter.", slot), "slotValues");
                        if (constructor == null)
                            constructor = possibleConstructor;
                        else if (!constructor.Equals(possibleConstructor))
                            throw new ArgumentException(String.Format("The parameter slot '{0}' belongs to a different constructor from a previous parameter slot.", slot), "slotValues");

                        constructorParameterBindings.Add(new KeyValuePair<ParameterInfo, object>(constructorParameter, value));
                        break;

                    case CodeElementKind.Property:
                        PropertyInfo property = ((IPropertyInfo) slot).Resolve(true);
                        EnsureSlotBelongsToClass(resolvedType, property.DeclaringType, slot);

                        propertyBindings.Add(new KeyValuePair<PropertyInfo, object>(property, value));
                        break;

                    case CodeElementKind.Field:
                        FieldInfo field = ((IFieldInfo)slot).Resolve(true);
                        EnsureSlotBelongsToClass(resolvedType, field.DeclaringType, slot);

                        fieldBindings.Add(new KeyValuePair<FieldInfo, object>(field, value));
                        break;

                    default:
                        throw new ArgumentException(String.Format("Slot '{0}' is not valid in this context.", slot), "slotValues");
                }
            }

            if (constructor == null)
            {
                // Note: Value types don't have default constructors so we have to create instances a different way.
                if (!resolvedType.IsValueType)
                {
                    constructor = resolvedType.GetConstructor(EmptyArray<Type>.Instance);
                    if (constructor == null)
                        throw new ArgumentException("The bindings do not contain any constructor parameters but the class does not have a default constructor.", "slotValues");
                }
            }

            int constructorParameterCount = constructor != null ? constructor.GetParameters().Length : 0;
            if (constructorParameterCount != constructorParameterBindings.Count)
                throw new ArgumentException(String.Format("The constructor has {0} parameters but the bindings only provide values for {1} of them.",
                    constructorParameterCount, constructorParameterBindings.Count), "slotValues");

            int genericParameterCount = resolvedType.IsGenericTypeDefinition ? resolvedType.GetGenericArguments().Length : 0;
            if (genericParameterCount != genericParameterBindings.Count)
                throw new ArgumentException(String.Format("The type has {0} generic parameters but the bindings only provide values for {1} of them.",
                    genericParameterCount, genericParameterBindings.Count), "slotValues");

            Type[] genericTypeArgs = GetGenericArgs(genericParameterBindings);
            Type typeInstantiation = genericTypeArgs.Length == 0 ? resolvedType : resolvedType.MakeGenericType(genericTypeArgs);

            object instance;
            if (constructor != null)
                instance = CreateInstance(typeInstantiation, constructor, constructorParameterBindings);
            else
                instance = Activator.CreateInstance(typeInstantiation);

            SetFieldValues(typeInstantiation, instance, fieldBindings);
            SetPropertyValues(typeInstantiation, instance, propertyBindings);

            return instance;
        }

        /// <summary>
        /// Invokes a static method given values for its slots.
        /// </summary>
        /// <param name="method">The static method to invoke</param>
        /// <param name="typeInstantiation">The non-generic type or generic type instantiation
        /// that declares the method to be invoked or is a subtype of the declaring type.
        /// This parameter is used to resolve <paramref name="method"/>'s declaring type
        /// to a particular instantiation in the event that it contains unbound generic type
        /// parameters.</param>
        /// <param name="slotValues">The slot values</param>
        /// <returns>The return value of the method</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="method"/>,
        /// <paramref name="typeInstantiation"/> or <paramref name="slotValues"/> is null</exception>
        /// <exception cref="ArgumentException">Thrown if <paramref name="method"/>
        /// is not concrete or if the <paramref name="slotValues"/> are invalid</exception>
        public static object InvokeStaticMethod(IMethodInfo method, Type typeInstantiation, IEnumerable<KeyValuePair<ISlotInfo, object>> slotValues)
        {
            if (method == null)
                throw new ArgumentNullException("method");
            if (typeInstantiation == null)
                throw new ArgumentNullException("typeInstantiation");
            if (slotValues == null)
                throw new ArgumentNullException("slotValues");
            if (! method.IsStatic)
                throw new ArgumentException("The method is not static.", "method");

            MethodInfo resolvedMethod = ResolveMemberOnTypeInstantiation(typeInstantiation, method.Resolve(true));
            return InvokeResolvedMethod(resolvedMethod, null, slotValues);
        }

        /// <summary>
        /// Invokes an instance method given values for its slots.
        /// </summary>
        /// <param name="method">The instance method to invoke</param>
        /// <param name="instance">The instance</param>
        /// <param name="slotValues">The slot values</param>
        /// <returns>The return value of the method</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="method"/>,
        /// <paramref name="instance"/> or <paramref name="slotValues"/> is null</exception>
        /// <exception cref="ArgumentException">Thrown if <paramref name="method"/>
        /// is not concrete or if the <paramref name="slotValues"/> are invalid</exception>
        public static object InvokeInstanceMethod(IMethodInfo method, object instance, IEnumerable<KeyValuePair<ISlotInfo, object>> slotValues)
        {
            if (method == null)
                throw new ArgumentNullException("method");
            if (instance == null)
                throw new ArgumentNullException("instance");
            if (slotValues == null)
                throw new ArgumentNullException("slotValues");
            if (method.IsStatic)
                throw new ArgumentException("The method is static.", "method");

            MethodInfo resolvedMethod = ResolveMemberOnTypeInstantiation(instance.GetType(), method.Resolve(true));
            if (!resolvedMethod.DeclaringType.IsInstanceOfType(instance))
                throw new ArgumentException("The instance is of the wrong type.", "instance");

            return InvokeResolvedMethod(resolvedMethod, instance, slotValues);
        }

        private static object InvokeResolvedMethod(MethodInfo method, object instance, IEnumerable<KeyValuePair<ISlotInfo, object>> slotValues)
        {
            List<KeyValuePair<Type, object>> genericParameterBindings = new List<KeyValuePair<Type, object>>();
            List<KeyValuePair<ParameterInfo, object>> methodParameterBindings = new List<KeyValuePair<ParameterInfo, object>>();

            foreach (KeyValuePair<ISlotInfo, object> slotValue in slotValues)
            {
                ISlotInfo slot = slotValue.Key;
                if (slot == null)
                    throw new ArgumentNullException("slotValues", "The slots in the slot values list must not be null.");

                object value = slotValue.Value;

                switch (slot.Kind)
                {
                    case CodeElementKind.GenericParameter:
                        Type genericParameter = ((IGenericParameterInfo)slot).Resolve(true);
                        EnsureSlotBelongsToMethod(method, genericParameter.DeclaringMethod, slot);

                        genericParameterBindings.Add(new KeyValuePair<Type, object>(genericParameter, value));
                        break;

                    case CodeElementKind.Parameter:
                        ParameterInfo methodParameter = ((IParameterInfo)slot).Resolve(true);
                        EnsureSlotBelongsToMethod(method, methodParameter.Member as MethodInfo, slot);

                        methodParameterBindings.Add(new KeyValuePair<ParameterInfo, object>(methodParameter, value));
                        break;

                    default:
                        throw new ArgumentException(String.Format("Slot '{0}' is not valid in this context.", slot), "slotValues");
                }
            }

            int methodParameterCount = method.GetParameters().Length;
            if (methodParameterCount != methodParameterBindings.Count)
                throw new ArgumentException(String.Format("The method has {0} parameters but the bindings only provide values for {1} of them.",
                    methodParameterBindings, methodParameterBindings.Count), "slotValues");

            int genericParameterCount = method.IsGenericMethodDefinition ? method.GetGenericArguments().Length : 0;
            if (genericParameterCount != genericParameterBindings.Count)
                throw new ArgumentException(String.Format("The method has {0} generic parameters but the bindings only provide values for {1} of them.",
                    genericParameterCount, genericParameterBindings.Count), "slotValues");

            Type[] genericMethodArgs = GetGenericArgs(genericParameterBindings);
            return InvokeMethodWithArgs(method, instance, genericMethodArgs, methodParameterBindings);
        }

        /// <summary>
        /// <para>
        /// Resolves a member that may be declared by a generic type using a particular
        /// instantiation of the type or one of its subtypes.
        /// </para>
        /// <para>
        /// For example, if <paramref name="member"/> was declared by type Foo&lt;T&gt;
        /// and <paramref name="typeInstantiation"/> is a subtype of Foo&lt;int&gt;, returns
        /// a reflection object for the member as declared by Foo&lt;int&gt;.
        /// </para>
        /// </summary>
        /// <typeparam name="T">The type of member</typeparam>
        /// <param name="typeInstantiation">The type instantiation</param>
        /// <param name="member">The member</param>
        /// <returns>The resolved member</returns>
        /// <todo>Decide whether and how to expose this method</todo>
        private static T ResolveMemberOnTypeInstantiation<T>(Type typeInstantiation, T member)
            where T : MemberInfo
        {
            if (typeInstantiation == null)
                throw new ArgumentNullException("typeInstantiation");
            if (member == null)
                throw new ArgumentNullException("member");
            if (typeInstantiation.ContainsGenericParameters)
                throw new ArgumentException("The type instantiation should not contain generic parameters.", "typeInstantiation");

            Type declaringType = member.DeclaringType;
            if (!declaringType.ContainsGenericParameters)
                return member;

            MemberInfo[] resolvedMembers = typeInstantiation.FindMembers(member.MemberType,
                BindingFlags.FlattenHierarchy | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static,
                delegate(MemberInfo candidate, object dummy) { return candidate.MetadataToken == member.MetadataToken; },
                null);

            if (resolvedMembers.Length != 1)
                throw new InvalidOperationException(String.Format("Could not resolve member '{0}' on type instantiation '{1}'.", member, typeInstantiation));

            return (T)resolvedMembers[0];
        }

        private static void EnsureSlotBelongsToClass(Type type, Type declaringTypeOfSlot, ISlotInfo slot)
        {
            if (declaringTypeOfSlot == null || !declaringTypeOfSlot.IsAssignableFrom(type))
                throw new ArgumentException(String.Format("Slot '{0}' was not declared by type '{1}'.", slot, type), "slotValues");
        }

        private static void EnsureSlotBelongsToMethod(MethodBase method, MethodBase declaringMethodOfSlot, ISlotInfo slot)
        {
            if (declaringMethodOfSlot == null || declaringMethodOfSlot.MetadataToken != method.MetadataToken
                || ! declaringMethodOfSlot.Module.Equals(method.Module))
                throw new ArgumentException(String.Format("Slot '{0}' was not declared by method '{1}'.", slot, method.Name), "slotValues");
        }

        private static object CreateInstance(Type typeInstantiation, ConstructorInfo constructor, ICollection<KeyValuePair<ParameterInfo, object>> constructorParameterBindings)
        {
            object[] constructorArgs = GetConstructorOrMethodArgs(constructorParameterBindings);
            return ResolveMemberOnTypeInstantiation(typeInstantiation, constructor).Invoke(constructorArgs);
        }

        private static void SetFieldValues(Type typeInstantiation, object instance, IEnumerable<KeyValuePair<FieldInfo, object>> fieldBindings)
        {
            foreach (KeyValuePair<FieldInfo, object> binding in fieldBindings)
                ResolveMemberOnTypeInstantiation(typeInstantiation, binding.Key).SetValue(instance, binding.Value);
        }

        private static void SetPropertyValues(Type typeInstantiation, object instance, IEnumerable<KeyValuePair<PropertyInfo, object>> propertyBindings)
        {
            foreach (KeyValuePair<PropertyInfo, object> binding in propertyBindings)
                ResolveMemberOnTypeInstantiation(typeInstantiation, binding.Key).SetValue(instance, binding.Value, null);
        }

        private static object InvokeMethodWithArgs(MethodInfo method, object instance, Type[] genericMethodArgs, ICollection<KeyValuePair<ParameterInfo, object>> methodParameterBindings)
        {
            if (genericMethodArgs.Length != 0)
                method = method.MakeGenericMethod(genericMethodArgs);

            object[] methodArgs = GetConstructorOrMethodArgs(methodParameterBindings);
            return method.Invoke(instance, methodArgs);
        }

        private static Type[] GetGenericArgs(ICollection<KeyValuePair<Type, object>> genericParameterBindings)
        {
            int genericParameterCount = genericParameterBindings.Count;
            if (genericParameterCount == 0)
                return EmptyArray<Type>.Instance;

            Type[] genericArgs = new Type[genericParameterCount];
            foreach (KeyValuePair<Type, object> binding in genericParameterBindings)
            {
                Type type = binding.Value as Type;
                if (type == null)
                    throw new ArgumentException(String.Format("Expected a Type to bind to the generic parameter slot '{0}'.", 
                        binding.Key.Name), "slotValues");

                genericArgs[binding.Key.GenericParameterPosition] = (Type)binding.Value;
            }

            return genericArgs;
        }

        private static object[] GetConstructorOrMethodArgs(ICollection<KeyValuePair<ParameterInfo, object>> parameterBindings)
        {
            int parameterCount = parameterBindings.Count;
            if (parameterCount == 0)
                return EmptyArray<object>.Instance;

            object[] constructorOrMethodArgs = new object[parameterCount];
            foreach (KeyValuePair<ParameterInfo, object> binding in parameterBindings)
                constructorOrMethodArgs[binding.Key.Position] = binding.Value;

            return constructorOrMethodArgs;
        }
    }
}
