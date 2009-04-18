using System.Drawing;
using System.Windows.Forms;

namespace Gallio.Icarus.Controllers.Interfaces
{
    public interface ITestResultsController : INotifyController
    {
        int ResultsCount { get; }
        string TestStatusBarStyle { get; }
        Color PassedColor { get; }
        Color FailedColor { get; }
        Color InconclusiveColor { get; }
        Color SkippedColor { get; }
        int Passed { get; }
        int Failed { get; }
        int Skipped { get; }
        int Inconclusive { get; }

        void CacheVirtualItems(int startIndex, int endIndex);
        ListViewItem RetrieveVirtualItem(int itemIndex);
    }
}
