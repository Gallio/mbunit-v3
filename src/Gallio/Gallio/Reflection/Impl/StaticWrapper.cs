using System;

namespace Gallio.Reflection.Impl
{
    /// <summary>
    /// <para>
    /// A <see cref="StaticReflectionPolicy"/> wrapper.
    /// </para>
    /// <para>
    /// A wrapper holds an underlying reflection object.  Its behavior is
    /// derived from by primitive operations on the <see cref="Handle" /> defined by the
    /// particular <see cref="Policy"/> implementation that is in use.
    /// </para>
    /// </summary>
    public abstract class StaticWrapper : IEquatable<StaticWrapper>
    {
        private readonly StaticReflectionPolicy policy;
        private readonly object handle;

        /// <summary>
        /// Creates a wrapper.
        /// </summary>
        /// <param name="policy">The reflection policy</param>
        /// <param name="handle">The underlying reflection object</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="policy"/> or <paramref name="handle"/> is null</exception>
        public StaticWrapper(StaticReflectionPolicy policy, object handle)
        {
            if (policy == null)
                throw new ArgumentNullException("policy");
            if (handle == null)
                throw new ArgumentNullException("handle");

            this.policy = policy;
            this.handle = handle;
        }

        /// <summary>
        /// Gets the reflection policy.
        /// </summary>
        public StaticReflectionPolicy Policy
        {
            get { return policy; }
        }

        /// <summary>
        /// Gets the underlying reflection object.
        /// </summary>
        public object Handle
        {
            get { return handle; }
        }

        /// <inhertdoc />
        public bool Equals(StaticWrapper other)
        {
            return other != null
                && policy == other.policy
                && policy.Equals(this, other);
        }

        /// <inhertdoc />
        public override bool Equals(object obj)
        {
            return Equals(obj as StaticWrapper);
        }

        /// <inhertdoc />
        public override int GetHashCode()
        {
            return policy.GetHashCode(this);
        }
    }
}
