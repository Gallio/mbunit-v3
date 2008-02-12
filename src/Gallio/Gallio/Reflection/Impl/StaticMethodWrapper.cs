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
                    return null;

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

        /// <summary>
        /// Returns true if this method overrides another.
        /// </summary>
        public bool IsOverride
        {
            get { return (MethodAttributes & (MethodAttributes.Virtual | MethodAttributes.NewSlot)) == MethodAttributes.Virtual; }
        }

        /// <summary>
        /// Gets the methods that this one overrides or hides.
        /// Only includes overrides that appear on class types, not interfaces.
        /// </summary>
        /// <param name="overridesOnly">If true, only returns overrides</param>
        public IEnumerable<StaticMethodWrapper> GetOverridenOrHiddenMethods(bool overridesOnly)
        {
            if (overridesOnly && !IsOverride)
                yield break;

            foreach (StaticDeclaredTypeWrapper baseType in DeclaringType.GetAllBaseTypes())
            {
                foreach (StaticMethodWrapper other in Policy.GetTypeMethods(baseType))
                {
                    if (HidesMethod(other))
                    {
                        yield return other;

                        if (overridesOnly && !other.IsOverride)
                            yield break;
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// Returns true if this method hides the specified method.
        /// </summary>
        /// <remarks>
        /// This method assumes that <paramref name="other"/> is defined by
        /// a base type of this method's declaring type.  It determines whether
        /// method hiding has taken place based on the method's name and its signature
        /// (when IsHideBySig is true).
        /// </remarks>
        /// <param name="other">The other method</param>
        /// <returns>True if this method hides the other method</returns>
        public bool HidesMethod(StaticMethodWrapper other)
        {
            if (Name != other.Name)
                return false;

            if (! IsHideBySig)
                return true;

            if (GenericArguments.Count != other.GenericArguments.Count)
                return false;

            if (IsGenericMethod)
            {
                IList<StaticGenericParameterWrapper> genericParameters = GenericParameters;
                IList<StaticGenericParameterWrapper> otherGenericParameters = other.GenericParameters;

                if (genericParameters.Count != otherGenericParameters.Count)
                    return false;

                // Note: We perform a substitution on the method parameters to ensure that the
                //       same generic parameter references are used for both.  Any generic method
                //       parameter references that appear in the signature should thus be directly comparable.
                return CompareSignatures(GenericMethodDefinition,
                    other.GenericMethodDefinition.MakeGenericMethod(
                    new CovariantList<StaticGenericParameterWrapper, ITypeInfo>(genericParameters)));
            }

            if (other.IsGenericMethod)
                return false;

            return CompareSignatures(this, other);
        }

        private static bool CompareSignatures(IMethodInfo a, IMethodInfo b)
        {
            IList<IParameterInfo> aParameters = a.Parameters;
            IList<IParameterInfo> bParameters = b.Parameters;

            int parameterCount = aParameters.Count;
            if (parameterCount != bParameters.Count)
                return false;

            for (int i = 0; i < parameterCount; i++)
            {
                if (!aParameters[i].ValueType.Equals(bParameters[i].ValueType))
                    return false;
            }

            return true;
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
            foreach (StaticMethodWrapper element in GetOverridenOrHiddenMethods(true))
                yield return element;
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
