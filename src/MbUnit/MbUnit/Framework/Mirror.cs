// Copyright 2005-2010 Gallio Project - http://www.gallio.org/
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
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading;
using Gallio.Common;
using Gallio.Common.Collections;
using Gallio.Common.Diagnostics;
using Gallio.Common.Reflection;

namespace MbUnit.Framework
{
    /// <summary>
    /// A mirror uses reflection to provide access to non-public members of an object
    /// or type for testing purposes.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Writing tests that require access to non-public members is not recommended.
    /// Non-public members should be considered internal implementation details of
    /// the subject under test.  Relying on their behavior can yield brittle tests.
    /// </para>
    /// <para>
    /// These reflection helpers are not intended to assist with all possible reflection
    /// scenarios.  For everything else, use the standard .Net reflection library.
    /// </para>
    /// <para>
    /// Another useful function related to <see cref="Mirror"/> is <see cref="ProxyUtils.CoerceDelegate"/>
    /// which you can use to convert a delegate from one type to another.  This is useful
    /// for creating instances of non-public delegate types.  In particular, the
    /// <see cref="MemberSet.AddHandler(Delegate)"/> method automatically coerces delegates
    /// if needed.
    /// </para>
    /// </remarks>
    /// <example>
    /// <para>
    /// In this example, we use a <see cref="Mirror"/> to access non-public members
    /// of an object and static members of an internal class.
    /// </para>
    /// <code>
    /// // A sample object with various members including a private nested class
    /// // that has a public yet inaccessible static method.
    /// public class SampleObject
    /// {
    ///     private int privateField;
    ///     private int PrivateProperty { get; set;}
    ///     private event EventHandler PrivateEvent;
    ///     private int PrivateMethod(int a, int b) { ... }
    ///     
    ///     private static class PrivateNestedClass
    ///     {
    ///         public int StaticMethod(object y) { ... }
    ///         
    ///         public int OverloadedMethod(object y) { ... }
    ///         
    ///         public int OverloadedMethod(string y) { ... }
    ///         
    ///         public T GenericMethod&lt;T&gt;(T x) { ... }
    ///     }
    /// }
    /// 
    /// internal class InternalObject { ... }
    /// 
    /// internal delegate int InternalDelegate(InternalObject x);
    /// 
    /// // A sample test fixture.
    /// public class Fixture
    /// {
    ///     [Test]
    ///     public void Test()
    ///     {
    ///         // Setup.
    ///         SampleObject obj = new SampleObject();
    ///         Mirror objMirror = Mirror.ForObject(obj);
    ///         
    ///         // Reading from a private field.
    ///         int privateFieldValue = (int) objMirror["privateField"].Value;
    ///         
    ///         // Writing to a private property.
    ///         objMirror["PrivateProperty"].Value = 42;
    ///         
    ///         // Obtaining a mirror for a value at index 5 in a private indexed property.
    ///         Mirror valueMirror = objMirror["Item"][5].ValueAsMirror;
    ///         
    ///         // Invoking a private method.
    ///         int result = (int) objMirror["PrivateMethod"].Invoke(1, 2);
    ///         
    ///         // Accessing a private type.
    ///         Type privateNestedClass = objMirror["PrivateNestedClass"].Type;
    ///         Mirror privateNestedClassMirror = Mirror.ForType(privateNestedClass);
    ///         
    ///         // Invoking a static method.
    ///         int result2 = privateNestedClassMirror["StaticMethod"].Invoke(42);
    ///         
    ///         // Invoking an overloaded static method when we cannot determine the parameter
    ///         // type automatically from the arguments, such as when an argument is null.
    ///         int result2 = privateNestedClassMirror["OverloadedMethod"].WithSignature(typeof(object)).Invoke(null);
    ///         int result3 = privateNestedClassMirror["OverloadedMethod"].WithSignature(typeof(string)).Invoke(null);
    /// 
    ///         // Invoking a generic method.
    ///         int result3 = privateNestedClassMirror["GenericMethod"].Invoke(42);
    ///         
    ///         // Invoking a generic method using specific type arguments.
    ///         object result4 = privateNestedClassMirror["GenericMethod"].WithTypeArgs(typeof(object)).Invoke(42);
    /// 
    ///         // Creating an instance of an internal object.
    ///         Mirror internalObjTypeMirror = Mirror.ForType("My.Namespace.InternalObject", "MyAssembly");
    ///         object internalObj = internalObjTypeMirror.CreateInstance();
    ///         
    ///         // Creating an instance of an internal delegate.
    ///         // Notice that the Mirror automatically adapts the provided delegate signature
    ///         // to the actual delegate signature to make it easier to write tests when the delegate
    ///         // parameter or result types are themselves inaccessible.
    ///         Mirror internalDelegateTypeMirror = Mirror.ForType("My.Namespace.InternalDelegate", "MyAssembly");
    ///         Delegate internalDelegate = internalDelegateTypeMirror.CreateDelegate(
    ///             new Func&lt;object, int&gt;(x => { return 5; }));
    ///     }
    /// }
    /// </code>
    /// </example>
    public sealed class Mirror
    {
        private const BindingFlags AnyBinding = BindingFlags.Public | BindingFlags.NonPublic
            | BindingFlags.Instance | BindingFlags.Static | BindingFlags.FlattenHierarchy;

