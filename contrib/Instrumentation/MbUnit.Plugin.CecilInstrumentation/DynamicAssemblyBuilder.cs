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
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Reflection.Emit;
using System.Security;
using MbUnit.Framework;
using MbUnit.Framework.Kernel.Utilities;
using Mono.Cecil;

namespace MbUnit.Plugin.CecilInstrumentation
{
    /// <summary>
    /// Builds dynamic assemblies from Cecil definitions.
    /// </summary>
    internal sealed class DynamicAssemblyBuilder
    {
        private delegate ILGenerator GetILGeneratorDelegate(int size);
        private delegate GenericTypeParameterBuilder[] DefineGenericParametersDelegate(string[] names);
        private delegate ParameterBuilder DefineParameterDelegate(int position,
            System.Reflection.ParameterAttributes attributes, string name);
        private delegate void AddDeclarativeSecurityDelegate(System.Security.Permissions.SecurityAction action,
            System.Security.PermissionSet perms);
        private delegate void SetCustomAttributeDelegate(CustomAttributeBuilder attributeBuilder);

        private readonly AssemblyDefinition assemblyDefinition;
        private readonly DynamicMethodBuilder dynamicMethodBuilder;

        private DynamicAssembly dynamicAssembly;

        private List<Block> resolvePass;
        private List<Block> metadataPass;
        private List<Block> ilPass;

        public DynamicAssemblyBuilder(AssemblyDefinition assemblyDefinition)
        {
            this.assemblyDefinition = assemblyDefinition;
            this.dynamicMethodBuilder = new DynamicMethodBuilder(this);
        }

        public DynamicAssembly Build(string instrumentedAssemblySavePath)
        {
            resolvePass = new List<Block>();
            metadataPass = new List<Block>();
            ilPass = new List<Block>();

            string dir = null, file = null;
            if (instrumentedAssemblySavePath != null)
            {
                dir = Path.GetDirectoryName(instrumentedAssemblySavePath);
                file = Path.GetFileName(instrumentedAssemblySavePath);
            }

            BuildAssembly(AppDomain.CurrentDomain, dir, file);

            // Do processing that depends on all types being declared.
            foreach (Block block in resolvePass)
                block();

            // Do processing that depends on all types being structurally complete
            // but possibly missing some metadata.
            foreach (Block block in metadataPass)
                block();

            // Do processing to initialize method IL.
            foreach (Block block in ilPass)
                block();

            // Create types.
            dynamicAssembly.CreateAllTypes();

            return dynamicAssembly;
        }

        private void BuildAssembly(AppDomain appDomain, string dir, string filename)
        {
            // Prepare the assembly-level permissions.
            PermissionSet requiredPermissions = new PermissionSet(null);
            PermissionSet optionalPermissions = new PermissionSet(null);
            PermissionSet refusedPermissions = new PermissionSet(null);

            foreach (SecurityDeclaration securityDeclaration in assemblyDefinition.SecurityDeclarations)
            {
                switch (securityDeclaration.Action)
                {
                    case Mono.Cecil.SecurityAction.RequestMinimum:
                        requiredPermissions = requiredPermissions.Union(securityDeclaration.PermissionSet);
                        break;
                    case Mono.Cecil.SecurityAction.RequestOptional:
                        optionalPermissions = optionalPermissions.Union(securityDeclaration.PermissionSet);
                        break;
                    case Mono.Cecil.SecurityAction.RequestRefuse:
                        refusedPermissions = refusedPermissions.Union(securityDeclaration.PermissionSet);
                        break;
                }
            }

            // Build the dynamic assembly.
            AssemblyBuilder assemblyBuilder = appDomain.DefineDynamicAssembly(
                new AssemblyName(assemblyDefinition.Name.FullName),
                AssemblyBuilderAccess.RunAndSave, dir,
                requiredPermissions, optionalPermissions, refusedPermissions);
            dynamicAssembly = new DynamicAssembly(assemblyBuilder, filename);

            // TODO: Set entry point and assembly kind.

            foreach (ModuleDefinition moduleDefinition in assemblyDefinition.Modules)
                BuildModule(moduleDefinition);

            metadataPass.Add(delegate
            {
                InitializeCustomAttributes(assemblyBuilder.SetCustomAttribute, assemblyDefinition.CustomAttributes);
            });
        }

