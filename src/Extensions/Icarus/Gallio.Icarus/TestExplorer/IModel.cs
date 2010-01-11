using System.Collections.Generic;
using System.Drawing;
using Aga.Controls.Tree;
using Gallio.UI.DataBinding;

namespace Gallio.Icarus.TestExplorer
{
    public interface IModel
    {
        Observable<bool> FilterPassed { get; }
        Observable<bool> FilterFailed { get; }
        Observable<bool> FilterInconclusive { get; }
        ITreeModel TreeModel { get; }
        Observable<Color> PassedColor { get; }
        Observable<Color> FailedColor { get; }
        Observable<Color> InconclusiveColor { get; }
        Observable<Color> SkippedColor { get; }
        Observable<IList<string>> TreeViewCategories { get; set; }
        Observable<string> CurrentTreeViewCategory { get; set; }
        Observable<IList<string>> CollapsedNodes { get; set; }
        Observable<bool> CanEditTree { get; }
    }
}