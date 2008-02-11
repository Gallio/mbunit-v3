using System;
using System.Collections.Generic;
using System.Text;
using Gallio.Collections;

namespace Gallio.Reflection.Impl
{
    /// <summary>
    /// A <see cref="StaticReflectionPolicy"/> code element wrapper.
    /// </summary>
    public abstract class StaticCodeElementWrapper : StaticWrapper, ICodeElementInfo
    {
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
            foreach (StaticAttributeWrapper attribute in GetCustomAttributes())
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
                            attributeUsage = GetAttributeUsage(inheritedAttributeType);
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
            return null;
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
        internal static void AppendParameterListToSignature(StringBuilder sig, IList<IParameterInfo> parameters)
        {
            for (int i = 0; i < parameters.Count; i++)
            {
                if (i != 0)
                    sig.Append(@", ");
                sig.Append(GetTypeNameForSignature(parameters[i].ValueType));
            }
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
            return IsPrimitiveForSignature(type) ? type.Name : type.ToString();
        }

        /// <summary>
        /// Determines whether a type is primitive for the purposes of creating a signature.
        /// </summary>
        /// <param name="type">The reflected type</param>
        /// <returns>True if the type is primitive</returns>
        private static bool IsPrimitiveForSignature(ITypeInfo type)
        {
            while (type.ElementType != null)
                type = type.ElementType;

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
                case "System.Void":
                    return true;

                default:
                    return false;
            }
        }

        private static AttributeUsageAttribute GetAttributeUsage(ITypeInfo attributeType)
        {
            AttributeUsageAttribute attributeUsage;

            if (attributeType.FullName == @"System.AttributeUsageAttribute")
            {
                // Note: Avoid indefinite recursion when determining whether AttributeUsageAttribute itself is inheritable.
                attributeUsage = new AttributeUsageAttribute(AttributeTargets.Class);
                attributeUsage.Inherited = true;
            }
            else
            {
                attributeUsage = AttributeUtils.GetAttribute<AttributeUsageAttribute>(attributeType, true);

                if (attributeUsage == null)
                {
                    attributeUsage = new AttributeUsageAttribute(AttributeTargets.All);
                    attributeUsage.Inherited = true;
                }
            }

            return attributeUsage;
        }
    }
}
