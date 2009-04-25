using System;
using System.Collections.Generic;
using System.Text;
using Gallio.Runtime.Extensibility;

namespace Gallio.Runner
{
    /// <summary>
    /// Describes traits of an <see cref="ITestRunnerFactory"/> component.
    /// </summary>
    public class TestRunnerFactoryTraits : Traits
    {
        private readonly string name;
        private readonly string description;

        /// <summary>
        /// Creates test runner factory traits.
        /// </summary>
        /// <param name="name">The unique name of the kind of test runner created by the factory</param>
        /// <param name="description">The description of test runner created by the factory</param>
        public TestRunnerFactoryTraits(string name, string description)
        {
            if (name == null)
                throw new ArgumentNullException("name");
            if (description == null)
                throw new ArgumentNullException("description");

            this.name = name;
            this.description = description;
        }

        /// <summary>
        /// Gets the unique name of the kind of test runner created by the factory.
        /// </summary>
        public string Name
        {
            get { return name; }
        }

        /// <summary>
        /// Gets the description of test runner created by the factory.
        /// </summary>
        public string Description
        {
            get { return description; }
        }
    }
}
