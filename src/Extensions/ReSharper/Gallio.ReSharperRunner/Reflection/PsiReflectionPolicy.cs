// Copyright 2005-2008 Gallio Project - http://www.gallio.org/
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
using Gallio.Reflection;
using Gallio.Reflection.Impl;
using JetBrains.ProjectModel;
using JetBrains.ProjectModel.Build;
using JetBrains.ReSharper.Editor;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Caches;
using Gallio.Collections;
using JetBrains.ReSharper.Psi.Impl.Special;
using JetBrains.ReSharper.Psi.Resolve;
using JetBrains.ReSharper.Psi.Tree;
using ReSharperDocumentRange = JetBrains.ReSharper.Editor.DocumentRange;

namespace Gallio.ReSharperRunner.Reflection
{
    /// <summary>
    /// Wraps ReSharper code model objects using the reflection adapter interfaces.
    /// </summary>
    public class PsiReflectionPolicy : ReSharperReflectionPolicy
    {
        private readonly PsiManager psiManager;

        /// <summary>
        /// Creates a reflector with the specified PSI manager.
        /// </summary>
        /// <param name="psiManager">The PSI manager</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="psiManager"/> is null</exception>
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
        /// <param name="target">The element, or null if none</param>
        /// <returns>The reflection wrapper, or null if none</returns>
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

            throw new NotSupportedException("Unsupported declared element type: " + target);
        }

        /// <summary>
        /// Obtains a reflection wrapper for a type.
        /// </summary>
        /// <param name="target">The type, or null if none</param>
        /// <returns>The reflection wrapper, or null if none</returns>
        public StaticTypeWrapper Wrap(ITypeElement target)
        {
            return target != null ? MakeTypeWithoutSubstitution(target) : null;
        }

        /// <summary>
        /// Obtains a reflection wrapper for a function.
        /// </summary>
        /// <param name="target">The function, or null if none</param>
        /// <returns>The reflection wrapper, or null if none</returns>
        public StaticFunctionWrapper Wrap(IFunction target)
        {
            if (target == null)
                return null;

            IConstructor constructor = target as IConstructor;
            return constructor != null ? (StaticFunctionWrapper) Wrap(constructor) : Wrap((IMethod) target);
        }

        /// <summary>
        /// Obtains a reflection wrapper for a constructor.
        /// </summary>
        /// <param name="target">The constructor, or null if none</param>
        /// <returns>The reflection wrapper, or null if none</returns>
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
        /// <param name="target">The method, or null if none</param>
        /// <returns>The reflection wrapper, or null if none</returns>
        public StaticMethodWrapper Wrap(IMethod target)
        {
            if (target == null)
                return null;

            StaticDeclaredTypeWrapper declaringType = MakeDeclaredTypeWithoutSubstitution(target.GetContainingType());
            return new StaticMethodWrapper(this, target, declaringType, declaringType.Substitution);
        }

        /// <summary>
        /// Obtains a reflection wrapper for a property.
        /// </summary>
        /// <param name="target">The property, or null if none</param>
        /// <returns>The reflection wrapper, or null if none</returns>
        public StaticPropertyWrapper Wrap(IProperty target)
        {
            if (target == null)
                return null;

            StaticDeclaredTypeWrapper declaringType = MakeDeclaredTypeWithoutSubstitution(target.GetContainingType());
            return new StaticPropertyWrapper(this, target, declaringType);
        }

        /// <summary>
        /// Obtains a reflection wrapper for a field.
        /// </summary>
        /// <param name="target">The field, or null if none</param>
        /// <returns>The reflection wrapper, or null if none</returns>
        public StaticFieldWrapper Wrap(IField target)
        {
            if (target == null)
                return null;

            StaticDeclaredTypeWrapper declaringType = MakeDeclaredTypeWithoutSubstitution(target.GetContainingType());
            return new StaticFieldWrapper(this, target, declaringType);
        }

        /// <summary>
        /// Obtains a reflection wrapper for an event.
        /// </summary>
        /// <param name="target">The event, or null if none</param>
        /// <returns>The reflection wrapper, or null if none</returns>
        public StaticEventWrapper Wrap(IEvent target)
        {
            if (target == null)
                return null;

            StaticDeclaredTypeWrapper declaringType = MakeDeclaredTypeWithoutSubstitution(target.GetContainingType());
            return new StaticEventWrapper(this, target, declaringType);
        }

