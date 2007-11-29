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
using System.Globalization;
using System.Reflection;
using Gallio.Collections;
using Gallio.Properties;

namespace Gallio.Model.Reflection
{
    /// <summary>
    /// Provides functions for working with reflection.
    /// </summary>
    public static class ReflectionUtils
    {
        /// <summary>
        /// Gets the assembly that declares the code element, or the
        /// code element itself if it is an <see cref="IAssemblyInfo"/>.
        /// </summary>
        /// <param name="codeElement">The code element, possibly null</param>
        /// <returns>The assembly, or null if not found</returns>
        public static IAssemblyInfo GetAssembly(ICodeElementInfo codeElement)
        {
            IAssemblyInfo assembly = codeElement as IAssemblyInfo;
            if (assembly != null)
                return assembly;

            ITypeInfo type = GetType(codeElement);
            if (type != null)
                return type.Assembly;

            return null;
        }

        /// <summary>
        /// Gets the namespace that declares the code element, or the
        /// code element itself if it is an <see cref="INamespaceInfo"/>.
        /// </summary>
        /// <param name="codeElement">The code element, possibly null</param>
        /// <returns>The namespace, or null if not found</returns>
        public static INamespaceInfo GetNamespace(ICodeElementInfo codeElement)
        {
            INamespaceInfo @namespace = codeElement as INamespaceInfo;
            if (@namespace != null)
                return @namespace;

            ITypeInfo type = GetType(codeElement);
            if (type != null)
                return type.Namespace;

            return null;
        }

        /// <summary>
        /// Gets the type that declares the code element, or the
        /// code element itself if it is an <see cref="ITypeInfo"/>.
        /// </summary>
        /// <param name="codeElement">The code element, possibly null</param>
        /// <returns>The type, or null if not found</returns>
        public static ITypeInfo GetType(ICodeElementInfo codeElement)
        {
            ITypeInfo type = codeElement as ITypeInfo;
            if (type != null)
                return type;

            IMemberInfo member = GetMember(codeElement);
            if (member != null)
                return member.DeclaringType;

            return null;
        }

        /// <summary>
        /// Gets the non-type member that declares the code element, or the
        /// code element itself if it is an <see cref="IMemberInfo"/>
        /// other than a <see cref="ITypeInfo" />.
        /// </summary>
        /// <param name="codeElement">The code element, possibly null</param>
        /// <returns>The member, or null if not found</returns>
        public static IMemberInfo GetMember(ICodeElementInfo codeElement)
        {
            IMemberInfo member = codeElement as IMemberInfo;
            if (member != null)
                return member is ITypeInfo ? null : member;

            IParameterInfo parameter = GetParameter(codeElement);
            if (parameter != null)
                return parameter.Member;

            return null;
        }

        /// <summary>
        /// Gets the parameter that declares the code element, or the
        /// code element itself if it is an <see cref="IParameterInfo"/>.
        /// </summary>
        /// <param name="codeElement">The code element, possibly null</param>
        /// <returns>The parameter, or null if not found</returns>
        public static IParameterInfo GetParameter(ICodeElementInfo codeElement)
        {
            IParameterInfo type = codeElement as IParameterInfo;
            if (type != null)
                return type;

            return null;
        }

