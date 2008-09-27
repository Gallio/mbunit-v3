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
using System.Collections.Generic;
using System.Text;

namespace Gallio.Runner.Sessions
{
    /// <summary>
    /// Describes the status of a test run.
    /// </summary>
    public enum TestRunStatus
    {
        /// <summary>
        /// The test run has not yet been started.
        /// </summary>
        Pending = 0,

        /// <summary>
        /// The test run has been started and is running.
        /// </summary>
        Running,

        /// <summary>
        /// The test run has stopped because an error occurred.
        /// </summary>
        Error,

        /// <summary>
        /// The test run has stopped because it was canceled.
        /// </summary>
        Canceled,

        /// <summary>
        /// The test run has finished normally.
        /// </summary>
        Finished
    }
}
