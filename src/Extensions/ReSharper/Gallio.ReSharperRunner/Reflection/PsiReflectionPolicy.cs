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
using Gallio.Common;
using Gallio.Common.Reflection;
using Gallio.Common.Reflection.Impl;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Caches;
using Gallio.Common.Collections;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Impl.Caches2;
using JetBrains.ReSharper.Psi.Impl.Special;
using JetBrains.ReSharper.Psi.Resolve;
using JetBrains.ReSharper.Psi.Tree;
#if RESHARPER_31
using JetBrains.ReSharper.Editor;
using ReSharperDocumentRange = JetBrains.ReSharper.Editor.DocumentRange;
#else
using JetBrains.DocumentModel;
using ReSharperDocumentRange = JetBrains.DocumentModel.DocumentRange;
#endif
#if ! RESHARPER_50_OR_NEWER
using JetBrains.ProjectModel.Build;
#endif

namespace Gallio.ReSharperRunner.Reflection
{
    /// <summary>
    /// Wraps ReSharper code model objects using the reflection adapter interfaces.
    /// </summary>
    public class PsiReflectionPolicy : ReSharperReflectionPolicy
    {
        private readonly PsiManager psiManager;

        private KeyedMemoizer<IModule, StaticAssemblyWrapper> assemblyMemoizer = new KeyedMemoizer<IModule, StaticAssemblyWrapper>();
        private KeyedMemoizer<IType, StaticTypeWrapper> typeMemoizer = new KeyedMemoizer<IType, StaticTypeWrapper>();
        private KeyedMemoizer<ITypeElement, StaticDeclaredTypeWrapper> typeWithoutSubstitutionMemoizer = new KeyedMemoizer<ITypeElement, StaticDeclaredTypeWrapper>();
        private KeyedMemoizer<IDeclaredType, StaticDeclaredTypeWrapper> declaredTypeMemoizer = new KeyedMemoizer<IDeclaredType, StaticDeclaredTypeWrapper>();

        /// <summary>
        /// Creates a reflector with the specified PSI manager.
        /// </summary>
        /// <param name="psiManager">The PSI manager.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="psiManager"/> is null.</exception>
        public PsiReflectionPolicy(PsiManager psiManager)
        {
            if (psiManager == null)
                throw new ArgumentNullException("psiManager");

            this.psiManager = psiManager;
        }

        #region Wrapping
        /// <summary>
        /// Obtains a reflection wrapper for a declared element.
        /// </summary>
        /// <param name="target">The element, or null if none.</param>
        /// <returns>The reflection wrapper, or null if none.</returns>
        public ICodeElementInfo Wrap(IDeclaredElement target)
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
                return Reflector.WrapNamespace(@namespace.QualifiedName);

            return null;
        }

        /// <summary>
        /// Obtains a reflection wrapper for a type.
        /// </summary>
        /// <param name="target">The type, or null if none.</param>
        /// <returns>The reflection wrapper, or null if none.</returns>
        public StaticTypeWrapper Wrap(ITypeElement target)
        {
            return target != null ? MakeTypeWithoutSubstitution(target) : null;
        }

        /// <summary>
        /// Obtains a reflection wrapper for a function.
        /// </summary>
        /// <param name="target">The function, or null if none.</param>
        /// <returns>The reflection wrapper, or null if none.</returns>
        public StaticFunctionWrapper Wrap(IFunction target)
        {
            if (target == null)
                return null;

            IConstructor constructor = target as IConstructor;
            if (constructor != null)
                return Wrap(constructor);

            IMethod method = target as IMethod;
            if (method != null)
                return Wrap(method);

            IOperator @operator = target as IOperator;
            if (@operator != null)
                return Wrap(@operator);

            throw new NotSupportedException("Unsupported declared element type: " + target);
        }

        /// <summary>
        /// Obtains a reflection wrapper for a constructor.
        /// </summary>
        /// <param name="target">The constructor, or null if none.</param>
        /// <returns>The reflection wrapper, or null if none.</returns>
        public StaticConstructorWrapper Wrap(IConstructor target)
        {
            if (target == null)
                return null;

            StaticDeclaredTypeWrapper declaringType = MakeDeclaredTypeWithoutSubstitution(target.GetContainingType());
            return new StaticConstructorWrapper(this, target, declaringType);
        }

        /// <summary>
        /// Obtains a reflection wrapper for a method.
        /// </summary>
        /// <param name="target">The method, or null if none.</param>
        /// <returns>The reflection wrapper, or null if none.</returns>
        public StaticMethodWrapper Wrap(IMethod target)
        {
            if (target == null)
                return null;

            StaticDeclaredTypeWrapper declaringType = MakeDeclaredTypeWithoutSubstitution(target.GetContainingType());
            return new StaticMethodWrapper(this, target, declaringType, declaringType, declaringType.Substitution);
        }

        /// <summary>
        /// Obtains a reflection wrapper for an operator.
        /// </summary>
        /// <param name="target">The method, or null if none.</param>
        /// <returns>The reflection wrapper, or null if none.</returns>
        public StaticMethodWrapper Wrap(IOperator target)
        {
            if (target == null)
                return null;

            StaticDeclaredTypeWrapper declaringType = MakeDeclaredTypeWithoutSubstitution(target.GetContainingType());
            return new StaticMethodWrapper(this, target, declaringType, declaringType, declaringType.Substitution);
        }

        /// <summary>
        /// Obtains a reflection wrapper for a property.
        /// </summary>
        /// <param name="target">The property, or null if none.</param>
        /// <returns>The reflection wrapper, or null if none.</returns>
        public StaticPropertyWrapper Wrap(IProperty target)
        {
            if (target == null)
                return null;

            StaticDeclaredTypeWrapper declaringType = MakeDeclaredTypeWithoutSubstitution(target.GetContainingType());
            return new StaticPropertyWrapper(this, target, declaringType, declaringType);
        }

        /// <summary>
        /// Obtains a reflection wrapper for a field.
        /// </summary>
        /// <param name="target">The field, or null if none.</param>
        /// <returns>The reflection wrapper, or null if none.</returns>
        public StaticFieldWrapper Wrap(IField target)
        {
            if (target == null)
                return null;

            StaticDeclaredTypeWrapper declaringType = MakeDeclaredTypeWithoutSubstitution(target.GetContainingType());
            return new StaticFieldWrapper(this, target, declaringType, declaringType);
        }

        /// <summary>
        /// Obtains a reflection wrapper for an event.
        /// </summary>
        /// <param name="target">The event, or null if none.</param>
        /// <returns>The reflection wrapper, or null if none.</returns>
        public StaticEventWrapper Wrap(IEvent target)
        {
            if (target == null)
                return null;

            StaticDeclaredTypeWrapper declaringType = MakeDeclaredTypeWithoutSubstitution(target.GetContainingType());
            return new StaticEventWrapper(this, target, declaringType, declaringType);
        }

        /// <summary>
        /// Obtains a reflection wrapper for a parameter.
        /// </summary>
        /// <param name="target">The parameter, or null if none.</param>
        /// <returns>The reflection wrapper, or null if none.</returns>
        public StaticParameterWrapper Wrap(IParameter target)
        {
            if (target == null)
                return null;

            var member = (StaticMemberWrapper) Wrap(target.ContainingParametersOwner);
            if (member == null)
                return null;

            return new StaticParameterWrapper(this, target, member);
        }
        #endregion

