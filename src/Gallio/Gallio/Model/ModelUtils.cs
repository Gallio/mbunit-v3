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
using System.Reflection;
using Gallio.Model.Reflection;
using Gallio.Properties;

namespace Gallio.Model
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
        public static void PopulateMetadataFromAssembly(IAssemblyInfo assembly, MetadataMap metadataMap)
        {
            metadataMap.Add(MetadataKeys.CodeBase, assembly.Path);

            AssemblyCompanyAttribute companyAttribute = assembly.GetAttribute<AssemblyCompanyAttribute>(false);
            if (companyAttribute != null)
                AddMetadataIfNotEmptyOrNull(metadataMap, MetadataKeys.Company, companyAttribute.Company);

            AssemblyConfigurationAttribute configurationAttribute = assembly.GetAttribute<AssemblyConfigurationAttribute>(false);
            if (configurationAttribute != null)
                AddMetadataIfNotEmptyOrNull(metadataMap, MetadataKeys.Configuration, configurationAttribute.Configuration);

            AssemblyCopyrightAttribute copyrightAttribute = assembly.GetAttribute<AssemblyCopyrightAttribute>(false);
            if (copyrightAttribute != null)
                AddMetadataIfNotEmptyOrNull(metadataMap, MetadataKeys.Copyright, copyrightAttribute.Copyright);

            AssemblyDescriptionAttribute descriptionAttribute = assembly.GetAttribute<AssemblyDescriptionAttribute>(false);
            if (descriptionAttribute != null)
                AddMetadataIfNotEmptyOrNull(metadataMap, MetadataKeys.Description, descriptionAttribute.Description);

            AssemblyFileVersionAttribute fileVersionAttribute = assembly.GetAttribute<AssemblyFileVersionAttribute>(false);
            if (fileVersionAttribute != null)
                AddMetadataIfNotEmptyOrNull(metadataMap, MetadataKeys.FileVersion, fileVersionAttribute.Version);

            AssemblyInformationalVersionAttribute informationalVersionAttribute = assembly.GetAttribute<AssemblyInformationalVersionAttribute>(false);
            if (informationalVersionAttribute != null)
                AddMetadataIfNotEmptyOrNull(metadataMap, MetadataKeys.InformationalVersion, informationalVersionAttribute.InformationalVersion);

            AssemblyProductAttribute productAttribute = assembly.GetAttribute<AssemblyProductAttribute>(false);
            if (productAttribute != null)
                AddMetadataIfNotEmptyOrNull(metadataMap, MetadataKeys.Product, productAttribute.Product);

            AssemblyTitleAttribute titleAttribute = assembly.GetAttribute<AssemblyTitleAttribute>(false);
            if (titleAttribute != null)
                AddMetadataIfNotEmptyOrNull(metadataMap, MetadataKeys.Title, titleAttribute.Title);

            AssemblyTrademarkAttribute trademarkAttribute = assembly.GetAttribute<AssemblyTrademarkAttribute>(false);
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
