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
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Gallio.Common.Collections;
using Gallio.Common;
using Gallio.Common.Reflection;
using Gallio.Common.Reflection.Impl;
using System.IO;
using Mono.Cecil;
using Mono.Cecil.Metadata;
using EventAttributes=System.Reflection.EventAttributes;
using FieldAttributes=System.Reflection.FieldAttributes;
using GenericParameterAttributes=System.Reflection.GenericParameterAttributes;
using MethodAttributes=System.Reflection.MethodAttributes;
using ParameterAttributes=System.Reflection.ParameterAttributes;
using PropertyAttributes=System.Reflection.PropertyAttributes;
using TypeAttributes=System.Reflection.TypeAttributes;

namespace Gallio.Common.Reflection.Impl
{
    // Uses Cecil to open up assembly files.
    //
    // TODO: There are bugs due to incomplete support for attributes in Cecil and to a few other miscellaneous
    //       unimplemented features here.  Looks for "fixme" comments.
    //       Also see: https://bugzilla.novell.com/show_bug.cgi?id=325481
    internal sealed class CecilReflectionPolicy : StaticReflectionPolicy
    {
        private static readonly Key<string> PathAnnotationKey = new Key<string>("Path");

        private KeyedMemoizer<AssemblyDefinition, StaticAssemblyWrapper> assemblyMemoizer = new KeyedMemoizer<AssemblyDefinition, StaticAssemblyWrapper>();
        private KeyedMemoizer<TypeReference, StaticTypeWrapper> typeMemoizer = new KeyedMemoizer<TypeReference, StaticTypeWrapper>();
        private KeyedMemoizer<TypeDefinition, StaticDeclaredTypeWrapper> typeWithoutSubstitutionMemoizer = new KeyedMemoizer<TypeDefinition, StaticDeclaredTypeWrapper>();

        private readonly CustomAssemblyResolver assemblyResolver = new CustomAssemblyResolver();
        private IDebugSymbolResolver symbolResolver;

        public void AddHintDirectory(string path)
        {
            assemblyResolver.AddHintDirectory(path);
        }

        #region Wrapping
        private StaticAssemblyWrapper Wrap(AssemblyDefinition target)
        {
            return target != null
                ? assemblyMemoizer.Memoize(target, () => new StaticAssemblyWrapper(this, target))
                : null;
        }

        private StaticMethodWrapper WrapAccessor(MethodDefinition accessorHandle, StaticMemberWrapper member)
        {
            return accessorHandle != null ? new StaticMethodWrapper(this, accessorHandle, member.DeclaringType, member.ReflectedType, member.Substitution) : null;
        }

        private StaticConstructorWrapper WrapConstructor(MethodReference methodRefHandle)
        {
            StaticDeclaredTypeWrapper declaringType = MakeDeclaredType(methodRefHandle.DeclaringType);
            TypeDefinition declaringTypeDefnHandle = (TypeDefinition)declaringType.Handle;
            MethodDefinition methodDefnHandle = FindMatchingMethod(methodRefHandle.GetOriginalMethod(), declaringTypeDefnHandle.Constructors);
            return new StaticConstructorWrapper(this, methodDefnHandle, declaringType);
        }

        private StaticMethodWrapper WrapMethod(MethodReference methodRefHandle)
        {
            StaticDeclaredTypeWrapper declaringType = MakeDeclaredType(methodRefHandle.DeclaringType);
            TypeDefinition declaringTypeDefnHandle = (TypeDefinition)declaringType.Handle;
            MethodDefinition methodDefnHandle = FindMatchingMethod(methodRefHandle.GetOriginalMethod(), declaringTypeDefnHandle.Methods);
            StaticMethodWrapper method = new StaticMethodWrapper(this, methodDefnHandle, declaringType, declaringType, declaringType.Substitution);

            GenericInstanceMethod genericInstance = methodRefHandle as GenericInstanceMethod;
            if (genericInstance != null)
            {
                method = method.MakeGenericMethod(CollectionUtils.ConvertAllToArray<TypeReference, ITypeInfo>(genericInstance.GenericArguments,
                    delegate(TypeReference genericArgumentHandle) { return MakeType(genericArgumentHandle); }));
            }

            return method;
        }

        private MethodDefinition FindMatchingMethod(MethodReference methodRefHandle, IList methodDefnHandles)
        {
            MethodDefinition def = CollectionUtils.Find<MethodDefinition>(methodDefnHandles, delegate(MethodDefinition methodDefnHandle)
            {
                return IsMatchingMethod(methodDefnHandle, methodRefHandle);
            });

            if (def == null)
                throw new MissingMethodException("Could not find method definition.");
            return def;
        }