        #region Assemblies
        protected override IAssemblyInfo LoadAssemblyImpl(AssemblyName assemblyName)
        {
            foreach (IProject project in psiManager.Solution.GetAllProjects())
            {
                try {
#if RESHARPER_51
                    if (BuildSettingsManager.HasInstance(project) == false)
                        continue;
#endif
                    IAssemblyFile assemblyFile = BuildSettingsManager.GetInstance(project).GetOutputAssemblyFile();
#if RESHARPER_50_OR_NEWER
                    if (assemblyFile != null && IsMatchingAssemblyName(assemblyName, new AssemblyName(assemblyFile.AssemblyName.FullName)))
#else
                    if (assemblyFile != null && IsMatchingAssemblyName(assemblyName, assemblyFile.AssemblyName))
#endif
                    {
                        return WrapModule(project);
                    }
                }
                catch (InvalidOperationException)
                {
                    // May be thrown if build settings are not available for the project.
                }
            }

            foreach (IAssembly assembly in psiManager.Solution.GetAllAssemblies())
            {
#if RESHARPER_50_OR_NEWER
                if (IsMatchingAssemblyName(assemblyName, new AssemblyName(assembly.AssemblyName.FullName)))
#else
                if (IsMatchingAssemblyName(assemblyName, assembly.AssemblyName))
#endif
                {
                    return WrapModule(assembly);
                }
            }

            throw new ArgumentException(String.Format("Could not find assembly '{0}' in the ReSharper code cache.",
                assemblyName.FullName));
        }

        protected override IAssemblyInfo LoadAssemblyFromImpl(string assemblyFile)
        {
            throw new NotSupportedException("The PSI metadata policy does not support loading assemblies from files.");
        }

        protected override IEnumerable<StaticAttributeWrapper> GetAssemblyCustomAttributes(StaticAssemblyWrapper assembly)
        {
            IModule moduleHandle = (IModule) assembly.Handle;

#if RESHARPER_31 || RESHARPER_40 || RESHARPER_41
            foreach (IAttributeInstance attrib in psiManager.GetModuleAttributes(moduleHandle).AttributeInstances)
            {
                if (IsAttributeInstanceValid(attrib))
                    yield return new StaticAttributeWrapper(this, attrib);
            }
#else
            CacheManagerEx cacheManager = CacheManagerEx.GetInstance(psiManager.Solution);
            IPsiModule psiModule = GetPsiModule(moduleHandle);
            if (psiModule != null)
            {
                foreach (IAttributeInstance attrib in cacheManager.GetModuleAttributes(psiModule).AttributeInstances)
                {
                    if (IsAttributeInstanceValid(attrib))
                        yield return new StaticAttributeWrapper(this, attrib);
                }
            }
#endif
        }

        protected override AssemblyName GetAssemblyName(StaticAssemblyWrapper assembly)
        {
            IModule moduleHandle = (IModule) assembly.Handle;
            return GetAssemblyName(moduleHandle);
        }

        protected override string GetAssemblyPath(StaticAssemblyWrapper assembly)
        {
            IModule moduleHandle = (IModule)assembly.Handle;
            IAssemblyFile assemblyFile = GetAssemblyFile(moduleHandle);
            if (assemblyFile == null)
                return @"unknown";
            
            return assemblyFile.Location.FullPath;
        }

        protected override IList<AssemblyName> GetAssemblyReferences(StaticAssemblyWrapper assembly)
        {
            IProject projectHandle = assembly.Handle as IProject;
            if (projectHandle != null)
            {
                if (projectHandle.IsValid)
                {
                    ICollection<IModuleReference> moduleRefs = projectHandle.GetModuleReferences();
                    return GenericCollectionUtils.ConvertAllToArray(moduleRefs, delegate(IModuleReference moduleRef)
                    {
                        return GetAssemblyName(moduleRef.ResolveReferencedModule());
                    });
                }
            }

            // FIXME! Don't know how to handle referenced assemblies for assemblies without loading them.
            //IAssembly assemblyHandle = (IAssembly)assembly.Handle;
            //return assembly.Resolve(true).GetReferencedAssemblies();
            return EmptyArray<AssemblyName>.Instance;
        }

        protected override IList<StaticDeclaredTypeWrapper> GetAssemblyExportedTypes(StaticAssemblyWrapper assembly)
        {
            IModule moduleHandle = (IModule)assembly.Handle;
            return GetAssemblyTypes(moduleHandle, false);
        }

        protected override IList<StaticDeclaredTypeWrapper> GetAssemblyTypes(StaticAssemblyWrapper assembly)
        {
            IModule moduleHandle = (IModule)assembly.Handle;
            return GetAssemblyTypes(moduleHandle, true);
        }

        protected override StaticDeclaredTypeWrapper GetAssemblyType(StaticAssemblyWrapper assembly, string typeName)
        {
            IModule moduleHandle = (IModule)assembly.Handle;
            IDeclarationsCache cache = GetAssemblyDeclarationsCache(moduleHandle);
            if (cache == null)
                return null;
            
            ITypeElement typeHandle = cache.GetTypeElementByCLRName(typeName);
            return typeHandle != null && typeHandle.IsValid() ?
                MakeDeclaredTypeWithoutSubstitution(typeHandle) : null;
        }

        private static bool IsMatchingAssemblyName(AssemblyName desiredAssemblyName, AssemblyName candidateAssemblyName)
        {
            bool haveDesiredFullName = desiredAssemblyName.Name != desiredAssemblyName.FullName;
            bool haveCandidateFullName = candidateAssemblyName.Name != candidateAssemblyName.FullName;

            if (haveDesiredFullName && haveCandidateFullName)
                return desiredAssemblyName.FullName == candidateAssemblyName.FullName;

            return desiredAssemblyName.Name == candidateAssemblyName.Name;
        }

        // note: result may be null
        private static IAssemblyFile GetAssemblyFile(IModule moduleHandle)
        {
            IProject projectHandle = moduleHandle as IProject;
            if (projectHandle != null)
                return GetAssemblyFile(projectHandle);

            IAssembly assemblyHandle = (IAssembly) moduleHandle;
            IAssemblyFile[] files = assemblyHandle.GetFiles();
            return files[0];
        }

        // note: result may be null
        private static IAssemblyFile GetAssemblyFile(IProject projectHandle)
        {
            return BuildSettingsManager.GetInstance(projectHandle).GetOutputAssemblyFile();
        }

        private static AssemblyName GetAssemblyName(IModule moduleHandle)
        {
            IProject projectHandle = moduleHandle as IProject;
            if (projectHandle != null)
            {
                IAssemblyFile assemblyFile = GetAssemblyFile(projectHandle);
                AssemblyName name;

                if (assemblyFile == null)
                {
                    name = new AssemblyName(moduleHandle.Name);
                }
                else
                {
#if RESHARPER_50_OR_NEWER
                    name = new AssemblyName(assemblyFile.AssemblyName.FullName);
#else
                    name = assemblyFile.AssemblyName;
#endif
                }
                if (name.Version == null)
                {
                    name = (AssemblyName) name.Clone();
                    name.Version = new Version(0, 0, 0, 0);
                }
                return name;
            }

            IAssembly assemblyHandle = (IAssembly) moduleHandle;
#if RESHARPER_50_OR_NEWER
            return new AssemblyName(assemblyHandle.AssemblyName.FullName);
#else
            return assemblyHandle.AssemblyName;
#endif
        }

        private IList<StaticDeclaredTypeWrapper> GetAssemblyTypes(IModule moduleHandle, bool includeNonPublicTypes)
        {
            IDeclarationsCache cache = GetAssemblyDeclarationsCache(moduleHandle);
            if (cache == null)
                return EmptyArray<StaticDeclaredTypeWrapper>.Instance;

#if ! RESHARPER_50_OR_NEWER
            INamespace namespaceHandle = psiManager.GetNamespace("");
#else
            INamespace namespaceHandle = cache.GetNamespace("");
#endif

            List<StaticDeclaredTypeWrapper> types = new List<StaticDeclaredTypeWrapper>();
            PopulateAssemblyTypes(types, namespaceHandle, cache, includeNonPublicTypes);

            return types;
        }

