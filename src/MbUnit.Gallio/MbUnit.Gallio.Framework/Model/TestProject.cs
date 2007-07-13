using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace MbUnit.Framework.Model
{
    /// <summary>
    /// A test project provides parameters for test enumeration such as the list
    /// of test assemblies.
    /// </summary>
    public class TestProject
    {
        private List<Assembly> assemblies;

        /// <summary>
        /// Creates an empty test project.
        /// </summary>
        public TestProject()
        {
            assemblies = new List<Assembly>();
        }

        /// <summary>
        /// Gets the list of test assemblies.
        /// </summary>
        public IList<Assembly> Assemblies
        {
            get { return assemblies; }
        }

        // TODO: Filters, file-based test descriptions, framework-specific options etc...
    }
}
