using System;
using System.Reflection;
using MbUnit.Core.Model;

namespace MbUnit.Framework.Core.Model
{
    /// <summary>
    /// Represents a template derived from an MbUnit test assembly.
    /// </summary>
    public class MbUnitTestAssemblyTemplate : MbUnitTestTemplate
    {
        private MbUnitTestFrameworkTemplate frameworkTemplate;

        /// <summary>
        /// Initializes an MbUnit test assembly template model object.
        /// </summary>
        /// <param name="frameworkTemplate">The containing framework template</param>
        /// <param name="assembly">The test assembly</param>
        public MbUnitTestAssemblyTemplate(MbUnitTestFrameworkTemplate frameworkTemplate, Assembly assembly)
            : base(assembly.GetName().Name, CodeReference.CreateFromAssembly(assembly))
        {
            this.frameworkTemplate = frameworkTemplate;
        }

        /// <summary>
        /// Gets the containing framework template.
        /// </summary>
        public MbUnitTestFrameworkTemplate FrameworkTemplate
        {
            get { return frameworkTemplate; }
        }

        /// <summary>
        /// Gets the test assembly.
        /// </summary>
        public Assembly Assembly
        {
            get { return CodeReference.Assembly; }
        }

        /// <summary>
        /// Adds a test fixture template as a child of the assembly.
        /// </summary>
        /// <param name="fixtureTemplate">The test fixture template</param>
        public void AddFixtureTemplate(MbUnitTestFixtureTemplate fixtureTemplate)
        {
            TestTemplateTreeBuilder.LinkTemplate(this, fixtureTemplate);
        }

        /*
        public void AddAssemblySetUp()
        {
        }

        public void AddAssemblyTearDown()
        {
        }
         */
    }
}
