using System;
using System.Collections.Generic;
using System.Reflection;
using MbUnit.Core.Model;
using MbUnit.Core.Utilities;
using System.Collections;
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
            List<Assembly> testAssemblies = new List<Assembly>();
            Version version = null;

            foreach (Assembly assembly in project.Assemblies)
            {
                AssemblyName reference = ReflectionUtils.FindReferencedAssembly(assembly.GetName(), "MbUnit.Gallio");
                if (reference != null)
                {
                    if (reference.Version.Major == 3)
                    {
                        version = reference.Version;
                        testAssemblies.Add(assembly);
                    }
                }
            }

            if (version == null)
                return;

            MbUnitTestFrameworkTemplate frameworkTemplate = new MbUnitTestFrameworkTemplate(version);
            TestTemplateTreeBuilder.LinkTemplate(parent, frameworkTemplate);

            foreach (Assembly assembly in testAssemblies)
            {
                AssemblyPatternAttribute.ProcessAssembly(builder, frameworkTemplate, assembly);
            }
        }
    }
}
