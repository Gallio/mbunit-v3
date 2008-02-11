using System;
using System.Collections.Generic;
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

        /// <inheritdoc />
        public override ITypeInfo BaseType
        {
            get { return Substitution.Apply(Policy.GetTypeBaseType(this)); }
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
                    throw new InvalidOperationException("The type is not generic.");

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

            foreach (StaticMethodWrapper method in Policy.GetTypeMethods(this))
            {
                if (MatchesBindingFlags(bindingFlags, method.IsPublic, method.IsStatic))
                    result.Add(method);
            }

            BindingFlags inheritanceBindingFlags = GetInheritanceBindingFlags(bindingFlags);
            if (inheritanceBindingFlags != BindingFlags.Default)
            {
                foreach (ITypeInfo baseType in GetAllBaseTypes())
                    result.AddRange(baseType.GetMethods(inheritanceBindingFlags));
            }

            return result;
        }

        /// <inheritdoc />
        public override IList<IPropertyInfo> GetProperties(BindingFlags bindingFlags)
        {
            List<IPropertyInfo> result = new List<IPropertyInfo>();

            foreach (StaticPropertyWrapper property in Policy.GetTypeProperties(this))
            {
                IMethodInfo getMethod = property.GetMethod;
                IMethodInfo setMethod = property.SetMethod;

                bool isPublic = getMethod != null && getMethod.IsPublic
                    || setMethod != null && setMethod.IsPublic;
                bool isStatic = getMethod != null && getMethod.IsStatic
                    || setMethod != null && setMethod.IsStatic;

                if (MatchesBindingFlags(bindingFlags, isPublic, isStatic))
                    result.Add(property);
            }

            BindingFlags inheritanceBindingFlags = GetInheritanceBindingFlags(bindingFlags);
            if (inheritanceBindingFlags != BindingFlags.Default)
            {
                foreach (ITypeInfo baseType in GetAllBaseTypes())
                    result.AddRange(baseType.GetProperties(inheritanceBindingFlags));
            }

            return result;
        }

        /// <inheritdoc />
        public override IList<IFieldInfo> GetFields(BindingFlags bindingFlags)
        {
            List<IFieldInfo> result = new List<IFieldInfo>();

            foreach (StaticFieldWrapper field in Policy.GetTypeFields(this))
            {
                if (MatchesBindingFlags(bindingFlags, field.IsPublic, field.IsStatic))
                    result.Add(field);
            }

            BindingFlags inheritanceBindingFlags = GetInheritanceBindingFlags(bindingFlags);
            if (inheritanceBindingFlags != BindingFlags.Default)
            {
                foreach (ITypeInfo baseType in GetAllBaseTypes())
                    result.AddRange(baseType.GetFields(inheritanceBindingFlags));
            }

            return result;
        }

        /// <inheritdoc />
        public override IList<IEventInfo> GetEvents(BindingFlags bindingFlags)
        {
            List<IEventInfo> result = new List<IEventInfo>();

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
                    result.Add(@event);
            }

            BindingFlags inheritanceBindingFlags = GetInheritanceBindingFlags(bindingFlags);
            if (inheritanceBindingFlags != BindingFlags.Default)
            {
                foreach (ITypeInfo baseType in GetAllBaseTypes())
                    result.AddRange(baseType.GetEvents(inheritanceBindingFlags));
            }

            return result;
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

        /// <inheritdoc />
        protected internal override ITypeInfo ApplySubstitution(StaticTypeSubstitution substitution)
        {
            return new StaticDeclaredTypeWrapper(Policy, Handle, DeclaringType, Substitution.Compose(substitution));
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

        private IEnumerable<ITypeInfo> GetAllBaseTypes()
        {
            for (ITypeInfo baseType = BaseType; baseType != null; baseType = baseType.BaseType)
                yield return baseType;
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
    }
}
