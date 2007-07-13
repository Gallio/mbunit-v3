using System;
using System.Collections.Generic;
using System.Reflection;
using MbUnit.Framework.Collections;
using MbUnit.Framework.Model;
using MbUnit.Framework.Core.Attributes;
using MbUnit.Framework.Core.Model;
using MbUnit.Framework.Services.Runtime;
using MbUnit.Framework.Utilities;

namespace MbUnit.Framework.Core
{
    /// <summary>
    /// Builds a test object model based on reflection against MbUnit framework attributes.
    /// </summary>
    public class MbUnitTestFramework : ITestFramework
    {
        private IAssemblyResolverManager assemblyResolverManager;

        /// <summary>
        /// Initializes the test framework.
        /// </summary>
        /// <param name="assemblyResolverManager">The assembly resolver manager</param>
        public MbUnitTestFramework(IAssemblyResolverManager assemblyResolverManager)
        {
            this.assemblyResolverManager = assemblyResolverManager;
        }

        /// <inheritdoc />
        public string Name
        {
            get { return "MbUnit Gallio"; }
        }

        /// <inheritdoc />
        public void BuildTemplates(TestTemplateTreeBuilder builder, ITestTemplate parent)
        {
            MultiMap<AssemblyName, Assembly> map = ReflectionUtils.GetReverseAssemblyReferenceMap(builder.Project.Assemblies, "MbUnit.Gallio.Framework");
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

        /// <inheritdoc />
        public void InitializeTestAssembly(Assembly assembly)
        {
            foreach (AssemblyResolverAttribute resolverAttribute in
                assembly.GetCustomAttributes(typeof(AssemblyResolverAttribute), false))
            {
                Type type = resolverAttribute.AssemblyResolverType;
                try
                {
                    IAssemblyResolver resolver = (IAssemblyResolver) Activator.CreateInstance(type);
                    assemblyResolverManager.AssemblyResolve += delegate(object sender, ResolveEventArgs e)
                    {
                        return resolver.Resolve(e.Name);
                    };
                }
                catch (Exception ex)
                {
                    throw new ModelException(String.Format("Failed to create custom assembly resolver type '{0}'.", type), ex);
                }
            }
        }
    }
}