        /// <summary>
        /// Checks that the method has the specified signature otherwise throws a <see cref="ModelException" />.
        /// </summary>
        /// <param name="method">The method</param>
        /// <param name="signature">The list of parameter types (all input parameters)</param>
        /// <exception cref="ModelException">Thrown if the method has a different signature</exception>
        public static void CheckMethodSignature(IMethodInfo method, params ITypeInfo[] signature)
        {
            IParameterInfo[] parameters = method.GetParameters();
            if (parameters.Length == signature.Length)
            {
                for (int i = 0; i < parameters.Length; i++)
                {
                    IParameterInfo parameter = parameters[i];
                    if (parameter.ValueType != signature[i]
                        || (parameter.Modifiers & (ParameterAttributes.In | ParameterAttributes.Out)) != ParameterAttributes.In)
                        goto Fail;
                }

                return;
            }

        Fail:
            string[] expectedTypeNames = Array.ConvertAll<ITypeInfo, string>(signature, delegate(ITypeInfo parameterType)
            {
                return parameterType.FullName;
            });
            string[] actualTypeNames = Array.ConvertAll<IParameterInfo, string>(parameters, delegate(IParameterInfo parameter)
            {
                if ((parameter.Modifiers & ParameterAttributes.Out) != 0)
                {
                    string prefix = (parameter.Modifiers & ParameterAttributes.In) != 0 ? @"ref " : @"out ";
                    return prefix + parameter.ValueType.FullName;
                }

                return parameter.ValueType.FullName;
            });

            throw new ModelException(String.Format(CultureInfo.CurrentCulture,
                Resources.ModelUtils_InvalidSignature,
                string.Join(@", ", expectedTypeNames),
                string.Join(@", ", actualTypeNames)));
        }

        /// <summary>
        /// Finds the assembly name of the directly referenced assembly with the specified display name.
        /// </summary>
        /// <param name="assembly">The assembly to search</param>
        /// <param name="displayName">The display name of the referenced assembly to find</param>
        /// <returns>The referenced assembly name or null if none</returns>
        public static AssemblyName FindAssemblyReference(IAssemblyInfo assembly, string displayName)
        {
            foreach (AssemblyName reference in assembly.GetReferencedAssemblies())
            {
                if (reference.Name == displayName)
                    return reference;
            }

            return null;
        }

        /// <summary>
        /// Maps an enumeration of assemblies based on the assembly name of their
        /// reference with the specified display name.  Assemblies that do not have
        /// referenced with the specified display name are omitted from the map.
        /// </summary>
        /// <param name="assemblies">The assemblies to map</param>
        /// <param name="displayName">The display name of the referenced assembly</param>
        /// <returns>A map of the input assemblies indexed by the version of the desired reference</returns>
        public static IMultiMap<Version, IAssemblyInfo> MapByAssemblyReferenceVersion(IEnumerable<IAssemblyInfo> assemblies, string displayName)
        {
            MultiMap<Version, IAssemblyInfo> map = new MultiMap<Version, IAssemblyInfo>();
            foreach (IAssemblyInfo assembly in assemblies)
            {
                AssemblyName reference = FindAssemblyReference(assembly, displayName);
                if (reference != null)
                    map.Add(reference.Version, assembly);
            }

            return map;
        }

        /// <summary>
        /// Determines if the type can be instantiated using a public constructor.
        /// </summary>
        /// <param name="type">The type</param>
        /// <returns>True if the type can be instantiated</returns>
        public static bool CanInstantiate(ITypeInfo type)
        {
            return type != null
                && (type.Modifiers & (TypeAttributes.Abstract | TypeAttributes.Class | TypeAttributes.Public)) == (TypeAttributes.Class | TypeAttributes.Public)
                && type.ElementType == null
                && type.GetConstructors(BindingFlags.Instance | BindingFlags.Public).Length != 0;
        }

        /// <summary>
        /// Determines if the method is public, non-static and is non-abstract so it can be invoked.
        /// </summary>
        /// <param name="method">The method</param>
        /// <returns>True if the method can be invoked</returns>
        public static bool CanInvokeNonStatic(IMethodInfo method)
        {
            return method != null
                && (method.Modifiers & (MethodAttributes.Abstract | MethodAttributes.Public | MethodAttributes.Static)) == MethodAttributes.Public;
        }

        /// <summary>
        /// Determines if the property has public, non-static and non-abstract getter
        /// and setter functions.
        /// </summary>
        /// <param name="property">The property</param>
        /// <returns>True if the property can be get and set</returns>
        public static bool CanGetAndSetNonStatic(IPropertyInfo property)
        {
            return property != null
                && CanInvokeNonStatic(property.GetGetMethod())
                && CanInvokeNonStatic(property.GetSetMethod());
        }
    }
}
