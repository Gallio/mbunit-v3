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
using JetBrains.Metadata.Reader.Impl;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Caches;
using JetBrains.Shell;

namespace Gallio.ReSharperRunner.Reflection
{
    /// <summary>
    /// Wraps ReSharper metadata types using the reflection adapter interfaces.
    /// </summary>
    /// <todo author="jeff">
    /// Support inherited attribute lookup.
    /// Support Resolve() method.
    /// </todo>
    public class MetadataReflector
    {
        private readonly IProject contextProject;

        /// <summary>
        /// Creates a reflector with the specified project as its context.
        /// The context project is used to resolve metadata items to declared elements.
        /// </summary>
        /// <param name="contextProject">The context project, or null if none</param>
        public MetadataReflector(IProject contextProject)
        {
            this.contextProject = contextProject;
        }

        /// <summary>
        /// Gets the context project, or null if none.
        /// </summary>
        public IProject ContextProject
        {
            get { return contextProject; }
        }

        /// <summary>
        /// Obtains a reflection wrapper for an assembly.
        /// </summary>
        /// <param name="target">The assembly, or null if none</param>
        /// <returns>The reflection wrapper, or null if none</returns>
        public IAssemblyInfo Wrap(IMetadataAssembly target)
        {
            return target != null ? new AssemblyWrapper(this, target) : null;
        }

        /// <summary>
        /// Obtains a reflection wrapper for a namespace.
        /// </summary>
        /// <param name="name">The namespace name, or null if none</param>
        /// <returns>The reflection wrapper, or null if none</returns>
        public INamespaceInfo WrapNamespace(string name)
        {
            return Reflector.WrapNamespace(name);
        }

        /// <summary>
        /// Obtains a reflection wrapper for a type.
        /// </summary>
        /// <param name="target">The type, or null if none</param>
        /// <returns>The reflection wrapper, or null if none</returns>
        public ITypeInfo Wrap(IMetadataType target)
        {
            if (target == null)
                return null;

            IMetadataClassType classType = target as IMetadataClassType;
            if (classType != null)
                return new ClassTypeWrapper(this, classType);

            IMetadataArrayType arrayType = target as IMetadataArrayType;
            if (arrayType != null)
                return new ArrayTypeWrapper(this, arrayType);

            IMetadataPointerType pointerType = target as IMetadataPointerType;
            if (pointerType != null)
                return new PointerTypeWrapper(this, pointerType);

            IMetadataReferenceType referenceType = target as IMetadataReferenceType;
            if (referenceType != null)
                return new ReferenceTypeWrapper(this, referenceType);

            IMetadataGenericArgumentReferenceType argumentType = target as IMetadataGenericArgumentReferenceType;
            if (argumentType != null)
                return new GenericParameterWrapper(this, argumentType);

            throw new NotSupportedException("Unrecognized type.");
        }

        /// <summary>
        /// Obtains a reflection wrapper for a class type without
        /// specifying any generic arguments.
        /// </summary>
        /// <param name="target">The type, or null if none</param>
        /// <returns>The reflection wrapper, or null if none</returns>
        public ITypeInfo WrapOpenType(IMetadataTypeInfo target)
        {
            return target != null ? new ClassTypeWrapper(this, new OpenClassType(target)) : null;
        }

        /// <summary>
        /// Obtains a reflection wrapper for a function.
        /// </summary>
        /// <param name="target">The function, or null if none</param>
        /// <returns>The reflection wrapper, or null if none</returns>
        public IFunctionInfo Wrap(IMetadataMethod target)
        {
            if (target == null)
                return null;

            return IsConstructor(target) ? (IFunctionInfo) new ConstructorWrapper(this, target) : new MethodWrapper(this, target);
        }

        /// <summary>
        /// Obtains a reflection wrapper for a method.
        /// </summary>
        /// <param name="target">The method, or null if none</param>
        /// <returns>The reflection wrapper, or null if none</returns>
        public IMethodInfo WrapMethod(IMetadataMethod target)
        {
            return target != null ? new MethodWrapper(this, target) : null;
        }

        /// <summary>
        /// Obtains a reflection wrapper for a constructor.
        /// </summary>
        /// <param name="target">The method, or null if none</param>
        /// <returns>The reflection wrapper, or null if none</returns>
        public IConstructorInfo WrapConstructor(IMetadataMethod target)
        {
            return target != null ? new ConstructorWrapper(this, target) : null;
        }

        /// <summary>
        /// Obtains a reflection wrapper for a property.
        /// </summary>
        /// <param name="target">The property, or null if none</param>
        /// <returns>The reflection wrapper, or null if none</returns>
        public IPropertyInfo Wrap(IMetadataProperty target)
        {
            return target != null ? new PropertyWrapper(this, target) : null;
        }

        /// <summary>
        /// Obtains a reflection wrapper for a field.
        /// </summary>
        /// <param name="target">The field, or null if none</param>
        /// <returns>The reflection wrapper, or null if none</returns>
        public IFieldInfo Wrap(IMetadataField target)
        {
            return target != null ? new FieldWrapper(this, target) : null;
        }