        private void PopulateAssemblyTypes(List<StaticDeclaredTypeWrapper> types, INamespace namespaceHandle,
            IDeclarationsCache cache, bool includeNonPublicTypes)
        {
            if (namespaceHandle == null || ! namespaceHandle.IsValid())
                return;

            foreach (IDeclaredElement elementHandle in namespaceHandle.GetNestedElements(cache))
            {
                ITypeElement typeHandle = elementHandle as ITypeElement;
                if (typeHandle != null)
                {
                    PopulateAssemblyTypes(types, typeHandle, includeNonPublicTypes);
                }
                else
                {
                    INamespace nestedNamespace = elementHandle as INamespace;
                    if (nestedNamespace != null)
                        PopulateAssemblyTypes(types, nestedNamespace, cache, includeNonPublicTypes);
                }
            }
        }

        private void PopulateAssemblyTypes(List<StaticDeclaredTypeWrapper> types, ITypeElement typeHandle, bool includeNonPublicTypes)
        {
            if (!typeHandle.IsValid())
                return;

            IModifiersOwner modifiers = typeHandle as IModifiersOwner;
            if (modifiers != null && (includeNonPublicTypes || modifiers.GetAccessRights() == AccessRights.PUBLIC))
            {
                types.Add(MakeDeclaredTypeWithoutSubstitution(typeHandle));

                foreach (ITypeElement nestedType in typeHandle.NestedTypes)
                    PopulateAssemblyTypes(types, nestedType, includeNonPublicTypes);
            }
        }

        private IDeclarationsCache GetAssemblyDeclarationsCache(IModule moduleHandle)
        {
#if RESHARPER_31 || RESHARPER_40 || RESHARPER_41
            IProject projectHandle = moduleHandle as IProject;
            if (projectHandle != null)
                return psiManager.GetDeclarationsCache(DeclarationsCacheScope.ProjectScope(projectHandle, false), true);

            IAssembly assemblyHandle = (IAssembly) moduleHandle;
            return psiManager.GetDeclarationsCache(DeclarationsCacheScope.LibraryScope(assemblyHandle, false), true);
#else
            IPsiModule psiModule = GetPsiModule(moduleHandle);
            if (psiModule == null)
                return null;
                
            return psiManager.GetDeclarationsCache(DeclarationsScopeFactory.ModuleScope(psiModule, false), true);
#endif
        }

#if RESHARPER_45_OR_NEWER
        private IPsiModule GetPsiModule(IModule moduleHandle)
        {
            var moduleManager = PsiModuleManager.GetInstance(psiManager.Solution);
            return moduleManager.GetPrimaryPsiModule(moduleHandle);
        }
#endif

        private StaticAssemblyWrapper WrapModule(IModule module)
        {
            return assemblyMemoizer.Memoize(module, () => new StaticAssemblyWrapper(this, module));
        }

        #endregion

        #region Attributes
        protected override StaticConstructorWrapper GetAttributeConstructor(StaticAttributeWrapper attribute)
        {
            IAttributeInstance attributeHandle = (IAttributeInstance)attribute.Handle;
            IDeclaredType declaredTypeHandle = attributeHandle.AttributeType;
            IConstructor constructorHandle = attributeHandle.Constructor;

            return new StaticConstructorWrapper(this, constructorHandle, MakeDeclaredType(declaredTypeHandle));
        }

        protected override ConstantValue[] GetAttributeConstructorArguments(StaticAttributeWrapper attribute)
        {
            IAttributeInstance attributeHandle = (IAttributeInstance)attribute.Handle;

            IList<IParameter> parameters = attributeHandle.Constructor.Parameters;
            if (parameters.Count == 0)
                return EmptyArray<ConstantValue>.Instance;

            List<ConstantValue> values = new List<ConstantValue>();
            for (int i = 0; ; i++)
            {
#if RESHARPER_50_OR_NEWER
                if (i == attributeHandle.PositionParameterCount)
                    break;
#endif

                ConstantValue? value = GetAttributePositionParameter(attributeHandle, i);
                if (value.HasValue)
                    values.Add(value.Value);
                else
                    break;
            }

            int lastParameterIndex = parameters.Count - 1;
            IParameter lastParameter = parameters[lastParameterIndex];
            if (!lastParameter.IsParameterArray)
                return values.ToArray();

            // Note: When presented with a constructor that accepts a variable number of
            //       arguments, ReSharper treats them as a sequence of normal parameter
            //       values.  So we we need to map them back into a params array appropriately.                
            ConstantValue[] args = new ConstantValue[parameters.Count];
            values.CopyTo(0, args, 0, lastParameterIndex);

            int varArgsCount = values.Count - lastParameterIndex;
            ConstantValue[] varArgs = new ConstantValue[varArgsCount];

            for (int i = 0; i < varArgsCount; i++)
                varArgs[i] = values[lastParameterIndex + i];

            args[lastParameterIndex] = new ConstantValue(MakeType(lastParameter.Type), varArgs);
            return args;
        }

        protected override IEnumerable<KeyValuePair<StaticFieldWrapper, ConstantValue>> GetAttributeFieldArguments(StaticAttributeWrapper attribute)
        {
            IAttributeInstance attributeHandle = (IAttributeInstance)attribute.Handle;
            foreach (StaticFieldWrapper field in attribute.Type.GetFields(BindingFlags.Public | BindingFlags.Instance))
            {
                if (ReflectorAttributeUtils.IsAttributeField(field))
                {
                    ConstantValue? value = GetAttributeNamedParameter(attributeHandle, (IField)field.Handle);
                    if (value.HasValue)
                        yield return new KeyValuePair<StaticFieldWrapper, ConstantValue>(field, value.Value);
                }
            }
        }

        protected override IEnumerable<KeyValuePair<StaticPropertyWrapper, ConstantValue>> GetAttributePropertyArguments(StaticAttributeWrapper attribute)
        {
            IAttributeInstance attributeHandle = (IAttributeInstance)attribute.Handle;
            foreach (StaticPropertyWrapper property in attribute.Type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                if (ReflectorAttributeUtils.IsAttributeProperty(property))
                {
                    ConstantValue? value = GetAttributeNamedParameter(attributeHandle, (IProperty) property.Handle);
                    if (value.HasValue)
                        yield return new KeyValuePair<StaticPropertyWrapper, ConstantValue>(property, value.Value);
                }
            }
        }

        private ConstantValue? GetAttributeNamedParameter(IAttributeInstance attributeHandle, ITypeMember memberHandle)
        {
#if RESHARPER_31
            ConstantValue2 rawValue = GetAttributeNamedParameterHack(attributeHandle, memberHandle);
            return rawValue.IsBadValue() ? (ConstantValue?)null : ConvertConstantValue(rawValue.Value);
#else
            AttributeValue rawValue = attributeHandle.NamedParameter(memberHandle);
            return rawValue.IsBadValue ? (ConstantValue?) null : ConvertConstantValue(rawValue);
#endif
        }

        private ConstantValue? GetAttributePositionParameter(IAttributeInstance attributeHandle, int index)
        {
#if RESHARPER_31
            ConstantValue2 rawValue = GetAttributePositionParameterHack(attributeHandle, index);
            return rawValue.IsBadValue() ? (ConstantValue?)null : ConvertConstantValue(rawValue.Value);
#else
            AttributeValue rawValue = attributeHandle.PositionParameter(index);
            return rawValue.IsBadValue ? (ConstantValue?) null : ConvertConstantValue(rawValue);
#endif
        }

#if RESHARPER_31
        private ConstantValue ConvertConstantValue(object value)
        {
            return ConvertConstantValue<IType>(value, delegate(IType type) { return MakeType(type); });
        }
#else
        private ConstantValue ConvertConstantValue(AttributeValue value)
        {
            if (value.IsConstant)
                return new ConstantValue(MakeType(value.ConstantValue.Type), value.ConstantValue.Value);

            if (value.IsType)
                return new ConstantValue(Reflector.Wrap(typeof(Type)), MakeType(value.TypeValue));

            if (value.IsArray)
                return new ConstantValue(MakeType(value.ArrayType),
                    GenericCollectionUtils.ConvertAllToArray<AttributeValue, ConstantValue>(value.ArrayValue, ConvertConstantValue));

            throw new ReflectionResolveException("Unsupported attribute value type.");
        }
#endif
        #endregion

