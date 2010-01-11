using System.Collections.Generic;
using System.Drawing;
using Aga.Controls.Tree;
using Gallio.Icarus.Models;
using Gallio.UI.DataBinding;

namespace Gallio.Icarus.TestExplorer
{
    public class Model : IModel
    {
        public Observable<bool> FilterPassed { get; private set; }
        public Observable<bool> FilterFailed { get; private set; }
        public Observable<bool> FilterInconclusive { get; private set; }
        
        public ITreeModel TreeModel { get; private set; }

        public Observable<Color> PassedColor { get; private set; }
        public Observable<Color> FailedColor { get; private set; }
        public Observable<Color> InconclusiveColor { get; private set; }
        public Observable<Color> SkippedColor { get; private set; }

        public Observable<IList<string>> TreeViewCategories { get; set; }
        public Observable<string> CurrentTreeViewCategory { get; set; }
        public Observable<IList<string>> CollapsedNodes { get; set; }
        public Observable<bool> CanEditTree { get; private set; }

        public Model(ISortedTreeModel treeModel)
        {
            FilterPassed = new Observable<bool>();
            FilterFailed = new Observable<bool>();
            FilterInconclusive = new Observable<bool>();

            TreeModel = treeModel;

            PassedColor = new Observable<Color>();
            FailedColor = new Observable<Color>();
            InconclusiveColor = new Observable<Color>();
            SkippedColor = new Observable<Color>();

            TreeViewCategories = new Observable<IList<string>>(new List<string>());
            
            CurrentTreeViewCategory = new Observable<string>();

            CollapsedNodes = new Observable<IList<string>>(new List<string>());

            CanEditTree = new Observable<bool>();
        }
    }
}
