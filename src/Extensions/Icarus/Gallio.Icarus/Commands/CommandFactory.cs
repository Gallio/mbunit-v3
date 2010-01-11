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

        public ICommand CreateShowReportCommand(string reportFormat)
        {
            var command = GetCommand<ShowReportCommand>();
            command.ReportFormat = reportFormat;
            return command;
        }

        public ICommand CreateRunTestsCommand(bool attachDebugger)
        {
            var command = GetCommand<RunTestsCommand>();
            command.AttachDebugger = attachDebugger;
            return command;
        }

        public ICommand CreateAddFilesCommand(string[] files)
        {
            var command = GetCommand<AddFilesCommand>();
            command.Files = files;
            return command;
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