        private bool IsMatchingMethod(MethodReference a, MethodReference b)
        {
            if (a.Name != b.Name)
                return false;

            GenericParameterCollection aGenericParams = a.GenericParameters;
            GenericParameterCollection bGenericParams = b.GenericParameters;
            if (aGenericParams.Count != bGenericParams.Count)
                return false;

            ParameterDefinitionCollection aParams = a.Parameters;
            ParameterDefinitionCollection bParams = b.Parameters;
            if (aParams.Count != bParams.Count)
                return false;

            for (int i = 0; i < aGenericParams.Count; i++)
                if (!IsMatchingGenericParameter(a.GenericParameters[i], b.GenericParameters[i]))
                    return false;

            for (int i = 0; i < aParams.Count; i++)
                if (! IsMatchingParameter(a.Parameters[i], b.Parameters[i]))
                    return false;

            return true;
        }

        private bool IsMatchingGenericParameter(GenericParameter a, GenericParameter b)
        {
            // FIXME: Imprecise.  Should consider parameter constraints.
            return true;
        }

        private bool IsMatchingParameter(ParameterDefinition a, ParameterDefinition b)
        {
            return IsMatchingType(a.ParameterType, b.ParameterType);
        }

        private bool IsMatchingType(TypeReference a, TypeReference b)
        {
            // FIXME: Imprecise.
            return a.FullName == b.FullName;
        }

        #endregion

        #region Assemblies
        protected override IAssemblyInfo LoadAssemblyImpl(AssemblyName assemblyName)
        {
            return Wrap(assemblyResolver.Resolve(AssemblyNameReference.Parse(assemblyName.FullName)));
        }

        protected override IAssemblyInfo LoadAssemblyFromImpl(string assemblyFile)
        {
            return Wrap(assemblyResolver.LoadAssembly(assemblyFile));
        }

        protected internal override IEnumerable<StaticAttributeWrapper> GetAssemblyCustomAttributes(StaticAssemblyWrapper assembly)
        {
            AssemblyDefinition assemblyHandle = (AssemblyDefinition)assembly.Handle;
            return EnumerateAttributes(assemblyHandle.CustomAttributes);
        }

        protected internal override AssemblyName GetAssemblyName(StaticAssemblyWrapper assembly)
        {
            AssemblyDefinition assemblyHandle = (AssemblyDefinition)assembly.Handle;
            return new AssemblyName(assemblyHandle.Name.FullName);
        }

        protected internal override string GetAssemblyPath(StaticAssemblyWrapper assembly)
        {
            AssemblyDefinition assemblyHandle = (AssemblyDefinition)assembly.Handle;
            return assemblyResolver.GetAssemblyPath(assemblyHandle);
        }

        protected internal override IList<AssemblyName> GetAssemblyReferences(StaticAssemblyWrapper assembly)
        {
            List<AssemblyName> assemblyNames = new List<AssemblyName>();
            AssemblyDefinition assemblyHandle = (AssemblyDefinition)assembly.Handle;
            foreach (ModuleDefinition moduleHandle in assemblyHandle.Modules)
                foreach (AssemblyNameReference assemblyNameRef in moduleHandle.AssemblyReferences)
                    assemblyNames.Add(new AssemblyName(assemblyNameRef.FullName));
            return assemblyNames;
        }

        protected internal override IList<StaticDeclaredTypeWrapper> GetAssemblyExportedTypes(StaticAssemblyWrapper assembly)
        {
            AssemblyDefinition assemblyHandle = (AssemblyDefinition)assembly.Handle;
            List<StaticDeclaredTypeWrapper> types = new List<StaticDeclaredTypeWrapper>();
            foreach (TypeDefinition typeHandle in EnumerateAssemblyTypeDefinitions(assemblyHandle))
                if (typeHandle.IsPublic || typeHandle.IsNestedPublic)
                    types.Add(MakeDeclaredTypeWithoutSubstitution(typeHandle));

            return types;
        }

        protected internal override IList<StaticDeclaredTypeWrapper> GetAssemblyTypes(StaticAssemblyWrapper assembly)
        {
            AssemblyDefinition assemblyHandle = (AssemblyDefinition)assembly.Handle;
            List<StaticDeclaredTypeWrapper> types = new List<StaticDeclaredTypeWrapper>();
            foreach (TypeDefinition typeHandle in EnumerateAssemblyTypeDefinitions(assemblyHandle))
                types.Add(MakeDeclaredTypeWithoutSubstitution(typeHandle));

            return types;
        }

