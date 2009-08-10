// Copyright 2005-2009 Gallio Project - http://www.gallio.org/
// Portions Copyright 2000-2004 Jonathan de Halleux
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;
using Gallio.Common.Reflection.Impl;

namespace Gallio.Common.Reflection
{
    /// <summary>
    /// Wraps reflection types using the reflection adapter interfaces.
    /// Also resolves code references.
    /// </summary>
    public static class Reflector
    {
        /// <summary>
        /// Gets the singleton instance of the native <see cref="IReflectionPolicy" />
        /// based on .Net reflection.
        /// </summary>
        public static IReflectionPolicy NativeReflectionPolicy
        {
            get { return Impl.NativeReflectionPolicy.Instance; }
        }

        /// <summary>
        /// Resolves the specified <see cref="CodeReference" />. 
        /// </summary>
        /// <param name="reference">The code reference.</param>
        /// <param name="throwOnError">If true, throws an exception on error.</param>
        /// <returns>The associated code element, or null if the code reference
        /// is of kind <see cref="CodeReferenceKind.Unknown" /></returns>
        /// <exception cref="InvalidOperationException">Thrown if <paramref name="throwOnError"/>
        /// is true and <paramref name="reference"/> cannot be resolved.</exception>
        public static ICodeElementInfo Resolve(CodeReference reference, bool throwOnError)
        {
            try
            {
                CodeReferenceKind kind = reference.Kind;

                if (kind == CodeReferenceKind.Unknown)
                    return null;

                if (kind == CodeReferenceKind.Namespace)
                    return WrapNamespace(reference.NamespaceName);

                Assembly assembly = Assembly.Load(reference.AssemblyName);
                if (kind == CodeReferenceKind.Assembly)
                    return Wrap(assembly);

                Type type = assembly.GetType(reference.TypeName, true);
                if (kind == CodeReferenceKind.Type)
                    return Wrap(type);

                // TODO: Handle overloading by signature.
                MemberInfo[] members = type.GetMember(reference.MemberName,
                    BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);

                if (members.Length != 1)
                    throw new AmbiguousMatchException("There was not exactly one matching member.");

                MemberInfo member = members[0];

                if (kind == CodeReferenceKind.Member)
                    return Wrap(member);

                MethodBase method = member as MethodBase;
                if (method == null)
                    throw new AmbiguousMatchException("The member that was found was not a constructor or method as expected.");

                ParameterInfo parameter = Array.Find(method.GetParameters(), delegate(ParameterInfo candidate)
                {
                    return candidate.Name == reference.ParameterName;
                });

                if (parameter == null)
                    throw new AmbiguousMatchException("The named parameter was not found.");

                return Wrap(parameter);
            }
            catch (Exception ex)
            {
                if (throwOnError)
                    throw new InvalidOperationException(
                        String.Format("Could not resolve '{0}'.", reference), ex);
                return null;
            }
        }

        /// <summary>
        /// Obtains a reflection wrapper for an assembly.
        /// </summary>
        /// <param name="target">The assembly, or null if none.</param>
        /// <returns>The reflection wrapper, or null if none.</returns>
        public static IAssemblyInfo Wrap(Assembly target)
        {
            return target != null ? new NativeAssemblyWrapper(target) : null;
        }

        /// <summary>
        /// Obtains a reflection wrapper for a namespace.
        /// </summary>
        /// <param name="name">The namespace name, or null if none.</param>
        /// <returns>The reflection wrapper, or null if none.</returns>
        public static INamespaceInfo WrapNamespace(string name)
        {
            return name != null ? new NativeNamespaceWrapper(name) : null;
        }

        /// <summary>
        /// Obtains a reflection wrapper for a type.
        /// </summary>
        /// <param name="target">The type, or null if none.</param>
        /// <returns>The reflection wrapper, or null if none.</returns>
        public static ITypeInfo Wrap(Type target)
        {
            if (target == null)
                return null;

            UnresolvedType unresolvedTarget = target as UnresolvedType;
            if (unresolvedTarget != null)
                return unresolvedTarget.Adapter;

            return target.IsGenericParameter ? new NativeGenericParameterWrapper(target) : new NativeTypeWrapper(target);
        }

