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
using JetBrains.ProjectModel.Build;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Caches;
using JetBrains.ReSharper.Psi.Util;
using JetBrains.Shell;

namespace Gallio.ReSharperRunner.Reflection
{
    /// <summary>
    /// Wraps ReSharper code model types using the reflection adapter interfaces.
    /// </summary>
    /// <todo author="jeff">
    /// Support inherited attribute lookup.
    /// Support Resolve() method.
    /// </todo>
    public static class PsiReflector
    {
        /// <summary>
        /// Obtains a reflection wrapper for a module.
        /// </summary>
        /// <param name="target">The module, or null if none</param>
        /// <returns>The reflection wrapper, or null if none</returns>
        public static IAssemblyInfo Wrap(IModule target)
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
        public static IAssemblyInfo Wrap(IProject target)
        {
            return target != null ? new ProjectWrapper(target) : null;
        }

        /// <summary>
        /// Obtains a reflection wrapper for an assembly.
        /// </summary>
        /// <param name="target">The assembly, or null if none</param>
        /// <returns>The reflection wrapper, or null if none</returns>
        public static IAssemblyInfo Wrap(IAssembly target)
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
        /// <param name="throwIfUnsupported">If true, throws <exception="NotSupportedException" /> if
        /// the target is not of a recognized type, otherwise just returns null</param>
        /// <returns>The reflection wrapper, or null if none</returns>
        public static ITypeInfo Wrap(IType target, bool throwIfUnsupported)
        {
            if (target == null)
                return null;

            IDeclaredType declaredType = target as IDeclaredType;
            if (declaredType != null)
                return new DeclaredTypeWrapper(declaredType);

            IArrayType arrayType = target as IArrayType;
            if (arrayType != null)
                return new ArrayTypeWrapper(arrayType);

            IPointerType pointerType = target as IPointerType;
            if (pointerType != null)
                return new PointerTypeWrapper(pointerType);

            if (throwIfUnsupported)
                throw new NotSupportedException("Unsupported type.");
            return null;
        }

        /// <summary>
        /// Obtains a reflection wrapper for a declared element.
        /// </summary>
        /// <param name="target">The element, or null if none</param>
        /// be mapped, otherwise just returns null</param>
        /// <param name="throwIfUnsupported">If true, throws <exception="NotSupportedException" /> if
        /// the target is not of a recognized type, otherwise just returns null</param>
        /// <returns>The reflection wrapper, or null if none</returns>
        public static ICodeElementInfo Wrap(IDeclaredElement target, bool throwIfUnsupported)
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
        public static ITypeInfo Wrap(ITypeElement target)
        {
            return target != null ? Wrap(TypeFactory.CreateType(target), true) : null;
        }

        /// <summary>
        /// Obtains a reflection wrapper for a function.
        /// </summary>
        /// <param name="target">The function, or null if none</param>
        /// <returns>The reflection wrapper, or null if none</returns>
        public static IFunctionInfo Wrap(IFunction target)
        {
            if (target == null)
                return null;

            IConstructor constructor = target as IConstructor;
            return constructor != null ? (IFunctionInfo)new ConstructorWrapper(constructor) : new MethodWrapper(target);
        }

        /// <summary>
        /// Obtains a reflection wrapper for a method.
        /// </summary>
        /// <param name="target">The method, or null if none</param>
        /// <returns>The reflection wrapper, or null if none</returns>
        public static IMethodInfo Wrap(IMethod target)
        {
            return target != null ? new MethodWrapper(target) : null;
        }

        /// <summary>
        /// Obtains a reflection wrapper for a method.
        /// </summary>
        /// <param name="target">The method, or null if none</param>
        /// <returns>The reflection wrapper, or null if none</returns>
        public static IMethodInfo Wrap(IOperator target)
        {
            return target != null ? new MethodWrapper(target) : null;
        }

        /// <summary>
        /// Obtains a reflection wrapper for a constructor.
        /// </summary>
        /// <param name="target">The constructor, or null if none</param>
        /// <returns>The reflection wrapper, or null if none</returns>
        public static IConstructorInfo Wrap(IConstructor target)
        {
            return target != null ? new ConstructorWrapper(target) : null;
        }

        /// <summary>
        /// Obtains a reflection wrapper for a property.
        /// </summary>
        /// <param name="target">The property, or null if none</param>
        /// <returns>The reflection wrapper, or null if none</returns>
        public static IPropertyInfo Wrap(IProperty target)
        {
            return target != null ? new PropertyWrapper(target) : null;
        }