        protected internal override StaticDeclaredTypeWrapper GetAssemblyType(StaticAssemblyWrapper assembly, string typeName)
        {
            string translatedTypeName = typeName.Replace('+', '/');

            AssemblyDefinition assemblyHandle = (AssemblyDefinition)assembly.Handle;
            foreach (ModuleDefinition moduleHandle in assemblyHandle.Modules)
            {
                TypeDefinition typeHandle = moduleHandle.Types[translatedTypeName];
                if (typeHandle != null)
                    return MakeDeclaredTypeWithoutSubstitution(typeHandle);
            }

            return null;
        }

        private static IEnumerable<TypeDefinition> EnumerateAssemblyTypeDefinitions(AssemblyDefinition assemblyHandle)
        {
            foreach (ModuleDefinition moduleHandle in assemblyHandle.Modules)
            {
                foreach (TypeDefinition typeHandle in moduleHandle.Types)
                {
                    if (! typeHandle.Name.StartsWith("<")) // exclude types like <Module> and <PrivateImplementationDetails>
                        yield return typeHandle;
                }
            }
        }
        #endregion

        #region Attributes
        protected internal override StaticConstructorWrapper GetAttributeConstructor(StaticAttributeWrapper attribute)
        {
            CustomAttribute attributeHandle = (CustomAttribute)attribute.Handle;
            return WrapConstructor(attributeHandle.Constructor);
        }

        protected internal override ConstantValue[] GetAttributeConstructorArguments(StaticAttributeWrapper attribute)
        {
            CustomAttribute attributeHandle = (CustomAttribute)attribute.Handle;
            return CollectionUtils.ConvertAllToArray<Mono.Cecil.ConstantValue, ConstantValue>(attributeHandle.ConstructorParameters, ConvertConstantValue);
        }

        protected internal override IEnumerable<KeyValuePair<StaticFieldWrapper, ConstantValue>> GetAttributeFieldArguments(StaticAttributeWrapper attribute)
        {
            CustomAttribute attributeHandle = (CustomAttribute)attribute.Handle;
            StaticDeclaredTypeWrapper declaringType = GetAttributeType(attributeHandle);

            foreach (DictionaryEntry entry in attributeHandle.Fields)
            {
                string fieldName = (string) entry.Key;
                ConstantValue value = ConvertConstantValue((Mono.Cecil.ConstantValue)entry.Value);
                StaticFieldWrapper field = (StaticFieldWrapper) declaringType.GetField(fieldName, BindingFlags.Public | BindingFlags.Instance);
                yield return new KeyValuePair<StaticFieldWrapper, ConstantValue>(field, value);
            }
        }

        protected internal override IEnumerable<KeyValuePair<StaticPropertyWrapper, ConstantValue>> GetAttributePropertyArguments(StaticAttributeWrapper attribute)
        {
            CustomAttribute attributeHandle = (CustomAttribute)attribute.Handle;
            StaticDeclaredTypeWrapper declaringType = GetAttributeType(attributeHandle);

            foreach (DictionaryEntry entry in attributeHandle.Properties)
            {
                string propertyName = (string)entry.Key;
                ConstantValue value = ConvertConstantValue((Mono.Cecil.ConstantValue)entry.Value);
                StaticPropertyWrapper field = (StaticPropertyWrapper)declaringType.GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance);
                yield return new KeyValuePair<StaticPropertyWrapper, ConstantValue>(field, value);
            }
        }

        private IEnumerable<StaticAttributeWrapper> EnumerateAttributes(CustomAttributeCollection attributeHandles)
        {
            foreach (CustomAttribute attributeHandle in attributeHandles)
            {
                if (attributeHandle.Resolve())
                    yield return new StaticAttributeWrapper(this, attributeHandle);
            }
        }

        private StaticDeclaredTypeWrapper GetAttributeType(CustomAttribute attributeHandle)
        {
            return MakeDeclaredType(attributeHandle.Constructor.DeclaringType);
        }

        private ConstantValue ConvertConstantValue(Mono.Cecil.ConstantValue constant)
        {
            ITypeInfo type = MakeType(constant.Type);

            if (constant.Value != null)
            {
                TypeReference typeRef = constant.Value as TypeReference;
                if (typeRef != null)
                    return new ConstantValue(type, MakeType(typeRef));

                ArrayType arrayType = constant.Type as ArrayType;
                if (arrayType != null)
                {
                    Mono.Cecil.ConstantValue[] arrayConstants = (Mono.Cecil.ConstantValue[])constant.Value;
                    int length = arrayConstants.Length;

                    ConstantValue[] array = new ConstantValue[length];
                    for (int i = 0; i < length; i++)
                        array[i] = ConvertConstantValue(arrayConstants[i]);
                    return new ConstantValue(type, array);
                }
            }

            return new ConstantValue(type, constant.Value);
        }
        #endregion

