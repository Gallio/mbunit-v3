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
using Gallio.Common.Collections;

namespace Gallio.Common.Reflection.Impl
{
    /// <summary>
    /// A type substitution specifies how generic parameters are replaced by other types.
    /// It is used by implementors of <see cref="StaticReflectionPolicy" /> when returning
    /// types that may be represented as generic parameters.
    /// </summary>
    public struct StaticTypeSubstitution : IEquatable<StaticTypeSubstitution>
    {
        private readonly IDictionary<StaticGenericParameterWrapper, ITypeInfo> replacements;

        /// <summary>
        /// Gets the empty type substitution.
        /// </summary>
        public static readonly StaticTypeSubstitution Empty = new StaticTypeSubstitution(EmptyDictionary<StaticGenericParameterWrapper, ITypeInfo>.Instance);

        private StaticTypeSubstitution(IDictionary<StaticGenericParameterWrapper, ITypeInfo> replacements)
        {
            this.replacements = replacements;
        }

        /// <summary>
        /// Returns true if the type substitution does not contain any replacements.
        /// </summary>
        public bool IsEmpty
        {
            get { return replacements.Count == 0; }
        }

        /// <summary>
        /// Applies a type substitution to the specified type.
        /// </summary>
        /// <param name="type">The type to substitute.</param>
        /// <returns>The substituted type</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="type"/> is null.</exception>
        public ITypeInfo Apply(StaticTypeWrapper type)
        {
            if (type == null)
                throw new ArgumentNullException("type");

            if (IsEmpty)
                return type;

            return type.ApplySubstitution(this);
        }

        /// <summary>
        /// Applies a type substitution to the specified generic parameter.
        /// </summary>
        /// <param name="type">The generic parameter to substitute.</param>
        /// <returns>The substituted type</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="type"/> is null.</exception>
        public ITypeInfo Apply(StaticGenericParameterWrapper type)
        {
            if (type == null)
                throw new ArgumentNullException("type");

            ITypeInfo replacement;
            if (replacements.TryGetValue(type, out replacement))
                return replacement;

            return type;
        }

        /// <summary>
        /// Applies a type substitution to the specified list of types.
        /// </summary>
        /// <param name="types">The types to substitute.</param>
        /// <returns>The substituted types</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="types"/> is null.</exception>
        public IList<ITypeInfo> ApplyAll<T>(IList<T> types)
            where T : StaticTypeWrapper
        {
            if (types == null)
                throw new ArgumentNullException("types");

            if (IsEmpty)
                return new CovariantList<T, ITypeInfo>(types);

            int count = types.Count;
            ITypeInfo[] result = new ITypeInfo[count];
            for (int i = 0; i < count; i++)
                result[i] = types[i].ApplySubstitution(this);

            return result;
        }

        /// <summary>
        /// Returns a new substitution with the specified generic parameters replaced by their respective generic arguments.
        /// </summary>
        /// <remarks>
        /// The extended type substitution is normalized so that generic parameters that
        /// are idempotently replaced with themselves are excluded from the substitution altogether.
        /// </remarks>
        /// <param name="genericParameters">The generic parameters.</param>
        /// <param name="genericArguments">The generic arguments.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="genericParameters"/> or <paramref name="genericArguments"/> is null
        /// or contain nulls.</exception>
        /// <exception cref="ArgumentException">Thrown if <paramref name="genericArguments"/> does not have the same
        /// number of elements as <paramref name="genericParameters"/></exception>
        public StaticTypeSubstitution Extend(IList<StaticGenericParameterWrapper> genericParameters, IList<ITypeInfo> genericArguments)
        {
            if (genericParameters == null)
                throw new ArgumentNullException("genericParameters");
            if (genericArguments == null)
                throw new ArgumentNullException("genericArguments");

            int count = genericParameters.Count;
            if (genericArguments.Count != count)
                throw new ArgumentException("The generic argument count does not equal the generic parameter count.", "genericArguments");

            if (count == 0)
                return this;

            Dictionary<StaticGenericParameterWrapper, ITypeInfo> newReplacements = new Dictionary<StaticGenericParameterWrapper, ITypeInfo>(replacements);
            for (int i = 0; i < count; i++)
            {
                StaticGenericParameterWrapper genericParameter = genericParameters[i];
                if (genericParameter == null)
                    throw new ArgumentNullException("genericParameters", "The generic parameters list should not contain null values.");

                ITypeInfo genericArgument = genericArguments[i];
                if (genericArgument == null)
                    throw new ArgumentNullException("genericArguments", "The generic arguments list should not contain null values.");

                if (! genericParameter.Equals(genericArgument))
                    newReplacements[genericParameter] = genericArgument;
            }

            return new StaticTypeSubstitution(newReplacements);
        }

        /// <summary>
        /// Returns a new substitution formed by composing this substitution with the specified one.
        /// That is to say, each replacement type in this substitution is replaced as described
        /// in the specified substitution.
        /// </summary>
        /// <param name="substitution">The substitution to compose.</param>
        /// <returns>The new substitution</returns>
        public StaticTypeSubstitution Compose(StaticTypeSubstitution substitution)
        {
            if (substitution.IsEmpty)
                return this;

            Dictionary<StaticGenericParameterWrapper, ITypeInfo> newReplacements = new Dictionary<StaticGenericParameterWrapper, ITypeInfo>(replacements.Count);
            foreach (KeyValuePair<StaticGenericParameterWrapper, ITypeInfo> entry in replacements)
            {
                StaticTypeWrapper replacementType = entry.Value as StaticTypeWrapper;
                if (replacementType != null)
                    newReplacements.Add(entry.Key, substitution.Apply(replacementType));
                else
                    newReplacements.Add(entry.Key, entry.Value);
            }

            return new StaticTypeSubstitution(newReplacements);
        }

        /// <summary>
        /// Returns true if this substitution does not contain any of the specified generic parameters.
        /// </summary>
        /// <param name="genericParameters">The generic parameters.</param>
        /// <returns>True if none of the generic parameters are in the substitution</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="genericParameters"/> is null.</exception>
        public bool DoesNotContainAny(IList<StaticGenericParameterWrapper> genericParameters)
        {
            if (genericParameters == null)
                throw new ArgumentNullException("genericParameters");

            foreach (StaticGenericParameterWrapper genericParameter in genericParameters)
                if (replacements.ContainsKey(genericParameter))
                    return false;

            return true;
        }

        /// <summary>
        /// Compares two static type substitutions for equality.
        /// </summary>
        /// <param name="a">The first substitution.</param>
        /// <param name="b">The second substitution.</param>
        /// <returns>True if the substitutions are equal</returns>
        public static bool operator==(StaticTypeSubstitution a, StaticTypeSubstitution b)
        {
            return a.Equals(b);
        }

        /// <summary>
        /// Compares two static type substitutions for inequality.
        /// </summary>
        /// <param name="a">The first substitution.</param>
        /// <param name="b">The second substitution.</param>
        /// <returns>True if the substitutions are equal</returns>
        public static bool operator !=(StaticTypeSubstitution a, StaticTypeSubstitution b)
        {
            return ! a.Equals(b);
        }

        /// <inheritdoc />
        public bool Equals(StaticTypeSubstitution other)
        {
            return replacements == other.replacements || GenericCollectionUtils.KeyValuePairsEqual(replacements, other.replacements);
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            return obj is StaticTypeSubstitution && Equals((StaticTypeSubstitution)obj);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            // Note: This operation is not currently used.
            return 0;
        }
    }
}