        /// <summary>
        /// Obtains a reflection wrapper for an event.
        /// </summary>
        /// <param name="target">The event, or null if none</param>
        /// <returns>The reflection wrapper, or null if none</returns>
        public IEventInfo Wrap(IMetadataEvent target)
        {
            return target != null ? new EventWrapper(this, target) : null;
        }

        /// <summary>
        /// Obtains a reflection wrapper for a parameter.
        /// </summary>
        /// <param name="target">The parameter, or null if none</param>
        /// <returns>The reflection wrapper, or null if none</returns>
        public IParameterInfo Wrap(IMetadataParameter target)
        {
            return target != null ? new ParameterWrapper(this, target) : null;
        }

        /// <summary>
        /// Obtains a reflection wrapper for a generic parameter.
        /// </summary>
        /// <param name="target">The generic parameter, or null if none</param>
        /// <returns>The reflection wrapper, or null if none</returns>
        public IGenericParameterInfo Wrap(IMetadataGenericArgument target)
        {
            return target != null ? new GenericParameterWrapper(this, new MetadataGenericArgumentReferenceType(target)) : null;
        }

        /// <summary>
        /// Obtains a reflection wrapper for an attribute.
        /// </summary>
        /// <param name="target">The attribute, or null if none</param>
        /// <returns>The reflection wrapper, or null if none</returns>
        public IAttributeInfo Wrap(IMetadataCustomAttribute target)
        {
            return target != null ? new AttributeWrapper(this, target) : null;
        }

        private static bool IsConstructor(IMetadataMethod method)
        {
            string name = method.Name;
            return name == ".ctor" || name == ".cctor";
        }

        private IEnumerable<IAttributeInfo> GetAttributeInfosForEntity(IMetadataEntity entity, bool inherit)
        {
            // TODO: Support attribute inheritance.
            foreach (IMetadataCustomAttribute attrib in entity.CustomAttributes)
                yield return Wrap(attrib);
        }

        private IDeclarationsCache GetDeclarationsCache()
        {
            return PsiManager.GetInstance(contextProject.GetSolution()).
                GetDeclarationsCache(DeclarationsCacheScope.ProjectScope(contextProject, true), true);
        }

        private ITypeElement GetDeclaredElementWithLock(IMetadataTypeInfo type)
        {
            if (contextProject != null)
            {
                IDeclarationsCache cache = GetDeclarationsCache();

                // TODO: Verify expected assembly name in case there are multiple types with
                //       the same name distinguished only by declaring assembly.
                return cache.GetTypeElementByCLRName(type.FullyQualifiedName);
            }

            return null;
        }

        private IFunction GetDeclaredElementWithLock(IMetadataMethod metadataMethod)
        {
            ITypeElement type = GetDeclaredElementWithLock(metadataMethod.DeclaringType);

            if (type != null)
            {
                if (IsConstructor(metadataMethod))
                {
                    foreach (IConstructor constructor in type.Constructors)
                    {
                        if (constructor.IsStatic == metadataMethod.IsStatic
                            && IsSameSignature(constructor.Parameters, metadataMethod.Parameters))
                            return constructor;
                    }
                }
                else
                {
                    foreach (IMethod method in type.Methods)
                    {
                        if (method.IsStatic == metadataMethod.IsStatic
                            && method.ShortName == metadataMethod.Name
                            && IsSameSignature(method.Parameters, metadataMethod.Parameters))
                            return method;
                    }
                }
            }

            return null;
        }

        private static bool IsSameSignature(IList<IParameter> parameters, IMetadataParameter[] metadataParameters)
        {
            if (parameters.Count != metadataParameters.Length)
                return false;

            for (int i = 0; i < metadataParameters.Length; i++)
            {
                IParameter parameter = parameters[i];
                IMetadataParameter metadataParameter = metadataParameters[i];

                if (parameter.ShortName != metadataParameter.Name
                    || parameter.IsOptional != metadataParameter.IsOptional
                    || (parameter.Kind != ParameterKind.OUTPUT) != metadataParameter.IsIn
                    || (parameter.Kind != ParameterKind.VALUE) != metadataParameter.IsOut
                    || parameter.Type.GetPresentableName(PsiLanguageType.UNKNOWN) != metadataParameter.Type.PresentableName)
                    return false;
            }

            return true;
        }

        private IProperty GetDeclaredElementWithLock(IMetadataProperty metadataProperty)
        {
            ITypeElement type = GetDeclaredElementWithLock(metadataProperty.DeclaringType);

            if (type != null)
            {
                // TODO: Handle overloaded indexer properties.
                foreach (IProperty property in type.Properties)
                    if (property.ShortName == metadataProperty.Name)
                        return property;
            }

            return null;
        }

        private IField GetDeclaredElementWithLock(IMetadataField metadataField)
        {
            IClass type = GetDeclaredElementWithLock(metadataField.DeclaringType) as IClass;

            if (type != null)
            {
                foreach (IField field in type.Fields)
                    if (field.ShortName == metadataField.Name)
                        return field;
            }

            return null;
        }

