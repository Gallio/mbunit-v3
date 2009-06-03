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
using System.Reflection;
using System.Text;
using Gallio.Runtime.Conversions;
using Gallio.Runtime.Formatting;
using Gallio.Common.Reflection;

namespace Gallio.Framework.Data
{
    /// <summary>
    /// <para>
    /// A data binding specification describes how values are bound to slots (<see cref="ISlotInfo"/>)
    /// of a type or method.  The specification can then be used to create new objects or invoke
    /// methods.
    /// </para>
    /// <para>
    /// A specification automatically converts values to the correct types
    /// for data binding using a <see cref="IConverter" />.  It can also format
    /// the specification to a string using a <see cref="IFormatter" />.
    /// </para>
    /// </summary>
    /// <seealso cref="ObjectCreationSpec"/>
    public abstract class DataBindingSpec
    {
        private readonly IEnumerable<KeyValuePair<ISlotInfo, object>> slotValues;
        private readonly IConverter converter;

        /// <summary>
        /// Creates a data binding spec.
        /// </summary>
        /// <param name="slotValues">The slot values.</param>
        /// <param name="converter">The converter to use for converting slot values
        /// to the required types.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="slotValues"/>
        /// or <paramref name="converter"/> is null or if <paramref name="slotValues"/>
        /// contains a null slot.</exception>
        protected DataBindingSpec(IEnumerable<KeyValuePair<ISlotInfo, object>> slotValues, IConverter converter)
        {
            if (slotValues == null)
                throw new ArgumentNullException("slotValues");
            if (converter == null)
                throw new ArgumentNullException("converter");

            foreach (KeyValuePair<ISlotInfo, object> slotValue in slotValues)
                if (slotValue.Key == null)
                    throw new ArgumentNullException("slotValues", "Slots must not be null.");

            this.slotValues = slotValues;
            this.converter = converter;
        }

        /// <summary>
        /// Gets the slot values.
        /// </summary>
        public IEnumerable<KeyValuePair<ISlotInfo, object>> SlotValues
        {
            get { return slotValues; }
        }

        /// <summary>
        /// Gets the converter.
        /// </summary>
        public IConverter Converter
        {
            get { return converter; }
        }

        /// <summary>
        /// <para>
        /// Formats the specification to a string for presentation.
        /// </para>
        /// <para>
        /// The values are listed sequentially as follows:
        /// <list type="bullet">
        /// <item>The <paramref name="entity"/>.</item>
        /// <item>The <see cref="IGenericParameterInfo" /> slot values, if any, are ordered by index
        /// and enclosed within angle bracket.</item>
        /// <item>The <see cref="IParameterInfo" /> slot values, if any, are ordered by index
        /// and enclosed within parentheses.</item>
        /// <item>All other slot values, if any, are sorted by name and formatted as name-value
        /// pair assignments following a colon and delimited by a comma</item>
        /// </list>
        /// Example: 'SomeType&lt;int, string&gt;(42, "deep thought"): Author="Douglas Adams", Book="HGTTG"'.
        /// </para>
        /// <para>
        /// If there are no slots of a given kind, then the enclosing angle brackets or
        /// parentheses are ignored.  Therefore if <see cref="SlotValues"/> is empty
        /// then <paramref name="entity"/> will be returned unmodified.
        /// </para>
        /// </summary>
        /// <remarks>
        /// This method assumes that the slots all belong to the same declaring type or method
        /// which is always the case for <see cref="ObjectCreationSpec" /> and <see cref="MethodInvocationSpec" />.
        /// </remarks>
        /// <param name="entity">The entity that is qualified by the specification such as the name of a type or method.</param>
        /// <param name="formatter">The formatter.</param>
        /// <returns>The formatted specification.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="entity"/>
        /// or <paramref name="formatter"/> is null.</exception>
        public string Format(string entity, IFormatter formatter)
        {
            if (entity == null)
                throw new ArgumentNullException("entity");
            if (formatter == null)
                throw new ArgumentNullException("formatter");

            return FormatImpl(entity, formatter);
        }

        /// <summary>
        /// Internal implementation of <see cref="Format" /> after argument validation.
        /// </summary>
        /// <param name="entity">The entity that is qualified by the specification such as the name of a type or method.</param>
        /// <param name="formatter">The formatter, not null.</param>
        /// <returns>The formatted specification.</returns>
        protected abstract string FormatImpl(string entity, IFormatter formatter);

