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
using System.Reflection;
using Gallio.Collections;
using Gallio.Model.Reflection;
using JetBrains.Metadata.Access;
using JetBrains.Metadata.Reader.API;

namespace Gallio.ReSharperRunner.Reflection
{
    /// <summary>
    /// Wraps ReSharper metadata types using the reflection adapter interfaces.
    /// </summary>
    /// <todo author="jeff">
    /// Support inherited attribute lookup.
    /// Support Resolve() method.
    /// </todo>
    public static class MetadataReflector
    {
        /// <summary>
        /// Obtains a reflection wrapper for an assembly.
        /// </summary>
        /// <param name="target">The assembly, or null if none</param>
        /// <returns>The reflection wrapper, or null if none</returns>
        public static IAssemblyInfo Wrap(IMetadataAssembly target)
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
            return Reflector.WrapNamespace(name);
        }

        /// <summary>
        /// Obtains a reflection wrapper for a type.
        /// </summary>
        /// <param name="target">The type, or null if none</param>
        /// <returns>The reflection wrapper, or null if none</returns>
        public static ITypeInfo Wrap(IMetadataType target)
        {
            if (target == null)
                return null;

            IMetadataClassType classType = target as IMetadataClassType;
            if (classType != null)
                return new ClassTypeWrapper(classType);

            IMetadataArrayType arrayType = target as IMetadataArrayType;
            if (arrayType != null)
                return new ArrayTypeWrapper(arrayType);

            IMetadataPointerType pointerType = target as IMetadataPointerType;
            if (pointerType != null)
                return new PointerTypeWrapper(pointerType);

            IMetadataReferenceType referenceType = target as IMetadataReferenceType;
            if (referenceType != null)
                return new ReferenceTypeWrapper(referenceType);

            IMetadataGenericArgumentReferenceType argumentType = target as IMetadataGenericArgumentReferenceType;
            if (argumentType != null)
                return new GenericArgumentTypeWrapper(argumentType);

            throw new NotSupportedException("Unrecognized type.");
        }

        /// <summary>
        /// Obtains a reflection wrapper for a class type without
        /// specifying any generic arguments.
        /// </summary>
        /// <param name="target">The type, or null if none</param>
        /// <returns>The reflection wrapper, or null if none</returns>
        public static ITypeInfo WrapOpenType(IMetadataTypeInfo target)
        {
            return target != null ? new ClassTypeWrapper(new OpenClassType(target)) : null;
        }

        /// <summary>
        /// Obtains a reflection wrapper for a function.
        /// </summary>
        /// <param name="target">The function, or null if none</param>
        /// <returns>The reflection wrapper, or null if none</returns>
        public static IFunctionInfo Wrap(IMetadataMethod target)
        {
            if (target == null)
                return null;

            return IsConstructorName(target.Name) ? (IFunctionInfo) new ConstructorWrapper(target) : new MethodWrapper(target);
        }

        /// <summary>
        /// Obtains a reflection wrapper for a method.
        /// </summary>
        /// <param name="target">The method, or null if none</param>
        /// <returns>The reflection wrapper, or null if none</returns>
        public static IMethodInfo WrapMethod(IMetadataMethod target)
        {
            return target != null ? new MethodWrapper(target) : null;
        }

        /// <summary>
        /// Obtains a reflection wrapper for a constructor.
        /// </summary>
        /// <param name="target">The method, or null if none</param>
        /// <returns>The reflection wrapper, or null if none</returns>
        public static IConstructorInfo WrapConstructor(IMetadataMethod target)
        {
            return target != null ? new ConstructorWrapper(target) : null;
        }

        /// <summary>
        /// Obtains a reflection wrapper for a property.
        /// </summary>
        /// <param name="target">The property, or null if none</param>
        /// <returns>The reflection wrapper, or null if none</returns>
        public static IPropertyInfo Wrap(IMetadataProperty target)
        {
            return target != null ? new PropertyWrapper(target) : null;
        }

        /// <summary>
        /// Obtains a reflection wrapper for a field.
        /// </summary>
        /// <param name="target">The field, or null if none</param>
        /// <returns>The reflection wrapper, or null if none</returns>
        public static IFieldInfo Wrap(IMetadataField target)
        {
            return target != null ? new FieldWrapper(target) : null;
        }