        private readonly object instance;
        private readonly Type type; // only null if instance is null

        private Mirror(Type type, object instance)
        {
            this.type = type;
            this.instance = instance;
        }

        /// <summary>
        /// Gets an instance of a mirror that represents a null object of unknown type.
        /// </summary>
        public static readonly Mirror NullOfUnknownType = new Mirror(null, null);

        /// <summary>
        /// Returns true if the mirror represents a null object.
        /// </summary>
        public bool IsNull
        {
            get { return instance == null; }
        }

        /// <summary>
        /// Returns true if the mirror represents a null object of unknown type.
        /// </summary>
        public bool IsNullOfUnknownType
        {
            get { return type == null; }
        }

        /// <summary>
        /// Gets the reflected type seen in the mirror, or null if the mirror represents a null
        /// object of unknown type.
        /// </summary>
        public Type Type
        {
            get { return type; }
        }

        /// <summary>
        /// Gets the object instance seen in the mirror, or null if the mirror does not represent
        /// a particular instance of a type.
        /// </summary>
        public object Instance
        {
            get { return instance; }
        }

        /// <summary>
        /// Creates a mirror for the specified type.
        /// </summary>
        /// <typeparam name="T">The type.</typeparam>
        /// <returns>The new mirror.</returns>
        public static Mirror ForType<T>()
        {
            return new Mirror(typeof(T), null);
        }

        /// <summary>
        /// Creates a mirror for the specified type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>The new mirror.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="type"/> is null.</exception>
        public static Mirror ForType(Type type)
        {
            if (type == null)
                throw new ArgumentNullException("type");

            return new Mirror(type, null);
        }

        /// <summary>
        /// Creates a mirror for the specified type by assembly-qualified name.
        /// </summary>
        /// <param name="typeName">The assembly-qualified type name.  If the type is in the
        /// currently executing assembly or in mscorlib.dll it is sufficient to provide
        /// the namespace-qualified type name.</param>
        /// <returns>The new mirror.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="typeName"/> is null.</exception>
        /// <exception cref="MirrorException">Thrown if the type is not found.</exception>
        public static Mirror ForType(string typeName)
        {
            if (typeName == null)
                throw new ArgumentNullException("typeName");

            Type type;
            Exception ex = Eval(() => Type.GetType(typeName), out type);
            if (type == null)
                throw new MirrorException(string.Format("Could not find type '{0}'.", typeName), ex);

            return new Mirror(type, null);
        }

        /// <summary>
        /// Creates a mirror for the specified type by namespace-qualified name and assembly.
        /// </summary>
        /// <param name="typeName">The namespace-qualified type name.</param>
        /// <param name="assembly">The assembly.</param>
        /// <returns>The new mirror.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="typeName"/> or <paramref name="assembly"/> is null.</exception>
        /// <exception cref="MirrorException">Thrown if the type is not found.</exception>
        public static Mirror ForType(string typeName, Assembly assembly)
        {
            if (typeName == null)
                throw new ArgumentNullException("typeName");
            if (assembly == null)
                throw new ArgumentNullException("assembly");

            Type type;
            Exception ex = Eval(() => assembly.GetType(typeName), out type);
            if (type == null)
                throw new MirrorException(string.Format("Could not find type '{0}' in assembly '{1}'.", typeName, assembly.FullName), ex);

            return new Mirror(type, null);
        }

        /// <summary>
        /// Creates a mirror for the specified type by namespace-qualified name and assembly name.
        /// </summary>
        /// <param name="typeName">The namespace-qualified type name.</param>
        /// <param name="assemblyName">The assembly name.</param>
        /// <returns>The new mirror.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="typeName"/> or <paramref name="assemblyName"/> is null.</exception>
        /// <exception cref="MirrorException">Thrown if the type is not found.</exception>
        public static Mirror ForType(string typeName, string assemblyName)
        {
            if (typeName == null)
                throw new ArgumentNullException("typeName");
            if (assemblyName == null)
                throw new ArgumentNullException("assemblyName");

            Type type;
            Exception ex = Eval(() => Assembly.Load(assemblyName).GetType(typeName), out type);
            if (type == null)
                throw new MirrorException(string.Format("Could not find type '{0}' in assembly '{1}'.", typeName, assemblyName), ex);

            return new Mirror(type, null);
        }

