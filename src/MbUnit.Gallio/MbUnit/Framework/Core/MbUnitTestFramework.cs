using System;
using System.Collections.Generic;
using System.Reflection;
using MbUnit.Core.Collections;
using MbUnit.Core.Model;
using MbUnit.Core.Utilities;
using MbUnit.Framework.Core.Attributes;
using MbUnit.Framework.Core.Model;

namespace MbUnit.Framework.Core
{
    /// <summary>
    /// Builds a test object model based on reflection against MbUnit framework attributes.
    /// </summary>
    public class MbUnitTestFramework : ITestFramework
    {
        /// <inheritdoc />
        public string Name
        {
            get { return "MbUnit Gallio"; }
        }

        /// <inheritdoc />
        public void PopulateTestTemplateTree(TestTemplateTreeBuilder builder, ITestTemplate parent, TestProject project)
        {
            MultiMap<AssemblyName, Assembly> map = ReflectionUtils.GetReverseAssemblyReferenceMap(project.Assemblies, "MbUnit.Gallio");
            foreach (KeyValuePair<AssemblyName, IList<Assembly>> entry in map)
            {
                // Build templates for the contents of the assemblies that reference MbUnit Gallio
                // via reflection.  The attributes exercise a great deal of control over this
                // process so that it can be easily extended by users.
                Version frameworkVersion = entry.Key.Version;
                MbUnitTestFrameworkTemplate frameworkTemplate = new MbUnitTestFrameworkTemplate(frameworkVersion);
                parent.AddChild(frameworkTemplate);

                foreach (Assembly assembly in entry.Value)
                {
                    AssemblyPatternAttribute.ProcessAssembly(builder, frameworkTemplate, assembly);
                }
            }
        }
    }
}
