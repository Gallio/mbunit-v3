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
using System.Text;
using Gallio.Collections;
using Gallio.Framework.Data.Formatters;
using Gallio.Reflection;

namespace Gallio.Framework.Data
{
    /// <summary>
    /// A slot binder binds values to slots to create new objects and invoke methods.
    /// </summary>
    public static class SlotBinder
    {
        /// <summary>
        /// <para>
        /// Formats the slot values to a string for presentation.
        /// </para>
        /// <para>
        /// The values are listed sequentially as follows:
        /// <list type="bullet">
        /// <item>The <see cref="IGenericParameterInfo" /> slot values, if any, are sorted by index
        /// and enclosed within angle bracket.</item>
        /// <item>The <see cref="IParameterInfo" /> slot values, if any, are sorted by index
        /// and enclosed within parentheses.</item>
        /// <item>All other slot values, if any, are sorted by name and formatted as name-value
        /// pairs delimited by equals signs.</item>
        /// </list>
        /// Example: '&lt;int, string&gt;(42, "deep thought"), Book="HGTTG"'.
        /// </para>
        /// <para>
        /// If there are no slots of a given kind, then the enclosing angle brackets or
        /// parentheses are ignored.  Therefore if <paramref name="slotValues"/> is empty,
        /// then an empty string will be returned.
        /// </para>
        /// </summary>
        /// <remarks>
        /// Clearly, this method assumes that the slots all belong to the same
        /// declaring type or method.  If this is not the case, then the formatted output
        /// may not be very intelligible so a different algorithm should be selected instead.
        /// </remarks>
        /// <param name="slotValues">The slot values</param>
        /// <param name="formatter">The formatter</param>
        /// <returns>The formatted slot values</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="slotValues"/>
        /// is null or contains a null slot, or if <paramref name="formatter"/> is null</exception>
        public static string FormatSlotValues(IEnumerable<KeyValuePair<ISlotInfo, object>> slotValues,
            IFormatter formatter)
        {
            if (slotValues == null)
                throw new ArgumentNullException("slotValues");
            if (formatter == null)
                throw new ArgumentNullException("formatter");

            SortedList<int, object> genericParameterValues = new SortedList<int, object>();
            SortedList<int, object> methodParameterValues = new SortedList<int, object>();
            SortedList<string, object> namedParametersValues = new SortedList<string, object>();

            foreach (KeyValuePair<ISlotInfo, object> slotValue in slotValues)
            {
                ISlotInfo slot = slotValue.Key;
                if (slot == null)
                    throw new ArgumentNullException("slotValues", "The slots in the slot values list must not be null.");

                object value = slotValue.Value;
                switch (slot.Kind)
                {
                    case CodeElementKind.GenericParameter:
                        genericParameterValues.Add(slot.Position, value);
                        break;

                    case CodeElementKind.Parameter:
                        methodParameterValues.Add(slot.Position, value);
                        break;

                    case CodeElementKind.Field:
                    case CodeElementKind.Property:
                        namedParametersValues.Add(slot.Name, value);
                        break;

                    default:
                        throw new ArgumentException(String.Format("Slot '{0}' is not valid in this context.", slot), "slotValues");
                }
            }

            StringBuilder str = new StringBuilder();

            if (genericParameterValues.Count != 0)
            {
                str.Append('<');
                AppendFormattedSlotValues(str, genericParameterValues.Values, formatter);
                str.Append('>');
            }

            if (methodParameterValues.Count != 0)
            {
                str.Append('(');
                AppendFormattedSlotValues(str, methodParameterValues.Values, formatter);
                str.Append(')');
            }

            if (namedParametersValues.Count != 0)
            {
                foreach (KeyValuePair<string, object> entry in namedParametersValues)
                {
                    if (str.Length != 0)
                        str.Append(", ");

                    str.Append(entry.Key);
                    str.Append('=');
                    str.Append(formatter.Format(entry.Value));
                }
            }

            return str.ToString();
        }

        private static void AppendFormattedSlotValues(StringBuilder str, IEnumerable<object> values, IFormatter formatter)
        {
            bool first = true;
            foreach (object value in values)
            {
                if (first)
                    first = false;
                else
                    str.Append(", ");

                str.Append(formatter.Format(value));
            }
        }

