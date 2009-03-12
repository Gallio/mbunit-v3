// Copyright 2005-2009 Gallio Project - http://www.gallio.org/
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
using System.Collections.Generic;
using System.Reflection;
using Gallio.Collections;
using Gallio.Reflection;
using Gallio.Properties;

namespace Gallio.Model
{
    /// <summary>
    /// Provides utility functions for manipulating the object model.
    /// </summary>
    public static class ModelUtils
    {
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
        public static void PopulateMetadataFromAssembly(IAssemblyInfo assembly, PropertyBag metadataMap)
        {
            metadataMap.Add(MetadataKeys.CodeBase, assembly.Path);

            AssemblyCompanyAttribute companyAttribute = AttributeUtils.GetAttribute<AssemblyCompanyAttribute>(assembly, false);
            if (companyAttribute != null)
                AddMetadataIfNotEmptyOrNull(metadataMap, MetadataKeys.Company, companyAttribute.Company);

            AssemblyConfigurationAttribute configurationAttribute = AttributeUtils.GetAttribute<AssemblyConfigurationAttribute>(assembly, false);
            if (configurationAttribute != null)
                AddMetadataIfNotEmptyOrNull(metadataMap, MetadataKeys.Configuration, configurationAttribute.Configuration);

            AssemblyCopyrightAttribute copyrightAttribute = AttributeUtils.GetAttribute<AssemblyCopyrightAttribute>(assembly, false);
            if (copyrightAttribute != null)
                AddMetadataIfNotEmptyOrNull(metadataMap, MetadataKeys.Copyright, copyrightAttribute.Copyright);

            AssemblyDescriptionAttribute descriptionAttribute = AttributeUtils.GetAttribute<AssemblyDescriptionAttribute>(assembly, false);
            if (descriptionAttribute != null)
                AddMetadataIfNotEmptyOrNull(metadataMap, MetadataKeys.Description, descriptionAttribute.Description);

            AssemblyFileVersionAttribute fileVersionAttribute = AttributeUtils.GetAttribute<AssemblyFileVersionAttribute>(assembly, false);
            if (fileVersionAttribute != null)
                AddMetadataIfNotEmptyOrNull(metadataMap, MetadataKeys.FileVersion, fileVersionAttribute.Version);

            AssemblyInformationalVersionAttribute informationalVersionAttribute = AttributeUtils.GetAttribute<AssemblyInformationalVersionAttribute>(assembly, false);
            if (informationalVersionAttribute != null)
                AddMetadataIfNotEmptyOrNull(metadataMap, MetadataKeys.InformationalVersion, informationalVersionAttribute.InformationalVersion);

            AssemblyProductAttribute productAttribute = AttributeUtils.GetAttribute<AssemblyProductAttribute>(assembly, false);
            if (productAttribute != null)
                AddMetadataIfNotEmptyOrNull(metadataMap, MetadataKeys.Product, productAttribute.Product);

            AssemblyTitleAttribute titleAttribute = AttributeUtils.GetAttribute<AssemblyTitleAttribute>(assembly, false);
            if (titleAttribute != null)
                AddMetadataIfNotEmptyOrNull(metadataMap, MetadataKeys.Title, titleAttribute.Title);

            AssemblyTrademarkAttribute trademarkAttribute = AttributeUtils.GetAttribute<AssemblyTrademarkAttribute>(assembly, false);
            if (trademarkAttribute != null)
                AddMetadataIfNotEmptyOrNull(metadataMap, MetadataKeys.Trademark, trademarkAttribute.Trademark);

            // Note: AssemblyVersionAttribute cannot be accessed directly via reflection.  It gets baked into the assembly name.
            metadataMap.Add(MetadataKeys.Version, assembly.GetName().Version.ToString());
        }

        private static void AddMetadataIfNotEmptyOrNull(PropertyBag metadataMap, string key, string value)
        {
            if (! string.IsNullOrEmpty(value))
                metadataMap.Add(key, value);
        }
    }
}
