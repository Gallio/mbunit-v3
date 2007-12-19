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
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Caches;
using JetBrains.ReSharper.Psi.Util;
using JetBrains.ProjectModel.Build;

namespace Gallio.ReSharperRunner.Reflection
{
    /// <summary>
    /// Wraps ReSharper code model types using the reflection adapter interfaces.
    /// </summary>
    /// <todo author="jeff">
    /// Support inherited attribute lookup.
    /// </todo>
    internal class PsiReflector : BaseReSharperReflector, IReflectionPolicy
    {
        private readonly PsiManager manager;

        /// <summary>
        /// Creates a reflector with the specified PSI manager.
        /// </summary>
        /// <param name="assemblyResolver">The assembly resolver</param>
        /// <param name="manager">The PSI manager</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="manager"/> is null</exception>
        public PsiReflector(IAssemblyResolver assemblyResolver, PsiManager manager)
            : base(assemblyResolver)
        {
            if (manager == null)
                throw new ArgumentNullException("manager");

            this.manager = manager;
        }

        /// <summary>
        /// Obtains a reflection wrapper for a module.
        /// </summary>
        /// <param name="target">The module, or null if none</param>
        /// <returns>The reflection wrapper, or null if none</returns>
        public IAssemblyInfo Wrap(IModule target)
        {
            if (target == null)
                return null;

            IAssembly assembly = target as IAssembly;
            if (assembly != null)
                return Wrap(assembly);

            IProject project = target as IProject;
            if (project != null)
                return Wrap(project);

            throw new NotSupportedException("Unsupported module type.");
        }

        /// <summary>
        /// Obtains a reflection wrapper for a project.
        /// </summary>
        /// <param name="target">The project, or null if none</param>
        /// <returns>The reflection wrapper, or null if none</returns>
        public IAssemblyInfo Wrap(IProject target)
        {
            return target != null ? new ProjectWrapper(this, target) : null;
        }

        /// <summary>
        /// Obtains a reflection wrapper for an assembly.
        /// </summary>
        /// <param name="target">The assembly, or null if none</param>
        /// <returns>The reflection wrapper, or null if none</returns>
        public IAssemblyInfo Wrap(IAssembly target)
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
        /// <param name="throwIfUnsupported">If true, throws <exception="NotSupportedException" /> if
        /// the target is not of a recognized type, otherwise just returns null</param>
        /// <returns>The reflection wrapper, or null if none</returns>
        public ITypeInfo Wrap(IType target, bool throwIfUnsupported)
        {
            if (target == null)
                return null;

            IDeclaredType declaredType = target as IDeclaredType;
            if (declaredType != null)
            {
                if (declaredType.GetTypeElement() is ITypeParameter)
                    return new GenericParameterWrapper(this, declaredType);
                return new DeclaredTypeWrapper(this, declaredType);
            }

            IArrayType arrayType = target as IArrayType;
            if (arrayType != null)
                return new ArrayTypeWrapper(this, arrayType);

            IPointerType pointerType = target as IPointerType;
            if (pointerType != null)
                return new PointerTypeWrapper(this, pointerType);

            if (throwIfUnsupported)
                throw new NotSupportedException("Unsupported type.");
            return null;
        }

        /// <summary>
        /// Obtains a reflection wrapper for a type parameter.
        /// </summary>
        /// <param name="target">The type parameter, or null if none</param>
        /// <returns>The reflection wrapper, or null if none</returns>
        public IGenericParameterInfo Wrap(ITypeParameter target)
        {
            return target != null ? new GenericParameterWrapper(this, TypeFactory.CreateType(target)) : null;
        }

        /// <summary>
        /// Obtains a reflection wrapper for a declared element.
        /// </summary>
        /// <param name="target">The element, or null if none</param>
        /// be mapped, otherwise just returns null</param>
        /// <param name="throwIfUnsupported">If true, throws <exception="NotSupportedException" /> if
        /// the target is not of a recognized type, otherwise just returns null</param>
        /// <returns>The reflection wrapper, or null if none</returns>
        public ICodeElementInfo Wrap(IDeclaredElement target, bool throwIfUnsupported)
        {
            if (target == null)
                return null;

            ITypeElement typeElement = target as ITypeElement;
            if (typeElement != null)
                return Wrap(typeElement);

            IFunction function = target as IFunction;
            if (function != null)
                return Wrap(function);

            IProperty property = target as IProperty;
            if (property != null)
                return Wrap(property);

            IField field = target as IField;
            if (field != null)
                return Wrap(field);

            IEvent @event = target as IEvent;
            if (@event != null)
                return Wrap(@event);

            IParameter parameter = target as IParameter;
            if (parameter != null)
                return Wrap(parameter);

            INamespace @namespace = target as INamespace;
            if (@namespace != null)
                return WrapNamespace(@namespace.QualifiedName);

            if (throwIfUnsupported)
                throw new NotSupportedException("Unsupported declared element type.");
            return null;
        }