        /// <summary>
        /// Creates a mirror for the specified object.
        /// </summary>
        /// <param name="instance">The object instance, or null.</param>
        /// <returns>The new mirror.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="instance"/> is null.</exception>
        public static Mirror ForObject(object instance)
        {
            return instance == null ? NullOfUnknownType : new Mirror(instance.GetType(), instance);
        }

        /// <summary>
        /// Gets an object that can access the mirror type's members with the specified name.
        /// </summary>
        /// <remarks>
        /// <para>
        /// If there are no members with the specified name, the member set will be
        /// empty and all operations performed on it will fail.  If there are multiple
        /// members with the specified name (when the name is overloaded), the member set
        /// will refer to multiple members.  When an operation is performed on the
        /// member set, a specific member will chosen from the set or an error will
        /// be emitted.
        /// </para>
        /// </remarks>
        /// <param name="memberName">The member name.</param>
        /// <returns>The member set that represents the mirror type's members with the specified name.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="memberName"/> is null.</exception>
        /// <exception cref="MirrorException">Thrown if the mirror represents a null object of unknown type.</exception>
        public MemberSet this[string memberName]
        {
            get
            {
                if (memberName == null)
                    throw new ArgumentNullException("memberName");

                return InternalGetMemberSet(memberName);
            }
        }

        /// <summary>
        /// Gets an object that can access the mirror type's constructors.
        /// </summary>
        /// <remarks>
        /// <para>
        /// If there are no constructors, the member set will be
        /// empty and all operations performed on it will fail.  If there are multiple
        /// constructors (when the constructor is overloaded), the member set
        /// will refer to multiple constructors.  When an operation is performed on the
        /// member set, a specific constructor will chosen from the set or an error will
        /// be emitted.
        /// </para>
        /// </remarks>
        /// <returns>The member set that represents the mirror type's constructors.</returns>
        /// <exception cref="MirrorException">Thrown if the mirror represents a null object of unknown type.</exception>
        public MemberSet Constructor
        {
            get { return InternalGetMemberSet(".ctor"); }
        }

        /// <summary>
        /// Gets an object that can access the mirror type's static constructors.
        /// </summary>
        /// <remarks>
        /// <para>
        /// If there are no static constructors, the member set will be
        /// empty and all operations performed on it will fail.
        /// </para>
        /// </remarks>
        /// <returns>The member set that represents the mirror type's static constructors.</returns>
        /// <exception cref="MirrorException">Thrown if the mirror represents a null object of unknown type.</exception>
        public MemberSet StaticConstructor
        {
            get { return InternalGetMemberSet(".cctor"); }
        }

        private MemberSet InternalGetMemberSet(string memberName)
        {
            if (IsNullOfUnknownType)
                throw new MirrorException("Cannot access the members of a mirror that represents a null object of unknown type.");

            return new MemberSet(type, instance, memberName, null, null, AnyBinding);
        }

        private static Exception Eval<T>(Func<T> func, out T result)
        {
            try
            {
                result = func();
                return null;
            }
            catch (Exception ex)
            {
                result = default(T);
                return ex;
            }
        }

        /// <summary>
        /// A member set provides reflection operations on members of an object or type.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The specific member that is chosen for a given operation depends on the operation arguments.
        /// For example, if a member set represents an overloaded method then the arguments to
        /// <see cref="Invoke(object[])"/> are used to select the specific overload to be invoked.  If the
        /// overload cannot be determined automatically from the arguments, the client should
        /// use the <see cref="WithSignature"/> method to obtain a restricted member set given the
        /// method signature of the specific overload to be used then calling <see cref="Invoke(object[])"/>
        /// on the resulting member set.
        /// </para>
        /// </remarks>
        public sealed class MemberSet
        {
            private readonly Type type;
            private readonly object instance;
            private readonly string memberName;
            private readonly Type[] signature;
            private readonly Type[] genericArgs;
            private readonly BindingFlags bindingFlags;

            internal MemberSet(Type type, object instance, string memberName, Type[] signature,
                Type[] genericArgs, BindingFlags bindingFlags)
            {
                this.type = type;
                this.instance = instance;
                this.memberName = memberName;
                this.signature = signature;
                this.genericArgs = genericArgs;
                this.bindingFlags = bindingFlags;
            }

            /// <summary>
            /// Gets the member info that the member set represents.
            /// </summary>
            /// <remarks>
            /// <para>
            /// This call will fail if the member set matches multiple members in which case
            /// you should use the <see cref="WithSignature"/> method to choose a particular overload.
            /// </para>
            /// </remarks>
            /// <value>The nested type.</value>
            /// <exception cref="MirrorException">Thrown if the operation failed because
            /// of a reflection problem such as when the member access is invalid or ambiguous.</exception>
            public MemberInfo MemberInfo
            {
                get { return InternalGetMemberInfo(); }
            }

