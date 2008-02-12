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
using System.Globalization;
using System.Reflection;
using System.Text;
using Gallio.Collections;
using Gallio.Utilities;

namespace Gallio.Reflection.Impl
{
    /// <summary>
    /// A <see cref="StaticReflectionPolicy"/> declared type wrapper such as a
    /// class, struct, enum or interface definition.
    /// </summary>
    public sealed class StaticDeclaredTypeWrapper : StaticTypeWrapper
    {
        private readonly Memoizer<TypeAttributes> typeAttriutesMemoizer = new Memoizer<TypeAttributes>();
        private readonly Memoizer<IList<ITypeInfo>> genericArgumentsMemoizer = new Memoizer<IList<ITypeInfo>>();
        private readonly Memoizer<IList<StaticGenericParameterWrapper>> genericParametersMemoizer = new Memoizer<IList<StaticGenericParameterWrapper>>();
        private readonly Memoizer<string> fullNameMemoizer = new Memoizer<string>();
        private readonly Memoizer<string> signatureMemoizer = new Memoizer<string>();

        private readonly StaticTypeSubstitution substitution;

        /// <summary>
        /// Creates a wrapper.
        /// </summary>
        /// <param name="policy">The reflection policy</param>
        /// <param name="handle">The underlying reflection object</param>
        /// <param name="declaringType">The declaring type, or null if none</param>
        /// <param name="substitution">The type substitution for generic parameters</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="policy"/> or <paramref name="handle"/> is null</exception>
        public StaticDeclaredTypeWrapper(StaticReflectionPolicy policy, object handle,
            StaticDeclaredTypeWrapper declaringType, StaticTypeSubstitution substitution)
            : base(policy, handle, declaringType)
        {
            this.substitution = substitution;
        }

        /// <inheritdoc />
        public override StaticTypeSubstitution Substitution
        {
            get { return substitution; }
        }

        /// <inheritdoc />
        public override TypeAttributes TypeAttributes
        {
            get
            {
                return typeAttriutesMemoizer.Memoize(delegate
                {
                    return Policy.GetTypeAttributes(this);
                });
            }
        }

        /// <inheritdoc />
        public override IAssemblyInfo Assembly
        {
            get { return Policy.GetTypeAssembly(this); }
        }

        /// <inheritdoc />
        public override INamespaceInfo Namespace
        {
            get { return Reflector.WrapNamespace(Policy.GetTypeNamespace(this)); }
        }

        /// <summary>
        /// Gets the base type, or null if none.
        /// </summary>
        new public StaticDeclaredTypeWrapper BaseType
        {
            get
            {
                StaticDeclaredTypeWrapper baseType = Policy.GetTypeBaseType(this);
                return baseType != null ? baseType.ComposeSubstitution(Substitution) : null;
            }
        }

        /// <inheritdoc />
        protected override ITypeInfo BaseTypeInternal
        {
            get { return BaseType; }
        }

        /// <inheritdoc />
        public override IList<ITypeInfo> Interfaces
        {
            get { return Substitution.ApplyAll(Policy.GetTypeInterfaces(this)); }
        }

        /// <inheritdoc />
        public override bool IsGenericType
        {
            get { return GenericArguments.Count != 0; }
        }

        /// <inheritdoc />
        public override bool IsGenericTypeDefinition
        {
            get
            {
                IList<StaticGenericParameterWrapper> genericParameters = GenericParameters;
                return genericParameters.Count != 0 && Substitution.DoesNotContainAny(genericParameters);
            }
        }

        /// <inheritdoc />
        public override bool ContainsGenericParameters
        {
            get
            {
                foreach (ITypeInfo type in GenericArguments)
                    if (type.ContainsGenericParameters)
                        return true;
                return false;
            }
        }

        /// <inheritdoc />
        public override IList<ITypeInfo> GenericArguments
        {
	        get 
	        {
                return genericArgumentsMemoizer.Memoize(delegate
                {
                    return Substitution.ApplyAll(GenericParameters);
                });
	        }
        }

        /// <inheritdoc />
        public override ITypeInfo GenericTypeDefinition
        {
            get
            {
                if (!IsGenericType)
                    return null;

                return new StaticDeclaredTypeWrapper(Policy, Handle, DeclaringType, Substitution.Remove(GenericParameters));
            }
        }

