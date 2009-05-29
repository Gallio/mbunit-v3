// Copyright 2005-2009 Gallio Project - http://www.gallio.org/
// Portions Copyright 2000-2004 Jonathan de Halleux
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

using System;
using System.Collections.Generic;
using System.Drawing;
using Gallio.Loader;
using Gallio.Model;
using Gallio.Reflection;
using Gallio.ReSharperRunner.Properties;
using Gallio.ReSharperRunner.Provider;
using Gallio.ReSharperRunner.Provider.Facade;
using Gallio.ReSharperRunner.Reflection;
using Gallio.ReSharperRunner.Provider.Tasks;
using Gallio.Runtime;
using JetBrains.CommonControls;
using JetBrains.Metadata.Reader.API;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.ReSharper.TaskRunnerFramework;
using JetBrains.ReSharper.UnitTestExplorer;
using JetBrains.UI.TreeView;

#if RESHARPER_31
using JetBrains.Shell;
using JetBrains.Shell.Progress;
using JetBrains.Util.DataStructures.TreeModel;
#else
using JetBrains.Application;
using JetBrains.Application.Progress;
using JetBrains.TreeModels;
#endif

namespace Gallio.ReSharperRunner.Provider
{
    /// <summary>
    /// This is the main entry point into the Gallio test runner for ReSharper.
    /// </summary>
    [UnitTestProvider]
    public class GallioTestProvider : IUnitTestProvider
    {
        public const string ProviderId = "Gallio";
        private readonly Shim shim;

        static GallioTestProvider()
        {
            GallioLoader.Initialize().SetupRuntime();
        }

        public GallioTestProvider()
        {
            shim = new Shim(this);
        }

        public void ExploreExternal(UnitTestElementConsumer consumer)
        {
            shim.ExploreExternal(consumer);
        }

        public void ExploreSolution(ISolution solution, UnitTestElementConsumer consumer)
        {
            shim.ExploreSolution(solution, consumer);
        }

        public void ExploreAssembly(IMetadataAssembly assembly, IProject project, UnitTestElementConsumer consumer)
        {
            shim.ExploreAssembly(assembly, project, consumer);
        }

        public void ExploreFile(IFile psiFile, UnitTestElementLocationConsumer consumer, CheckForInterrupt interrupted)
        {
            shim.ExploreFile(psiFile, consumer, interrupted);
        }

        public bool IsUnitTestElement(IDeclaredElement element)
        {
            return shim.IsUnitTestElement(element);
        }

#if ! RESHARPER_31 && ! RESHARPER_40 && ! RESHARPER_41
        public bool IsUnitTestStuff(IDeclaredElement element)
        {
            return shim.IsUnitTestStuff(element);
        }
#endif

        public void Present(UnitTestElement element, IPresentableItem item, TreeModelNode node, PresentationState state)
        {
            shim.Present(element, item, node, state);
        }

        public RemoteTaskRunnerInfo GetTaskRunnerInfo()
        {
            return shim.GetTaskRunnerInfo();
        }

        public string Serialize(UnitTestElement element)
        {
            return shim.Serialize(element);
        }

        public UnitTestElement Deserialize(ISolution solution, string elementString)
        {
            return shim.Deserialize(solution, elementString);
        }

        public IList<UnitTestTask> GetTaskSequence(UnitTestElement element, IList<UnitTestElement> explicitElements)
        {
            return shim.GetTaskSequence(element, explicitElements);
        }

        public int CompareUnitTestElements(UnitTestElement x, UnitTestElement y)
        {
            return shim.CompareUnitTestElements(x, y);
        }

        public void ProfferConfiguration(TaskExecutorConfiguration configuration, UnitTestSession session)
        {
            shim.ProfferConfiguration(configuration, session);
        }

        public string ID
        {
            get { return shim.ID; }
        }

#if RESHARPER_45
        public Image Icon
        {
            get { return shim.Icon; }
        }

        public string Name
        {
            get { return shim.Name; }
        }

        public ProviderCustomOptionsControl GetCustomOptionsControl(ISolution solution)
        {
            return shim.GetCustomOptionsControl(solution);
        }
#endif

