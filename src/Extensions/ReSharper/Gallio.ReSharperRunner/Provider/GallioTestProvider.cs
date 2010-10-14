// Copyright 2005-2010 Gallio Project - http://www.gallio.org/
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
using Gallio.Common;
using Gallio.Common.Collections;
using Gallio.Common.Messaging;
using Gallio.Framework.Pattern;
using Gallio.Loader;
using Gallio.Model;
using Gallio.Common.Reflection;
using Gallio.Model.Messages.Exploration;
using Gallio.Model.Schema;
using Gallio.Model.Tree;
using Gallio.ReSharperRunner.Properties;
using Gallio.ReSharperRunner.Provider;
using Gallio.ReSharperRunner.Provider.Facade;
using Gallio.ReSharperRunner.Reflection;
using Gallio.ReSharperRunner.Provider.Tasks;
using Gallio.Runtime;
using Gallio.Runtime.Logging;
using Gallio.Runtime.ProgressMonitoring;
using JetBrains.CommonControls;
using JetBrains.Metadata.Reader.API;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.ReSharper.TaskRunnerFramework;
using JetBrains.UI.TreeView;
using Gallio.Runtime.Loader;

using JetBrains.ReSharper.UnitTestExplorer;
#if RESHARPER_50_OR_NEWER
using JetBrains.ReSharper.UnitTestFramework;
using JetBrains.ReSharper.UnitTestFramework.UI;
#endif

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
            LoaderManager.InitializeAndSetupRuntimeIfNeeded();
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

#if ! RESHARPER_50_OR_NEWER
        public bool IsUnitTestElement(IDeclaredElement element)
        {
            return shim.IsUnitTestElement(element);
        }
#endif

#if RESHARPER_45
        public bool IsUnitTestStuff(IDeclaredElement element)
        {
            return shim.IsUnitTestStuff(element);
        }
#endif