        /// <summary>
        /// Obtains a reflection wrapper for a field.
        /// </summary>
        /// <param name="target">The field, or null if none</param>
        /// <returns>The reflection wrapper, or null if none</returns>
        public static IFieldInfo Wrap(IField target)
        {
            return target != null ? new FieldWrapper(target) : null;
        }

        /// <summary>
        /// Obtains a reflection wrapper for an event.
        /// </summary>
        /// <param name="target">The event, or null if none</param>
        /// <returns>The reflection wrapper, or null if none</returns>
        public static IEventInfo Wrap(IEvent target)
        {
            return target != null ? new EventWrapper(target) : null;
        }

        /// <summary>
        /// Obtains a reflection wrapper for a parameter.
        /// </summary>
        /// <param name="target">The parameter, or null if none</param>
        /// <returns>The reflection wrapper, or null if none</returns>
        public static IParameterInfo Wrap(IParameter target)
        {
            return target != null ? new ParameterWrapper(target) : null;
        }

        /// <summary>
        /// Obtains a reflection wrapper for an attribute.
        /// </summary>
        /// <param name="target">The attribute, or null if none</param>
        /// <returns>The reflection wrapper, or null if none</returns>
        public static IAttributeInfo Wrap(IAttributeInstance target)
        {
            return target != null ? new AttributeWrapper(target) : null;
        }

        public static IEnumerable<IAttributeInfo> GetAttributeInfosForModule(IModuleAttributes moduleAttributes, bool inherit)
        {
            foreach (IAttributeInstance attrib in moduleAttributes.AttributeInstances)
                yield return Wrap(attrib);
        }

