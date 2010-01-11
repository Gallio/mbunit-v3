using Gallio.UI.ProgressMonitoring;

namespace Gallio.Icarus.Commands
{
    public interface ICommandFactory
    {
        ICommand CreateReloadCommand();
        ICommand CreateShowReportCommand(string reportFormat);
        ICommand CreateRunTestsCommand(bool attachDebugger);
        ICommand CreateAddFilesCommand(string[] files);
        ICommand CreateRemoveAllFilesCommand();
        ICommand CreateResetTestsCommand();
        ICommand CreateOpenProjectCommand(string projectLocation);
        ICommand CreateRemoveFileCommand(string fileName);
        ICommand CreateRefreshTestTreeCommand();
        ICommand CreateViewSourceCodeCommand(string testId);
        ICommand CreateSaveFilterCommand(string filterName);
    }
}