        /// <inheritdoc />
        public override StaticDeclaredTypeWrapper MakeGenericType(IList<ITypeInfo> genericArguments)
        {
            if (!IsGenericTypeDefinition)
                throw new InvalidOperationException("The type is not a generic type definition.");

            return new StaticDeclaredTypeWrapper(Policy, Handle, DeclaringType, Substitution.Extend(GenericParameters, genericArguments));
        }
        
        /// <inheritdoc />
        public override IList<IConstructorInfo> GetConstructors(BindingFlags bindingFlags)
        {
            List<IConstructorInfo> result = new List<IConstructorInfo>();

            foreach (StaticConstructorWrapper constructor in Policy.GetTypeConstructors(this))
            {
                if (MatchesBindingFlags(bindingFlags, constructor.IsPublic, constructor.IsStatic))
                    result.Add(constructor);
            }

            return result;
        }

        /// <inheritdoc />
        public override IList<IMethodInfo> GetMethods(BindingFlags bindingFlags)
        {
            List<IMethodInfo> result = new List<IMethodInfo>();
            AddAll(result, EnumerateDeclaredMethods(bindingFlags));

            BindingFlags inheritanceBindingFlags = GetInheritanceBindingFlags(bindingFlags);
            if (inheritanceBindingFlags != BindingFlags.Default)
            {
                HashSet<StaticMethodWrapper> overrides = new HashSet<StaticMethodWrapper>();
                foreach (StaticMethodWrapper method in result)
                    AddAll(overrides, method.GetOverrides());

                foreach (StaticDeclaredTypeWrapper baseType in GetAllBaseTypes())
                {
                    foreach (StaticMethodWrapper inheritedMethod in baseType.EnumerateDeclaredMethods(inheritanceBindingFlags))
                    {
                        if (!overrides.Contains(inheritedMethod) && !IsSpecialNonInheritedMethod(inheritedMethod))
                        {
                            result.Add(inheritedMethod);
                            AddAll(overrides, inheritedMethod.GetOverrides());
                        }
                    }
                }
            }

            return result;
        }
        private IEnumerable<StaticMethodWrapper> EnumerateDeclaredMethods(BindingFlags bindingFlags)
        {
            foreach (StaticMethodWrapper method in Policy.GetTypeMethods(this))
            {
                if (MatchesBindingFlags(bindingFlags, method.IsPublic, method.IsStatic))
                    yield return method;
            }
        }

        /// <inheritdoc />
        public override IList<IPropertyInfo> GetProperties(BindingFlags bindingFlags)
        {
            List<IPropertyInfo> result = new List<IPropertyInfo>();
            AddAll(result, EnumerateProperties(bindingFlags));

            BindingFlags inheritanceBindingFlags = GetInheritanceBindingFlags(bindingFlags);
            if (inheritanceBindingFlags != BindingFlags.Default)
            {
                HashSet<StaticPropertyWrapper> overrides = new HashSet<StaticPropertyWrapper>();
                foreach (StaticPropertyWrapper property in result)
                    AddAll(overrides, property.GetOverrides());

                foreach (StaticDeclaredTypeWrapper baseType in GetAllBaseTypes())
                {
                    foreach (StaticPropertyWrapper inheritedProperty in baseType.EnumerateProperties(inheritanceBindingFlags))
                    {
                        if (!overrides.Contains(inheritedProperty))
                        {
                            result.Add(inheritedProperty);
                            AddAll(overrides, inheritedProperty.GetOverrides());
                        }
                    }
                }
            }

            return result;
        }
        private IEnumerable<StaticPropertyWrapper> EnumerateProperties(BindingFlags bindingFlags)
        {
            foreach (StaticPropertyWrapper property in Policy.GetTypeProperties(this))
            {
                IMethodInfo getMethod = property.GetMethod;
                IMethodInfo setMethod = property.SetMethod;

                bool isPublic = getMethod != null && getMethod.IsPublic
                    || setMethod != null && setMethod.IsPublic;
                bool isStatic = getMethod != null && getMethod.IsStatic
                    || setMethod != null && setMethod.IsStatic;

                if (MatchesBindingFlags(bindingFlags, isPublic, isStatic))
                    yield return property;
            }
        }