        private void BuildModule(ModuleDefinition moduleDefinition)
        {
            DynamicModule module = dynamicAssembly.DefineDynamicModule(moduleDefinition.Name);
            // TODO: Set user entry point.

            foreach (Resource resource in moduleDefinition.Resources)
                BuildResource(module, resource);

            foreach (TypeDefinition typeDefinition in moduleDefinition.Types)
            {
                // FIXME: Just for now, don't build generic types and other incomplete stuff.
                //        I've spent a lot of time on it already and there are some hard problems
                //        lurking.  Right now I just want a proof of concept.  -- Jeff.
                if (!typeDefinition.Name.Contains("Sample"))
                    continue;

                BuildType(module, typeDefinition);
            }

            metadataPass.Add(delegate
            {
                InitializeCustomAttributes(module.Builder.SetCustomAttribute, moduleDefinition.CustomAttributes);
            });
        }

        private void BuildResource(DynamicModule module, Resource resource)
        {
            resource.Accept(new DynamicResourceBuilder(module));
        }

        private void BuildType(DynamicModule module, TypeDefinition typeDefinition)
        {
            if (typeDefinition.Name == "<Module>")
                return;

            DynamicType type = module.DefineType(typeDefinition.FullName,
                (System.Reflection.TypeAttributes)typeDefinition.Attributes, null,
                (System.Reflection.Emit.PackingSize)typeDefinition.PackingSize, (int)typeDefinition.ClassSize);

            InitializeType(type, typeDefinition);
        }

        private void BuildNestedType(DynamicType parentType, TypeDefinition typeDefinition)
        {
            DynamicType type = parentType.DefineNestedType(typeDefinition.Name,
                (System.Reflection.TypeAttributes)typeDefinition.Attributes, null, (int)typeDefinition.ClassSize);

            // TODO: How do we set PackingSize?

            InitializeType(type, typeDefinition);
        }

        private void InitializeType(DynamicType type, TypeDefinition typeDefinition)
        {
            foreach (TypeDefinition nestedTypeDefinition in typeDefinition.NestedTypes)
                BuildNestedType(type, nestedTypeDefinition);

            resolvePass.Add(delegate
            {
                if (typeDefinition.BaseType != null)
                    type.Builder.SetParent(ResolveType(typeDefinition.BaseType));

                InitializeGenericParameters(type.Builder.DefineGenericParameters, typeDefinition.GenericParameters);

                foreach (FieldDefinition fieldDefinition in typeDefinition.Fields)
                    BuildField(type, fieldDefinition);
                foreach (PropertyDefinition propertyDefinition in typeDefinition.Properties)
                    BuildProperty(type, propertyDefinition);
                foreach (EventDefinition eventDefinition in typeDefinition.Events)
                    BuildEvent(type, eventDefinition);
                foreach (MethodDefinition methodDefinition in typeDefinition.Methods)
                    BuildMethod(type, methodDefinition);
                foreach (MethodDefinition methodDefinition in typeDefinition.Constructors)
                    BuildConstructor(type, methodDefinition);
            });

            metadataPass.Add(delegate
            {
                InitializeDeclarativeSecurity(type.Builder.AddDeclarativeSecurity, typeDefinition.SecurityDeclarations);
                InitializeCustomAttributes(type.Builder.SetCustomAttribute, typeDefinition.CustomAttributes);
            });
        }

        private void BuildField(DynamicType type, FieldDefinition fieldDefinition)
        {
            Type fieldType = ResolveType(fieldDefinition.FieldType);
            FieldBuilder fieldBuilder = type.DefineField(fieldDefinition.Name,
                fieldType, (System.Reflection.FieldAttributes)fieldDefinition.Attributes);
            // TODO: Custom modifiers and other stuff.

            if (fieldDefinition.HasLayoutInfo)
                fieldBuilder.SetOffset((int) fieldDefinition.Offset);
            if (fieldDefinition.HasConstant)
                fieldBuilder.SetConstant(fieldDefinition.Constant);

            metadataPass.Add(delegate
            {
                InitializeCustomAttributes(fieldBuilder.SetCustomAttribute, fieldDefinition.CustomAttributes);
            });                
        }

