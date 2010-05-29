using Gallio.UI.DataBinding;

namespace Gallio.Icarus.ProjectProperties
{
    public interface IModel
    {
        Observable<string> ApplicationBaseDirectory { get; }
        Observable<string> ReportDirectory { get; }
        Observable<string> ReportNameFormat { get; }
        Observable<bool> ShadowCopy { get; }
        ObservableList<string> TestRunnerExtensionSpecifications { get; }
        Observable<string> WorkingDirectory { get; }
        ObservableList<string> HintDirectories { get; }
    }
}