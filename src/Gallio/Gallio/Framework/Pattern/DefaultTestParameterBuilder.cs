// Copyright 2005-2009 Gallio Project - http://www.gallio.org/
// Portions Copyright 2000-2004 Jonathan de Halleux
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

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
        /// <param name="testModelBuilder">The test model builder.</param>
        /// <param name="testParameter">The underlying test parameter.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="testModelBuilder"/>
        /// or <paramref name="testParameter"/> is null.</exception>
        public DefaultTestParameterBuilder(ITestModelBuilder testModelBuilder, PatternTestParameter testParameter)
            : base(testModelBuilder)
        {
            if (testParameter == null)
                throw new ArgumentNullException("testParameter");

            this.testParameter = testParameter;
        }

        /// <inheritdoc />
        public ITestDataContextBuilder TestDataContextBuilder
        {
            get { return new DefaultTestDataContextBuilder(GetTestModelBuilder(), testParameter.DataContext); }
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
