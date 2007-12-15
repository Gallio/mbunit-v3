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
using System;

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
            MetadataReflector reflector = new MetadataReflector(project, ReflectorUtils.GetMetadataLoaderHack(assembly));
            IAssemblyInfo assemblyInfo = reflector.Wrap(assembly);

            if (assemblyInfo != null)
            {
                TestModel testModel = new TestModel();
                ConsumerAdapter consumerAdapter = new ConsumerAdapter(provider, consumer);

                TestExplorer.ExploreAssembly(assemblyInfo, testModel, consumerAdapter.Consume);
            }
        }

        public void ExploreFile(IFile psiFile, UnitTestElementLocationConsumer consumer, CheckForInterrupt interrupted)
        {
            TestModel testModel = new TestModel();
            ConsumerAdapter consumerAdapter = new ConsumerAdapter(provider, consumer);

            psiFile.ProcessDescendants(new OneActionProcessorWithoutVisit(delegate(IElement element)
            {
                ITypeDeclaration declaration = element as ITypeDeclaration;
                if (declaration != null)
                    ExploreTypeDeclaration(declaration, testModel, consumerAdapter.Consume);
            }, delegate(IElement element)
            {
                if (interrupted())
                    throw new ProcessCancelledException();

                // Stop recursing at the first type declaration found.
                return element is ITypeDeclaration;
            }));
        }

        private void ExploreTypeDeclaration(ITypeDeclaration declaration, TestModel testModel,
            Action<ITest> consumer)
        {
            ITypeInfo typeInfo = PsiReflector.Wrap(declaration.DeclaredElement);

            if (typeInfo != null)
                TestExplorer.ExploreType(typeInfo, testModel, consumer);

            foreach (ITypeDeclaration nestedDeclaration in declaration.NestedTypeDeclarations)
                ExploreTypeDeclaration(nestedDeclaration, testModel, consumer);
        }

        public bool IsUnitTestElement(IDeclaredElement element)
        {
            ICodeElementInfo elementInfo = PsiReflector.Wrap(element, false);
            if (elementInfo == null)
                return false;

            return TestExplorer.IsTest(elementInfo);
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
            GallioTestElement self = element as GallioTestElement;

            PopulateSelfAndAncestorTasks(tasks, self);
            PopulateDescendentTasks(tasks, self);

            return tasks;
        }

        private static void PopulateSelfAndAncestorTasks(ICollection<UnitTestTask> tasks, GallioTestElement element)
        {
            if (element != null)
            {
                PopulateSelfAndAncestorTasks(tasks, element.Parent as GallioTestElement);

                tasks.Add(CreateUnitTestTask(element));
            }
        }

        private static void PopulateDescendentTasks(ICollection<UnitTestTask> tasks, GallioTestElement element)
        {
            if (element != null)
            {
            tasks.Add(CreateUnitTestTask(element));

            foreach (UnitTestElement childElement in element.Children)
                PopulateDescendentTasks(tasks, childElement as GallioTestElement);
            }
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

        private sealed class ConsumerAdapter
        {
            private readonly IUnitTestProvider provider;
            private readonly Dictionary<ITest, UnitTestElement> tests = new Dictionary<ITest, UnitTestElement>();
            private readonly UnitTestElementConsumer consumer;

            public ConsumerAdapter(IUnitTestProvider provider, UnitTestElementConsumer consumer)
            {
                this.provider = provider;
                this.consumer = consumer;
            }

            public ConsumerAdapter(IUnitTestProvider provider, UnitTestElementLocationConsumer consumer)
                : this(provider, delegate(UnitTestElement element)
                {
                    consumer(element.GetDisposition());
                }) 
            {
            }

            public void Consume(ITest test)
            {
                Consume(test, null);
            }

            private void Consume(ITest test, UnitTestElement parentElement)
            {
                UnitTestElement element = MapTest(test, parentElement);
                consumer(element);

                foreach (ITest childTest in test.Children)
                    Consume(childTest, element);
            }

            private UnitTestElement MapTest(ITest test, UnitTestElement parentElement)
            {
                if (test == null)
                    return null;

                UnitTestElement element;
                if (!tests.TryGetValue(test, out element))
                {
                    if (parentElement == null)
                        parentElement = MapTest(test.Parent, null);

                    element = new GallioTestElement(test, provider, parentElement);
                    tests.Add(test, element);
                }

                return element;
            }
        }
    }
}