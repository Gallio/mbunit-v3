using System;
using System.Collections.Generic;
using System.Text;
using Gallio.Framework.Data;
using Gallio.Model;

namespace Gallio.Framework.Pattern
{
    /// <summary>
    /// Default implementation of a test parameter builder.
    /// </summary>
    public class DefaultTestParameterBuilder : BaseTestComponentBuilder, ITestParameterBuilder
    {
        private readonly PatternTestParameter testParameter;

        /// <summary>
        /// Creates a test parameter builder.
        /// </summary>
        /// <param name="testModelBuilder">The test model builder</param>
        /// <param name="testParameter">The underlying test parameter</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="testModelBuilder"/>
        /// or <paramref name="testParameter"/> is null</exception>
        public DefaultTestParameterBuilder(ITestModelBuilder testModelBuilder, PatternTestParameter testParameter)
            : base(testModelBuilder)
        {
            if (testParameter == null)
                throw new ArgumentNullException("testParameter");

            this.testParameter = testParameter;
        }

        /// <inheritdoc />
        public PatternTestParameterActions TestParameterActions
        {
            get { return testParameter.TestParameterActions; }
        }

        /// <inheritdoc />
        public IDataBinder Binder
        {
            get { return testParameter.Binder; }
            set { testParameter.Binder = value; }
        }

        /// <inheritdoc />
        public PatternTestParameter ToTestParameter()
        {
            return testParameter;
        }

        /// <inheritdoc />
        protected sealed override IPatternTestComponent GetTestComponent()
        {
            return testParameter;
        }
    }
}
