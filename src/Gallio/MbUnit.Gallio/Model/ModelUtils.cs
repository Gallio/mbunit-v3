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
using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using MbUnit.Collections;
using MbUnit.Properties;

namespace MbUnit.Model
{
    /// <summary>
    /// Provides utility functions for manipulating the object model.
    /// </summary>
    public static class ModelUtils
    {
        /// <summary>
        /// Links a node into the list of children managed by a given parent.
        /// </summary>
        /// <param name="parent">The parent node</param>
        /// <param name="child">The child to add</param>
        /// <exception cref="InvalidOperationException">Thrown if the child already has a parent</exception>
        public static void Link<T>(T parent, T child)
            where T : class, IModelTreeNode<T>
        {
            if (child.Parent != null)
                throw new InvalidOperationException(Resources.ModelUtils_NodeAlreadyHasAParent);

            child.Parent = parent;
            parent.Children.Add(child);
        }

        /// <summary>
        /// Gets all children of the node that have the specified type.
        /// </summary>
        /// <typeparam name="S">The node type</typeparam>
        /// <typeparam name="T">The type to filter by</typeparam>
        /// <param name="node">The node whose children are to be scanned</param>
        /// <returns>The filtered list of children</returns>
        public static IList<T> FilterChildrenByType<S, T>(IModelTreeNode<S> node)
            where S : class, IModelTreeNode<S> where T : class, S
        {
            List<T> filteredChildren = new List<T>();
            foreach (S child in node.Children)
            {
                T filteredChild = child as T;
                if (filteredChild != null)
                    filteredChildren.Add(filteredChild);
            }

            return filteredChildren;
        }

        /// <summary>
        /// Checks that the method has the specified signature otherwise throws a <see cref="ModelException" />.
        /// </summary>
        /// <param name="method">The method</param>
        /// <param name="signature">The list of parameter types (all input parameters)</param>
        /// <exception cref="ModelException">Thrown if the method has a different signature</exception>
        public static void CheckMethodSignature(MethodInfo method, params Type[] signature)
        {
            ParameterInfo[] parameters = method.GetParameters();
            if (parameters.Length == signature.Length)
            {
                for (int i = 0; i < parameters.Length; i++)
                {
                    ParameterInfo parameter = parameters[i];
                    if (parameter.ParameterType != signature[i] || !parameter.IsIn || parameter.IsOut)
                        goto Fail;
                }

                return;
            }

        Fail:
            string[] expectedTypeNames = Array.ConvertAll<Type, string>(signature, delegate(Type parameterType)
            {
                return parameterType.FullName;
            });
            string[] actualTypeNames = Array.ConvertAll<ParameterInfo, string>(parameters, delegate(ParameterInfo parameter)
            {
                if (parameter.IsOut)
                {
                    string prefix = parameter.IsIn ? @"ref " : @"out ";
                    return prefix + parameter.ParameterType.FullName;
                }

                return parameter.ParameterType.FullName;
            });

            throw new ModelException(String.Format(CultureInfo.CurrentCulture,
                Resources.ModelUtils_InvalidSignature,
                string.Join(@", ", expectedTypeNames),
                string.Join(@", ", actualTypeNames)));
        }

        /// <summary>
        /// Gets the attribute with the specified type.
        /// </summary>
        /// <typeparam name="T">The attribute type</typeparam>
        /// <param name="provider">The attribute provider</param>
        /// <returns>The attribute or null if none</returns>
        /// <exception cref="InvalidOperationException">Thrown if the attribute provider
        /// contains multiple attributes of the specified type</exception>
        public static T GetAttribute<T>(ICustomAttributeProvider provider) where T : class
        {
            object[] attribs = provider.GetCustomAttributes(typeof(T), true);

            if (attribs.Length == 0)
                return null;
            else if (attribs.Length == 1)
                return (T)attribs[0];
            else
                throw new InvalidOperationException(String.Format(CultureInfo.InvariantCulture,
                    "There are multiple instances of attribute '{0}'.", typeof(T).FullName)); 
        }

        /// <summary>
        /// Finds the assembly name of the directly referenced assembly with the specified display name.
        /// </summary>
        /// <param name="assembly">The assembly to search</param>
        /// <param name="displayName">The display name of the referenced assembly to find</param>
        /// <returns>The referenced assembly name or null if none</returns>
        public static AssemblyName FindAssemblyReference(Assembly assembly, string displayName)
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
        /// <returns>A map of the input assemblies indexed by the full assembly name of the desired reference</returns>
        public static IMultiMap<AssemblyName, Assembly> MapByAssemblyReference(IEnumerable<Assembly> assemblies, string displayName)
        {
            MultiMap<AssemblyName, Assembly> map = new MultiMap<AssemblyName, Assembly>();
            foreach (Assembly assembly in assemblies)
            {
                AssemblyName reference = FindAssemblyReference(assembly, displayName);
                if (reference != null)
                    map.Add(reference, assembly);
            }

            return map;
        }

        /// <summary>
        /// Determines if the type can be instantiated using a public constructor.
        /// </summary>
        /// <param name="type">The type</param>
        /// <returns>True if the type can be instantiated</returns>
        public static bool CanInstantiate(Type type)
        {
            return !type.IsAbstract && type.IsClass && type.IsVisible
                && !type.HasElementType && type.GetConstructors().Length != 0;
        }

        /// <summary>
        /// Determines if the method is public, non-static and is non-abstract so it can be invoked.
        /// </summary>
        /// <param name="method">The method</param>
        /// <returns>True if the method can be invoked</returns>
        public static bool CanInvokeNonStatic(MethodInfo method)
        {
            return !method.IsAbstract && method.IsPublic && !method.IsStatic;
        }