        /// <summary>
        /// Obtains a reflection wrapper for an event.
        /// </summary>
        /// <param name="target">The event, or null if none</param>
        /// <returns>The reflection wrapper, or null if none</returns>
        public static IEventInfo Wrap(IMetadataEvent target)
        {
            return target != null ? new EventWrapper(target) : null;
        }

        /// <summary>
        /// Obtains a reflection wrapper for a parameter.
        /// </summary>
        /// <param name="target">The parameter, or null if none</param>
        /// <returns>The reflection wrapper, or null if none</returns>
        public static IParameterInfo Wrap(IMetadataParameter target)
        {
            return target != null ? new ParameterWrapper(target) : null;
        }

        /// <summary>
        /// Obtains a reflection wrapper for an attribute.
        /// </summary>
        /// <param name="target">The attribute, or null if none</param>
        /// <returns>The reflection wrapper, or null if none</returns>
        public static IAttributeInfo Wrap(IMetadataCustomAttribute target)
        {
            return target != null ? new AttributeWrapper(target) : null;
        }

        private static bool IsConstructorName(string name)
        {
            return name == ".ctor" || name == ".cctor";
        }

        public static IEnumerable<IAttributeInfo> GetAttributeInfosForEntity(IMetadataEntity entity, bool inherit)
        {
            foreach (IMetadataCustomAttribute attrib in entity.CustomAttributes)
                yield return Wrap(attrib);
        }

        private abstract class CodeElementWrapper<TTarget> : ICodeElementInfo
            where TTarget : class
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

            public abstract IEnumerable<IAttributeInfo> GetAttributeInfos(bool inherit);

            public IEnumerable<IAttributeInfo> GetAttributeInfos(Type attributeType, bool inherit)
            {
                return AttributeUtils.FilterAttributesOfType(GetAttributeInfos(inherit), attributeType);
            }

            public bool HasAttribute(Type attributeType, bool inherit)
            {
                return AttributeUtils.ContainsAttributeOfType(GetAttributeInfos(inherit), attributeType);
            }

            public IEnumerable<object> GetAttributes(bool inherit)
            {
                return AttributeUtils.ResolveAttributes(GetAttributeInfos(inherit));
            }

            public IEnumerable<object> GetAttributes(Type attributeType, bool inherit)
            {
                return AttributeUtils.ResolveAttributesOfType(GetAttributeInfos(inherit), attributeType);
            }

            public string GetXmlDocumentation()
            {
                return null;
            }

