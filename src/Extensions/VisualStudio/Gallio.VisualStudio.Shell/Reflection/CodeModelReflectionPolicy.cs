// Copyright 2005-2009 Gallio Project - http://www.gallio.org/
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
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using EnvDTE80;
using Gallio.Common.Collections;
using Gallio.Common.Reflection;
using Gallio.Common.Reflection.Impl;
using EnvDTE;

namespace Gallio.VisualStudio.Shell.Reflection
{
    /// <summary>
    /// A reflection policy implemented over the Visual Studio CodeModel (ie. Intellisense).
    /// </summary>
    /// <remarks>
    /// <para>
    /// The implementation is currently incomplete.
    /// </para>
    /// </remarks>
    public class CodeModelReflectionPolicy : StaticReflectionPolicy
    {
        private readonly Solution solution;

        /// <summary>
        /// Creates a reflector with the specified CodeModel.
        /// </summary>
        /// <param name="solution">The Visual Studio Solution.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="solution"/> is null.</exception>
        public CodeModelReflectionPolicy(Solution solution)
        {
            if (solution == null)
                throw new ArgumentNullException("solution");

            this.solution = solution;
        }

        #region Wrapping
        /// <summary>
        /// Obtains a reflection wrapper for a code element.
        /// </summary>
        /// <param name="target">The element, or null if none.</param>
        /// <returns>The reflection wrapper, or null if none.</returns>
        public ICodeElementInfo Wrap(CodeElement2 target)
        {
            if (target == null)
                return null;

            switch (target.Kind)
            {
                case vsCMElement.vsCMElementClass:
                case vsCMElement.vsCMElementStruct:
                case vsCMElement.vsCMElementInterface:
                case vsCMElement.vsCMElementDelegate:
                case vsCMElement.vsCMElementEnum:
                    return Wrap((CodeType)target);
                case vsCMElement.vsCMElementFunction:
                    return Wrap((CodeFunction2) target);
                case vsCMElement.vsCMElementProperty:
                    return Wrap((CodeProperty2) target);
                case vsCMElement.vsCMElementVariable:
                    return Wrap((CodeVariable2) target);
                case vsCMElement.vsCMElementEvent:
                    return Wrap((CodeEvent)target);
                case vsCMElement.vsCMElementParameter:
                    return Wrap((CodeParameter2) target);
            }

            throw new NotSupportedException("Unsupported code element type: " + target);
        }

        /// <summary>
        /// Obtains a reflection wrapper for a type.
        /// </summary>
        /// <param name="target">The type, or null if none.</param>
        /// <returns>The reflection wrapper, or null if none.</returns>
        public StaticTypeWrapper Wrap(CodeType target)
        {
            return target != null ? MakeDeclaredType(target) : null;
        }

        /// <summary>
        /// Obtains a reflection wrapper for a function.
        /// </summary>
        /// <param name="target">The function, or null if none.</param>
        /// <returns>The reflection wrapper, or null if none.</returns>
        public StaticFunctionWrapper Wrap(CodeFunction2 target)
        {
            if (target == null)
                return null;

            StaticDeclaredTypeWrapper declaringType = MakeDeclaredType(GetContainingType((CodeElement)target));
            if (target.FunctionKind == vsCMFunction.vsCMFunctionConstructor)
                return new StaticConstructorWrapper(this, target, declaringType);
            else
                return new StaticMethodWrapper(this, target, declaringType, declaringType, declaringType.Substitution);
        }

        /// <summary>
        /// Obtains a reflection wrapper for a property.
        /// </summary>
        /// <param name="target">The property, or null if none.</param>
        /// <returns>The reflection wrapper, or null if none.</returns>
        public StaticPropertyWrapper Wrap(CodeProperty2 target)
        {
            if (target == null)
                return null;

            StaticDeclaredTypeWrapper declaringType = MakeDeclaredType(GetContainingType((CodeElement)target));
            return new StaticPropertyWrapper(this, target, declaringType, declaringType);
        }

        /// <summary>
        /// Obtains a reflection wrapper for a field.
        /// </summary>
        /// <param name="target">The field, or null if none.</param>
        /// <returns>The reflection wrapper, or null if none.</returns>
        public StaticFieldWrapper Wrap(CodeVariable2 target)
        {
            if (target == null)
                return null;

            StaticDeclaredTypeWrapper declaringType = MakeDeclaredType(GetContainingType((CodeElement)target));
            return new StaticFieldWrapper(this, target, declaringType, declaringType);
        }

        /// <summary>
        /// Obtains a reflection wrapper for an event.
        /// </summary>
        /// <param name="target">The event, or null if none.</param>
        /// <returns>The reflection wrapper, or null if none.</returns>
        public StaticEventWrapper Wrap(CodeEvent target)
        {
            if (target == null)
                return null;

            StaticDeclaredTypeWrapper declaringType = MakeDeclaredType(GetContainingType((CodeElement)target));
            return new StaticEventWrapper(this, target, declaringType, declaringType);
        }