        /// <summary>
        /// Obtains a reflection wrapper for a parameter.
        /// </summary>
        /// <param name="target">The parameter, or null if none</param>
        /// <returns>The reflection wrapper, or null if none</returns>
        public StaticParameterWrapper Wrap(IParameter target)
        {
            if (target == null)
                return null;

            StaticMemberWrapper member = Wrap((IFunction)target.ContainingParametersOwner);
            return new StaticParameterWrapper(this, target, member);
        }
        #endregion

        #region Assemblies
        protected override StaticAssemblyWrapper LoadAssemblyInternal(AssemblyName assemblyName)
        {
            foreach (IProject project in psiManager.Solution.GetAllProjects())
            {
                IAssemblyFile assemblyFile = BuildSettingsManager.GetInstance(project).GetOutputAssemblyFile();

                if (assemblyFile != null && IsMatchingAssemblyName(assemblyName, assemblyFile.AssemblyName))
                    return new StaticAssemblyWrapper(this, project);
            }

            foreach (IAssembly assembly in psiManager.Solution.GetAllAssemblies())
            {
                if (IsMatchingAssemblyName(assemblyName, assembly.AssemblyName))
                    return new StaticAssemblyWrapper(this, assembly);
            }

            throw new ArgumentException(String.Format("Could not find assembly '{0}' in the ReSharper code cache.",
                assemblyName.FullName));
        }

        protected override IEnumerable<StaticAttributeWrapper> GetAssemblyCustomAttributes(StaticAssemblyWrapper assembly)
        {
            IModule moduleHandle = (IModule) assembly.Handle;
            foreach (IAttributeInstance attrib in psiManager.GetModuleAttributes(moduleHandle).AttributeInstances)
                yield return new StaticAttributeWrapper(this, attrib);
        }

        protected override AssemblyName GetAssemblyName(StaticAssemblyWrapper assembly)
        {
            IModule moduleHandle = (IModule) assembly.Handle;
            return GetAssemblyName(moduleHandle);
        }

        protected override string GetAssemblyPath(StaticAssemblyWrapper assembly)
        {
            IModule moduleHandle = (IModule)assembly.Handle;
            return GetAssemblyFile(moduleHandle).Location.FullPath;
        }

        protected override IList<AssemblyName> GetAssemblyReferences(StaticAssemblyWrapper assembly)
        {
            IProject projectHandle = assembly.Handle as IProject;
            if (projectHandle != null)
            {
                ICollection<IModuleReference> moduleRefs = projectHandle.GetModuleReferences();
                return GenericUtils.ConvertAllToArray<IModuleReference, AssemblyName>(moduleRefs, delegate(IModuleReference moduleRef)
                {
                    return GetAssemblyName(moduleRef.ResolveReferencedModule());
                });
            }

            // FIXME! Don't know how to handle referenced assemblies for modules.
            return assembly.Resolve().GetReferencedAssemblies();
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
            ITypeElement typeHandle = GetAssemblyDeclarationsCache(moduleHandle).GetTypeElementByCLRName(typeName);
            return typeHandle != null ? MakeDeclaredTypeWithoutSubstitution(typeHandle) : null;
        }

        private static bool IsMatchingAssemblyName(AssemblyName desiredAssemblyName, AssemblyName candidateAssemblyName)
        {
            bool haveDesiredFullName = desiredAssemblyName.Name != desiredAssemblyName.FullName;
            bool haveCandidateFullName = candidateAssemblyName.Name != candidateAssemblyName.FullName;

            if (haveDesiredFullName && haveCandidateFullName)
                return desiredAssemblyName.FullName == candidateAssemblyName.FullName;

            return desiredAssemblyName.Name == candidateAssemblyName.Name;
        }

        private static IAssemblyFile GetAssemblyFile(IModule moduleHandle)
        {
            IProject projectHandle = moduleHandle as IProject;
            if (projectHandle != null)
                return GetAssemblyFile(projectHandle);

            IAssembly assemblyHandle = (IAssembly) moduleHandle;
            IAssemblyFile[] files = assemblyHandle.GetFiles();
            return files[0];
        }

        private static IAssemblyFile GetAssemblyFile(IProject projectHandle)
        {
            return BuildSettingsManager.GetInstance(projectHandle).GetOutputAssemblyFile();
        }