            /// <summary>
            /// Gets the nested type that the member set represents.
            /// </summary>
            /// <value>The nested type.</value>
            /// <exception cref="MirrorException">Thrown if the operation failed because
            /// of a reflection problem such as when the member access is invalid or ambiguous.</exception>
            public Type NestedType
            {
                get { return InternalGetNestedType(); }
            }

            /// <summary>
            /// Gets the nested type that the member set represents as a mirror.
            /// </summary>
            /// <value>The nested type as a mirror.</value>
            /// <exception cref="MirrorException">Thrown if the operation failed because
            /// of a reflection problem such as when the member access is invalid or ambiguous.</exception>
            public Mirror NestedTypeAsMirror
            {
                get { return Mirror.ForType(InternalGetNestedType()); }
            }

            /// <summary>
            /// Gets an object that represents a particular index of a property whose value can be manipulated.
            /// </summary>
            /// <param name="indexArg">The index argument.</param>
            /// <returns>The property index.</returns>
            /// <exception cref="MirrorException">Thrown if the operation failed because
            /// of a reflection problem such as when the member access is invalid or ambiguous.</exception>
            public PropertyIndex this[object indexArg]
            {
                get { return InternalGetPropertyIndex(new[] { indexArg }); }
            }

            /// <summary>
            /// Gets an object that represents a particular index of a property whose value can be manipulated.
            /// </summary>
            /// <param name="indexArgs">The index arguments.</param>
            /// <returns>The property index.</returns>
            /// <exception cref="MirrorException">Thrown if the operation failed because
            /// of a reflection problem such as when the member access is invalid or ambiguous.</exception>
            public PropertyIndex this[params object[] indexArgs]
            {
                get
                {
                    if (indexArgs == null)
                        throw new ArgumentNullException("indexArgs");

                    return InternalGetPropertyIndex(indexArgs);
                }
            }

            /// <summary>
            /// Gets or sets the value of the field or property represented by the member set.
            /// </summary>
            /// <value>The value.</value>
            /// <exception cref="MirrorException">Thrown if the operation failed because
            /// of a reflection problem such as when the member access is invalid or ambiguous.</exception>
            public object Value
            {
                get { return InternalGetValue(); }
                set { InternalSetValue(value); }
            }

            /// <summary>
            /// Gets the value of the field or property represented by the member set as a mirror.
            /// </summary>
            /// <value>The value as a mirror.</value>
            /// <exception cref="MirrorException">Thrown if the operation failed because
            /// of a reflection problem such as when the member access is invalid or ambiguous.</exception>
            public Mirror ValueAsMirror
            {
                get { return Mirror.ForObject(InternalGetValue()); }
            }

            /// <summary>
            /// Invokes the method represented by the member set and returns its result.
            /// </summary>
            /// <remarks>
            /// <para>
            /// When the method set contains multiple overloads attempts to choose an
            /// overload that is compatible with the argument list or raises an exception
            /// if none is found.
            /// </para>
            /// </remarks>
            /// <returns>The method result.</returns>
            /// <exception cref="MirrorException">Thrown if the operation failed because
            /// of a reflection problem such as when the member access is invalid or ambiguous.</exception>
            public object Invoke()
            {
                return InternalInvoke(EmptyArray<object>.Instance);
            }

            /// <summary>
            /// Invokes the method represented by the member set and returns its result.
            /// </summary>
            /// <remarks>
            /// <para>
            /// When the method set contains multiple overloads attempts to choose an
            /// overload that is compatible with the argument list or raises an exception
            /// if none is found.
            /// </para>
            /// </remarks>
            /// <param name="arg">The method argument.</param>
            /// <returns>The method result.</returns>
            /// <exception cref="MirrorException">Thrown if the operation failed because
            /// of a reflection problem such as when the member access is invalid or ambiguous.</exception>
            public object Invoke(object arg)
            {
                return InternalInvoke(new[] { arg });
            }

            /// <summary>
            /// Invokes the method represented by the member set and returns its result.
            /// </summary>
            /// <remarks>
            /// <para>
            /// When the method set contains multiple overloads attempts to choose an
            /// overload that is compatible with the argument list or raises an exception
            /// if none is found.
            /// </para>
            /// </remarks>
            /// <param name="args">The method arguments.</param>
            /// <returns>The method result.</returns>
            /// <exception cref="ArgumentNullException">Thrown if <paramref name="args"/> is null.</exception>
            /// <exception cref="MirrorException">Thrown if the operation failed because
            /// of a reflection problem such as when the member access is invalid or ambiguous.</exception>
            public object Invoke(params object[] args)
            {
                if (args == null)
                    throw new ArgumentNullException("args");

                return InternalInvoke(args);
            }

