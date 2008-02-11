using System;
using System.Collections.Generic;
using System.Reflection;
using Gallio.Collections;
using Gallio.Utilities;

namespace Gallio.Reflection.Impl
{
    /// <summary>
    /// A <see cref="StaticReflectionPolicy"/> generic parameter wrapper.
    /// </summary>
    public sealed class StaticGenericParameterWrapper : StaticDelegatingTypeWrapper, IGenericParameterInfo
    {
        private readonly Memoizer<GenericParameterAttributes> genericParameterAttributesMemoizer = new Memoizer<GenericParameterAttributes>();
        private readonly Memoizer<IList<ITypeInfo>> constraintsMemoizer = new Memoizer<IList<ITypeInfo>>();

        private readonly StaticMethodWrapper declaringMethod;

        /// <summary>
        /// Creates a wrapper.
        /// </summary>
        /// <param name="policy">The reflection policy</param>
        /// <param name="handle">The underlying reflection object</param>
        /// <param name="declaringType">The declaring type, or null if none</param>
        /// <param name="declaringMethod">The declaring method, or null if none</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="policy"/> or <paramref name="handle"/> is null</exception>
        /// <exception cref="ArgumentException">Thrown if <paramref name="declaringType"/> or <paramref name="declaringMethod"/>
        /// are either both null or both non-null</exception>
        public StaticGenericParameterWrapper(StaticReflectionPolicy policy, object handle,
            StaticDeclaredTypeWrapper declaringType, StaticMethodWrapper declaringMethod)
            : base(policy, handle, declaringType)
        {
            if (declaringType == null && declaringMethod == null
                || declaringType != null && declaringMethod != null)
                throw new ArgumentException("Either declaringType or declaringMethod must be null but not both.");

            this.declaringMethod = declaringMethod;
        }

        /// <summary>
        /// Gets the declaring method, or null if none.
        /// </summary>
        public StaticMethodWrapper DeclaringMethod
        {
            get { return declaringMethod; }
        }
        IMethodInfo IGenericParameterInfo.DeclaringMethod
        {
            get { return DeclaringMethod; }
        }

        /// <inheritdoc />
        public override CodeElementKind Kind
        {
            get { return CodeElementKind.GenericParameter; }
        }

        /// <inheritdoc />
        public override ITypeInfo BaseType
        {
            get { return Reflector.Wrap(typeof(Object)); }
        }

        /// <inheritdoc />
        protected override ITypeInfo EffectiveType
        {
            get { return Reflector.Wrap(typeof(Object)); }
        }

        /// <inheritdoc />
        public override bool IsGenericParameter
        {
            get { return true; }
        }

        /// <inheritdoc />
        public override bool ContainsGenericParameters
        {
            get { return true; }
        }

        /// <inheritdoc />
        public override string FullName
        {
            get { return null; }
        }

        /// <inheritdoc />
        public override string AssemblyQualifiedName
        {
            get { return null; }
        }

        /// <inheritdoc />
        public override IAssemblyInfo Assembly
        {
            get { return UltimateDeclaringType.Assembly; }
        }

        /// <inheritdoc />
        public override INamespaceInfo Namespace
        {
            get { return UltimateDeclaringType.Namespace; }
        }

        /// <inheritdoc />
        public override TypeAttributes TypeAttributes
        {
            get { return TypeAttributes.Public | TypeAttributes.Class; }
        }

        /// <inheritdoc />
        public GenericParameterAttributes GenericParameterAttributes
        {
            get
            {
                return genericParameterAttributesMemoizer.Memoize(delegate
                {
                    return Policy.GetGenericParameterAttributes(this);
                });
            }
        }

        /// <inheritdoc />
        public IList<ITypeInfo> Constraints
        {
            get
            {
                return constraintsMemoizer.Memoize(delegate
                {
                    return new CovariantList<StaticTypeWrapper, ITypeInfo>(Policy.GetGenericParameterConstraints(this));
                });
            }
        }

        /// <inheritdoc />
        public ITypeInfo ValueType
        {
            get { return Reflector.Wrap(typeof(Type)); }
        }

        /// <inheritdoc />
        public int Position
        {
            get { return Policy.GetGenericParameterPosition(this); }
        }

        /// <inheritdoc />
        public override IList<IConstructorInfo> GetConstructors(BindingFlags bindingFlags)
        {
            return EmptyArray<IConstructorInfo>.Instance;
        }

        /// <inheritdoc />
        protected internal override ITypeInfo ApplySubstitution(StaticTypeSubstitution substitution)
        {
            return substitution.Apply(this);
        }

        /// <inheritdoc />
        public bool Equals(ISlotInfo other)
        {
            return Equals((object)other);
        }

        /// <inheritdoc />
        public bool Equals(IGenericParameterInfo other)
        {
            return Equals((object)other);
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return Name;
        }

        private ITypeInfo UltimateDeclaringType
        {
            get { return DeclaringType ?? DeclaringMethod.DeclaringType; }
        }
    }
}