        internal sealed class Shim
        {
            private static readonly Dictionary<string, Guid> IncompatibleProviders = new Dictionary<string, Guid>()
            {
                { "nUnit", new Guid("{E0273D0F-BEAE-47ff-9391-D6782417F000}") },
                { "MSTest", new Guid("{559AA77B-E0E5-43bb-AF48-EF50D0025D3C}") }
            };

            private readonly IUnitTestProvider provider;
            private readonly GallioTestPresenter presenter;
            private readonly ITestPackageExplorerFactory explorerFactory;

            /// <summary>
            /// Initializes the provider.
            /// </summary>
            public Shim(IUnitTestProvider provider)
            {
                this.provider = provider;

                explorerFactory = RuntimeAccessor.Instance.Resolve<ITestPackageExplorerFactory>();
                presenter = new GallioTestPresenter();
            }

            /// <summary>
            /// Explores the "world", i.e. retrieves tests not associated with current solution.
            /// </summary>
            public void ExploreExternal(UnitTestElementConsumer consumer)
            {
                if (consumer == null)
                    throw new ArgumentNullException("consumer");

                // Nothing to do currently.
            }

            /// <summary>
            /// Explores given solution, but not containing projects.
            /// </summary>
            public void ExploreSolution(ISolution solution, UnitTestElementConsumer consumer)
            {
                if (solution == null)
                    throw new ArgumentNullException("solution");
                if (consumer == null)
                    throw new ArgumentNullException("consumer");

                // Nothing to do currently.
            }

            /// <summary>
            /// Explores given compiled project.
            /// </summary>
            public void ExploreAssembly(IMetadataAssembly assembly, IProject project, UnitTestElementConsumer consumer)
            {
                if (assembly == null)
                    throw new ArgumentNullException("assembly");
                if (project == null)
                    throw new ArgumentNullException("project");
                if (consumer == null)
                    throw new ArgumentNullException("consumer");

#if ! RESHARPER_31 && ! RESHARPER_40 && ! RESHARPER_41
                using (ReadLockCookie.Create())
#endif
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
            }

            private ITestExplorer CreateTestExplorer(IReflectionPolicy reflectionPolicy)
            {
                TestPackageConfig config = new TestPackageConfig();
                
                // Exclude the NUnit framework if the built-in NUnit provider is enabled.
                foreach (IUnitTestProvider provider in UnitTestManager.GetInstance(SolutionManager.Instance.CurrentSolution).GetEnabledProviders())
                {
                    Guid frameworkId;
                    if (IncompatibleProviders.TryGetValue(provider.ID, out frameworkId))
                        config.ExcludedFrameworkIds.Add(frameworkId.ToString());
                }

                return explorerFactory.CreateTestExplorer(config, reflectionPolicy);
            }

            /// <summary>
            /// Explores given PSI file.
            /// </summary>
            public void ExploreFile(IFile psiFile, UnitTestElementLocationConsumer consumer, CheckForInterrupt interrupted)
            {
                if (psiFile == null)
                    throw new ArgumentNullException("psiFile");
                if (consumer == null)
                    throw new ArgumentNullException("consumer");

#if ! RESHARPER_31 && ! RESHARPER_40 && ! RESHARPER_41
                using (ReadLockCookie.Create())
#endif
                {
                    PsiReflectionPolicy reflectionPolicy = new PsiReflectionPolicy(psiFile.GetManager());
                    ConsumerAdapter consumerAdapter = new ConsumerAdapter(provider, consumer, psiFile);
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

                    // Note: We don't call FinishModel because it is ok for the model to be incomplete.

                    ProjectFileState state = explorer.TestModel.Annotations.Count != 0
                        ? ProjectFileState.CreateFromAnnotations(explorer.TestModel.Annotations)
                        : null;
                    ProjectFileState.SetFileState(psiFile.GetProjectFile(), state);
                }
            }

            private static void ExploreTypeDeclaration(PsiReflectionPolicy reflectionPolicy, ITestExplorer explorer, ITypeDeclaration declaration, Action<ITest> consumer)
            {
                ITypeInfo typeInfo = reflectionPolicy.Wrap(declaration.DeclaredElement);

                if (typeInfo != null)
                    explorer.ExploreType(typeInfo, consumer);

                foreach (ITypeDeclaration nestedDeclaration in declaration.NestedTypeDeclarations)
                    ExploreTypeDeclaration(reflectionPolicy, explorer, nestedDeclaration, consumer);
            }

