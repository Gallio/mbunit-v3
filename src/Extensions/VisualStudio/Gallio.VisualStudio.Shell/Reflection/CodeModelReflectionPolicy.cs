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
using EnvDTE80;
using Gallio.Collections;
using Gallio.Reflection;
using Gallio.Reflection.Impl;
using EnvDTE;

namespace Gallio.VisualStudio.Shell.Reflection
{
    /// <summary>
    /// A reflection policy implemented over the Visual Studio CodeModel (ie. Intellisense).
    /// </summary>
    /// <remarks>
    /// The implementation is currently incomplete.
    /// </remarks>
    public class CodeModelReflectionPolicy : StaticReflectionPolicy
    {
        private readonly CodeModel2 codeModel;

        /// <summary>
        /// Creates a reflector with the specified CodeModel.
        /// </summary>
        /// <param name="codeModel">The Visual Studio CodeModel</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="codeModel"/> is null</exception>
        public CodeModelReflectionPolicy(CodeModel2 codeModel)
        {
            if (codeModel == null)
                throw new ArgumentNullException("codeModel");

            this.codeModel = codeModel;
        }

        #region Wrapping
        /// <summary>
        /// Obtains a reflection wrapper for a code element.
        /// </summary>
        /// <param name="target">The element, or null if none</param>
        /// <returns>The reflection wrapper, or null if none</returns>
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
        /// <param name="target">The type, or null if none</param>
        /// <returns>The reflection wrapper, or null if none</returns>
        public StaticTypeWrapper Wrap(CodeType target)
        {
            return target != null ? MakeTypeWithoutSubstitution(target) : null;
        }

        /// <summary>
        /// Obtains a reflection wrapper for a function.
        /// </summary>
        /// <param name="target">The function, or null if none</param>
        /// <returns>The reflection wrapper, or null if none</returns>
        public StaticFunctionWrapper Wrap(CodeFunction2 target)
        {
            if (target == null)
                return null;

            StaticDeclaredTypeWrapper declaringType = MakeDeclaredTypeWithoutSubstitution(GetContainingType((CodeElement)target));
            if (target.FunctionKind == vsCMFunction.vsCMFunctionConstructor)
                return new StaticConstructorWrapper(this, target, declaringType);
            else
                return new StaticMethodWrapper(this, target, declaringType, declaringType, declaringType.Substitution);
        }

        /// <summary>
        /// Obtains a reflection wrapper for a property.
        /// </summary>
        /// <param name="target">The property, or null if none</param>
        /// <returns>The reflection wrapper, or null if none</returns>
        public StaticPropertyWrapper Wrap(CodeProperty2 target)
        {
            if (target == null)
                return null;

            StaticDeclaredTypeWrapper declaringType = MakeDeclaredTypeWithoutSubstitution(GetContainingType((CodeElement)target));
            return new StaticPropertyWrapper(this, target, declaringType, declaringType);
        }

        /// <summary>
        /// Obtains a reflection wrapper for a field.
        /// </summary>
        /// <param name="target">The field, or null if none</param>
        /// <returns>The reflection wrapper, or null if none</returns>
        public StaticFieldWrapper Wrap(CodeVariable2 target)
        {
            if (target == null)
                return null;

            StaticDeclaredTypeWrapper declaringType = MakeDeclaredTypeWithoutSubstitution(GetContainingType((CodeElement)target));
            return new StaticFieldWrapper(this, target, declaringType, declaringType);
        }

        /// <summary>
        /// Obtains a reflection wrapper for an event.
        /// </summary>
        /// <param name="target">The event, or null if none</param>
        /// <returns>The reflection wrapper, or null if none</returns>
        public StaticEventWrapper Wrap(CodeEvent target)
        {
            if (target == null)
                return null;

            StaticDeclaredTypeWrapper declaringType = MakeDeclaredTypeWithoutSubstitution(GetContainingType((CodeElement)target));
            return new StaticEventWrapper(this, target, declaringType, declaringType);
        }

        /// <summary>
        /// Obtains a reflection wrapper for a parameter.
        /// </summary>
        /// <param name="target">The parameter, or null if none</param>
        /// <returns>The reflection wrapper, or null if none</returns>
        public StaticParameterWrapper Wrap(CodeParameter2 target)
        {
            if (target == null)
                return null;

            StaticMemberWrapper member = Wrap(GetContainingFunction((CodeElement)target));
            return new StaticParameterWrapper(this, target, member);
        }
        #endregion

        #region Assemblies
        protected override IAssemblyInfo LoadAssemblyFromImpl(string assemblyFile)
        {
            throw new NotImplementedException();
        }

