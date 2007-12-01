using System;
using System.Collections.Generic;
using System.Text;
using Gallio.ReSharperRunner.Hosting;
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
    /// <summary>
    /// A unit test provider that does nothing.
    /// This is used when Gallio could not be successfully initialized.
    /// </summary>
    /// <remarks>
    /// The implementation of this class must not use Gallio types since
    /// it is possible that those types could not be loaded.  Service location
    /// should be performed via the <see cref="RuntimeProxy"/>.
    /// </remarks>
    public class NullUnitTestProviderDelegate : IUnitTestProviderDelegate
    {
        public void SetProvider(IUnitTestProvider provider)
        {
        }

        public void ExploreExternal(UnitTestElementConsumer consumer)
        {
        }

        public void ExploreSolution(ISolution solution, UnitTestElementConsumer consumer)
        {
        }

        public void ExploreAssembly(IMetadataAssembly assembly, IProject project, UnitTestElementConsumer consumer)
        {
        }

        public void ExploreFile(IFile psiFile, UnitTestElementLocationConsumer consumer, CheckForInterrupt interrupted)
        {
        }

        public bool IsUnitTestElement(IDeclaredElement element)
        {
            return false;
        }

        public void Present(UnitTestElement element, IPresentableItem item, TreeModelNode node, PresentationState state)
        {
        }

        public RemoteTaskRunnerInfo GetTaskRunnerInfo()
        {
            return new RemoteTaskRunnerInfo();
        }

        public IList<UnitTestTask> GetTaskSequence(UnitTestElement element, IList<UnitTestElement> explicitElements)
        {
            return new UnitTestTask[0];
        }

        public int CompareUnitTestElements(UnitTestElement x, UnitTestElement y)
        {
            return 0;
        }
    }
}
