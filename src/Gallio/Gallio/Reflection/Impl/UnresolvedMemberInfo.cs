using System;
using System.Reflection;
using Gallio.Collections;

/*
 * This compilation unit contains MemberInfo overrides that must be duplicated for each
 * of the unresolved reflection types because C# does not support multiple inheritance.
 */

namespace Gallio.Reflection.Impl
{
    internal static class UnresolvedMemberInfo
    {
        public static Type GetDeclaringType(IMemberInfo member)
        {
            return ResolveType(member.DeclaringType);
        }

        public static MethodInfo ResolveMethod(IMethodInfo method, bool nonPublic)
        {
            return method != null && (nonPublic || method.IsPublic) ? method.Resolve(false) : null;
        }

        public static Type ResolveType(ITypeInfo type)
        {
            return type != null ? type.Resolve(false) : null;
        }
    }

    public partial class UnresolvedType
    {
        /// <inheritdoc />
        public override Type DeclaringType
        {
            get { return UnresolvedMemberInfo.GetDeclaringType(adapter); }
        }

        /// <inheritdoc />
        public override int MetadataToken
        {
            get { throw new NotSupportedException(); }
        }

        /// <inheritdoc />
        public override Module Module
        {
            get { throw new NotSupportedException(); }
        }

        /// <inheritdoc />
        public override string Name
        {
            get { return adapter.Name; }
        }

        /// <inheritdoc />
        public override Type ReflectedType
        {
            get { return DeclaringType; }
        }

        /// <inheritdoc />
        public override bool Equals(object o)
        {
            UnresolvedType other = o as UnresolvedType;
            return other != null && adapter.Equals(other.adapter);
        }

        /// <inheritdoc />
        public override object[] GetCustomAttributes(bool inherit)
        {
            return GenericUtils.ToArray(adapter.GetAttributes(inherit));
        }

        /// <inheritdoc />
        public override object[] GetCustomAttributes(Type attributeType, bool inherit)
        {
            return GenericUtils.ToArray(adapter.GetAttributes(attributeType, inherit));
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return adapter.GetHashCode();
        }

        /// <inheritdoc />
        public override bool IsDefined(Type attributeType, bool inherit)
        {
            return adapter.HasAttribute(attributeType, inherit);
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return adapter.ToString();
        }
    }

    public partial class UnresolvedFieldInfo
    {
        /// <inheritdoc />
        public override Type DeclaringType
        {
            get { return UnresolvedMemberInfo.GetDeclaringType(adapter); }
        }

        /// <inheritdoc />
        public override int MetadataToken
        {
            get { throw new NotSupportedException(); }
        }

        /// <inheritdoc />
        public override Module Module
        {
            get { throw new NotSupportedException(); }
        }

        /// <inheritdoc />
        public override string Name
        {
            get { return adapter.Name; }
        }

        /// <inheritdoc />
        public override Type ReflectedType
        {
            get { return DeclaringType; }
        }

        /// <inheritdoc />
        public override bool Equals(object o)
        {
            UnresolvedFieldInfo other = o as UnresolvedFieldInfo;
            return other != null && adapter.Equals(other.adapter);
        }

        /// <inheritdoc />
        public override object[] GetCustomAttributes(bool inherit)
        {
            return GenericUtils.ToArray(adapter.GetAttributes(inherit));
        }

        /// <inheritdoc />
        public override object[] GetCustomAttributes(Type attributeType, bool inherit)
        {
            return GenericUtils.ToArray(adapter.GetAttributes(attributeType, inherit));
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return adapter.GetHashCode();
        }

        /// <inheritdoc />
        public override bool IsDefined(Type attributeType, bool inherit)
        {
            return adapter.HasAttribute(attributeType, inherit);
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return adapter.ToString();
        }
    }

    public partial class UnresolvedPropertyInfo
    {
        /// <inheritdoc />
        public override Type DeclaringType
        {
            get { return UnresolvedMemberInfo.GetDeclaringType(adapter); }
        }

        /// <inheritdoc />
        public override int MetadataToken
        {
            get { throw new NotSupportedException(); }
        }

        /// <inheritdoc />
        public override Module Module
        {
            get { throw new NotSupportedException(); }
        }

        /// <inheritdoc />
        public override string Name
        {
            get { return adapter.Name; }
        }

        /// <inheritdoc />
        public override Type ReflectedType
        {
            get { return DeclaringType; }
        }

        /// <inheritdoc />
        public override bool Equals(object o)
        {
            UnresolvedPropertyInfo other = o as UnresolvedPropertyInfo;
            return other != null && adapter.Equals(other.adapter);
        }

        /// <inheritdoc />
        public override object[] GetCustomAttributes(bool inherit)
        {
            return GenericUtils.ToArray(adapter.GetAttributes(inherit));
        }