            /// <summary>
            /// Invokes the method represented by the member set and returns its result as a mirror.
            /// </summary>
            /// <remarks>
            /// <para>
            /// When the method set contains multiple overloads attempts to choose an
            /// overload that is compatible with the argument list or raises an exception
            /// if none is found.
            /// </para>
            /// </remarks>
            /// <returns>The method result as a mirror.</returns>
            /// <exception cref="MirrorException">Thrown if the operation failed because
            /// of a reflection problem such as when the member access is invalid or ambiguous.</exception>
            public Mirror InvokeAsMirror()
            {
                return Mirror.ForObject(InternalInvoke(EmptyArray<object>.Instance));
            }

            /// <summary>
            /// Invokes the method represented by the member set and returns its result as a mirror.
            /// </summary>
            /// <remarks>
            /// <para>
            /// When the method set contains multiple overloads attempts to choose an
            /// overload that is compatible with the argument list or raises an exception
            /// if none is found.
            /// </para>
            /// </remarks>
            /// <param name="arg">The method argument.</param>
            /// <returns>The method result as a mirror.</returns>
            /// <exception cref="MirrorException">Thrown if the operation failed because
            /// of a reflection problem such as when the member access is invalid or ambiguous.</exception>
            public Mirror InvokeAsMirror(object arg)
            {
                return Mirror.ForObject(InternalInvoke(new[] { arg }));
            }

            /// <summary>
            /// Invokes the method represented by the member set and returns its result as a mirror.
            /// </summary>
            /// <remarks>
            /// <para>
            /// When the method set contains multiple overloads attempts to choose an
            /// overload that is compatible with the argument list or raises an exception
            /// if none is found.
            /// </para>
            /// </remarks>
            /// <param name="args">The method arguments.</param>
            /// <returns>The method result as a mirror.</returns>
            /// <exception cref="ArgumentNullException">Thrown if <paramref name="args"/> is null.</exception>
            /// <exception cref="MirrorException">Thrown if the operation failed because
            /// of a reflection problem such as when the member access is invalid or ambiguous.</exception>
            public Mirror InvokeAsMirror(params object[] args)
            {
                if (args == null)
                    throw new ArgumentNullException("args");

                return Mirror.ForObject(InternalInvoke(args));
            }

            /// <summary>
            /// Adds an event handler to the event represented by the member set.
            /// </summary>
            /// <param name="eventHandler">The event handler, or null if none.</param>
            /// <exception cref="MirrorException">Thrown if the operation failed because
            /// of a reflection problem such as when the member access is invalid or ambiguous.</exception>
            public void AddHandler(Delegate eventHandler)
            {
                InternalAddHandler(eventHandler);
            }

            /// <summary>
            /// Adds an event handler to the event represented by the member set.
            /// </summary>
            /// <param name="eventHandler">The event handler, or null if none.</param>
            /// <exception cref="MirrorException">Thrown if the operation failed because
            /// of a reflection problem such as when the member access is invalid or ambiguous.</exception>
            public void AddHandler(EventHandler eventHandler)
            {
                InternalAddHandler(eventHandler);
            }

            /// <summary>
            /// Adds an event handler to the event represented by the member set.
            /// </summary>
            /// <param name="eventHandler">The event handler, or null if none.</param>
            /// <exception cref="MirrorException">Thrown if the operation failed because
            /// of a reflection problem such as when the member access is invalid or ambiguous.</exception>
            public void AddHandler<TEventArgs>(EventHandler<TEventArgs> eventHandler)
                where TEventArgs : EventArgs
            {
                InternalAddHandler(eventHandler);
            }

            /// <summary>
            /// Removes an event handler to the event represented by the member set.
            /// </summary>
            /// <param name="eventHandler">The event handler, or null if none.</param>
            /// <exception cref="MirrorException">Thrown if the operation failed because
            /// of a reflection problem such as when the member access is invalid or ambiguous.</exception>
            public void RemoveHandler(Delegate eventHandler)
            {
                InternalRemoveHandler(eventHandler);
            }

            /// <summary>
            /// Removes an event handler to the event represented by the member set.
            /// </summary>
            /// <param name="eventHandler">The event handler, or null if none.</param>
            /// <exception cref="MirrorException">Thrown if the operation failed because
            /// of a reflection problem such as when the member access is invalid or ambiguous.</exception>
            public void RemoveHandler(EventHandler eventHandler)
            {
                InternalRemoveHandler(eventHandler);
            }

            /// <summary>
            /// Removes an event handler to the event represented by the member set.
            /// </summary>
            /// <param name="eventHandler">The event handler, or null if none.</param>
            /// <exception cref="MirrorException">Thrown if the operation failed because
            /// of a reflection problem such as when the member access is invalid or ambiguous.</exception>
            public void RemoveHandler<TEventArgs>(EventHandler<TEventArgs> eventHandler)
                where TEventArgs : EventArgs
            {
                InternalRemoveHandler(eventHandler);
            }