        #region Members
        protected override IEnumerable<StaticAttributeWrapper> GetMemberCustomAttributes(StaticMemberWrapper member)
        {
            IAttributesOwner memberHandle = (IAttributesOwner)member.Handle;
            return GetAttributesForAttributeOwner(memberHandle);
        }

        protected override string GetMemberName(StaticMemberWrapper member)
        {
            IDeclaredElement memberHandle = (IDeclaredElement)member.Handle;
            string shortName = memberHandle.ShortName;

            if (shortName == "get_this")
                shortName = "get_Item";
            else if (shortName == "set_this")
                shortName = "set_Item";

            IOverridableMember overridableMemberHandle = memberHandle as IOverridableMember;
            if (overridableMemberHandle != null && overridableMemberHandle.IsExplicitImplementation)
                return overridableMemberHandle.ExplicitImplementations[0].DeclaringType.GetCLRName().Replace('+', '.') + "." + shortName;

            return shortName;
        }

        protected override CodeLocation GetMemberSourceLocation(StaticMemberWrapper member)
        {
            IDeclaredElement memberHandle = (IDeclaredElement)member.Handle;
            return GetDeclaredElementSourceLocation(memberHandle);
        }
        #endregion

        #region Events
        protected override EventAttributes GetEventAttributes(StaticEventWrapper @event)
        {
            return EventAttributes.None;
        }

        protected override StaticMethodWrapper GetEventAddMethod(StaticEventWrapper @event)
        {
            IEvent eventHandle = (IEvent)@event.Handle;
            if (!eventHandle.IsValid())
                return null;

            IAccessor accessorHandle = eventHandle.Adder;
            return accessorHandle != null && accessorHandle.IsValid() ? WrapAccessor(accessorHandle, @event) : null;
        }

        protected override StaticMethodWrapper GetEventRaiseMethod(StaticEventWrapper @event)
        {
            IEvent eventHandle = (IEvent)@event.Handle;
            if (!eventHandle.IsValid())
                return null;

            IAccessor accessorHandle = eventHandle.Raiser;
            return accessorHandle != null && accessorHandle.IsValid() ? WrapAccessor(accessorHandle, @event) : null;
        }

        protected override StaticMethodWrapper GetEventRemoveMethod(StaticEventWrapper @event)
        {
            IEvent eventHandle = (IEvent)@event.Handle;
            if (!eventHandle.IsValid())
                return null;

            IAccessor accessorHandle = eventHandle.Remover;
            return accessorHandle != null && accessorHandle.IsValid() ? WrapAccessor(accessorHandle, @event) : null;
        }

        protected override StaticTypeWrapper GetEventHandlerType(StaticEventWrapper @event)
        {
            IEvent eventHandle = (IEvent)@event.Handle;
            return MakeType(eventHandle.Type);
        }
        #endregion

        #region Fields
        protected override FieldAttributes GetFieldAttributes(StaticFieldWrapper field)
        {
            IField fieldHandle = (IField)field.Handle;

            FieldAttributes flags = 0;
            switch (fieldHandle.GetAccessRights())
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

            ReflectorFlagsUtils.AddFlagIfTrue(ref flags, FieldAttributes.Static, fieldHandle.IsStatic);
            ReflectorFlagsUtils.AddFlagIfTrue(ref flags, FieldAttributes.InitOnly, fieldHandle.IsReadonly && ! fieldHandle.IsConstant);
            ReflectorFlagsUtils.AddFlagIfTrue(ref flags, FieldAttributes.Literal | FieldAttributes.HasDefault, fieldHandle.IsConstant);
            return flags;
        }

        protected override StaticTypeWrapper GetFieldType(StaticFieldWrapper field)
        {
            IField fieldHandle = (IField)field.Handle;
            return MakeType(fieldHandle.Type);
        }
        #endregion

        #region Properties
        protected override PropertyAttributes GetPropertyAttributes(StaticPropertyWrapper property)
        {
            // Note: There don't seem to be any usable property attributes.
            return 0;
        }

        protected override StaticTypeWrapper GetPropertyType(StaticPropertyWrapper property)
        {
            IProperty propertyHandle = (IProperty)property.Handle;
            return MakeType(propertyHandle.Type);
        }

        protected override StaticMethodWrapper GetPropertyGetMethod(StaticPropertyWrapper property)
        {
            IProperty propertyHandle = (IProperty)property.Handle;
            if (!propertyHandle.IsValid())
                return null;

#if RESHARPER_31
            IAccessor accessorHandle = propertyHandle.Getter(false);
#else
            IAccessor accessorHandle = propertyHandle.Getter;
#endif
            return accessorHandle != null && accessorHandle.IsValid() ? WrapAccessor(accessorHandle, property) : null;
        }

        protected override StaticMethodWrapper GetPropertySetMethod(StaticPropertyWrapper property)
        {
            IProperty propertyHandle = (IProperty)property.Handle;
            if (!propertyHandle.IsValid())
                return null;

#if RESHARPER_31
            IAccessor accessorHandle = propertyHandle.Setter(false);
#else
            IAccessor accessorHandle = propertyHandle.Setter;
#endif
            return accessorHandle != null && accessorHandle.IsValid() ? WrapAccessor(accessorHandle, property) : null;
        }
        #endregion