        private void BuildProperty(DynamicType type, PropertyDefinition propertyDefinition)
        {
            PropertyBuilder propertyBuilder = type.DefineProperty(propertyDefinition.Name,
                (System.Reflection.PropertyAttributes)propertyDefinition.Attributes,
                ResolveType(propertyDefinition.PropertyType),
                ResolveParameterTypes(propertyDefinition.Parameters));
            // TODO: Custom modifiers and other stuff.

            if (propertyDefinition.HasConstant)
                propertyBuilder.SetConstant(propertyDefinition.Constant);
            if (propertyDefinition.GetMethod != null)
                propertyBuilder.SetGetMethod(BuildMethod(type, propertyDefinition.GetMethod));
            if (propertyDefinition.SetMethod != null)
                propertyBuilder.SetSetMethod(BuildMethod(type, propertyDefinition.SetMethod));

            metadataPass.Add(delegate
            {
                InitializeCustomAttributes(propertyBuilder.SetCustomAttribute, propertyDefinition.CustomAttributes);
            });
        }

        private void BuildEvent(DynamicType type, EventDefinition eventDefinition)
        {
            EventBuilder eventBuilder = type.DefineEvent(eventDefinition.Name,
                (System.Reflection.EventAttributes) eventDefinition.Attributes,
                ResolveType(eventDefinition.EventType));
            // TODO: Custom modifiers and other stuff.

            if (eventDefinition.InvokeMethod != null)
                eventBuilder.SetRaiseMethod(BuildMethod(type, eventDefinition.InvokeMethod));
            if (eventDefinition.AddMethod != null)
                eventBuilder.SetAddOnMethod(BuildMethod(type, eventDefinition.AddMethod));
            if (eventDefinition.RemoveMethod != null)
                eventBuilder.SetRemoveOnMethod(BuildMethod(type, eventDefinition.RemoveMethod));

            metadataPass.Add(delegate
            {
                InitializeCustomAttributes(eventBuilder.SetCustomAttribute, eventDefinition.CustomAttributes);
            });
        }

        private void BuildConstructor(DynamicType type, MethodDefinition methodDefinition)
        {
            DynamicConstructor constructor = type.DefineConstructor(
                (System.Reflection.MethodAttributes) methodDefinition.Attributes,
                CecilUtils.GetCallingConventions(methodDefinition),
                ResolveParameterTypes(methodDefinition.Parameters));
            // TODO: Custom modifiers and other stuff.

            InitializeParameters(constructor.Builder.DefineParameter, methodDefinition.Parameters);

            constructor.Builder.SetImplementationFlags((System.Reflection.MethodImplAttributes)
                methodDefinition.ImplAttributes);

            metadataPass.Add(delegate
            {
                InitializeDeclarativeSecurity(constructor.Builder.AddDeclarativeSecurity, methodDefinition.SecurityDeclarations);
                InitializeCustomAttributes(constructor.Builder.SetCustomAttribute, methodDefinition.CustomAttributes);
            });

            if (methodDefinition.HasBody)
            {
                ilPass.Add(delegate
                {
                    constructor.Builder.InitLocals = methodDefinition.Body.InitLocals;
                    BuildIL(constructor.Builder, constructor.Builder.GetILGenerator, methodDefinition.Body);
                });
            }
        }

        private MethodBuilder BuildMethod(DynamicType type, MethodDefinition methodDefinition)
        {
            MethodBuilder methodBuilder = type.DefineMethod(methodDefinition.Name,
                (System.Reflection.MethodAttributes) methodDefinition.Attributes,
                CecilUtils.GetCallingConventions(methodDefinition));
            // TODO: Custom modifiers and other stuff.

            InitializeGenericParameters(methodBuilder.DefineGenericParameters, methodDefinition.GenericParameters);
            methodBuilder.SetParameters(ResolveParameterTypes(methodDefinition.Parameters));
            InitializeParameters(methodBuilder.DefineParameter, methodDefinition.Parameters);
            InitializeReturnType(methodBuilder, methodDefinition.ReturnType);

            methodBuilder.SetImplementationFlags((System.Reflection.MethodImplAttributes)
                methodDefinition.ImplAttributes);

            metadataPass.Add(delegate
            {
                InitializeDeclarativeSecurity(methodBuilder.AddDeclarativeSecurity, methodDefinition.SecurityDeclarations);
                InitializeCustomAttributes(methodBuilder.SetCustomAttribute, methodDefinition.CustomAttributes);
            });

            if (methodDefinition.HasBody)
            {
                ilPass.Add(delegate
                {
                    methodBuilder.InitLocals = methodDefinition.Body.InitLocals;
                    BuildIL(methodBuilder, methodBuilder.GetILGenerator, methodDefinition.Body);
                });
            }

            return methodBuilder;
        }

