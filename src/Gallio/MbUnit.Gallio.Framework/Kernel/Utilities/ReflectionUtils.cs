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
using MbUnit.Framework.Kernel.Collections;
using MbUnit.Framework.Kernel.Model;

namespace MbUnit.Framework.Kernel.Utilities
{
    /// <summary>
    /// Provides helpers for performing reflection.
    /// </summary>
    public static class ReflectionUtils
    {
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
        /// Finds the assembly name of the referenced assembly with the specified display name.
        /// Recursively searches referenced assemblies beginning with the one specified as a parameter.
        /// Loads referenced assemblies on demand to resolve names.
        /// </summary>
        /// <param name="assembly">The assembly whose references are to be scanned</param>
        /// <param name="displayName">The display name of the assembly to search for</param>
        /// <returns>The referenced assembly name or null if none</returns>
        public static AssemblyName FindReferencedAssembly(Assembly assembly, string displayName)
        {
            return FindReferencedAssembly(assembly, displayName, new Dictionary<string, bool>());
        }

        /// <summary>
        /// Searches a list of assemblies for all of those that contain an assembly reference with
        /// the specified display name.  Produces a map from the versions of the referenced
        /// assemblies to the source assemblies specified.
        /// </summary>
        /// <param name="assemblies">The assemblies to search</param>
        /// <param name="displayName">The display name of the referenced assembly to search for</param>
        /// <returns>The reverse reference map</returns>
        public static MultiMap<Version, Assembly> GetReverseAssemblyReferenceMap(IList<Assembly> assemblies, string displayName)
        {
            MultiMap<Version, Assembly> map = new MultiMap<Version, Assembly>();

            foreach (Assembly assembly in assemblies)
            {
                AssemblyName reference = FindReferencedAssembly(assembly, displayName);
                if (reference != null)
                    map.Add(reference.Version, assembly);
            }

            return map;
        }

        private static AssemblyName FindReferencedAssembly(Assembly assembly, string displayName,
                                                           Dictionary<string, bool> visitedSet)
        {
            foreach (AssemblyName reference in assembly.GetReferencedAssemblies())
            {
                if (reference.Name == displayName)
                    return reference;

                if (!visitedSet.ContainsKey(reference.FullName))
                {
                    visitedSet.Add(reference.FullName, false);

                    Assembly referencedAssembly;
                    try
                    {
                        referencedAssembly = Assembly.ReflectionOnlyLoad(reference.FullName);
                    }
                    catch (Exception ex)
                    {
                        // Ignore failures to load the referenced assembly.
                        // Obviously the referenced assembly wasn't found there...
                        Debug.WriteLine(String.Format(CultureInfo.CurrentCulture,
                            "Could not scan references of assembly '{0}'.\n{1}", reference.FullName, ex));
                        continue;
                    }

                    AssemblyName result = FindReferencedAssembly(referencedAssembly, displayName, visitedSet);
                    if (result != null)
                        return result;
                }
            }

            return null;
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
            metadataMap.Entries.Add(MetadataKeys.Version, assembly.GetName().Version.ToString());
        }

        private static void AddMetadataIfNotEmptyOrNull(MetadataMap metadataMap, string key, string value)
        {
            if (! string.IsNullOrEmpty(value))
                metadataMap.Entries.Add(key, value);
        }
    }
}