        #region Members
        protected internal override IEnumerable<StaticAttributeWrapper> GetMemberCustomAttributes(StaticMemberWrapper member)
        {
            Mono.Cecil.ICustomAttributeProvider memberHandle = member.Handle as Mono.Cecil.ICustomAttributeProvider;
            if (memberHandle != null)
                return EnumerateAttributes(memberHandle.CustomAttributes);
            return EmptyArray<StaticAttributeWrapper>.Instance;
        }

        protected internal override string GetMemberName(StaticMemberWrapper member)
        {
            IMemberReference memberHandle = (IMemberReference)member.Handle;

            // Strip off generic parameter count from name.
            string name = memberHandle.Name;
            int tickPos = name.IndexOf('`');
            if (tickPos < 0)
                return name;
            return name.Substring(0, tickPos);
        }

        protected internal override CodeLocation GetMemberSourceLocation(StaticMemberWrapper member)
        {
            IMemberReference memberHandle = (IMemberReference) member.Handle;

            switch (memberHandle.MetadataToken.TokenType)
            {
                case TokenType.TypeDef:
                    return GuessTypeSourceLocation((TypeDefinition)memberHandle);

                case TokenType.Method:
                    return GetMethodSourceLocation((MethodDefinition) memberHandle);

                case TokenType.GenericParam:
                    IGenericParameterProvider owner = ((GenericParameter)memberHandle).Owner;
                    if (owner is TypeDefinition)
                        return GuessTypeSourceLocation((TypeDefinition)owner);
                    else if (owner is MethodDefinition)
                        return GetMethodSourceLocation((MethodDefinition) owner);
                    return CodeLocation.Unknown;

                default:
                    return GuessTypeSourceLocation((TypeDefinition)memberHandle.DeclaringType);
            }
        }

        private CodeLocation GuessTypeSourceLocation(TypeDefinition typeDefinition)
        {
            if (typeDefinition != null)
            {
                foreach (MethodDefinition methodDefinition in typeDefinition.Methods)
                {
                    CodeLocation location = GetMethodSourceLocation(methodDefinition);
                    if (location != CodeLocation.Unknown)
                        return new CodeLocation(location.Path, 0, 0);
                }
            }

            return CodeLocation.Unknown;
        }

        private CodeLocation GetMethodSourceLocation(MethodDefinition methodDefinition)
        {
            if (symbolResolver == null)
                symbolResolver = DebugSymbolUtils.CreateResolver();

            string assemblyPath = assemblyResolver.GetAssemblyPath(methodDefinition.DeclaringType.Module.Assembly);
            int metadataToken = (int) methodDefinition.MetadataToken.ToUInt();
            return symbolResolver.GetSourceLocationForMethod(assemblyPath, metadataToken);
        }

        #endregion

        #region Events
        protected internal override EventAttributes GetEventAttributes(StaticEventWrapper @event)
        {
            EventDefinition eventHandle = (EventDefinition)@event.Handle;
            return (EventAttributes)eventHandle.Attributes;
        }

        protected internal override StaticMethodWrapper GetEventAddMethod(StaticEventWrapper @event)
        {
            EventDefinition eventHandle = (EventDefinition)@event.Handle;
            return WrapAccessor(eventHandle.AddMethod, @event);
        }

        protected internal override StaticMethodWrapper GetEventRaiseMethod(StaticEventWrapper @event)
        {
            EventDefinition eventHandle = (EventDefinition)@event.Handle;
            return WrapAccessor(eventHandle.InvokeMethod, @event);
        }

        protected internal override StaticMethodWrapper GetEventRemoveMethod(StaticEventWrapper @event)
        {
            EventDefinition eventHandle = (EventDefinition)@event.Handle;
            return WrapAccessor(eventHandle.RemoveMethod, @event);
        }

        protected internal override StaticTypeWrapper GetEventHandlerType(StaticEventWrapper @event)
        {
            EventDefinition eventHandle = (EventDefinition)@event.Handle;
            return MakeType(eventHandle.EventType);
        }
        #endregion

        #region Fields
        protected internal override FieldAttributes GetFieldAttributes(StaticFieldWrapper field)
        {
            FieldDefinition fieldHandle = (FieldDefinition)field.Handle;
            return (FieldAttributes)fieldHandle.Attributes;
        }

        protected internal override StaticTypeWrapper GetFieldType(StaticFieldWrapper field)
        {
            FieldDefinition fieldHandle = (FieldDefinition)field.Handle;
            return MakeType(fieldHandle.FieldType);
        }
        #endregion

        #region Properties
        protected internal override PropertyAttributes GetPropertyAttributes(StaticPropertyWrapper property)
        {
            PropertyDefinition propertyHandle = (PropertyDefinition)property.Handle;
            return (PropertyAttributes)propertyHandle.Attributes;
        }

