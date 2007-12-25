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
using Gallio.Model;
using Gallio.Reflection;
using Gallio.Plugin.MbUnit2Adapter.Properties;

using TestFixturePatternAttribute2 = MbUnit2::MbUnit.Core.Framework.TestFixturePatternAttribute;
using TestPatternAttribute2 = MbUnit2::MbUnit.Core.Framework.TestPatternAttribute;
using FixtureCategoryAttribute2 = MbUnit2::MbUnit.Framework.FixtureCategoryAttribute;
using TestCategoryAttribute2 = MbUnit2::MbUnit.Framework.TestCategoryAttribute;
using TestImportance2 = MbUnit2::MbUnit.Framework.TestImportance;
using AuthorAttribute2 = MbUnit2::MbUnit.Framework.AuthorAttribute;
using TestsOnAttribute2 = MbUnit2::MbUnit.Framework.TestsOnAttribute;
using ImportanceAttribute2 = MbUnit2::MbUnit.Framework.ImportanceAttribute;
using MbUnit2::MbUnit.Core;
using MbUnit2::MbUnit.Core.Remoting;
using MbUnit2::MbUnit.Core.Filters;
using MbUnit2::MbUnit.Core.Invokers;

namespace Gallio.Plugin.MbUnit2Adapter.Model
{
    /// <summary>
    /// Explores tests in MbUnit v2 assemblies.
    /// </summary>
    internal class MbUnit2TestExplorer : BaseTestExplorer
    {
        private const string MbUnitFrameworkAssemblyDisplayName = @"MbUnit.Framework";

        private readonly Dictionary<Version, ITest> frameworkTests;
        private readonly Dictionary<IAssemblyInfo, ITest> assemblyTests;
        private readonly List<KeyValuePair<ITest, string>> unresolvedDependencies;

        public MbUnit2TestExplorer(TestModel testModel)
            : base(testModel)
        {
            frameworkTests = new Dictionary<Version, ITest>();
            assemblyTests = new Dictionary<IAssemblyInfo, ITest>();
            unresolvedDependencies = new List<KeyValuePair<ITest, string>>();
        }

        /// <inheritdoc />
        public override void ExploreAssembly(IAssemblyInfo assembly, Action<ITest> consumer)
        {
            Version frameworkVersion = GetFrameworkVersion(assembly);

            if (frameworkVersion != null)
            {
                ITest frameworkTest = GetFrameworkTest(frameworkVersion, TestModel.RootTest);
                ITest assemblyTest = GetAssemblyTest(assembly, frameworkTest);

                if (consumer != null)
                    consumer(assemblyTest);
            }
        }

        private static Version GetFrameworkVersion(IAssemblyInfo assembly)
        {
            AssemblyName frameworkAssemblyName = ReflectionUtils.FindAssemblyReference(assembly, MbUnitFrameworkAssemblyDisplayName);
            return frameworkAssemblyName != null ? frameworkAssemblyName.Version : null;
        }

        private ITest GetFrameworkTest(Version frameworkVersion, ITest rootTest)
        {
            ITest frameworkTest;
            if (! frameworkTests.TryGetValue(frameworkVersion, out frameworkTest))
            {
                frameworkTest = CreateFrameworkTest(frameworkVersion);
                rootTest.AddChild(frameworkTest);

                frameworkTests.Add(frameworkVersion, frameworkTest);
            }

            return frameworkTest;
        }

        private static ITest CreateFrameworkTest(Version frameworkVersion)
        {
            BaseTest frameworkTest = new BaseTest(
                String.Format(Resources.MbUnit2TestExplorer_FrameworkNameWithVersionFormat, frameworkVersion), null);
            frameworkTest.BaselineLocalId = Resources.MbUnit2TestFramework_FrameworkName;
            frameworkTest.Kind = TestKinds.Framework;

            return frameworkTest;
        }

