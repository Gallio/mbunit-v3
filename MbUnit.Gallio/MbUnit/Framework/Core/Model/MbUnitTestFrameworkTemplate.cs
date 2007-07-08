using System;
using System.Collections.Generic;
using System.Text;
using MbUnit.Core.Model;

namespace MbUnit.Framework.Core.Model
{
    /// <summary>
    /// The template that is the ancestor of all templates declared
    /// by the MbUnit test framework.
    /// </summary>
    public class MbUnitTestFrameworkTemplate : MbUnitTestTemplate
    {
        private Version version;

        /// <summary>
        /// Initializes the MbUnit framework template model object.
        /// </summary>
        /// <param name="version">The MbUnit framework version</param>
        public MbUnitTestFrameworkTemplate(Version version)
            : base("MbUnit Gallio v" + version, CodeReference.Unknown)
        {
            this.version = version;
        }

        /// <summary>
        /// Gets the MbUnit framework version.
        /// </summary>
        public Version Version
        {
            get { return version; }
        }

        /// <summary>
        /// Adds an assembly template to the framework.
        /// </summary>
        /// <param name="assemblyTemplate">The assembly template</param>
        public void AddAssemblyTemplate(MbUnitTestAssemblyTemplate assemblyTemplate)
        {
            TestTemplateTreeBuilder.LinkTemplate(this, assemblyTemplate);
        }
    }
}