        private static AssemblyName GetAssemblyName(IModule moduleHandle)
        {
            IProject projectHandle = moduleHandle as IProject;
            if (projectHandle != null)
            {
                AssemblyName name = (AssemblyName)GetAssemblyFile(projectHandle).AssemblyName.Clone();
                name.Version = new Version(0, 0, 0, 0);
                return name;
            }

            IAssembly assemblyHandle = (IAssembly) moduleHandle;
            return assemblyHandle.AssemblyName;
        }

        private IList<StaticDeclaredTypeWrapper> GetAssemblyTypes(IModule moduleHandle, bool includeNonPublicTypes)
        {
            INamespace namespaceHandle = psiManager.GetNamespace("");
            IDeclarationsCache cache = GetAssemblyDeclarationsCache(moduleHandle);

            List<StaticDeclaredTypeWrapper> types = new List<StaticDeclaredTypeWrapper>();
            PopulateAssemblyTypes(types, namespaceHandle, cache, includeNonPublicTypes);

            return types;
        }

        private void PopulateAssemblyTypes(List<StaticDeclaredTypeWrapper> types, INamespace namespaceHandle,
            IDeclarationsCache cache, bool includeNonPublicTypes)
        {
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
            IProject projectHandle = moduleHandle as IProject;
            if (projectHandle != null)
                return psiManager.GetDeclarationsCache(DeclarationsCacheScope.ProjectScope(projectHandle, false), true);

            IAssembly assemblyHandle = (IAssembly) moduleHandle;
            return psiManager.GetDeclarationsCache(DeclarationsCacheScope.LibraryScope(assemblyHandle, false), true);
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

        protected override object[] GetAttributeConstructorArguments(StaticAttributeWrapper attribute)
        {
            IAttributeInstance attributeHandle = (IAttributeInstance)attribute.Handle;

            IList<IParameter> parameters = attributeHandle.Constructor.Parameters;
            if (parameters.Count == 0)
                return EmptyArray<object>.Instance;

            List<object> values = new List<object>();
            for (int i = 0; ; i++)
            {
                ConstantValue2 rawValue = attributeHandle.PositionParameter(i);
                if (rawValue.IsBadValue())
                    break;

                values.Add(ResolveAttributeValue(rawValue.Value));
            }

            int lastParameterIndex = parameters.Count - 1;
            IParameter lastParameter = parameters[lastParameterIndex];
            if (!lastParameter.IsParameterArray)
                return values.ToArray();

            // Note: When presented with a constructor that accepts a variable number of
            //       arguments, ReSharper treats them as a sequence of normal parameter
            //       values.  So we we need to map them back into a params array appropriately.                
            object[] args = new object[parameters.Count];
            values.CopyTo(0, args, 0, lastParameterIndex);

            Type lastParameterType = MakeType(lastParameter.Type).Resolve(true).GetElementType();
            int varArgsCount = values.Count - lastParameterIndex;
            Array varArgs = Array.CreateInstance(lastParameterType, varArgsCount);

            for (int i = 0; i < varArgsCount; i++)
                varArgs.SetValue(values[lastParameterIndex + i], i);

            args[lastParameterIndex] = varArgs;
            return args;
        }

        protected override IEnumerable<KeyValuePair<StaticFieldWrapper, object>> GetAttributeFieldArguments(
            StaticAttributeWrapper attribute)
        {
            IAttributeInstance attributeHandle = (IAttributeInstance)attribute.Handle;
            List<KeyValuePair<StaticFieldWrapper, object>> values = new List<KeyValuePair<StaticFieldWrapper, object>>();

            foreach (StaticFieldWrapper field in attribute.Type.GetFields(BindingFlags.Public | BindingFlags.Instance))
            {
                if (ReflectorAttributeUtils.IsAttributeField(field))
                {
                    IField fieldHandle = (IField)field.Handle;
                    ConstantValue2 value = attributeHandle.NamedParameter(fieldHandle);
                    if (!value.IsBadValue())
                        values.Add(new KeyValuePair<StaticFieldWrapper, object>(field, ResolveAttributeValue(value.Value)));
                }
            }

            return values;
        }

        protected override IEnumerable<KeyValuePair<StaticPropertyWrapper, object>> GetAttributePropertyArguments(
            StaticAttributeWrapper attribute)
        {
            IAttributeInstance attributeHandle = (IAttributeInstance)attribute.Handle;
            List<KeyValuePair<StaticPropertyWrapper, object>> values = new List<KeyValuePair<StaticPropertyWrapper, object>>();

            foreach (StaticPropertyWrapper property in attribute.Type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                if (ReflectorAttributeUtils.IsAttributeProperty(property))
                {
                    IProperty propertyHandle = (IProperty)property.Handle;
                    ConstantValue2 value = attributeHandle.NamedParameter(propertyHandle);
                    if (!value.IsBadValue())
                        values.Add(new KeyValuePair<StaticPropertyWrapper, object>(property, ResolveAttributeValue(value.Value)));
                }
            }

            return values;
        }

        private object ResolveAttributeValue(object value)
        {
            if (value != null)
            {
                IType type = value as IType;
                if (type != null)
                    return MakeType(type).Resolve(false);

                // TODO: It's not clear to me that the PSI internal implementation is complete!
                //       I found a special case for mapping types but nothing for arrays.
                //       So I've omitted the array code from here for now.  -- Jeff.
            }

            return value;
        }
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
            IDeclaration[] decl = memberHandle.GetDeclarations();
            if (decl.Length == 0)
                return null;

            ReSharperDocumentRange range = decl[0].GetDocumentRange();
            if (!range.IsValid)
                return null;

            string filename = decl[0].GetProjectFile().Location.FullPath;
            DocumentCoords start = range.Document.GetCoordsByOffset(range.TextRange.StartOffset);

            return new CodeLocation(filename, start.Line, start.Column);
        }
        #endregion

