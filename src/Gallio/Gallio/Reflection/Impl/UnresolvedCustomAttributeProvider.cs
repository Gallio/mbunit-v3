using System;
using Gallio.Collections;

/*
 * This compilation unit contains ICustomAttributeProvider overrides that must be duplicated for each
 * of the unresolved reflection types because C# does not support multiple inheritance.
 */

namespace Gallio.Reflection.Impl
{
    internal static class UnresolvedCustomAttributeProvider
    {
        public static object[] GetCustomAttributes(ICodeElementInfo adapter, bool inherit)
        {
            return GenericUtils.ToArray(adapter.GetAttributes(null, inherit));
        }

        public static object[] GetCustomAttributes(ICodeElementInfo adapter, Type attributeType, bool inherit)
        {
            return GenericUtils.ToArray(adapter.GetAttributes(Reflector.Wrap(attributeType), inherit));
        }

        public static bool IsDefined(ICodeElementInfo adapter, Type attributeType, bool inherit)
        {
            return adapter.HasAttribute(Reflector.Wrap(attributeType), inherit);
        }
    }

    public partial class UnresolvedConstructorInfo
    {
        /// <inheritdoc />
        public override object[] GetCustomAttributes(bool inherit)
        {
            return UnresolvedCustomAttributeProvider.GetCustomAttributes(adapter, inherit);
        }

        /// <inheritdoc />
        public override object[] GetCustomAttributes(Type attributeType, bool inherit)
        {
            return UnresolvedCustomAttributeProvider.GetCustomAttributes(adapter, attributeType, inherit);
        }

        /// <inheritdoc />
        public override bool IsDefined(Type attributeType, bool inherit)
        {
            return UnresolvedCustomAttributeProvider.IsDefined(adapter, attributeType, inherit);
        }
    }

    public partial class UnresolvedEventInfo
    {
        /// <inheritdoc />
        public override object[] GetCustomAttributes(bool inherit)
        {
            return UnresolvedCustomAttributeProvider.GetCustomAttributes(adapter, inherit);
        }

        /// <inheritdoc />
        public override object[] GetCustomAttributes(Type attributeType, bool inherit)
        {
            return UnresolvedCustomAttributeProvider.GetCustomAttributes(adapter, attributeType, inherit);
        }

        /// <inheritdoc />
        public override bool IsDefined(Type attributeType, bool inherit)
        {
            return UnresolvedCustomAttributeProvider.IsDefined(adapter, attributeType, inherit);
        }
    }

    public partial class UnresolvedFieldInfo
    {
        /// <inheritdoc />
        public override object[] GetCustomAttributes(bool inherit)
        {
            return UnresolvedCustomAttributeProvider.GetCustomAttributes(adapter, inherit);
        }

        /// <inheritdoc />
        public override object[] GetCustomAttributes(Type attributeType, bool inherit)
        {
            return UnresolvedCustomAttributeProvider.GetCustomAttributes(adapter, attributeType, inherit);
        }

        /// <inheritdoc />
        public override bool IsDefined(Type attributeType, bool inherit)
        {
            return UnresolvedCustomAttributeProvider.IsDefined(adapter, attributeType, inherit);
        }
    }

    public partial class UnresolvedMethodInfo
    {
        /// <inheritdoc />
        public override object[] GetCustomAttributes(bool inherit)
        {
            return UnresolvedCustomAttributeProvider.GetCustomAttributes(adapter, inherit);
        }

        /// <inheritdoc />
        public override object[] GetCustomAttributes(Type attributeType, bool inherit)
        {
            return UnresolvedCustomAttributeProvider.GetCustomAttributes(adapter, attributeType, inherit);
        }

        /// <inheritdoc />
        public override bool IsDefined(Type attributeType, bool inherit)
        {
            return UnresolvedCustomAttributeProvider.IsDefined(adapter, attributeType, inherit);
        }
    }

    public partial class UnresolvedParameterInfo
    {
        /// <inheritdoc />
        public override object[] GetCustomAttributes(bool inherit)
        {
            return UnresolvedCustomAttributeProvider.GetCustomAttributes(adapter, inherit);
        }

        /// <inheritdoc />
        public override object[] GetCustomAttributes(Type attributeType, bool inherit)
        {
            return UnresolvedCustomAttributeProvider.GetCustomAttributes(adapter, attributeType, inherit);
        }

        /// <inheritdoc />
        public override bool IsDefined(Type attributeType, bool inherit)
        {
            return UnresolvedCustomAttributeProvider.IsDefined(adapter, attributeType, inherit);
        }
    }

    public partial class UnresolvedPropertyInfo
    {
        /// <inheritdoc />
        public override object[] GetCustomAttributes(bool inherit)
        {
            return UnresolvedCustomAttributeProvider.GetCustomAttributes(adapter, inherit);
        }

        /// <inheritdoc />
        public override object[] GetCustomAttributes(Type attributeType, bool inherit)
        {
            return UnresolvedCustomAttributeProvider.GetCustomAttributes(adapter, attributeType, inherit);
        }

        /// <inheritdoc />
        public override bool IsDefined(Type attributeType, bool inherit)
        {
            return UnresolvedCustomAttributeProvider.IsDefined(adapter, attributeType, inherit);
        }
    }

    public partial class UnresolvedType
    {
        /// <inheritdoc />
        public override object[] GetCustomAttributes(bool inherit)
        {
            return UnresolvedCustomAttributeProvider.GetCustomAttributes(adapter, inherit);
        }

        /// <inheritdoc />
        public override object[] GetCustomAttributes(Type attributeType, bool inherit)
        {
            return UnresolvedCustomAttributeProvider.GetCustomAttributes(adapter, attributeType, inherit);
        }

        /// <inheritdoc />
        public override bool IsDefined(Type attributeType, bool inherit)
        {
            return UnresolvedCustomAttributeProvider.IsDefined(adapter, attributeType, inherit);
        }
    }
}
