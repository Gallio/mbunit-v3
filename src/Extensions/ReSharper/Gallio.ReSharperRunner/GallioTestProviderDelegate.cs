// Copyright 2005-2008 Gallio Project - http://www.gallio.org/
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
using Gallio.Hosting;
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
            MetadataReflectionPolicy reflectionPolicy = new MetadataReflectionPolicy(assembly, project);
            IAssemblyInfo assemblyInfo = reflectionPolicy.Wrap(assembly);

            if (assemblyInfo != null)
            {
                ConsumerAdapter consumerAdapter = new ConsumerAdapter(provider, consumer);
                ITestExplorer explorer = CreateTestExplorer(reflectionPolicy);

                explorer.ExploreAssembly(assemblyInfo, consumerAdapter.Consume);
                explorer.FinishModel();
            }
        }

        public void ExploreFile(IFile psiFile, UnitTestElementLocationConsumer consumer, CheckForInterrupt interrupted)
        {
            PsiReflectionPolicy reflectionPolicy = new PsiReflectionPolicy(psiFile.GetManager());
            ConsumerAdapter consumerAdapter = new ConsumerAdapter(provider, consumer);
            ITestExplorer explorer = CreateTestExplorer(reflectionPolicy);

            psiFile.ProcessDescendants(new OneActionProcessorWithoutVisit(delegate(IElement element)
            {
                ITypeDeclaration declaration = element as ITypeDeclaration;
                if (declaration != null)
                    ExploreTypeDeclaration(reflectionPolicy, explorer, declaration, consumerAdapter.Consume);
            }, delegate(IElement element)
            {
                if (interrupted())
                    throw new ProcessCancelledException();

                // Stop recursing at the first type declaration found.
                return element is ITypeDeclaration;
            }));

            // Note: We don't call FinishModel because we know the model will be incomplete.

            GallioProjectFileState state = explorer.TestModel.Annotations.Count != 0
                ? new GallioProjectFileState(explorer.TestModel.Annotations)
                : null;
            GallioProjectFileState.SetFileState(psiFile.GetProjectFile(), state);
        }

        private static void ExploreTypeDeclaration(PsiReflectionPolicy reflectionPolicy, ITestExplorer explorer, ITypeDeclaration declaration, Action<ITest> consumer)
        {
            ITypeInfo typeInfo = reflectionPolicy.Wrap(declaration.DeclaredElement);

            if (typeInfo != null)
                explorer.ExploreType(typeInfo, consumer);

            foreach (ITypeDeclaration nestedDeclaration in declaration.NestedTypeDeclarations)
                ExploreTypeDeclaration(reflectionPolicy, explorer, nestedDeclaration, consumer);
        }

        public bool IsUnitTestElement(IDeclaredElement element)
        {
            PsiReflectionPolicy reflectionPolicy = new PsiReflectionPolicy(element.GetManager());
            ICodeElementInfo elementInfo = reflectionPolicy.Wrap(element);
            if (elementInfo == null)
                return false;

            ITestExplorer explorer = CreateTestExplorer(reflectionPolicy);
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
            List<UnitTestTask> tasks = new List<UnitTestTask>();

            // Add the run task.  Must always be first.
            tasks.Add(new UnitTestTask(null, GallioTestRunTask.Instance));

            // Add the test case branch.
            AddTestTasksFromRootToLeaf(tasks, topElement);

            // Now that we're done with the critical parts of the task tree, we can add other
            // arbitrary elements.  We don't care about the structure of the task tree beyond this depth.

            // Add the assembly location.
            tasks.Add(new UnitTestTask(null, new GallioTestAssemblyTask(topElement.GetAssemblyLocation())));

            // Add explicit element markers.
            foreach (GallioTestElement explicitElement in explicitElements)
                tasks.Add(new UnitTestTask(null, new GallioTestExplicitTask(explicitElement.Test.Id)));

            return tasks;
        }

        private static void AddTestTasksFromRootToLeaf(List<UnitTestTask> tasks, GallioTestElement element)
        {
            // This is disabled right now because R# does weird things with the test tree
            // It introduces additional bogus top-level notes in the Unit Test Session.
            //
            // To reproduce:
            //   Run a test by clicking on the Run Test action in the margin of a test editor.
            //   Modify the test.
            //   Switch to Projects and Namespaces view.
            //   Then click Run All Tests from within the Unit Test Session.
            //
            // Notice:
            //   The first time, all is well.
            //   The second time, a new node for the test fixture's containing namespace is created at the top level.
            //   so the fixture will appear twice, once under its project and once under the duplicate namespace.
            /*
            GallioTestElement parentElement = element.Parent as GallioTestElement;
            if (parentElement != null)
                AddTestTasksFromRootToLeaf(tasks, parentElement);
             */

            if (!element.Test.IsTestCase)
                return; // workaround

            tasks.Add(new UnitTestTask(element, new GallioTestItemTask(element.Test.Id)));
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

            AggregateTestExplorer aggregate = new AggregateTestExplorer();
            foreach (ITestFramework framework in Runtime.Instance.ResolveAll<ITestFramework>())
                aggregate.AddTestExplorer(framework.CreateTestExplorer(testModel));

            return aggregate;
        }
    }
}