        #region Events
        protected override EventAttributes GetEventAttributes(StaticEventWrapper @event)
        {
            IEvent eventHandle = (IEvent)@event.Handle;
            return EventAttributes.None;
        }

        protected override StaticMethodWrapper GetEventAddMethod(StaticEventWrapper @event)
        {
            IEvent eventHandle = (IEvent)@event.Handle;
            return WrapAccessor(eventHandle.Adder, @event);
        }

        protected override StaticMethodWrapper GetEventRaiseMethod(StaticEventWrapper @event)
        {
            IEvent eventHandle = (IEvent)@event.Handle;
            return WrapAccessor(eventHandle.Raiser, @event);
        }

        protected override StaticMethodWrapper GetEventRemoveMethod(StaticEventWrapper @event)
        {
            IEvent eventHandle = (IEvent)@event.Handle;
            return WrapAccessor(eventHandle.Remover, @event);
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
            return WrapAccessor(propertyHandle.Getter(false), property);
        }

        protected override StaticMethodWrapper GetPropertySetMethod(StaticPropertyWrapper property)
        {
            IProperty propertyHandle = (IProperty)property.Handle;
            return WrapAccessor(propertyHandle.Setter(false), property);
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
            ReflectorFlagsUtils.AddFlagIfTrue(ref flags, MethodAttributes.Final, isVirtual && ! functionHandle.CanBeOverriden);
            ReflectorFlagsUtils.AddFlagIfTrue(ref flags, MethodAttributes.Static, functionHandle.IsStatic);
            ReflectorFlagsUtils.AddFlagIfTrue(ref flags, MethodAttributes.Virtual, isVirtual);
            ReflectorFlagsUtils.AddFlagIfTrue(ref flags, MethodAttributes.NewSlot, isVirtual && !functionHandle.IsOverride);
            ReflectorFlagsUtils.AddFlagIfTrue(ref flags, MethodAttributes.HideBySig, true); //FIXME unreliable: functionHandle.HidePolicy == MemberHidePolicy.HIDE_BY_SIGNATURE);
            return flags;
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

            return GenericUtils.ConvertAllToArray<IParameter, StaticParameterWrapper>(functionHandle.Parameters, delegate(IParameter parameter)
            {
                return new StaticParameterWrapper(this, parameter, function);
            });
        }
        #endregion

        #region Methods
        protected override IList<StaticGenericParameterWrapper> GetMethodGenericParameters(StaticMethodWrapper method)
        {
            IFunction methodHandle = (IFunction)method.Handle;
            ITypeParameter[] parameterHandles = methodHandle.GetSignature(methodHandle.IdSubstitution).GetTypeParameters();

            return Array.ConvertAll<ITypeParameter, StaticGenericParameterWrapper>(parameterHandles, delegate(ITypeParameter parameter)
            {
                return StaticGenericParameterWrapper.CreateGenericMethodParameter(this, parameter, method);
            });
        }

