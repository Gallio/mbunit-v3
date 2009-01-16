using System;
using System.Collections.Generic;
using System.Text;
using Gallio.Model;
using Gallio.Reflection;

namespace Gallio.Framework.Pattern
{
    /// <summary>
    /// Abstract base class for test component builders.
    /// </summary>
    public abstract class BaseTestComponentBuilder : BaseBuilder, ITestComponentBuilder
    {
        private readonly ITestModelBuilder testModelBuilder;

        /// <summary>
        /// Creates a test component builder.
        /// </summary>
        /// <param name="testModelBuilder">The associated test model builder</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="testModelBuilder"/> is null</exception>
        protected BaseTestComponentBuilder(ITestModelBuilder testModelBuilder)
        {
            if (testModelBuilder == null)
                throw new ArgumentNullException("testModelBuilder");

            this.testModelBuilder = testModelBuilder;
        }

        /// <inheritdoc />
        public string Id
        {
            get { return GetTestComponent().Id; }
        }

        /// <inheritdoc />
        public string Name
        {
            get { return GetTestComponent().Name; }
            set { GetTestComponent().SetName(value); }
        }

        /// <inheritdoc />
        public ICodeElementInfo CodeElement
        {
            get { return GetTestComponent().CodeElement; }
        }

        /// <inheritdoc />
        public void AddMetadata(string key, string value)
        {
            if (key == null)
                throw new ArgumentNullException("key");
            if (value == null)
                throw new ArgumentNullException("value");

            GetTestComponent().Metadata.Add(key, value);
        }

        /// <inheritdoc />
        public IPatternTestComponent ToTestComponent()
        {
            return GetTestComponent();
        }

        /// <summary>
        /// Gets the underlying test component.
        /// </summary>
        /// <returns>The test component</returns>
        protected abstract IPatternTestComponent GetTestComponent();

        /// <inheritdoc />
        protected sealed override ITestModelBuilder GetTestModelBuilder()
        {
            return testModelBuilder;
        }
    }
}
