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

namespace Gallio.Model
{
    /// <summary>
    /// Wraps a list of <see cref="ITest" /> for reflection.
    /// </summary>
    public sealed class TestInfoList : BaseInfoList<ITest, TestInfo>
    {
        /// <summary>
        /// Creates a wrapper for the specified list of tests.
        /// </summary>
        /// <param name="inputList">The source test list.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="inputList"/> is null.</exception>
        public TestInfoList(IList<ITest> inputList)
            : base(inputList)
        {
        }

        /// <inheritdoc />
        protected override TestInfo Wrap(ITest inputItem)
        {
            return new TestInfo(inputItem);
        }
    }
}