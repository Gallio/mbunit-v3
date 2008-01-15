using System;
using System.Reflection;
using Gallio.Collections;

namespace Gallio.Reflection.Impl
{
    /// <summary>
    /// Represents a <see cref="EventInfo" /> whose native definition could not be resolved
    /// so we fall back on the <see cref="IEventInfo"/> wrapper.
    /// </summary>
    public partial class UnresolvedEventInfo : EventInfo
    {
        private readonly IEventInfo adapter;

        /// <summary>
        /// Creates a reflection object backed by the specified adapter.
        /// </summary>
        /// <param name="adapter">The adapter</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="adapter"/> is null</exception>
        public UnresolvedEventInfo(IEventInfo adapter)
        {
            if (adapter == null)
                throw new ArgumentNullException("adapter");

            this.adapter = adapter;
        }

        /// <inheritdoc />
        public override EventAttributes Attributes
        {
            get { return EventAttributes.None; }
        }

        /// <inheritdoc />
        public override MemberTypes MemberType
        {
            get { return MemberTypes.Event; }
        }

        /// <inheritdoc />
        public override MethodInfo GetAddMethod(bool nonPublic)
        {
            return UnresolvedMemberInfo.ResolveMethod(adapter.AddMethod, nonPublic);
        }

        /// <inheritdoc />
        public override MethodInfo[] GetOtherMethods(bool nonPublic)
        {
            return EmptyArray<MethodInfo>.Instance;
        }

        /// <inheritdoc />
        public override MethodInfo GetRaiseMethod(bool nonPublic)
        {
            return UnresolvedMemberInfo.ResolveMethod(adapter.RaiseMethod, nonPublic);
        }

        /// <inheritdoc />
        public override MethodInfo GetRemoveMethod(bool nonPublic)
        {
            return UnresolvedMemberInfo.ResolveMethod(adapter.RemoveMethod, nonPublic);
        }
    }
}