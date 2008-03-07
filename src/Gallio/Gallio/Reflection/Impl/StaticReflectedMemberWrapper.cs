using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Gallio.Collections;
using Gallio.Utilities;

namespace Gallio.Reflection.Impl
{
    /// <summary>
    /// A <see cref="StaticReflectionPolicy"/> member wrapper for members that are not types,
    /// generic parameters or nested types.  These members must be declared by types, so
    /// they all share the constraint that the declaring type and reflected type must not be null.
    /// In particular, the reflected type may be a subtype of the declaring type in the case
    /// of inherited members.
    /// </summary>
    public abstract class StaticReflectedMemberWrapper : StaticMemberWrapper
    {
        private readonly StaticDeclaredTypeWrapper reflectedType;

        /// <summary>
        /// Creates a wrapper.
        /// </summary>
        /// <param name="policy">The reflection policy</param>
        /// <param name="handle">The underlying reflection object</param>
        /// <param name="declaringType">The declaring type</param>
        /// <param name="reflectedType">The reflected type</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="policy"/>, <paramref name="handle"/>,
        /// <paramref name="declaringType"/>, or <paramref name="reflectedType"/> is null</exception>
        protected StaticReflectedMemberWrapper(StaticReflectionPolicy policy, object handle, StaticDeclaredTypeWrapper declaringType,
            StaticDeclaredTypeWrapper reflectedType)
            : base(policy, handle, declaringType)
        {
            if (declaringType == null)
                throw new ArgumentNullException("declaringType");
            if (reflectedType == null)
                throw new ArgumentNullException("reflectedType");

            this.reflectedType = reflectedType;
        }

        /// <inheritdoc />
        public override StaticDeclaredTypeWrapper ReflectedType
        {
            get { return reflectedType; }
        }
    }
}