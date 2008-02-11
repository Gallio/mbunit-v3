using System;
using System.Collections.Generic;
using System.Reflection;
using Gallio.Utilities;

namespace Gallio.Reflection.Impl
{
    /// <summary>
    /// A <see cref="StaticReflectionPolicy"/> method wrapper.
    /// </summary>
    public sealed class StaticMethodWrapper : StaticFunctionWrapper, IMethodInfo
    {
        private readonly Memoizer<IList<ITypeInfo>> genericArgumentsMemoizer = new Memoizer<IList<ITypeInfo>>();
        private readonly Memoizer<IList<StaticGenericParameterWrapper>> genericParametersMemoizer = new Memoizer<IList<StaticGenericParameterWrapper>>();
        private readonly Memoizer<IParameterInfo> returnParameterMemoizer = new Memoizer<IParameterInfo>();

        private readonly StaticTypeSubstitution substitution;

        /// <summary>
        /// Creates a wrapper.
        /// </summary>
        /// <param name="policy">The reflection policy</param>
        /// <param name="handle">The underlying reflection object</param>
        /// <param name="declaringType">The declaring type</param>
        /// <param name="substitution">The type substitution for generic parameters</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="policy"/>, <paramref name="handle"/>,
        /// or <paramref name="declaringType"/> is null</exception>
        public StaticMethodWrapper(StaticReflectionPolicy policy, object handle, StaticDeclaredTypeWrapper declaringType,
            StaticTypeSubstitution substitution)
            : base(policy, handle, declaringType)
        {
            if (declaringType == null)
                throw new ArgumentNullException("declaringType");

            this.substitution = substitution;
        }

        /// <inheritdoc />
        public override StaticTypeSubstitution Substitution
        {
            get { return substitution; }
        }

        /// <inheritdoc />
        public override CodeElementKind Kind
        {
            get { return CodeElementKind.Method; }
        }

        /// <inheritdoc />
        public bool IsGenericMethod
        {
            get { return GenericArguments.Count != 0; }
        }

        /// <inheritdoc />
        public bool IsGenericMethodDefinition
        {
            get
            {
                IList<StaticGenericParameterWrapper> genericParameters = GenericParameters;
                return genericParameters.Count != 0 && Substitution.DoesNotContainAny(genericParameters);
            }
        }

        /// <inheritdoc />
        public bool ContainsGenericParameters
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
        public IList<ITypeInfo> GenericArguments
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
        public IMethodInfo GenericMethodDefinition
        {
            get
            {
                if (!IsGenericMethod)
                    throw new InvalidOperationException("The method is not generic.");

                return new StaticMethodWrapper(Policy, Handle, DeclaringType, Substitution.Remove(GenericParameters));
            }
        }

        /// <inheritdoc />
        public ITypeInfo ReturnType
        {
            get { return ReturnParameter.ValueType; }
        }

        /// <inheritdoc />
        public IParameterInfo ReturnParameter
        {
            get
            {
                return returnParameterMemoizer.Memoize(delegate
                {
                    return Policy.GetMethodReturnParameter(this);
                });
            }
        }

        /// <inheritdoc />
        public StaticMethodWrapper MakeGenericMethod(IList<ITypeInfo> genericArguments)
        {
            if (!IsGenericMethodDefinition)
                throw new InvalidOperationException("The method is not a generic method definition.");

            return new StaticMethodWrapper(Policy, Handle, DeclaringType, Substitution.Extend(GenericParameters, genericArguments));
        }
        IMethodInfo IMethodInfo.MakeGenericMethod(IList<ITypeInfo> genericArguments)
        {
            return MakeGenericMethod(genericArguments);
        }

        /// <inheritdoc />
        public MethodInfo Resolve(bool throwOnError)
        {
            return ReflectorResolveUtils.ResolveMethod(this, throwOnError);
        }

        /// <inheritdoc />
        protected override MethodBase ResolveMethodBase(bool throwOnError)
        {
            return Resolve(throwOnError);
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return ToString(ReturnType, GenericArguments);
        }

        /// <inheritdoc />
        protected override IEnumerable<ICodeElementInfo> GetInheritedElements()
        {
            return ReflectorInheritanceUtils.EnumerateSuperMethods(this);
        }

        private IList<StaticGenericParameterWrapper> GenericParameters
        {
            get
            {
                return genericParametersMemoizer.Memoize(delegate
                {
                    return Policy.GetMethodGenericParameters(this);
                });
            }
        }
    }
}