        protected internal override StaticTypeWrapper GetPropertyType(StaticPropertyWrapper property)
        {
            PropertyDefinition propertyHandle = (PropertyDefinition)property.Handle;
            return MakeType(propertyHandle.PropertyType);
        }

        protected internal override StaticMethodWrapper GetPropertyGetMethod(StaticPropertyWrapper property)
        {
            PropertyDefinition propertyHandle = (PropertyDefinition)property.Handle;
            return WrapAccessor(propertyHandle.GetMethod, property);
        }

        protected internal override StaticMethodWrapper GetPropertySetMethod(StaticPropertyWrapper property)
        {
            PropertyDefinition propertyHandle = (PropertyDefinition)property.Handle;
            return WrapAccessor(propertyHandle.SetMethod, property);
        }
        #endregion

        #region Functions
        protected internal override MethodAttributes GetFunctionAttributes(StaticFunctionWrapper function)
        {
            MethodDefinition methodHandle = (MethodDefinition)function.Handle;
            return (MethodAttributes)methodHandle.Attributes;
        }

        protected internal override CallingConventions GetFunctionCallingConvention(StaticFunctionWrapper function)
        {
            MethodDefinition methodHandle = (MethodDefinition)function.Handle;
            CallingConventions flags = 0;
            ReflectorFlagsUtils.AddFlagIfTrue(ref flags, CallingConventions.HasThis, methodHandle.HasThis);
            ReflectorFlagsUtils.AddFlagIfTrue(ref flags, CallingConventions.ExplicitThis, methodHandle.ExplicitThis);
            
            switch (methodHandle.CallingConvention)
            {
                case MethodCallingConvention.VarArg:
                    flags |= CallingConventions.VarArgs;
                    break;
                case MethodCallingConvention.ThisCall:
                    break;
                default:
                    flags |= CallingConventions.Standard;
                    break;
            }

            return flags;
        }

        protected internal override IList<StaticParameterWrapper> GetFunctionParameters(StaticFunctionWrapper function)
        {
            MethodDefinition methodHandle = (MethodDefinition)function.Handle;
            return CollectionUtils.ConvertAllToArray<ParameterDefinition, StaticParameterWrapper>(methodHandle.Parameters, delegate(ParameterDefinition parameter)
            {
                return new StaticParameterWrapper(this, parameter, function);
            });
        }
        #endregion

        #region Methods
        protected internal override StaticParameterWrapper GetMethodReturnParameter(StaticMethodWrapper method)
        {
            MethodDefinition methodHandle = (MethodDefinition)method.Handle;
            return new StaticParameterWrapper(this, methodHandle.ReturnType, method);
        }

        protected internal override IList<StaticGenericParameterWrapper> GetMethodGenericParameters(StaticMethodWrapper method)
        {
            MethodDefinition methodHandle = (MethodDefinition)method.Handle;
            return CollectionUtils.ConvertAllToArray<GenericParameter, StaticGenericParameterWrapper>(methodHandle.GenericParameters, delegate(GenericParameter parameterHandle)
            {
                return StaticGenericParameterWrapper.CreateGenericMethodParameter(this, parameterHandle, method);
            });
        }
        #endregion

        #region Parameters
        protected internal override ParameterAttributes GetParameterAttributes(StaticParameterWrapper parameter)
        {
            ParameterDefinition parameterHandle = parameter.Handle as ParameterDefinition;
            if (parameterHandle != null)
                return (ParameterAttributes)parameterHandle.Attributes;

            MethodReturnType returnTypeHandle = (MethodReturnType) parameter.Handle;
            return returnTypeHandle.HasConstant ? ParameterAttributes.HasDefault : ParameterAttributes.None;
        }

        protected internal override IEnumerable<StaticAttributeWrapper> GetParameterCustomAttributes(StaticParameterWrapper parameter)
        {
            ParameterDefinition parameterHandle = parameter.Handle as ParameterDefinition;
            if (parameterHandle != null)
                return EnumerateAttributes(parameterHandle.CustomAttributes);

            MethodReturnType returnTypeHandle = (MethodReturnType)parameter.Handle;
            return EnumerateAttributes(returnTypeHandle.CustomAttributes);
        }

        protected internal override string GetParameterName(StaticParameterWrapper parameter)
        {
            ParameterDefinition parameterHandle = parameter.Handle as ParameterDefinition;
            if (parameterHandle != null)
                return parameterHandle.Name;

            return null;
        }

        protected internal override int GetParameterPosition(StaticParameterWrapper parameter)
        {
            ParameterDefinition parameterHandle = parameter.Handle as ParameterDefinition;
            if (parameterHandle != null)
                return parameterHandle.Method.Parameters.IndexOf(parameterHandle);

            return -1;
        }