        private void InitializeGenericParameters(DefineGenericParametersDelegate defineGenericParameters,
            GenericParameterCollection genericParameters)
        {
            if (genericParameters.Count == 0)
                return;

            string[] names = CollectionUtils.ConvertAllToArray<GenericParameter, string>(genericParameters, delegate(GenericParameter genericParameter)
            {
                return genericParameter.Name;
            });

            GenericTypeParameterBuilder[] genericTypeParameterBuilders = defineGenericParameters(names);

            for (int i = 0; i < genericParameters.Count; i++)
            {
                GenericTypeParameterBuilder genericTypeParameterBuilder = genericTypeParameterBuilders[i];
                GenericParameter genericParameter = genericParameters[i];

                genericTypeParameterBuilder.SetGenericParameterAttributes(
                    (System.Reflection.GenericParameterAttributes) genericParameter.Attributes);

                if (genericParameter.Constraints.Count != 0)
                {
                    Type classConstraint = null;
                    List<Type> interfaceConstraints = new List<Type>();

                    foreach (TypeReference typeReference in genericParameter.Constraints)
                    {
                        Type type = ResolveType(typeReference);
                        if (type.IsInterface)
                            interfaceConstraints.Add(type);
                        else
                            classConstraint = type;
                    }

                    if (classConstraint != null)
                        genericTypeParameterBuilder.SetBaseTypeConstraint(classConstraint);
                    if (interfaceConstraints.Count != 0)
                        genericTypeParameterBuilder.SetInterfaceConstraints(interfaceConstraints.ToArray());
                }
            }

            metadataPass.Add(delegate
            {
                for (int i = 0; i < genericParameters.Count; i++)
                {
                    InitializeCustomAttributes(genericTypeParameterBuilders[i].SetCustomAttribute,
                        genericParameters[i].CustomAttributes);
                }
            });
        }

        private void InitializeParameters(DefineParameterDelegate defineParameter, ParameterDefinitionCollection parameterDefinitions)
        {
            foreach (ParameterDefinition parameterDefinition in parameterDefinitions)
            {
                ParameterBuilder parameterBuilder = defineParameter(parameterDefinition.Sequence,
                    (System.Reflection.ParameterAttributes)parameterDefinition.Attributes, parameterDefinition.Name);

                if (parameterDefinition.HasConstant)
                    parameterBuilder.SetConstant(parameterDefinition.Constant);

                metadataPass.Add(delegate
                {
                    InitializeCustomAttributes(parameterBuilder.SetCustomAttribute, parameterDefinition.CustomAttributes);
                });
            }
        }

        private void InitializeReturnType(MethodBuilder methodBuilder, MethodReturnType returnType)
        {
            methodBuilder.SetReturnType(ResolveReturnType(returnType));

            ParameterBuilder parameterBuilder = methodBuilder.DefineParameter(0,
                System.Reflection.ParameterAttributes.Retval, null);

            if (returnType.HasConstant)
                parameterBuilder.SetConstant(returnType.Constant);

            metadataPass.Add(delegate
            {
                InitializeCustomAttributes(parameterBuilder.SetCustomAttribute, returnType.CustomAttributes);
            });
        }

        private void InitializeDeclarativeSecurity(AddDeclarativeSecurityDelegate addDeclarativeSecurity,
            SecurityDeclarationCollection securityDeclarations)
        {
            foreach (SecurityDeclaration securityDeclaration in securityDeclarations)
            {
                addDeclarativeSecurity(
                    (System.Security.Permissions.SecurityAction)securityDeclaration.Action,
                    securityDeclaration.PermissionSet);
            }
        }