        /// <summary>
        /// Obtains a reflection wrapper for a parameter.
        /// </summary>
        /// <param name="target">The parameter, or null if none.</param>
        /// <returns>The reflection wrapper, or null if none.</returns>
        public StaticParameterWrapper Wrap(CodeParameter2 target)
        {
            if (target == null)
                return null;

            StaticMemberWrapper member = Wrap(GetContainingFunction((CodeElement)target));
            return new StaticParameterWrapper(this, target, member);
        }
        #endregion

        #region Assemblies
        /// <inheritdoc />
        protected override IAssemblyInfo LoadAssemblyFromImpl(string assemblyFile)
        {
            throw new NotSupportedException("The Visual Studio CodeModel metadata policy does not support loading assemblies from files.");
        }

        /// <inheritdoc />
        protected override IAssemblyInfo LoadAssemblyImpl(AssemblyName assemblyName)
        {
            foreach (Project project in FindAllProjects())
            {
                string projectAssemblyName = GetProjectAssemblyName(project);
                if (projectAssemblyName == assemblyName.Name)
                {
                    CodeModel assemblyHandle = project.CodeModel;
                    if (assemblyHandle != null)
                        return new StaticAssemblyWrapper(this, assemblyHandle);
                }
            }

            throw new ArgumentException(String.Format("Could not find assembly '{0}' in the VisualStudio solution.",
                assemblyName.FullName));
        }

        /// <inheritdoc />
        protected override IEnumerable<StaticAttributeWrapper> GetAssemblyCustomAttributes(StaticAssemblyWrapper assembly)
        {
            // TODO
            yield break;
        }

        /// <inheritdoc />
        protected override AssemblyName GetAssemblyName(StaticAssemblyWrapper assembly)
        {
            CodeModel assemblyHandle = (CodeModel)assembly.Handle;
            return new AssemblyName(GetProjectAssemblyName(assemblyHandle.Parent));
        }

        /// <inheritdoc />
        protected override string GetAssemblyPath(StaticAssemblyWrapper assembly)
        {
            CodeModel assemblyHandle = (CodeModel)assembly.Handle;
            string targetPath = GetProjectTargetPath(assemblyHandle.Parent);
            if (targetPath == null)
                throw new InvalidOperationException(String.Format("Could not obtain output path of assembly: '{0}'.", assembly));

            return targetPath;
        }

        /// <inheritdoc />
        protected override IList<AssemblyName> GetAssemblyReferences(StaticAssemblyWrapper assembly)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        protected override IList<StaticDeclaredTypeWrapper> GetAssemblyExportedTypes(StaticAssemblyWrapper assembly)
        {
            return GetAssemblyTypes(assembly, true);
        }

        /// <inheritdoc />
        protected override IList<StaticDeclaredTypeWrapper> GetAssemblyTypes(StaticAssemblyWrapper assembly)
        {
            return GetAssemblyTypes(assembly, false);
        }

        /// <inheritdoc />
        protected override StaticDeclaredTypeWrapper GetAssemblyType(StaticAssemblyWrapper assembly, string typeName)
        {
            CodeModel assemblyHandle = (CodeModel)assembly.Handle;

            if (typeName.IndexOf('`') < 0)
            {
                try
                {
                    CodeType typeHandle = assemblyHandle.CodeTypeFromFullName(typeName.Replace('+', '.'));
                    if (typeHandle != null)
                        return MakeDeclaredType(typeHandle);
                }
                catch (COMException)
                {
                }
            }
            else
            {
                // The CodeTypeFromFullName API can find generic types but it needs the names of
                // the generic type parameters to be provided.  eg. "Class<T>" instead of "Class`1".
                throw new NotImplementedException("Searching for generic types not implemented currently.");
            }

            return null;
        }

        private IList<StaticDeclaredTypeWrapper> GetAssemblyTypes(StaticAssemblyWrapper assembly, bool exportedOnly)
        {
            CodeModel assemblyHandle = (CodeModel)assembly.Handle;
            var types = new List<StaticDeclaredTypeWrapper>();

            AddAssemblyTypes(assemblyHandle.CodeElements, exportedOnly, types, null);

            return types;
        }

        private void AddAssemblyTypes(CodeElements codeElements, bool exportedOnly,
            List<StaticDeclaredTypeWrapper> types, StaticDeclaredTypeWrapper declaringType)
        {
            foreach (CodeElement codeElement in codeElements)
            {
                bool mayContainNestedTypes;
                switch (codeElement.Kind)
                {
                    case vsCMElement.vsCMElementClass:
                    case vsCMElement.vsCMElementStruct:
                        mayContainNestedTypes = true;
                        break;

                    case vsCMElement.vsCMElementInterface:
                    case vsCMElement.vsCMElementDelegate:
                    case vsCMElement.vsCMElementEnum:
                        mayContainNestedTypes = false;
                        break;

                    default:
                        continue;
                }

                CodeType typeHandle = (CodeType)codeElement;
                if (! exportedOnly || typeHandle.Access == vsCMAccess.vsCMAccessPublic)
                {
                    StaticDeclaredTypeWrapper type = new StaticDeclaredTypeWrapper(this, codeElement,
                        declaringType, StaticTypeSubstitution.Empty);
                    types.Add(type);

                    if (mayContainNestedTypes)
                        AddAssemblyTypes(typeHandle.Members, exportedOnly, types, type);
                }
            }
        }

