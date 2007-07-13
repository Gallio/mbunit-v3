using System;
using System.Collections.Generic;
using System.Text;

namespace MbUnit.Framework.Model
{
    /// <summary>
    /// Builds a test tree for a given test project.  The builder retains
    /// context information used during the construction of a test tree.
    /// </summary>
    public class TestTreeBuilder
    {
        private ITest root;
        private TestProject project;

        /// <summary>
        /// Creates a test tree builder for the specified project.
        /// </summary>
        /// <param name="project">The test project</param>
        public TestTreeBuilder(TestProject project)
        {
            this.project = project;
            root = CreateRoot();
        }

        /// <summary>
        /// Gets the test project.
        /// </summary>
        public TestProject Project
        {
            get { return project; }
        }

        /// <summary>
        /// Gets the root of the test tree.
        /// </summary>
        public ITest Root
        {
            get { return root; }
        }

        private static ITest CreateRoot()
        {
            return null;
        }
    }
}