        private IEvent GetDeclaredElementWithLock(IMetadataEvent metadataEvent)
        {
            ITypeElement type = GetDeclaredElementWithLock(metadataEvent.DeclaringType);

            if (type != null)
            {
                foreach (IEvent @event in type.Events)
                    if (@event.ShortName == metadataEvent.Name)
                        return @event;
            }

            return null;
        }

        private IParameter GetDeclaredElementWithLock(IMetadataParameter metadataParameter)
        {
            IFunction function = GetDeclaredElementWithLock(metadataParameter.DeclaringMethod);

            if (function != null)
            {
                foreach (IParameter parameter in function.Parameters)
                    if (parameter.ShortName == metadataParameter.Name)
                        return parameter;
            }

            return null;
        }

        private abstract class BaseWrapper<TTarget>
            where TTarget : class
        {
            private readonly MetadataReflector reflector;
            private readonly TTarget target;

            public BaseWrapper(MetadataReflector reflector, TTarget target)
            {
                if (reflector == null)
                    throw new ArgumentNullException("reflector");
                if (target == null)
                    throw new ArgumentNullException("target");

                this.reflector = reflector;
                this.target = target;
            }

            public TTarget Target
            {
                get { return target; }
            }

            public MetadataReflector Reflector
            {
                get { return reflector; }
            }

            public override int GetHashCode()
            {
                return target.GetHashCode();
            }

            public override bool Equals(object obj)
            {
                BaseWrapper<TTarget> other = obj as BaseWrapper<TTarget>;
                return other != null && target.Equals(other.target);
            }
        }

        private abstract class CodeElementWrapper<TTarget> : BaseWrapper<TTarget>, ICodeElementInfo,
            IDeclaredElementAccessor, IProjectAccessor
            where TTarget : class
        {
            public CodeElementWrapper(MetadataReflector reflector, TTarget target)
                : base(reflector, target)
            {
            }

            public IProject Project
            {
                get { return Reflector.ContextProject; }
            }

            public IDeclaredElement DeclaredElement
            {
                get
                {
                    using (ReadLockCookie.Create())
                        return GetDeclaredElementWithLock(); 
                }
            }

            protected virtual IDeclaredElement GetDeclaredElementWithLock()
            {
                return null;
            }

            public abstract string Name { get; }

            public abstract CodeElementKind Kind { get; }

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

            public bool Equals(ICodeElementInfo other)
            {
                return Equals((object)other);
            }
        }

        private sealed class AssemblyWrapper : CodeElementWrapper<IMetadataAssembly>, IAssemblyInfo
        {
            public AssemblyWrapper(MetadataReflector reflector, IMetadataAssembly target)
                : base(reflector, target)
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

            public override CodeElementKind Kind
            {
                get { return CodeElementKind.Assembly; }
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
                List<ITypeInfo> types = new List<ITypeInfo>();

                foreach (IMetadataTypeInfo type in Target.GetTypes())
                {
                    if (type.IsPublic || type.IsNestedPublic)
                        types.Add(Reflector.WrapOpenType(type));
                }

                return types;
            }

            public IList<ITypeInfo> GetTypes()
            {
                IMetadataTypeInfo[] types = Target.GetTypes();
                return Array.ConvertAll<IMetadataTypeInfo, ITypeInfo>(types, Reflector.WrapOpenType);
            }

            public ITypeInfo GetType(string typeName)
            {
                return Reflector.Wrap(Target.GetTypeFromQualifiedName(typeName, false));
            }

            public override IEnumerable<IAttributeInfo> GetAttributeInfos(bool inherit)
            {
                return Reflector.GetAttributeInfosForEntity(Target, inherit);
            }

            public Assembly Resolve()
            {
                return Assembly.LoadFrom(Path);
            }

            public bool Equals(IAssemblyInfo other)
            {
                return Equals((object)other);
            }
        }

        private abstract class MemberWrapper<TTarget> : CodeElementWrapper<TTarget>, IMemberInfo
            where TTarget : class, IMetadataEntity
        {
            public MemberWrapper(MetadataReflector reflector, TTarget target)
                : base(reflector, target)
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
                return Reflector.GetAttributeInfosForEntity(Target, inherit);
            }

            MemberInfo IMemberInfo.Resolve()
            {
                return ResolveMemberInfo();
            }

            public abstract MemberInfo ResolveMemberInfo();

            public bool Equals(IMemberInfo other)
            {
                return Equals((object)other);
            }
        }