        private IEnumerable<Project> FindAllProjects()
        {
            foreach (Project project in solution.Projects)
            {
                if (project.Kind != Constants.vsProjectKindSolutionItems)
                    yield return project;

                foreach (Project subProject in FindAllProjects(project.ProjectItems))
                    yield return subProject;
            }
        }

        private static IEnumerable<Project> FindAllProjects(ProjectItems parent)
        {
            foreach (ProjectItem projectItem in parent)
            {
                Project project = projectItem.SubProject;
                if (project != null)
                {
                    if (project.Kind != Constants.vsProjectKindSolutionItems)
                        yield return project;

                    foreach (Project subProject in FindAllProjects(project.ProjectItems))
                        yield return subProject;
                }
            }
        }

        private static string GetProjectAssemblyName(Project project)
        {
            try
            {
                Property projectAssemblyNameProp = project.Properties.Item("AssemblyName");
                if (projectAssemblyNameProp != null)
                    return projectAssemblyNameProp.Value as string;
            }
            catch (ArgumentException)
            {
            }

            return null;
        }

        private static string GetProjectTargetPath(Project project)
        {
            try
            {
                Configuration configuration = project.ConfigurationManager.ActiveConfiguration;
                if (configuration != null)
                {
                    string fullPath = (string)project.Properties.Item("FullPath").Value;
                    string outputPath = (string)configuration.Properties.Item("OutputPath").Value;
                    string outputFileName = (string)project.Properties.Item("OutputFileName").Value;

                    return Path.Combine(Path.Combine(fullPath, outputPath), outputFileName);
                }
            }
            catch (ArgumentException)
            {
            }

            return null;
        }
        #endregion

