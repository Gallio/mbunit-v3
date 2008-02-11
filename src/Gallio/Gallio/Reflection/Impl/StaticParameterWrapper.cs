using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Gallio.Utilities;

namespace Gallio.Reflection.Impl
{
    /// <summary>
    /// A <see cref="StaticReflectionPolicy"/> parameter wrapper.
    /// </summary>
    public sealed class StaticParameterWrapper : StaticCodeElementWrapper, IParameterInfo
    {
        private readonly Memoizer<ParameterAttributes> parameterAttributesMemoizer = new Memoizer<ParameterAttributes>();
        private readonly StaticMemberWrapper member;

        /// <summary>
        /// Creates a wrapper.
        /// </summary>
        /// <param name="policy">The reflection policy</param>
        /// <param name="handle">The underlying reflection object</param>
        /// <param name="member">The member to which the parameter belongs</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="policy"/>, <paramref name="handle"/>
        /// or <paramref name="member"/> is null</exception>
        public StaticParameterWrapper(StaticReflectionPolicy policy, object handle, StaticMemberWrapper member)
            : base(policy, handle)
        {
            if (member == null)
                throw new ArgumentNullException("member");

            this.member = member;
        }

        /// <summary>
        /// Gets the member to which the parameter belongs.
        /// </summary>
        public StaticMemberWrapper Member
        {
            get { return member; }
        }
        IMemberInfo IParameterInfo.Member
        {
            get { return member; }
        }

        /// <inheritdoc />
        public override CodeElementKind Kind
        {
            get { return CodeElementKind.Parameter; }
        }

        /// <inheritdoc />
        public override string Name
        {
            get { return Policy.GetParameterName(this); }
        }

        /// <inheritdoc />
        public override CodeReference CodeReference
        {
            get
            {
                CodeReference reference = member.CodeReference;
                reference.ParameterName = Name;
                return reference;
            }
        }

        /// <inheritdoc />
        public ParameterAttributes ParameterAttributes
        {
            get
            {
                return parameterAttributesMemoizer.Memoize(delegate
                {
                    return Policy.GetParameterAttributes(this);
                });
            }
        }

        /// <inheritdoc />
        public ParameterInfo Resolve(bool throwOnError)
        {
            return ReflectorResolveUtils.ResolveParameter(this, throwOnError);
        }

        /// <inheritdoc />
        public ITypeInfo ValueType
        {
            get { return member.Substitution.Apply(Policy.GetParameterType(this)); }
        }

        /// <inheritdoc />
        public int Position
        {
            get { return Policy.GetParameterPosition(this); }
        }

        /// <inheritdoc />
        public bool Equals(ISlotInfo other)
        {
            return Equals((object)other);
        }

        /// <inheritdoc />
        public bool Equals(IParameterInfo other)
        {
            return Equals((object)other);
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            StaticParameterWrapper other = obj as StaticParameterWrapper;
            return base.Equals(other) && member.Equals(other.member);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return base.GetHashCode() ^ member.GetHashCode();
        }

        /// <inheritdoc />
        protected override IEnumerable<StaticAttributeWrapper> GetCustomAttributes()
        {
            return Policy.GetParameterCustomAttributes(this);
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

        /// <inheritdoc />
        protected override IEnumerable<ICodeElementInfo> GetInheritedElements()
        {
            return ReflectorInheritanceUtils.EnumerateSuperParameters(this);
        }
    }
}