        protected override IAssemblyInfo LoadAssemblyImpl(AssemblyName assemblyName)
        {
            throw new NotImplementedException();
        }

        protected override IEnumerable<StaticAttributeWrapper> GetAssemblyCustomAttributes(StaticAssemblyWrapper assembly)
        {
            throw new NotImplementedException();
        }

        protected override AssemblyName GetAssemblyName(StaticAssemblyWrapper assembly)
        {
            throw new NotImplementedException();
        }

        protected override string GetAssemblyPath(StaticAssemblyWrapper assembly)
        {
            throw new NotImplementedException();
        }

        protected override IList<AssemblyName> GetAssemblyReferences(StaticAssemblyWrapper assembly)
        {
            throw new NotImplementedException();
        }

        protected override IList<StaticDeclaredTypeWrapper> GetAssemblyExportedTypes(StaticAssemblyWrapper assembly)
        {
            throw new NotImplementedException();
        }

        protected override IList<StaticDeclaredTypeWrapper> GetAssemblyTypes(StaticAssemblyWrapper assembly)
        {
            throw new NotImplementedException();
        }

        protected override StaticDeclaredTypeWrapper GetAssemblyType(StaticAssemblyWrapper assembly, string typeName)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region Attributes
        protected override StaticConstructorWrapper GetAttributeConstructor(StaticAttributeWrapper attribute)
        {
            CodeAttribute2 attributeHandle = (CodeAttribute2)attribute.Handle;
            throw new NotImplementedException();
        }

        protected override ConstantValue[] GetAttributeConstructorArguments(StaticAttributeWrapper attribute)
        {
            CodeAttribute2 attributeHandle = (CodeAttribute2)attribute.Handle;
            throw new NotImplementedException();
        }

        protected override IEnumerable<KeyValuePair<StaticFieldWrapper, ConstantValue>> GetAttributeFieldArguments(
            StaticAttributeWrapper attribute)
        {
            CodeAttribute2 attributeHandle = (CodeAttribute2)attribute.Handle;
            throw new NotImplementedException();
        }

        protected override IEnumerable<KeyValuePair<StaticPropertyWrapper, ConstantValue>> GetAttributePropertyArguments(
            StaticAttributeWrapper attribute)
        {
            CodeAttribute2 attributeHandle = (CodeAttribute2)attribute.Handle;
            throw new NotImplementedException();
        }
        #endregion

        #region Members
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

        protected override string GetMemberName(StaticMemberWrapper member)
        {
            CodeElement memberHandle = (CodeElement)member.Handle;
            return memberHandle.Name;
        }

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
        protected override EventAttributes GetEventAttributes(StaticEventWrapper @event)
        {
            return EventAttributes.None;
        }

        protected override StaticMethodWrapper GetEventAddMethod(StaticEventWrapper @event)
        {
            CodeEvent eventHandle = (CodeEvent)@event.Handle;
            return WrapAccessor(eventHandle.Adder, @event);
        }

        protected override StaticMethodWrapper GetEventRaiseMethod(StaticEventWrapper @event)
        {
            // FIXME: Not supported by code model.
            return null;
        }

        protected override StaticMethodWrapper GetEventRemoveMethod(StaticEventWrapper @event)
        {
            CodeEvent eventHandle = (CodeEvent)@event.Handle;
            return WrapAccessor(eventHandle.Remover, @event);
        }

        protected override StaticTypeWrapper GetEventHandlerType(StaticEventWrapper @event)
        {
            CodeEvent eventHandle = (CodeEvent)@event.Handle;
            return MakeType(eventHandle.Type);
        }
        #endregion

        #region Fields
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

        protected override StaticTypeWrapper GetFieldType(StaticFieldWrapper field)
        {
            CodeVariable2 fieldHandle = (CodeVariable2)field.Handle;
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
            CodeProperty2 propertyHandle = (CodeProperty2)property.Handle;
            return MakeType(propertyHandle.Type);
        }

        protected override StaticMethodWrapper GetPropertyGetMethod(StaticPropertyWrapper property)
        {
            CodeProperty2 propertyHandle = (CodeProperty2)property.Handle;
            return WrapAccessor(propertyHandle.Getter, property);
        }

        protected override StaticMethodWrapper GetPropertySetMethod(StaticPropertyWrapper property)
        {
            CodeProperty2 propertyHandle = (CodeProperty2)property.Handle;
            return WrapAccessor(propertyHandle.Setter, property);
        }
        #endregion

        #region Functions
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

