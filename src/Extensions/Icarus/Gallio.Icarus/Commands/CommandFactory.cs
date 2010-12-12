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
using Gallio.Model.Filters;
using Gallio.Runner.Projects.Schema;
using Gallio.Runtime.Extensibility;
using Gallio.UI.ProgressMonitoring;

namespace Gallio.Icarus.Commands
{
    public class CommandFactory : ICommandFactory
    {
        private readonly IServiceLocator serviceLocator;

        public CommandFactory(IServiceLocator serviceLocator)
        {
            this.serviceLocator = serviceLocator;
        }

        public ICommand CreateReloadCommand()
        {
            return GetCommand<ReloadCommand>();
        }

        private T GetCommand<T>()
        {
            return (T)serviceLocator.ResolveByComponentId(typeof (T).FullName);
        }

        public ICommand CreateSaveProjectCommand(string fileName)
        {
            var command = GetCommand<SaveProjectCommand>();
            command.ProjectLocation = fileName;
            return command;
        }

        public ICommand CreateShowReportCommand(string reportFormat)
        {
            var command = GetCommand<ShowReportCommand>();
            command.ReportFormat = reportFormat;
            return command;
        }

        public ICommand CreateRestoreFilterCommand(string filterName)
        {
            var command = GetCommand<RestoreFilterCommand>();
            command.FilterName = filterName;
            return command;
        }

        public ICommand CreateRunTestsCommand(bool attachDebugger)
        {
            var command = GetCommand<RunTestsCommand>();
            command.AttachDebugger = attachDebugger;
            return command;
        }

        public ICommand CreateAddFilesCommand(IList<string> files)
        {
            var command = GetCommand<AddFilesCommand>();
            command.Files = files;
            return command;
        }

        public ICommand CreateApplyFilterCommand(FilterSet<ITestDescriptor> filterSet)
        {
            var command = GetCommand<ApplyFilterCommand>();
            command.FilterSet = filterSet;
            return command;
        }

        public ICommand CreateConvertSavedReportCommand(string fileName, string format)
        {
            var command = GetCommand<ConvertSavedReportCommand>();
            command.FileName = fileName;
            command.Format = format;
            return command;
        }

        public ICommand CreateDeleteFilterCommand(FilterInfo filterInfo)
        {
            var command = GetCommand<DeleteFilterCommand>();
            command.FilterInfo = filterInfo;
            return command;
        }

        public ICommand CreateDeleteReportCommand(string fileName)
        {
            var command = GetCommand<DeleteReportCommand>();
            command.FileName = fileName;
            return command;
        }

        public ICommand CreateLoadPackageCommand()
        {
            return GetCommand<LoadPackageCommand>();
        }

        public ICommand CreateNewProjectCommand()
        {
            return GetCommand<NewProjectCommand>();
        }

        public ICommand CreateRemoveAllFilesCommand()
        {
            return GetCommand<RemoveAllFilesCommand>();
        }

        public ICommand CreateResetTestsCommand()
        {
            return GetCommand<ResetTestsCommand>();
        }

        public ICommand CreateOpenProjectCommand(string projectLocation)
        {
            var command = GetCommand<OpenProjectCommand>();
            command.ProjectLocation = projectLocation;
            return command;
        }

        public ICommand CreateRemoveFileCommand(string fileName)
        {
            var command = GetCommand<RemoveFileCommand>();
            command.FileName = fileName;
            return command;
        }

        public ICommand CreateRefreshTestTreeCommand()
        {
            return GetCommand<RefreshTestTreeCommand>();
        }

        public ICommand CreateViewSourceCodeCommand(string testId)
        {
            var command = GetCommand<ViewSourceCodeCommand>();
            command.TestId = testId;
            return command;
        }

        public ICommand CreateSaveFilterCommand(string filterName)
        {
            var command = GetCommand<SaveFilterCommand>();
            command.FilterName = filterName;
            return command;
        }
    }
}
