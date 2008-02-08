// Copyright 2008 MbUnit Project - http://www.mbunit.com/
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
using Gallio.Reflection;
using Gallio.ReSharperRunner.Reflection.Impl;
using JetBrains.Metadata.Reader.API;
using JetBrains.Metadata.Reader.Impl;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Caches;

namespace Gallio.ReSharperRunner.Reflection
{
    /// <summary>
    /// Wraps ReSharper metadata types using the reflection adapter interfaces.
    /// </summary>
    /// <todo author="jeff">
    /// Support inherited attribute lookup.
    /// </todo>
    public class MetadataReflector : ReSharperReflector, IReflectionPolicy
    {
        private readonly IProject contextProject;
        private readonly MetadataLoader metadataLoader;

        /// <summary>
        /// Creates a reflector with the specified project as its context.
        /// The context project is used to resolve metadata items to declared elements.
        /// </summary>
        /// <param name="assemblyResolver">The assembly resolver</param>
        /// <param name="assembly">The assembly provide context for the loader</param>
        /// <param name="contextProject">The context project, or null if none</param>
        public MetadataReflector(IAssemblyResolver assemblyResolver, IMetadataAssembly assembly, IProject contextProject)
            : base(assemblyResolver)
        {
            if (assembly == null)
                throw new ArgumentNullException("assembly");

            metadataLoader = GetMetadataLoaderHack(assembly);
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
            return target != null ? new MetadataAssemblyWrapper(this, target) : null;
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
                return new MetadataClassTypeWrapper(this, classType);

            IMetadataArrayType arrayType = target as IMetadataArrayType;
            if (arrayType != null)
                return new MetadataArrayTypeWrapper(this, arrayType);

            IMetadataPointerType pointerType = target as IMetadataPointerType;
            if (pointerType != null)
                return new MetadataPointerTypeWrapper(this, pointerType);

            IMetadataReferenceType referenceType = target as IMetadataReferenceType;
            if (referenceType != null)
                return new MetadataReferenceTypeWrapper(this, referenceType);

            IMetadataGenericArgumentReferenceType argumentType = target as IMetadataGenericArgumentReferenceType;
            if (argumentType != null)
                return new MetadataGenericParameterWrapper(this, argumentType);

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
            return target != null ? new MetadataClassTypeWrapper(this, new OpenClassType(target)) : null;
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

            return IsConstructor(target) ? (IFunctionInfo) new MetadataConstructorWrapper(this, target) : new MetadataMethodWrapper(this, target);
        }

        /// <summary>
        /// Obtains a reflection wrapper for a method.
        /// </summary>
        /// <param name="target">The method, or null if none</param>
        /// <returns>The reflection wrapper, or null if none</returns>
        public IMethodInfo WrapMethod(IMetadataMethod target)
        {
            return target != null ? new MetadataMethodWrapper(this, target) : null;
        }

        /// <summary>
        /// Obtains a reflection wrapper for a constructor.
        /// </summary>
        /// <param name="target">The method, or null if none</param>
        /// <returns>The reflection wrapper, or null if none</returns>
        public IConstructorInfo WrapConstructor(IMetadataMethod target)
        {
            return target != null ? new MetadataConstructorWrapper(this, target) : null;
        }

        /// <summary>
        /// Obtains a reflection wrapper for a property.
        /// </summary>
        /// <param name="target">The property, or null if none</param>
        /// <returns>The reflection wrapper, or null if none</returns>
        public IPropertyInfo Wrap(IMetadataProperty target)
        {
            return target != null ? new MetadataPropertyWrapper(this, target) : null;
        }

        /// <summary>
        /// Obtains a reflection wrapper for a field.
        /// </summary>
        /// <param name="target">The field, or null if none</param>
        /// <returns>The reflection wrapper, or null if none</returns>
        public IFieldInfo Wrap(IMetadataField target)
        {
            return target != null ? new MetadataFieldWrapper(this, target) : null;
        }

        /// <summary>
        /// Obtains a reflection wrapper for an event.
        /// </summary>
        /// <param name="target">The event, or null if none</param>
        /// <returns>The reflection wrapper, or null if none</returns>
        public IEventInfo Wrap(IMetadataEvent target)
        {
            return target != null ? new MetadataEventWrapper(this, target) : null;
        }

        /// <summary>
        /// Obtains a reflection wrapper for a parameter.
        /// </summary>
        /// <param name="target">The parameter, or null if none</param>
        /// <returns>The reflection wrapper, or null if none</returns>
        public IParameterInfo Wrap(IMetadataParameter target)
        {
            return target != null ? new MetadataParameterWrapper(this, target) : null;
        }

