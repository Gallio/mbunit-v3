using Aga.Controls.Tree;
using Gallio.UI.DataBinding;

namespace Gallio.Icarus.TestExplorer
{
    public class Model
    {
        public Observable<bool> FilterPassed { get; private set; }
        public Observable<bool> FilterFailed { get; private set; }
        public Observable<bool> FilterInconclusive { get; private set; }
        
        public ITreeModel TreeModel { get; private set; }

        public Model(ITreeModel treeModel)
        {
            FilterPassed = new Observable<bool>();
            FilterFailed = new Observable<bool>();
            FilterInconclusive = new Observable<bool>();

            TreeModel = treeModel;
        }
    }
}
