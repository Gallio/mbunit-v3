using System;
using System.Reflection;
using System.Text;
using Gallio.Utilities;

namespace Gallio.Reflection.Impl
{
    /// <summary>
    /// A <see cref="StaticReflectionPolicy"/> field wrapper.
    /// </summary>
    public class StaticFieldWrapper : StaticMemberWrapper, IFieldInfo
    {
        private readonly Memoizer<FieldAttributes> fieldAttributesMemoizer = new Memoizer<FieldAttributes>();

        /// <summary>
        /// Creates a wrapper.
        /// </summary>
        /// <param name="policy">The reflection policy</param>
        /// <param name="handle">The underlying reflection object</param>
        /// <param name="declaringType">The declaring type</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="policy"/>, <paramref name="handle"/>,
        /// or <paramref name="declaringType"/> is null</exception>
        public StaticFieldWrapper(StaticReflectionPolicy policy, object handle, StaticDeclaredTypeWrapper declaringType)
            : base(policy, handle, declaringType)
        {
            if (declaringType == null)
                throw new ArgumentNullException("declaringType");
        }

        /// <inheritdoc />
        public override CodeElementKind Kind
        {
            get { return CodeElementKind.Field; }
        }

        /// <inheritdoc />
        public FieldAttributes FieldAttributes
        {
            get
            {
                return fieldAttributesMemoizer.Memoize(delegate
                {
                    return Policy.GetFieldAttributes(this); 
                });
            }
        }

        /// <inheritdoc />
        public bool IsLiteral
        {
            get { return (FieldAttributes & FieldAttributes.Literal) != 0; }
        }

        /// <inheritdoc />
        public bool IsPublic
        {
            get { return (FieldAttributes & FieldAttributes.FieldAccessMask) == FieldAttributes.Public; }
        }

        /// <inheritdoc />
        public bool IsInitOnly
        {
            get { return (FieldAttributes & FieldAttributes.InitOnly) != 0; }
        }

        /// <inheritdoc />
        public bool IsStatic
        {
            get { return (FieldAttributes & FieldAttributes.Static) != 0; }
        }

        /// <inheritdoc />
        public ITypeInfo ValueType
        {
            get { return Substitution.Apply(Policy.GetFieldType(this)); }
        }

        /// <inheritdoc />
        public int Position
        {
            get { return 0; }
        }

        /// <inheritdoc />
        public bool Equals(ISlotInfo other)
        {
            return Equals((object)other);
        }

        /// <inheritdoc />
        public bool Equals(IFieldInfo other)
        {
            return Equals((object)other);
        }

        /// <inheritdoc />
        public FieldInfo Resolve(bool throwOnError)
        {
            return ReflectorResolveUtils.ResolveField(this, throwOnError);
        }

        /// <inheritdoc />
        protected override MemberInfo ResolveMemberInfo(bool throwOnError)
        {
            return Resolve(throwOnError);
        }

        /// <inheritdoc />
        public override string ToString()
        {
            StringBuilder sig = new StringBuilder();

            sig.Append(GetTypeNameForSignature(ValueType));
            sig.Append(' ');
            sig.Append(Name);

            return sig.ToString();
        }
    }
}