            /// <summary>
            /// Returns a new member set that selects members with the specified signature.
            /// This is particularly useful when the member is overloaded and cannot be resolved
            /// automatically.
            /// </summary>
            /// <param name="argTypes">The expected argument types, which may contain null elements
            /// as placeholders for unspecified types.</param>
            /// <returns>A member set that selects members with the specified signature.</returns>
            /// <exception cref="ArgumentNullException">Thrown if <paramref name="argTypes"/> is null.</exception>
            public MemberSet WithSignature(params Type[] argTypes)
            {
                if (argTypes == null)
                    throw new ArgumentNullException("argTypes");

                return InternalWithSignature(argTypes);
            }

            /// <summary>
            /// Returns a new member set that selects members with the specified generic arguments.
            /// This is particularly useful when the member has multiple generic overloads
            /// that cannot be resolved automatically.
            /// </summary>
            /// <param name="genericArgs">The expected generic argument types, which may contain null elements
            /// as placeholders for unspecified types.</param>
            /// <returns>A member set that selects members with the specified generic arguments.</returns>
            /// <exception cref="ArgumentNullException">Thrown if <paramref name="genericArgs"/> is null.</exception>
            public MemberSet WithGenericArgs(params Type[] genericArgs)
            {
                if (genericArgs == null)
                    throw new ArgumentNullException("genericArgs");

                return InternalWithGenericArgs(genericArgs);
            }

            /// <summary>
            /// Returns a new member set that selects members that match the specified binding flags.
            /// </summary>
            /// <param name="bindingFlags">The expected binding flags.</param>
            /// <returns>A member set that selects members that match the specified binding flags.</returns>
            public MemberSet WithBindingFlags(BindingFlags bindingFlags)
            {
                return InternalWithBindingFlags(bindingFlags);
            }

            private MemberSet InternalWithSignature(Type[] newSignature)
            {
                return new MemberSet(type, instance, memberName, newSignature, genericArgs, bindingFlags);
            }

            private MemberSet InternalWithGenericArgs(Type[] newGenericArgs)
            {
                return new MemberSet(type, instance, memberName, signature, newGenericArgs, bindingFlags);
            }

            private MemberSet InternalWithBindingFlags(BindingFlags newBindingFlags)
            {
                return new MemberSet(type, instance, memberName, signature, genericArgs, newBindingFlags);
            }

            private PropertyIndex InternalGetPropertyIndex(object[] indexArgs)
            {
                Type[] argTypes = GetTypes(indexArgs);
                var propertyInfo = (PropertyInfo) ResolveMember(MemberTypes.Property, "property", argTypes);
                return new PropertyIndex(propertyInfo, instance, indexArgs);
            }

            private object InternalGetValue()
            {
                MemberInfo memberInfo = ResolveMember(MemberTypes.Field | MemberTypes.Property, "field or property", null);

                try
                {
                    var fieldInfo = memberInfo as FieldInfo;
                    if (fieldInfo != null)
                        return fieldInfo.GetValue(instance);

                    var propertyInfo = (PropertyInfo)memberInfo;
                    return propertyInfo.GetValue(instance, null);
                }
                catch (TargetInvocationException ex)
                {
                    ExceptionUtils.RethrowWithNoStackTraceLoss(ex);
                    throw;
                }
                catch (Exception ex)
                {
                    throw new MirrorException(string.Format("Could not get value of field or property '{0}' of type '{1}'.",
                        memberInfo, memberInfo.ReflectedType), ex);
                }
            }

            private void InternalSetValue(object value)
            {
                MemberInfo memberInfo = ResolveMember(MemberTypes.Field | MemberTypes.Property, "field or property", null);

                try
                {
                    var fieldInfo = memberInfo as FieldInfo;
                    if (fieldInfo != null)
                    {
                        fieldInfo.SetValue(instance, value);
                    }
                    else
                    {
                        var propertyInfo = (PropertyInfo)memberInfo;
                        propertyInfo.SetValue(instance, value, null);
                    }
                }
                catch (TargetInvocationException ex)
                {
                    ExceptionUtils.RethrowWithNoStackTraceLoss(ex);
                    throw;
                }
                catch (Exception ex)
                {
                    throw new MirrorException(string.Format("Could not set value of field or property '{0}' of type '{1}'.",
                        memberInfo, memberInfo.ReflectedType), ex);
                }
            }

            private object InternalInvoke(object[] args)
            {
                Type[] argTypes = GetTypes(args);
                var methodBase = (MethodBase) ResolveMember(MemberTypes.Method | MemberTypes.Constructor, "method or constructor", argTypes);

                try
                {
                    var constructorInfo = methodBase as ConstructorInfo;
                    if (constructorInfo != null && ! constructorInfo.IsStatic)
                        return constructorInfo.Invoke(bindingFlags, null, args, null);

                    return methodBase.Invoke(instance, args);
                }
                catch (TargetInvocationException ex)
                {
                    ExceptionUtils.RethrowWithNoStackTraceLoss(ex);
                    throw;
                }
                catch (Exception ex)
                {
                    throw new MirrorException(string.Format("Could not invoke method or constructor '{0}' of type '{1}'.",
                        methodBase, methodBase.ReflectedType), ex);
                }
            }

