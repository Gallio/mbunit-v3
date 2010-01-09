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
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Gallio.Common.Reflection;
using Gallio.MbUnit2Adapter.Properties;
using Gallio.Model;
using Gallio.Model.Tree;

namespace Gallio.MbUnit2Adapter.Model
{
    internal abstract class MbUnit2TestExplorerEngine
    {
        private const string MbUnitFrameworkAssemblyDisplayName = @"MbUnit.Framework";

        public abstract Test GetAssemblyTest();

        public abstract void ExploreAssembly(bool skipChildren, ICollection<KeyValuePair<Test, string>> unresolvedDependencies);

        public virtual void ExploreType(ITypeInfo type)
        {
        }

        protected void PopulateAssemblyTestMetadata(Test assemblyTest, IAssemblyInfo assembly)
        {
            MbUnit2MetadataUtils.PopulateAssemblyMetadata(assemblyTest, assembly);

            Version frameworkVersion = GetFrameworkVersion(assembly);

            string frameworkName = String.Format(Resources.MbUnit2TestExplorer_FrameworkNameWithVersionFormat,
                frameworkVersion);
            assemblyTest.Metadata.SetValue(MetadataKeys.Framework, frameworkName);
            assemblyTest.Metadata.SetValue(MetadataKeys.File, assembly.Path);
            assemblyTest.Kind = MbUnit2TestExplorer.AssemblyKind;
        }

        private static Version GetFrameworkVersion(IAssemblyInfo assembly)
        {
            AssemblyName frameworkAssemblyName = ReflectionUtils.FindAssemblyReference(assembly, MbUnitFrameworkAssemblyDisplayName);
            return frameworkAssemblyName != null ? frameworkAssemblyName.Version : null;
        }
    }
}
