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
using Gallio.Runtime.Logging;

namespace Gallio.Runtime.Debugging
{
    /// <summary>
    /// Default implementation of a debugger manager.
    /// </summary>
    public class DefaultDebuggerManager : IDebuggerManager
    {
        private const string VisualStudioDebuggerTypeName = "Gallio.VisualStudio.Interop.VisualStudioDebugger, Gallio.VisualStudio.Interop";

        /// <inheritdoc />
        public IDebugger GetDebugger(DebuggerSetup debuggerSetup, ILogger logger)
        {
            if (debuggerSetup == null)
                throw new ArgumentNullException("debuggerSetup");
            if (logger == null)
                throw new ArgumentNullException("logger");

            // FIXME: Should resolve service using IoC but we would have to start the runtime
            //        which is a lot of overhead for the host process.  -- Jeff.
            Type debuggerType = Type.GetType(VisualStudioDebuggerTypeName);
            if (debuggerType == null)
                return new NullDebugger();

            return (IDebugger)Activator.CreateInstance(debuggerType, debuggerSetup, logger, null);
        }
    }
}