        /// <inheritdoc />
        public override object[] GetCustomAttributes(Type attributeType, bool inherit)
        {
            return GenericUtils.ToArray(adapter.GetAttributes(attributeType, inherit));
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return adapter.GetHashCode();
        }

        /// <inheritdoc />
        public override bool IsDefined(Type attributeType, bool inherit)
        {
            return adapter.HasAttribute(attributeType, inherit);
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return adapter.ToString();
        }
    }

    public partial class UnresolvedEventInfo
    {
        /// <inheritdoc />
        public override Type DeclaringType
        {
            get { return UnresolvedMemberInfo.GetDeclaringType(adapter); }
        }

        /// <inheritdoc />
        public override int MetadataToken
        {
            get { throw new NotSupportedException(); }
        }

        /// <inheritdoc />
        public override Module Module
        {
            get { throw new NotSupportedException(); }
        }

        /// <inheritdoc />
        public override string Name
        {
            get { return adapter.Name; }
        }

        /// <inheritdoc />
        public override Type ReflectedType
        {
            get { return DeclaringType; }
        }

        /// <inheritdoc />
        public override bool Equals(object o)
        {
            UnresolvedEventInfo other = o as UnresolvedEventInfo;
            return other != null && adapter.Equals(other.adapter);
        }

        /// <inheritdoc />
        public override object[] GetCustomAttributes(bool inherit)
        {
            return GenericUtils.ToArray(adapter.GetAttributes(inherit));
        }

        /// <inheritdoc />
        public override object[] GetCustomAttributes(Type attributeType, bool inherit)
        {
            return GenericUtils.ToArray(adapter.GetAttributes(attributeType, inherit));
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return adapter.GetHashCode();
        }

        /// <inheritdoc />
        public override bool IsDefined(Type attributeType, bool inherit)
        {
            return adapter.HasAttribute(attributeType, inherit);
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return adapter.ToString();
        }
    }

    public partial class UnresolvedMethodInfo
    {
        /// <inheritdoc />
        public override Type DeclaringType
        {
            get { return UnresolvedMemberInfo.GetDeclaringType(adapter); }
        }

        /// <inheritdoc />
        public override int MetadataToken
        {
            get { throw new NotSupportedException(); }
        }

        /// <inheritdoc />
        public override Module Module
        {
            get { throw new NotSupportedException(); }
        }

        /// <inheritdoc />
        public override string Name
        {
            get { return adapter.Name; }
        }

        /// <inheritdoc />
        public override Type ReflectedType
        {
            get { return DeclaringType; }
        }

        /// <inheritdoc />
        public override bool Equals(object o)
        {
            UnresolvedMethodInfo other = o as UnresolvedMethodInfo;
            return other != null && adapter.Equals(other.adapter);
        }

        /// <inheritdoc />
        public override object[] GetCustomAttributes(bool inherit)
        {
            return GenericUtils.ToArray(adapter.GetAttributes(inherit));
        }

        /// <inheritdoc />
        public override object[] GetCustomAttributes(Type attributeType, bool inherit)
        {
            return GenericUtils.ToArray(adapter.GetAttributes(attributeType, inherit));
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return adapter.GetHashCode();
        }

        /// <inheritdoc />
        public override bool IsDefined(Type attributeType, bool inherit)
        {
            return adapter.HasAttribute(attributeType, inherit);
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return adapter.ToString();
        }
    }

    public partial class UnresolvedConstructorInfo
    {
        /// <inheritdoc />
        public override Type DeclaringType
        {
            get { return UnresolvedMemberInfo.GetDeclaringType(adapter); }
        }

        /// <inheritdoc />
        public override int MetadataToken
        {
            get { throw new NotSupportedException(); }
        }

        /// <inheritdoc />
        public override Module Module
        {
            get { throw new NotSupportedException(); }
        }

        /// <inheritdoc />
        public override string Name
        {
            get { return adapter.Name; }
        }

        /// <inheritdoc />
        public override Type ReflectedType
        {
            get { return DeclaringType; }
        }

        /// <inheritdoc />
        public override bool Equals(object o)
        {
            UnresolvedConstructorInfo other = o as UnresolvedConstructorInfo;
            return other != null && adapter.Equals(other.adapter);
        }

        /// <inheritdoc />
        public override object[] GetCustomAttributes(bool inherit)
        {
            return GenericUtils.ToArray(adapter.GetAttributes(inherit));
        }

        /// <inheritdoc />
        public override object[] GetCustomAttributes(Type attributeType, bool inherit)
        {
            return GenericUtils.ToArray(adapter.GetAttributes(attributeType, inherit));
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return adapter.GetHashCode();
        }

        /// <inheritdoc />
        public override bool IsDefined(Type attributeType, bool inherit)
        {
            return adapter.HasAttribute(attributeType, inherit);
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return adapter.ToString();
        }
    }
}