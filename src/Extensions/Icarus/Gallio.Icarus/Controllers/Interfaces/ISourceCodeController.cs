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
using Gallio.Common.Reflection;
using Gallio.Icarus.Controllers.EventArgs;
using Gallio.Runtime.ProgressMonitoring;

namespace Gallio.Icarus.Controllers.Interfaces
{
    public interface ISourceCodeController
    {
        /// <summary>
        /// Event raised when showing source code.
        /// </summary>
        event EventHandler<ShowSourceCodeEventArgs> ShowSourceCode;

        /// <summary>
        /// Views the source code associated with a particular test.
        /// </summary>
        /// <param name="testId">The test id.</param>
        /// <param name="progressMonitor">The progress monitor.</param>
        void ViewSourceCode(string testId, IProgressMonitor progressMonitor);

        /// <summary>
        /// Views the source code associated with a particular test.
        /// </summary>
        /// <param name="codeLocation">The location of the code.</param>
        void ViewSourceCode(CodeLocation codeLocation);
    }
}
