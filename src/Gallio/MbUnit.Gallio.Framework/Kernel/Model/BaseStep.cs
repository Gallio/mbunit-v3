using System;
using System.Collections.Generic;
using System.Text;

namespace MbUnit.Framework.Kernel.Model
{
    /// <summary>
    /// Base implementation of <see cref="IStep"/>.
    /// </summary>
    public class BaseStep : IStep
    {
        private string id;
        private string name;
        private IStep parent;
        private ITest test;

        /// <summary>
        /// Creates a step.
        /// </summary>
        /// <param name="name">The step name</param>
        /// <param name="test">The test to which the step belongs</param>
        /// <param name="parent">The parent step, or null if none</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="id"/>, <paramref name="name"/>,
        /// or <paramref name="test"/> is null</exception>
        public BaseStep(string name, ITest test, IStep parent)
        {
            if (name == null)
                throw new ArgumentNullException("name");
            if (test == null)
                throw new ArgumentNullException("test");

            this.name = name;
            this.test = test;
            this.parent = parent;

            id = Guid.NewGuid().ToString();
        }

        /// <inheritdoc />
        public string Id
        {
            get { return id; }
        }

        /// <inheritdoc />
        public string Name
        {
            get { return name; }
        }

        /// <inheritdoc />
        public IStep Parent
        {
            get { return parent; }
        }

        /// <inheritdoc />
        public ITest Test
        {
            get { return test; }
        }

        /// <summary>
        /// Creates a root step for a test.
        /// </summary>
        /// <param name="test">The test</param>
        /// <returns>The root step</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="test"/> is null</exception>
        public static BaseStep CreateRootStep(ITest test)
        {
            if (test == null)
                throw new ArgumentNullException("test");

            return new BaseStep("Root", test, null);
        }
    }
}
