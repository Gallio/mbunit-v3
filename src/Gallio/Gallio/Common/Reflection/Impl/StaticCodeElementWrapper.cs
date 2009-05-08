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
using Gallio.Common.Collections;
using Gallio.Common;

namespace Gallio.Common.Reflection.Impl
{
    /// <summary>
    /// A <see cref="StaticReflectionPolicy"/> code element wrapper.
    /// </summary>
    public abstract class StaticCodeElementWrapper : StaticWrapper, ICodeElementInfo
    {
        private Memoizer<IEnumerable<IAttributeInfo>> allCustomAttributesMemoizer = new Memoizer<IEnumerable<IAttributeInfo>>();

        /// <summary>
        /// Creates a wrapper.
        /// </summary>
        /// <param name="policy">The reflection policy</param>
        /// <param name="handle">The underlying reflection object</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="policy"/> or <paramref name="handle"/> is null</exception>
        public StaticCodeElementWrapper(StaticReflectionPolicy policy, object handle)
            : base(policy, handle)
        {
        }

        /// <inheritdoc />
        public abstract string Name { get; }

        /// <inheritdoc />
        public abstract CodeElementKind Kind { get; }

        /// <inheritdoc />
        public abstract CodeReference CodeReference { get; }

        /// <inheritdoc />
        public IEnumerable<IAttributeInfo> GetAttributeInfos(ITypeInfo attributeType, bool inherit)
        {
            Dictionary<ITypeInfo, AttributeUsageAttribute> attributeUsages = 
                inherit ? new Dictionary<ITypeInfo, AttributeUsageAttribute>() : null;

            // Yield all attributes declared by the member itself.
            // Keep track of which types were seen so we can resolve inherited but non-multiple attributes later.
            string qualifiedTypeName = attributeType != null ? attributeType.FullName : null;
            foreach (IAttributeInfo attribute in GetAllCustomAttributes())
            {
                ITypeInfo type = attribute.Type;
                if (qualifiedTypeName == null || ReflectionUtils.IsDerivedFrom(type, qualifiedTypeName))
                {
                    yield return attribute;

                    if (inherit)
                        attributeUsages[type] = null;
                }
            }

            // Now loop over the inherited member declarations to find inherited attributes.
            // If we see an attribute of a kind we have seen before, then we check whether
            // multiple instances of it are allowed and discard it if not.
            if (inherit)
            {
                foreach (ICodeElementInfo inheritedMember in GetInheritedElements())
                {
                    foreach (IAttributeInfo attribute in inheritedMember.GetAttributeInfos(attributeType, false))
                    {
                        ITypeInfo inheritedAttributeType = attribute.Type;

                        AttributeUsageAttribute attributeUsage;
                        bool seenBefore = attributeUsages.TryGetValue(inheritedAttributeType, out attributeUsage);

                        if (attributeUsage == null)
                        {
                            attributeUsage = Policy.GetAttributeUsage(inheritedAttributeType);
                            attributeUsages[inheritedAttributeType] = attributeUsage;
                        }

                        if (!attributeUsage.Inherited)
                            continue;

                        if (seenBefore && !attributeUsage.AllowMultiple)
                            continue;

                        yield return attribute;
                    }
                }
            }
        }

        /// <inheritdoc />
        public bool HasAttribute(ITypeInfo attributeType, bool inherit)
        {
            return GetAttributeInfos(attributeType, inherit).GetEnumerator().MoveNext();
        }

        /// <inheritdoc />
        public IEnumerable<object> GetAttributes(ITypeInfo attributeType, bool inherit)
        {
            return AttributeUtils.ResolveAttributes(GetAttributeInfos(attributeType, inherit));
        }

        /// <inheritdoc />
        public virtual string GetXmlDocumentation()
        {
            return null;
        }

        /// <inheritdoc />
        public virtual CodeLocation GetCodeLocation()
        {
            return CodeLocation.Unknown;
        }

        /// <inheritdoc />
        public bool Equals(ICodeElementInfo other)
        {
            return Equals((object)other);
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return Name;
        }