        /// <summary>
        /// Obtains a reflection wrapper for a member.
        /// </summary>
        /// <param name="target">The member, or null if none.</param>
        /// <returns>The reflection wrapper, or null if none.</returns>
        public static IMemberInfo Wrap(MemberInfo target)
        {
            if (target == null)
                return null;

            switch (target.MemberType)
            {
                case MemberTypes.Event:
                    return Wrap((EventInfo)target);

                case MemberTypes.Field:
                    return Wrap((FieldInfo)target);

                case MemberTypes.Constructor:
                    return Wrap((ConstructorInfo)target);

                case MemberTypes.Method:
                    return Wrap((MethodInfo)target);

                case MemberTypes.Property:
                    return Wrap((PropertyInfo)target);

                case MemberTypes.TypeInfo:
                case MemberTypes.NestedType:
                    return Wrap((Type)target);

                default:
                    throw new NotSupportedException("Unsupported member type.");
            }
        }

        /// <summary>
        /// Obtains a reflection wrapper for a function.
        /// </summary>
        /// <param name="target">The function, or null if none.</param>
        /// <returns>The reflection wrapper, or null if none.</returns>
        public static IFunctionInfo Wrap(MethodBase target)
        {
            MethodInfo method = target as MethodInfo;
            if (method != null)
                return Wrap(method);

            return Wrap((ConstructorInfo)target);
        }

        /// <summary>
        /// Obtains a reflection wrapper for a constructor.
        /// </summary>
        /// <param name="target">The constructor, or null if none.</param>
        /// <returns>The reflection wrapper, or null if none.</returns>
        public static IConstructorInfo Wrap(ConstructorInfo target)
        {
            if (target == null)
                return null;

            UnresolvedConstructorInfo unresolvedTarget = target as UnresolvedConstructorInfo;
            if (unresolvedTarget != null)
                return unresolvedTarget.Adapter;

            return new NativeConstructorWrapper(target);
        }

        /// <summary>
        /// Obtains a reflection wrapper for a method.
        /// </summary>
        /// <param name="target">The method, or null if none.</param>
        /// <returns>The reflection wrapper, or null if none.</returns>
        public static IMethodInfo Wrap(MethodInfo target)
        {
            if (target == null)
                return null;

            UnresolvedMethodInfo unresolvedTarget = target as UnresolvedMethodInfo;
            if (unresolvedTarget != null)
                return unresolvedTarget.Adapter;

            return new NativeMethodWrapper(target);
        }

        /// <summary>
        /// Obtains a reflection wrapper for a property.
        /// </summary>
        /// <param name="target">The property, or null if none.</param>
        /// <returns>The reflection wrapper, or null if none.</returns>
        public static IPropertyInfo Wrap(PropertyInfo target)
        {
            if (target == null)
                return null;

            UnresolvedPropertyInfo unresolvedTarget = target as UnresolvedPropertyInfo;
            if (unresolvedTarget != null)
                return unresolvedTarget.Adapter;

            return new NativePropertyWrapper(target);
        }

        /// <summary>
        /// Obtains a reflection wrapper for a field.
        /// </summary>
        /// <param name="target">The field, or null if none.</param>
        /// <returns>The reflection wrapper, or null if none.</returns>
        public static IFieldInfo Wrap(FieldInfo target)
        {
            if (target == null)
                return null;

            UnresolvedFieldInfo unresolvedTarget = target as UnresolvedFieldInfo;
            if (unresolvedTarget != null)
                return unresolvedTarget.Adapter;

            return new NativeFieldWrapper(target);
        }

        /// <summary>
        /// Obtains a reflection wrapper for an event.
        /// </summary>
        /// <param name="target">The event, or null if none.</param>
        /// <returns>The reflection wrapper, or null if none.</returns>
        public static IEventInfo Wrap(EventInfo target)
        {
            if (target == null)
                return null;

            UnresolvedEventInfo unresolvedTarget = target as UnresolvedEventInfo;
            if (unresolvedTarget != null)
                return unresolvedTarget.Adapter;

            return new NativeEventWrapper(target);
        }