        public static IEnumerable<IAttributeInfo> GetAttributeInfosForElement(IAttributesOwner element, bool inherit)
        {
            foreach (IAttributeInstance attrib in element.GetAttributeInstances(inherit))
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

            public bool HasAttribute(Type attributeType, bool inherit)
            {
                return AttributeUtils.HasAttributeOfType(GetAttributeInfos(inherit), attributeType);
            }

            public IEnumerable<object> GetAttributes(Type attributeType, bool inherit)
            {
                return AttributeUtils.CreateAttributesOfType(GetAttributeInfos(inherit), attributeType);
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

        private abstract class ModuleWrapper<TTarget> : CodeElementWrapper<TTarget>, IAssemblyInfo
            where TTarget : class, IModule
        {
            public ModuleWrapper(TTarget target)
                : base(target)
            {
            }

            public override string Name
            {
                get { return GetName().Name; }
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
                return Assembly.LoadFrom(Path);
            }

            public string Path
            {
                get { return GetAssemblyFile().Location.FullPath; }
            }

            public AssemblyName GetName()
            {
                return GetAssemblyFile().AssemblyName;
            }

            public ITypeInfo GetType(string typeName)
            {
                return Wrap(GetDeclarationsCache().GetTypeElementByCLRName(typeName));
            }

            public ITypeInfo[] GetExportedTypes()
            {
                INamespace ns = PsiManager.GetNamespace("");
                IDeclarationsCache cache = GetDeclarationsCache();

                List<ITypeInfo> types = new List<ITypeInfo>();
                PopulateExportedTypes(types, ns, cache);

                return types.ToArray();
            }

            public override IEnumerable<IAttributeInfo> GetAttributeInfos(bool inherit)
            {
                return GetAttributeInfosForModule(PsiManager.GetModuleAttributes(Target), inherit);
            }

            private void PopulateExportedTypes(IList<ITypeInfo> types, INamespace ns, IDeclarationsCache cache)
            {
                foreach (IDeclaredElement element in ns.GetNestedElements(cache))
                {
                    ITypeElement type = element as ITypeElement;
                    if (type != null)
                    {
                        PopulateExportedTypes(types, type);
                    }
                    else
                    {
                        INamespace nestedNamespace = element as INamespace;
                        if (nestedNamespace != null)
                            PopulateExportedTypes(types, nestedNamespace, cache);
                    }
                }
            }

            private void PopulateExportedTypes(IList<ITypeInfo> types, ITypeElement type)
            {
                IModifiersOwner modifiers = type as IModifiersOwner;
                if (modifiers != null && modifiers.GetAccessRights() == AccessRights.PUBLIC)
                {
                    types.Add(Wrap(type));

                    foreach (ITypeElement nestedType in type.NestedTypes)
                        PopulateExportedTypes(types, nestedType);
                }
            }

            public abstract AssemblyName[] GetReferencedAssemblies();

            protected abstract IAssemblyFile GetAssemblyFile();

            protected abstract IDeclarationsCache GetDeclarationsCache();

            protected PsiManager PsiManager
            {
                get { return PsiManager.GetInstance(Target.GetSolution()); }
            }
        }

        private sealed class AssemblyWrapper : ModuleWrapper<IAssembly>
        {
            public AssemblyWrapper(IAssembly target)
                : base(target)
            {
            }

            public override AssemblyName[] GetReferencedAssemblies()
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
            public ProjectWrapper(IProject target)
                : base(target)
            {
            }

            public override AssemblyName[] GetReferencedAssemblies()
            {
                ICollection<IModuleReference> moduleRefs = Target.GetModuleReferences();
                return GenericUtils.ConvertAllToArray<IModuleReference, AssemblyName>(moduleRefs, delegate(IModuleReference moduleRef)
                {
                    return Wrap(moduleRef.ResolveReferencedModule()).GetName();
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

        private abstract class MemberWrapper<TTarget> : CodeElementWrapper<TTarget>, IMemberInfo, IDeclaredElementAccessor
            where TTarget : class, ITypeMember
        {
            public MemberWrapper(TTarget target)
                : base(target)
            {
            }

            public IDeclaredElement DeclaredElement
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
                get { return Wrap(Target.GetContainingType()); }
            }

            public override IEnumerable<IAttributeInfo> GetAttributeInfos(bool inherit)
            {
                return GetAttributeInfosForElement(Target, inherit);
            }

            MemberInfo IMemberInfo.Resolve()
            {
                return ResolveMemberInfo();
            }

            public abstract MemberInfo ResolveMemberInfo();
        }

        private interface ITypeWrapper
        {
            IType Target { get; }
        }

        private abstract class TypeWrapper<TTarget> : CodeElementWrapper<TTarget>, ITypeInfo, ITypeWrapper
            where TTarget : class, IType
        {
            public TypeWrapper(TTarget target)
                : base(target)
            {
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
            public abstract TypeAttributes Modifiers { get; }
            public abstract ITypeInfo[] GetInterfaces();
            public abstract IConstructorInfo[] GetConstructors(BindingFlags bindingFlags);
            public abstract IMethodInfo GetMethod(string methodName, BindingFlags bindingFlags);
            public abstract IMethodInfo[] GetMethods(BindingFlags bindingFlags);
            public abstract IPropertyInfo[] GetProperties(BindingFlags bindingFlags);
            public abstract IFieldInfo[] GetFields(BindingFlags bindingFlags);
            public abstract IEventInfo[] GetEvents(BindingFlags bindingFlags);
            public abstract Type Resolve();

            MemberInfo IMemberInfo.Resolve()
            {
                return Resolve();
            }
        }

        private sealed class DeclaredTypeWrapper : TypeWrapper<IDeclaredType>, IDeclaredElementAccessor
        {
            public DeclaredTypeWrapper(IDeclaredType target)
                : base(target)
            {
            }

            public IDeclaredElement DeclaredElement
            {
                get { return TypeElement; }
            }

            public override IAssemblyInfo Assembly
            {
                get
                {
                    IModule module = TypeElement.Module;
                    return Wrap(module);
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
                get { return Wrap(TypeElement.GetContainingType()); }
            }

            public override INamespaceInfo Namespace
            {
                get { return WrapNamespace(NamespaceName); }
            }

            public override ITypeInfo BaseType
            {
                get
                {
                    foreach (IDeclaredType superType in Target.GetSuperTypes())
                    {
                        IClass @class = superType.GetTypeElement() as IClass;
                        if (@class != null)
                            return Wrap(@class);
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
                get { return Target is ITypeParameter;  }
            }

            public override TypeAttributes Modifiers
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

                        switch (modifiers.GetAccessRights())
                        {
                            case AccessRights.PUBLIC:
                                flags |= TypeAttributes.NestedPublic;
                                break;
                            case AccessRights.INTERNAL:
                                flags |= TypeAttributes.NestedAssembly;
                                break;
                            case AccessRights.PRIVATE:
                                flags |= TypeAttributes.NestedPrivate;
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

            public override ITypeInfo[] GetInterfaces()
            {
                List<ITypeInfo> interfaces = new List<ITypeInfo>();

                foreach (IDeclaredType superType in Target.GetSuperTypes())
                {
                    IInterface @interface = superType.GetTypeElement() as IInterface;
                    if (@interface != null)
                        interfaces.Add(Wrap(@interface));
                }

                return interfaces.ToArray();
            }

            public override IConstructorInfo[] GetConstructors(BindingFlags bindingFlags)
            {
                List<IConstructorInfo> result = new List<IConstructorInfo>();
                AddMatchingMembers(result, bindingFlags, TypeElement.Constructors, Wrap);
                return result.ToArray();
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

                return Wrap(match);
            }

            public override IMethodInfo[] GetMethods(BindingFlags bindingFlags)
            {
                List<IMethodInfo> result = new List<IMethodInfo>();
                AddMatchingMembers(result, bindingFlags, TypeElement.Methods, Wrap);
                AddMatchingMembers(result, bindingFlags, TypeElement.Operators, Wrap);
                return result.ToArray();
            }

            public override IPropertyInfo[] GetProperties(BindingFlags bindingFlags)
            {
                List<IPropertyInfo> result = new List<IPropertyInfo>();
                AddMatchingMembers(result, bindingFlags, TypeElement.Properties, Wrap);
                return result.ToArray();
            }

            public override IFieldInfo[] GetFields(BindingFlags bindingFlags)
            {
                List<IFieldInfo> result = new List<IFieldInfo>();

                IClass @class = TypeElement as IClass;
                if (@class != null)
                {
                    AddMatchingMembers(result, bindingFlags, @class.Fields, Wrap);
                    AddMatchingMembers(result, bindingFlags, @class.Constants, Wrap);
                }

                return result.ToArray();
            }

            public override IEventInfo[] GetEvents(BindingFlags bindingFlags)
            {
                List<IEventInfo> result = new List<IEventInfo>();
                AddMatchingMembers(result, bindingFlags, TypeElement.Events, Wrap);
                return result.ToArray();
            }

            public override IEnumerable<IAttributeInfo> GetAttributeInfos(bool inherit)
            {
                return GetAttributeInfosForElement(TypeElement, inherit);
            }

            public override Type Resolve()
            {
                return Assembly.Resolve().GetType(FullName, true);
            }

            private string NamespaceName
            {
                get { return CLRTypeName.NamespaceName; }
            }

            private CLRTypeName CLRTypeName
            {
                get { return new CLRTypeName(Target.GetCLRName()); }
            }

            private ITypeElement TypeElement
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

        private abstract class ConstructedTypeWrapper<TTarget> : TypeWrapper<TTarget>
            where TTarget : class, IType
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

            public override ITypeInfo[] GetInterfaces()
            {
                return EffectiveClassType.GetInterfaces();
            }

            public override IConstructorInfo[] GetConstructors(BindingFlags bindingFlags)
            {
                return EffectiveClassType.GetConstructors(bindingFlags);
            }

            public override IMethodInfo GetMethod(string methodName, BindingFlags bindingFlags)
            {
                return EffectiveClassType.GetMethod(methodName, bindingFlags);
            }

            public override IMethodInfo[] GetMethods(BindingFlags bindingFlags)
            {
                return EffectiveClassType.GetMethods(bindingFlags);
            }

            public override IPropertyInfo[] GetProperties(BindingFlags bindingFlags)
            {
                return EffectiveClassType.GetProperties(bindingFlags);
            }

            public override IFieldInfo[] GetFields(BindingFlags bindingFlags)
            {
                return EffectiveClassType.GetFields(bindingFlags);
            }

            public override IEventInfo[] GetEvents(BindingFlags bindingFlags)
            {
                return EffectiveClassType.GetEvents(bindingFlags);
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

        private sealed class ArrayTypeWrapper : ConstructedTypeWrapper<IArrayType>
        {
            public ArrayTypeWrapper(IArrayType target)
                : base(target)
            {
            }

            public override string Name
            {
                get { return ElementType.Name + "[]"; }
            }

            public override ITypeInfo ElementType
            {
                get { return Wrap(Target.ElementType, true); }
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
                    // Should return a type of System.Array.
                    throw new NotImplementedException();
                }
            }

            public override Type Resolve()
            {
                int rank = Target.Rank;
                Type elementType = ElementType.Resolve();

                if (rank == 1)
                    return elementType.MakeArrayType();
                else
                    return elementType.MakeArrayType(rank);
            }
        }

        private sealed class PointerTypeWrapper : ConstructedTypeWrapper<IPointerType>
        {
            public PointerTypeWrapper(IPointerType target)
                : base(target)
            {
            }

            public override string Name
            {
                get { return ElementType.Name + "*"; }
            }

            public override ITypeInfo ElementType
            {
                get { return Wrap(Target.ElementType, true); }
            }

            public override bool IsPointer
            {
                get { return true; }
            }

            public override ITypeInfo EffectiveClassType
            {
                get
                {
                    // Should return System.Pointer.
                    throw new NotImplementedException();
                }
            }

            public override Type Resolve()
            {
                return ElementType.Resolve().MakePointerType();
            }
        }

        private abstract class FunctionWrapper<TTarget> : MemberWrapper<TTarget>, IFunctionInfo
            where TTarget : class, IFunction
        {
            public FunctionWrapper(TTarget target)
                : base(target)
            {
            }

            public MethodAttributes Modifiers
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
                        case AccessRights.INTERNAL:
                            flags |= MethodAttributes.Assembly;
                            break;
                        case AccessRights.PRIVATE:
                            flags |= MethodAttributes.Private;
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

            public IParameterInfo[] GetParameters()
            {
                IList<IParameter> parameters = Target.Parameters;
                return GenericUtils.ConvertAllToArray<IParameter, IParameterInfo>(parameters, Wrap);
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

        private sealed class ConstructorWrapper : FunctionWrapper<IConstructor>, IConstructorInfo
        {
            public ConstructorWrapper(IConstructor target)
                : base(target)
            {
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

        private sealed class MethodWrapper : FunctionWrapper<IFunction>, IMethodInfo
        {
            public MethodWrapper(IFunction target)
                : base(target)
            {
            }

            public ITypeInfo ReturnType
            {
                get { return Wrap(Target.ReturnType, true); }
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

        private sealed class PropertyWrapper : MemberWrapper<IProperty>, IPropertyInfo
        {
            public PropertyWrapper(IProperty target)
                : base(target)
            {
            }

            public ITypeInfo ValueType
            {
                get { return Wrap(Target.Type, true); }
            }

            public int Position
            {
                get { return 0; }
            }

            public IMethodInfo GetGetMethod()
            {
                return Wrap(Target.Getter(false));
            }

            public IMethodInfo GetSetMethod()
            {
                return Wrap(Target.Setter(false));
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

        private sealed class FieldWrapper : MemberWrapper<IField>, IFieldInfo
        {
            public FieldWrapper(IField target)
                : base(target)
            {
            }

            public ITypeInfo ValueType
            {
                get { return Wrap(Target.Type, true); }
            }

            public int Position
            {
                get { return 0; }
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

        private sealed class EventWrapper : MemberWrapper<IEvent>, IEventInfo
        {
            public EventWrapper(IEvent target)
                : base(target)
            {
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

        private sealed class ParameterWrapper : CodeElementWrapper<IParameter>, IParameterInfo
        {
            public ParameterWrapper(IParameter target)
                : base(target)
            {
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
                get { return Wrap(Target.Type, true); }
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
                get { return Wrap((IFunction) Target.ContainingParametersOwner); }
            }

            public ParameterAttributes Modifiers
            {
                get
                {
                    ParameterAttributes flags = 0;
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

            public override IEnumerable<IAttributeInfo> GetAttributeInfos(bool inherit)
            {
                return GetAttributeInfosForElement(Target, inherit);
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
            private readonly IAttributeInstance attrib;

            public AttributeWrapper(IAttributeInstance attrib)
            {
                if (attrib == null)
                    throw new ArgumentNullException("attrib");

                this.attrib = attrib;
            }


            public ITypeInfo Type
            {
                get { return Wrap(attrib.AttributeType, true); }
            }

            public IConstructorInfo Constructor
            {
                get { return Wrap(attrib.Constructor); }
            }

            public object[] ArgumentValues
            {
                get
                {
                    object[] values = new object[attrib.Constructor.Parameters.Count];

                    for (int i = 0; i < values.Length; i++)
                        values[i] = attrib.PositionParameter(i).Value;

                    return values;
                }
            }

            public IDictionary<IFieldInfo, object> FieldValues
            {
                get
                {
                    Dictionary<IFieldInfo, object> values = new Dictionary<IFieldInfo, object>();

                    IClass @class = attrib.AttributeType as IClass;
                    if (@class != null)
                    {
                        foreach (IField field in @class.Fields)
                        {
                            if (!field.IsStatic && ! field.IsReadonly && !field.IsConstant)
                            {
                                ConstantValue2 value = attrib.NamedParameter(field);
                                if (!value.IsBadValue())
                                    values.Add(Wrap(field), value.Value);
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

                    IClass @class = attrib.AttributeType as IClass;
                    if (@class != null)
                    {
                        foreach (IProperty property in @class.Properties)
                        {
                            if (!property.IsStatic && property.IsWritable && !property.IsAbstract)
                            {
                                ConstantValue2 value = attrib.NamedParameter(property);
                                if (!value.IsBadValue())
                                    values.Add(Wrap(property), value.Value);
                            }
                        }
                    }

                    return values;
                }
            }
        }
    }
}