        /// <inheritdoc />
        public override IList<IEventInfo> GetEvents(BindingFlags bindingFlags)
        {
            List<IEventInfo> result = new List<IEventInfo>();
            AddAll(result, EnumerateEvents(bindingFlags));

            BindingFlags inheritanceBindingFlags = GetInheritanceBindingFlags(bindingFlags);
            if (inheritanceBindingFlags != BindingFlags.Default)
            {
                HashSet<StaticEventWrapper> overrides = new HashSet<StaticEventWrapper>();
                foreach (StaticEventWrapper @event in result)
                    AddAll(overrides, @event.GetOverrides());

                foreach (StaticDeclaredTypeWrapper baseType in GetAllBaseTypes())
                {
                    foreach (StaticEventWrapper inheritedEvent in baseType.EnumerateEvents(inheritanceBindingFlags))
                    {
                        if (!overrides.Contains(inheritedEvent))
                        {
                            result.Add(inheritedEvent);
                            AddAll(overrides, inheritedEvent.GetOverrides());
                        }
                    }
                }
            }

            return result;
        }
        private IEnumerable<StaticEventWrapper> EnumerateEvents(BindingFlags bindingFlags)
        {
            foreach (StaticEventWrapper @event in Policy.GetTypeEvents(this))
            {
                IMethodInfo addMethod = @event.AddMethod;
                IMethodInfo removeMethod = @event.RemoveMethod;
                IMethodInfo raiseMethod = @event.RaiseMethod;

                bool isPublic = addMethod != null && addMethod.IsPublic
                    || removeMethod != null && removeMethod.IsPublic
                    || raiseMethod != null && raiseMethod.IsPublic;
                bool isStatic = addMethod != null && addMethod.IsStatic
                    || removeMethod != null && removeMethod.IsStatic
                    || raiseMethod != null && raiseMethod.IsStatic;

                if (MatchesBindingFlags(bindingFlags, isPublic, isStatic))
                    yield return @event;
            }
        }

        /// <inheritdoc />
        public override IList<IFieldInfo> GetFields(BindingFlags bindingFlags)
        {
            List<IFieldInfo> result = new List<IFieldInfo>();
            AddAll(result, EnumerateFields(bindingFlags));

            BindingFlags inheritanceBindingFlags = GetInheritanceBindingFlags(bindingFlags);
            if (inheritanceBindingFlags != BindingFlags.Default)
            {
                foreach (StaticDeclaredTypeWrapper baseType in GetAllBaseTypes())
                    AddAll(result, baseType.EnumerateFields(inheritanceBindingFlags));
            }

            return result;
        }
        private IEnumerable<StaticFieldWrapper> EnumerateFields(BindingFlags bindingFlags)
        {
            foreach (StaticFieldWrapper field in Policy.GetTypeFields(this))
            {
                if (MatchesBindingFlags(bindingFlags, field.IsPublic, field.IsStatic))
                    yield return field;
            }
        }

        /// <inheritdoc />
        public override string Name
        {
            get
            {
                string name = base.Name;
                if (IsGenericType)
                    name = string.Concat(name, "`", GenericParameters.Count.ToString(CultureInfo.InvariantCulture));
                return name;
            }
        }

        /// <inheritdoc />
        public override string FullName
        {
            get
            {
                return fullNameMemoizer.Memoize(delegate
                {
                    StringBuilder fullName = new StringBuilder();

                    ITypeInfo declaringType = DeclaringType;
                    if (declaringType != null)
                    {
                        fullName.Append(declaringType.FullName).Append('+');
                    }
                    else
                    {
                        string namespaceName = Namespace.Name;
                        if (namespaceName.Length != 0)
                            fullName.Append(namespaceName).Append('.');
                    }

                    fullName.Append(Name);

                    if (!IsGenericTypeDefinition)
                    {
                        IList<ITypeInfo> genericArguments = GenericArguments;
                        if (genericArguments.Count != 0)
                        {
                            fullName.Append('[');

                            for (int i = 0; i < genericArguments.Count; i++)
                            {
                                if (i != 0)
                                    fullName.Append(',');
                                fullName.Append('[').Append(genericArguments[i].AssemblyQualifiedName).Append(']');
                            }

                            fullName.Append(']');
                        }
                    }

                    return fullName.ToString();
                });
            }
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return signatureMemoizer.Memoize(delegate
            {
                StringBuilder sig = new StringBuilder();

                ITypeInfo declaringType = DeclaringType;
                if (declaringType != null)
                {
                    sig.Append(declaringType).Append('+');
                }
                else
                {
                    string namespaceName = Namespace.Name;
                    if (namespaceName.Length != 0)
                        sig.Append(namespaceName).Append('.');
                }

                sig.Append(Name);

                IList<ITypeInfo> genericArguments = GenericArguments;
                if (genericArguments.Count != 0)
                {
                    sig.Append('[');

                    for (int i = 0; i < genericArguments.Count; i++)
                    {
                        if (i != 0)
                            sig.Append(',');
                        sig.Append(genericArguments[i]);
                    }

                    sig.Append(']');
                }

                return sig.ToString();
            });
        }

