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
            if (! resolvedType.IsClass || resolvedType.IsAbstract)
                throw new ArgumentException("The type must be a concrete class.", "type");

            List<KeyValuePair<Type, object>> genericParameterBindings = new List<KeyValuePair<Type, object>>();
            List<KeyValuePair<ParameterInfo, object>> constructorParameterBindings = new List<KeyValuePair<ParameterInfo, object>>();
            List<KeyValuePair<FieldInfo, object>> fieldBindings = new List<KeyValuePair<FieldInfo, object>>();
            List<KeyValuePair<PropertyInfo, object>> propertyBindings = new List<KeyValuePair<PropertyInfo, object>>();

            ConstructorInfo constructor = null;
            foreach (KeyValuePair<ISlotInfo, object> slotValue in slotValues)
            {
                ISlotInfo slot = slotValue.Key;
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
                constructor = resolvedType.GetConstructor(EmptyArray<Type>.Instance);
                if (constructor == null)
                    throw new ArgumentException("The bindings do not contain any constructor parameters but the class does not have a default constructor.", "slotValues");
            }

            int constructorParameterCount = constructor.GetParameters().Length;
            if (constructorParameterCount != constructorParameterBindings.Count)
                throw new ArgumentException(String.Format("The constructor has {0} parameters but the bindings only provide factories for {1} of them.",
                    constructorParameterCount, constructorParameterBindings.Count), "slotValues");

            int genericParameterCount = resolvedType.IsGenericTypeDefinition ? resolvedType.GetGenericArguments().Length : 0;
            if (genericParameterCount != genericParameterBindings.Count)
                throw new ArgumentException(String.Format("The type has {0} generic parameters but the bindings only provide factories for {1} of them.",
                    genericParameterCount, genericParameterBindings.Count), "slotValues");

            Type[] genericTypeArgs = GetGenericArgs(genericParameterBindings);
            object instance = CreateInstance(constructor, constructorParameterBindings, genericTypeArgs);
            SetFieldValues(instance, fieldBindings, genericTypeArgs);
            SetPropertyValues(instance, propertyBindings, genericTypeArgs);

            return instance;
        }

        /// <summary>
        /// Invokes a method given values for its slots.
        /// </summary>
        /// <param name="method">The method to invoke</param>
        /// <param name="instance">The instance or null if the method is static</param>
        /// <param name="slotValues">The slot values</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="method"/> 
        /// or <paramref name="slotValues"/> is null</exception>
        /// <exception cref="ArgumentException">Thrown if <paramref name="method"/>
        /// is not concrete or if the <paramref name="slotValues"/> are invalid</exception>
        public static void InvokeMethod(IMethodInfo method, object instance, IEnumerable<KeyValuePair<ISlotInfo, object>> slotValues)
        {
            if (method == null)
                throw new ArgumentNullException("method");
            if (slotValues == null)
                throw new ArgumentNullException("slotValues");

            MethodInfo resolvedMethod = method.Resolve(true);
            if (resolvedMethod.IsAbstract)
                throw new ArgumentException("The method must not be abstract.", "method");
            if (resolvedMethod.IsStatic && (instance == null))
                throw new ArgumentException("The instance should be null if and only if the method is static.", "instance");

            List<KeyValuePair<Type, object>> genericParameterBindings = new List<KeyValuePair<Type, object>>();
            List<KeyValuePair<ParameterInfo, object>> methodParameterBindings = new List<KeyValuePair<ParameterInfo, object>>();

            foreach (KeyValuePair<ISlotInfo, object> slotValue in slotValues)
            {
                ISlotInfo slot = slotValue.Key;
                object value = slotValue.Value;

                switch (slot.Kind)
                {
                    case CodeElementKind.GenericParameter:
                        Type genericParameter = ((IGenericParameterInfo)slot).Resolve(true);
                        EnsureSlotBelongsToMethod(resolvedMethod, genericParameter.DeclaringMethod, slot);

                        genericParameterBindings.Add(new KeyValuePair<Type, object>(genericParameter, value));
                        break;

                    case CodeElementKind.Parameter:
                        ParameterInfo methodParameter = ((IParameterInfo)slot).Resolve(true);
                        EnsureSlotBelongsToMethod(resolvedMethod, methodParameter.Member as MethodInfo, slot);

                        methodParameterBindings.Add(new KeyValuePair<ParameterInfo, object>(methodParameter, value));
                        break;

                    default:
                        throw new ArgumentException(String.Format("Slot '{0}' is not valid in this context.", slot), "slotValues");
                }
            }

            Type[] genericMethodArgs = GetGenericArgs(genericParameterBindings);
            InvokeMethod(resolvedMethod, instance, methodParameterBindings, genericMethodArgs);
        }

        private static void EnsureSlotBelongsToClass(Type type, Type declaringTypeOfSlot, ISlotInfo slot)
        {
            if (declaringTypeOfSlot == null || !declaringTypeOfSlot.IsAssignableFrom(type))
                throw new ArgumentException(String.Format("Slot '{0}' was not declared by type '{1}'.", slot, type.FullName), "slotValues");
        }

        private static void EnsureSlotBelongsToMethod(MethodBase method, MethodBase declaringMethodOfSlot, ISlotInfo slot)
        {
            if (declaringMethodOfSlot == null || !declaringMethodOfSlot.Equals(method))
                throw new ArgumentException(String.Format("Slot '{0}' was not declared by method '{1}'.", slot, method.Name), "slotValues");
        }

        private static object CreateInstance(ConstructorInfo constructor, ICollection<KeyValuePair<ParameterInfo, object>> constructorParameterBindings, Type[] genericTypeArgs)
        {
            if (genericTypeArgs.Length != 0)
                constructor = (ConstructorInfo) constructor.Module.ResolveMethod(constructor.MetadataToken, genericTypeArgs, null);

            object[] constructorArgs = GetConstructorOrMethodArgs(constructorParameterBindings);
            return constructor.Invoke(constructorArgs);
        }

        private static void SetFieldValues(object instance, IEnumerable<KeyValuePair<FieldInfo, object>> fieldBindings, Type[] genericTypeArgs)
        {
            foreach (KeyValuePair<FieldInfo, object> binding in fieldBindings)
            {
                FieldInfo field = binding.Key;
                if (genericTypeArgs.Length != 0)
                    field = field.Module.ResolveField(field.MetadataToken, genericTypeArgs, null);

                field.SetValue(instance, binding.Value);
            }
        }

        private static void SetPropertyValues(object instance, IEnumerable<KeyValuePair<PropertyInfo, object>> propertyBindings, Type[] genericTypeArgs)
        {
            foreach (KeyValuePair<PropertyInfo, object> binding in propertyBindings)
            {
                PropertyInfo property = binding.Key;
                if (genericTypeArgs.Length != 0)
                    property = (PropertyInfo) property.Module.ResolveMember(property.MetadataToken, genericTypeArgs, null);

                property.SetValue(instance, binding.Value, null);
            }
        }

        private static void InvokeMethod(MethodInfo method, object instance, ICollection<KeyValuePair<ParameterInfo, object>> methodParameterBindings, Type[] genericMethodArgs)
        {
            if (genericMethodArgs.Length != 0)
                method = method.MakeGenericMethod(genericMethodArgs);

            object[] methodArgs = GetConstructorOrMethodArgs(methodParameterBindings);
            method.Invoke(instance, methodArgs);
        }

        private static Type[] GetGenericArgs(ICollection<KeyValuePair<Type, object>> genericParameterBindings)
        {
            int genericParameterCount = genericParameterBindings.Count;
            if (genericParameterCount == 0)
                return EmptyArray<Type>.Instance;

            Type[] genericArgs = new Type[genericParameterCount];
            foreach (KeyValuePair<Type, object> binding in genericParameterBindings)
                genericArgs[binding.Key.GenericParameterPosition] = (Type)binding.Value;

            return genericArgs;
        }

        private static object[] GetConstructorOrMethodArgs(ICollection<KeyValuePair<ParameterInfo, object>> parameterBindings)
        {
            int parameterCount = parameterBindings.Count;
            if (parameterCount == 0)
                return EmptyArray<object>.Instance;

            object[] constructorOrMethodArgs = new Type[parameterCount];
            foreach (KeyValuePair<ParameterInfo, object> binding in parameterBindings)
                constructorOrMethodArgs[binding.Key.Position] = binding.Value;

            return constructorOrMethodArgs;
        }
    }
}
