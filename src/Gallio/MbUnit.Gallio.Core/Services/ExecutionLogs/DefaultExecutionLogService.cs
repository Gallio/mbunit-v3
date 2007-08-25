// Copyright 2007 MbUnit Project - http://www.mbunit.com/
// Portions Copyright 2000-2004 Jonathan De Halleux, Jamie Cansdale
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
using MbUnit.Framework.Kernel.ExecutionLogs;
using MbUnit.Framework.Kernel.Contexts;

namespace MbUnit.Core.Services.ExecutionLogs
{
    /// <summary>
    /// The default implementation of the execution log service.
    /// </summary>
    public sealed class DefaultExecutionLogService : IExecutionLogService
    {
        private const string ContextDataKey = "$$DefaultExecutionLogService.ContextData$$";

        /// <inheritdoc />
        public IExecutionLog GetExecutionLog(IContext context)
        {
            ContextData state = GetInitializedContextData(context);
            return state.ExecutionLog;
        }

        /// <summary>
        /// Gets the service data for the specified context.
        /// Initializes it if absent.
        /// </summary>
        /// <param name="context">The context</param>
        /// <returns>The assertion state</returns>
        private ContextData GetInitializedContextData(IContext context)
        {
            lock (context.SyncRoot)
            {
                ContextData contextData;
                if (context.TryGetData(ContextDataKey, out contextData))
                {
                    contextData = new ContextData(this);
                    context.SetData(ContextDataKey, contextData);
                }

                return contextData;
            }
        }

        /// <summary>
        /// Maintains service state information associated with the context.
        /// </summary>
        private class ContextData
        {
            private IExecutionLog executionLog;

            public ContextData(DefaultExecutionLogService service)
            {
            }

            public IExecutionLog ExecutionLog
            {
                get { return executionLog; }
            }
        }
    }
}
