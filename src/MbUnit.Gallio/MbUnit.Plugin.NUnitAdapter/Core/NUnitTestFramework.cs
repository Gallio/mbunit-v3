using System;
using System.Collections.Generic;
using System.Reflection;
using MbUnit.Framework.Collections;
using MbUnit.Framework.Model.Metadata;
using MbUnit.Framework.Model;
using MbUnit.Core.Utilities;
using MbUnit.Framework.Utilities;

namespace MbUnit.Plugin.NUnitAdapter.Core
{
    /// <summary>
    /// Builds a test object model based on reflection against NUnit framework attributes.
    /// </summary>
    public class NUnitTestFramework : ITestFramework
    {
        /// <inheritdoc />
        public string Name
        {
            get { return "NUnit"; }
        }

        /// <inheritdoc />
        public void BuildTemplates(TestTemplateTreeBuilder builder, ITestTemplate parent)
        {
            MultiMap<AssemblyName, Assembly> map = ReflectionUtils.GetReverseAssemblyReferenceMap(builder.Project.Assemblies, "nunit.framework");
            foreach (KeyValuePair<AssemblyName, IList<Assembly>> entry in map)
            {
                // Add a framework template with suitable rules to populate tests using the
                // NUnit test enumerator.  We don't actually represent each test as a
                // template because we can't perform any interesting meta-operations
                // on them like binding test parameters or composing tests.
                Version frameworkVersion = entry.Key.Version;
                TestTemplateGroup frameworkTemplate = new TestTemplateGroup("NUnit v" + frameworkVersion, CodeReference.Unknown);
                frameworkTemplate.Kind = TemplateKind.Framework;
                parent.AddChild(frameworkTemplate);

                foreach (Assembly assembly in entry.Value)
                {
                    TestTemplateGroup assemblyTemplate = new TestTemplateGroup(assembly.FullName, CodeReference.CreateFromAssembly(assembly));
                    assemblyTemplate.Kind = TemplateKind.Assembly;
                    frameworkTemplate.AddChild(assemblyTemplate);

                    // TODO: Add rules to populate the test graph using NUnit.
                }
            }
        }

        /// <inheritdoc />
        public void InitializeTestAssembly(Assembly assembly)
        {
        }
    }
}