        #region Functions
        protected override MethodAttributes GetFunctionAttributes(StaticFunctionWrapper function)
        {
            IFunction functionHandle = (IFunction)function.Handle;

            AccessRights accessRights = functionHandle.GetAccessRights();
            if (functionHandle is DefaultConstructor && function.DeclaringType.IsAbstract)
                accessRights = AccessRights.PROTECTED;

            MethodAttributes flags = 0;
            switch (accessRights)
            {
                case AccessRights.PUBLIC:
                    flags |= MethodAttributes.Public;
                    break;
                case AccessRights.NONE:
                case AccessRights.PRIVATE:
                    flags |= MethodAttributes.Private;
                    break;
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

            bool isVirtual = functionHandle.IsVirtual || functionHandle.IsAbstract || functionHandle.IsOverride;
            if (!isVirtual)
            {
                IOverridableMember overridableMember = functionHandle as IOverridableMember;
                if (overridableMember != null && overridableMember.IsExplicitImplementation)
                    isVirtual = true;
            }

            ReflectorFlagsUtils.AddFlagIfTrue(ref flags, MethodAttributes.Abstract, functionHandle.IsAbstract);
            ReflectorFlagsUtils.AddFlagIfTrue(ref flags, MethodAttributes.Final, isVirtual && ! CanBeOverriden(functionHandle));
            ReflectorFlagsUtils.AddFlagIfTrue(ref flags, MethodAttributes.Static, functionHandle.IsStatic);
            ReflectorFlagsUtils.AddFlagIfTrue(ref flags, MethodAttributes.Virtual, isVirtual);
            ReflectorFlagsUtils.AddFlagIfTrue(ref flags, MethodAttributes.NewSlot, isVirtual && !functionHandle.IsOverride);
            ReflectorFlagsUtils.AddFlagIfTrue(ref flags, MethodAttributes.HideBySig, true); //FIXME unreliable: functionHandle.HidePolicy == MemberHidePolicy.HIDE_BY_SIGNATURE);
            return flags;
        }

        private static bool CanBeOverriden(IFunction functionHandle)
        {
#if RESHARPER_31
            return functionHandle.CanBeOverriden;
#else
            return functionHandle.CanBeOverriden();
#endif
        }

        protected override CallingConventions GetFunctionCallingConvention(StaticFunctionWrapper function)
        {
            IFunction functionHandle = (IFunction)function.Handle;

            // FIXME: No way to determine VarArgs convention.
            CallingConventions flags = CallingConventions.Standard;
            ReflectorFlagsUtils.AddFlagIfTrue(ref flags, CallingConventions.HasThis, !functionHandle.IsStatic);
            return flags;
        }

        protected override IList<StaticParameterWrapper> GetFunctionParameters(StaticFunctionWrapper function)
        {
            IFunction functionHandle = (IFunction)function.Handle;
            if (!functionHandle.IsValid())
                return EmptyArray<StaticParameterWrapper>.Instance;

            return GenericCollectionUtils.ConvertAllToArray<IParameter, StaticParameterWrapper>(functionHandle.Parameters, delegate(IParameter parameter)
            {
                return new StaticParameterWrapper(this, parameter, function);
            });
        }
        #endregion

        #region Methods
        protected override IList<StaticGenericParameterWrapper> GetMethodGenericParameters(StaticMethodWrapper method)
        {
            IFunction methodHandle = (IFunction)method.Handle;
            if (!methodHandle.IsValid())
                return EmptyArray<StaticGenericParameterWrapper>.Instance;

            ITypeParameter[] parameterHandles = methodHandle.GetSignature(methodHandle.IdSubstitution).GetTypeParameters();

            return Array.ConvertAll<ITypeParameter, StaticGenericParameterWrapper>(parameterHandles, delegate(ITypeParameter parameter)
            {
                return StaticGenericParameterWrapper.CreateGenericMethodParameter(this, parameter, method);
            });
        }

        protected override StaticParameterWrapper GetMethodReturnParameter(StaticMethodWrapper method)
        {
            IFunction methodHandle = (IFunction)method.Handle;
            if (!methodHandle.IsValid())
                return null;

            // TODO: This won't provide access to any parameter attributes.  How should we retrieve them?
            IType type = methodHandle.ReturnType;
#if RESHARPER_31 || RESHARPER_40 || RESHARPER_41
            if (type == null || ! type.IsValid)
#else
            if (type == null || ! type.IsValid())
#endif
                return null;

#if RESHARPER_31 || RESHARPER_40 || RESHARPER_41
            var parameter = new Parameter(methodHandle, type, null);
#else
            var parameter = new Parameter(methodHandle, 0, type, null);
#endif
            return new StaticParameterWrapper(this, parameter, method);
        }
        #endregion

        #region Parameters
        protected override ParameterAttributes GetParameterAttributes(StaticParameterWrapper parameter)
        {
            IParameter parameterHandle = (IParameter)parameter.Handle;

            ParameterAttributes flags = 0;
            ReflectorFlagsUtils.AddFlagIfTrue(ref flags, ParameterAttributes.HasDefault, !parameterHandle.GetDefaultValue().IsBadValue());
            ReflectorFlagsUtils.AddFlagIfTrue(ref flags, ParameterAttributes.In, parameterHandle.Kind == ParameterKind.REFERENCE);
            ReflectorFlagsUtils.AddFlagIfTrue(ref flags, ParameterAttributes.Out, parameterHandle.Kind == ParameterKind.OUTPUT || parameterHandle.Kind == ParameterKind.REFERENCE);
            ReflectorFlagsUtils.AddFlagIfTrue(ref flags, ParameterAttributes.Optional, parameterHandle.IsOptional);
            return flags;
        }

        protected override IEnumerable<StaticAttributeWrapper> GetParameterCustomAttributes(StaticParameterWrapper parameter)
        {
            IParameter parameterHandle = (IParameter)parameter.Handle;
            return GetAttributesForAttributeOwner(parameterHandle);
        }

        protected override string GetParameterName(StaticParameterWrapper parameter)
        {
            IParameter parameterHandle = (IParameter)parameter.Handle;
            return parameterHandle.ShortName;
        }

        protected override int GetParameterPosition(StaticParameterWrapper parameter)
        {
            IParameter parameterHandle = (IParameter)parameter.Handle;
            int parameterIndex = parameterHandle.ContainingParametersOwner.Parameters.IndexOf(parameterHandle);
            return parameterIndex;
        }

        protected override StaticTypeWrapper GetParameterType(StaticParameterWrapper parameter)
        {
            IParameter parameterHandle = (IParameter)parameter.Handle;
            StaticTypeWrapper parameterType = MakeType(parameterHandle.Type);

            if (parameterHandle.Kind != ParameterKind.VALUE)
                parameterType = parameterType.MakeByRefType();

            return parameterType;
        }
        #endregion

        #region Types
        protected override TypeAttributes GetTypeAttributes(StaticDeclaredTypeWrapper type)
        {
            ITypeElement typeHandle = (ITypeElement) type.Handle;

            TypeAttributes flags = 0;
            ReflectorFlagsUtils.AddFlagIfTrue(ref flags, TypeAttributes.Interface, typeHandle is IInterface);

            IModifiersOwner modifiers = typeHandle as IModifiersOwner;
            if (modifiers != null)
            {
                ReflectorFlagsUtils.AddFlagIfTrue(ref flags, TypeAttributes.Abstract, modifiers.IsAbstract);
                ReflectorFlagsUtils.AddFlagIfTrue(ref flags, TypeAttributes.Sealed, modifiers.IsSealed);

                bool isNested = typeHandle.GetContainingType() != null;

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
                        flags |= isNested ? TypeAttributes.NestedAssembly : TypeAttributes.NotPublic;
                        break;
                    case AccessRights.PROTECTED:
                        flags |= isNested ? TypeAttributes.NestedFamily : TypeAttributes.NotPublic;
                        break;
                    case AccessRights.PROTECTED_AND_INTERNAL:
                        flags |= isNested ? TypeAttributes.NestedFamANDAssem : TypeAttributes.NotPublic;
                        break;
                    case AccessRights.PROTECTED_OR_INTERNAL:
                        flags |= isNested ? TypeAttributes.NestedFamORAssem : TypeAttributes.NotPublic;
                        break;
                }
            }

            if (typeHandle is IDelegate || typeHandle is IEnum || typeHandle is IStruct)
                flags |= TypeAttributes.Sealed;

            return flags;
        }

        protected override StaticAssemblyWrapper GetTypeAssembly(StaticDeclaredTypeWrapper type)
        {
            ITypeElement typeHandle = (ITypeElement)type.Handle;
            return WrapModule(GetModule(typeHandle));
        }

        private IModule GetModule(IDeclaredElement declaredElement)
        {
#if RESHARPER_31 || RESHARPER_40 || RESHARPER_41
            return declaredElement.Module;
#else
            return declaredElement.Module.ContainingProjectModule;
#endif
        }

        protected override string GetTypeNamespace(StaticDeclaredTypeWrapper type)
        {
            ITypeElement typeHandle = (ITypeElement)type.Handle;
            if (!typeHandle.IsValid())
                return "";
            return typeHandle.GetContainingNamespace().QualifiedName;
        }

        protected override StaticDeclaredTypeWrapper GetTypeBaseType(StaticDeclaredTypeWrapper type)
        {
            ITypeElement typeHandle = (ITypeElement)type.Handle;
            if (!typeHandle.IsValid())
                return null;

            if (!(typeHandle is IInterface))
            {
                foreach (IDeclaredType superTypeHandle in SafeGetSuperTypes(typeHandle))
                {
                    IClass @class = superTypeHandle.GetTypeElement() as IClass;
                    if (@class != null)
                    {
                        StaticDeclaredTypeWrapper baseType = MakeDeclaredType(superTypeHandle);

                        // Handles an edge case where the base type is also the containing type of the original type.
                        // This can occur when the original type is nested within its own basetype.
                        // In that case, the containing type should be parameterized by the generic type parameters
                        // of the nested that apply to it.
                        int containingTypeParamCount = baseType.GenericArguments.Count;
                        if (containingTypeParamCount != 0 && IsContainingType(@class, typeHandle))
                        {
                            var containingTypeArgs = new ITypeInfo[containingTypeParamCount];
                            IList<ITypeInfo> genericArgs = type.GenericArguments;
                            for (int i = 0; i < containingTypeParamCount; i++)
                                containingTypeArgs[i] = genericArgs[i];

                            baseType = baseType.MakeGenericType(containingTypeArgs);
                        }

                        return baseType;
                    }
                }
            }

            return null;
        }