        #region Attributes
        /// <inheritdoc />
        protected override StaticConstructorWrapper GetAttributeConstructor(StaticAttributeWrapper attribute)
        {
            CodeAttribute2 attributeHandle = (CodeAttribute2)attribute.Handle;
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        protected override ConstantValue[] GetAttributeConstructorArguments(StaticAttributeWrapper attribute)
        {
            CodeAttribute2 attributeHandle = (CodeAttribute2)attribute.Handle;
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        protected override IEnumerable<KeyValuePair<StaticFieldWrapper, ConstantValue>> GetAttributeFieldArguments(
            StaticAttributeWrapper attribute)
        {
            CodeAttribute2 attributeHandle = (CodeAttribute2)attribute.Handle;
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        protected override IEnumerable<KeyValuePair<StaticPropertyWrapper, ConstantValue>> GetAttributePropertyArguments(
            StaticAttributeWrapper attribute)
        {
            CodeAttribute2 attributeHandle = (CodeAttribute2)attribute.Handle;
            throw new NotImplementedException();
        }
        #endregion

        #region Members
        /// <inheritdoc />
        protected override IEnumerable<StaticAttributeWrapper> GetMemberCustomAttributes(StaticMemberWrapper member)
        {
            CodeElement2 memberHandle = (CodeElement2)member.Handle;
            switch (memberHandle.Kind)
            {
                case vsCMElement.vsCMElementClass:
                    return WrapAttributes(((CodeClass2)memberHandle).Attributes);
                case vsCMElement.vsCMElementStruct:
                    return WrapAttributes(((CodeStruct2)memberHandle).Attributes);
                case vsCMElement.vsCMElementInterface:
                    return WrapAttributes(((CodeInterface2)memberHandle).Attributes);
                case vsCMElement.vsCMElementDelegate:
                    return WrapAttributes(((CodeDelegate2)memberHandle).Attributes);
                case vsCMElement.vsCMElementEnum:
                    return WrapAttributes(((CodeEnum)memberHandle).Attributes);
                case vsCMElement.vsCMElementFunction:
                    return WrapAttributes(((CodeFunction2)memberHandle).Attributes);
                case vsCMElement.vsCMElementProperty:
                    return WrapAttributes(((CodeProperty2)memberHandle).Attributes);
                case vsCMElement.vsCMElementVariable:
                    return WrapAttributes(((CodeVariable2)memberHandle).Attributes);
                case vsCMElement.vsCMElementEvent:
                    return WrapAttributes(((CodeEvent)memberHandle).Attributes);
            }

            return EmptyArray<StaticAttributeWrapper>.Instance;
        }

        /// <inheritdoc />
        protected override string GetMemberName(StaticMemberWrapper member)
        {
            CodeElement memberHandle = (CodeElement)member.Handle;
            return memberHandle.Name;
        }

        /// <inheritdoc />
        protected override CodeLocation GetMemberSourceLocation(StaticMemberWrapper member)
        {
            CodeElement memberHandle = (CodeElement)member.Handle;
            ProjectItem projectItem = memberHandle.ProjectItem;
            if (projectItem.FileCount == 0)
                return CodeLocation.Unknown;

            string filename = projectItem.get_FileNames(0);
            TextPoint point = memberHandle.GetStartPoint(vsCMPart.vsCMPartName);

            return new CodeLocation(filename, point.Line, point.LineCharOffset + 1);
        }
        #endregion

        #region Events
        /// <inheritdoc />
        protected override EventAttributes GetEventAttributes(StaticEventWrapper @event)
        {
            return EventAttributes.None;
        }

        /// <inheritdoc />
        protected override StaticMethodWrapper GetEventAddMethod(StaticEventWrapper @event)
        {
            CodeEvent eventHandle = (CodeEvent)@event.Handle;
            return WrapAccessor(eventHandle.Adder, @event);
        }

        /// <inheritdoc />
        protected override StaticMethodWrapper GetEventRaiseMethod(StaticEventWrapper @event)
        {
            // FIXME: Not supported by code model.
            return null;
        }

        /// <inheritdoc />
        protected override StaticMethodWrapper GetEventRemoveMethod(StaticEventWrapper @event)
        {
            CodeEvent eventHandle = (CodeEvent)@event.Handle;
            return WrapAccessor(eventHandle.Remover, @event);
        }

        /// <inheritdoc />
        protected override StaticTypeWrapper GetEventHandlerType(StaticEventWrapper @event)
        {
            CodeEvent eventHandle = (CodeEvent)@event.Handle;
            return MakeType(eventHandle.Type);
        }
        #endregion

        #region Fields
        /// <inheritdoc />
        protected override FieldAttributes GetFieldAttributes(StaticFieldWrapper field)
        {
            CodeVariable2 fieldHandle = (CodeVariable2)field.Handle;

            // FIXME: Don't know how to handle vsCMAccessWithEvents
            FieldAttributes flags = 0;
            switch (fieldHandle.Access)
            {
                case vsCMAccess.vsCMAccessPublic:
                    flags |= FieldAttributes.Public;
                    break;
                case vsCMAccess.vsCMAccessPrivate:
                    flags |= FieldAttributes.Private;
                    break;
                case vsCMAccess.vsCMAccessDefault:
                case vsCMAccess.vsCMAccessProject:
                    flags |= FieldAttributes.Assembly;
                    break;
                case vsCMAccess.vsCMAccessProtected:
                    flags |= FieldAttributes.Family;
                    break;
                case vsCMAccess.vsCMAccessAssemblyOrFamily:
                case vsCMAccess.vsCMAccessProjectOrProtected:
                    flags |= FieldAttributes.FamORAssem;
                    break;
            }

            ReflectorFlagsUtils.AddFlagIfTrue(ref flags, FieldAttributes.Static, fieldHandle.IsShared);
            ReflectorFlagsUtils.AddFlagIfTrue(ref flags, FieldAttributes.InitOnly, fieldHandle.ConstKind == vsCMConstKind.vsCMConstKindReadOnly);
            ReflectorFlagsUtils.AddFlagIfTrue(ref flags, FieldAttributes.Literal, fieldHandle.ConstKind == vsCMConstKind.vsCMConstKindConst);
            return flags;
        }

        /// <inheritdoc />
        protected override StaticTypeWrapper GetFieldType(StaticFieldWrapper field)
        {
            CodeVariable2 fieldHandle = (CodeVariable2)field.Handle;
            return MakeType(fieldHandle.Type);
        }
        #endregion

        #region Properties
        /// <inheritdoc />
        protected override PropertyAttributes GetPropertyAttributes(StaticPropertyWrapper property)
        {
            // Note: There don't seem to be any usable property attributes.
            return 0;
        }

        /// <inheritdoc />
        protected override StaticTypeWrapper GetPropertyType(StaticPropertyWrapper property)
        {
            CodeProperty propertyHandle = (CodeProperty)property.Handle;
            return MakeType(propertyHandle.Type);
        }

        /// <inheritdoc />
        protected override StaticMethodWrapper GetPropertyGetMethod(StaticPropertyWrapper property)
        {
            CodeProperty propertyHandle = (CodeProperty)property.Handle;
            return WrapAccessor(propertyHandle.Getter, property);
        }

        /// <inheritdoc />
        protected override StaticMethodWrapper GetPropertySetMethod(StaticPropertyWrapper property)
        {
            CodeProperty propertyHandle = (CodeProperty)property.Handle;
            return WrapAccessor(propertyHandle.Setter, property);
        }
        #endregion

        #region Functions
        /// <inheritdoc />
        protected override MethodAttributes GetFunctionAttributes(StaticFunctionWrapper function)
        {
            CodeFunction2 functionHandle = (CodeFunction2)function.Handle;

            // FIXME: Don't know how to handle vsCMAccessWithEvents
            MethodAttributes flags = 0;
            switch (functionHandle.Access)
            {
                case vsCMAccess.vsCMAccessPublic:
                    flags |= MethodAttributes.Public;
                    break;
                case vsCMAccess.vsCMAccessPrivate:
                    flags |= MethodAttributes.Private;
                    break;
                case vsCMAccess.vsCMAccessDefault:
                case vsCMAccess.vsCMAccessProject:
                    flags |= MethodAttributes.Assembly;
                    break;
                case vsCMAccess.vsCMAccessProtected:
                    flags |= MethodAttributes.Family;
                    break;
                case vsCMAccess.vsCMAccessAssemblyOrFamily:
                case vsCMAccess.vsCMAccessProjectOrProtected:
                    flags |= MethodAttributes.FamORAssem;
                    break;
            }

            ReflectorFlagsUtils.AddFlagIfTrue(ref flags, MethodAttributes.Static, functionHandle.IsShared);
            ReflectorFlagsUtils.AddFlagIfTrue(ref flags, MethodAttributes.Abstract, (functionHandle.OverrideKind & vsCMOverrideKind.vsCMOverrideKindAbstract) != 0);
            ReflectorFlagsUtils.AddFlagIfTrue(ref flags, MethodAttributes.Final, (functionHandle.OverrideKind & vsCMOverrideKind.vsCMOverrideKindSealed) != 0);
            ReflectorFlagsUtils.AddFlagIfTrue(ref flags, MethodAttributes.Virtual, (functionHandle.OverrideKind & vsCMOverrideKind.vsCMOverrideKindVirtual) != 0);
            ReflectorFlagsUtils.AddFlagIfTrue(ref flags, MethodAttributes.NewSlot, (functionHandle.OverrideKind & vsCMOverrideKind.vsCMOverrideKindOverride) == 0);
            ReflectorFlagsUtils.AddFlagIfTrue(ref flags, MethodAttributes.HideBySig, true); // FIXME
            return flags;
        }

        /// <inheritdoc />
        protected override CallingConventions GetFunctionCallingConvention(StaticFunctionWrapper function)
        {
            CodeFunction2 functionHandle = (CodeFunction2)function.Handle;

            // FIXME: No way to determine VarArgs convention.
            CallingConventions flags = CallingConventions.Standard;
            ReflectorFlagsUtils.AddFlagIfTrue(ref flags, CallingConventions.HasThis, !functionHandle.IsShared);
            return flags;
        }

        /// <inheritdoc />
        protected override IList<StaticParameterWrapper> GetFunctionParameters(StaticFunctionWrapper function)
        {
            CodeFunction2 functionHandle = (CodeFunction2)function.Handle;

            return ConvertCodeElementsToArray<CodeParameter2, StaticParameterWrapper>(functionHandle.Parameters, delegate(CodeParameter2 parameter)
            {
                return new StaticParameterWrapper(this, parameter, function);
            });
        }
        #endregion

        #region Methods
        /// <inheritdoc />
        protected override IList<StaticGenericParameterWrapper> GetMethodGenericParameters(StaticMethodWrapper method)
        {
            // FIXME: Not supported.
            return EmptyArray<StaticGenericParameterWrapper>.Instance;
        }

        /// <inheritdoc />
        protected override StaticParameterWrapper GetMethodReturnParameter(StaticMethodWrapper method)
        {
            CodeFunction2 methodHandle = (CodeFunction2)method.Handle;

            // TODO: This won't provide access to any parameter attributes.  How should we retrieve them?
            CodeTypeRef type = methodHandle.Type;
            return type != null ? new StaticParameterWrapper(this, type, method) : null;
        }
        #endregion

        #region Parameters
        /// <inheritdoc />
        protected override ParameterAttributes GetParameterAttributes(StaticParameterWrapper parameter)
        {
            if (parameter.Handle is CodeTypeRef)
                return ParameterAttributes.None;

            CodeParameter2 parameterHandle = (CodeParameter2)parameter.Handle;

            vsCMParameterKind kind = parameterHandle.ParameterKind;
            ParameterAttributes flags = 0;
            ReflectorFlagsUtils.AddFlagIfTrue(ref flags, ParameterAttributes.HasDefault, parameterHandle.DefaultValue != null);
            ReflectorFlagsUtils.AddFlagIfTrue(ref flags, ParameterAttributes.In, (kind & vsCMParameterKind.vsCMParameterKindIn) != 0);
            ReflectorFlagsUtils.AddFlagIfTrue(ref flags, ParameterAttributes.Out, (kind & vsCMParameterKind.vsCMParameterKindOut) != 0);
            ReflectorFlagsUtils.AddFlagIfTrue(ref flags, ParameterAttributes.Optional, (kind & vsCMParameterKind.vsCMParameterKindOptional) != 0);
            return flags;
        }

        /// <inheritdoc />
        protected override IEnumerable<StaticAttributeWrapper> GetParameterCustomAttributes(StaticParameterWrapper parameter)
        {
            if (parameter.Handle is CodeTypeRef)
                return EmptyArray<StaticAttributeWrapper>.Instance;

            CodeParameter2 parameterHandle = (CodeParameter2)parameter.Handle;
            return WrapAttributes(parameterHandle.Attributes);
        }

        /// <inheritdoc />
        protected override string GetParameterName(StaticParameterWrapper parameter)
        {
            if (parameter.Handle is CodeTypeRef)
                return null;

            CodeParameter2 parameterHandle = (CodeParameter2)parameter.Handle;
            return parameterHandle.Name;
        }

        /// <inheritdoc />
        protected override int GetParameterPosition(StaticParameterWrapper parameter)
        {
            if (parameter.Handle is CodeTypeRef)
                return -1;

            CodeParameter2 parameterHandle = (CodeParameter2)parameter.Handle;

            CodeElements codeElements = parameterHandle.Collection;
            for (int i = 0; i < codeElements.Count; i++)
                if (codeElements.Item(i) == parameterHandle)
                    return i;

            throw new InvalidOperationException("Could not obtain position of parameter.");
        }

        /// <inheritdoc />
        protected override StaticTypeWrapper GetParameterType(StaticParameterWrapper parameter)
        {
            CodeTypeRef returnTypeHandle = parameter.Handle as CodeTypeRef;
            if (returnTypeHandle != null)
                return MakeType(returnTypeHandle);

            CodeParameter2 parameterHandle = (CodeParameter2)parameter.Handle;
            StaticTypeWrapper parameterType = MakeType(parameterHandle.Type);

            if ((parameterHandle.ParameterKind & (vsCMParameterKind.vsCMParameterKindRef | vsCMParameterKind.vsCMParameterKindOut)) != 0)
                parameterType = parameterType.MakeByRefType();

            return parameterType;
        }
        #endregion

        #region Types
        /// <inheritdoc />
        protected override TypeAttributes GetTypeAttributes(StaticDeclaredTypeWrapper type)
        {
            CodeType typeHandle = (CodeType)type.Handle;

            TypeAttributes flags = 0;
            ReflectorFlagsUtils.AddFlagIfTrue(ref flags, TypeAttributes.Interface, typeHandle.Kind == vsCMElement.vsCMElementInterface);

            //TODO ReflectorFlagsUtils.AddFlagIfTrue(ref flags, TypeAttributes.Abstract, typeHandle.);
            //TODO ReflectorFlagsUtils.AddFlagIfTrue(ref flags, TypeAttributes.Sealed, modifiers.IsSealed);

            bool isNested = typeHandle.Parent is CodeType;

            // FIXME: Don't know what to do with WithEvents
            switch (typeHandle.Access)
            {
                case vsCMAccess.vsCMAccessPublic:
                    flags |= isNested ? TypeAttributes.NestedPublic : TypeAttributes.Public;
                    break;
                case vsCMAccess.vsCMAccessPrivate:
                    flags |= isNested ? TypeAttributes.NestedPrivate : TypeAttributes.NotPublic;
                    break;
                case vsCMAccess.vsCMAccessDefault:
                case vsCMAccess.vsCMAccessProject:
                    flags |= isNested ? TypeAttributes.NestedAssembly : TypeAttributes.NotPublic;
                    break;
                case vsCMAccess.vsCMAccessProtected:
                    flags |= isNested ? TypeAttributes.NestedFamily : TypeAttributes.NotPublic;
                    break;
                case vsCMAccess.vsCMAccessAssemblyOrFamily:
                case vsCMAccess.vsCMAccessProjectOrProtected:
                    flags |= isNested ? TypeAttributes.NestedFamORAssem : TypeAttributes.NotPublic;
                    break;
            }

            return flags;
        }

        /// <inheritdoc />
        protected override StaticAssemblyWrapper GetTypeAssembly(StaticDeclaredTypeWrapper type)
        {
            CodeType typeHandle = (CodeType)type.Handle;
            return new StaticAssemblyWrapper(this, typeHandle.ProjectItem.ContainingProject.CodeModel);
        }

        /// <inheritdoc />
        protected override string GetTypeNamespace(StaticDeclaredTypeWrapper type)
        {
            CodeType typeHandle = (CodeType)type.Handle;
            CodeNamespace namespaceHandle = typeHandle.Namespace;
            return namespaceHandle != null ? namespaceHandle.FullName : "";
        }

        /// <inheritdoc />
        protected override StaticDeclaredTypeWrapper GetTypeBaseType(StaticDeclaredTypeWrapper type)
        {
            CodeType typeHandle = (CodeType)type.Handle;

            if (typeHandle.Kind == vsCMElement.vsCMElementClass)
            {
                foreach (CodeType superType in typeHandle.Bases)
                {
                    if (superType.Kind == vsCMElement.vsCMElementClass)
                        return MakeDeclaredType(superType);
                }
            }

            return null;
        }

        /// <inheritdoc />
        protected override IList<StaticDeclaredTypeWrapper> GetTypeInterfaces(StaticDeclaredTypeWrapper type)
        {
            CodeType typeHandle = (CodeType)type.Handle;
            List<StaticDeclaredTypeWrapper> interfaces = new List<StaticDeclaredTypeWrapper>();

            foreach (CodeType superType in typeHandle.Bases)
            {
                if (superType.Kind == vsCMElement.vsCMElementInterface)
                    interfaces.Add(MakeDeclaredType(superType));
            }

            return interfaces;
        }

        /// <inheritdoc />
        protected override IList<StaticGenericParameterWrapper> GetTypeGenericParameters(StaticDeclaredTypeWrapper type)
        {
            // FIXME: Not supported.
            return EmptyArray<StaticGenericParameterWrapper>.Instance;
        }

        /// <inheritdoc />
        protected override IEnumerable<StaticConstructorWrapper> GetTypeConstructors(StaticDeclaredTypeWrapper type)
        {
            CodeType typeHandle = (CodeType)type.Handle;

            foreach (CodeElement candidate in typeHandle.Members)
            {
                if (candidate.Kind == vsCMElement.vsCMElementFunction)
                {
                    CodeFunction2 function = (CodeFunction2)candidate;
                    if (function.FunctionKind == vsCMFunction.vsCMFunctionConstructor)
                        yield return new StaticConstructorWrapper(this, function, type);
                }
            }
        }

        /// <inheritdoc />
        protected override IEnumerable<StaticMethodWrapper> GetTypeMethods(StaticDeclaredTypeWrapper type,
            StaticDeclaredTypeWrapper reflectedType)
        {
            CodeType typeHandle = (CodeType)type.Handle;

            foreach (CodeElement candidate in typeHandle.Members)
            {
                if (candidate.Kind == vsCMElement.vsCMElementFunction)
                    yield return new StaticMethodWrapper(this, candidate, type, reflectedType, type.Substitution);
            }
        }

        /// <inheritdoc />
        protected override IEnumerable<StaticPropertyWrapper> GetTypeProperties(StaticDeclaredTypeWrapper type,
            StaticDeclaredTypeWrapper reflectedType)
        {
            CodeType typeHandle = (CodeType)type.Handle;

            foreach (CodeElement candidate in typeHandle.Members)
            {
                if (candidate.Kind == vsCMElement.vsCMElementProperty)
                    yield return new StaticPropertyWrapper(this, candidate, type, reflectedType);
            }
        }

        /// <inheritdoc />
        protected override IEnumerable<StaticFieldWrapper> GetTypeFields(StaticDeclaredTypeWrapper type,
            StaticDeclaredTypeWrapper reflectedType)
        {
            CodeType typeHandle = (CodeType)type.Handle;

            foreach (CodeElement candidate in typeHandle.Members)
            {
                if (candidate.Kind == vsCMElement.vsCMElementVariable)
                    yield return new StaticFieldWrapper(this, candidate, type, reflectedType);
            }
        }

        /// <inheritdoc />
        protected override IEnumerable<StaticEventWrapper> GetTypeEvents(StaticDeclaredTypeWrapper type,
            StaticDeclaredTypeWrapper reflectedType)
        {
            CodeType typeHandle = (CodeType)type.Handle;

            foreach (CodeElement candidate in typeHandle.Members)
            {
                if (candidate.Kind == vsCMElement.vsCMElementEvent)
                    yield return new StaticEventWrapper(this, candidate, type, reflectedType);
            }
        }

        /// <inheritdoc />
        protected override IEnumerable<StaticTypeWrapper> GetTypeNestedTypes(StaticDeclaredTypeWrapper type)
        {
            CodeType typeHandle = (CodeType)type.Handle;

            foreach (CodeElement candidate in typeHandle.Members)
            {
                vsCMElement kind = candidate.Kind;
                if (kind == vsCMElement.vsCMElementClass
                    || kind == vsCMElement.vsCMElementDelegate
                    || kind == vsCMElement.vsCMElementEnum
                    || kind == vsCMElement.vsCMElementEvent
                    || kind == vsCMElement.vsCMElementInterface
                    || kind == vsCMElement.vsCMElementStruct)
                    yield return new StaticDeclaredTypeWrapper(this, candidate, type, type.Substitution);
            }
        }

        private StaticTypeWrapper MakeType(CodeTypeRef typeHandle)
        {
            int kind = (int) typeHandle.TypeKind;
            switch (kind)
            {
                case (int) vsCMTypeRef.vsCMTypeRefOther:
                    throw new NotSupportedException("Other");

                case (int) vsCMTypeRef.vsCMTypeRefCodeType:
                    return MakeDeclaredType(typeHandle.CodeType);

                case (int)vsCMTypeRef.vsCMTypeRefArray:
                    return MakeType(typeHandle.ElementType).MakeArrayType(typeHandle.Rank);

                case (int)vsCMTypeRef.vsCMTypeRefPointer:
                    return MakeType(typeHandle.ElementType).MakePointerType();

                case (int)vsCMTypeRef2.vsCMTypeRefReference:
                    return MakeType(typeHandle.ElementType).MakeByRefType();

                case (int)vsCMTypeRef2.vsCMTypeRefMCBoxedReference:
                    throw new NotSupportedException("Boxed Reference");

                case (int)vsCMTypeRef.vsCMTypeRefVoid:
                    return WrapNativeType(typeof(void));

                case (int)vsCMTypeRef.vsCMTypeRefString:
                    return WrapNativeType(typeof(String));

                case (int)vsCMTypeRef.vsCMTypeRefObject:
                    return WrapNativeType(typeof(Object));

                case (int)vsCMTypeRef.vsCMTypeRefByte:
                    return WrapNativeType(typeof(Byte));

                case (int)vsCMTypeRef2.vsCMTypeRefSByte:
                    return WrapNativeType(typeof(SByte));

                case (int)vsCMTypeRef.vsCMTypeRefChar:
                    return WrapNativeType(typeof(Char));

                case (int)vsCMTypeRef2.vsCMTypeRefUnsignedChar:
                    throw new NotSupportedException("Unsigned Char");

                case (int)vsCMTypeRef.vsCMTypeRefShort:
                    return WrapNativeType(typeof(Int16));

                case (int)vsCMTypeRef2.vsCMTypeRefUnsignedShort:
                    return WrapNativeType(typeof(UInt16));

                case (int)vsCMTypeRef.vsCMTypeRefInt:
                    return WrapNativeType(typeof(Int32));

                case (int)vsCMTypeRef2.vsCMTypeRefUnsignedInt:
                    return WrapNativeType(typeof(UInt32));

                case (int)vsCMTypeRef.vsCMTypeRefLong:
                    return WrapNativeType(typeof(Int64));

                case (int)vsCMTypeRef2.vsCMTypeRefUnsignedLong:
                    return WrapNativeType(typeof(UInt64));

                case (int)vsCMTypeRef.vsCMTypeRefFloat:
                    return WrapNativeType(typeof(Single));

                case (int)vsCMTypeRef.vsCMTypeRefDouble:
                    return WrapNativeType(typeof(Double));

                case (int)vsCMTypeRef.vsCMTypeRefDecimal:
                    return WrapNativeType(typeof(Decimal));

                case (int)vsCMTypeRef.vsCMTypeRefBool:
                    return WrapNativeType(typeof(Boolean));

                case (int)vsCMTypeRef.vsCMTypeRefVariant:
                    throw new NotSupportedException("Variant");

                default:
                    throw new NotSupportedException("Kind: " + kind);
            }
        }

        private StaticTypeWrapper WrapNativeType(Type type)
        {
            // FIXME: Could perhaps return a reference to a Cecil static type wrapper.
            // Or maybe there's a way to get a code model object for an arbitrary type.
            throw new NotSupportedException(type.ToString());
        }

        private StaticDeclaredTypeWrapper MakeDeclaredType(CodeType typeHandle)
        {
            CodeType declaringTypeHandle = typeHandle.Parent as CodeType;
            StaticDeclaredTypeWrapper declaringType = declaringTypeHandle != null ? MakeDeclaredType(declaringTypeHandle) : null;

            return new StaticDeclaredTypeWrapper(this, typeHandle, declaringType, StaticTypeSubstitution.Empty);
        }
        #endregion

        #region Generic Parameters
        /// <inheritdoc />
        protected override GenericParameterAttributes GetGenericParameterAttributes(StaticGenericParameterWrapper genericParameter)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        protected override int GetGenericParameterPosition(StaticGenericParameterWrapper genericParameter)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        protected override IList<StaticTypeWrapper> GetGenericParameterConstraints(StaticGenericParameterWrapper genericParameter)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region Misc
        private IEnumerable<StaticAttributeWrapper> WrapAttributes(CodeElements attributeHandles)
        {
            foreach (CodeAttribute2 attributeHandle in attributeHandles)
                yield return new StaticAttributeWrapper(this, attributeHandle);
        }

        private StaticMethodWrapper WrapAccessor(CodeFunction accessorHandle, StaticMemberWrapper member)
        {
            return accessorHandle != null ? new StaticMethodWrapper(this, accessorHandle, member.DeclaringType, member.ReflectedType, member.Substitution) : null;
        }

        private static TOutput[] ConvertCodeElementsToArray<TInput, TOutput>(CodeElements codeElements, Converter<TInput, TOutput> converter)
        {
            int count = codeElements.Count;
            if (count == 0)
                return EmptyArray<TOutput>.Instance;

            TOutput[] result = new TOutput[count];
            for (int i = 0; i < count; i++)
                result[i] = converter((TInput)codeElements.Item(i + 1));

            return result;
        }

        private static CodeType GetContainingType(CodeElement codeElement)
        {
            for (;;)
            {
                codeElement = (CodeElement) codeElement.Collection.Parent;
                if (codeElement == null)
                    throw new InvalidOperationException("Could not find declaring type.");

                if (codeElement.IsCodeType)
                    return (CodeType)codeElement;
            }
        }

        private static CodeFunction2 GetContainingFunction(CodeElement codeElement)
        {
            for (;;)
            {
                codeElement = (CodeElement)codeElement.Collection.Parent;
                if (codeElement == null)
                    throw new InvalidOperationException("Could not find declaring type.");

                if (codeElement.Kind == vsCMElement.vsCMElementFunction)
                    return (CodeFunction2)codeElement;
            }
        }
        #endregion
    }
}
