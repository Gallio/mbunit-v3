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

extern alias MbUnit2;
using System;
using System.Collections.Generic;
using System.Reflection;
using Gallio.Model;
using Gallio.Common.Reflection;

using FixtureExplorer = MbUnit2::MbUnit.Core.Remoting.FixtureExplorer;
using RunPipe = MbUnit2::MbUnit.Core.RunPipe;
using RunPipeStarter = MbUnit2::MbUnit.Core.RunPipeStarter;
using Fixture = MbUnit2::MbUnit.Core.Fixture;
using AnyFixtureFilter = MbUnit2::MbUnit.Core.Filters.AnyFixtureFilter;
using AnyRunPipeFilter = MbUnit2::MbUnit.Core.Filters.AnyRunPipeFilter;
using RunInvokerVertex = MbUnit2::MbUnit.Core.Invokers.RunInvokerVertex;
using IRunInvoker = MbUnit2::MbUnit.Core.Invokers.IRunInvoker;

namespace Gallio.MbUnit2Adapter.Model
{
    /// <summary>
    /// Builds a test tree using the MbUnit v2 framework itself to
    /// perform reflection.  This mechanism can only be used when the
    /// test assemblies have been loaded into the AppDomain and are executable.
    /// </summary>
    /// <seealso cref="MbUnit2ReflectiveTestExplorer" />
    internal static class MbUnit2NativeTestExplorer
    {
        public static ITest BuildAssemblyTest(Assembly assembly, ICollection<KeyValuePair<ITest, string>> unresolvedDependencies)
        {
            FixtureExplorer fixtureExplorer = InitializeFixtureExplorer(assembly);
            MbUnit2AssemblyTest assemblyTest = CreateAssemblyTest(fixtureExplorer, Reflector.Wrap(assembly));

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

            return assemblyTest;
        }

        private static FixtureExplorer InitializeFixtureExplorer(Assembly assembly)
        {
            FixtureExplorer fixtureExplorer = new FixtureExplorer(assembly);
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

            MbUnit2MetadataUtils.PopulateAssemblyMetadata(test, assembly);
            return test;
        }

        private static MbUnit2Test CreateFixtureTest(Fixture fixture)
        {
            ITypeInfo fixtureType = Reflector.Wrap(fixture.Type);
            MbUnit2Test test = new MbUnit2Test(fixtureType.Name, fixtureType, fixture, null);
            test.Kind = TestKinds.Fixture;

            MbUnit2MetadataUtils.PopulateFixtureMetadata(test, fixtureType);
            return test;
        }

        private static MbUnit2Test CreateTest(RunPipe runPipe)
        {
            IMemberInfo member = GuessMemberInfoFromRunPipe(runPipe);
            ICodeElementInfo codeElement = member ?? Reflector.Wrap(runPipe.FixtureType);

            MbUnit2Test test = new MbUnit2Test(runPipe.ShortName, codeElement, runPipe.Fixture, runPipe);
            test.Kind = TestKinds.Test;
            test.IsTestCase = true;

            if (member != null)
                MbUnit2MetadataUtils.PopulateTestMetadata(test, member);

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