#if RESHARPER_50_OR_NEWER
        public bool IsElementOfKind(IDeclaredElement element, UnitTestElementKind kind)
        {
            return shim.IsElementOfKind(element, kind);
        }

        public bool IsElementOfKind(UnitTestElement element, UnitTestElementKind kind)
        {
            return shim.IsElementOfKind(element, kind);
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

#if RESHARPER_45_OR_NEWER
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
            private static readonly MultiMap<string, string> IncompatibleProviders = new MultiMap<string, string>()
            {
                { "nUnit", new[] { "NUnitAdapter248.TestFramework", "NUnitAdapter25.TestFramework" } },
                { "MSTest", new[] { "MSTestAdapter.TestFramework" } }
            };

            private readonly IUnitTestProvider provider;
            private readonly GallioTestPresenter presenter;
            private readonly ITestFrameworkManager testFrameworkManager;
            private readonly ILogger logger;
            private readonly FacadeTaskFactory facadeTaskFactory;

            /// <summary>
            /// Initializes the provider.
            /// </summary>
            public Shim(IUnitTestProvider provider)
            {
                this.provider = provider;

                testFrameworkManager = RuntimeAccessor.ServiceLocator.Resolve<ITestFrameworkManager>();
                presenter = new GallioTestPresenter();
                facadeTaskFactory = new FacadeTaskFactory();
                logger = new FacadeLoggerWrapper(new AdapterFacadeLogger());

                RuntimeAccessor.Instance.AddLogListener(logger);
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

#if ! RESHARPER_31 && ! RESHARPER_40 && ! RESHARPER_41
                using (ReadLockCookie.Create())
#endif
                {
                    if (!solution.IsValid)
                        return;

                    // Nothing to do currently.
                    // TODO: Should consider test files supported by other frameworks like RSpec.
                }
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
                    if (!project.IsValid)
                        return;

                    try
                    {
                        MetadataReflectionPolicy reflectionPolicy = new MetadataReflectionPolicy(assembly, project);
                        IAssemblyInfo assemblyInfo = reflectionPolicy.Wrap(assembly);

                        if (assemblyInfo != null)
                        {
                            ConsumerAdapter consumerAdapter = new ConsumerAdapter(provider, consumer);
                            Describe(reflectionPolicy, new ICodeElementInfo[] { assemblyInfo }, consumerAdapter);
                        }
                    }
                    catch (Exception ex)
                    {
                        HandleEmbeddedProcessCancelledException(ex);
                        throw;
                    }
                }
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
                    if (!psiFile.IsValid())
                        return;

                    try
                    {
                        PsiReflectionPolicy reflectionPolicy = new PsiReflectionPolicy(psiFile.GetManager());
                        ConsumerAdapter consumerAdapter = new ConsumerAdapter(provider, consumer, psiFile);

                        var codeElements = new List<ICodeElementInfo>();
                        psiFile.ProcessDescendants(new OneActionProcessorWithoutVisit(delegate(IElement element)
                        {
                            ITypeDeclaration declaration = element as ITypeDeclaration;
                            if (declaration != null)
                                PopulateCodeElementsFromTypeDeclaration(codeElements, reflectionPolicy, declaration);
                        }, delegate(IElement element)
                        {
                            if (interrupted())
                                throw new ProcessCancelledException();

                            // Stop recursing at the first type declaration found.
                            return element is ITypeDeclaration;
                        }));

                        Describe(reflectionPolicy, codeElements, consumerAdapter);

                        ProjectFileState.SetFileState(psiFile.GetProjectFile(), consumerAdapter.CreateProjectFileState());
                    }
                    catch (Exception ex)
                    {
                        HandleEmbeddedProcessCancelledException(ex);
                        throw;
                    }
                }
            }

            private static void PopulateCodeElementsFromTypeDeclaration(List<ICodeElementInfo> codeElements, PsiReflectionPolicy reflectionPolicy, ITypeDeclaration declaration)
            {
                if (! declaration.IsValid())
                    return;

                ITypeInfo typeInfo = reflectionPolicy.Wrap(declaration.DeclaredElement);

                if (typeInfo != null)
                    codeElements.Add(typeInfo);

                foreach (ITypeDeclaration nestedDeclaration in declaration.NestedTypeDeclarations)
                    PopulateCodeElementsFromTypeDeclaration(codeElements, reflectionPolicy, nestedDeclaration);
            }

            private ITestDriver CreateTestDriver()
            {
                var excludedTestFrameworkIds = new List<string>();
                foreach (IUnitTestProvider provider in UnitTestManager.GetInstance(SolutionManager.Instance.CurrentSolution).GetEnabledProviders())
                {
                    IList<string> frameworkIds;
                    if (IncompatibleProviders.TryGetValue(provider.ID, out frameworkIds))
                        excludedTestFrameworkIds.AddRange(frameworkIds);
                }

                var testFrameworkSelector = new TestFrameworkSelector()
                {
                    Filter = testFrameworkHandle => !excludedTestFrameworkIds.Contains(testFrameworkHandle.Id),
                    FallbackMode = TestFrameworkFallbackMode.Approximate
                };
                return testFrameworkManager.GetTestDriver(testFrameworkSelector, logger);
            }

            private void Describe(IReflectionPolicy reflectionPolicy, IList<ICodeElementInfo> codeElements, ConsumerAdapter consumerAdapter)
            {
                var testDriver = CreateTestDriver();
                var testExplorationOptions = new TestExplorationOptions();
                testDriver.Describe(reflectionPolicy, codeElements, testExplorationOptions, consumerAdapter,
                    NullProgressMonitor.CreateInstance());
            }

#if ! RESHARPER_50_OR_NEWER
            /// <summary>
            /// Checks if given declared element is UnitTestElement.
            /// </summary>
            public bool IsUnitTestElement(IDeclaredElement element)
            {
                if (element == null)
                    throw new ArgumentNullException("element");
                return EvalTestPartPredicate(element, x => x.IsTest);
            }
#endif