        protected override CallingConventions GetFunctionCallingConvention(StaticFunctionWrapper function)
        {
            CodeFunction2 functionHandle = (CodeFunction2)function.Handle;

            // FIXME: No way to determine VarArgs convention.
            CallingConventions flags = CallingConventions.Standard;
            ReflectorFlagsUtils.AddFlagIfTrue(ref flags, CallingConventions.HasThis, !functionHandle.IsShared);
            return flags;
        }

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
        protected override IList<StaticGenericParameterWrapper> GetMethodGenericParameters(StaticMethodWrapper method)
        {
            // FIXME: Not supported.
            return EmptyArray<StaticGenericParameterWrapper>.Instance;
        }

        protected override StaticParameterWrapper GetMethodReturnParameter(StaticMethodWrapper method)
        {
            CodeFunction2 methodHandle = (CodeFunction2)method.Handle;

            // TODO: This won't provide access to any parameter attributes.  How should we retrieve them?
            CodeTypeRef type = methodHandle.Type;
            return type != null ? new StaticParameterWrapper(this, type, method) : null;
        }
        #endregion

        #region Parameters
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

        protected override IEnumerable<StaticAttributeWrapper> GetParameterCustomAttributes(StaticParameterWrapper parameter)
        {
            if (parameter.Handle is CodeTypeRef)
                return EmptyArray<StaticAttributeWrapper>.Instance;

            CodeParameter2 parameterHandle = (CodeParameter2)parameter.Handle;
            return WrapAttributes(parameterHandle.Attributes);
        }

        protected override string GetParameterName(StaticParameterWrapper parameter)
        {
            if (parameter.Handle is CodeTypeRef)
                return null;

            CodeParameter2 parameterHandle = (CodeParameter2)parameter.Handle;
            return parameterHandle.Name;
        }

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
        protected override TypeAttributes GetTypeAttributes(StaticDeclaredTypeWrapper type)
        {
            CodeType typeHandle = (CodeType)type.Handle;

            TypeAttributes flags = 0;
            ReflectorFlagsUtils.AddFlagIfTrue(ref flags, TypeAttributes.Interface, typeHandle.Kind == vsCMElement.vsCMElementInterface);

            //TODO ReflectorFlagsUtils.AddFlagIfTrue(ref flags, TypeAttributes.Abstract, typeHandle.);
            //TODO ReflectorFlagsUtils.AddFlagIfTrue(ref flags, TypeAttributes.Sealed, modifiers.IsSealed);

            bool isNested = GetContainingType((CodeElement)typeHandle) != null;

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

        protected override StaticAssemblyWrapper GetTypeAssembly(StaticDeclaredTypeWrapper type)
        {
            CodeType typeHandle = (CodeType)type.Handle;
            throw new NotImplementedException();
            // TODO: return new StaticAssemblyWrapper(this, typeHandle.);
        }

        protected override string GetTypeNamespace(StaticDeclaredTypeWrapper type)
        {
            CodeType typeHandle = (CodeType)type.Handle;
            return typeHandle.Namespace.FullName;
        }

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

        protected override IList<StaticGenericParameterWrapper> GetTypeGenericParameters(StaticDeclaredTypeWrapper type)
        {
            // FIXME: Not supported.
            return EmptyArray<StaticGenericParameterWrapper>.Instance;
        }

        protected override IEnumerable<StaticConstructorWrapper> GetTypeConstructors(StaticDeclaredTypeWrapper type)
        {
            CodeType typeHandle = (CodeType)type.Handle;

            foreach (CodeElement candidate in typeHandle.Members)
            {
                if (candidate.Kind == vsCMElement.vsCMElementFunction)
                {
                    CodeFunction2 function = (CodeFunction2)typeHandle;
                    if (function.FunctionKind == vsCMFunction.vsCMFunctionConstructor)
                        yield return new StaticConstructorWrapper(this, function, type);
                }
            }
        }

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
            throw new NotImplementedException();
        }

        private StaticTypeWrapper MakeType(CodeTypeRef2 typeHandle)
        {
            throw new NotImplementedException();
        }

        private StaticTypeWrapper MakeTypeWithoutSubstitution(CodeType typeElementHandle)
        {
            throw new NotImplementedException();
        }

        private StaticDeclaredTypeWrapper MakeDeclaredTypeWithoutSubstitution(CodeType typeElementHandle)
        {
            throw new NotImplementedException();
        }

        private StaticDeclaredTypeWrapper MakeDeclaredType(CodeType typeHandle)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region Generic Parameters
        protected override GenericParameterAttributes GetGenericParameterAttributes(StaticGenericParameterWrapper genericParameter)
        {
            throw new NotImplementedException();
        }

        protected override int GetGenericParameterPosition(StaticGenericParameterWrapper genericParameter)
        {
            throw new NotImplementedException();
        }

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
                result[i] = converter((TInput)codeElements.Item(0));

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