        protected internal override StaticTypeWrapper GetParameterType(StaticParameterWrapper parameter)
        {
            ParameterDefinition parameterHandle = parameter.Handle as ParameterDefinition;
            if (parameterHandle != null)
                return MakeType(parameterHandle.ParameterType);

            MethodReturnType returnTypeHandle = (MethodReturnType)parameter.Handle;
            return MakeType(returnTypeHandle.ReturnType);
        }
        #endregion

        #region Types
        protected internal override TypeAttributes GetTypeAttributes(StaticDeclaredTypeWrapper type)
        {
            TypeDefinition typehandle = (TypeDefinition)type.Handle;
            return (TypeAttributes)typehandle.Attributes;
        }

        protected internal override IList<StaticGenericParameterWrapper> GetTypeGenericParameters(StaticDeclaredTypeWrapper type)
        {
            TypeDefinition typeHandle = (TypeDefinition)type.Handle;
            return CollectionUtils.ConvertAllToArray<GenericParameter, StaticGenericParameterWrapper>(typeHandle.GenericParameters, delegate(GenericParameter parameterHandle)
            {
                return StaticGenericParameterWrapper.CreateGenericTypeParameter(this, parameterHandle, type);
            });
        }

        protected internal override StaticAssemblyWrapper GetTypeAssembly(StaticDeclaredTypeWrapper type)
        {
            TypeDefinition typeHandle = (TypeDefinition)type.Handle;
            return Wrap(typeHandle.Module.Assembly);
        }

        protected internal override string GetTypeNamespace(StaticDeclaredTypeWrapper type)
        {
            TypeReference typeHandle = (TypeDefinition)type.Handle;
            while (typeHandle.DeclaringType != null)
                typeHandle = typeHandle.DeclaringType;

            return typeHandle.Namespace;
        }

        protected internal override StaticDeclaredTypeWrapper GetTypeBaseType(StaticDeclaredTypeWrapper type)
        {
            TypeDefinition typeHandle = (TypeDefinition)type.Handle;
            TypeReference baseTypeHandle = typeHandle.BaseType;
            return baseTypeHandle != null ? MakeDeclaredType(baseTypeHandle) : null;
        }

        protected internal override IList<StaticDeclaredTypeWrapper> GetTypeInterfaces(StaticDeclaredTypeWrapper type)
        {
            TypeDefinition typeHandle = (TypeDefinition)type.Handle;
            return CollectionUtils.ConvertAllToArray<TypeReference, StaticDeclaredTypeWrapper>(typeHandle.Interfaces, MakeDeclaredType);
        }

        protected internal override IEnumerable<StaticConstructorWrapper> GetTypeConstructors(StaticDeclaredTypeWrapper type)
        {
            TypeDefinition typeHandle = (TypeDefinition)type.Handle;

            foreach (MethodDefinition methodHandle in typeHandle.Constructors)
                yield return new StaticConstructorWrapper(this, methodHandle, type);
        }

        protected internal override IEnumerable<StaticMethodWrapper> GetTypeMethods(StaticDeclaredTypeWrapper type,
            StaticDeclaredTypeWrapper reflectedType)
        {
            TypeDefinition typeHandle = (TypeDefinition)type.Handle;

            foreach (MethodDefinition methodHandle in typeHandle.Methods)
                yield return new StaticMethodWrapper(this, methodHandle, type, reflectedType, type.Substitution);
        }

        protected internal override IEnumerable<StaticPropertyWrapper> GetTypeProperties(StaticDeclaredTypeWrapper type,
            StaticDeclaredTypeWrapper reflectedType)
        {
            TypeDefinition typeHandle = (TypeDefinition)type.Handle;

            foreach (PropertyDefinition propertyHandle in typeHandle.Properties)
                yield return new StaticPropertyWrapper(this, propertyHandle, type, reflectedType);
        }

        protected internal override IEnumerable<StaticFieldWrapper> GetTypeFields(StaticDeclaredTypeWrapper type,
            StaticDeclaredTypeWrapper reflectedType)
        {
            TypeDefinition typeHandle = (TypeDefinition)type.Handle;

            foreach (FieldDefinition fieldHandle in typeHandle.Fields)
                yield return new StaticFieldWrapper(this, fieldHandle, type, reflectedType);
        }

        protected internal override IEnumerable<StaticEventWrapper> GetTypeEvents(StaticDeclaredTypeWrapper type,
            StaticDeclaredTypeWrapper reflectedType)
        {
            TypeDefinition typeHandle = (TypeDefinition)type.Handle;

            foreach (EventDefinition eventHandle in typeHandle.Events)
                yield return new StaticEventWrapper(this, eventHandle, type, reflectedType);
        }