        /// <summary>
        /// Gets all attributes that appear on this code element, excluding inherited attributes.
        /// </summary>
        /// <returns>The attribute wrappers</returns>
        protected abstract IEnumerable<StaticAttributeWrapper> GetCustomAttributes();

        /// <summary>
        /// <para>
        /// Gets all pseudo custom attributes associated with a member.
        /// </para>
        /// <para>
        /// These attributes do not really exist as custom attributes in the metadata.  Rather, they are
        /// realizations of other metadata features in attribute form.  For example,
        /// <see cref="SerializableAttribute" /> is represented in the metadata as a <see cref="TypeAttributes" />
        /// flag.  Pseudo custom attributes preserve the illusion of these attributes.
        /// </para>
        /// </summary>
        /// <returns>The pseudo custom attributes</returns>
        protected abstract IEnumerable<Attribute> GetPseudoCustomAttributes();

        /// <summary>
        /// Gets an enumeration of elements from which this code element inherits.
        /// </summary>
        /// <returns>The inherited code elements</returns>
        protected virtual IEnumerable<ICodeElementInfo> GetInheritedElements()
        {
            return EmptyArray<ICodeElementInfo>.Instance;
        }

        /// <summary>
        /// Appends a list of parameters to a signature.
        /// </summary>
        internal static void AppendParameterListToSignature(StringBuilder sig, IList<StaticParameterWrapper> parameters, bool isVarArgs)
        {
            for (int i = 0; i < parameters.Count; i++)
            {
                if (i != 0)
                    sig.Append(@", ");

                sig.Append(GetTypeNameForSignature(parameters[i].ValueType));
            }

            if (isVarArgs)
                sig.Append(@", ...");
        }

        /// <summary>
        /// Appends a list of generic parameters to a signature.
        /// </summary>
        internal static void AppendGenericArgumentListToSignature(StringBuilder sig, IList<ITypeInfo> genericArguments)
        {
            if (genericArguments.Count != 0)
            {
                sig.Append('[');

                for (int i = 0; i < genericArguments.Count; i++)
                {
                    if (i != 0)
                        sig.Append(',');
                    sig.Append(GetTypeNameForSignature(genericArguments[i]));
                }

                sig.Append(']');
            }
        }

        /// <summary>
        /// Gets the name of a type for use in a ToString signature of an event, field,
        /// property, method, or constructor.
        /// </summary>
        internal static string GetTypeNameForSignature(ITypeInfo type)
        {
            return (ShouldUseShortNameForSignature(type) ? type.Name : type.ToString()).Replace("&", " ByRef");
        }

        /// <summary>
        /// Determines whether a type should be represented by its short name
        /// for the purposes of creating a signature.
        /// </summary>
        /// <param name="type">The reflected type</param>
        /// <returns>True if the type is primitive</returns>
        private static bool ShouldUseShortNameForSignature(ITypeInfo type)
        {
            while (type.ElementType != null)
                type = type.ElementType;

            if (type.IsNested)
                return true;

            switch (type.FullName)
            {
                case "System.Boolean":
                case "System.Byte":
                case "System.Char":
                case "System.Double":
                case "System.Int16":
                case "System.Int32":
                case "System.Int64":
                case "System.SByte":
                case "System.Single":
                case "System.UInt16":
                case "System.UInt32":
                case "System.UInt64":
                case "System.IntPtr":
                case "System.UIntPtr":
                case "System.Void":
                    return true;
            }

            return false;
        }

        private IEnumerable<IAttributeInfo> GetAllCustomAttributes()
        {
            return allCustomAttributesMemoizer.Memoize(() =>
            {
                List<IAttributeInfo> attributes = new List<IAttributeInfo>();

                foreach (StaticAttributeWrapper attributeWrapper in GetCustomAttributes())
                    attributes.Add(attributeWrapper);

                foreach (Attribute attribute in GetPseudoCustomAttributes())
                    attributes.Add(Reflector.Wrap(attribute));

                return attributes;
            });
        }
    }
}
