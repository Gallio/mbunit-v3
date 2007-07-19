extern alias MbUnit2;

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using MbUnit.Framework.Kernel.Metadata;
using MbUnit.Framework.Kernel.Model;

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

            ITest assemblyTest = CreateAssemblyTest(Scope, Assembly);

            foreach (Fixture fixture in fixtureExplorer.FixtureGraph.Fixtures)
            {
                ITest fixtureTest = CreateFixtureTest(assemblyTest.Scope, fixture.Type, fixture.Name);

                foreach (RunPipeStarter starter in fixture.Starters)
                {
                    ITest test = CreateTest(fixtureTest.Scope, starter.Pipe.Name);
                }
            }

            // Use GetDependentAssemblies() to handle assembly dependencies
        }

        private void RunTests()
        {
            RunFixtureExplorerIfNeeded();

            IFixtureFilter fixtureFilter = new AnyFixtureFilter(); // fixme
            IRunPipeFilter runPipeFilter = new AnyRunPipeFilter(); // fixme
            ReportListener reportListener = new ReportListener();

            DependencyFixtureRunner fixtureRunner = new DependencyFixtureRunner();
            fixtureRunner.FixtureFilter = fixtureFilter;
            fixtureRunner.RunPipeFilter = runPipeFilter;

            // todo: hook event handlers

            fixtureRunner.Run(fixtureExplorer, reportListener);

            // todo: do something with the result
        }

        private void RunFixtureExplorerIfNeeded()
        {
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

        private ITest CreateAssemblyTest(TestScope parentScope, Assembly assembly)
        {
            BaseTest test = new BaseTest(assembly.FullName, CodeReference.CreateFromAssembly(assembly), parentScope);
            test.Kind = ComponentKind.Assembly;
            // TODO: Get metadata...

            parentScope.ContainingTest.AddChild(test);
            return test;
        }

        private ITest CreateFixtureTest(TestScope parentScope, Type fixtureType, string fixtureName)
        {
            BaseTest test = new BaseTest(fixtureName, CodeReference.CreateFromType(fixtureType), parentScope);
            test.Kind = ComponentKind.Fixture;
            // TODO: Get metadata...

            parentScope.ContainingTest.AddChild(test);
            return test;
        }

        private ITest CreateTest(TestScope parentScope, string testName)
        {
            BaseTest test = new BaseTest(testName, CodeReference.Unknown, parentScope);
            test.Kind = ComponentKind.Test;
            // TODO: Get metadata...

            parentScope.ContainingTest.AddChild(test);
            return test;
        }
    }
}
