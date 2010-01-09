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
using Gallio.Common.Collections;
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
        private readonly List<NUnit.Core.TestSuite> nunitFixtures;

        private bool fullyPopulated;
        private HashSet<ITypeInfo> populatedTypes;

        public NUnitReflectiveTestExplorerEngine(TestModel testModel, IAssemblyInfo assembly)
        {
            this.testModel = testModel;
            this.assembly = assembly;

            nunitFixtures = new List<NUnit.Core.TestSuite>();
        }

        public override void ExploreAssembly(bool skipChildren)
        {
            if (!NUnit.Core.CoreExtensions.Host.Initialized)
                NUnit.Core.CoreExtensions.Host.InitializeService();

            if (!skipChildren && !fullyPopulated)
            {
                foreach (ITypeInfo type in assembly.GetExportedTypes())
                    ExploreTypeIfNotAlreadyPopulated(type);

                fullyPopulated = true;
            }
        }

        public override void ExploreType(ITypeInfo type)
        {
            if (fullyPopulated)
                return;

            ExploreTypeIfNotAlreadyPopulated(type);
        }

        public override void Finish()
        {
            var nunitNamespaceTreeBuilder = new NUnit.Core.NamespaceTreeBuilder(new NUnit.Core.TestSuite(assembly.Name));
            foreach (var nunitFixture in nunitFixtures)
                nunitNamespaceTreeBuilder.Add(nunitFixture);
            NUnit.Core.TestSuite nunitRootSuite = nunitNamespaceTreeBuilder.RootSuite;

            var assemblyTest = new NUnitTest(assembly.Name, assembly, nunitRootSuite);
            PopulateMetadata(assemblyTest);
            PopulateAssemblyTestMetadata(assemblyTest, assembly);
            testModel.RootTest.AddChild(assemblyTest);

            BuildTestChildren(assembly, assemblyTest, nunitRootSuite);
        }

        private void ExploreTypeIfNotAlreadyPopulated(ITypeInfo type)
        {
            if (populatedTypes == null)
            {
                populatedTypes = new HashSet<ITypeInfo>();
            }
            else if (populatedTypes.Contains(type))
            {
                return;
            }

            BuildNUnitFixturesFromType(type);
            populatedTypes.Add(type);
        }

        private void BuildNUnitFixturesFromType(ITypeInfo type)
        {
            try
            {
                // Note: This code takes advantage of the fact that ITypeInfo.Resolve will
                //       return an UnresolvedType object which adapts ITypeInfo to Type in a
                //       way that enables the native NUnit builders to perform the needed
                //       reflection in most cases.
                Type resolvedType = type.Resolve(false);

                if (NUnit.Core.TestFixtureBuilder.CanBuildFrom(resolvedType))
                {
                    var nunitFixture = (NUnit.Core.TestSuite) NUnit.Core.TestFixtureBuilder.BuildFrom(resolvedType);
                    nunitFixtures.Add(nunitFixture);
                }
            }
            catch (Exception ex)
            {
                testModel.AddAnnotation(new Annotation(AnnotationType.Error, type,
                    "An exception was thrown while exploring an NUnit test type.  This probably indicates that the type uses certain NUnit features that are not supported by the reflection-only test explorer engine.", ex));
            }
        }
    }
}
