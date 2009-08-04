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
using Gallio.Model;
using Gallio.Common.Reflection;
using Gallio.Model.Tree;

namespace Gallio.NUnitAdapter.Model
{
    /// <summary>
    /// Explores tests in NUnit assemblies using a reflection-based emulation.
    /// </summary>
    internal class NUnitReflectiveTestExplorerEngine : NUnitTestExplorerEngine
    {
        private readonly TestModel testModel;
        private readonly IAssemblyInfo assembly;

        private Test assemblyTest;
        private bool fullyPopulated;

        public NUnitReflectiveTestExplorerEngine(TestModel testModel, IAssemblyInfo assembly)
        {
            this.testModel = testModel;
            this.assembly = assembly;
        }

        public override Test GetAssemblyTest()
        {
            return assemblyTest;
        }

        public override void ExploreAssembly(bool skipChildren)
        {
            if (assemblyTest == null)
            {
                assemblyTest = BuildAssemblyTest(testModel.RootTest);
            }

            if (!skipChildren && !fullyPopulated)
            {
                foreach (ITypeInfo type in assembly.GetExportedTypes())
                    BuildFixturesFromType(assemblyTest, type);

                fullyPopulated = true;
            }
        }

        public override void ExploreType(ITypeInfo type)
        {
            if (fullyPopulated)
                return;

            BuildFixturesFromType(assemblyTest, type);
        }

        private Test BuildAssemblyTest(Test parent)
        {
            testModel.AddAnnotation(new Annotation(AnnotationType.Error, assembly, "Reflection based test explorer for NUnit not yet implemented."));

            Test assemblyTest = new Test(assembly.Name, assembly);
            PopulateAssemblyTestMetadata(assemblyTest, assembly);

            parent.AddChild(assemblyTest);
            return assemblyTest;
        }


        private void BuildFixturesFromType(Test parent, ITypeInfo type)
        {
            try
            {
                // TODO
            }
            catch (Exception ex)
            {
                testModel.AddAnnotation(new Annotation(AnnotationType.Error, type,
                    "An exception was thrown while exploring an MbUnit v2 test type.", ex));
            }
        }
    }
}
