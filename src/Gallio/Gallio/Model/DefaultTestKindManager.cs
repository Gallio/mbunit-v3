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
using System.Collections.ObjectModel;
using Gallio.Runtime.Extensibility;

namespace Gallio.Model
{
    /// <summary>
    /// Default implementation of a <see cref="ITestKindManager" />.
    /// </summary>
    public class DefaultTestKindManager : ITestKindManager
    {
        private readonly ComponentHandle<ITestKind, TestKindTraits>[] testKindHandles;

        /// <summary>
        /// Creates a test kind manager.
        /// </summary>
        /// <param name="testKindHandles">The test kind handles</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="testKindHandles"/> is null</exception>
        public DefaultTestKindManager(ComponentHandle<ITestKind, TestKindTraits>[] testKindHandles)
        {
            if (testKindHandles == null || Array.IndexOf(testKindHandles, null) >= 0)
                throw new ArgumentNullException("testKindHandles");

            this.testKindHandles = testKindHandles;
        }

        /// <inheritdoc />
        public IList<ComponentHandle<ITestKind, TestKindTraits>> TestKindHandles
        {
            get { return new ReadOnlyCollection<ComponentHandle<ITestKind, TestKindTraits>>(testKindHandles); }
        }
    }
}