        /// <summary>
        /// Obtains a reflection wrapper for a return value.
        /// </summary>
        /// <param name="target">The return value, or null if none</param>
        /// <returns>The reflection wrapper, or null if none</returns>
        public IParameterInfo Wrap(IMetadataReturnValue target)
        {
            return target != null ? new MetadataReturnValueWrapper(this, target) : null;
        }

        /// <summary>
        /// Obtains a reflection wrapper for a generic argument.
        /// </summary>
        /// <param name="target">The generic parameter, or null if none</param>
        /// <returns>The reflection wrapper, or null if none</returns>
        public IGenericParameterInfo Wrap(IMetadataGenericArgument target)
        {
            return target != null ? new MetadataGenericParameterWrapper(this, new MetadataGenericArgumentReferenceType(target)) : null;
        }

        /// <summary>
        /// Obtains a reflection wrapper for an attribute.
        /// </summary>
        /// <param name="target">The attribute, or null if none</param>
        /// <returns>The reflection wrapper, or null if none</returns>
        public IAttributeInfo Wrap(IMetadataCustomAttribute target)
        {
            return target != null ? new MetadataAttributeWrapper(this, target) : null;
        }

        /// <inheritdoc />
        public override IAssemblyInfo LoadAssembly(AssemblyName assemblyName)
        {
            return Wrap(LoadMetadataAssembly(assemblyName, true));
        }

        internal static bool IsConstructor(IMetadataMethod method)
        {
            string name = method.Name;
            return name == ".ctor" || name == ".cctor";
        }

        private IDeclarationsCache GetDeclarationsCache()
        {
            return PsiManager.GetInstance(contextProject.GetSolution()).
                GetDeclarationsCache(DeclarationsCacheScope.ProjectScope(contextProject, true), true);
        }

        internal ITypeElement GetDeclaredElementWithLock(IMetadataTypeInfo type)
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

        internal IFunction GetDeclaredElementWithLock(IMetadataMethod metadataMethod)
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

        internal static bool IsSameSignature(IList<IParameter> parameters, IMetadataParameter[] metadataParameters)
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

        internal IProperty GetDeclaredElementWithLock(IMetadataProperty metadataProperty)
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

        internal IField GetDeclaredElementWithLock(IMetadataField metadataField)
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

        internal IEvent GetDeclaredElementWithLock(IMetadataEvent metadataEvent)
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

        internal IParameter GetDeclaredElementWithLock(IMetadataParameter metadataParameter)
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

        internal IParameter GetDeclaredElementWithLock(IMetadataReturnValue metadataParameter)
        {
            // FIXME: Not sure which ReSharper code model element represents a return value.
            return null;
        }

        internal IMetadataAssembly LoadMetadataAssembly(AssemblyName assemblyName, bool throwOnError)
        {
            if (metadataLoader != null)
            {
                IMetadataAssembly assembly = metadataLoader.Load(assemblyName, delegate { return true; });
                if (assembly != null)
                    return assembly;
            }

            if (throwOnError)
                throw new InvalidOperationException(String.Format("The metadata loader could not load assembly '{0}'.", assemblyName));

            return null;
        }

        internal static IMetadataAssembly GetMetadataAssemblyHack(IMetadataTypeInfo typeInfo)
        {
            // HACK: This type contains a reference to its assembly but it
            //       does not expose it in a useful manner.
            FieldInfo myAssemblyField = typeInfo.GetType().GetField("myAssembly", BindingFlags.Instance | BindingFlags.NonPublic);
            return myAssemblyField != null ? (IMetadataAssembly)myAssemblyField.GetValue(typeInfo) : null;
        }

        internal static MetadataLoader GetMetadataLoaderHack(IMetadataAssembly assembly)
        {
            // HACK: The assembly contains a reference back to its loader
            //       which is useful for loading referenced assemblies but it
            //       does not expose it in a useful manner.
            PropertyInfo loaderProperty = assembly.GetType().GetProperty("Loader", BindingFlags.Instance | BindingFlags.NonPublic);
            return loaderProperty != null ? (MetadataLoader)loaderProperty.GetValue(assembly, null) : null;
        }

        private sealed class OpenClassType : IMetadataClassType
        {
            private readonly IMetadataTypeInfo type;
            private IMetadataType[] arguments;

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
                get
                {
                    if (arguments == null)
                    {
                        IMetadataGenericArgument[] genericParameters = type.GenericParameters;
                        if (genericParameters.Length == 0)
                        {
                            arguments = EmptyArray<IMetadataType>.Instance;
                        }
                        else
                        {
                            arguments = Array.ConvertAll<IMetadataGenericArgument, IMetadataType>(genericParameters, delegate(IMetadataGenericArgument genericParameter)
                            {
                                return new MetadataGenericArgumentReferenceType(genericParameter);
                            });
                        }
                    }

                    return arguments;
                }
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