#if RESHARPER_45
            /// <summary>
            /// Checks if given declared element is part of a unit test.  
            /// </summary>
            /// <remarks>
            /// <para>
            /// Could be a set up or tear down method, or something else that belongs to a test.
            /// </para>
            /// </remarks>
            public bool IsUnitTestStuff(IDeclaredElement element)
            {
                if (element == null)
                    throw new ArgumentNullException("element");
                return EvalTestPartPredicate(element, x => x.IsTest || x.IsTestContribution);
            }
#endif

#if RESHARPER_50_OR_NEWER
            public bool IsElementOfKind(IDeclaredElement element, UnitTestElementKind kind)
            {
                if (element == null)
                    throw new ArgumentNullException("element");
                return EvalTestPartPredicate(element, testPart =>
                {
                    switch (kind)
                    {
                        case UnitTestElementKind.Unknown:
                            return false;
                        case UnitTestElementKind.Test:
                            return testPart.IsTest;
                        case UnitTestElementKind.TestContainer:
                            return testPart.IsTestContainer;
                        case UnitTestElementKind.TestStuff:
                            return testPart.IsTestContribution;
                        default:
                            throw new ArgumentOutOfRangeException("kind");
                    }
                });
            }

            public bool IsElementOfKind(UnitTestElement element, UnitTestElementKind kind)
            {
                if (element == null)
                    throw new ArgumentNullException("element");

                GallioTestElement gallioTestElement = element as GallioTestElement;
                if (gallioTestElement == null)
                    return false;

                switch (kind)
                {
                    case UnitTestElementKind.Unknown:
                    case UnitTestElementKind.TestStuff:
                        return false;
                    case UnitTestElementKind.Test:
                        return gallioTestElement.IsTestCase;
                    case UnitTestElementKind.TestContainer:
                        return ! gallioTestElement.IsTestCase;
                    default:
                        throw new ArgumentOutOfRangeException("kind");
                }
            }
