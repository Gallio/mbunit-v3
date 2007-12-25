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
using Gallio.Reflection;
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

        public GallioTestProviderDelegate()
        {
            presenter = new GallioTestPresenter();
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
            MetadataReflector reflector = new MetadataReflector(BuiltInAssemblyResolver.Instance, assembly, project);
            IAssemblyInfo assemblyInfo = reflector.Wrap(assembly);

            if (assemblyInfo != null)
            {
                ConsumerAdapter consumerAdapter = new ConsumerAdapter(provider, consumer);
                ITestExplorer explorer = CreateTestExplorer(reflector);

              explorer.ExploreAssembly(assemblyInfo, consumerAdapter.Consume);
            }
        }

        public void ExploreFile(IFile psiFile, UnitTestElementLocationConsumer consumer, CheckForInterrupt interrupted)
        {
            PsiReflector reflector = new PsiReflector(BuiltInAssemblyResolver.Instance, psiFile.GetManager());
            ConsumerAdapter consumerAdapter = new ConsumerAdapter(provider, consumer);
            ITestExplorer explorer = CreateTestExplorer(reflector);

            psiFile.ProcessDescendants(new OneActionProcessorWithoutVisit(delegate(IElement element)
            {
                ITypeDeclaration declaration = element as ITypeDeclaration;
                if (declaration != null)
                    ExploreTypeDeclaration(reflector, explorer, declaration, consumerAdapter.Consume);
            }, delegate(IElement element)
            {
                if (interrupted())
                    throw new ProcessCancelledException();

                // Stop recursing at the first type declaration found.
                return element is ITypeDeclaration;
            }));
        }

        private void ExploreTypeDeclaration(PsiReflector reflector, ITestExplorer explorer, ITypeDeclaration declaration, Action<ITest> consumer)
        {
            ITypeInfo typeInfo = reflector.Wrap(declaration.DeclaredElement);

            if (typeInfo != null)
                explorer.ExploreType(typeInfo, consumer);

            foreach (ITypeDeclaration nestedDeclaration in declaration.NestedTypeDeclarations)
                ExploreTypeDeclaration(reflector, explorer, nestedDeclaration, consumer);
        }

        public bool IsUnitTestElement(IDeclaredElement element)
        {
            PsiReflector reflector = new PsiReflector(BuiltInAssemblyResolver.Instance, element.GetManager());
            ICodeElementInfo elementInfo = reflector.Wrap(element, false);
            if (elementInfo == null)
                return false;

            ITestExplorer explorer = CreateTestExplorer(reflector);
            return explorer.IsTest(elementInfo);
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
            GallioTestElement topElement = (GallioTestElement)element;
            if (!topElement.Test.IsTestCase)
                return Collections.EmptyArray<UnitTestTask>.Instance;

            List<UnitTestTask> tasks = new List<UnitTestTask>();

            // Add the run task.  Must always be first.
            tasks.Add(new UnitTestTask(null, GallioTestRunTask.Instance));

            // Add the assembly location.
            tasks.Add(new UnitTestTask(null, new GallioTestAssemblyTask(topElement.GetAssemblyLocation())));

            // Add the test case.
            tasks.Add(new UnitTestTask(topElement, new GallioTestItemTask(topElement.Test.Id)));

            // Now add each explicit test id directly under the run task so they do not form chains
            // under other tasks which can produce strange results in the task execution node tree.
            foreach (GallioTestElement explicitElement in explicitElements)
            {
                tasks.Add(new UnitTestTask(null, GallioTestRunTask.Instance));
                tasks.Add(new UnitTestTask(null, new GallioTestExplicitTask(explicitElement.Test.Id)));
            }

            return tasks;
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
                UnitTestElement element;

                if (ShouldTestBePresented(test))
                {
                    element = MapTest(test, parentElement);
                    consumer(element);
                }
                else
                {
                    element = null;
                }

                foreach (ITest childTest in test.Children)
                    Consume(childTest, element);
            }

            private UnitTestElement MapTest(ITest test, UnitTestElement parentElement)
            {
                UnitTestElement element;
                if (!tests.TryGetValue(test, out element))
                {
                    element = new GallioTestElement(test, provider, parentElement);
                    tests.Add(test, element);
                }

                return element;
            }

            /// <summary>
            /// ReSharper does not know how to present tests with a granularity any
            /// larger than a namespace.  The tree it shows to users in such cases is
            /// not very helpful because it appear that the root test is a child of the
            /// project that resides in the root namespace.  So we filter out
            /// certain kinds of tests from view.
            /// </summary>
            private static bool ShouldTestBePresented(ITest test)
            {
                switch (test.Metadata.GetValue(MetadataKeys.TestKind))
                {
                    case TestKinds.Root:
                    case TestKinds.Framework:
                    case TestKinds.Assembly:
                        return false;

                    default:
                        return true;
                }
            }
        }

        private static ITestExplorer CreateTestExplorer(IReflectionPolicy reflectionPolicy)
        {
            TestPackage testPackage = new TestPackage(new TestPackageConfig(), reflectionPolicy);
            TestModel testModel = new TestModel(testPackage);
            return AggregateTestExplorer.CreateExplorerForAllTestFrameworks(testModel);
        }
    }
}