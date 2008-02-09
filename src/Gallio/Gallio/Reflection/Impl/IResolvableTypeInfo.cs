using System;
using System.Reflection;

namespace Gallio.Reflection.Impl
{
    /// <summary>
    /// This extension of <see cref="ITypeInfo" /> is provided to enable the resolution of
    /// a type with <see cref="ReflectorResolveUtils.ResolveType" />.
    /// </summary>
    public interface IResolvableTypeInfo : ITypeInfo
    {
        /// <summary>
        /// Resolves the wrapper to its native reflection target within the scope
        /// of the specified method.
        /// </summary>
        /// <param name="methodContext">The method that is currently in scope, or null if none.
        /// This parameter is used when resolving types that are part of the signature
        /// of a generic method so that generic method arguments can be handled correctly.</param>
        /// <param name="throwOnError">If true, throws an exception if the target could
        /// not be resolved, otherwise returns a reflection object that represents an
        /// unresolved member which may only support a subset of the usual operations</param>
        /// <returns>The native reflection target</returns>
        /// <exception cref="CodeElementResolveException">Thrown if the target cannot be resolved</exception>
        Type Resolve(MethodInfo methodContext, bool throwOnError);
    }
}