        /// <summary>
        /// Makes a type given values for its <see cref="IGenericParameterInfo"/> slots, if any.
        /// Ignores <see cref="IParameterInfo" />, <see cref="IPropertyInfo" /> and <see cref="IFieldInfo" /> slots.
        /// </summary>
        /// <param name="type">The type or generic type definition</param>
        /// <param name="slotValues">The slot values</param>
        /// <returns>The type or generic type instantiation</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="type"/> or
        /// <paramref name="slotValues"/> is null</exception>
        /// <exception cref="ArgumentException">Thrown if <paramref name="type"/>
        /// has an element type or if <paramref name="slotValues" /> are invalid</exception>
        public static Type MakeType(ITypeInfo type, IEnumerable<KeyValuePair<ISlotInfo, object>> slotValues)
        {
            if (type == null)
                throw new ArgumentNullException("type");
            if (slotValues == null)
                throw new ArgumentNullException("slotValues");
            EnsureSlotsNonNullAndOfTheRightKinds(slotValues, true);

            Type resolvedType = type.Resolve(true);
            if (resolvedType.HasElementType || resolvedType.IsGenericParameter)
                throw new ArgumentException("The fixture type must not be an array, pointer, reference or generic parameter.", "type");

            return MakeTypeInstantiation(resolvedType, slotValues);
        }

        /// <summary>
        /// Creates an object given values for its slots.
        /// </summary>
        /// <param name="type">The type of object to create</param>
        /// <param name="slotValues">The slot values</param>
        /// <returns>The new object</returns>
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
            EnsureSlotsNonNullAndOfTheRightKinds(slotValues, true);

            Type resolvedType = type.Resolve(true);
            if (resolvedType.IsInterface || resolvedType.IsAbstract || resolvedType.HasElementType || resolvedType.IsGenericParameter)
                throw new ArgumentException("The type must be a concrete class.", "type");

            Type typeInstantiation = MakeTypeInstantiation(resolvedType, slotValues);
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

            object instance;
            if (constructor != null)
                instance = CreateInstance(typeInstantiation, constructor, constructorParameterBindings);
            else
                instance = Activator.CreateInstance(typeInstantiation);

            SetFieldValues(typeInstantiation, instance, fieldBindings);
            SetPropertyValues(typeInstantiation, instance, propertyBindings);

            return instance;
        }

        private static Type MakeTypeInstantiation(Type resolvedType, IEnumerable<KeyValuePair<ISlotInfo, object>> slotValues)
        {
            List<KeyValuePair<Type, object>> genericParameterBindings = new List<KeyValuePair<Type, object>>();
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
                }
            }

            int genericParameterCount = resolvedType.IsGenericTypeDefinition ? resolvedType.GetGenericArguments().Length : 0;
            if (genericParameterCount != genericParameterBindings.Count)
                throw new ArgumentException(String.Format("The type has {0} generic parameters but the bindings only provide values for {1} of them.",
                    genericParameterCount, genericParameterBindings.Count), "slotValues");

            Type[] genericTypeArgs = GetGenericArgs(genericParameterBindings);
            Type typeInstantiation = genericTypeArgs.Length == 0 ? resolvedType : resolvedType.MakeGenericType(genericTypeArgs);
            return typeInstantiation;
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
            EnsureSlotsNonNullAndOfTheRightKinds(slotValues, false);

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
            EnsureSlotsNonNullAndOfTheRightKinds(slotValues, false);

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
                }
            }

            int methodParameterCount = method.GetParameters().Length;
            if (methodParameterCount != methodParameterBindings.Count)
                throw new ArgumentException(String.Format("The method has {0} parameters but the bindings only provide values for {1} of them.",
                    methodParameterCount, methodParameterBindings.Count), "slotValues");

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

        private static void EnsureSlotsNonNullAndOfTheRightKinds(IEnumerable<KeyValuePair<ISlotInfo, object>> slotValues,
            bool isType)
        {
            foreach (KeyValuePair<ISlotInfo, object> slotValue in slotValues)
            {
                ISlotInfo slot = slotValue.Key;
                if (slot == null)
                    throw new ArgumentNullException("slotValues", "The slots in the slot values list must not be null.");

                switch (slot.Kind)
                {
                    case CodeElementKind.GenericParameter:
                    case CodeElementKind.Parameter:
                        continue;

                    case CodeElementKind.Field:
                    case CodeElementKind.Property:
                        if (isType)
                            continue;
                        break;
                }

                throw new ArgumentException(String.Format("Slot '{0}' is not valid in this context.", slot), "slotValues");
            }
        }
    }
}