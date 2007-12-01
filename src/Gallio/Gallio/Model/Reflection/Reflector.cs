// Copyright 2007 MbUnit Project - http://www.mbunit.com/
// Portions Copyright 2000-2004 Jonathan De Halleux, Jamie Cansdale
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
using System.Diagnostics;
using System.Reflection;
using System.Security.Permissions;
using Gallio.Hosting;
using Gallio.Utilities;

namespace Gallio.Model.Reflection
{
    /// <summary>
    /// Wraps reflection types using the reflection adapter interfaces.
    /// Also resolves code references.
    /// </summary>
    public static class Reflector
    {
        /// <summary>
        /// Resolves the specified <see cref="CodeReference" />. 
        /// </summary>
        /// <param name="reference">The code reference, or null</param>
        /// <param name="throwOnError">If true, throws an exception on error</param>
        /// <returns>The associated code element, or null if the code reference
        /// is null or if it is of kind <see cref="CodeReferenceKind.Unknown" /></returns>
        /// <exception cref="InvalidOperationException">Thrown if <paramref name="throwOnError"/>
        /// is true and <paramref name="reference"/> cannot be resolved</exception>
        public static ICodeElementInfo Resolve(CodeReference reference, bool throwOnError)
        {
            if (reference == null)
                return null;

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
        /// <param name="target">The assembly, or null if none</param>
        /// <returns>The reflection wrapper, or null if none</returns>
        public static IAssemblyInfo Wrap(Assembly target)
        {
            return target != null ? new AssemblyWrapper(target) : null;
        }

        /// <summary>
        /// Obtains a reflection wrapper for a namespace.
        /// </summary>
        /// <param name="name">The namespace name, or null if none</param>
        /// <returns>The reflection wrapper, or null if none</returns>
        public static INamespaceInfo WrapNamespace(string name)
        {
            return name != null ? new NamespaceWrapper(name) : null;
        }

        /// <summary>
        /// Obtains a reflection wrapper for a type.
        /// </summary>
        /// <param name="target">The type, or null if none</param>
        /// <returns>The reflection wrapper, or null if none</returns>
        public static ITypeInfo Wrap(Type target)
        {
            return target != null ? new TypeWrapper(target) : null;
        }

        /// <summary>
        /// Obtains a reflection wrapper for a member.
        /// </summary>
        /// <param name="target">The member, or null if none</param>
        /// <returns>The reflection wrapper, or null if none</returns>
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
        /// <param name="target">The function, or null if none</param>
        /// <returns>The reflection wrapper, or null if none</returns>
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
        /// <param name="target">The constructor, or null if none</param>
        /// <returns>The reflection wrapper, or null if none</returns>
        public static IConstructorInfo Wrap(ConstructorInfo target)
        {
            return target != null ? new ConstructorWrapper(target) : null;
        }

        /// <summary>
        /// Obtains a reflection wrapper for a method.
        /// </summary>
        /// <param name="target">The method, or null if none</param>
        /// <returns>The reflection wrapper, or null if none</returns>
        public static IMethodInfo Wrap(MethodInfo target)
        {
            return target != null ? new MethodWrapper(target) : null;
        }

        /// <summary>
        /// Obtains a reflection wrapper for a property.
        /// </summary>
        /// <param name="target">The property, or null if none</param>
        /// <returns>The reflection wrapper, or null if none</returns>
        public static IPropertyInfo Wrap(PropertyInfo target)
        {
            return target != null ? new PropertyWrapper(target) : null;
        }

        /// <summary>
        /// Obtains a reflection wrapper for a field.
        /// </summary>
        /// <param name="target">The field, or null if none</param>
        /// <returns>The reflection wrapper, or null if none</returns>
        public static IFieldInfo Wrap(FieldInfo target)
        {
            return target != null ? new FieldWrapper(target) : null;
        }

        /// <summary>
        /// Obtains a reflection wrapper for an event.
        /// </summary>
        /// <param name="target">The event, or null if none</param>
        /// <returns>The reflection wrapper, or null if none</returns>
        public static IEventInfo Wrap(EventInfo target)
        {
            return target != null ? new EventWrapper(target) : null;
        }

        /// <summary>
        /// Obtains a reflection wrapper for a parameter.
        /// </summary>
        /// <param name="target">The parameter, or null if none</param>
        /// <returns>The reflection wrapper, or null if none</returns>
        public static IParameterInfo Wrap(ParameterInfo target)
        {
            return target != null ? new ParameterWrapper(target) : null;
        }

        /// <summary>
        /// Obtains a reflection wrapper for an attribute instance.
        /// </summary>
        /// <param name="target">The attribute, or null if none</param>
        /// <returns>The reflection wrapper, or null if none</returns>
        public static IAttributeInfo Wrap(Attribute target)
        {
            return target != null ? new AttributeWrapper(target) : null;
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
        /// <code>
        /// [NonInlined(SecurityAction.Demand)]
        /// public void Foo()
        /// {
        ///     IMethodInfo r = NativeReflector.GetExecutingFunction();
        ///     // ...
        /// }
        /// </code>
        /// </para>
        /// </remarks>
        /// <returns>The code reference</returns>
        [NonInlined(SecurityAction.Demand)]
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
        /// <code>
        /// [NonInlined(SecurityAction.Demand)]
        /// public void Foo()
        /// {
        ///     IMethodInfo r = NativeReflector.GetCallingFunction();
        ///     // ...
        /// }
        /// </code>
        /// </para>
        /// </remarks>
        /// <returns>The code reference</returns>
        [NonInlined(SecurityAction.Demand)]
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
        /// <returns>The code reference</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="framesToSkip"/> is less than zero</exception>
        [NonInlined(SecurityAction.Demand)]
        public static IFunctionInfo GetFunctionFromStackFrame(int framesToSkip)
        {
            if (framesToSkip < 0)
                throw new ArgumentOutOfRangeException("framesToSkip", framesToSkip, "Must be zero or more.");

            StackTrace stackTrace = new StackTrace(framesToSkip + 1, false);
            StackFrame stackFrame = stackTrace.GetFrame(0);
            return Wrap(stackFrame.GetMethod());
        }

        private abstract class CodeElementWrapper<TTarget> : ICodeElementInfo
            where TTarget : class, ICustomAttributeProvider
        {
            private readonly TTarget target;

            public CodeElementWrapper(TTarget target)
            {
                if (target == null)
                    throw new ArgumentNullException(@"target");

                this.target = target;
            }

            public TTarget Target
            {
                get { return target; }
            }

            public abstract string Name { get; }

            public abstract CodeReference CodeReference { get; }

            public IEnumerable<IAttributeInfo> GetAttributeInfos(bool inherit)
            {
                foreach (Attribute attrib in Target.GetCustomAttributes(inherit))
                    yield return Wrap(attrib);
            }

            public bool HasAttribute(Type attributeType, bool inherit)
            {
                return Target.IsDefined(attributeType, inherit);
            }

            public IEnumerable<object> GetAttributes(Type attributeType, bool inherit)
            {
                return Target.GetCustomAttributes(attributeType, inherit);
            }

            public abstract string GetXmlDocumentation();

            public override string ToString()
            {
                return Target.ToString();
            }
        }

        private sealed class AssemblyWrapper : CodeElementWrapper<Assembly>, IAssemblyInfo
        {
            public AssemblyWrapper(Assembly target)
                : base(target)
            {
            }

            public override string Name
            {
                get { return Target.GetName().Name; }
            }

            public override CodeReference CodeReference
            {
                get { return CodeReference.CreateFromAssembly(Target); }
            }

            public string Path
            {
                get { return Loader.GetFriendlyAssemblyCodeBase(Target); }
            }

            public string FullName
            {
                get { return Target.FullName; }
            }

            public AssemblyName GetName()
            {
                return Target.GetName();
            }

            public AssemblyName[] GetReferencedAssemblies()
            {
                return Target.GetReferencedAssemblies();
            }

            public ITypeInfo[] GetExportedTypes()
            {
                Type[] types = Target.GetExportedTypes();
                return Array.ConvertAll<Type, ITypeInfo>(types, Wrap);
            }

            public ITypeInfo GetType(string typeName)
            {
                return Wrap(Target.GetType(typeName));
            }

            public Assembly Resolve()
            {
                return Target;
            }

            public override string GetXmlDocumentation()
            {
                return null;
            }
        }

        private abstract class MemberWrapper<TTarget> : CodeElementWrapper<TTarget>, IMemberInfo
            where TTarget : MemberInfo
        {
            public MemberWrapper(TTarget target)
                : base(target)
            {
            }

            public override string Name
            {
                get { return Target.Name; }
            }

            public virtual string CompoundName
            {
                get
                {
                    ITypeInfo declaringType = DeclaringType;
                    return declaringType != null ? declaringType.CompoundName + @"." + Name : Name;
                }
            }

            public override CodeReference CodeReference
            {
                get { return CodeReference.CreateFromMember(Target); }
            }

            public ITypeInfo DeclaringType
            {
                get { return Wrap(Target.DeclaringType); }
            }

            public MemberInfo Resolve()
            {
                return Target;
            }
        }

        private sealed class TypeWrapper : MemberWrapper<Type>, ITypeInfo
        {
            public TypeWrapper(Type target)
                : base(target)
            {
            }

            public override CodeReference CodeReference
            {
                get { return CodeReference.CreateFromType(Target); }
            }

            public IAssemblyInfo Assembly
            {
                get { return Wrap(Target.Assembly); }
            }

            public INamespaceInfo Namespace
            {
                get { return WrapNamespace(Target.Namespace); }
            }

            public ITypeInfo BaseType
            {
                get { return Wrap(Target.BaseType); }
            }

            public ITypeInfo ElementType
            {
                get { return Wrap(Target.GetElementType()); }
            }

            public bool IsArray
            {
                get { return Target.IsArray; }
            }

            public bool IsPointer
            {
                get { return Target.IsPointer; }
            }

            public bool IsByRef
            {
                get { return Target.IsByRef; }
            }

            public bool IsGenericParameter
            {
                get { return Target.IsGenericParameter; }
            }

            public int ArrayRank
            {
                get
                {
                    if (! Target.IsArray)
                        throw new InvalidOperationException("Not an array type.");
                    return Target.GetArrayRank();
                }
            }

            public string AssemblyQualifiedName
            {
                get { return Target.AssemblyQualifiedName; }
            }

            public string FullName
            {
                get { return Target.FullName; }
            }

            public TypeAttributes Modifiers
            {
                get { return Target.Attributes; }
            }

            public ITypeInfo[] GetInterfaces()
            {
                Type[] interfaces = Target.GetInterfaces();
                return Array.ConvertAll<Type, ITypeInfo>(interfaces, Wrap);
            }

            public IConstructorInfo[] GetConstructors(BindingFlags bindingFlags)
            {
                ConstructorInfo[] constructors = Target.GetConstructors(bindingFlags);
                return Array.ConvertAll<ConstructorInfo, IConstructorInfo>(constructors, Wrap);
            }

            public IMethodInfo GetMethod(string methodName, BindingFlags bindingFlags)
            {
                return Wrap(Target.GetMethod(methodName, bindingFlags));
            }

            public IMethodInfo[] GetMethods(BindingFlags bindingFlags)
            {
                MethodInfo[] methods = Target.GetMethods(bindingFlags);
                return Array.ConvertAll<MethodInfo, IMethodInfo>(methods, Wrap);
            }

            public IPropertyInfo[] GetProperties(BindingFlags bindingFlags)
            {
                PropertyInfo[] properties = Target.GetProperties(bindingFlags);
                return Array.ConvertAll<PropertyInfo, IPropertyInfo>(properties, Wrap);
            }

            public IFieldInfo[] GetFields(BindingFlags bindingFlags)
            {
                FieldInfo[] fields = Target.GetFields(bindingFlags);
                return Array.ConvertAll<FieldInfo, IFieldInfo>(fields, Wrap);
            }

            public IEventInfo[] GetEvents(BindingFlags bindingFlags)
            {
                EventInfo[] events = Target.GetEvents(bindingFlags);
                return Array.ConvertAll<EventInfo, IEventInfo>(events, Wrap);
            }

            public bool IsAssignableFrom(ITypeInfo type)
            {
                return Target.IsAssignableFrom(type.Resolve());
            }

            new public Type Resolve()
            {
                return Target;
            }

            public override string GetXmlDocumentation()
            {
                return Loader.XmlDocumentationResolver.GetXmlDocumentation(Target);
            }
        }

        private class FunctionWrapper<TTarget> : MemberWrapper<TTarget>, IFunctionInfo
            where TTarget : MethodBase
        {
            public FunctionWrapper(TTarget target)
                : base(target)
            {
            }

            public MethodAttributes Modifiers
            {
                get { return Target.Attributes; }
            }

            public IParameterInfo[] GetParameters()
            {
                ParameterInfo[] parameters = Target.GetParameters();
                return Array.ConvertAll<ParameterInfo, IParameterInfo>(parameters, Wrap);
            }

            new public MethodBase Resolve()
            {
                return Target;
            }

            public override string GetXmlDocumentation()
            {
                return Loader.XmlDocumentationResolver.GetXmlDocumentation(Target);
            }
        }

        private sealed class ConstructorWrapper : FunctionWrapper<ConstructorInfo>, IConstructorInfo
        {
            public ConstructorWrapper(ConstructorInfo target)
                : base(target)
            {
            }

            new public ConstructorInfo Resolve()
            {
                return Target;
            }
        }

        private sealed class MethodWrapper : FunctionWrapper<MethodInfo>, IMethodInfo
        {
            public MethodWrapper(MethodInfo target)
                : base(target)
            {
            }

            public ITypeInfo ReturnType
            {
                get { return Wrap(Target.ReturnType); }
            }

            new public MethodInfo Resolve()
            {
                return Target;
            }
        }

        private sealed class PropertyWrapper : MemberWrapper<PropertyInfo>, IPropertyInfo
        {
            public PropertyWrapper(PropertyInfo target)
                : base(target)
            {
            }

            public ITypeInfo ValueType
            {
                get { return Wrap(Target.PropertyType); }
            }

            public int Position
            {
                get { return 0; }
            }

            public IMethodInfo GetGetMethod()
            {
                return Wrap(Target.GetGetMethod());
            }

            public IMethodInfo GetSetMethod()
            {
                return Wrap(Target.GetSetMethod());
            }

            new public PropertyInfo Resolve()
            {
                return Target;
            }

            public override string GetXmlDocumentation()
            {
                return Loader.XmlDocumentationResolver.GetXmlDocumentation(Target);
            }
        }

        private sealed class FieldWrapper : MemberWrapper<FieldInfo>, IFieldInfo
        {
            public FieldWrapper(FieldInfo target)
                : base(target)
            {
            }

            public ITypeInfo ValueType
            {
                get { return Wrap(Target.FieldType); }
            }

            public int Position
            {
                get { return 0; }
            }

            new public FieldInfo Resolve()
            {
                return Target;
            }

            public override string GetXmlDocumentation()
            {
                return Loader.XmlDocumentationResolver.GetXmlDocumentation(Target);
            }
        }

        private sealed class EventWrapper : MemberWrapper<EventInfo>, IEventInfo
        {
            public EventWrapper(EventInfo target)
                : base(target)
            {
            }

            new public EventInfo Resolve()
            {
                return Target;
            }

            public override string GetXmlDocumentation()
            {
                return Loader.XmlDocumentationResolver.GetXmlDocumentation(Target);
            }
        }

        private sealed class ParameterWrapper : CodeElementWrapper<ParameterInfo>, IParameterInfo
        {
            public ParameterWrapper(ParameterInfo target)
                : base(target)
            {
            }

            public override string Name
            {
                get { return Target.Name; }
            }

            public override CodeReference CodeReference
            {
                get { return CodeReference.CreateFromParameter(Target); }
            }

            public ITypeInfo ValueType
            {
                get { return Wrap(Target.ParameterType); }
            }

            public int Position
            {
                get { return Target.Position; }
            }

            public IMemberInfo Member
            {
                get { return Wrap(Target.Member); }
            }

            public ParameterAttributes Modifiers
            {
                get { return Target.Attributes; }
            }

            public ParameterInfo Resolve()
            {
                return Target;
            }

            public override string GetXmlDocumentation()
            {
                return null;
            }
        }

        private sealed class NamespaceWrapper : INamespaceInfo
        {
            private readonly string name;

            public NamespaceWrapper(string name)
            {
                if (name == null)
                    throw new ArgumentNullException("name");

                this.name = name;
            }

            public string Name
            {
                get { return name; }
            }

            public CodeReference CodeReference
            {
                get { return CodeReference.CreateFromNamespace(name); }
            }

            public IEnumerable<IAttributeInfo> GetAttributeInfos(bool inherit)
            {
                yield break;
            }

            public bool HasAttribute(Type attributeType, bool inherit)
            {
                return false;
            }

            public IEnumerable<object> GetAttributes(Type attributeType, bool inherit)
            {
                yield break;
            }

            public string GetXmlDocumentation()
            {
                return null;
            }

            public override string ToString()
            {
                return name;
            }
        }

        private sealed class AttributeWrapper : IAttributeInfo
        {
            private readonly Attribute attrib;

            public AttributeWrapper(Attribute attrib)
            {
                if (attrib == null)
                    throw new ArgumentNullException("attrib");

                this.attrib = attrib;
            }

            public ITypeInfo Type
            {
                get { return Wrap(attrib.GetType()); }
            }

            public IConstructorInfo Constructor
            {
                get { throw new NotImplementedException(); }
            }

            public object[] ArgumentValues
            {
                get { throw new NotImplementedException(); }
            }

            public IDictionary<IFieldInfo, object> FieldValues
            {
                get { throw new NotImplementedException(); }
            }

            public IDictionary<IPropertyInfo, object> PropertyValues
            {
                get { throw new NotImplementedException(); }
            }
        }
    }
}