        protected internal override IEnumerable<StaticTypeWrapper> GetTypeNestedTypes(StaticDeclaredTypeWrapper type)
        {
            TypeDefinition typeHandle = (TypeDefinition)type.Handle;

            foreach (TypeDefinition nestedTypeHandle in typeHandle.NestedTypes)
                yield return new StaticDeclaredTypeWrapper(this, nestedTypeHandle, type, type.Substitution);
        }

        private StaticTypeWrapper MakeType(TypeReference typeHandle)
        {
            return typeMemoizer.Memoize(typeHandle, () =>
            {
                TypeDefinition declaredTypeHandle = typeHandle as TypeDefinition;
                if (declaredTypeHandle != null)
                    return MakeDeclaredTypeWithoutSubstitution(declaredTypeHandle);

                GenericParameter genericParameterHandle = typeHandle as GenericParameter;
                if (genericParameterHandle != null)
                    return MakeGenericParameter(genericParameterHandle);

                ArrayType arrayTypeHandle = typeHandle as ArrayType;
                if (arrayTypeHandle != null)
                    return MakeArrayType(arrayTypeHandle);

                PointerType pointerTypeHandle = typeHandle as PointerType;
                if (pointerTypeHandle != null)
                    return MakePointerType(pointerTypeHandle);

                ReferenceType referenceTypeHandle = typeHandle as ReferenceType;
                if (referenceTypeHandle != null)
                    return MakeByRefType(referenceTypeHandle);

                GenericInstanceType genericInstanceTypeHandle = typeHandle as GenericInstanceType;
                if (genericInstanceTypeHandle != null)
                    return MakeGenericInstanceType(genericInstanceTypeHandle);

                AssemblyNameReference assemblyRef = typeHandle.Scope as AssemblyNameReference;
                if (assemblyRef != null)
                {
                    AssemblyDefinition assemblyDefn = assemblyResolver.Resolve(assemblyRef);
                    foreach (ModuleDefinition moduleDefn in assemblyDefn.Modules)
                    {
                        TypeDefinition typeDefn = moduleDefn.Types[typeHandle.FullName];
                        if (typeDefn != null)
                            return MakeDeclaredType(typeDefn);
                    }
                }

                throw new NotSupportedException("Unsupported type: " + typeHandle);
            });
        }

        private StaticDeclaredTypeWrapper MakeDeclaredTypeWithoutSubstitution(TypeDefinition typeHandle)
        {
            return typeWithoutSubstitutionMemoizer.Memoize(typeHandle, () =>
            {
                StaticDeclaredTypeWrapper declaringType = typeHandle.DeclaringType != null
                    ? MakeDeclaredType(typeHandle.DeclaringType)
                    : null;
                return new StaticDeclaredTypeWrapper(this, typeHandle, declaringType, StaticTypeSubstitution.Empty);
            });
        }

        private StaticDeclaredTypeWrapper MakeDeclaredType(TypeReference typeHandle)
        {
            return (StaticDeclaredTypeWrapper)MakeType(typeHandle);
        }

        private StaticArrayTypeWrapper MakeArrayType(ArrayType arrayTypeHandle)
        {
            return MakeType(arrayTypeHandle.ElementType).MakeArrayType(arrayTypeHandle.Rank);
        }

        private StaticPointerTypeWrapper MakePointerType(PointerType pointerTypeHandle)
        {
            return MakeType(pointerTypeHandle.ElementType).MakePointerType();
        }

        private StaticByRefTypeWrapper MakeByRefType(ReferenceType referenceTypeHandle)
        {
            return MakeType(referenceTypeHandle.ElementType).MakeByRefType();
        }

        private StaticGenericParameterWrapper MakeGenericParameter(GenericParameter parameterHandle)
        {
            TypeReference typeHandle = parameterHandle.Owner as TypeReference;
            if (typeHandle != null)
            {
                StaticDeclaredTypeWrapper declaringType = MakeDeclaredType(typeHandle);
                return StaticGenericParameterWrapper.CreateGenericTypeParameter(this, parameterHandle, declaringType);
            }
            else
            {
                MethodReference methodHandle = (MethodReference)parameterHandle.Owner;
                StaticMethodWrapper declaringMethod = WrapMethod(methodHandle);
                return StaticGenericParameterWrapper.CreateGenericMethodParameter(this, parameterHandle, declaringMethod);
            }
        }

        private StaticTypeWrapper MakeGenericInstanceType(GenericInstanceType typeHandle)
        {
            StaticDeclaredTypeWrapper nonGenericType = MakeDeclaredType(typeHandle.ElementType);
            IList<ITypeInfo> genericArguments = CollectionUtils.ConvertAllToArray<TypeReference, StaticTypeWrapper>(typeHandle.GenericArguments, MakeType);
            return nonGenericType.MakeGenericType(genericArguments);
        }
        #endregion