            /// <summary>
            /// Checks if given declared element is UnitTestElement.
            /// </summary>
            public bool IsUnitTestElement(IDeclaredElement element)
            {
                if (element == null)
                    throw new ArgumentNullException("element");

#if ! RESHARPER_31 && ! RESHARPER_40 && ! RESHARPER_41
                using (ReadLockCookie.Create())
#endif
                {
                    PsiReflectionPolicy reflectionPolicy = new PsiReflectionPolicy(element.GetManager());
                    ICodeElementInfo elementInfo = reflectionPolicy.Wrap(element);
                    if (elementInfo == null)
                        return false;

                    ITestExplorer explorer = CreateTestExplorer(reflectionPolicy);
                    return explorer.IsTest(elementInfo);
                }
            }

#if ! RESHARPER_31 && ! RESHARPER_40 && ! RESHARPER_41
            /// <summary>
            /// Checks if given declared element is part of a unit test.  Could be a set up or tear down
            /// method, or something else that belongs to a test.
            /// </summary>
            public bool IsUnitTestStuff(IDeclaredElement element)
            {
                // TODO: Need to change the ITestExplorer API to support this kind of stuff.
                return IsUnitTestElement(element);
            }
#endif

            /// <summary>
            /// Present unit test.
            ///</summary>
            public void Present(UnitTestElement element, IPresentableItem item, TreeModelNode node, PresentationState state)
            {
                if (element == null)
                    throw new ArgumentNullException("element");
                if (item == null)
                    throw new ArgumentNullException("item");
                if (node == null)
                    throw new ArgumentNullException("node");
                if (state == null)
                    throw new ArgumentNullException("state");

                presenter.UpdateItem(element, node, item, state);
            }

            /// <summary>
            /// Gets instance of <see cref="T:JetBrains.ReSharper.TaskRunnerFramework.RemoteTaskRunnerInfo" />.
            /// </summary>
            public RemoteTaskRunnerInfo GetTaskRunnerInfo()
            {
                return new RemoteTaskRunnerInfo(typeof(FacadeTaskRunner));
            }

            /// <summary>
            /// Serializes element for persistence.
            /// </summary>
            public string Serialize(UnitTestElement element)
            {
                if (element == null)
                    throw new ArgumentNullException("element");

                return null;
            }

            /// <summary>
            /// Deserializes element from persisted string.
            /// </summary>
            public UnitTestElement Deserialize(ISolution solution, string elementString)
            {
                if (solution == null)
                    throw new ArgumentNullException("solution");
                if (elementString == null)
                    throw new ArgumentNullException("elementString");

                return null;
            }

            /// <summary>
            /// Gets task information for <see cref="T:JetBrains.ReSharper.TaskRunnerFramework.RemoteTaskRunner" /> from element.
            /// </summary>
            public IList<UnitTestTask> GetTaskSequence(UnitTestElement element, IList<UnitTestElement> explicitElements)
            {
                if (element == null)
                    throw new ArgumentNullException("element");
                if (explicitElements == null)
                    throw new ArgumentNullException("explicitElements");

                GallioTestElement topElement = (GallioTestElement)element;
                List<UnitTestTask> tasks = new List<UnitTestTask>();

                // Add the run task.  Must always be first.
                tasks.Add(new UnitTestTask(null, FacadeUtils.ToRemoteTask(GallioTestRunTask.Instance)));

                // Add the test case branch.
                AddTestTasksFromRootToLeaf(tasks, topElement);

                // Now that we're done with the critical parts of the task tree, we can add other
                // arbitrary elements.  We don't care about the structure of the task tree beyond this depth.

                // Add the assembly location.
                tasks.Add(new UnitTestTask(null, FacadeUtils.ToRemoteTask(new GallioTestAssemblyTask(topElement.GetAssemblyLocation()))));

                if (explicitElements.Count != 0)
                {
                    // Add explicit element markers.
                    foreach (GallioTestElement explicitElement in explicitElements)
                        tasks.Add(new UnitTestTask(null,
                            FacadeUtils.ToRemoteTask(new GallioTestExplicitTask(explicitElement.TestId))));
                }
                else
                {
                    // No explicit elements but we must have at least one to filter by, so we consider
                    // the top test explicitly selected.
                    tasks.Add(new UnitTestTask(null, FacadeUtils.ToRemoteTask(new GallioTestExplicitTask(topElement.TestId))));
                }

                return tasks;
            }

