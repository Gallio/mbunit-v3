// Copyright 2005-2008 Gallio Project - http://www.gallio.org/
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
using Gallio.Reflection;

namespace Gallio.Model
{
    /// <summary>
    /// A base test explorer that does nothing.
    /// </summary>
    public abstract class BaseTestExplorer : ITestExplorer
    {
        private readonly TestModel testModel;

        /// <summary>
        /// Creates a test explorer.
        /// </summary>
        /// <param name="testModel">The test model that is incrementally populated by the test
        /// explorer as it explores tests.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="testModel"/> is null</exception>
        public BaseTestExplorer(TestModel testModel)
        {
            if (testModel == null)
                throw new ArgumentNullException("testModel");

            this.testModel = testModel;
        }

        /// <inheritdoc />
        public TestModel TestModel
        {
            get { return testModel; }
        }

        /// <inheritdoc />
        public virtual bool IsTest(ICodeElementInfo element)
        {
            return false;
        }

        /// <inheritdoc />
        public virtual void ExploreAssembly(IAssemblyInfo assembly, Action<ITest> consumer)
        {
        }

        /// <inheritdoc />
        public virtual void ExploreType(ITypeInfo type, Action<ITest> consumer)
        {
        }

        /// <inheritdoc />
        public virtual void FinishModel()
        {
        }
    }
}