        private ITest GetAssemblyTest(IAssemblyInfo assembly, ITest frameworkTest)
        {
            ITest assemblyTest;
            if (assemblyTests.TryGetValue(assembly, out assemblyTest))
                return assemblyTest;

            try
            {
                FixtureExplorer fixtureExplorer = InitializeFixtureExplorer(assembly);

                assemblyTest = CreateAssemblyTest(fixtureExplorer, assembly);

                foreach (Fixture fixture in fixtureExplorer.FixtureGraph.Fixtures)
                {
                    MbUnit2Test fixtureTest = CreateFixtureTest(fixture);

                    foreach (RunPipeStarter starter in fixture.Starters)
                    {
                        MbUnit2Test test = CreateTest(starter.Pipe);

                        fixtureTest.AddChild(test);
                    }

                    assemblyTest.AddChild(fixtureTest);
                }

                foreach (string assemblyName in fixtureExplorer.GetDependentAssemblies())
                    unresolvedDependencies.Add(new KeyValuePair<ITest, string>(assemblyTest, assemblyName));
            }
            catch (Exception ex)
            {
                assemblyTest = new ErrorTest(assembly,
                    String.Format("An exception occurred while enumerating MbUnit v2 tests in assembly '{0}'.  {1}", assembly.Name, ex));
            }

            for (int i = 0; i < unresolvedDependencies.Count; i++)
            {
                foreach (KeyValuePair<IAssemblyInfo, ITest> entry in assemblyTests)
                {
                    if (entry.Key.FullName == unresolvedDependencies[i].Value)
                    {
                        unresolvedDependencies[i].Key.AddDependency(entry.Value);
                        unresolvedDependencies.RemoveAt(i--);
                        break;
                    }
                }
            }

            frameworkTest.AddChild(assemblyTest);

            assemblyTests.Add(assembly, assemblyTest);
            return assemblyTest;
        }

        private static FixtureExplorer InitializeFixtureExplorer(IAssemblyInfo assembly)
        {
            FixtureExplorer fixtureExplorer = new FixtureExplorer(assembly.Resolve());
            fixtureExplorer.Filter = new AnyFixtureFilter();
            fixtureExplorer.Explore();

            AnyRunPipeFilter runPipeFilter = new AnyRunPipeFilter();
            foreach (Fixture fixture in fixtureExplorer.FixtureGraph.Fixtures)
                fixture.Load(runPipeFilter);

            return fixtureExplorer;
        }

        private static MbUnit2AssemblyTest CreateAssemblyTest(FixtureExplorer fixtureExplorer, IAssemblyInfo assembly)
        {
            MbUnit2AssemblyTest test = new MbUnit2AssemblyTest(fixtureExplorer, assembly);

            // Add assembly-level metadata.
            ModelUtils.PopulateMetadataFromAssembly(assembly, test.Metadata);

            return test;
        }

