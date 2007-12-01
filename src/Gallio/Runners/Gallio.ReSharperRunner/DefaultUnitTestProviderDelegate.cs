using System.Collections.Generic;
using Gallio.Model;
using Gallio.Model.Reflection;
using Gallio.ReSharperRunner.Reflection;
using JetBrains.CommonControls;
using JetBrains.Metadata.Reader.API;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.ReSharper.TaskRunnerFramework;
using JetBrains.ReSharper.UnitTestExplorer;
using JetBrains.Shell;
using JetBrains.Shell.Progress;
using JetBrains.UI.TreeView;
using JetBrains.Util.DataStructures.TreeModel;

namespace Gallio.ReSharperRunner
{
    public class DefaultUnitTestProviderDelegate : IUnitTestProviderDelegate
    {
        private readonly GallioUnitTestPresenter presenter;

        private IUnitTestProvider provider;
        private ITestExplorer explorer;

        public DefaultUnitTestProviderDelegate()
        {
            presenter = new GallioUnitTestPresenter();
        }

        private ITestExplorer TestExplorer
        {
            get
            {
                if (explorer == null)
                    explorer = AggregateTestExplorer.CreateExplorerForAllTestFrameworks();
                return explorer;
            }
        }

        public void SetProvider(IUnitTestProvider provider)
        {
            this.provider = provider;
        }

        public void ExploreExternal(UnitTestElementConsumer consumer)
        {
        }

        public void ExploreSolution(ISolution solution, UnitTestElementConsumer consumer)
        {
        }

        public void ExploreAssembly(IMetadataAssembly assembly, IProject project, UnitTestElementConsumer consumer)
        {
            try
            {
                using (ReadLockCookie.Create())
                {
                    IAssemblyInfo assemblyInfo = PsiReflector.Wrap(project);
                    if (assemblyInfo != null)
                    {
                        foreach (ITest test in TestExplorer.ExploreAssembly(assemblyInfo))
                        {
                            consumer(CreateUnitTestElement(test, null));
                        }
                    }
                }
            }
            finally
            {
                TestExplorer.Reset();
            }
        }

        public void ExploreFile(IFile psiFile, UnitTestElementLocationConsumer consumer, CheckForInterrupt interrupted)
        {
            try
            {
                psiFile.ProcessDescendants(new OneActionProcessorWithoutVisit(delegate(IElement element)
                {
                    ITypeDeclaration declaration = element as ITypeDeclaration;
                    if (declaration != null)
                        ExploreTypeDeclaration(declaration, consumer, interrupted);
                }, delegate(IElement element)
                {
                    if (interrupted())
                        throw new ProcessCancelledException();

                    // Stop recursing at the first type declaration found.
                    return element is ITypeDeclaration;
                }));
            }
            finally
            {
                TestExplorer.Reset();
            }
        }

        private void ExploreTypeDeclaration(ITypeDeclaration declaration,
            UnitTestElementLocationConsumer consumer, CheckForInterrupt interrupted)
        {
            ITypeInfo typeInfo = PsiReflector.Wrap(declaration.DeclaredElement);

            if (typeInfo != null)
            {
                foreach (ITest test in TestExplorer.ExploreType(typeInfo))
                {
                    if (interrupted())
                        throw new ProcessCancelledException();

                    consumer(CreateUnitTestElement(test, null).GetDisposition());
                }
            }

            foreach (ITypeDeclaration nestedDeclaration in declaration.NestedTypeDeclarations)
            {
                if (interrupted())
                    throw new ProcessCancelledException();

                ExploreTypeDeclaration(nestedDeclaration, consumer, interrupted);
            }
        }

        public bool IsUnitTestElement(IDeclaredElement element)
        {
            try
            {
                ICodeElementInfo elementInfo = PsiReflector.Wrap(element, false);
                if (elementInfo == null)
                    return false;

                return TestExplorer.IsTest(elementInfo);
            }
            finally
            {
                TestExplorer.Reset();
            }
        }

        public void Present(UnitTestElement element, IPresentableItem item, TreeModelNode node, PresentationState state)
        {
            presenter.UpdateItem(element, node, item, state);
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
            GallioUnitTestElement xe = (GallioUnitTestElement)x;
            GallioUnitTestElement ye = (GallioUnitTestElement)y;

            return xe.CompareTo(ye);
        }

        private UnitTestElement CreateUnitTestElement(ITest test, UnitTestElement parent)
        {
            return new GallioUnitTestElement(test, provider, parent);
        }
    }
}