        protected override IList<StaticDeclaredTypeWrapper> GetTypeInterfaces(StaticDeclaredTypeWrapper type)
        {
            ITypeElement typeHandle = (ITypeElement)type.Handle;
            if (!typeHandle.IsValid())
                return EmptyArray<StaticDeclaredTypeWrapper>.Instance;

            List<StaticDeclaredTypeWrapper> interfaces = new List<StaticDeclaredTypeWrapper>();

            foreach (IDeclaredType superTypeHandle in SafeGetSuperTypes(typeHandle))
            {
                IInterface @interface = superTypeHandle.GetTypeElement() as IInterface;
                if (@interface != null)
                    interfaces.Add(MakeDeclaredType(superTypeHandle));
            }

            return interfaces;
        }

        private static bool IsContainingType(ITypeElement candidateContainingType, ITypeElement type)
        {
            ITypeElement containingType = type;
            for (; ; )
            {
                containingType = containingType.GetContainingType();
                if (containingType == null)
                    return false;

                if (containingType.Equals(candidateContainingType))
                    return true;
            }
        }

        /// <summary>
        /// It is possible for GetSuperTypes to return types that would form a cycle
        /// if the user typed in something like "class A : A" (even accidentally).
        /// This will cause big problems down the line so we drop supertypes with cycles.
        /// </summary>
        private static IEnumerable<IDeclaredType> SafeGetSuperTypes(ITypeElement typeElement)
        {
            if (!typeElement.IsValid())
                yield break;

            IList<IDeclaredType> superTypes = typeElement.GetSuperTypes();
            if (superTypes.Count == 0)
                yield break;

            foreach (IDeclaredType superType in typeElement.GetSuperTypes())
            {
#if RESHARPER_31 || RESHARPER_40 || RESHARPER_41
                if (superType.IsValid)
#else
                if (superType.IsValid())
#endif
                {
                    ITypeElement superTypeElement = superType.GetTypeElement();
                    if (superTypeElement.IsValid())
                    {
                        if (!HasSuperTypeCycle(superTypeElement))
                            yield return superType;
                    }
                }
            }
        }

        private static bool HasSuperTypeCycle(ITypeElement typeElement)
        {
            var visitedSet = new Gallio.Common.Collections.HashSet<ITypeElement>();
            var queue = new Queue<ITypeElement>();
            queue.Enqueue(typeElement);

            while (queue.Count > 0)
            {
                ITypeElement currentTypeElement = queue.Dequeue();
                if (visitedSet.Contains(currentTypeElement))
                    continue;

                visitedSet.Add(currentTypeElement);

                foreach (IDeclaredType superType in currentTypeElement.GetSuperTypes())
                {
                    ITypeElement superTypeElement = superType.GetTypeElement();
                    if (superTypeElement == typeElement)
                        return true;

                    queue.Enqueue(superTypeElement);
                }
            }

            return false;
        }

        protected override IList<StaticGenericParameterWrapper> GetTypeGenericParameters(StaticDeclaredTypeWrapper type)
        {
            ITypeElement typeHandle = (ITypeElement)type.Handle;
            if (!typeHandle.IsValid())
                return EmptyArray<StaticGenericParameterWrapper>.Instance;

            var genericParameters = new List<StaticGenericParameterWrapper>();
            BuildTypeGenericParameters(type, typeHandle, genericParameters);
            return genericParameters;
        }

        private void BuildTypeGenericParameters(StaticDeclaredTypeWrapper ownerType, ITypeElement typeHandle, List<StaticGenericParameterWrapper> genericParameters)
        {
            ITypeElement declaringType = typeHandle.GetContainingType();
            if (declaringType != null)
                BuildTypeGenericParameters(ownerType, declaringType, genericParameters);

            foreach (ITypeParameter parameterHandle in typeHandle.TypeParameters)
                genericParameters.Add(StaticGenericParameterWrapper.CreateGenericTypeParameter(this, parameterHandle, ownerType));
        }

        protected override IEnumerable<StaticConstructorWrapper> GetTypeConstructors(StaticDeclaredTypeWrapper type)
        {
            ITypeElement typeHandle = (ITypeElement)type.Handle;
            if (!typeHandle.IsValid())
                yield break;

            bool foundDefault = false;
            foreach (IConstructor constructorHandle in typeHandle.Constructors)
            {
                if (constructorHandle.IsValid())
                {
                    if (constructorHandle.IsDefault)
                    {
                        if (typeHandle is IStruct)
                            continue; // Note: Default constructors for structs are not visible via reflection
                        foundDefault = true;
                    }

                    yield return new StaticConstructorWrapper(this, constructorHandle, type);
                }
            }

            if (!foundDefault)
            {
                IClass classHandle = typeHandle as IClass;
                if (classHandle != null && !classHandle.IsStatic)
                    yield return new StaticConstructorWrapper(this, new DefaultConstructor(typeHandle), type);

                IDelegate delegateHandle = typeHandle as IDelegate;
                if (delegateHandle != null)
                    yield return new StaticConstructorWrapper(this, new DelegateConstructor(delegateHandle), type);
            }
        }

        protected override IEnumerable<StaticMethodWrapper> GetTypeMethods(StaticDeclaredTypeWrapper type,
            StaticDeclaredTypeWrapper reflectedType)
        {
            ITypeElement typeHandle = (ITypeElement)type.Handle;
            if (!typeHandle.IsValid())
                yield break;

            foreach (IMethod methodHandle in typeHandle.Methods)
            {
                if (methodHandle.IsValid())
                    yield return new StaticMethodWrapper(this, methodHandle, type, reflectedType, type.Substitution);
            }

            foreach (IOperator operatorHandle in typeHandle.Operators)
            {
                if (operatorHandle.IsValid())
                    yield return new StaticMethodWrapper(this, operatorHandle, type, reflectedType, type.Substitution);
            }

            foreach (StaticPropertyWrapper property in GetTypeProperties(type, reflectedType))
            {
                if (property.GetMethod != null)
                    yield return property.GetMethod;
                if (property.SetMethod != null)
                    yield return property.SetMethod;
            }

            foreach (StaticEventWrapper @event in GetTypeEvents(type, reflectedType))
            {
                if (@event.AddMethod != null)
                    yield return @event.AddMethod;
                if (@event.RemoveMethod != null)
                    yield return @event.RemoveMethod;
                if (@event.RaiseMethod != null)
                    yield return @event.RaiseMethod;
            }
        }

        protected override IEnumerable<StaticPropertyWrapper> GetTypeProperties(StaticDeclaredTypeWrapper type,
            StaticDeclaredTypeWrapper reflectedType)
        {
            ITypeElement typeHandle = (ITypeElement)type.Handle;
            if (!typeHandle.IsValid())
                yield break;

            foreach (IProperty propertyHandle in typeHandle.Properties)
            {
                if (propertyHandle.IsValid())
                    yield return new StaticPropertyWrapper(this, propertyHandle, type, reflectedType);
            }
        }

        protected override IEnumerable<StaticFieldWrapper> GetTypeFields(StaticDeclaredTypeWrapper type,
            StaticDeclaredTypeWrapper reflectedType)
        {
            ITypeElement typeHandle = (ITypeElement)type.Handle;
            if (!typeHandle.IsValid())
                yield break;

            IClass classHandle = typeHandle as IClass;
            if (classHandle != null)
            {
                foreach (IField fieldHandle in classHandle.Fields)
                {
                    if (fieldHandle.IsValid())
                        yield return new StaticFieldWrapper(this, fieldHandle, type, reflectedType);
                }

                foreach (IField fieldHandle in classHandle.Constants)
                {
                    if (fieldHandle.IsValid())
                        yield return new StaticFieldWrapper(this, fieldHandle, type, reflectedType);
                }
            }
            else
            {
                IStruct structHandle = typeHandle as IStruct;
                if (structHandle != null)
                {
                    foreach (IField fieldHandle in structHandle.Fields)
                    {
                        if (fieldHandle.IsValid())
                            yield return new StaticFieldWrapper(this, fieldHandle, type, reflectedType);
                    }

                    foreach (IField fieldHandle in structHandle.Constants)
                    {
                        if (fieldHandle.IsValid())
                            yield return new StaticFieldWrapper(this, fieldHandle, type, reflectedType);
                    }
                }
            }
        }

