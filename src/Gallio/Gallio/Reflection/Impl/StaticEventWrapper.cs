using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Gallio.Utilities;

namespace Gallio.Reflection.Impl
{
    /// <summary>
    /// A <see cref="StaticReflectionPolicy"/> event wrapper.
    /// </summary>
    public sealed class StaticEventWrapper : StaticMemberWrapper, IEventInfo
    {
        private readonly Memoizer<EventAttributes> eventAttributesMemoizer = new Memoizer<EventAttributes>();

        /// <summary>
        /// Creates a wrapper.
        /// </summary>
        /// <param name="policy">The reflection policy</param>
        /// <param name="handle">The underlying reflection object</param>
        /// <param name="declaringType">The declaring type</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="policy"/>, <paramref name="handle"/>,
        /// or <paramref name="declaringType"/> is null</exception>
        public StaticEventWrapper(StaticReflectionPolicy policy, object handle, StaticDeclaredTypeWrapper declaringType)
            : base(policy, handle, declaringType)
        {
            if (declaringType == null)
                throw new ArgumentNullException("declaringType");
        }

        /// <inheritdoc />
        public override CodeElementKind Kind
        {
            get { return CodeElementKind.Event; }
        }

        /// <inheritdoc />
        public EventAttributes EventAttributes
        {
            get
            {
                return eventAttributesMemoizer.Memoize(delegate
                {
                    return Policy.GetEventAttributes(this);
                });
            }
        }

        /// <inheritdoc />
        public StaticMethodWrapper AddMethod
        {
            get { return Policy.GetEventAddMethod(this); }
        }
        IMethodInfo IEventInfo.AddMethod
        {
            get { return AddMethod; }
        }

        /// <inheritdoc />
        public StaticMethodWrapper RaiseMethod
        {
            get { return Policy.GetEventRaiseMethod(this); }
        }
        IMethodInfo IEventInfo.RaiseMethod
        {
            get { return RaiseMethod; }
        }

        /// <inheritdoc />
        public StaticMethodWrapper RemoveMethod
        {
            get { return Policy.GetEventRemoveMethod(this); }
        }
        IMethodInfo IEventInfo.RemoveMethod
        {
            get { return RemoveMethod; }
        }

        /// <inheritdoc />
        public ITypeInfo EventHandlerType
        {
            get { return Substitution.Apply(Policy.GetEventHandlerType(this)); }
        }

        /// <inheritdoc />
        public EventInfo Resolve(bool throwOnError)
        {
            return ReflectorResolveUtils.ResolveEvent(this, throwOnError);
        }

        /// <inheritdoc />
        public bool Equals(IEventInfo other)
        {
            return Equals((object)other);
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

            sig.Append(GetTypeNameForSignature(EventHandlerType));
            sig.Append(' ');
            sig.Append(Name);

            return sig.ToString();
        }

        /// <inheritdoc />
        protected override IEnumerable<ICodeElementInfo> GetInheritedElements()
        {
            return ReflectorInheritanceUtils.EnumerateSuperEvents(this);
        }
    }
}