        /// <summary>
        /// Determines if the property has public, non-static and non-abstract getter
        /// and setter functions.
        /// </summary>
        /// <param name="property">The property</param>
        /// <returns>True if the property can be get and set</returns>
        public static bool CanGetAndSetNonStatic(PropertyInfo property)
        {
            return property.CanRead && property.CanWrite
                && CanInvokeNonStatic(property.GetGetMethod())
                    && CanInvokeNonStatic(property.GetSetMethod());
        }

        /// <summary>
        /// Sorts an array of members that all belong to the same type
        /// such that the members declared by supertypes appear before those
        /// declared by subtypes.
        /// </summary>
        /// <example>
        /// If type A derives from types B and C then given methods
        /// A.Foo, A.Bar, B.Foo, C.Quux one possible sorted order will be:
        /// B.Foo, C.Quux, A.Bar, A.Foo.  The members are not sorted by name or
        /// by any other criterion except by relative specificity of the
        /// declaring types.
        /// </example>
        public static void SortMembersBySubTypes<T>(T[] members)
            where T : MemberInfo
        {
            Array.Sort(members, delegate(T a, T b)
            {
                Type ta = a.DeclaringType, tb = b.DeclaringType;
                if (ta != tb)
                {
                    if (ta.IsAssignableFrom(tb))
                        return -1;
                    if (tb.IsAssignableFrom(ta))
                        return 1;
                }
                return 0;
            });
        }

        /// <summary>
        /// <para>
        /// Populates the provided metadata map with asembly-level metadata derived
        /// from custom attributes.
        /// </para>
        /// <para>
        /// Currently recognized attributes:
        /// <list type="bullet">
        /// <item><see cref="AssemblyCompanyAttribute" /></item>
        /// <item><see cref="AssemblyConfigurationAttribute" /></item>
        /// <item><see cref="AssemblyCopyrightAttribute" /></item>
        /// <item><see cref="AssemblyDescriptionAttribute" /></item>
        /// <item><see cref="AssemblyFileVersionAttribute" /></item>
        /// <item><see cref="AssemblyInformationalVersionAttribute" /></item>
        /// <item><see cref="AssemblyProductAttribute" /></item>
        /// <item><see cref="AssemblyTitleAttribute" /></item>
        /// <item><see cref="AssemblyTrademarkAttribute" /></item>
        /// <item><see cref="AssemblyVersionAttribute" /></item>
        /// </list>
        /// </para>
        /// </summary>
        /// <param name="assembly">The assembly</param>
        /// <param name="metadataMap">The metadata map</param>
        public static void PopulateMetadataFromAssembly(Assembly assembly, MetadataMap metadataMap)
        {
            AssemblyCompanyAttribute companyAttribute = GetAttribute<AssemblyCompanyAttribute>(assembly);
            if (companyAttribute != null)
                AddMetadataIfNotEmptyOrNull(metadataMap, MetadataKeys.Company, companyAttribute.Company);

            AssemblyConfigurationAttribute configurationAttribute = GetAttribute<AssemblyConfigurationAttribute>(assembly);
            if (configurationAttribute != null)
                AddMetadataIfNotEmptyOrNull(metadataMap, MetadataKeys.Configuration, configurationAttribute.Configuration);

            AssemblyCopyrightAttribute copyrightAttribute = GetAttribute<AssemblyCopyrightAttribute>(assembly);
            if (copyrightAttribute != null)
                AddMetadataIfNotEmptyOrNull(metadataMap, MetadataKeys.Copyright, copyrightAttribute.Copyright);

            AssemblyDescriptionAttribute descriptionAttribute = GetAttribute<AssemblyDescriptionAttribute>(assembly);
            if (descriptionAttribute != null)
                AddMetadataIfNotEmptyOrNull(metadataMap, MetadataKeys.Description, descriptionAttribute.Description);

            AssemblyFileVersionAttribute fileVersionAttribute = GetAttribute<AssemblyFileVersionAttribute>(assembly);
            if (fileVersionAttribute != null)
                AddMetadataIfNotEmptyOrNull(metadataMap, MetadataKeys.FileVersion, fileVersionAttribute.Version);

            AssemblyInformationalVersionAttribute informationalVersionAttribute = GetAttribute<AssemblyInformationalVersionAttribute>(assembly);
            if (informationalVersionAttribute != null)
                AddMetadataIfNotEmptyOrNull(metadataMap, MetadataKeys.InformationalVersion, informationalVersionAttribute.InformationalVersion);

            AssemblyProductAttribute productAttribute = GetAttribute<AssemblyProductAttribute>(assembly);
            if (productAttribute != null)
                AddMetadataIfNotEmptyOrNull(metadataMap, MetadataKeys.Product, productAttribute.Product);

            AssemblyTitleAttribute titleAttribute = GetAttribute<AssemblyTitleAttribute>(assembly);
            if (titleAttribute != null)
                AddMetadataIfNotEmptyOrNull(metadataMap, MetadataKeys.Title, titleAttribute.Title);

            AssemblyTrademarkAttribute trademarkAttribute = GetAttribute<AssemblyTrademarkAttribute>(assembly);
            if (trademarkAttribute != null)
                AddMetadataIfNotEmptyOrNull(metadataMap, MetadataKeys.Trademark, trademarkAttribute.Trademark);

            // Note: AssemblyVersionAttribute cannot be accessed directly via reflection.  It gets baked into the assembly name.
            metadataMap.Add(MetadataKeys.Version, assembly.GetName().Version.ToString());
        }

        private static void AddMetadataIfNotEmptyOrNull(MetadataMap metadataMap, string key, string value)
        {
            if (! string.IsNullOrEmpty(value))
                metadataMap.Add(key, value);
        }
    }
}