            private static void AddTestTasksFromRootToLeaf(List<UnitTestTask> tasks, GallioTestElement element)
            {
                GallioTestElement parentElement = element.Parent as GallioTestElement;
                if (parentElement != null)
                    AddTestTasksFromRootToLeaf(tasks, parentElement);

                tasks.Add(new UnitTestTask(element, FacadeUtils.ToRemoteTask(new GallioTestItemTask(element.TestId))));
            }

            /// <summary>
            /// Compares unit tests elements to determine relative sort order.
            /// </summary>
            public int CompareUnitTestElements(UnitTestElement x, UnitTestElement y)
            {
                if (x == null)
                    throw new ArgumentNullException("x");
                if (y == null)
                    throw new ArgumentNullException("y");

                GallioTestElement xe = (GallioTestElement)x;
                GallioTestElement ye = (GallioTestElement)y;

                return xe.CompareTo(ye);
            }

            /// <summary>
            /// Obtains configuration data.
            /// </summary>
            public void ProfferConfiguration(TaskExecutorConfiguration configuration, UnitTestSession session)
            {
            }

            /// <summary>
            /// Gets the ID of the provider.
            /// </summary>
            public string ID
            {
                get { return ProviderId; }
            }

#if RESHARPER_45
            /// <summary>
            /// Gets the icon to display in the Options panel or null to use the default.
            /// </summary>
            public Image Icon
            {
                get { return Resources.ProviderIcon; }
            }

            /// <summary>
            /// Gets the name to display in the Options panel.
            /// </summary>
            public string Name
            {
                get { return Resources.ProviderName; }
            }

            /// <summary>
            /// Gets a custom options panel control or null if none.
            /// </summary>
            /// <param name="solution">The solution</param>
            /// <returns>The control, or null if none</returns>
            public ProviderCustomOptionsControl GetCustomOptionsControl(ISolution solution)
            {
                return null;
            }
#endif

            private sealed class ConsumerAdapter
            {
                private readonly IUnitTestProvider provider;
                private readonly Dictionary<ITest, GallioTestElement> tests = new Dictionary<ITest, GallioTestElement>();
                private readonly UnitTestElementConsumer consumer;

                public ConsumerAdapter(IUnitTestProvider provider, UnitTestElementConsumer consumer)
                {
                    this.provider = provider;
                    this.consumer = consumer;
                }

                public ConsumerAdapter(IUnitTestProvider provider, UnitTestElementLocationConsumer consumer, IFile psiFile)
                    : this(provider, delegate(UnitTestElement element)
                    {
#if RESHARPER_31
                        IProjectFile projectFile = psiFile.ProjectItem;
#else
                        IProjectFile projectFile = psiFile.ProjectFile;
#endif

                        UnitTestElementDisposition disposition = element.GetDisposition();
                        if (disposition.Locations.Count != 0 && disposition.Locations[0].ProjectFile == projectFile)
                        {
                            consumer(element.GetDisposition());
                        }
                    })
                {
                }

                public void Consume(ITest test)
                {
                    Consume(test, null);
                }

                private void Consume(ITest test, GallioTestElement parentElement)
                {
                    GallioTestElement element;
                    if (!tests.TryGetValue(test, out element))
                    {
                        if (ShouldTestBePresented(test))
                        {
                            element = GallioTestElement.CreateFromTest(test, provider, parentElement);
                            consumer(element);
                        }

                        tests.Add(test, element);

                        foreach (ITest child in test.Children)
                            Consume(child, element);
                    }
                }

                /// <summary>
                /// ReSharper does not know how to present tests with a granularity any
                /// larger than a type.  The tree it shows to users in such cases is
                /// not very helpful because it appears that the root test is a child of the
                /// project that resides in the root namespace.  So we filter out
                /// certain kinds of tests from view.
                /// </summary>
                private static bool ShouldTestBePresented(ITest test)
                {
                    ICodeElementInfo codeElement = test.CodeElement;
                    if (codeElement == null)
                        return false;

                    switch (codeElement.Kind)
                    {
                        case CodeElementKind.Assembly:
                        case CodeElementKind.Namespace:
                            return false;

                        default:
                            return true;
                    }
                }
            }
        }
    }
}