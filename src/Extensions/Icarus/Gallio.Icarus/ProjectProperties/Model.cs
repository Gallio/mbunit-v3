using Gallio.UI.DataBinding;

namespace Gallio.Icarus.ProjectProperties
{
    /// <summary>
    /// View model for Project Properties tab.
    /// </summary>
    public class Model : IModel
    {
        public Observable<string> ApplicationBaseDirectory { get; private set; }
        public ObservableList<string> HintDirectories { get; private set; }
        public Observable<string> ReportDirectory { get; private set; }
        public Observable<string> ReportNameFormat { get; private set; }
        public Observable<bool> ShadowCopy { get; private set; }
        public ObservableList<string> TestRunnerExtensionSpecifications { get; private set; }
        public Observable<string> WorkingDirectory { get; private set; }

        public Model()
        {
            ApplicationBaseDirectory = new Observable<string>();
            HintDirectories = new ObservableList<string>();
            ReportDirectory = new Observable<string>();
            ReportNameFormat = new Observable<string>();
            ShadowCopy = new Observable<bool>();
            TestRunnerExtensionSpecifications = new ObservableList<string>();
            WorkingDirectory = new Observable<string>();
        }
    }
}