#endif

            private bool EvalTestPartPredicate(IDeclaredElement element, Predicate<TestPart> predicate)
            {
#if RESHARPER_45_OR_NEWER
                using (ReadLockCookie.Create())
#endif
                {
                    if (!element.IsValid())
                        return false;

                    try
                    {
                        PsiReflectionPolicy reflectionPolicy = new PsiReflectionPolicy(element.GetManager());
                        ICodeElementInfo elementInfo = reflectionPolicy.Wrap(element);
                        if (elementInfo == null)
                            return false;

                        ITestDriver driver = CreateTestDriver();
                        IList<TestPart> testParts = driver.GetTestParts(reflectionPolicy, elementInfo);
                        return GenericCollectionUtils.Exists(testParts, predicate);
                    }
                    catch (Exception ex)
                    {
                        HandleEmbeddedProcessCancelledException(ex);
                        throw;
                    }
                }
            }

            /// <summary>
            /// Presents unit test.
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
                tasks.Add(new UnitTestTask(null, facadeTaskFactory.CreateRootTask()));

                // Add the test case branch.
                AddTestTasksFromRootToLeaf(tasks, topElement);

                // Now that we're done with the critical parts of the task tree, we can add other
                // arbitrary elements.  We don't care about the structure of the task tree beyond this depth.

                // Add the assembly location.
                tasks.Add(new UnitTestTask(null, facadeTaskFactory.CreateAssemblyTaskFrom(topElement)));

                if (explicitElements.Count != 0)
                {
                    // Add explicit element markers.
                    foreach (GallioTestElement explicitElement in explicitElements)
                        tasks.Add(new UnitTestTask(null, facadeTaskFactory.CreateExplicitTestTaskFrom(explicitElement)));
                }
                else
                {
                    // No explicit elements but we must have at least one to filter by, so we consider
                    // the top test explicitly selected.
                    tasks.Add(new UnitTestTask(null, facadeTaskFactory.CreateExplicitTestTaskFrom(topElement)));
                }

                return tasks;
            }

            private void AddTestTasksFromRootToLeaf(List<UnitTestTask> tasks, GallioTestElement element)
            {
                GallioTestElement parentElement = element.Parent as GallioTestElement;
                if (parentElement != null)
                    AddTestTasksFromRootToLeaf(tasks, parentElement);

                tasks.Add(new UnitTestTask(element, facadeTaskFactory.CreateTestTaskFrom(element)));
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

#if RESHARPER_45_OR_NEWER
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
            /// <param name="solution">The solution.</param>
            /// <returns>The control, or null if none.</returns>
            public ProviderCustomOptionsControl GetCustomOptionsControl(ISolution solution)
            {
                return null;
            }
#endif

            /// <summary>
            /// ReSharper can throw ProcessCancelledException while we are performing reflection
            /// over code elements.  Gallio will then often wrap the exception as a ModelException
            /// which ReSharper does not expect.  So we check to see whether a ProcessCancelledException
            /// was wrapped up and throw it again if needed.
            /// </summary>
            private static void HandleEmbeddedProcessCancelledException(Exception exception)
            {
                do
                {
                    if (exception is ProcessCancelledException)
                        throw exception;
                    exception = exception.InnerException;
                }
                while (exception != null);
            }

            private sealed class ConsumerAdapter : IMessageSink
            {
                private readonly IUnitTestProvider provider;
                private readonly UnitTestElementConsumer consumer;
                private readonly MessageConsumer messageConsumer;
                private readonly Dictionary<string, GallioTestElement> tests = new Dictionary<string, GallioTestElement>();
                private readonly List<AnnotationData> annotations = new List<AnnotationData>();

                public ConsumerAdapter(IUnitTestProvider provider, UnitTestElementConsumer consumer)
                {
                    this.provider = provider;
                    this.consumer = consumer;

                    messageConsumer = new MessageConsumer()
                        .Handle<TestDiscoveredMessage>(HandleTestDiscoveredMessage)
                        .Handle<AnnotationDiscoveredMessage>(HandleAnnotationDiscoveredMessage);
                }

                public ConsumerAdapter(IUnitTestProvider provider, UnitTestElementLocationConsumer consumer, IFile psiFile)
                    : this(provider, delegate(UnitTestElement element)
                    {
#if RESHARPER_31
                        IProjectFile projectFile = psiFile.ProjectItem;
#else
                        IProjectFile projectFile = psiFile.ProjectFile;
#endif
                        if (projectFile.IsValid)
                        {
                            UnitTestElementDisposition disposition = element.GetDisposition();
                            if (disposition.Locations.Count != 0 && disposition.Locations[0].ProjectFile == projectFile)
                            {
                                consumer(disposition);
                            }
                        }
                    })
                {
                }

                public void Publish(Message message)
                {
                    messageConsumer.Consume(message);
                }

                private void HandleTestDiscoveredMessage(TestDiscoveredMessage message)
                {
                    GallioTestElement element;
                    if (!tests.TryGetValue(message.Test.Id, out element))
                    {
                        if (ShouldTestBePresented(message.Test.CodeElement))
                        {
                            GallioTestElement parentElement;
                            tests.TryGetValue(message.ParentTestId, out parentElement);

                            element = GallioTestElement.CreateFromTest(message.Test, message.Test.CodeElement, provider, parentElement);
                            consumer(element);
                        }

                        tests.Add(message.Test.Id, element);
                    }
                }

                private void HandleAnnotationDiscoveredMessage(AnnotationDiscoveredMessage message)
                {
                    annotations.Add(message.Annotation);
                }

                public ProjectFileState CreateProjectFileState()
                {
                    return annotations.Count != 0
                        ? ProjectFileState.CreateFromAnnotations(annotations)
                        : null;
                }

                /// <summary>
                /// ReSharper does not know how to present tests with a granularity any
                /// larger than a type.
                /// </summary>
                /// <remarks>
                /// <para>
                /// The tree it shows to users in such cases is
                /// not very helpful because it appears that the root test is a child of the
                /// project that resides in the root namespace.  So we filter out
                /// certain kinds of tests from view.
                /// </para>
                /// </remarks>
                private static bool ShouldTestBePresented(ICodeElementInfo codeElement)
                {
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