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

using System.Collections.Generic;
using Gallio.UI.ProgressMonitoring;

namespace Gallio.Icarus.Commands
{
    public interface ICommandFactory
    {
        ICommand CreateAddFilesCommand(IList<string> files);
        ICommand CreateConvertSavedReportCommand(string fileName, string format);
        ICommand CreateDeleteReportCommand(string fileName);
        ICommand CreateLoadPackageCommand();
        ICommand CreateOpenProjectCommand(string projectLocation);
        ICommand CreateRefreshTestTreeCommand();
        ICommand CreateReloadCommand();
        ICommand CreateRemoveAllFilesCommand();
        ICommand CreateRemoveFileCommand(string fileName);
        ICommand CreateResetTestsCommand();
        ICommand CreateRestoreFilterCommand();
        ICommand CreateRunTestsCommand(bool attachDebugger);
        ICommand CreateSaveFilterCommand(string filterName);
        ICommand CreateShowReportCommand(string reportFormat);
        ICommand CreateViewSourceCodeCommand(string testId);
    }
}
