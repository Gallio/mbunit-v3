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

extern alias MbUnit2;

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using MbUnit.Framework;
using MbUnit.Framework.Kernel.Metadata;
using MbUnit.Framework.Kernel.Model;

using TestFixturePatternAttribute2 = MbUnit2::MbUnit.Core.Framework.TestFixturePatternAttribute;
using AuthorAttribute2 = MbUnit2::MbUnit.Framework.AuthorAttribute;
using FixtureCategoryAttribute2 = MbUnit2::MbUnit.Framework.FixtureCategoryAttribute;
using TestCategoryAttribute2 = MbUnit2::MbUnit.Framework.TestCategoryAttribute;
using TestsOnAttribute2 = MbUnit2::MbUnit.Framework.TestsOnAttribute;
using ImportanceAttribute2 = MbUnit2::MbUnit.Framework.ImportanceAttribute;
using TestImportance2 = MbUnit2::MbUnit.Framework.TestImportance;

using MbUnit2::MbUnit.Core;
using MbUnit2::MbUnit.Core.Remoting;
using MbUnit2::MbUnit.Core.Filters;
using MbUnit2::MbUnit.Core.Reports.Serialization;

namespace MbUnit.Plugin.MbUnit2Adapter.Core
{
    /// <summary>
    /// The MbUnit v2 test assembly template binding.
    /// This binding performs full exploration of all tests in MbUnit v2 test
    /// assemblies during test construction.
    /// </summary>
    public class MbUnit2AssemblyTemplateBinding : BaseTemplateBinding
    {
        private FixtureExplorer fixtureExplorer;

        /// <summary>
        /// Creates a template binding.
        /// </summary>
        /// <param name="template">The template that was bound</param>
        /// <param name="scope">The scope in which the binding occurred</param>
        /// <param name="arguments">The template arguments</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="template"/>,
        /// <paramref name="scope"/> or <paramref name="arguments"/> is null</exception>
        public MbUnit2AssemblyTemplateBinding(MbUnit2AssemblyTemplate template, TestScope scope,
            IDictionary<ITemplateParameter, object> arguments)
            : base(template, scope, arguments)
        {
        }

        /// <summary>
        /// Gets the assembly.
        /// </summary>
        public Assembly Assembly
        {
            get { return ((MbUnit2AssemblyTemplate) Template).Assembly; }
        }

        /// <inheritdoc />
        public override void BuildTests(TestTreeBuilder builder)
        {
            RunFixtureExplorerIfNeeded();

            MbUnit2Test assemblyTest = CreateAssemblyTest(Scope, Assembly);
            assemblyTest.Batch = new TestBatch("MbUnit v2: " + Template.Name, delegate
            {
                return new MbUnit2TestController(fixtureExplorer);
            });

            foreach (Fixture fixture in fixtureExplorer.FixtureGraph.Fixtures)
            {
                MbUnit2Test fixtureTest = CreateFixtureTest(assemblyTest.Scope, fixture);

                foreach (RunPipeStarter starter in fixture.Starters)
                {
                    MbUnit2Test test = CreateTest(fixtureTest.Scope, starter.Pipe);
                }
            }

            // TODO: Use GetDependentAssemblies() to handle assembly dependencies
        }

        private void RunFixtureExplorerIfNeeded()
        {
            if (fixtureExplorer != null)
                return;

            try
            {
                fixtureExplorer = new FixtureExplorer(Assembly);
                fixtureExplorer.Filter = new AnyFixtureFilter();
                fixtureExplorer.Explore();

                AnyRunPipeFilter runPipeFilter = new AnyRunPipeFilter();
                foreach (Fixture fixture in fixtureExplorer.FixtureGraph.Fixtures)
                    fixture.Load(runPipeFilter);
            }
            catch (Exception)
            {
                fixtureExplorer = null;
                throw;
            }
        }