        /// <summary>
        /// Appends formatted generic arguments within angle brackets, if any.
        /// </summary>
        /// <param name="str">The string builder, not null.</param>
        /// <param name="arguments">The arguments, not null.</param>
        /// <param name="formatter">The formatter, not null.</param>
        protected static void AppendFormattedGenericArguments(StringBuilder str, Type[] arguments, IFormatter formatter)
        {
            if (arguments.Length != 0)
            {
                str.Append('<');
                AppendFormattedArguments(str, arguments, formatter);
                str.Append('>');
            }
        }

        /// <summary>
        /// Appends formatted generic arguments within parentheses, if any.
        /// </summary>
        /// <param name="str">The string builder, not null.</param>
        /// <param name="arguments">The arguments, not null.</param>
        /// <param name="formatter">The formatter, not null.</param>
        protected static void AppendFormattedMethodArguments(StringBuilder str, object[] arguments, IFormatter formatter)
        {
            if (arguments.Length != 0)
            {
                str.Append('(');
                AppendFormattedArguments(str, arguments, formatter);
                str.Append(')');
            }
        }

        /// <summary>
        /// Appends formatted values keyed and sorted by name, if any.
        /// This method is used with fields and properties.
        /// </summary>
        /// <param name="str">The string builder, not null.</param>
        /// <param name="namedValues">The named values, not null.</param>
        /// <param name="formatter">The formatter, not null.</param>
        protected static void AppendFormattedNamedValues(StringBuilder str,
            IEnumerable<KeyValuePair<string, object>> namedValues, IFormatter formatter)
        {
            SortedList<string, object> sortedNamedValues = new SortedList<string, object>(StringComparer.InvariantCultureIgnoreCase);
            foreach (KeyValuePair<string, object> entry in namedValues)
                sortedNamedValues.Add(entry.Key, entry.Value);

            if (sortedNamedValues.Count != 0)
            {
                str.Append(": ");

                bool first = true;
                foreach (KeyValuePair<string, object> entry in sortedNamedValues)
                {
                    if (first)
                        first = false;
                    else
                        str.Append(", ");

                    str.Append(entry.Key);
                    str.Append('=');
                    str.Append(formatter.Format(entry.Value));
                }
            }
        }

        private static void AppendFormattedArguments(StringBuilder str, IEnumerable<object> arguments, IFormatter formatter)
        {
            bool first = true;
            foreach (object value in arguments)
            {
                if (first)
                    first = false;
                else
                    str.Append(", ");

                str.Append(formatter.Format(value));
            }
        }

        /// <summary>
        /// <para>
        /// Resolves a member that may be declared by a generic type using the
        /// resolved type or one of its subtypes.
        /// </para>
        /// <para>
        /// For example, if <paramref name="member"/> was declared by type Foo&lt;T&gt;
        /// and <paramref name="resolvedType"/> is a subtype of Foo&lt;int&gt;, returns
        /// a reflection object for the member as declared by Foo&lt;int&gt;.
        /// </para>
        /// </summary>
        /// <typeparam name="T">The type of member.</typeparam>
        /// <param name="resolvedType">The resolved type, not null.</param>
        /// <param name="member">The member, not null.</param>
        /// <returns>The resolved member.</returns>
        protected static T ResolveMember<T>(Type resolvedType, T member)
            where T : MemberInfo
        {
            if (resolvedType.ContainsGenericParameters)
                throw new ArgumentException("The resolved type should not contain generic parameters.", "resolvedType");

            Type declaringType = member.DeclaringType;
            if (!declaringType.ContainsGenericParameters)
                return member;

            Module desiredModule = member.Module;
            int desiredMetadataToken = member.MetadataToken;
            MemberInfo[] resolvedMembers = resolvedType.FindMembers(member.MemberType,
                BindingFlags.FlattenHierarchy | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static,
                delegate(MemberInfo candidate, object dummy) {
                    return candidate.Module == desiredModule
                        && candidate.MetadataToken == desiredMetadataToken;
                },
                null);

            if (resolvedMembers.Length != 1)
                throw new InvalidOperationException(String.Format("Could not resolve member '{0}' on type '{1}'.", member, resolvedType));

            return (T)resolvedMembers[0];
        }
    }
}
