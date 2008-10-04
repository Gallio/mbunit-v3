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

namespace Gallio.ReSharperRunner.Provider.Facade
{
    /// <summary>
    /// A facade of the ReSharper task server interface.
    /// </summary>
    /// <remarks>
    /// This type is part of a facade that decouples the Gallio test runner from the ReSharper interfaces.
    /// </remarks>
    public interface IFacadeTaskServer
    {
        string SessionId { get; }

        void TaskError(FacadeTask task, string message);
        void TaskException(FacadeTask task, FacadeTaskException[] exceptions);
        void TaskExplain(FacadeTask task, string explanation);
        void TaskFinished(FacadeTask task, string message, FacadeTaskResult result);
        void TaskOutput(FacadeTask task, string text, FacadeTaskOutputType outputType);
        void TaskProgress(FacadeTask task, string message);
        void TaskStarting(FacadeTask task);
    }
}