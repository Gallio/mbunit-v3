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

using Gallio.Model.Tree;

namespace Gallio.Framework.Data
{
    /// <summary>
    /// Describes a data binding that binds to a test parameter.
    /// </summary>
    /// <remarks>
    /// <para>
    /// A data set that encomasses multiple tests may use the info in <see cref="TestParameter"/>
    /// to access parameter properties like type or containing fixture.
    /// </para>
    /// </remarks>
    public class TestParameterDataBinding : DataBinding
    {
        private readonly TestParameter testParameter;

        /// <summary>
        /// Creates a new data binding with an optional index, path and a reference to the corresponding <see cref="Gallio.Model.Tree.TestParameter"/>.
        /// </summary>
        /// <param name="index">The binding index or null if none. <seealso cref="DataBinding.Index"/>.</param>
        /// <param name="path">The binding path or null if none. <seealso cref="DataBinding.Path"/>.</param>
        /// <param name="testParameter"><see cref="Gallio.Model.Tree.TestParameter"/> that this binding corresponds to.</param>
        public TestParameterDataBinding(int? index, string path, TestParameter testParameter)
            : base(index, path)
        {
            this.testParameter = testParameter;
        }

        /// <summary>
        /// Gets the <see cref="Gallio.Model.Tree.TestParameter"/> that this binding corresponds to.
        /// </summary>
        public virtual TestParameter TestParameter
        {
            get { return testParameter; }
        }
    }
}