        private void InitializeCustomAttributes(SetCustomAttributeDelegate setCustomAttribute, CustomAttributeCollection attributes)
        {
            foreach (CustomAttribute attribute in attributes)
            {
                attribute.Resolve();

                ConstructorInfo constructor = ResolveConstructor(attribute.Constructor);
                object[] constructorArgs = CollectionUtils.ToArray<object>(attribute.ConstructorParameters);
                Type attributeType = constructor.ReflectedType;

                PropertyInfo[] namedProperties = new PropertyInfo[attribute.Properties.Count];
                object[] propertyValues = new object[attribute.Properties.Count];
                int i = 0;
                foreach (DictionaryEntry entry in attribute.Properties)
                {
                    namedProperties[i] = GetProperty(attributeType, (string)entry.Key);
                    propertyValues[i] = entry.Value;
                    i += 1;
                }

                FieldInfo[] namedFields = new FieldInfo[attribute.Fields.Count];
                object[] fieldValues = new object[attribute.Fields.Count];
                i = 0;
                foreach (DictionaryEntry entry in attribute.Fields)
                {
                    namedFields[i] = GetField(attributeType, (string)entry.Key);
                    fieldValues[i] = entry.Value;
                    i += 1;
                }

                WorkaroundCecilBug82814(ref constructorArgs, CollectionUtils.ConvertAllToArray<ParameterInfo, Type>(constructor.GetParameters(),
                    delegate(ParameterInfo parameter) { return parameter.ParameterType; }));
                WorkaroundCecilBug82814(ref propertyValues, CollectionUtils.ConvertAllToArray<PropertyInfo, Type>(namedProperties,
                    delegate(PropertyInfo property) { return property.PropertyType; }));
                WorkaroundCecilBug82814(ref fieldValues, CollectionUtils.ConvertAllToArray<FieldInfo, Type>(namedFields,
                    delegate(FieldInfo field) { return field.FieldType; }));

                CustomAttributeBuilder attributeBuilder = new CustomAttributeBuilder(
                    constructor, constructorArgs,
                    namedProperties, propertyValues,
                    namedFields, fieldValues);
                setCustomAttribute(attributeBuilder);
            }
        }

        /// <summary>
        /// FIXME: We need to detect and reinterpret type names that appear as strings.
        /// This isn't perfect and will be fooled by object[] arrays.
        /// http://bugzilla.ximian.com/show_bug.cgi?id=82814
        /// </summary>
        private void WorkaroundCecilBug82814(ref object[] values, Type[] types)
        {
            for (int i = 0; i < values.Length; i++)
            {
                Type elementType = types[i];
                if (elementType.HasElementType)
                    elementType = elementType.GetElementType();

                if (elementType == typeof(Type))
                {
                    string typeName = (string)values[i];
                    values[i] = GetType(typeName);
                }
            }
        }

        private void BuildIL(MethodBase method, GetILGeneratorDelegate getILGenerator, Mono.Cecil.Cil.MethodBody body)
        {
            ILGenerator generator = getILGenerator(body.CodeSize);
            dynamicMethodBuilder.BuildIL(method, generator, body);
        }

        #region Resolvers
        internal Module ResolveModule(ModuleReference reference)
        {
            return GetModule(reference.Name);
        }

        internal Type[] ResolveTypes(ICollection references)
        {
            return CollectionUtils.ConvertAllToArray<TypeReference, Type>(references, ResolveType);
        }

        internal Type ResolveType(TypeReference reference)
        {
            GenericParameter genericParameter = reference as GenericParameter;
            if (genericParameter != null)
                return ResolveGenericParameter(genericParameter);

            TypeSpecification typeSpecification = reference as TypeSpecification;
            if (typeSpecification != null)
                return ResolveTypeSpecification(typeSpecification);

            // Simple type reference.
            string typeName = reference.FullName.Replace('/', '+'); // FIXME: Strangely Cecil is using '/' to delimit nested types!

            ModuleReference moduleReference = reference.Scope as ModuleReference;
            if (moduleReference != null)
            {
                Module module = ResolveModule(moduleReference);
                return GetType(module, typeName);
            }
            else
            {
                AssemblyNameReference assemblyNameReference = (AssemblyNameReference)reference.Scope;
                Assembly assembly = Assembly.Load(new AssemblyName(assemblyNameReference.FullName));
                return GetType(assembly, typeName);
            }
        }