        protected override IEnumerable<StaticEventWrapper> GetTypeEvents(StaticDeclaredTypeWrapper type,
            StaticDeclaredTypeWrapper reflectedType)
        {
            ITypeElement typeHandle = (ITypeElement)type.Handle;
            if (!typeHandle.IsValid())
                yield break;

            foreach (IEvent eventHandle in typeHandle.Events)
            {
                if (eventHandle.IsValid())
                    yield return new StaticEventWrapper(this, eventHandle, type, reflectedType);
            }
        }

        protected override IEnumerable<StaticTypeWrapper> GetTypeNestedTypes(StaticDeclaredTypeWrapper type)
        {
            ITypeElement typeHandle = (ITypeElement)type.Handle;
            if (!typeHandle.IsValid())
                yield break;

            foreach (ITypeElement nestedTypeHandle in typeHandle.NestedTypes)
            {
                if (nestedTypeHandle.IsValid())
                    yield return new StaticDeclaredTypeWrapper(this, nestedTypeHandle, type, type.Substitution);
            }
        }

        private StaticTypeWrapper MakeType(IType typeHandle)
        {
            return typeMemoizer.Memoize(typeHandle, () =>
            {
                IDeclaredType declaredTypeHandle = typeHandle as IDeclaredType;
                if (declaredTypeHandle != null)
                    return MakeType(declaredTypeHandle);

                IArrayType arrayTypeHandle = typeHandle as IArrayType;
                if (arrayTypeHandle != null)
                    return MakeArrayType(arrayTypeHandle);

                IPointerType pointerTypeHandle = typeHandle as IPointerType;
                if (pointerTypeHandle != null)
                    return MakePointerType(pointerTypeHandle);

                throw new NotSupportedException("Unsupported type: " + typeHandle);
            });
        }

        private StaticTypeWrapper MakeType(IDeclaredType typeHandle)
        {
            ITypeParameter typeParameterHandle = typeHandle.GetTypeElement() as ITypeParameter;
            if (typeParameterHandle != null)
                return MakeGenericParameterType(typeParameterHandle);

            return MakeDeclaredType(typeHandle);
        }

        private StaticTypeWrapper MakeTypeWithoutSubstitution(ITypeElement typeElementHandle)
        {
            ITypeParameter typeParameterHandle = typeElementHandle as ITypeParameter;
            if (typeParameterHandle != null)
                return MakeGenericParameterType(typeParameterHandle);

            return MakeDeclaredTypeWithoutSubstitution(typeElementHandle);
        }

        private StaticDeclaredTypeWrapper MakeDeclaredTypeWithoutSubstitution(ITypeElement typeElementHandle)
        {
            return typeWithoutSubstitutionMemoizer.Memoize(typeElementHandle, () =>
            {
                return MakeDeclaredType(typeElementHandle, typeElementHandle.IdSubstitution);
            });
        }

        private StaticDeclaredTypeWrapper MakeDeclaredType(IDeclaredType typeHandle)
        {
            return declaredTypeMemoizer.Memoize(typeHandle, () =>
            {
                ITypeElement typeElement = typeHandle.GetTypeElement();
                if (typeElement == null)
                    throw new ReflectionResolveException(
                        String.Format(
                            "Cannot obtain type element for type '{0}' possibly because its source code is not available.",
                            typeHandle.GetCLRName()));

                return MakeDeclaredType(typeElement, typeHandle.GetSubstitution());
            });
        }

        private StaticDeclaredTypeWrapper MakeDeclaredType(ITypeElement typeElementHandle, ISubstitution substitutionHandle)
        {
            if (typeElementHandle is ITypeParameter)
                throw new ArgumentException("This method should never be called with a generic parameter as input.",
                    "typeElementHandle");

            ITypeElement declaringTypeElementHandle = typeElementHandle.GetContainingType();
            StaticDeclaredTypeWrapper type;
            if (declaringTypeElementHandle != null)
            {
                StaticDeclaredTypeWrapper declaringType = MakeDeclaredType(declaringTypeElementHandle,
                    substitutionHandle);
                type = new StaticDeclaredTypeWrapper(this, typeElementHandle, declaringType, declaringType.Substitution);
            }
            else
            {
                type = new StaticDeclaredTypeWrapper(this, typeElementHandle, null, StaticTypeSubstitution.Empty);
            }

#if ! RESHARPER_50_OR_NEWER
            var typeParameterHandles = new List<ITypeParameter>(typeElementHandle.AllTypeParameters);
#else
            var typeParameterHandles = new List<ITypeParameter>(typeElementHandle.GetAllTypeParameters());
#endif
#if RESHARPER_31 || RESHARPER_40 || RESHARPER_41
            if (substitutionHandle.IsIdempotent(typeParameterHandles))
#else
            if (substitutionHandle.IsIdempotentAll(typeParameterHandles))
#endif
            {
                return type;
            }

        ITypeInfo[] genericArguments = GenericCollectionUtils.ConvertAllToArray<ITypeParameter, ITypeInfo>(typeParameterHandles, delegate(ITypeParameter typeParameterHandle)
            {
                IType substitutedType = substitutionHandle.Apply(typeParameterHandle);
                if (substitutedType.IsUnknown)
                    return MakeGenericParameterType(typeParameterHandle);

                return MakeType(substitutedType);
            });
            return type.MakeGenericType(genericArguments);
        }

        private StaticArrayTypeWrapper MakeArrayType(IArrayType arrayTypeHandle)
        {
            return MakeType(arrayTypeHandle.ElementType).MakeArrayType(arrayTypeHandle.Rank);
        }

        private StaticPointerTypeWrapper MakePointerType(IPointerType pointerTypeHandle)
        {
            return MakeType(pointerTypeHandle.ElementType).MakePointerType();
        }

        private StaticGenericParameterWrapper MakeGenericParameterType(ITypeParameter typeParameterHandle)
        {
            ITypeElement declaringTypeHandle = typeParameterHandle.OwnerType;
            if (declaringTypeHandle != null)
            {
                return StaticGenericParameterWrapper.CreateGenericTypeParameter(this, typeParameterHandle, MakeDeclaredTypeWithoutSubstitution(declaringTypeHandle));
            }
            else
            {
                return StaticGenericParameterWrapper.CreateGenericMethodParameter(this, typeParameterHandle, Wrap(typeParameterHandle.OwnerMethod));
            }
        }
        #endregion

        #region Generic Parameters
        protected override GenericParameterAttributes GetGenericParameterAttributes(StaticGenericParameterWrapper genericParameter)
        {
            ITypeParameter genericParameterHandle = (ITypeParameter)genericParameter.Handle;

            GenericParameterAttributes flags = 0;
            ReflectorFlagsUtils.AddFlagIfTrue(ref flags, GenericParameterAttributes.NotNullableValueTypeConstraint, genericParameterHandle.IsValueType);
            ReflectorFlagsUtils.AddFlagIfTrue(ref flags, GenericParameterAttributes.ReferenceTypeConstraint, genericParameterHandle.IsClassType);
            ReflectorFlagsUtils.AddFlagIfTrue(ref flags, GenericParameterAttributes.DefaultConstructorConstraint, genericParameterHandle.HasDefaultConstructor);
            return flags;
        }