        private abstract class TypeWrapper<TTarget> : CodeElementWrapper<TTarget>, ITypeInfo
            where TTarget : class, IMetadataType
        {
            public TypeWrapper(MetadataReflector reflector, TTarget target)
                : base(reflector, target)
            {
            }

            public string AssemblyQualifiedName
            {
                get { return FullName + @", " + Assembly.FullName; }
            }

            public override CodeElementKind Kind
            {
                get { return CodeElementKind.Type; }
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
            public abstract TypeAttributes TypeAttributes { get; }
            public abstract IList<ITypeInfo> GetInterfaces();
            public abstract IList<IConstructorInfo> GetConstructors(BindingFlags bindingFlags);
            public abstract IMethodInfo GetMethod(string methodName, BindingFlags bindingFlags);
            public abstract IList<IMethodInfo> GetMethods(BindingFlags bindingFlags);
            public abstract IList<IPropertyInfo> GetProperties(BindingFlags bindingFlags);
            public abstract IList<IFieldInfo> GetFields(BindingFlags bindingFlags);
            public abstract IList<IEventInfo> GetEvents(BindingFlags bindingFlags);
            public abstract IList<IGenericParameterInfo> GetGenericParameters();
            public abstract bool IsAssignableFrom(ITypeInfo type);
            public abstract Type Resolve();

            MemberInfo IMemberInfo.Resolve()
            {
                return Resolve();
            }

            public override int GetHashCode()
            {
                // FIXME: Not very precise.
                return Target.PresentableName.GetHashCode();
            }

            public override bool Equals(object obj)
            {
                // FIXME: Not very precise.
                TypeWrapper<TTarget> other = obj as TypeWrapper<TTarget>;
                return other != null && Target.PresentableName == other.Target.PresentableName;
            }

            public bool Equals(IMemberInfo other)
            {
                return Equals((object)other);
            }

            public bool Equals(ITypeInfo other)
            {
                return Equals((object)other);
            }
        }

        private sealed class ClassTypeWrapper : TypeWrapper<IMetadataClassType>
        {
            public ClassTypeWrapper(MetadataReflector reflector, IMetadataClassType target)
                : base(reflector, target)
            {
            }

            protected override IDeclaredElement GetDeclaredElementWithLock()
            {
                return Reflector.GetDeclaredElementWithLock(Target.Type);
            }

            public override string Name
            {
                get { return new CLRTypeName(TargetInfo.FullyQualifiedName).ShortName; }
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
                get { return Reflector.WrapOpenType(TargetInfo.DeclaringType); }
            }

            public override IAssemblyInfo Assembly
            {
                get
                {
                    // TODO: Fix this hack!  We really need the IAssemblyResolver to do this correctly.
                    FieldInfo myAssemblyField = TargetInfo.GetType().GetField("myAssembly", BindingFlags.Instance | BindingFlags.NonPublic);
                    IMetadataAssembly assembly = (IMetadataAssembly)myAssemblyField.GetValue(TargetInfo);

                    return Reflector.Wrap(assembly);
                }
            }

            public override INamespaceInfo Namespace
            {
                get { return Reflector.WrapNamespace(NamespaceName); }
            }

            public override ITypeInfo BaseType
            {
                get { return Reflector.Wrap(TargetInfo.Base); }
            }

            public override string FullName
            {
                get { return Target.Type.FullyQualifiedName; }
            }

            public override TypeAttributes TypeAttributes
            {
                get
                {
                    IMetadataTypeInfo info = TargetInfo;
                    TypeAttributes flags = 0;
                    ReflectorUtils.AddFlagIfTrue(ref flags, TypeAttributes.Abstract, info.IsAbstract);
                    ReflectorUtils.AddFlagIfTrue(ref flags, TypeAttributes.Class, info.IsClass);
                    ReflectorUtils.AddFlagIfTrue(ref flags, TypeAttributes.Interface, info.IsInterface);
                    ReflectorUtils.AddFlagIfTrue(ref flags, TypeAttributes.NestedAssembly, info.IsNestedAssembly);
                    ReflectorUtils.AddFlagIfTrue(ref flags, TypeAttributes.NestedFamily, info.IsNestedFamily);
                    ReflectorUtils.AddFlagIfTrue(ref flags, TypeAttributes.NestedFamANDAssem, info.IsNestedFamilyAndAssembly);
                    ReflectorUtils.AddFlagIfTrue(ref flags, TypeAttributes.NestedFamORAssem, info.IsNestedFamilyOrAssembly);
                    ReflectorUtils.AddFlagIfTrue(ref flags, TypeAttributes.NestedPrivate, info.IsNestedPrivate);
                    ReflectorUtils.AddFlagIfTrue(ref flags, TypeAttributes.NestedPublic, info.IsNestedPublic);
                    ReflectorUtils.AddFlagIfTrue(ref flags, TypeAttributes.Public, info.IsPublic);
                    ReflectorUtils.AddFlagIfTrue(ref flags, TypeAttributes.NotPublic, info.IsNotPublic);
                    ReflectorUtils.AddFlagIfTrue(ref flags, TypeAttributes.Sealed, info.IsSealed);
                    ReflectorUtils.AddFlagIfTrue(ref flags, TypeAttributes.Serializable, info.IsSerializable);
                    ReflectorUtils.AddFlagIfTrue(ref flags, TypeAttributes.SpecialName, info.IsSpecialName);
                    return flags;
                }
            }

            public override IList<ITypeInfo> GetInterfaces()
            {
                IMetadataClassType[] interfaces = TargetInfo.Interfaces;
                return Array.ConvertAll<IMetadataClassType, ITypeInfo>(interfaces, Reflector.Wrap);
            }

            public override IList<IConstructorInfo> GetConstructors(BindingFlags bindingFlags)
            {
                List<IConstructorInfo> result = new List<IConstructorInfo>();

                foreach (IMetadataMethod method in TargetInfo.GetMethods())
                {
                    if (IsConstructor(method) && CheckBindingFlags(bindingFlags, method.IsPublic, method.IsStatic))
                        result.Add(Reflector.WrapConstructor(method));
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

                return (IMethodInfo) Reflector.Wrap(match);
            }

            public override IList<IMethodInfo> GetMethods(BindingFlags bindingFlags)
            {
                List<IMethodInfo> result = new List<IMethodInfo>();

                foreach (IMetadataMethod method in TargetInfo.GetMethods())
                {
                    if (!IsConstructor(method) && CheckBindingFlags(bindingFlags, method.IsPublic, method.IsStatic))
                        result.Add(Reflector.WrapMethod(method));
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
                        result.Add(Reflector.Wrap(property));
                }

                return result;
            }

            public override IList<IFieldInfo> GetFields(BindingFlags bindingFlags)
            {
                List<IFieldInfo> result = new List<IFieldInfo>();

                foreach (IMetadataField field in TargetInfo.GetFields())
                {
                    if (CheckBindingFlags(bindingFlags, field.IsPublic, field.IsStatic))
                        result.Add(Reflector.Wrap(field));
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
                        result.Add(Reflector.Wrap(@event));
                }

                return result;
            }

            public override bool IsAssignableFrom(ITypeInfo type)
            {
                // TODO
                throw new NotImplementedException();
            }

            public override IEnumerable<IAttributeInfo> GetAttributeInfos(bool inherit)
            {
                return Reflector.GetAttributeInfosForEntity(TargetInfo, inherit);
            }

            public override IList<IGenericParameterInfo> GetGenericParameters()
            {
                IMetadataGenericArgument[] parameters = TargetInfo.GenericParameters;
                return Array.ConvertAll<IMetadataGenericArgument, IGenericParameterInfo>(parameters, Reflector.Wrap);
            }

            public override Type Resolve()
            {
                // TODO
                throw new NotImplementedException();
            }

            private string NamespaceName
            {
                get { return new CLRTypeName(TargetInfo.FullyQualifiedName).NamespaceName; }
            }

            private IMetadataTypeInfo TargetInfo
            {
                get { return Target.Type; }
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
            public ConstructedTypeWrapper(MetadataReflector reflector, TTarget target)
                : base(reflector, target)
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

            public override TypeAttributes TypeAttributes
            {
                get { return EffectiveClassType.TypeAttributes; }
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

            public override IList<IGenericParameterInfo> GetGenericParameters()
            {
                return EmptyArray<IGenericParameterInfo>.Instance;
            }

            public override bool IsAssignableFrom(ITypeInfo type)
            {
                // TODO
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
            public ArrayTypeWrapper(MetadataReflector reflector, IMetadataArrayType target)
                : base(reflector, target)
            {
            }

            public override string Name
            {
                get { return ElementType.Name + "[]"; }
            }

            public override ITypeInfo ElementType
            {
                get { return Reflector.Wrap(Target.ElementType); }
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
                get { return Gallio.Model.Reflection.Reflector.Wrap(typeof(Array)); }
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

        private sealed class GenericParameterWrapper : ConstructedTypeWrapper<IMetadataGenericArgumentReferenceType>, IGenericParameterInfo
        {
            public GenericParameterWrapper(MetadataReflector reflector, IMetadataGenericArgumentReferenceType target)
                : base(reflector, target)
            {
            }

            public override string Name
            {
                get { return Target.Argument.Name; }
            }

            public override CodeElementKind Kind
            {
                get { return CodeElementKind.GenericParameter; }
            }

            public override bool IsGenericParameter
            {
                get { return true; }
            }

            public override ITypeInfo EffectiveClassType
            {
                get
                {
                    // TODO: In actuality we can treat this case as producing a type whose members
                    //       are the union of the classes or interfaces in the generic type parameter constraint.
                    throw new NotImplementedException("Cannot perform this operation on a generic type parameter.");
                }
            }

            public override Type Resolve()
            {
                // TODO
                throw new NotImplementedException();
            }

            public GenericParameterAttributes GenericParameterAttributes
            {
                get
                {
                    // Note: The values are exactly the same, it's just the type that's different.
                    return (GenericParameterAttributes)Target.Argument.Attributes;
                }
            }

            public ITypeInfo ValueType
            {
                get { return Gallio.Model.Reflection.Reflector.Wrap(typeof(Type)); }
            }

            public int Position
            {
                get { return (int) Target.Argument.Index; }
            }

            public bool Equals(ISlotInfo other)
            {
                return Equals((object)other);
            }
        }

        private sealed class PointerTypeWrapper : ConstructedTypeWrapper<IMetadataPointerType>
        {
            public PointerTypeWrapper(MetadataReflector reflector, IMetadataPointerType target)
                : base(reflector, target)
            {
            }

            public override string Name
            {
                get { return ElementType.Name + "*"; }
            }

            public override ITypeInfo ElementType
            {
                get { return Reflector.Wrap(Target.Type); }
            }

            public override bool IsPointer
            {
                get { return true; }
            }

            public override ITypeInfo EffectiveClassType
            {
                get { return Gallio.Model.Reflection.Reflector.Wrap(typeof(Pointer)); }
            }

            public override Type Resolve()
            {
                return ElementType.Resolve().MakePointerType();
            }
        }

        private sealed class ReferenceTypeWrapper : ConstructedTypeWrapper<IMetadataReferenceType>
        {
            public ReferenceTypeWrapper(MetadataReflector reflector, IMetadataReferenceType target)
                : base(reflector, target)
            {
            }

            public override string Name
            {
                get { return ElementType.Name + "&"; }
            }

            public override ITypeInfo ElementType
            {
                get { return Reflector.Wrap(Target.Type); }
            }

            public override bool IsByRef
            {
                get{ return true; }
            }

            public override ITypeInfo EffectiveClassType
            {
                get { return Gallio.Model.Reflection.Reflector.Wrap(typeof(TypedReference)); }
            }

            public override Type Resolve()
            {
                // TODO
                throw new NotImplementedException();
            }
        }

        private abstract class FunctionWrapper : MemberWrapper<IMetadataMethod>, IFunctionInfo
        {
            public FunctionWrapper(MetadataReflector reflector, IMetadataMethod target)
                : base(reflector, target)
            {
            }

            protected override IDeclaredElement GetDeclaredElementWithLock()
            {
                return Reflector.GetDeclaredElementWithLock(Target);
            }

            public override string Name
            {
                get { return Target.Name; }
            }

            public override ITypeInfo DeclaringType
            {
                get { return Reflector.WrapOpenType(Target.DeclaringType); }
            }

            public MethodAttributes MethodAttributes
            {
                get
                {
                    MethodAttributes flags = 0;
                    ReflectorUtils.AddFlagIfTrue(ref flags, MethodAttributes.Abstract, Target.IsAbstract);
                    ReflectorUtils.AddFlagIfTrue(ref flags, MethodAttributes.Assembly, Target.IsAssembly);
                    ReflectorUtils.AddFlagIfTrue(ref flags, MethodAttributes.Family, Target.IsFamily);
                    ReflectorUtils.AddFlagIfTrue(ref flags, MethodAttributes.FamANDAssem, Target.IsFamilyAndAssembly);
                    ReflectorUtils.AddFlagIfTrue(ref flags, MethodAttributes.FamORAssem, Target.IsFamilyOrAssembly);
                    ReflectorUtils.AddFlagIfTrue(ref flags, MethodAttributes.Final, Target.IsFinal);
                    ReflectorUtils.AddFlagIfTrue(ref flags, MethodAttributes.HideBySig, Target.IsHideBySig);
                    ReflectorUtils.AddFlagIfTrue(ref flags, MethodAttributes.NewSlot, Target.IsNewSlot);
                    ReflectorUtils.AddFlagIfTrue(ref flags, MethodAttributes.Private, Target.IsPrivate);
                    ReflectorUtils.AddFlagIfTrue(ref flags, MethodAttributes.Public, Target.IsPublic);
                    ReflectorUtils.AddFlagIfTrue(ref flags, MethodAttributes.SpecialName, Target.IsSpecialName);
                    ReflectorUtils.AddFlagIfTrue(ref flags, MethodAttributes.Static, Target.IsStatic);
                    ReflectorUtils.AddFlagIfTrue(ref flags, MethodAttributes.Virtual, Target.IsVirtual);
                    return flags;
                }
            }

            public IList<IParameterInfo> GetParameters()
            {
                IMetadataParameter[] parameters = Target.Parameters;
                return Array.ConvertAll<IMetadataParameter, IParameterInfo>(parameters, Reflector.Wrap);
            }

            public IList<IGenericParameterInfo> GetGenericParameters()
            {
                IMetadataGenericArgument[] parameters = Target.GenericArguments;
                return Array.ConvertAll<IMetadataGenericArgument, IGenericParameterInfo>(parameters, Reflector.Wrap);
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

            public bool Equals(IFunctionInfo other)
            {
                return Equals((object)other);
            }
        }

        private sealed class ConstructorWrapper : FunctionWrapper, IConstructorInfo
        {
            public ConstructorWrapper(MetadataReflector reflector, IMetadataMethod target)
                : base(reflector, target)
            {
                if (!IsConstructor(target))
                    throw new ArgumentException("target");
            }

            public override CodeElementKind Kind
            {
                get { return CodeElementKind.Constructor; }
            }

            public override MethodBase ResolveMethodBase()
            {
                return Resolve();
            }

            public ConstructorInfo Resolve()
            {
                // TODO
                throw new NotImplementedException();
            }

            public bool Equals(IConstructorInfo other)
            {
                return Equals((object)other);
            }
        }

        private sealed class MethodWrapper : FunctionWrapper, IMethodInfo
        {
            public MethodWrapper(MetadataReflector reflector, IMetadataMethod target)
                : base(reflector, target)
            {
                if (IsConstructor(target))
                    throw new ArgumentException("target");
            }

            public ITypeInfo ReturnType
            {
                get { return Reflector.Wrap(Target.ReturnValue.Type); }
            }

            public override CodeElementKind Kind
            {
                get { return CodeElementKind.Method; }
            }

            public override MethodBase ResolveMethodBase()
            {
                return Resolve();
            }

            public MethodInfo Resolve()
            {
                // TODO
                throw new NotImplementedException();
            }
        }

        private sealed class PropertyWrapper : MemberWrapper<IMetadataProperty>, IPropertyInfo
        {
            public PropertyWrapper(MetadataReflector reflector, IMetadataProperty target)
                : base(reflector, target)
            {
            }

            protected override IDeclaredElement GetDeclaredElementWithLock()
            {
                return Reflector.GetDeclaredElementWithLock(Target);
            }

            public override string Name
            {
                get { return Target.Name; }
            }

            public override ITypeInfo DeclaringType
            {
                get { return Reflector.WrapOpenType(Target.DeclaringType); }
            }

            public ITypeInfo ValueType
            {
                get { return Reflector.Wrap(Target.Type); }
            }

            public int Position
            {
                get { return 0; }
            }

            public PropertyAttributes PropertyAttributes
            {
                get
                {
                    // Note: There don't seem to be any usable property attributes.
                    return 0;
                }
            }

            public override CodeElementKind Kind
            {
                get { return CodeElementKind.Property; }
            }

            public IMethodInfo GetGetMethod()
            {
                return Reflector.WrapMethod(Target.Getter);
            }

            public IMethodInfo GetSetMethod()
            {
                return Reflector.WrapMethod(Target.Setter);
            }

            public override MemberInfo ResolveMemberInfo()
            {
                return Resolve();
            }

            public PropertyInfo Resolve()
            {
                // TODO
                throw new NotImplementedException();
            }

            public bool Equals(ISlotInfo other)
            {
                return Equals((object)other);
            }

            public bool Equals(IPropertyInfo other)
            {
                return Equals((object)other);
            }
        }

        private sealed class FieldWrapper : MemberWrapper<IMetadataField>, IFieldInfo
        {
            public FieldWrapper(MetadataReflector reflector, IMetadataField target)
                : base(reflector, target)
            {
            }

            protected override IDeclaredElement GetDeclaredElementWithLock()
            {
                return Reflector.GetDeclaredElementWithLock(Target);
            }

            public ITypeInfo ValueType
            {
                get { return Reflector.Wrap(Target.Type); }
            }

            public int Position
            {
                get { return 0; }
            }

            public override ITypeInfo DeclaringType
            {
                get { return Reflector.WrapOpenType(Target.DeclaringType); }
            }

            public override string Name
            {
                get { return Target.Name; }
            }

            public override MemberInfo ResolveMemberInfo()
            {
                return Resolve();
            }

            public FieldAttributes FieldAttributes
            {
                get
                {
                    FieldAttributes flags = 0;
                    ReflectorUtils.AddFlagIfTrue(ref flags, FieldAttributes.Assembly, Target.IsAssembly);
                    ReflectorUtils.AddFlagIfTrue(ref flags, FieldAttributes.Family, Target.IsFamily);
                    ReflectorUtils.AddFlagIfTrue(ref flags, FieldAttributes.FamANDAssem, Target.IsFamilyAndAssembly);
                    ReflectorUtils.AddFlagIfTrue(ref flags, FieldAttributes.FamORAssem, Target.IsFamilyOrAssembly);
                    ReflectorUtils.AddFlagIfTrue(ref flags, FieldAttributes.Private, Target.IsPrivate);
                    ReflectorUtils.AddFlagIfTrue(ref flags, FieldAttributes.Public, Target.IsPublic);
                    ReflectorUtils.AddFlagIfTrue(ref flags, FieldAttributes.SpecialName, Target.IsSpecialName);
                    ReflectorUtils.AddFlagIfTrue(ref flags, FieldAttributes.Static, Target.IsStatic);
                    ReflectorUtils.AddFlagIfTrue(ref flags, FieldAttributes.Literal, Target.IsLiteral);
                    ReflectorUtils.AddFlagIfTrue(ref flags, FieldAttributes.InitOnly, Target.IsInitOnly);
                    return flags;
                }
            }

            public override CodeElementKind Kind
            {
                get { return CodeElementKind.Field; }
            }

            public FieldInfo Resolve()
            {
                // TODO
                throw new NotImplementedException();
            }

            public bool Equals(ISlotInfo other)
            {
                return Equals((object)other);
            }

            public bool Equals(IFieldInfo other)
            {
                return Equals((object)other);
            }
        }

        private sealed class EventWrapper : MemberWrapper<IMetadataEvent>, IEventInfo
        {
            public EventWrapper(MetadataReflector reflector, IMetadataEvent target)
                : base(reflector, target)
            {
            }

            protected override IDeclaredElement GetDeclaredElementWithLock()
            {
                return Reflector.GetDeclaredElementWithLock(Target);
            }

            public override ITypeInfo DeclaringType
            {
                get { return Reflector.WrapOpenType(Target.DeclaringType); }
            }

            public override string Name
            {
                get { return Target.Name; }
            }

            public override CodeElementKind Kind
            {
                get { return CodeElementKind.Event; }
            }

            public override MemberInfo ResolveMemberInfo()
            {
                return Resolve();
            }

            public EventInfo Resolve()
            {
                // TODO
                throw new NotImplementedException();
            }

            public bool Equals(IEventInfo other)
            {
                return Equals((object)other);
            }
        }

        private sealed class ParameterWrapper : CodeElementWrapper<IMetadataParameter>, IParameterInfo
        {
            public ParameterWrapper(MetadataReflector reflector, IMetadataParameter target)
                : base(reflector, target)
            {
            }

            protected override IDeclaredElement GetDeclaredElementWithLock()
            {
                return Reflector.GetDeclaredElementWithLock(Target);
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
                get { return Reflector.Wrap(Target.Type); }
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
                get { return Reflector.Wrap(Target.DeclaringMethod); }
            }

            public ParameterAttributes ParameterAttributes
            {
                get
                {
                    ParameterAttributes flags = 0;
                    ReflectorUtils.AddFlagIfTrue(ref flags, ParameterAttributes.In, Target.IsIn);
                    ReflectorUtils.AddFlagIfTrue(ref flags, ParameterAttributes.Out, Target.IsOut);
                    ReflectorUtils.AddFlagIfTrue(ref flags, ParameterAttributes.Optional, Target.IsOptional);
                    return flags;
                }
            }

            public override CodeElementKind Kind
            {
                get { return CodeElementKind.Parameter; }
            }

            public override IEnumerable<IAttributeInfo> GetAttributeInfos(bool inherit)
            {
                return Reflector.GetAttributeInfosForEntity(Target, inherit);
            }

            public ParameterInfo Resolve()
            {
                // TODO
                throw new NotImplementedException();
            }

            public bool Equals(ISlotInfo other)
            {
                return Equals((object)other);
            }

            public bool Equals(IParameterInfo other)
            {
                return Equals((object)other);
            }
        }

        private sealed class AttributeWrapper : BaseWrapper<IMetadataCustomAttribute>, IAttributeInfo
        {
            public AttributeWrapper(MetadataReflector reflector, IMetadataCustomAttribute target)
                : base(reflector, target)
            {
            }

            public ITypeInfo Type
            {
                get { return Reflector.WrapOpenType(Target.UsedConstructor.DeclaringType); }
            }

            public IConstructorInfo Constructor
            {
                get { return Reflector.WrapConstructor(Target.UsedConstructor); }
            }

            public object[] ArgumentValues
            {
                get { return Target.ConstructorArguments; }
            }

            public object GetFieldValue(string name)
            {
                foreach (IMetadataCustomAttributeFieldInitialization initialization in Target.InitializedFields)
                    if (initialization.Field.Name == name)
                        return initialization.Value;

                throw new ArgumentException(String.Format("The attribute does not have an initialized field named '{0}'.", name));
            }

            public object GetPropertyValue(string name)
            {
                foreach (IMetadataCustomAttributePropertyInitialization initialization in Target.InitializedProperties)
                    if (initialization.Property.Name == name)
                        return initialization.Value;

                throw new ArgumentException(String.Format("The attribute does not have an initialized property named '{0}'.", name));
            }

            public IDictionary<IFieldInfo, object> FieldValues
            {
                get
                {
                    IMetadataCustomAttributeFieldInitialization[] initializations = Target.InitializedFields;
                    Dictionary<IFieldInfo, object> values = new Dictionary<IFieldInfo, object>(initializations.Length);

                    foreach (IMetadataCustomAttributeFieldInitialization initialization in initializations)
                        values.Add(Reflector.Wrap(initialization.Field), initialization.Value);

                    return values;
                }
            }

            public IDictionary<IPropertyInfo, object> PropertyValues
            {
                get
                {
                    IMetadataCustomAttributePropertyInitialization[] initializations = Target.InitializedProperties;
                    Dictionary<IPropertyInfo, object> values = new Dictionary<IPropertyInfo, object>(initializations.Length);

                    foreach (IMetadataCustomAttributePropertyInitialization initialization in initializations)
                        values.Add(Reflector.Wrap(initialization.Property), initialization.Value);

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
