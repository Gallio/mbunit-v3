using System;
using System.Collections.Generic;
using System.Text;
using Gallio.Common.Reflection;
using System.Reflection;

namespace Gallio.MbUnitCppAdapter.Model.Bridge
{
    /// <summary>
    /// A fake member info that represents an MbUnitCpp test case.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Used to fool the Gallio filtering engine so that we can use filters on an MbUnitCpp test tree.
    /// </para>
    /// </remarks>
    /// <seealso cref="Gallio.Model.Filters.MemberFilter{T}"/>
    internal class FakeMemberInfo : IMemberInfo
    {
        private readonly string name;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="name">The name of the MbUnitCpp test.</param>
        public FakeMemberInfo(string name)
        {
            this.name = name;
        }

        /// <inheritdoc />
        public ITypeInfo DeclaringType
        {
            get { return null; }
        }

        /// <inheritdoc />
        public ITypeInfo ReflectedType
        {
            get { throw new NotSupportedException(); }
        }

        /// <inheritdoc />
        public MemberInfo Resolve(bool throwOnError)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc />
        public string Name
        {
            get { return name; }
        }

        /// <inheritdoc />
        public CodeElementKind Kind
        {
            get { throw new NotSupportedException(); }
        }

        /// <inheritdoc />
        public CodeReference CodeReference
        {
            get { return CodeReference.Unknown; }
        }

        /// <inheritdoc />
        public IEnumerable<IAttributeInfo> GetAttributeInfos(ITypeInfo attributeType, bool inherit)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc />
        public bool HasAttribute(ITypeInfo attributeType, bool inherit)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc />
        public IEnumerable<object> GetAttributes(ITypeInfo attributeType, bool inherit)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc />
        public string GetXmlDocumentation()
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc />
        public CodeLocation GetCodeLocation()
        {
            return CodeLocation.Unknown;
        }

        /// <inheritdoc />
        public IReflectionPolicy ReflectionPolicy
        {
            get { throw new NotSupportedException(); }
        }

        /// <inheritdoc />
        public bool Equals(ICodeElementInfo other)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc />
        public bool Equals(IMemberInfo other)
        {
            throw new NotSupportedException();
        }
    }
}
