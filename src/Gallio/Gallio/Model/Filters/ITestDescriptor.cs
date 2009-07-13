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
using Gallio.Common.Collections;
using Gallio.Common.Reflection;

namespace Gallio.Model.Filters
{
    /// <summary>
    /// Describes characteristics of a test.
    /// </summary>
    public interface ITestDescriptor
    {
        /// <summary>
        /// Gets the test id.
        /// </summary>
        string Id { get; }

        /// <summary>
        /// Gets the test name.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets the code element associated with the test, or null if none.
        /// </summary>
        ICodeElementInfo CodeElement { get; }

        /// <summary>
        /// Gets the test metadata.
        /// </summary>
        PropertyBag Metadata { get; }

    }
}
