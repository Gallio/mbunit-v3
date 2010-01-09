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

namespace Gallio.TDNetRunner.Core
{
    /// <summary>
    /// Specifies the installation mode for the TDNet runner with respect to a given framework.
    /// </summary>
    public enum TDNetRunnerInstallationMode
    {
        /// <summary>
        /// The runner has been disabled for a particular framework.
        /// </summary>
        Disabled,

        /// <summary>
        /// The runner should be used in preference to any other runner for the framework is installed on the machine.
        /// </summary>
        Preferred,

        /// <summary>
        /// The runner should be used if no other runner for the framework is installed on the machine.
        /// </summary>
        Default
    }
}
