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

namespace Gallio.Framework.Pattern
{
    /// <summary>
    /// Default implementation of a test data context builder.
    /// </summary>
    public class DefaultTestDataContextBuilder : BaseBuilder, ITestDataContextBuilder
    {
        private readonly ITestModelBuilder testModelBuilder;
        private readonly PatternTestDataContext testDataContext;

        /// <summary>
        /// Creates a test data context builder.
        /// </summary>
        /// <param name="testModelBuilder">The associated test model builder.</param>
        /// <param name="testDataContext">The underlying test data context.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="testModelBuilder"/>
        /// or <paramref name="testDataContext"/> is null.</exception>
        public DefaultTestDataContextBuilder(ITestModelBuilder testModelBuilder, PatternTestDataContext testDataContext)
        {
            if (testModelBuilder == null)
                throw new ArgumentNullException("testModelBuilder");
            if (testDataContext == null)
                throw new ArgumentNullException("testDataContext");

            this.testModelBuilder = testModelBuilder;
            this.testDataContext = testDataContext;
        }

        /// <inheritdoc />
        public DataSource DefineDataSource(string name)
        {
            return testDataContext.DefineDataSource(name);
        }

        /// <inheritdoc />
        public ITestDataContextBuilder CreateChild()
        {
            return new DefaultTestDataContextBuilder(testModelBuilder, testDataContext.CreateChild());
        }

        /// <inheritdoc />
        public int ImplicitDataBindingIndexOffset
        {
            get { return testDataContext.ImplicitDataBindingIndexOffset; }
            set { testDataContext.ImplicitDataBindingIndexOffset = value; }
        }

        /// <inheritdoc />
        public PatternTestDataContext ToPatternTestDataContext()
        {
            return testDataContext;
        }

        /// <inheritdoc />
        protected sealed override ITestModelBuilder GetTestModelBuilder()
        {
            return testModelBuilder;
        }
    }
}