            private void InternalAddHandler(Delegate eventHandler)
            {
                var eventInfo = (EventInfo)ResolveMember(MemberTypes.Event, "event", null);

                try
                {
                    Delegate coercedEventHandler = ProxyUtils.CoerceDelegate(eventInfo.EventHandlerType, eventHandler);

                    MethodInfo addMethod = eventInfo.GetAddMethod(true);
                    if (addMethod == null)
                        throw new MirrorException("Could not add handler because no add method exists for the event.");
                    addMethod.Invoke(instance, new object[] { coercedEventHandler });
                }
                catch (TargetInvocationException ex)
                {
                    ExceptionUtils.RethrowWithNoStackTraceLoss(ex);
                    throw;
                }
                catch (Exception ex)
                {
                    throw new MirrorException(string.Format("Could not add handler to event '{0}' of type '{1}'.",
                        eventInfo, eventInfo.ReflectedType), ex);
                }
            }

            private void InternalRemoveHandler(Delegate eventHandler)
            {
                var eventInfo = (EventInfo)ResolveMember(MemberTypes.Event, "event", null);

                try
                {
                    Delegate coercedEventHandler = ProxyUtils.CoerceDelegate(eventInfo.EventHandlerType, eventHandler);

                    MethodInfo removeMethod = eventInfo.GetRemoveMethod(true);
                    if (removeMethod == null)
                        throw new MirrorException("Could not remove handler because no remove method exists for the event.");
                    removeMethod.Invoke(instance, new object[] { coercedEventHandler });
                }
                catch (TargetInvocationException ex)
                {
                    ExceptionUtils.RethrowWithNoStackTraceLoss(ex);
                    throw;
                }
                catch (Exception ex)
                {
                    throw new MirrorException(string.Format("Could not remove handler from event '{0}' of type '{1}'.",
                        eventInfo, eventInfo.ReflectedType), ex);
                }
            }

            private Type InternalGetNestedType()
            {
                return (Type)ResolveMember(MemberTypes.NestedType, "nested type", null);
            }

            private MemberInfo InternalGetMemberInfo()
            {
                return ResolveMember(MemberTypes.All, "member", null);
            }

            private MemberInfo ResolveMember(MemberTypes memberTypes, string memberTypeDescription, Type[] expectedArgTypes)
            {
                MemberInfo[] memberInfos;
                Exception ex = Eval(() => ReflectionUtils.FindMembersWorkaround(type, memberTypes, bindingFlags, IsMemberWithMatchingName, null), out memberInfos);

                if (ex != null || memberInfos.Length == 0)
                    throw new MirrorException(string.Format("Could not find any {0} '{1}' of type '{2}'.",
                        memberTypeDescription, memberName, type), ex);

                MemberInfo match = null;
                int matchCount = 0;
                foreach (MemberInfo memberInfo in memberInfos)
                {
                    MemberInfo resolvedMemberInfo = ResolveMemberIfMatch(memberInfo, expectedArgTypes);
                    if (resolvedMemberInfo != null)
                    {
                        match = resolvedMemberInfo;
                        matchCount += 1;
                    }
                }

                if (matchCount == 1)
                    return match;

                throw new MirrorException(string.Format("Could not find a unique matching {0} '{1}' of type '{2}'.  There were {3} matches out of {4} members with the same name.  Try providing additional information to narrow down the choices.",
                    memberTypeDescription, memberName, type, matchCount, memberInfos.Length));
            }

            private bool IsMemberWithMatchingName(MemberInfo memberInfo, object state)
            {
                return memberInfo.Name == memberName;
            }