            public override string ToString()
            {
                return Target.ToString();
            }
        }

        private sealed class AssemblyWrapper : CodeElementWrapper<IMetadataAssembly>, IAssemblyInfo
        {
            public AssemblyWrapper(IMetadataAssembly target)
                : base(target)
            {
            }

            public override string Name
            {
                get { return Target.AssemblyName.Name; }
            }

            public override CodeReference CodeReference
            {
                get { return new CodeReference(FullName, null, null, null, null); }
            }

            public string Path
            {
                get { return Target.Location; }
            }

            public string FullName
            {
                get { return Target.AssemblyName.FullName; }
            }

            public AssemblyName GetName()
            {
                return Target.AssemblyName;
            }

            public IList<AssemblyName> GetReferencedAssemblies()
            {
                AssemblyReference[] references = Target.ReferencedAssembliesNames;
                return Array.ConvertAll<AssemblyReference, AssemblyName>(references, delegate(AssemblyReference reference)
                {
                    return reference.AssemblyName;
                });
            }

            public IList<ITypeInfo> GetExportedTypes()
            {
                IMetadataTypeInfo[] types = Target.GetTypes();
                return Array.ConvertAll<IMetadataTypeInfo, ITypeInfo>(types, WrapOpenType);
            }

            public ITypeInfo GetType(string typeName)
            {
                return Wrap(Target.GetTypeFromQualifiedName(typeName, false));
            }

            public override IEnumerable<IAttributeInfo> GetAttributeInfos(bool inherit)
            {
                return GetAttributeInfosForEntity(Target, inherit);
            }

            public Assembly Resolve()
            {
                return Assembly.LoadFrom(Path);
            }
        }

        private abstract class MemberWrapper<TTarget> : CodeElementWrapper<TTarget>, IMemberInfo
            where TTarget : class, IMetadataEntity
        {
            public MemberWrapper(TTarget target)
                : base(target)
            {
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
                get
                {
                    CodeReference reference = DeclaringType.CodeReference;
                    reference.MemberName = Name;
                    return reference;
                }
            }

            public abstract ITypeInfo DeclaringType { get; }

            public override IEnumerable<IAttributeInfo> GetAttributeInfos(bool inherit)
            {
                return GetAttributeInfosForEntity(Target, inherit);
            }

            MemberInfo IMemberInfo.Resolve()
            {
                return ResolveMemberInfo();
            }

            public abstract MemberInfo ResolveMemberInfo();
        }

        private abstract class TypeWrapper<TTarget> : CodeElementWrapper<TTarget>, ITypeInfo
            where TTarget : class, IMetadataType
        {
            public TypeWrapper(TTarget target)
                : base(target)
            {
            }

            public string AssemblyQualifiedName
            {
                get { return FullName + ", " + Assembly.FullName; }
            }

            public override CodeReference CodeReference
            {
                get { return new CodeReference(Assembly.FullName, Namespace.Name, FullName, null, null); }
            }

            public virtual ITypeInfo ElementType
            {
                get { return null; }
            }

            public virtual int ArrayRank
            {
                get { throw new InvalidOperationException("Not an array type."); }
            }

            public virtual bool IsArray
            {
                get { return false; }
            }

            public virtual bool IsPointer
            {
                get { return false; }
            }

            public virtual bool IsByRef
            {
                get { return false; }
            }

            public virtual bool IsGenericParameter
            {
                get { return false; }
            }

            public abstract string CompoundName { get; }
            public abstract ITypeInfo DeclaringType { get; }
            public abstract IAssemblyInfo Assembly { get; }
            public abstract INamespaceInfo Namespace { get; }
            public abstract ITypeInfo BaseType { get; }
            public abstract string FullName { get; }
            public abstract TypeAttributes Modifiers { get; }
            public abstract IList<ITypeInfo> GetInterfaces();
            public abstract IList<IConstructorInfo> GetConstructors(BindingFlags bindingFlags);
            public abstract IMethodInfo GetMethod(string methodName, BindingFlags bindingFlags);
            public abstract IList<IMethodInfo> GetMethods(BindingFlags bindingFlags);
            public abstract IList<IPropertyInfo> GetProperties(BindingFlags bindingFlags);
            public abstract IList<IFieldInfo> GetFields(BindingFlags bindingFlags);
            public abstract IList<IEventInfo> GetEvents(BindingFlags bindingFlags);
            public abstract bool IsAssignableFrom(ITypeInfo type);
            public abstract Type Resolve();

            MemberInfo IMemberInfo.Resolve()
            {
                return Resolve();
            }
        }

        private sealed class ClassTypeWrapper : TypeWrapper<IMetadataClassType>
        {
            public ClassTypeWrapper(IMetadataClassType target)
                : base(target)
            {
            }

            public override string Name
            {
                get { return FullName.Substring(0, FullName.LastIndexOf('.')); }
            }

            public override string CompoundName
            {
                get
                {
                    ITypeInfo declaringType = DeclaringType;
                    return declaringType != null ? declaringType.CompoundName + @"." + Name : Name;
                }
            }

            public override ITypeInfo DeclaringType
            {
                get { return WrapOpenType(TargetInfo.DeclaringType); }
            }

            public override IAssemblyInfo Assembly
            {
                get
                {
                    // FIXME: HACK!  We really need the IAssemblyResolver to do this correctly.
                    FieldInfo myAssemblyField = TargetInfo.GetType().GetField("myAssembly", BindingFlags.Instance | BindingFlags.NonPublic);
                    IMetadataAssembly assembly = (IMetadataAssembly)myAssemblyField.GetValue(TargetInfo);

                    return Wrap(assembly);
                }
            }

            public override INamespaceInfo Namespace
            {
                get { return WrapNamespace(NamespaceName); }
            }

            public override ITypeInfo BaseType
            {
                get { return Wrap(TargetInfo.Base); }
            }

            public override string FullName
            {
                get { return Target.Type.FullyQualifiedName; }
            }

            public override TypeAttributes Modifiers
            {
                get
                {
                    IMetadataTypeInfo info = TargetInfo;
                    TypeAttributes flags = 0;
                    AddFlagIfTrue(ref flags, TypeAttributes.Abstract, info.IsAbstract);
                    AddFlagIfTrue(ref flags, TypeAttributes.Class, info.IsClass);
                    AddFlagIfTrue(ref flags, TypeAttributes.Interface, info.IsInterface);
                    AddFlagIfTrue(ref flags, TypeAttributes.NestedAssembly, info.IsNestedAssembly);
                    AddFlagIfTrue(ref flags, TypeAttributes.NestedFamily, info.IsNestedFamily);
                    AddFlagIfTrue(ref flags, TypeAttributes.NestedFamANDAssem, info.IsNestedFamilyAndAssembly);
                    AddFlagIfTrue(ref flags, TypeAttributes.NestedFamORAssem, info.IsNestedFamilyOrAssembly);
                    AddFlagIfTrue(ref flags, TypeAttributes.NestedPrivate, info.IsNestedPrivate);
                    AddFlagIfTrue(ref flags, TypeAttributes.NestedPublic, info.IsNestedPublic);
                    AddFlagIfTrue(ref flags, TypeAttributes.Sealed, info.IsSealed);
                    AddFlagIfTrue(ref flags, TypeAttributes.Serializable, info.IsSerializable);
                    AddFlagIfTrue(ref flags, TypeAttributes.SpecialName, info.IsSpecialName);
                    return flags;
                }
            }

            public override IList<ITypeInfo> GetInterfaces()
            {
                IMetadataClassType[] interfaces = TargetInfo.Interfaces;
                return Array.ConvertAll<IMetadataClassType, ITypeInfo>(interfaces, Wrap);
            }

            public override IList<IConstructorInfo> GetConstructors(BindingFlags bindingFlags)
            {
                List<IConstructorInfo> result = new List<IConstructorInfo>();

                foreach (IMetadataMethod method in TargetInfo.GetMethods())
                {
                    if (IsConstructorName(method.Name) && CheckBindingFlags(bindingFlags, method.IsPublic, method.IsStatic))
                        result.Add(WrapConstructor(method));
                }

                return result;
            }

            public override IMethodInfo GetMethod(string methodName, BindingFlags bindingFlags)
            {
                IMetadataMethod match = null;
                foreach (IMetadataMethod method in TargetInfo.GetMethods())
                {
                    if (method.Name == methodName && CheckBindingFlags(bindingFlags, method.IsPublic, method.IsStatic))
                    {
                        if (match != null)
                            throw new AmbiguousMatchException("Found two matching methods with the same name.");

                        match = method;
                    }
                }

                return (IMethodInfo)Wrap(match);
            }

            public override IList<IMethodInfo> GetMethods(BindingFlags bindingFlags)
            {
                List<IMethodInfo> result = new List<IMethodInfo>();

                foreach (IMetadataMethod method in TargetInfo.GetMethods())
                {
                    if (!IsConstructorName(method.Name) && CheckBindingFlags(bindingFlags, method.IsPublic, method.IsStatic))
                        result.Add(WrapMethod(method));
                }

                return result;
            }

            public override IList<IPropertyInfo> GetProperties(BindingFlags bindingFlags)
            {
                List<IPropertyInfo> result = new List<IPropertyInfo>();

                foreach (IMetadataProperty property in TargetInfo.GetProperties())
                {
                    IMetadataMethod method = property.Getter ?? property.Setter;
                    if (CheckBindingFlags(bindingFlags, method.IsPublic, method.IsStatic))
                        result.Add(Wrap(property));
                }

                return result;
            }

            public override IList<IFieldInfo> GetFields(BindingFlags bindingFlags)
            {
                List<IFieldInfo> result = new List<IFieldInfo>();

                foreach (IMetadataField field in TargetInfo.GetFields())
                {
                    if (CheckBindingFlags(bindingFlags, field.IsPublic, field.IsStatic))
                        result.Add(Wrap(field));
                }

                return result;
            }

            public override IList<IEventInfo> GetEvents(BindingFlags bindingFlags)
            {
                List<IEventInfo> result = new List<IEventInfo>();

                foreach (IMetadataEvent @event in TargetInfo.GetEvents())
                {
                    IMetadataMethod method = @event.Adder ?? @event.Remover ?? @event.Raiser;

                    if (CheckBindingFlags(bindingFlags, method.IsPublic, method.IsStatic))
                        result.Add(Wrap(@event));
                }

                return result;
            }

            public override bool IsAssignableFrom(ITypeInfo type)
            {
                throw new NotImplementedException();
            }

            public override IEnumerable<IAttributeInfo> GetAttributeInfos(bool inherit)
            {
                return GetAttributeInfosForEntity(TargetInfo, inherit);
            }

            public override Type Resolve()
            {
                throw new NotImplementedException();
            }

            private string NamespaceName
            {
                get { return FullName.Substring(FullName.LastIndexOf('.') + 1); }
            }

            private IMetadataTypeInfo TargetInfo
            {
                get { return Target.Type; }
            }

            private static void AddFlagIfTrue(ref TypeAttributes flags, TypeAttributes flagToAdd, bool condition)
            {
                if (condition)
                    flags |= flagToAdd;
            }

            private static bool CheckBindingFlags(BindingFlags flags, bool @public, bool @static)
            {
                return (@public && (flags & BindingFlags.Public) != 0
                    || !@public && (flags & BindingFlags.NonPublic) != 0)
                    && (@static && (flags & BindingFlags.Static) != 0
                    || !@static && (flags & BindingFlags.Instance) != 0);
            }
        }

        private abstract class ConstructedTypeWrapper<TTarget> : TypeWrapper<TTarget>
            where TTarget : class, IMetadataType
        {
            public ConstructedTypeWrapper(TTarget target)
                : base(target)
            {
            }

            public abstract ITypeInfo EffectiveClassType { get; }

            public override string CompoundName
            {
                get { return Name; }
            }

            public override ITypeInfo DeclaringType
            {
                get { return null; }
            }

            public override IAssemblyInfo Assembly
            {
                get { return ElementType.Assembly; }
            }

            public override INamespaceInfo Namespace
            {
                get { return ElementType.Namespace; }
            }

            public override ITypeInfo BaseType
            {
                get { return null; }
            }

            public override string FullName
            {
                get { return Name; }
            }

            public override TypeAttributes Modifiers
            {
                get { return EffectiveClassType.Modifiers; }
            }

            public override IList<ITypeInfo> GetInterfaces()
            {
                return EffectiveClassType.GetInterfaces();
            }

            public override IList<IConstructorInfo> GetConstructors(BindingFlags bindingFlags)
            {
                return EffectiveClassType.GetConstructors(bindingFlags);
            }

            public override IMethodInfo GetMethod(string methodName, BindingFlags bindingFlags)
            {
                return EffectiveClassType.GetMethod(methodName, bindingFlags);
            }

            public override IList<IMethodInfo> GetMethods(BindingFlags bindingFlags)
            {
                return EffectiveClassType.GetMethods(bindingFlags);
            }

            public override IList<IPropertyInfo> GetProperties(BindingFlags bindingFlags)
            {
                return EffectiveClassType.GetProperties(bindingFlags);
            }

            public override IList<IFieldInfo> GetFields(BindingFlags bindingFlags)
            {
                return EffectiveClassType.GetFields(bindingFlags);
            }

            public override IList<IEventInfo> GetEvents(BindingFlags bindingFlags)
            {
                return EffectiveClassType.GetEvents(bindingFlags);
            }

            public override bool IsAssignableFrom(ITypeInfo type)
            {
                throw new NotImplementedException();
            }

            public override IEnumerable<IAttributeInfo> GetAttributeInfos(bool inherit)
            {
                yield break;
            }

            public override Type Resolve()
            {
                return EffectiveClassType.Resolve();
            }
        }

        private sealed class ArrayTypeWrapper : ConstructedTypeWrapper<IMetadataArrayType>
        {
            public ArrayTypeWrapper(IMetadataArrayType target)
                : base(target)
            {
            }

            public override string Name
            {
                get { return ElementType.Name + "[]"; }
            }

            public override ITypeInfo ElementType
            {
                get { return Wrap(Target.ElementType); }
            }

            public override int ArrayRank
            {
                get { return checked((int) Target.Rank); }
            }

            public override bool IsArray
            {
                get { return true; }
            }

            public override ITypeInfo EffectiveClassType
            {
                get
                {
                    // Return System.Array.
                    throw new NotImplementedException();
                }
            }

            public override Type Resolve()
            {
                int rank = checked((int) Target.Rank);
                Type elementType = ElementType.Resolve();

                if (rank == 1)
                    return elementType.MakeArrayType();
                else
                    return elementType.MakeArrayType(rank);
            }
        }

        private sealed class GenericArgumentTypeWrapper : ConstructedTypeWrapper<IMetadataGenericArgumentReferenceType>
        {
            public GenericArgumentTypeWrapper(IMetadataGenericArgumentReferenceType target)
                : base(target)
            {
            }

            public override string Name
            {
                get { return Target.Argument.Name; }
            }

            public override bool IsGenericParameter
            {
                get { return true; }
            }

            public override ITypeInfo EffectiveClassType
            {
                get
                {
                    // FIXME: In actuality we can treat this case as producing a type whose members
                    //        are the union of the classes or interfaces in the generic type parameter constraint.
                    throw new NotSupportedException("Cannot perform this operation on a generic type parameter.");
                }
            }

            public override Type Resolve()
            {
                throw new NotImplementedException();
            }
        }

        private sealed class PointerTypeWrapper : ConstructedTypeWrapper<IMetadataPointerType>
        {
            public PointerTypeWrapper(IMetadataPointerType target)
                : base(target)
            {
            }

            public override string Name
            {
                get { return ElementType.Name + "*"; }
            }

            public override ITypeInfo ElementType
            {
                get { return Wrap(Target.Type); }
            }

            public override bool IsPointer
            {
                get { return true; }
            }

            public override ITypeInfo EffectiveClassType
            {
                get
                {
                    // Return System.Pointer.
                    throw new NotImplementedException();
                }
            }

            public override Type Resolve()
            {
                return ElementType.Resolve().MakePointerType();
            }
        }

        private sealed class ReferenceTypeWrapper : ConstructedTypeWrapper<IMetadataReferenceType>
        {
            public ReferenceTypeWrapper(IMetadataReferenceType target)
                : base(target)
            {
            }

            public override string Name
            {
                get { return ElementType.Name + "&"; }
            }

            public override ITypeInfo ElementType
            {
                get { return Wrap(Target.Type); }
            }

            public override bool IsByRef
            {
                get{ return true; }
            }

            public override ITypeInfo EffectiveClassType
            {
                get
                {
                    // Return System.TypedReference.
                    throw new NotImplementedException();
                }
            }

            public override Type Resolve()
            {
                throw new NotImplementedException();
            }
        }

        private abstract class FunctionWrapper : MemberWrapper<IMetadataMethod>, IFunctionInfo
        {
            public FunctionWrapper(IMetadataMethod target)
                : base(target)
            {
            }

            public override string Name
            {
                get { return Target.Name; }
            }

            public override ITypeInfo DeclaringType
            {
                get { return WrapOpenType(Target.DeclaringType); }
            }

            public MethodAttributes Modifiers
            {
                get
                {
                    MethodAttributes flags = 0;
                    AddFlagIfTrue(ref flags, MethodAttributes.Abstract, Target.IsAbstract);
                    AddFlagIfTrue(ref flags, MethodAttributes.Assembly, Target.IsAssembly);
                    AddFlagIfTrue(ref flags, MethodAttributes.Family, Target.IsFamily);
                    AddFlagIfTrue(ref flags, MethodAttributes.FamANDAssem, Target.IsFamilyAndAssembly);
                    AddFlagIfTrue(ref flags, MethodAttributes.FamORAssem, Target.IsFamilyOrAssembly);
                    AddFlagIfTrue(ref flags, MethodAttributes.Final, Target.IsFinal);
                    AddFlagIfTrue(ref flags, MethodAttributes.HideBySig, Target.IsHideBySig);
                    AddFlagIfTrue(ref flags, MethodAttributes.NewSlot, Target.IsNewSlot);
                    AddFlagIfTrue(ref flags, MethodAttributes.Private, Target.IsPrivate);
                    AddFlagIfTrue(ref flags, MethodAttributes.Public, Target.IsPublic);
                    AddFlagIfTrue(ref flags, MethodAttributes.SpecialName, Target.IsSpecialName);
                    AddFlagIfTrue(ref flags, MethodAttributes.Static, Target.IsStatic);
                    AddFlagIfTrue(ref flags, MethodAttributes.Virtual, Target.IsVirtual);
                    return flags;
                }
            }

            public IList<IParameterInfo> GetParameters()
            {
                IMetadataParameter[] parameters = Target.Parameters;
                return Array.ConvertAll<IMetadataParameter, IParameterInfo>(parameters, Wrap);
            }

            public override MemberInfo ResolveMemberInfo()
            {
                return ResolveMethodBase();
            }

            MethodBase IFunctionInfo.Resolve()
            {
                return ResolveMethodBase();
            }

            public abstract MethodBase ResolveMethodBase();

            private static void AddFlagIfTrue(ref MethodAttributes flags, MethodAttributes flagToAdd, bool condition)
            {
                if (condition)
                    flags |= flagToAdd;
            }
        }

        private sealed class ConstructorWrapper : FunctionWrapper, IConstructorInfo
        {
            public ConstructorWrapper(IMetadataMethod target)
                : base(target)
            {
                if (!IsConstructorName(target.Name))
                    throw new ArgumentException("target");
            }

            public override MethodBase ResolveMethodBase()
            {
                return Resolve();
            }

            public ConstructorInfo Resolve()
            {
                throw new NotImplementedException();
            }
        }

        private sealed class MethodWrapper : FunctionWrapper, IMethodInfo
        {
            public MethodWrapper(IMetadataMethod target)
                : base(target)
            {
                if (IsConstructorName(target.Name))
                    throw new ArgumentException("target");
            }

            public ITypeInfo ReturnType
            {
                get { return Wrap(Target.ReturnValue.Type); }
            }

            public override MethodBase ResolveMethodBase()
            {
                return Resolve();
            }

            public MethodInfo Resolve()
            {
                throw new NotImplementedException();
            }
        }

        private sealed class PropertyWrapper : MemberWrapper<IMetadataProperty>, IPropertyInfo
        {
            public PropertyWrapper(IMetadataProperty target)
                : base(target)
            {
            }

            public override string Name
            {
                get { return Target.Name; }
            }

            public override ITypeInfo DeclaringType
            {
                get { return WrapOpenType(Target.DeclaringType); }
            }

            public ITypeInfo ValueType
            {
                get { return Wrap(Target.Type); }
            }

            public int Position
            {
                get { return 0; }
            }

            public IMethodInfo GetGetMethod()
            {
                return WrapMethod(Target.Getter);
            }

            public IMethodInfo GetSetMethod()
            {
                return WrapMethod(Target.Setter);
            }

            public override MemberInfo ResolveMemberInfo()
            {
                return Resolve();
            }

            public PropertyInfo Resolve()
            {
                throw new NotImplementedException();
            }
        }

        private sealed class FieldWrapper : MemberWrapper<IMetadataField>, IFieldInfo
        {
            public FieldWrapper(IMetadataField target)
                : base(target)
            {
            }

            public ITypeInfo ValueType
            {
                get { return Wrap(Target.Type); }
            }

            public int Position
            {
                get { return 0; }
            }

            public override ITypeInfo DeclaringType
            {
                get { return WrapOpenType(Target.DeclaringType); }
            }

            public override string Name
            {
                get { return Target.Name; }
            }

            public override MemberInfo ResolveMemberInfo()
            {
                return Resolve();
            }

            public FieldInfo Resolve()
            {
                throw new NotImplementedException();
            }
        }

        private sealed class EventWrapper : MemberWrapper<IMetadataEvent>, IEventInfo
        {
            public EventWrapper(IMetadataEvent target)
                : base(target)
            {
            }

            public override ITypeInfo DeclaringType
            {
                get { return WrapOpenType(Target.DeclaringType); }
            }

            public override string Name
            {
                get { return Target.Name; }
            }

            public override MemberInfo ResolveMemberInfo()
            {
                return Resolve();
            }

            public EventInfo Resolve()
            {
                throw new NotImplementedException();
            }
        }

        private sealed class ParameterWrapper : CodeElementWrapper<IMetadataParameter>, IParameterInfo
        {
            public ParameterWrapper(IMetadataParameter target)
                : base(target)
            {
            }

            public override string Name
            {
                get { return Target.Name; }
            }

            public override CodeReference CodeReference
            {
                get
                {
                    CodeReference reference = Member.CodeReference;
                    reference.ParameterName = Name;
                    return reference;
                }
            }

            public ITypeInfo ValueType
            {
                get { return Wrap(Target.Type); }
            }

            public int Position
            {
                get
                {
                    return Array.IndexOf(Target.DeclaringMethod.Parameters, Target);
                }
            }

            public IMemberInfo Member
            {
                get { return Wrap(Target.DeclaringMethod); }
            }

            public ParameterAttributes Modifiers
            {
                get
                {
                    ParameterAttributes flags = 0;
                    AddFlagIfTrue(ref flags, ParameterAttributes.In, Target.IsIn);
                    AddFlagIfTrue(ref flags, ParameterAttributes.Out, Target.IsOut);
                    AddFlagIfTrue(ref flags, ParameterAttributes.Optional, Target.IsOptional);
                    return flags;
                }
            }

            public override IEnumerable<IAttributeInfo> GetAttributeInfos(bool inherit)
            {
                return GetAttributeInfosForEntity(Target, inherit);
            }

            public ParameterInfo Resolve()
            {
                throw new NotImplementedException();
            }

            private static void AddFlagIfTrue(ref ParameterAttributes flags, ParameterAttributes flagToAdd, bool condition)
            {
                if (condition)
                    flags |= flagToAdd;
            }
        }

        private sealed class AttributeWrapper : IAttributeInfo
        {
            private readonly IMetadataCustomAttribute attrib;

            public AttributeWrapper(IMetadataCustomAttribute attrib)
            {
                if (attrib == null)
                    throw new ArgumentNullException("attrib");

                this.attrib = attrib;
            }


            public ITypeInfo Type
            {
                get { return WrapOpenType(attrib.UsedConstructor.DeclaringType); }
            }

            public IConstructorInfo Constructor
            {
                get { return WrapConstructor(attrib.UsedConstructor); }
            }

            public object[] ArgumentValues
            {
                get { return attrib.ConstructorArguments; }
            }

            public object GetFieldValue(string name)
            {
                foreach (IMetadataCustomAttributeFieldInitialization initialization in attrib.InitializedFields)
                    if (initialization.Field.Name == name)
                        return initialization.Value;

                throw new ArgumentException(String.Format("The attribute does not have an initialized field named '{0}'.", name));
            }

            public object GetPropertyValue(string name)
            {
                foreach (IMetadataCustomAttributePropertyInitialization initialization in attrib.InitializedProperties)
                    if (initialization.Property.Name == name)
                        return initialization.Value;

                throw new ArgumentException(String.Format("The attribute does not have an initialized property named '{0}'.", name));
            }

            public IDictionary<IFieldInfo, object> FieldValues
            {
                get
                {
                    IMetadataCustomAttributeFieldInitialization[] initializations = attrib.InitializedFields;
                    Dictionary<IFieldInfo, object> values = new Dictionary<IFieldInfo, object>(initializations.Length);

                    foreach (IMetadataCustomAttributeFieldInitialization initialization in initializations)
                        values.Add(Wrap(initialization.Field), initialization.Value);

                    return values;
                }
            }

            public IDictionary<IPropertyInfo, object> PropertyValues
            {
                get
                {
                    IMetadataCustomAttributePropertyInitialization[] initializations = attrib.InitializedProperties;
                    Dictionary<IPropertyInfo, object> values = new Dictionary<IPropertyInfo, object>(initializations.Length);

                    foreach (IMetadataCustomAttributePropertyInitialization initialization in initializations)
                        values.Add(Wrap(initialization.Property), initialization.Value);

                    return values;
                }
            }

            public object Resolve()
            {
                return AttributeUtils.CreateAttribute(this);
            }
        }

        private sealed class OpenClassType : IMetadataClassType
        {
            private readonly IMetadataTypeInfo type;

            public OpenClassType(IMetadataTypeInfo type)
            {
                this.type = type;
            }

            public IMetadataTypeInfo Type
            {
                get { return type; }
            }

            public IMetadataType[] Arguments
            {
                get { return EmptyArray<IMetadataType>.Instance; }
            }

            public string PresentableName
            {
                get { return Type.FullyQualifiedName; }
            }

            public IMetadataTypeInfo[] OptionalModifiers
            {
                get { return EmptyArray<IMetadataTypeInfo>.Instance; }
            }

            public IMetadataTypeInfo[] RequiredModifiers
            {
                get { return EmptyArray<IMetadataTypeInfo>.Instance; }
            }
        }
    }
}