        internal Type ResolveGenericParameter(GenericParameter genericParameter)
        {
            IGenericParameterProvider owner = genericParameter.Owner;

            TypeReference ownerTypeReference = owner as TypeReference;
            if (ownerTypeReference != null)
            {
                Type ownerType = ResolveType(ownerTypeReference);
                return ownerType.GetGenericArguments()[genericParameter.Position];
            }

            MethodReference ownerMethodReference = (MethodReference)owner;
            MethodBase ownerMethod = ResolveMethod(ownerMethodReference);
            return ownerMethod.GetGenericArguments()[genericParameter.Position];
        }

        internal Type ResolveTypeSpecification(TypeSpecification typeSpecification)
        {
            Type elementType = ResolveType(typeSpecification.ElementType);

            ArrayType arrayType = typeSpecification as ArrayType;
            if (arrayType != null)
            {
                if (arrayType.IsSizedArray)
                    return elementType.MakeArrayType();

                return elementType.MakeArrayType(arrayType.Rank);
            }

            GenericInstanceType genericInstanceType = typeSpecification as GenericInstanceType;
            if (genericInstanceType != null)
            {
                Type[] args = ResolveTypes(genericInstanceType.GenericArguments);
                return elementType.MakeGenericType(args);
            }

            ReferenceType referenceType = typeSpecification as ReferenceType;
            if (referenceType != null)
            {
                return elementType.MakeByRefType();
            }

            PointerType pointerType = typeSpecification as PointerType;
            if (pointerType != null)
            {
                return elementType.MakePointerType();
            }

            FunctionPointerType functionPointerType = typeSpecification as FunctionPointerType;
            PinnedType pinnedType = typeSpecification as PinnedType;
            ModifierOptional modifierOptional = typeSpecification as ModifierOptional;
            ModifierRequired modifierRequired = typeSpecification as ModifierRequired;
            SentinelType sentinelType = typeSpecification as SentinelType;
            throw new NotImplementedException();
        }

        internal Type ResolveReturnType(MethodReturnType returnType)
        {
            return ResolveType(returnType.ReturnType);
        }

        internal Type[] ResolveParameterTypes(ICollection parameterReferences)
        {
            return CollectionUtils.ConvertAllToArray<ParameterReference, Type>(parameterReferences, ResolveParameterType);
        }

        internal Type ResolveParameterType(ParameterReference parameterReference)
        {
            return ResolveType(parameterReference.ParameterType);
        }

        internal MethodBase ResolveMethod(MethodReference reference)
        {
            if (reference.Name == @".ctor" || reference.Name == @".cctor")
                return ResolveConstructor(reference);

            Type[] parameterTypes = ResolveParameterTypes(reference.Parameters);

            Type type = ResolveType(reference.DeclaringType);
            return GetMethod(type, reference.Name, reference.HasThis, parameterTypes);
        }

        internal ConstructorInfo ResolveConstructor(MethodReference reference)
        {
            Type[] parameterTypes = ResolveParameterTypes(reference.Parameters);

            Type type = ResolveType(reference.DeclaringType);
            return GetConstructor(type, reference.HasThis, parameterTypes);
        }

        internal FieldInfo ResolveField(FieldReference reference)
        {
            Type type = ResolveType(reference.DeclaringType);
            return GetField(type, reference.Name);
        }

        internal PropertyInfo ResolveProperty(PropertyReference reference)
        {
            Type returnType = ResolveType(reference.PropertyType);
            Type[] parameterTypes = ResolveParameterTypes(reference.Parameters);

            Type type = ResolveType(reference.DeclaringType);
            return GetProperty(type, reference.Name, returnType, parameterTypes);
        }
        #endregion

        #region Reflection support for dynamically created types
        internal Module GetModule(string name)
        {
            Module module = dynamicAssembly.Builder.GetDynamicModule(name);

            if (module == null)
                throw new InvalidOperationException(string.Format("Module not found: {0}", name));
            return module;
        }

