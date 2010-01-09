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

namespace Gallio.Model
{
    /// <summary>
    /// Specifies how Gallio responds to test files that do not match any frameworks.
    /// </summary>
    /// <seealso cref="TestFrameworkSelector"/>
    [Serializable]
    public enum TestFrameworkFallbackMode
    {
        /// <summary>
        /// Default match: always fallback to the fallback test framework when 
        /// a file does not satisfy the constraits of any other
        /// test framework exactly.
        /// </summary>
        Default = 0,

        /// <summary>
        /// Approximate match: fallback to the fallback test framework when
        /// a file does not satisfy the constraints of any other
        /// test framework exactly but appears to be an approximate match
        /// for one of them (perhaps differing in version only).  Otherwise
        /// the file is not considered a test file.
        /// </summary>
        Approximate = 1,

        /// <summary>
        /// Strict match: no fallback.  If a file does not satisfy the
        /// constraints of any framework exactly then it is not considered a test file.
        /// </summary>
        Strict = 2,
    }
}