        /// <summary>
        /// Obtains a reflection wrapper for a type.
        /// </summary>
        /// <param name="target">The type, or null if none</param>
        /// <returns>The reflection wrapper, or null if none</returns>
        public ITypeInfo Wrap(ITypeElement target)
        {
            return target != null ? Wrap(TypeFactory.CreateType(target), true) : null;
        }

        /// <summary>
        /// Obtains a reflection wrapper for a function.
        /// </summary>
        /// <param name="target">The function, or null if none</param>
        /// <returns>The reflection wrapper, or null if none</returns>
        public IFunctionInfo Wrap(IFunction target)
        {
            if (target == null)
                return null;

            IConstructor constructor = target as IConstructor;
            return constructor != null ? (IFunctionInfo)new ConstructorWrapper(this, constructor) : new MethodWrapper(this, target);
        }

        /// <summary>
        /// Obtains a reflection wrapper for a method.
        /// </summary>
        /// <param name="target">The method, or null if none</param>
        /// <returns>The reflection wrapper, or null if none</returns>
        public IMethodInfo Wrap(IMethod target)
        {
            return target != null ? new MethodWrapper(this, target) : null;
        }

        /// <summary>
        /// Obtains a reflection wrapper for a method.
        /// </summary>
        /// <param name="target">The method, or null if none</param>
        /// <returns>The reflection wrapper, or null if none</returns>
        public IMethodInfo Wrap(IOperator target)
        {
            return target != null ? new MethodWrapper(this, target) : null;
        }

        /// <summary>
        /// Obtains a reflection wrapper for a constructor.
        /// </summary>
        /// <param name="target">The constructor, or null if none</param>
        /// <returns>The reflection wrapper, or null if none</returns>
        public IConstructorInfo Wrap(IConstructor target)
        {
            return target != null ? new ConstructorWrapper(this, target) : null;
        }

        /// <summary>
        /// Obtains a reflection wrapper for a property.
        /// </summary>
        /// <param name="target">The property, or null if none</param>
        /// <returns>The reflection wrapper, or null if none</returns>
        public IPropertyInfo Wrap(IProperty target)
        {
            return target != null ? new PropertyWrapper(this, target) : null;
        }

        /// <summary>
        /// Obtains a reflection wrapper for a field.
        /// </summary>
        /// <param name="target">The field, or null if none</param>
        /// <returns>The reflection wrapper, or null if none</returns>
        public IFieldInfo Wrap(IField target)
        {
            return target != null ? new FieldWrapper(this, target) : null;
        }

        /// <summary>
        /// Obtains a reflection wrapper for an event.
        /// </summary>
        /// <param name="target">The event, or null if none</param>
        /// <returns>The reflection wrapper, or null if none</returns>
        public IEventInfo Wrap(IEvent target)
        {
            return target != null ? new EventWrapper(this, target) : null;
        }

        /// <summary>
        /// Obtains a reflection wrapper for a parameter.
        /// </summary>
        /// <param name="target">The parameter, or null if none</param>
        /// <returns>The reflection wrapper, or null if none</returns>
        public IParameterInfo Wrap(IParameter target)
        {
            return target != null ? new ParameterWrapper(this, target) : null;
        }

        /// <summary>
        /// Obtains a reflection wrapper for an attribute.
        /// </summary>
        /// <param name="target">The attribute, or null if none</param>
        /// <returns>The reflection wrapper, or null if none</returns>
        public IAttributeInfo Wrap(IAttributeInstance target)
        {
            return target != null ? new AttributeWrapper(this, target) : null;
        }

        /// <inheritdoc />
        public override IAssemblyInfo LoadAssembly(AssemblyName assemblyName)
        {
            bool partialName = assemblyName.Name == assemblyName.FullName;

            foreach (IAssembly candidateAssembly in manager.Solution.GetAllAssemblies())
            {
                AssemblyName candidateAssemblyName = candidateAssembly.AssemblyName;

                if (partialName && assemblyName.Name == candidateAssemblyName.Name
                    || ! partialName && assemblyName.FullName == candidateAssemblyName.FullName)
                    return Wrap(candidateAssembly);
            }

            throw new ArgumentException(String.Format("Could not find assembly '{0}' in the ReSharper code cache.",
                assemblyName.FullName));
        }

        private IEnumerable<IAttributeInfo> GetAttributeInfosForModule(IModuleAttributes moduleAttributes)
        {
            foreach (IAttributeInstance attrib in moduleAttributes.AttributeInstances)
                yield return Wrap(attrib);
        }

        private IEnumerable<IAttributeInfo> GetAttributeInfosForElement(IAttributesOwner element, bool inherit)
        {
            // TODO: Support attribute inheritance.
            foreach (IAttributeInstance attrib in element.GetAttributeInstances(inherit))
                yield return Wrap(attrib);
        }