        protected override int GetGenericParameterPosition(StaticGenericParameterWrapper genericParameter)
        {
            ITypeParameter genericParameterHandle = (ITypeParameter)genericParameter.Handle;
            int parameterIndex = genericParameterHandle.Index;

            // Must also factor in generic parameters of declaring types.
            for (ITypeElement declaringType = genericParameterHandle.OwnerType; declaringType != null; )
            {
                declaringType = declaringType.GetContainingType();
                if (declaringType == null)
                    break;

                parameterIndex += declaringType.TypeParameters.Length;
            }

            return parameterIndex;
        }

        protected override IList<StaticTypeWrapper> GetGenericParameterConstraints(StaticGenericParameterWrapper genericParameter)
        {
            ITypeParameter genericParameterHandle = (ITypeParameter)genericParameter.Handle;
            return GenericCollectionUtils.ConvertAllToArray<IType, StaticTypeWrapper>(genericParameterHandle.TypeConstraints, MakeType);
        }
        #endregion

        #region GetDeclaredElementResolver and GetProject
        protected override IDeclaredElementResolver GetDeclaredElementResolver(StaticWrapper element)
        {
            IDeclaredElement declaredElement = GetDeclaredElement(element);
            return new DeclaredElementResolver(declaredElement);
        }

        protected override IProject GetProject(StaticWrapper element)
        {
            IProject project = element.Handle as IProject;
            if (project != null)
                return project;

            IDeclaredElement declaredElement = GetDeclaredElement(element);
            return declaredElement != null && declaredElement.IsValid() ? GetModule(declaredElement) as IProject : null;
        }

        private IDeclaredElement GetDeclaredElement(StaticWrapper element)
        {
            return element.Handle as IDeclaredElement;
        }

        private sealed class DeclaredElementResolver : IDeclaredElementResolver
        {
            private readonly IDeclaredElement declaredElement;

            public DeclaredElementResolver(IDeclaredElement declaredElement)
            {
                this.declaredElement = declaredElement;
            }

            public IDeclaredElement ResolveDeclaredElement()
            {
                return declaredElement;
            }
        }
        #endregion

        #region Misc
        private IEnumerable<StaticAttributeWrapper> GetAttributesForAttributeOwner(IAttributesOwner owner)
        {
            if (!owner.IsValid())
                yield break;

            foreach (IAttributeInstance attribute in owner.GetAttributeInstances(false))
            {
                if (IsAttributeInstanceValid(attribute))
                    yield return new StaticAttributeWrapper(this, attribute);
            }
        }

        private StaticMethodWrapper WrapAccessor(IAccessor accessorHandle, StaticMemberWrapper member)
        {
            return accessorHandle != null ? new StaticMethodWrapper(this, accessorHandle, member.DeclaringType, member.ReflectedType, member.Substitution) : null;
        }
        #endregion

        #region HACKS
#if RESHARPER_31
        private static FieldInfo CSharpAttributeInstanceMyAttributeField;

        private ConstantValue2 GetAttributePositionParameterHack(IAttributeInstance attributeInstance, int index)
        {
            IAttribute attribute = GetCSharpAttributeHack(attributeInstance);
            if (attribute != null)
            {
                IList<ICSharpArgument> arguments = attribute.Arguments;
                if (index >= arguments.Count)
                    return ConstantValue2.BAD_VALUE;

                ICSharpExpression expression = arguments[index].Value;

                IList<IParameter> parameters = attributeInstance.Constructor.Parameters;
                int lastParameterIndex = parameters.Count - 1;
                if (index >= lastParameterIndex && parameters[lastParameterIndex].IsParameterArray)
                    return GetCSharpConstantValueHack(expression, ((IArrayType)parameters[lastParameterIndex].Type).ElementType);

                return GetCSharpConstantValueHack(arguments[index].Value, parameters[index].Type);
            }

            return attributeInstance.PositionParameter(index);
        }

        private ConstantValue2 GetAttributeNamedParameterHack(IAttributeInstance attributeInstance, ITypeMember typeMember)
        {
            IAttribute attribute = GetCSharpAttributeHack(attributeInstance);
            if (attribute != null)
            {
                foreach (IPropertyAssignmentNode propertyAssignmentNode in attribute.ToTreeNode().PropertyAssignments)
                {
                    IPropertyAssignment propertyAssignment = propertyAssignmentNode;
                    if (propertyAssignment.Reference.Resolve().DeclaredElement == typeMember)
                    {
                        IType propertyType = ((ITypeOwner)typeMember).Type;
                        ICSharpExpression expression = propertyAssignment.Source;
                        return GetCSharpConstantValueHack(expression, propertyType);
                    }
                }

                return ConstantValue2.BAD_VALUE;
            }

            return attributeInstance.NamedParameter(typeMember);
        }

        private IAttribute GetCSharpAttributeHack(IAttributeInstance attributeInstance)
        {
            if (attributeInstance.GetType().Name != "CSharpAttributeInstance")
                return null;

            if (CSharpAttributeInstanceMyAttributeField == null)
                CSharpAttributeInstanceMyAttributeField = attributeInstance.GetType().GetField("myAttribute",
                    BindingFlags.Instance | BindingFlags.NonPublic);

            if (CSharpAttributeInstanceMyAttributeField == null)
                return null;

            return (IAttribute) CSharpAttributeInstanceMyAttributeField.GetValue(attributeInstance);
        }

        private ConstantValue2 GetCSharpConstantValueHack(ICSharpExpression expression, IType type)
        {
            if (expression == null)
                return ConstantValue2.NOT_COMPILE_TIME_CONSTANT;

            IArrayCreationExpression arrayExpression = expression as IArrayCreationExpression;
            if (arrayExpression != null)
            {
                int[] dimensions = arrayExpression.Dimensions;
                int rank = dimensions.Length;
                for (int i = 0; i < rank; i++)
                    if (dimensions[i] != 1)
                        return ConstantValue2.NOT_COMPILE_TIME_CONSTANT;

                IArrayType arrayType = arrayExpression.Type() as IArrayType;
                if (arrayType == null)
                    return ConstantValue2.NOT_COMPILE_TIME_CONSTANT;

                IArrayInitializer arrayInitializer = arrayExpression.Initializer;
                if (arrayInitializer == null)
                    return ConstantValue2.NOT_COMPILE_TIME_CONSTANT;
                
                IType elementType = arrayType.ElementType;

                Type resolvedScalarType = MakeType(arrayType.GetScalarType()).Resolve(true);
                if (resolvedScalarType == typeof(Type))
                    resolvedScalarType = typeof(IType);

                Type resolvedElementType = rank == 1 ? resolvedScalarType : rank == 2 ? resolvedScalarType.MakeArrayType() : resolvedScalarType.MakeArrayType(rank - 1);

                IList<IVariableInitializer> elementInitializers = arrayInitializer.ElementInitializers;
                int length = elementInitializers.Count;
                Array array = Array.CreateInstance(resolvedElementType, length);

                for (int i = 0; i < length; i++)
                {
                    IExpressionInitializer initializer = elementInitializers[i] as IExpressionInitializer;
                    if (initializer == null)
                        return ConstantValue2.NOT_COMPILE_TIME_CONSTANT;

                    ConstantValue2 elementValue = GetCSharpConstantValueHack(initializer.Value, elementType);
                    if (elementValue.IsBadValue())
                        return ConstantValue2.NOT_COMPILE_TIME_CONSTANT;

                    array.SetValue(elementValue.Value, i);
                }

                return new ConstantValue2(array, arrayType, type.GetManager().Solution);
            }

            ITypeofExpression typeExpression = expression as ITypeofExpression;
            if (typeExpression != null)
            {
                return new ConstantValue2(typeExpression.ArgumentType, type, type.GetManager().Solution);
            }

            return expression.ConstantCalculator.ToTypeImplicit(expression.CompileTimeConstantValue(), type);
        }
#endif
        #endregion

        private bool IsAttributeInstanceValid(IAttributeInstance attrib)
        {
            return attrib.Constructor != null && attrib.AttributeType != null;
        }
    }
}