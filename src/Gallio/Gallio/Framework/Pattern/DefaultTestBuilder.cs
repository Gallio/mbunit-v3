// Copyright 2005-2010 Gallio Project - http://www.gallio.org/
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
using System.Threading;
using Gallio.Common;
using Gallio.Common.Collections;
using Gallio.Model;
using Gallio.Common.Reflection;
using Gallio.Model.Tree;

namespace Gallio.Framework.Pattern
{
    /// <summary>
    /// Default implementation of a test model builder.
    /// </summary>
    public class DefaultTestBuilder : BaseTestComponentBuilder, ITestBuilder
    {
        private readonly PatternTest test;

        /// <summary>
        /// Creates a test builder.
        /// </summary>
        /// <param name="testModelBuilder">The test model builder.</param>
        /// <param name="test">The underlying test.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="testModelBuilder"/>
        /// or <paramref name="test"/> is null.</exception>
        public DefaultTestBuilder(ITestModelBuilder testModelBuilder, PatternTest test)
            : base(testModelBuilder)
        {
            if (test == null)
                throw new ArgumentNullException("test");

            this.test = test;
        }

        /// <inheritdoc />
        public string Kind
        {
            get { return test.Kind; }
            set { test.Kind = value; }
        }

        /// <inheritdoc />
        public ApartmentState ApartmentState
        {
            get { return test.ApartmentState; }
            set { test.ApartmentState = value; }
        }

        /// <inheritdoc />
        public Func<TimeSpan?> TimeoutFunc
        {
            get { return test.TimeoutFunc; }
            set { test.TimeoutFunc = value; }
        }

        /// <inheritdoc />
        public bool IsTestCase
        {
            get { return test.IsTestCase; }
            set { test.IsTestCase = value; }
        }

        /// <inheritdoc />
        public bool IsParallelizable
        {
            get { return test.IsParallelizable; }
            set { test.IsParallelizable = value; }
        }

        /// <inheritdoc />
        public int Order
        {
            get { return test.Order; }
            set { test.Order = value; }
        }

        /// <inheritdoc />
        public string LocalId
        {
            get { return test.LocalId; }
        }

        /// <inheritdoc />
        public string LocalIdHint
        {
            get { return test.LocalIdHint; }
            set { test.LocalIdHint = value; }
        }

        /// <inheritdoc />
        public PatternTestActions TestActions
        {
            get { return test.TestActions; }
        }

        /// <inheritdoc />
        public PatternTestInstanceActions TestInstanceActions
        {
            get { return test.TestInstanceActions; }
        }

        /// <inheritdoc />
        public ITestBuilder CreateChild(string name, ICodeElementInfo codeElement, ITestDataContextBuilder dataContextBuilder)
        {
            if (name == null)
                throw new ArgumentNullException("name");
            if (dataContextBuilder == null)
                throw new ArgumentNullException("dataContextBuilder");

            PatternTest childTest = new PatternTest(name, codeElement, dataContextBuilder.ToPatternTestDataContext());
            test.AddChild(childTest);
            return new DefaultTestBuilder(GetTestModelBuilder(), childTest);
        }

        /// <inheritdoc />
        public ITestParameterBuilder CreateParameter(string name, ICodeElementInfo codeElement, ITestDataContextBuilder dataContextBuilder)
        {
            if (name == null)
                throw new ArgumentNullException("name");
            if (dataContextBuilder == null)
                throw new ArgumentNullException("dataContextBuilder");

            PatternTestParameter testParameter = new PatternTestParameter(name, codeElement, dataContextBuilder.ToPatternTestDataContext());
            test.AddParameter(testParameter);
            return new DefaultTestParameterBuilder(GetTestModelBuilder(), testParameter);
        }

        /// <inheritdoc />
        public ITestParameterBuilder GetParameter(string name)
        {
            if (name == null)
                throw new ArgumentNullException("name");

            var testParameter = (PatternTestParameter) GenericCollectionUtils.Find(test.Parameters, x => x.Name == name);
            if (testParameter == null)
                return null;
            return new DefaultTestParameterBuilder(GetTestModelBuilder(), testParameter);
        }

        /// <inheritdoc />
        public void AddDependency(Test testDependency)
        {
            if (testDependency == null)
                throw new ArgumentNullException("testDependency");

            test.AddDependency(testDependency);
        }

        /// <inheritdoc />
        public PatternTest ToTest()
        {
            return test;
        }

        /// <inheritdoc />
        protected sealed override IPatternTestComponent GetTestComponent()
        {
            return test;
        }
    }
}
