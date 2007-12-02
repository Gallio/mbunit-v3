// Copyright 2007 MbUnit Project - http://www.mbunit.com/
// Portions Copyright 2000-2004 Jonathan De Halleux, Jamie Cansdale
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System.Collections.Generic;
using Gallio.Model;
using Gallio.Model.Reflection;
using Gallio.ReSharperRunner.Reflection;
using Gallio.ReSharperRunner.Tasks;
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
    public class GallioTestProviderDelegate : IUnitTestProviderDelegate
    {
        private readonly GallioTestPresenter presenter;

        private IUnitTestProvider provider;
        private ITestExplorer explorer;

        public GallioTestProviderDelegate()
        {
            presenter = new GallioTestPresenter();
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
                IAssemblyInfo assemblyInfo = new MetadataReflector(project).Wrap(assembly);
                if (assemblyInfo != null)
                {
                    // Note: We need the read lock because we access the IDeclaredElement from the declarations cache.
                    foreach (ITest test in TestExplorer.ExploreAssembly(assemblyInfo))
                        ConsumeTest(test, null, consumer);
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

                    ConsumeTest(test, null, delegate(UnitTestElement element)
                    {
                        consumer(element.GetDisposition());
                    });
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
            return new RemoteTaskRunnerInfo(typeof(GallioRemoteTaskRunner));
        }

        public IList<UnitTestTask> GetTaskSequence(UnitTestElement element, IList<UnitTestElement> explicitElements)
        {
            List<UnitTestTask> tasks = new List<UnitTestTask>();
            PopulateUnitTestTasks(tasks, (GallioTestElement) element);

            return tasks;
        }

        private static void PopulateUnitTestTasks(ICollection<UnitTestTask> tasks, GallioTestElement element)
        {
            tasks.Add(CreateUnitTestTask(element));
        }

        private static UnitTestTask CreateUnitTestTask(GallioTestElement element)
        {
            return new UnitTestTask(element,
                new GallioTestRunTask(element.Test.Id, element.GetAssemblyLocation()));
        }

        public int CompareUnitTestElements(UnitTestElement x, UnitTestElement y)
        {
            GallioTestElement xe = (GallioTestElement)x;
            GallioTestElement ye = (GallioTestElement)y;

            return xe.CompareTo(ye);
        }

        private void ConsumeTest(ITest test, UnitTestElement parent, UnitTestElementConsumer consumer)
        {
            GallioTestElement element = new GallioTestElement(test, provider, parent);
            consumer(element);

            foreach (ITest child in test.Children)
                ConsumeTest(child, element, consumer);
        }
    }
}