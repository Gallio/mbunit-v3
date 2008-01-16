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
using System.Reflection;
using Gallio.Reflection;
using Gallio.ReSharperRunner.Reflection.Impl;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Psi;

namespace Gallio.ReSharperRunner.Reflection
{
    /// <summary>
    /// Wraps ReSharper code model types using the reflection adapter interfaces.
    /// </summary>
    /// <todo author="jeff">
    /// Support inherited attribute lookup.
    /// </todo>
    public class PsiReflector : ReSharperReflector, IReflectionPolicy
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
            return target != null ? new PsiProjectWrapper(this, target) : null;
        }

        /// <summary>
        /// Obtains a reflection wrapper for an assembly.
        /// </summary>
        /// <param name="target">The assembly, or null if none</param>
        /// <returns>The reflection wrapper, or null if none</returns>
        public IAssemblyInfo Wrap(IAssembly target)
        {
            return target != null ? new PsiAssemblyWrapper(this, target) : null;
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
                    return new PsiGenericParameterWrapper(this, declaredType);
                return new PsiDeclaredTypeWrapper(this, declaredType);
            }

            IArrayType arrayType = target as IArrayType;
            if (arrayType != null)
                return new PsiArrayTypeWrapper(this, arrayType);

            IPointerType pointerType = target as IPointerType;
            if (pointerType != null)
                return new PsiPointerTypeWrapper(this, pointerType);

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
            return target != null ? new PsiGenericParameterWrapper(this, TypeFactory.CreateType(target)) : null;
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
            return constructor != null ? (IFunctionInfo)new PsiConstructorWrapper(this, constructor) : new PsiMethodWrapper(this, target);
        }

        /// <summary>
        /// Obtains a reflection wrapper for a method.
        /// </summary>
        /// <param name="target">The method, or null if none</param>
        /// <returns>The reflection wrapper, or null if none</returns>
        public IMethodInfo Wrap(IMethod target)
        {
            return target != null ? new PsiMethodWrapper(this, target) : null;
        }

        /// <summary>
        /// Obtains a reflection wrapper for a method.
        /// </summary>
        /// <param name="target">The method, or null if none</param>
        /// <returns>The reflection wrapper, or null if none</returns>
        public IMethodInfo Wrap(IOperator target)
        {
            return target != null ? new PsiMethodWrapper(this, target) : null;
        }

        /// <summary>
        /// Obtains a reflection wrapper for a constructor.
        /// </summary>
        /// <param name="target">The constructor, or null if none</param>
        /// <returns>The reflection wrapper, or null if none</returns>
        public IConstructorInfo Wrap(IConstructor target)
        {
            return target != null ? new PsiConstructorWrapper(this, target) : null;
        }

        /// <summary>
        /// Obtains a reflection wrapper for a property.
        /// </summary>
        /// <param name="target">The property, or null if none</param>
        /// <returns>The reflection wrapper, or null if none</returns>
        public IPropertyInfo Wrap(IProperty target)
        {
            return target != null ? new PsiPropertyWrapper(this, target) : null;
        }

        /// <summary>
        /// Obtains a reflection wrapper for a field.
        /// </summary>
        /// <param name="target">The field, or null if none</param>
        /// <returns>The reflection wrapper, or null if none</returns>
        public IFieldInfo Wrap(IField target)
        {
            return target != null ? new PsiFieldWrapper(this, target) : null;
        }

        /// <summary>
        /// Obtains a reflection wrapper for an event.
        /// </summary>
        /// <param name="target">The event, or null if none</param>
        /// <returns>The reflection wrapper, or null if none</returns>
        public IEventInfo Wrap(IEvent target)
        {
            return target != null ? new PsiEventWrapper(this, target) : null;
        }

        /// <summary>
        /// Obtains a reflection wrapper for a parameter.
        /// </summary>
        /// <param name="target">The parameter, or null if none</param>
        /// <returns>The reflection wrapper, or null if none</returns>
        public IParameterInfo Wrap(IParameter target)
        {
            return target != null ? new PsiParameterWrapper(this, target) : null;
        }

        /// <summary>
        /// Obtains a reflection wrapper for an attribute.
        /// </summary>
        /// <param name="target">The attribute, or null if none</param>
        /// <returns>The reflection wrapper, or null if none</returns>
        public IAttributeInfo Wrap(IAttributeInstance target)
        {
            return target != null ? new PsiAttributeWrapper(this, target) : null;
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
    }
}