        protected override StaticParameterWrapper GetMethodReturnParameter(StaticMethodWrapper method)
        {
            IFunction methodHandle = (IFunction)method.Handle;

            // TODO: This won't provide access to any parameter attributes.  How should we retrieve them?
            IType type = methodHandle.ReturnType;
            return type != null ? new StaticParameterWrapper(this, new Parameter(methodHandle, type, null), method) : null;
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
            return parameterHandle.ContainingParametersOwner.Parameters.IndexOf(parameterHandle);
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
            return new StaticAssemblyWrapper(this, typeHandle.Module);
        }

        protected override string GetTypeNamespace(StaticDeclaredTypeWrapper type)
        {
            ITypeElement typeHandle = (ITypeElement)type.Handle;
            return typeHandle.GetContainingNamespace().QualifiedName;
        }

        protected override StaticDeclaredTypeWrapper GetTypeBaseType(StaticDeclaredTypeWrapper type)
        {
            ITypeElement typeHandle = (ITypeElement)type.Handle;

            if (!(typeHandle is IInterface))
            {
                foreach (IDeclaredType superTypeHandle in typeHandle.GetSuperTypes())
                {
                    IClass @class = superTypeHandle.GetTypeElement() as IClass;
                    if (@class != null)
                        return MakeDeclaredType(superTypeHandle);
                }
            }

            return null;
        }

        protected override IList<StaticDeclaredTypeWrapper> GetTypeInterfaces(StaticDeclaredTypeWrapper type)
        {
            ITypeElement typeHandle = (ITypeElement)type.Handle;
            List<StaticDeclaredTypeWrapper> interfaces = new List<StaticDeclaredTypeWrapper>();

            foreach (IDeclaredType superType in typeHandle.GetSuperTypes())
            {
                IInterface @interface = superType.GetTypeElement() as IInterface;
                if (@interface != null)
                    interfaces.Add(MakeDeclaredType(superType));
            }

            return interfaces;
        }

        protected override IList<StaticGenericParameterWrapper> GetTypeGenericParameters(StaticDeclaredTypeWrapper type)
        {
            ITypeElement typeHandle = (ITypeElement)type.Handle;
            return Array.ConvertAll<ITypeParameter, StaticGenericParameterWrapper>(typeHandle.TypeParameters, delegate(ITypeParameter parameterHandle)
            {
                return StaticGenericParameterWrapper.CreateGenericTypeParameter(this, parameterHandle, type);
            });
        }