        /// <summary>
        /// Obtains a reflection wrapper for a parameter.
        /// </summary>
        /// <param name="target">The parameter, or null if none.</param>
        /// <returns>The reflection wrapper, or null if none.</returns>
        public static IParameterInfo Wrap(ParameterInfo target)
        {
            if (target == null)
                return null;

            UnresolvedParameterInfo unresolvedTarget = target as UnresolvedParameterInfo;
            if (unresolvedTarget != null)
                return unresolvedTarget.Adapter;

            return new NativeParameterWrapper(target);
        }

        /// <summary>
        /// Obtains a reflection wrapper for an attribute instance.
        /// </summary>
        /// <param name="target">The attribute, or null if none.</param>
        /// <returns>The reflection wrapper, or null if none.</returns>
        public static IAttributeInfo Wrap(Attribute target)
        {
            return target != null ? new NativeAttributeWrapper(target) : null;
        }

        /// <summary>
        /// Creates a code element from the executing function.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This information may be unreliable if the compiler has inlined
        /// the executing method into its caller because the stack frame
        /// information will be incomplete.  This can can occur when compiler
        /// optimizations are turned on and the method body is simple.
        /// </para>
        /// <para>
        /// The inlining problem can be circumvented like this:
        /// <code><![CDATA[
        /// [MethodImpl(MethodImplOptions.NoInlining)]
        /// public void Foo()
        /// {
        ///     IMethodInfo r = NativeReflector.GetExecutingFunction();
        ///     // ...
        /// }
        /// ]]></code>
        /// </para>
        /// </remarks>
        /// <returns>The code reference.</returns>
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static IFunctionInfo GetExecutingFunction()
        {
            return GetFunctionFromStackFrame(1);
        }

        /// <summary>
        /// Creates a code element from the caller of the executing function.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This information may be unreliable if the compiler has inlined
        /// the executing or calling method into its caller because the stack frame
        /// information will be incomplete.  This can can occur when compiler
        /// optimizations are turned on and the method body is simple.
        /// </para>
        /// <para>
        /// The inlining problem can be circumvented like this:
        /// <code><![CDATA[
        /// [MethodImpl(MethodImplOptions.NoInlining)]
        /// public void Foo()
        /// {
        ///     IMethodInfo r = NativeReflector.GetCallingFunction();
        ///     // ...
        /// }
        /// ]]></code>
        /// </para>
        /// </remarks>
        /// <returns>The code reference.</returns>
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static IFunctionInfo GetCallingFunction()
        {
            return GetFunctionFromStackFrame(2);
        }

        /// <summary>
        /// Creates a code element representing a function from a particular frame on the current stack.
        /// </summary>
        /// <param name="framesToSkip">The number of frames to skip.  If this number is 0,
        /// the code reference will refer to the direct caller of this method;
        /// if it is 1, it will refer to the caller's caller, and so on.</param>
        /// <returns>The code reference.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="framesToSkip"/> is less than zero.</exception>
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static IFunctionInfo GetFunctionFromStackFrame(int framesToSkip)
        {
            if (framesToSkip < 0)
                throw new ArgumentOutOfRangeException("framesToSkip", framesToSkip, "Must be zero or more.");

            StackTrace stackTrace = new StackTrace(framesToSkip + 1, false);
            StackFrame stackFrame = stackTrace.GetFrame(0);
            return Wrap(stackFrame.GetMethod());
        }

        /// <summary>
        /// Returns true if the target represents an unresolved member with
        /// limited support for reflection.
        /// </summary>
        /// <seealso cref="IUnresolvedCodeElement"/>
        /// <param name="target">The member, or null if none.</param>
        /// <returns>True if the target is unresolved.</returns>
        public static bool IsUnresolved(MemberInfo target)
        {
            return target is IUnresolvedCodeElement;
        }

        /// <summary>
        /// Returns true if the target represents an unresolved parameter with
        /// limited support for reflection.
        /// </summary>
        /// <seealso cref="IUnresolvedCodeElement"/>
        /// <param name="target">The parameter, or null if none.</param>
        /// <returns>True if the target is unresolved.</returns>
        public static bool IsUnresolved(ParameterInfo target)
        {
            return target is UnresolvedParameterInfo;
        }
    }
}
