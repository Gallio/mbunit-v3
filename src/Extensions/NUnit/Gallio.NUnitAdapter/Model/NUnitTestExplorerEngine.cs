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
using System.Text;
using Gallio.Common.Reflection;
using Gallio.Model;
using Gallio.Model.Tree;
using Gallio.NUnitAdapter.Properties;

namespace Gallio.NUnitAdapter.Model
{
    internal abstract class NUnitTestExplorerEngine
    {
        private const string NUnitFrameworkAssemblyDisplayName = @"nunit.framework";

        public abstract Test GetAssemblyTest();

        public abstract void ExploreAssembly(bool skipChildren);

        public virtual void ExploreType(ITypeInfo type)
        {
        }

        protected void PopulateAssemblyTestMetadata(Test assemblyTest, IAssemblyInfo assembly)
        {
            ModelUtils.PopulateMetadataFromAssembly(assembly, assemblyTest.Metadata);

            Version frameworkVersion = GetFrameworkVersion(assembly);

            string frameworkName = String.Format(Resources.NUnitTestExplorer_FrameworkNameWithVersionFormat, frameworkVersion);
            assemblyTest.Metadata.SetValue(MetadataKeys.Framework, frameworkName);
            assemblyTest.Metadata.SetValue(MetadataKeys.File, assembly.Path);
            assemblyTest.Kind = NUnitTestExplorer.AssemblyKind;
        }

        private static Version GetFrameworkVersion(IAssemblyInfo assembly)
        {
            AssemblyName frameworkAssemblyName = ReflectionUtils.FindAssemblyReference(assembly, NUnitFrameworkAssemblyDisplayName);
            return frameworkAssemblyName != null ? frameworkAssemblyName.Version : null;
        }
    }
}
