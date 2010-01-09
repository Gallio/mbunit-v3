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
using Gallio.Common.Reflection;
using Gallio.Model.Tree;
using Gallio.Common.Messaging;
using Gallio.Runtime.ProgressMonitoring;

namespace Gallio.Model.Helpers
{
    /// <summary>
    /// A test explorer generates a <see cref="TestModel" /> from test code.
    /// </summary>
    /// <remarks>
    /// <para>
    /// A test explorer may be stateful since it is only used to build one <see cref="TestModel" />
    /// incrementally and then is disposed.
    /// </para>
    /// <para>
    /// Subclasses of this class provide the algorithm used to populate the test model.
    /// </para>
    /// </remarks>
    public abstract class TestExplorer : IDisposable
    {
        private TestModel testModel;

        /// <summary>
        /// Disposes the test explorer.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Disposes the test explorer.
        /// </summary>
        /// <param name="disposing">True if <see cref="Dispose()" /> was called directly.</param>
        protected virtual void Dispose(bool disposing)
        {
        }

        /// <summary>
        /// Gets the test model.
        /// </summary>
        public TestModel TestModel
        {
            get
            {
                if (testModel == null)
                    testModel = CreateTestModel();
                return testModel;
            }
        }

        /// <summary>
        /// Explores tests defined by the specified code element and populates the explorer's test model.
        /// </summary>
        /// <param name="reflectionPolicy">The reflection policy.</param>
        /// <param name="codeElement">The code element.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="reflectionPolicy"/>,
        /// <paramref name="codeElement"/> is null.</exception>
        public void Explore(IReflectionPolicy reflectionPolicy, ICodeElementInfo codeElement)
        {
            if (reflectionPolicy == null)
                throw new ArgumentNullException("reflectionPolicy");
            if (codeElement == null)
                throw new ArgumentNullException("codeElement");

            ExploreImpl(reflectionPolicy, codeElement);
        }

        /// <summary>
        /// Finishes populating the model.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The default implementation does nothing.  Subclasses should override this method
        /// to apply any deferred changes to the test model once exploration is complete.
        /// </para>
        /// </remarks>
        public virtual void Finish()
        {
        }

        /// <summary>
        /// Explores tests defined by the specified code element and populates the explorer's test model.
        /// </summary>
        /// <param name="reflectionPolicy">The reflection policy, not null.</param>
        /// <param name="codeElement">The code element, not null.</param>
        protected abstract void ExploreImpl(IReflectionPolicy reflectionPolicy, ICodeElementInfo codeElement);

        /// <summary>
        /// Creates the <see cref="TestModel" />.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The default implementation returns a new <see cref="TestModel"/>.
        /// </para>
        /// </remarks>
        /// <returns>The test model.</returns>
        protected virtual TestModel CreateTestModel()
        {
            return new TestModel();
        }
    }
}