        #region Generic Parameters
        protected internal override GenericParameterAttributes GetGenericParameterAttributes(StaticGenericParameterWrapper genericParameter)
        {
            GenericParameter parameterHandle = (GenericParameter)genericParameter.Handle;
            return (GenericParameterAttributes)parameterHandle.Attributes;
        }

        protected internal override int GetGenericParameterPosition(StaticGenericParameterWrapper genericParameter)
        {
            GenericParameter parameterHandle = (GenericParameter)genericParameter.Handle;
            return parameterHandle.Position;
        }

        protected internal override IList<StaticTypeWrapper> GetGenericParameterConstraints(StaticGenericParameterWrapper genericParameter)
        {
            GenericParameter parameterHandle = (GenericParameter)genericParameter.Handle;
            return CollectionUtils.ConvertAllToArray<TypeReference, StaticTypeWrapper>(parameterHandle.Constraints, MakeType);
        }
        #endregion

        #region Misc
        private static bool TryGetAnnotation<T>(IAnnotationProvider annotationProvider, Key<T> key, out T value)
        {
            if (annotationProvider.Annotations.Contains(key.Name))
            {
                value = (T) annotationProvider.Annotations[key.Name];
                return true;
            }

            value = default(T);
            return false;
        }

        private static void SetAnnotation<T>(IAnnotationProvider annotationProvider, Key<T> key, T value)
        {
            annotationProvider.Annotations[key.Name] = value;
        }
        #endregion

        internal sealed class CustomAssemblyResolver : IAssemblyResolver
        {
            private static readonly string[] extensions = new string[] { ".dll", ".exe" };

            private readonly List<string> hintDirectories = new List<string>();
            private readonly Dictionary<string, AssemblyDefinition> cache = new Dictionary<string, AssemblyDefinition>();

            public void AddHintDirectory(string path)
            {
                hintDirectories.Add(path);
            }

            public string GetAssemblyPath(AssemblyDefinition assemblyDefn)
            {
                string path;
                if (!TryGetAnnotation(assemblyDefn, PathAnnotationKey, out path))
                    throw new InvalidOperationException("The assembly path is not available.");
                return path;
            }

            public AssemblyDefinition LoadAssembly(string assemblyFile)
            {
                assemblyFile = Path.GetFullPath(assemblyFile);

                AssemblyDefinition assemblyDefn;
                if (!cache.TryGetValue(assemblyFile, out assemblyDefn))
                {
                    assemblyDefn = AssemblyFactory.GetAssembly(assemblyFile);
                    assemblyDefn.Resolver = this;
                    SetAnnotation(assemblyDefn, PathAnnotationKey, assemblyFile);

                    cache[assemblyFile] = assemblyDefn;
                    cache[assemblyDefn.Name.FullName] = assemblyDefn;
                    cache[assemblyDefn.Name.Name] = assemblyDefn;
                }

                return assemblyDefn;
            }

            public AssemblyDefinition Resolve(string fullName)
            {
                return Resolve(AssemblyNameReference.Parse(fullName));
            }

            public AssemblyDefinition Resolve(AssemblyNameReference name)
            {
                AssemblyDefinition assemblyDefn;
                if (!cache.TryGetValue(name.FullName, out assemblyDefn))
                {
                    string assemblyFile = FindAssemblyByPartialName(name.Name);
                    if (assemblyFile == null)
                    {
                        Assembly assembly = Assembly.ReflectionOnlyLoad(name.FullName);
                        if (assembly != null)
                            assemblyFile = AssemblyUtils.GetAssemblyLocalPath(assembly);
                        else
                            throw new FileNotFoundException(String.Format("Could not find assembly '{0}'.", name.FullName));
                    }

                    assemblyDefn = LoadAssembly(assemblyFile);
                    cache[name.FullName] = assemblyDefn; // ensure this exact variation of the full name is cached too
                }

                return assemblyDefn;
            }

            private string FindAssemblyByPartialName(string partialName)
            {
                foreach (string path in hintDirectories)
                {
                    foreach (string extension in extensions)
                    {
                        string assemblyFile = Path.Combine(path, partialName + extension);
                        if (File.Exists(assemblyFile))
                            return assemblyFile;
                    }
                }

                return null;
            }
        }

        /// <summary>
        /// Hack to provide access to the internal structure of Cecil attributes.
        /// The standard interface makes it impossible to extract arrays and types.
        /// </summary>
        private sealed class CustomAttrib
        {
        }
    }
}
