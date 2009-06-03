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
using Gallio.Model;
using Gallio.Common.Reflection;

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
        /// <param name="testModelBuilder">The associated test model builder.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="testModelBuilder"/> is null.</exception>
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
        /// <returns>The test component.</returns>
        protected abstract IPatternTestComponent GetTestComponent();

        /// <inheritdoc />
        protected sealed override ITestModelBuilder GetTestModelBuilder()
        {
            return testModelBuilder;
        }
    }
}