        private MbUnit2Test CreateAssemblyTest(TestScope parentScope, Assembly assembly)
        {
            MbUnit2Test test = new MbUnit2Test(assembly.FullName, CodeReference.CreateFromAssembly(assembly), parentScope, null, null);
            test.Kind = ComponentKind.Assembly;

            // TODO: Is there any metadata on assemblies that we need?

            parentScope.ContainingTest.AddChild(test);
            return test;
        }

        private MbUnit2Test CreateFixtureTest(TestScope parentScope, Fixture fixture)
        {
            Type fixtureType = fixture.Type;
            MbUnit2Test test = new MbUnit2Test(fixture.Name, CodeReference.CreateFromType(fixtureType), parentScope, fixture, null);
            test.Kind = ComponentKind.Fixture;

            // Populate metadata
            foreach (AuthorAttribute2 attrib in fixtureType.GetCustomAttributes(typeof(AuthorAttribute2), true))
            {
                if (! String.IsNullOrEmpty(attrib.Name))
                    test.Metadata.Entries.Add(MetadataConstants.AuthorNameKey, attrib.Name);
                if (! String.IsNullOrEmpty(attrib.EMail) && attrib.EMail != "unspecified")
                    test.Metadata.Entries.Add(MetadataConstants.AuthorEmailKey, attrib.EMail);
                if (!String.IsNullOrEmpty(attrib.HomePage) && attrib.HomePage != "unspecified")
                    test.Metadata.Entries.Add(MetadataConstants.AuthorHomepageKey, attrib.HomePage);
            }
            foreach (FixtureCategoryAttribute2 attrib in fixtureType.GetCustomAttributes(typeof(FixtureCategoryAttribute2), true))
            {
                test.Metadata.Entries.Add(MetadataConstants.CategoryNameKey, attrib.Category);
            }
            foreach (TestsOnAttribute2 attrib in fixtureType.GetCustomAttributes(typeof(TestsOnAttribute2), true))
            {
                test.Metadata.Entries.Add(MetadataConstants.TestsOnKey, attrib.TestedType.AssemblyQualifiedName);
            }
            foreach (ImportanceAttribute2 attrib in fixtureType.GetCustomAttributes(typeof(ImportanceAttribute2), true))
            {
                // Note: In principle we could eliminate the call to MapImportance because TestImportance is
                //       defined the same way in Gallio as in MbUnit v2.  But there is no guarantee that will remain the case.
                test.Metadata.Entries.Add(MetadataConstants.TestsOnKey, MapImportance(attrib.Importance).ToString());
            }
            foreach (TestFixturePatternAttribute2 attrib in fixtureType.GetCustomAttributes(typeof(TestFixturePatternAttribute2), true))
            {
                if (! String.IsNullOrEmpty(attrib.Description))
                    test.Metadata.Entries.Add(MetadataConstants.DescriptionKey, attrib.Description);
            }

            parentScope.ContainingTest.AddChild(test);
            return test;
        }

        private MbUnit2Test CreateTest(TestScope parentScope, RunPipe runPipe)
        {
            MbUnit2Test test = new MbUnit2Test(runPipe.Name, CodeReference.CreateFromType(runPipe.FixtureType), parentScope, runPipe.Fixture, runPipe);
            test.Kind = ComponentKind.Test;

            // TODO: How can we populate the metadata?
            //       We don't even know the MethodInfo!

            parentScope.ContainingTest.AddChild(test);
            return test;
        }

        private TestImportance MapImportance(TestImportance2 importance)
        {
            switch (importance)
            {
                case TestImportance2.Critical:
                    return TestImportance.Critical;

                case TestImportance2.Severe:
                    return TestImportance.Severe;

                case TestImportance2.Serious:
                    return TestImportance.Serious;

                case TestImportance2.NoOneReallyCaresAbout:
                    return TestImportance.NoOneReallyCaresAbout;

                default:
                case TestImportance2.Default:
                    return TestImportance.Default;
            }
        }
    }
}
