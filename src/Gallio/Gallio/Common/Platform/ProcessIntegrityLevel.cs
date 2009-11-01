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

namespace Gallio.Common.Platform
{
    /// <summary>
    /// Specifies the integrity level of a Windows process.
    /// </summary>
    public enum ProcessIntegrityLevel
    {
        /// <summary>
        /// Unknown integrity level.
        /// </summary>
        Unknown = 0,

        /// <summary>
        /// Process is running with Untrusted integrity.
        /// </summary>
        Untrusted,

        /// <summary>
        /// Process is running with Low integrity.
        /// </summary>
        Low,

        /// <summary>
        /// Process is running with Medium integrity.
        /// </summary>
        Medium,

        /// <summary>
        /// Process is running with High integrity.
        /// </summary>
        High,

        /// <summary>
        /// Process is running with System integrity.
        /// </summary>
        System,

        /// <summary>
        /// Process is running with Protected Process integrity.
        /// </summary>
        ProtectedProcess
    }
}