            private MemberInfo ResolveMemberIfMatch(MemberInfo memberInfo, Type[] expectedArgTypes)
            {
                ParameterInfo[] parameters;
                Type[] genericParameters;
                GetMemberParameters(memberInfo, out parameters, out genericParameters);

                if (signature != null && signature.Length != parameters.Length)
                    return null;
                if (genericArgs != null && genericArgs.Length != genericParameters.Length)
                    return null;
                if (expectedArgTypes != null && expectedArgTypes.Length != parameters.Length)
                    return null;

                Type[] resolvedGenericArgs;
                if (genericParameters.Length != 0)
                {
                    resolvedGenericArgs = new Type[genericParameters.Length];

                    for (int i = 0; i < genericParameters.Length; i++)
                    {
                        if (genericArgs != null && genericArgs[i] != null)
                            resolvedGenericArgs[i] = genericArgs[i];
                        else
                            resolvedGenericArgs[i] = genericParameters[i];
                    }
                }
                else
                {
                    resolvedGenericArgs = null;
                }

                for (int i = 0; i < parameters.Length; i++)
                {
                    Type parameterType = parameters[i].ParameterType;
                    if (parameterType.IsGenericParameter)
                    {
                        parameterType = resolvedGenericArgs[parameterType.GenericParameterPosition];

                        if (parameterType.IsGenericParameter && expectedArgTypes != null)
                            resolvedGenericArgs[parameterType.GenericParameterPosition] = expectedArgTypes[i];
                    }

                    if (expectedArgTypes != null && !parameterType.IsAssignableFrom(expectedArgTypes[i]))
                        return null; // parameter is not assignable
                }

                if (resolvedGenericArgs != null)
                {
                    if (Array.IndexOf(resolvedGenericArgs, null) >= 0)
                        return null; // missing necessary generic argument information

                    try
                    {
                        MethodInfo methodInfo = (MethodInfo)memberInfo;
                        return methodInfo.MakeGenericMethod(resolvedGenericArgs);
                    }
                    catch (ArgumentException)
                    {
                        // Does not satisfy the generic parameter constraints.
                        return null;
                    }
                }

                return memberInfo;
            }

            private static Type[] GetTypes(object[] objs)
            {
                return Array.ConvertAll(objs, x => x != null ? x.GetType() : null);
            }

            private static void GetMemberParameters(MemberInfo memberInfo,
                out ParameterInfo[] parameters, out Type[] genericParameters)
            {
                switch (memberInfo.MemberType)
                {
                    case MemberTypes.Property:
                        var propertyInfo = (PropertyInfo)memberInfo;
                        parameters = propertyInfo.GetIndexParameters();
                        genericParameters = Type.EmptyTypes;
                        break;
                    case MemberTypes.Constructor:
                        var constructorInfo = (ConstructorInfo)memberInfo;
                        parameters = constructorInfo.GetParameters();
                        genericParameters = Type.EmptyTypes;
                        break;
                    case MemberTypes.Method:
                        var methodInfo = (MethodInfo)memberInfo;
                        parameters = methodInfo.GetParameters();
                        genericParameters = methodInfo.ContainsGenericParameters ? methodInfo.GetGenericArguments() : Type.EmptyTypes;
                        break;
                    case MemberTypes.Event:
                    case MemberTypes.Field:
                    case MemberTypes.TypeInfo:
                    case MemberTypes.Custom:
                    case MemberTypes.NestedType:
                        parameters = EmptyArray<ParameterInfo>.Instance;
                        genericParameters = Type.EmptyTypes;
                        break;
                    default:
                        throw new NotSupportedException();
                }
            }
        }

        /// <summary>
        /// A property index provides a means of getting or setting the value of an indexed property
        /// using particular index arguments.
        /// </summary>
        public sealed class PropertyIndex
        {
            private readonly PropertyInfo propertyInfo;
            private readonly object instance;
            private readonly object[] indexArgs;

            internal PropertyIndex(PropertyInfo propertyInfo, object instance, object[] indexArgs)
            {
                this.propertyInfo = propertyInfo;
                this.instance = instance;
                this.indexArgs = indexArgs;
            }

            /// <summary>
            /// Gets or sets the value of the indexed property.
            /// </summary>
            /// <value>The value.</value>
            /// <exception cref="MirrorException">Thrown if the operation failed because
            /// of a reflection problem such as when the member access is invalid or ambiguous.</exception>
            public object Value
            {
                get { return InternalGetValue(); }
                set { InternalSetValue(value); }
            }

            /// <summary>
            /// Gets the value of the indexed property as a mirror.
            /// </summary>
            /// <value>The value as a mirror.</value>
            /// <exception cref="MirrorException">Thrown if the operation failed because
            /// of a reflection problem such as when the member access is invalid or ambiguous.</exception>
            public Mirror ValueAsMirror
            {
                get { return Mirror.ForObject(InternalGetValue()); }
            }

            private object InternalGetValue()
            {
                try
                {
                    return propertyInfo.GetValue(instance, indexArgs);
                }
                catch (TargetInvocationException ex)
                {
                    ExceptionUtils.RethrowWithNoStackTraceLoss(ex);
                    throw;
                }
                catch (Exception ex)
                {
                    throw new MirrorException(string.Format("Could not get value of indexed property '{0}' of type '{1}'",
                        propertyInfo, propertyInfo.ReflectedType), ex);
                }
            }

            private void InternalSetValue(object value)
            {
                try
                {
                    propertyInfo.SetValue(instance, value, indexArgs);
                }
                catch (TargetInvocationException ex)
                {
                    ExceptionUtils.RethrowWithNoStackTraceLoss(ex);
                    throw;
                }
                catch (Exception ex)
                {
                    throw new MirrorException(string.Format("Could not set value of indexed property '{0}' of type '{1}'",
                        propertyInfo, propertyInfo.ReflectedType), ex);
                }
            }
        }
    }
}
