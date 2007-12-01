using System;
using System.Collections.Generic;
using System.Text;
using JetBrains.CommonControls;
using JetBrains.Metadata.Reader.API;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.ReSharper.TaskRunnerFramework;
using JetBrains.ReSharper.UnitTestExplorer;
using JetBrains.Shell;
using JetBrains.UI.TreeView;
using JetBrains.Util.DataStructures.TreeModel;

namespace Gallio.ReSharperRunner
{
    public interface IUnitTestProviderDelegate
    {
        void SetProvider(IUnitTestProvider provider);

        void ExploreExternal(UnitTestElementConsumer consumer);

        void ExploreSolution(ISolution solution, UnitTestElementConsumer consumer);

        void ExploreAssembly(IMetadataAssembly assembly, IProject project, UnitTestElementConsumer consumer);

        void ExploreFile(IFile psiFile, UnitTestElementLocationConsumer consumer, CheckForInterrupt interrupted);

        bool IsUnitTestElement(IDeclaredElement element);

        void Present(UnitTestElement element, IPresentableItem item, TreeModelNode node, PresentationState state);

        RemoteTaskRunnerInfo GetTaskRunnerInfo();

        IList<UnitTestTask> GetTaskSequence(UnitTestElement element, IList<UnitTestElement> explicitElements);

        int CompareUnitTestElements(UnitTestElement x, UnitTestElement y);
    }
}