        private static MbUnit2Test CreateFixtureTest(Fixture fixture)
        {
            ITypeInfo fixtureType = Reflector.Wrap(fixture.Type);
            MbUnit2Test test = new MbUnit2Test(fixtureType.CompoundName, fixtureType, fixture, null);
            test.Kind = TestKinds.Fixture;

            // Populate metadata
            foreach (AuthorAttribute2 attrib in AttributeUtils.GetAttributes<AuthorAttribute2>(fixtureType, true))
            {
                if (!String.IsNullOrEmpty(attrib.Name))
                    test.Metadata.Add(MetadataKeys.AuthorName, attrib.Name);
                if (!String.IsNullOrEmpty(attrib.EMail) && attrib.EMail != @"unspecified")
                    test.Metadata.Add(MetadataKeys.AuthorEmail, attrib.EMail);
                if (!String.IsNullOrEmpty(attrib.HomePage) && attrib.HomePage != @"unspecified")
                    test.Metadata.Add(MetadataKeys.AuthorHomepage, attrib.HomePage);
            }
            foreach (FixtureCategoryAttribute2 attrib in AttributeUtils.GetAttributes<FixtureCategoryAttribute2>(fixtureType, true))
            {
                test.Metadata.Add(MetadataKeys.CategoryName, attrib.Category);
            }
            foreach (TestsOnAttribute2 attrib in AttributeUtils.GetAttributes<TestsOnAttribute2>(fixtureType, true))
            {
                test.Metadata.Add(MetadataKeys.TestsOn, attrib.TestedType.AssemblyQualifiedName);
            }
            foreach (ImportanceAttribute2 attrib in AttributeUtils.GetAttributes<ImportanceAttribute2>(fixtureType, true))
            {
                test.Metadata.Add(MetadataKeys.Importance, attrib.Importance.ToString());
            }
            foreach (TestFixturePatternAttribute2 attrib in AttributeUtils.GetAttributes<TestFixturePatternAttribute2>(fixtureType, true))
            {
                if (!String.IsNullOrEmpty(attrib.Description))
                    test.Metadata.Add(MetadataKeys.Description, attrib.Description);
            }

            string xmlDocumentation = fixtureType.GetXmlDocumentation();
            if (xmlDocumentation != null)
                test.Metadata.Add(MetadataKeys.XmlDocumentation, xmlDocumentation);

            return test;
        }

        private static MbUnit2Test CreateTest(RunPipe runPipe)
        {
            IMemberInfo member = GuessMemberInfoFromRunPipe(runPipe);
            ICodeElementInfo codeElement = member ?? Reflector.Wrap(runPipe.FixtureType);

            MbUnit2Test test = new MbUnit2Test(runPipe.Name, codeElement, runPipe.Fixture, runPipe);
            test.Kind = TestKinds.Test;
            test.IsTestCase = true;

            // Populate metadata
            if (member != null)
            {
                foreach (TestPatternAttribute2 attrib in AttributeUtils.GetAttributes<TestPatternAttribute2>(member, true))
                {
                    if (!String.IsNullOrEmpty(attrib.Description))
                        test.Metadata.Add(MetadataKeys.Description, attrib.Description);
                }

                string xmlDocumentation = member.GetXmlDocumentation();
                if (xmlDocumentation != null)
                    test.Metadata.Add(MetadataKeys.XmlDocumentation, xmlDocumentation);
            }

            return test;
        }

        /// <summary>
        /// MbUnit v2 does not expose the MemberInfo directly.  Arguably
        /// that allows more general filtering rules than Gallio's simple
        /// CodeReference but it is a bit of a nuisance for us here.
        /// So to avoid breaking the MbUnit v2 API, we resort to a
        /// hack based on guessing the right method.
        /// </summary>
        private static IMemberInfo GuessMemberInfoFromRunPipe(RunPipe runPipe)
        {
            foreach (RunInvokerVertex vertex in runPipe.Invokers)
            {
                if (!vertex.HasInvoker)
                    continue;

                IRunInvoker invoker = vertex.Invoker;
                if (invoker.Generator.IsTest)
                {
                    // Note: This is the hack.
                    //       We assume the run invoker's name matches the name of
                    //       the actual member and that the member is public and is
                    //       declared by the fixture type.  That should be true with
                    //       all built-in MbUnit v2 invokers.  -- Jeff.
                    Type fixtureType = runPipe.FixtureType;
                    string probableMemberName = invoker.Name;

                    // Strip off arguments from a RowTest's member name.  eg. FooMember(123, 456)
                    int parenthesis = probableMemberName.IndexOf('(');
                    if (parenthesis >= 0)
                        probableMemberName = probableMemberName.Substring(0, parenthesis);

                    foreach (MemberInfo member in fixtureType.GetMember(probableMemberName,
                        BindingFlags.Public | BindingFlags.Instance))
                    {
                        if (invoker.ContainsMemberInfo(member))
                            return Reflector.Wrap(member);
                    }
                }
            }

            return null;
        }
    }
}
