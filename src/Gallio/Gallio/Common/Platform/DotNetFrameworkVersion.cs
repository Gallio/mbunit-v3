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

namespace Gallio.Common.Platform
{
    /// <summary>
    /// Specifies the version of the .Net framework.
    /// </summary>
    /// <seealso cref="DotNetFrameworkSupport"/>
    public enum DotNetFrameworkVersion
    {
        /// <summary>
        /// .Net Framework 2.0.
        /// </summary>
        DotNet20 = 0,

        /// <summary>
        /// .Net Framework 3.0.
        /// </summary>
        DotNet30 = 1,

        /// <summary>
        /// .Net Framework 3.5.
        /// </summary>
        DotNet35 = 2,

        /// <summary>
        /// .Net Framework 4.0.
        /// </summary>
        DotNet40 = 3
    }
}