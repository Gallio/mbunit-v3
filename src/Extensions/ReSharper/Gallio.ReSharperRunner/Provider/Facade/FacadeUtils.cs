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
using JetBrains.ReSharper.TaskRunnerFramework;

namespace Gallio.ReSharperRunner.Provider.Facade
{
    /// <summary>
    /// Utilities for translating facade types.
    /// </summary>
    /// <remarks>
    /// This type is part of a facade that decouples the Gallio test runner from the ReSharper interfaces.
    /// </remarks>
    public class FacadeUtils
    {
        public static TaskException ToTaskException(FacadeTaskException exception)
        {
            return TaskExceptionFactory.CreateTaskException(exception.Type, exception.Message, exception.StackTrace);
        }

        public static TaskResult ToTaskResult(FacadeTaskResult result)
        {
            switch (result)
            {
                case FacadeTaskResult.Success:
                    return TaskResult.Success;

                case FacadeTaskResult.Skipped:
                    return TaskResult.Skipped;

                case FacadeTaskResult.Error:
                    return TaskResult.Error;

                case FacadeTaskResult.Exception:
                    return TaskResult.Exception;

                default:
                    throw new ArgumentOutOfRangeException("result");
            }
        }

        public static TaskOutputType ToTaskOutputType(FacadeTaskOutputType outputType)
        {
            switch (outputType)
            {
                case FacadeTaskOutputType.StandardOutput:
                    return TaskOutputType.STDOUT;

                case FacadeTaskOutputType.StandardError:
                    return TaskOutputType.STDERR;

                case FacadeTaskOutputType.DebugTrace:
                    return TaskOutputType.DEBUGTRACE;

                default:
                    throw new ArgumentOutOfRangeException("outputType");
            }
        }

        public static RemoteTask ToRemoteTask(FacadeTask facadeTask)
        {
            return new FacadeTaskWrapper(facadeTask);
        }
    }
}