        protected override IEnumerable<StaticConstructorWrapper> GetTypeConstructors(StaticDeclaredTypeWrapper type)
        {
            ITypeElement typeHandle = (ITypeElement)type.Handle;

            bool foundDefault = false;
            foreach (IConstructor constructorHandle in typeHandle.Constructors)
            {
                if (constructorHandle.IsDefault)
                {
                    if (typeHandle is IStruct)
                        continue; // Note: Default constructors for structs are not visible via reflection
                    foundDefault = true;
                }

                yield return new StaticConstructorWrapper(this, constructorHandle, type);
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

        protected override IEnumerable<StaticMethodWrapper> GetTypeMethods(StaticDeclaredTypeWrapper type)
        {
            ITypeElement typeHandle = (ITypeElement)type.Handle;

            foreach (IMethod methodHandle in typeHandle.Methods)
                yield return new StaticMethodWrapper(this, methodHandle, type, type.Substitution);

            foreach (IOperator operatorHandle in typeHandle.Operators)
                yield return new StaticMethodWrapper(this, operatorHandle, type, type.Substitution);

            foreach (StaticPropertyWrapper property in GetTypeProperties(type))
            {
                if (property.GetMethod != null)
                    yield return property.GetMethod;
                if (property.SetMethod != null)
                    yield return property.SetMethod;
            }

            foreach (StaticEventWrapper @event in GetTypeEvents(type))
            {
                if (@event.AddMethod != null)
                    yield return @event.AddMethod;
                if (@event.RemoveMethod != null)
                    yield return @event.RemoveMethod;
                if (@event.RaiseMethod != null)
                    yield return @event.RaiseMethod;
            }
        }

        protected override IEnumerable<StaticPropertyWrapper> GetTypeProperties(StaticDeclaredTypeWrapper type)
        {
            ITypeElement typeHandle = (ITypeElement)type.Handle;

            foreach (IProperty propertyHandle in typeHandle.Properties)
                yield return new StaticPropertyWrapper(this, propertyHandle, type);
        }

        protected override IEnumerable<StaticFieldWrapper> GetTypeFields(StaticDeclaredTypeWrapper type)
        {
            ITypeElement typeHandle = (ITypeElement)type.Handle;

            IClass classHandle = typeHandle as IClass;
            if (classHandle != null)
            {
                foreach (IField fieldHandle in classHandle.Fields)
                    yield return new StaticFieldWrapper(this, fieldHandle, type);
                foreach (IField fieldHandle in classHandle.Constants)
                    yield return new StaticFieldWrapper(this, fieldHandle, type);
            }
            else
            {
                IStruct structHandle = typeHandle as IStruct;
                if (structHandle != null)
                {
                    foreach (IField fieldHandle in structHandle.Fields)
                        yield return new StaticFieldWrapper(this, fieldHandle, type);
                    foreach (IField fieldHandle in structHandle.Constants)
                        yield return new StaticFieldWrapper(this, fieldHandle, type);
                }
            }
        }

        protected override IEnumerable<StaticEventWrapper> GetTypeEvents(StaticDeclaredTypeWrapper type)
        {
            ITypeElement typeHandle = (ITypeElement)type.Handle;

            foreach (IEvent eventHandle in typeHandle.Events)
                yield return new StaticEventWrapper(this, eventHandle, type);
        }

        private StaticTypeWrapper MakeType(IType typeHandle)
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
            return MakeDeclaredType(typeElementHandle, typeElementHandle.IdSubstitution);
        }

        private StaticDeclaredTypeWrapper MakeDeclaredType(IDeclaredType typeHandle)
        {
            return MakeDeclaredType(typeHandle.GetTypeElement(), typeHandle.GetSubstitution());
        }

        private StaticDeclaredTypeWrapper MakeDeclaredType(ITypeElement typeElementHandle, ISubstitution substitutionHandle)
        {
            if (typeElementHandle is ITypeParameter)
                throw new ArgumentException("This method should never be called with a generic parameter as input.", "typeElementHandle");

            ITypeElement declaringTypeElementHandle = typeElementHandle.GetContainingType();
            StaticDeclaredTypeWrapper type;
            if (declaringTypeElementHandle != null)
            {
                StaticDeclaredTypeWrapper declaringType = MakeDeclaredType(declaringTypeElementHandle, substitutionHandle);
                type = new StaticDeclaredTypeWrapper(this, typeElementHandle, declaringType, declaringType.Substitution);
            }
            else
            {
                type = new StaticDeclaredTypeWrapper(this, typeElementHandle, null, StaticTypeSubstitution.Empty);
            }

            ITypeParameter[] typeParameterHandles = typeElementHandle.TypeParameters;
            if (substitutionHandle.IsIdempotent(typeParameterHandles))
                return type;

            ITypeInfo[] genericArguments = GenericUtils.ConvertAllToArray<ITypeParameter, ITypeInfo>(typeParameterHandles, delegate(ITypeParameter typeParameterHandle)
            {
                return MakeType(substitutionHandle.Apply(typeParameterHandle));
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
            return genericParameterHandle.Index;
        }

        protected override IList<StaticTypeWrapper> GetGenericParameterConstraints(StaticGenericParameterWrapper genericParameter)
        {
            ITypeParameter genericParameterHandle = (ITypeParameter)genericParameter.Handle;
            return GenericUtils.ConvertAllToArray<IType, StaticTypeWrapper>(genericParameterHandle.TypeConstraints, MakeType);
        }
        #endregion

        #region GetDeclaredElement and GetProject
        protected override IDeclaredElement GetDeclaredElement(StaticWrapper element)
        {
            return element.Handle as IDeclaredElement;
        }

        protected override IProject GetProject(StaticWrapper element)
        {
            IProject project = element.Handle as IProject;
            if (project != null)
                return project;

            IDeclaredElement declaredElement = GetDeclaredElement(element);
            return declaredElement != null && declaredElement.IsValid() ? declaredElement.Module as IProject : null;
        }
        #endregion

        #region Misc
        private IEnumerable<StaticAttributeWrapper> GetAttributesForAttributeOwner(IAttributesOwner owner)
        {
            foreach (IAttributeInstance attribute in owner.GetAttributeInstances(false))
                yield return new StaticAttributeWrapper(this, attribute);
        }

        private StaticMethodWrapper WrapAccessor(IAccessor accessorHandle, StaticMemberWrapper member)
        {
            return accessorHandle != null ? new StaticMethodWrapper(this, accessorHandle, member.DeclaringType, member.Substitution) : null;
        }
        #endregion
    }
}