        /// <summary>
        /// Gets an enumeration of all base types.
        /// </summary>
        /// <returns>The enumeration of base types</returns>
        public IEnumerable<StaticDeclaredTypeWrapper> GetAllBaseTypes()
        {
            for (StaticDeclaredTypeWrapper baseType = BaseType; baseType != null; baseType = baseType.BaseType)
                yield return baseType;
        }

        /// <summary>
        /// Composes the substitution of the type with the specified substitution and returns a new wrapper.
        /// </summary>
        /// <param name="substitution">The substitution</param>
        /// <returns>The new wrapper with the composed substitution</returns>
        public StaticDeclaredTypeWrapper ComposeSubstitution(StaticTypeSubstitution substitution)
        {
            return new StaticDeclaredTypeWrapper(Policy, Handle, DeclaringType, Substitution.Compose(substitution));
        }

        /// <inheritdoc />
        protected internal override ITypeInfo ApplySubstitution(StaticTypeSubstitution substitution)
        {
            return ComposeSubstitution(substitution);
        }

        private IList<StaticGenericParameterWrapper> GenericParameters
        {
            get
            {
                return genericParametersMemoizer.Memoize(delegate
                {
                    return Policy.GetTypeGenericParameters(this);
                });
            }
        }

        private static bool MatchesBindingFlags(BindingFlags bindingFlags, bool isPublic, bool isStatic)
        {
            if (isPublic)
            {
                if ((bindingFlags & BindingFlags.Public) == 0)
                    return false;
            }
            else
            {
                if ((bindingFlags & BindingFlags.NonPublic) == 0)
                    return false;
            }

            if (isStatic)
            {
                if ((bindingFlags & BindingFlags.Static) == 0)
                    return false;
            }
            else
            {
                if ((bindingFlags & BindingFlags.Instance) == 0)
                    return false;
            }

            return true;
        }

        private static BindingFlags GetInheritanceBindingFlags(BindingFlags bindingFlags)
        {
            if ((bindingFlags & BindingFlags.DeclaredOnly) != 0)
                return BindingFlags.Default;

            BindingFlags newBindingFlags = bindingFlags & (BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

            if ((bindingFlags & (BindingFlags.FlattenHierarchy | BindingFlags.Static)) == (BindingFlags.Static | BindingFlags.FlattenHierarchy))
                newBindingFlags |= BindingFlags.Static;

            return newBindingFlags | BindingFlags.DeclaredOnly;
        }

        /// <summary>
        /// The .Net framework handles several built-in types in a special way.  Basically,
        /// to inheritors, certain 'extern' private methods appear to vanish.  We deal
        /// with those cases uniformly here.  There may yet be others!
        /// </summary>
        private static bool IsSpecialNonInheritedMethod(IMethodInfo method)
        {
            if (!method.IsPrivate)
                return false;

            switch (method.DeclaringType.FullName + ":" + method.Name)
            {
                case "System.Object:FieldGetter":
                case "System.Object:FieldSetter":
                case "System.Object:GetFieldInfo":
                case "System.ValueType:CanCompareBits":
                case "System.ValueType:FastEqualsCheck":
                case "System.Enum:InternalGetValue":
                case "System.Enum:GetValue":
                case "System.Array:InternalGetReference":
                    return true;

                default:
                    return false;
            }
        }

        private static void AddAll<S, T>(ICollection<S> collection, IEnumerable<T> values)
            where T : S
        {
            foreach (T value in values)
                collection.Add(value);
        }
    }
}