        private abstract class BaseWrapper<TTarget>
            where TTarget : class
        {
            private readonly PsiReflector reflector;
            private readonly TTarget target;

            public BaseWrapper(PsiReflector reflector, TTarget target)
            {
                if (reflector == null)
                    throw new ArgumentNullException("reflector");
                if (target == null)
                    throw new ArgumentNullException("target");

                this.reflector = reflector;
                this.target = target;
            }

            public PsiReflector Reflector
            {
                get { return reflector; }
            }

            public TTarget Target
            {
                get { return target; }
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
            public CodeElementWrapper(PsiReflector reflector, TTarget target)
                : base(reflector, target)
            {
            }

            public virtual IProject Project
            {
                get
                {
                    IDeclaredElement declaredElement = DeclaredElement;
                    return declaredElement != null ? declaredElement.Module as IProject : null;
                }
            }

            public virtual IDeclaredElement DeclaredElement
            {
                get { return null; }
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
                return Name;
            }

            public bool Equals(ICodeElementInfo other)
            {
                return Equals((object) other);
            }
        }

        private abstract class ModuleWrapper<TTarget> : CodeElementWrapper<TTarget>, IAssemblyInfo
            where TTarget : class, IModule
        {
            public ModuleWrapper(PsiReflector reflector, TTarget target)
                : base(reflector, target)
            {
            }

            public override string Name
            {
                get { return GetName().Name; }
            }

            public override CodeElementKind Kind
            {
                get { return CodeElementKind.Assembly; }
            }

            public override CodeReference CodeReference
            {
                get { return new CodeReference(FullName, null, null, null, null); }
            }

            public string FullName
            {
                get { return GetName().FullName; }
            }

            public Assembly Resolve()
            {
                return Reflector.ResolveAssembly(this);
            }

            public string Path
            {
                get { return GetAssemblyFile().Location.FullPath; }
            }

            public virtual AssemblyName GetName()
            {
                return GetAssemblyFile().AssemblyName;
            }

            public ITypeInfo GetType(string typeName)
            {
                return Reflector.Wrap(GetDeclarationsCache().GetTypeElementByCLRName(typeName));
            }

            public IList<ITypeInfo> GetExportedTypes()
            {
                return GetTypes(false);
            }

            public IList<ITypeInfo> GetTypes()
            {
                return GetTypes(true);
            }

            public override IEnumerable<IAttributeInfo> GetAttributeInfos(bool inherit)
            {
                return Reflector.GetAttributeInfosForModule(PsiManager.GetModuleAttributes(Target));
            }

            private IList<ITypeInfo> GetTypes(bool includeNonPublicTypes)
            {
                INamespace ns = PsiManager.GetNamespace("");
                IDeclarationsCache cache = GetDeclarationsCache();

                List<ITypeInfo> types = new List<ITypeInfo>();
                PopulateTypes(types, ns, cache, includeNonPublicTypes);

                return types;
            }

            private void PopulateTypes(IList<ITypeInfo> types, INamespace ns, IDeclarationsCache cache, bool includeNonPublicTypes)
            {
                foreach (IDeclaredElement element in ns.GetNestedElements(cache))
                {
                    ITypeElement type = element as ITypeElement;
                    if (type != null)
                    {
                        PopulateTypes(types, type, includeNonPublicTypes);
                    }
                    else
                    {
                        INamespace nestedNamespace = element as INamespace;
                        if (nestedNamespace != null)
                            PopulateTypes(types, nestedNamespace, cache, includeNonPublicTypes);
                    }
                }
            }

            private void PopulateTypes(IList<ITypeInfo> types, ITypeElement type, bool includeNonPublicTypes)
            {
                IModifiersOwner modifiers = type as IModifiersOwner;
                if (modifiers != null && (includeNonPublicTypes || modifiers.GetAccessRights() == AccessRights.PUBLIC))
                {
                    types.Add(Reflector.Wrap(type));

                    foreach (ITypeElement nestedType in type.NestedTypes)
                        PopulateTypes(types, nestedType, includeNonPublicTypes);
                }
            }

            public abstract IList<AssemblyName> GetReferencedAssemblies();

            protected abstract IAssemblyFile GetAssemblyFile();

            protected abstract IDeclarationsCache GetDeclarationsCache();

            protected PsiManager PsiManager
            {
                get { return PsiManager.GetInstance(Target.GetSolution()); }
            }

            public bool Equals(IAssemblyInfo other)
            {
                return Equals((object)other);
            }
        }

        private sealed class AssemblyWrapper : ModuleWrapper<IAssembly>
        {
            public AssemblyWrapper(PsiReflector reflector, IAssembly target)
                : base(reflector, target)
            {
            }

            public override AssemblyName GetName()
            {
                return Target.AssemblyName;
            }

            public override IList<AssemblyName> GetReferencedAssemblies()
            {
                return Resolve().GetReferencedAssemblies();
            }

            protected override IAssemblyFile GetAssemblyFile()
            {
                IAssemblyFile[] files = Target.GetFiles();
                if (files.Length < 1)
                    return null;
                return files[0];
            }

            protected override IDeclarationsCache GetDeclarationsCache()
            {
                return PsiManager.GetDeclarationsCache(DeclarationsCacheScope.LibraryScope(Target, false), true);
            }
        }

        private sealed class ProjectWrapper : ModuleWrapper<IProject>
        {
            public ProjectWrapper(PsiReflector reflector, IProject target)
                : base(reflector, target)
            {
            }

            public override IProject Project
            {
                get { return Target; }
            }

            public override IList<AssemblyName> GetReferencedAssemblies()
            {
                ICollection<IModuleReference> moduleRefs = Target.GetModuleReferences();
                return GenericUtils.ConvertAllToArray<IModuleReference, AssemblyName>(moduleRefs, delegate(IModuleReference moduleRef)
                {
                    return Reflector.Wrap(moduleRef.ResolveReferencedModule()).GetName();
                });
            }

            protected override IAssemblyFile GetAssemblyFile()
            {
                return BuildSettingsManager.GetInstance(Target).GetOutputAssemblyFile();
            }

            protected override IDeclarationsCache GetDeclarationsCache()
            {
                return PsiManager.GetDeclarationsCache(DeclarationsCacheScope.ProjectScope(Target, false), true);
            }
        }

        private abstract class MemberWrapper<TTarget> : CodeElementWrapper<TTarget>, IMemberInfo
            where TTarget : class, ITypeMember
        {
            public MemberWrapper(PsiReflector reflector, TTarget target)
                : base(reflector, target)
            {
            }

            public override IDeclaredElement DeclaredElement
            {
                get { return Target; }
            }

            public override string Name
            {
                get { return Target.ShortName; }
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

            public ITypeInfo DeclaringType
            {
                get { return Reflector.Wrap(Target.GetContainingType()); }
            }

            public override IEnumerable<IAttributeInfo> GetAttributeInfos(bool inherit)
            {
                return Reflector.GetAttributeInfosForElement(Target, inherit);
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

            public override string ToString()
            {
                return CompoundName;
            }
        }

        private interface ITypeWrapper
        {
            IType Target { get; }
        }

        private abstract class TypeWrapper<TTarget> : CodeElementWrapper<TTarget>, ITypeInfo, ITypeWrapper
            where TTarget : class, IType
        {
            public TypeWrapper(PsiReflector reflector, TTarget target)
                : base(reflector, target)
            {
            }

            public override CodeElementKind Kind
            {
                get { return CodeElementKind.Type; }
            }

            IType ITypeWrapper.Target
            {
                get { return Target; }
            }

            public string AssemblyQualifiedName
            {
                get { return FullName + ", " + Assembly.FullName; }
            }

            public override CodeReference CodeReference
            {
                get { return new CodeReference(Assembly.FullName, Namespace.Name, FullName, null, null); }
            }

            public bool IsAssignableFrom(ITypeInfo type)
            {
                ITypeWrapper typeWrapper = type as ITypeWrapper;
                return typeWrapper != null && typeWrapper.Target.IsImplicitlyConvertibleTo(Target, PsiLanguageType.UNKNOWN);
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

            public Type Resolve()
            {
                return ResolveType(this);
            }

            MemberInfo IMemberInfo.Resolve()
            {
                return Resolve();
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

        private class DeclaredTypeWrapper : TypeWrapper<IDeclaredType>
        {
            public DeclaredTypeWrapper(PsiReflector reflector, IDeclaredType target)
                : base(reflector, target)
            {
            }

            public override IDeclaredElement DeclaredElement
            {
                get { return TypeElement; }
            }

            public override IAssemblyInfo Assembly
            {
                get
                {
                    IModule module = TypeElement.Module;
                    return Reflector.Wrap(module);
                }
            }

            public override string Name
            {
                get { return TypeElement.ShortName; }
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
                get { return Reflector.Wrap(TypeElement.GetContainingType()); }
            }

            public override INamespaceInfo Namespace
            {
                get { return Reflector.WrapNamespace(NamespaceName); }
            }

            public override ITypeInfo BaseType
            {
                get
                {
                    foreach (IDeclaredType superType in Target.GetSuperTypes())
                    {
                        IClass @class = superType.GetTypeElement() as IClass;
                        if (@class != null)
                            return Reflector.Wrap(@class);
                    }

                    return null;
                }
            }

            public override string FullName
            {
                get { return Target.GetCLRName(); }
            }

            public override bool IsGenericParameter
            {
                get { return false; }
            }

            public override TypeAttributes TypeAttributes
            {
                get
                {
                    TypeAttributes flags = 0;
                    ITypeElement typeElement = TypeElement;

                    if (typeElement is IClass)
                        flags |= TypeAttributes.Class;
                    else if (typeElement is IInterface)
                        flags |= TypeAttributes.Interface;

                    IModifiersOwner modifiers = typeElement as IModifiersOwner;
                    if (modifiers != null)
                    {
                        if (modifiers.IsAbstract)
                            flags |= TypeAttributes.Abstract;
                        if (modifiers.IsSealed)
                            flags |= TypeAttributes.Sealed;

                        bool isNested = typeElement.GetContainingType() != null;

                        switch (modifiers.GetAccessRights())
                        {
                            case AccessRights.PUBLIC:
                                flags |= isNested ? TypeAttributes.NestedPublic : TypeAttributes.Public;
                                break;
                            case AccessRights.PRIVATE:
                                flags |= isNested ? TypeAttributes.NestedPrivate : TypeAttributes.NotPublic;
                                break;
                            case AccessRights.NONE:
                            case AccessRights.INTERNAL:
                                flags |= TypeAttributes.NestedAssembly;
                                break;
                            case AccessRights.PROTECTED:
                                flags |= TypeAttributes.NestedFamily;
                                break;
                            case AccessRights.PROTECTED_AND_INTERNAL:
                                flags |= TypeAttributes.NestedFamANDAssem;
                                break;
                            case AccessRights.PROTECTED_OR_INTERNAL:
                                flags |= TypeAttributes.NestedFamORAssem;
                                break;
                        }
                    }

                    return flags;
                }
            }

            public override IList<ITypeInfo> GetInterfaces()
            {
                List<ITypeInfo> interfaces = new List<ITypeInfo>();

                foreach (IDeclaredType superType in Target.GetSuperTypes())
                {
                    IInterface @interface = superType.GetTypeElement() as IInterface;
                    if (@interface != null)
                        interfaces.Add(Reflector.Wrap(@interface));
                }

                return interfaces;
            }

            public override IList<IConstructorInfo> GetConstructors(BindingFlags bindingFlags)
            {
                List<IConstructorInfo> result = new List<IConstructorInfo>();
                AddMatchingMembers(result, bindingFlags, TypeElement.Constructors, Reflector.Wrap);
                return result;
            }

            public override IMethodInfo GetMethod(string methodName, BindingFlags bindingFlags)
            {
                IMethod match = null;
                foreach (ITypeMember member in MiscUtil.EnumerateMembers(TypeElement, methodName, true))
                {
                    IMethod method = member as IMethod;
                    if (method != null && CheckBindingFlags(bindingFlags, member))
                    {
                        if (match != null)
                            throw new AmbiguousMatchException("Found two matching methods with the same name.");

                        match = method;
                    }
                }

                return Reflector.Wrap(match);
            }

            public override IList<IMethodInfo> GetMethods(BindingFlags bindingFlags)
            {
                List<IMethodInfo> result = new List<IMethodInfo>();
                AddMatchingMembers(result, bindingFlags, TypeElement.Methods, Reflector.Wrap);
                AddMatchingMembers(result, bindingFlags, TypeElement.Operators, Reflector.Wrap);
                return result;
            }

            public override IList<IPropertyInfo> GetProperties(BindingFlags bindingFlags)
            {
                List<IPropertyInfo> result = new List<IPropertyInfo>();
                AddMatchingMembers(result, bindingFlags, TypeElement.Properties, Reflector.Wrap);
                return result;
            }

            public override IList<IFieldInfo> GetFields(BindingFlags bindingFlags)
            {
                List<IFieldInfo> result = new List<IFieldInfo>();

                IClass @class = TypeElement as IClass;
                if (@class != null)
                {
                    AddMatchingMembers(result, bindingFlags, @class.Fields, Reflector.Wrap);
                    AddMatchingMembers(result, bindingFlags, @class.Constants, Reflector.Wrap);
                }

                return result;
            }

            public override IList<IEventInfo> GetEvents(BindingFlags bindingFlags)
            {
                List<IEventInfo> result = new List<IEventInfo>();
                AddMatchingMembers(result, bindingFlags, TypeElement.Events, Reflector.Wrap);
                return result;
            }

            public override IList<IGenericParameterInfo> GetGenericParameters()
            {
                ITypeParameter[] parameter = TypeElement.TypeParameters;
                return Array.ConvertAll<ITypeParameter, IGenericParameterInfo>(parameter, Reflector.Wrap);
            }

            public override IEnumerable<IAttributeInfo> GetAttributeInfos(bool inherit)
            {
                return Reflector.GetAttributeInfosForElement(TypeElement, inherit);
            }

            private string NamespaceName
            {
                get { return CLRTypeName.NamespaceName; }
            }

            private CLRTypeName CLRTypeName
            {
                get { return new CLRTypeName(Target.GetCLRName()); }
            }

            protected ITypeElement TypeElement
            {
                get { return Target.GetTypeElement(); }
            }

            private static void AddMatchingMembers<TInput, TOutput>(ICollection<TOutput> outputCollection,
                BindingFlags flags, IEnumerable<TInput> members, Converter<TInput, TOutput> converter)
                where TInput : IModifiersOwner
            {
                foreach (TInput member in members)
                {
                    if (CheckBindingFlags(flags, member))
                        outputCollection.Add(converter(member));
                }
            }

            private static bool CheckBindingFlags(BindingFlags flags, IModifiersOwner modifiers)
            {
                bool @public = modifiers.GetAccessRights() == AccessRights.PUBLIC;
                bool @static = modifiers.IsStatic;

                return (@public && (flags & BindingFlags.Public) != 0
                    || !@public && (flags & BindingFlags.NonPublic) != 0)
                    && (@static && (flags & BindingFlags.Static) != 0
                    || !@static && (flags & BindingFlags.Instance) != 0);
            }
        }

        private sealed class GenericParameterWrapper : DeclaredTypeWrapper, IGenericParameterInfo
        {
            public GenericParameterWrapper(PsiReflector reflector, IDeclaredType target)
                : base(reflector, target)
            {
            }

            public override CodeElementKind Kind
            {
                get { return CodeElementKind.GenericParameter; }
            }

            public override bool IsGenericParameter
            {
                get { return true; }
            }

            public GenericParameterAttributes GenericParameterAttributes
            {
                get
                {
                    GenericParameterAttributes flags = 0;
                    ITypeParameter typeParameter = TypeParameter;

                    if (typeParameter.IsValueType)
                        flags |= GenericParameterAttributes.NotNullableValueTypeConstraint;
                    if (typeParameter.IsClassType)
                        flags |= GenericParameterAttributes.ReferenceTypeConstraint;
                    if (typeParameter.HasDefaultConstructor)
                        flags |= GenericParameterAttributes.DefaultConstructorConstraint;

                    return flags;
                }
            }


            public ITypeInfo ValueType
            {
                get { return Gallio.Model.Reflection.Reflector.Wrap(typeof(Type)); }
            }

            public int Position
            {
                get { return TypeParameter.Index; }
            }

            public bool Equals(ISlotInfo other)
            {
                return Equals((object) other);
            }

            private ITypeParameter TypeParameter
            {
                get { return (ITypeParameter) TypeElement; }
            }
        }

        private abstract class ConstructedTypeWrapper<TTarget> : TypeWrapper<TTarget>
            where TTarget : class, IType
        {
            public ConstructedTypeWrapper(PsiReflector reflector, TTarget target)
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

            public override IEnumerable<IAttributeInfo> GetAttributeInfos(bool inherit)
            {
                yield break;
            }
        }

        private sealed class ArrayTypeWrapper : ConstructedTypeWrapper<IArrayType>
        {
            public ArrayTypeWrapper(PsiReflector reflector, IArrayType target)
                : base(reflector, target)
            {
            }

            public override string Name
            {
                get { return ElementType.Name + "[]"; }
            }

            public override ITypeInfo ElementType
            {
                get { return Reflector.Wrap(Target.ElementType, true); }
            }

            public override int ArrayRank
            {
                get { return Target.Rank; }
            }

            public override bool IsArray
            {
                get { return true; }
            }

            public override ITypeInfo EffectiveClassType
            {
                get
                {
                    return Gallio.Model.Reflection.Reflector.Wrap(typeof(Array));
                }
            }
        }

        private sealed class PointerTypeWrapper : ConstructedTypeWrapper<IPointerType>
        {
            public PointerTypeWrapper(PsiReflector reflector, IPointerType target)
                : base(reflector, target)
            {
            }

            public override string Name
            {
                get { return ElementType.Name + "*"; }
            }

            public override ITypeInfo ElementType
            {
                get { return Reflector.Wrap(Target.ElementType, true); }
            }

            public override bool IsPointer
            {
                get { return true; }
            }

            public override ITypeInfo EffectiveClassType
            {
                get
                {
                    return Gallio.Model.Reflection.Reflector.Wrap(typeof(Pointer));
                }
            }
        }

        private abstract class FunctionWrapper<TTarget> : MemberWrapper<TTarget>, IFunctionInfo
            where TTarget : class, IFunction
        {
            public FunctionWrapper(PsiReflector reflector, TTarget target)
                : base(reflector, target)
            {
            }

            public MethodAttributes MethodAttributes
            {
                get
                {
                    MethodAttributes flags = 0;
                    IModifiersOwner modifiers = Target;

                    switch (modifiers.GetAccessRights())
                    {
                        case AccessRights.PUBLIC:
                            flags |= MethodAttributes.Public;
                            break;
                        case AccessRights.PRIVATE:
                            flags |= MethodAttributes.Private;
                            break;
                        case AccessRights.NONE:
                        case AccessRights.INTERNAL:
                            flags |= MethodAttributes.Assembly;
                            break;
                        case AccessRights.PROTECTED:
                            flags |= MethodAttributes.Family;
                            break;
                        case AccessRights.PROTECTED_AND_INTERNAL:
                            flags |= MethodAttributes.FamANDAssem;
                            break;
                        case AccessRights.PROTECTED_OR_INTERNAL:
                            flags |= MethodAttributes.FamORAssem;
                            break;
                    }

                    AddFlagIfTrue(ref flags, MethodAttributes.Abstract, modifiers.IsAbstract);
                    AddFlagIfTrue(ref flags, MethodAttributes.Final, modifiers.IsSealed);
                    AddFlagIfTrue(ref flags, MethodAttributes.Static, modifiers.IsStatic);
                    AddFlagIfTrue(ref flags, MethodAttributes.Virtual, modifiers.IsVirtual);
                    return flags;
                }
            }

            public IList<IParameterInfo> GetParameters()
            {
                IList<IParameter> parameters = Target.Parameters;
                return GenericUtils.ConvertAllToArray<IParameter, IParameterInfo>(parameters, Reflector.Wrap);
            }

            public IList<IGenericParameterInfo> GetGenericParameters()
            {
                ITypeParameter[] parameter = Target.GetSignature(null).GetTypeParameters();
                return Array.ConvertAll<ITypeParameter, IGenericParameterInfo>(parameter, Reflector.Wrap);
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

        private sealed class ConstructorWrapper : FunctionWrapper<IConstructor>, IConstructorInfo
        {
            public ConstructorWrapper(PsiReflector reflector, IConstructor target)
                : base(reflector, target)
            {
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
                return ResolveConstructor(this);
            }

            public bool Equals(IConstructorInfo other)
            {
                return Equals((object)other);
            }
        }

        private sealed class MethodWrapper : FunctionWrapper<IFunction>, IMethodInfo
        {
            public MethodWrapper(PsiReflector reflector, IFunction target)
                : base(reflector, target)
            {
            }

            public ITypeInfo ReturnType
            {
                get { return Reflector.Wrap(Target.ReturnType, true); }
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
                return ResolveMethod(this);
            }
        }

        private sealed class PropertyWrapper : MemberWrapper<IProperty>, IPropertyInfo
        {
            public PropertyWrapper(PsiReflector reflector, IProperty target)
                : base(reflector, target)
            {
            }

            public ITypeInfo ValueType
            {
                get { return Reflector.Wrap(Target.Type, true); }
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
                return Reflector.Wrap(Target.Getter(false));
            }

            public IMethodInfo GetSetMethod()
            {
                return Reflector.Wrap(Target.Setter(false));
            }

            public override MemberInfo ResolveMemberInfo()
            {
                return Resolve();
            }

            public PropertyInfo Resolve()
            {
                return ResolveProperty(this);
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

        private sealed class FieldWrapper : MemberWrapper<IField>, IFieldInfo
        {
            public FieldWrapper(PsiReflector reflector, IField target)
                : base(reflector, target)
            {
            }

            public ITypeInfo ValueType
            {
                get { return Reflector.Wrap(Target.Type, true); }
            }

            public int Position
            {
                get { return 0; }
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
                    IModifiersOwner modifiers = Target;

                    switch (modifiers.GetAccessRights())
                    {
                        case AccessRights.PUBLIC:
                            flags |= FieldAttributes.Public;
                            break;
                        case AccessRights.PRIVATE:
                            flags |= FieldAttributes.Private;
                            break;
                        case AccessRights.NONE:
                        case AccessRights.INTERNAL:
                            flags |= FieldAttributes.Assembly;
                            break;
                        case AccessRights.PROTECTED:
                            flags |= FieldAttributes.Family;
                            break;
                        case AccessRights.PROTECTED_AND_INTERNAL:
                            flags |= FieldAttributes.FamANDAssem;
                            break;
                        case AccessRights.PROTECTED_OR_INTERNAL:
                            flags |= FieldAttributes.FamORAssem;
                            break;
                    }

                    AddFlagIfTrue(ref flags, FieldAttributes.Static, modifiers.IsStatic);
                    return flags;
                }
            }

            public override CodeElementKind Kind
            {
                get { return CodeElementKind.Field; }
            }

            public FieldInfo Resolve()
            {
                return ResolveField(this);
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

        private sealed class EventWrapper : MemberWrapper<IEvent>, IEventInfo
        {
            public EventWrapper(PsiReflector reflector, IEvent target)
                : base(reflector, target)
            {
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
                return ResolveEvent(this);
            }

            public bool Equals(IEventInfo other)
            {
                return Equals((object)other);
            }
        }

        private sealed class ParameterWrapper : CodeElementWrapper<IParameter>, IParameterInfo
        {
            public ParameterWrapper(PsiReflector reflector, IParameter target)
                : base(reflector, target)
            {
            }

            public override IDeclaredElement DeclaredElement
            {
                get { return Target; }
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
                get { return Reflector.Wrap(Target.Type, true); }
            }

            public int Position
            {
                get
                {
                    return Target.ContainingParametersOwner.Parameters.IndexOf(Target);
                }
            }

            public IMemberInfo Member
            {
                get { return Reflector.Wrap((IFunction)Target.ContainingParametersOwner); }
            }

            public ParameterAttributes ParameterAttributes
            {
                get
                {
                    ParameterAttributes flags = 0;
                    AddFlagIfTrue(ref flags, ParameterAttributes.HasDefault, ! Target.GetDefaultValue().IsBadValue());
                    AddFlagIfTrue(ref flags, ParameterAttributes.In, Target.Kind != ParameterKind.OUTPUT);
                    AddFlagIfTrue(ref flags, ParameterAttributes.Out, Target.Kind != ParameterKind.VALUE);
                    AddFlagIfTrue(ref flags, ParameterAttributes.Optional, Target.IsOptional);
                    return flags;
                }
            }

            public override string Name
            {
                get { return Target.ShortName; }
            }

            public override CodeElementKind Kind
            {
                get { return CodeElementKind.Parameter; }
            }

            public override IEnumerable<IAttributeInfo> GetAttributeInfos(bool inherit)
            {
                return Reflector.GetAttributeInfosForElement(Target, inherit);
            }

            public ParameterInfo Resolve()
            {
                return ResolveParameter(this);
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

        private sealed class AttributeWrapper : BaseWrapper<IAttributeInstance>, IAttributeInfo
        {
            public AttributeWrapper(PsiReflector reflector, IAttributeInstance target)
                : base(reflector, target)
            {
            }


            public ITypeInfo Type
            {
                get { return Reflector.Wrap(Target.AttributeType, true); }
            }

            public IConstructorInfo Constructor
            {
                get { return Reflector.Wrap(Target.Constructor); }
            }

            public object[] ArgumentValues
            {
                get
                {
                    object[] values = new object[Target.Constructor.Parameters.Count];

                    for (int i = 0; i < values.Length; i++)
                        values[i] = Target.PositionParameter(i).Value;

                    return values;
                }
            }

            public object GetFieldValue(string name)
            {
                IClass @class = Target.AttributeType as IClass;
                if (@class != null)
                {
                    foreach (IField field in @class.Fields)
                    {
                        if (field.ShortName == name)
                        {
                            ConstantValue2 value = Target.NamedParameter(field);
                            if (!value.IsBadValue())
                                return value.Value;
                        }
                    }
                }

                throw new ArgumentException(String.Format("The attribute does not have an initialized field named '{0}'.", name));
            }

            public object GetPropertyValue(string name)
            {
                IClass @class = Target.AttributeType as IClass;
                if (@class != null)
                {
                    foreach (IProperty property in @class.Properties)
                    {
                        if (property.ShortName == name)
                        {
                            ConstantValue2 value = Target.NamedParameter(property);
                            if (!value.IsBadValue())
                                return value.Value;
                        }
                    }
                }

                throw new ArgumentException(String.Format("The attribute does not have an initialized property named '{0}'.", name));
            }


            public IDictionary<IFieldInfo, object> FieldValues
            {
                get
                {
                    Dictionary<IFieldInfo, object> values = new Dictionary<IFieldInfo, object>();

                    IClass @class = Target.AttributeType as IClass;
                    if (@class != null)
                    {
                        foreach (IField field in @class.Fields)
                        {
                            if (!field.IsStatic && ! field.IsReadonly && !field.IsConstant)
                            {
                                ConstantValue2 value = Target.NamedParameter(field);
                                if (!value.IsBadValue())
                                    values.Add(Reflector.Wrap(field), value.Value);
                            }
                        }
                    }

                    return values;
                }
            }

            public IDictionary<IPropertyInfo, object> PropertyValues
            {
                get
                {
                    Dictionary<IPropertyInfo, object> values = new Dictionary<IPropertyInfo, object>();

                    IClass @class = Target.AttributeType as IClass;
                    if (@class != null)
                    {
                        foreach (IProperty property in @class.Properties)
                        {
                            if (!property.IsStatic && property.IsWritable && !property.IsAbstract)
                            {
                                ConstantValue2 value = Target.NamedParameter(property);
                                if (!value.IsBadValue())
                                    values.Add(Reflector.Wrap(property), value.Value);
                            }
                        }
                    }

                    return values;
                }
            }

            public object Resolve()
            {
                return AttributeUtils.CreateAttribute(this);
            }
        }
    }
}