        internal Type GetType(string name)
        {
            Type type = Type.GetType(name);

            if (type == null)
                throw new InvalidOperationException(string.Format("Type not found: {0}", name));
            return type;
        }

        internal Type GetType(Assembly assembly, string name)
        {
            Type type = assembly.GetType(name);

            if (type == null)
                throw new InvalidOperationException(string.Format("Type not found: {0}", name));
            return type;
        }

        internal Type GetType(Module module, string name)
        {
            Type type = module.GetType(name);

            if (type == null)
                throw new InvalidOperationException(string.Format("Type not found: {0}", name));
            return type;
        }

        internal Type GetNestedType(Type type, string name)
        {
            Type nestedType;
            DynamicType dynamicType = dynamicAssembly.GetDynamicType(type);
            if (dynamicType != null)
                nestedType = dynamicType.GetNestedType(name);
            else
                nestedType = type.GetNestedType(name, BindingFlags.Public | BindingFlags.NonPublic);

            if (nestedType == null)
                throw new InvalidOperationException(string.Format("Nested type not found: {0}+{1}", type, name));
            return nestedType;
        }

        internal FieldInfo GetField(Type type, string name)
        {
            FieldInfo field;
            DynamicType dynamicType = dynamicAssembly.GetDynamicType(type);
            if (dynamicType != null)
                field = dynamicType.GetField(name);
            else
                field = type.GetField(name, BindingFlags.Public | BindingFlags.NonPublic
                | BindingFlags.Instance | BindingFlags.Static);

            if (field == null)
                throw new InvalidOperationException(string.Format("Field not found: {0}.{1}", type, name));
            return field;
        }

        internal PropertyInfo GetProperty(Type type, string name)
        {
            PropertyInfo property;
            DynamicType dynamicType = dynamicAssembly.GetDynamicType(type);
            if (dynamicType != null)
                property = dynamicType.GetProperty(name);
            else
                property = type.GetProperty(name, BindingFlags.Public | BindingFlags.NonPublic
                    | BindingFlags.Instance | BindingFlags.Static);

            if (property == null)
                throw new InvalidOperationException(string.Format("Property not found: {0}.{1}", type, name));
            return property;
        }

        internal PropertyInfo GetProperty(Type type, string name, Type returnType, Type[] paramTypes)
        {
            PropertyInfo property;
            DynamicType dynamicType = dynamicAssembly.GetDynamicType(type);
            if (dynamicType != null)
                property = dynamicType.GetProperty(name, returnType, paramTypes);
            else
                property = type.GetProperty(name, BindingFlags.Public | BindingFlags.NonPublic
                | BindingFlags.Instance | BindingFlags.Static | BindingFlags.ExactBinding, null, returnType, paramTypes, null);

            if (property == null)
                throw new InvalidOperationException(string.Format("Property not found: {0}.{1}", type, name));
            return property;
        }

        internal MethodInfo GetMethod(Type type, string name, bool instance, Type[] paramTypes)
        {
            MethodInfo method;
            DynamicType dynamicType = dynamicAssembly.GetDynamicType(type);
            if (dynamicType != null)
                method = dynamicType.GetMethod(name, instance, paramTypes);
            else
                method = type.GetMethod(name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.ExactBinding
                    | (instance ? BindingFlags.Instance : BindingFlags.Static), null, CallingConventions.Any, paramTypes, null);

            if (method == null)
                throw new InvalidOperationException(string.Format("Method not found: {0}.{1}", type, name));
            return method;
        }

        internal ConstructorInfo GetConstructor(Type type, bool instance, Type[] paramTypes)
        {
            ConstructorInfo constructor;
            DynamicType dynamicType = dynamicAssembly.GetDynamicType(type);
            if (dynamicType != null)
            {
                DynamicConstructor dynamicConstructor = dynamicType.GetConstructor(instance, paramTypes);
                constructor = dynamicConstructor != null ? dynamicConstructor.Builder : null;
            }
            else
                constructor = type.GetConstructor(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.ExactBinding
                    | (instance ? BindingFlags.Instance : BindingFlags.Static), null, CallingConventions.Any, paramTypes, null);

            if (constructor == null)
                throw new InvalidOperationException(string.Format("Constructor not found: {0}", type));
            return constructor;
        }
        #endregion
    }
}