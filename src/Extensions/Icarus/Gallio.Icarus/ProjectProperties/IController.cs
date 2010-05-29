namespace Gallio.Icarus.ProjectProperties
{
    public interface IController
    {
        void AddHintDirectory(string hintDirectory);
        void AddTestRunnerExtensionSpecification(string testRunnerExtensionSpecification);
        void Load();
        void RemoveHintDirectory(string hintDirectory);
        void RemoveTestRunnerExtensionSpecification(string testRunnerExtension);
        void SetApplicationBaseDirectory(string applicationBaseDirectory);
        void SetReportNameFormat(string reportNameFormat);
        void SetShadowCopy(bool shadowCopy);
        void SetWorkingDirectory(string workingDirectory);
    }
}