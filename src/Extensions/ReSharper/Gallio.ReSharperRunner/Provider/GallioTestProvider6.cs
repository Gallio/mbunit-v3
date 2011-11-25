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
using System.Xml;
using Gallio.Common.Collections;
using Gallio.Common.Messaging;
using Gallio.Common.Messaging.MessageSinks;
using Gallio.Loader;
using Gallio.Model;
using Gallio.Common.Reflection;
using Gallio.Model.Messages.Exploration;
using Gallio.Model.Schema;
using Gallio.ReSharperRunner.Properties;
using Gallio.ReSharperRunner.Provider.Facade;
using Gallio.ReSharperRunner.Reflection;
using Gallio.Runtime;
using Gallio.Runtime.Logging;
using Gallio.Runtime.ProgressMonitoring;
using JetBrains.Metadata.Reader.API;
using JetBrains.ProjectModel;
#if RESHARPER_61
using JetBrains.ReSharper.Feature.Services.UnitTesting;
#endif
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Impl.Caches2;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.ReSharper.TaskRunnerFramework;
using JetBrains.ReSharper.UnitTestFramework;
using JetBrains.Application;
using JetBrains.Application.Progress;

namespace Gallio.ReSharperRunner.Provider
{
    /// <summary>
    /// This is the main entry point into the Gallio test runner for ReSharper.
    /// </summary>
    [UnitTestProvider]
    public class GallioTestProvider : IUnitTestProvider
#if RESHARPER_61
        , IUnitTestingCategoriesAttributeProvider
#endif
    {
        public const string ProviderId = "Gallio";
        private readonly Shim shim;

        private static readonly IClrTypeName CategoryAttribute = new ClrTypeName("MbUnit.Framework.CategoryAttribute");

#if RESHARPER_60
		private UnitTestManager unitTestManager;
#endif

        static GallioTestProvider()
        {
            LoaderManager.InitializeAndSetupRuntimeIfNeeded();
        }

#if RESHARPER_60
		public GallioTestProvider(ISolution solution, PsiModuleManager psiModuleManager, CacheManagerEx cacheManager)
#else
		public GallioTestProvider()
#endif
        {
#if RESHARPER_60
            Solution = solution;
			PsiModuleManager = psiModuleManager;
			CacheManager = cacheManager;
#endif
            shim = new Shim(this);
        }

        public CacheManagerEx CacheManager { get; set; }

#if RESHARPER_60
		public UnitTestManager UnitTestManager
        {
            get { return unitTestManager ?? (unitTestManager = Solution.GetComponent<UnitTestManager>()); }
        }
#else
		public IUnitTestProvidersManager UnitTestProvidersManager { get; set; }
#endif

        public void ExploreExternal(UnitTestElementConsumer consumer)
        {
        }

    	public void ExploreSolution(ISolution solution, UnitTestElementConsumer consumer)
        {
        }

        public void ExploreAssembly(IMetadataAssembly assembly, IProject project, UnitTestElementConsumer consumer)
        {
            shim.ExploreAssembly(assembly, project, consumer);
        }

        public void ExploreFile(IFile psiFile, UnitTestElementLocationConsumer consumer, CheckForInterrupt interrupted)
        {
            shim.ExploreFile(psiFile, consumer, interrupted);
        }

    	public bool IsElementOfKind(IDeclaredElement element, UnitTestElementKind kind)
        {
            return shim.IsElementOfKind(element, kind);
        }

    	public bool IsElementOfKind(IUnitTestElement element, UnitTestElementKind kind)
        {
            return shim.IsElementOfKind(element, kind);
        }

        public bool IsSupported(IHostProvider hostProvider)
        {
            return true;
        }

        public IUnitTestElement DeserializeElement(XmlElement parent, IUnitTestElement parentElement)
        {
            return GallioTestElement.Deserialize(parent, parentElement, this);
        }

        public RemoteTaskRunnerInfo GetTaskRunnerInfo()
        {
            return shim.GetTaskRunnerInfo();
        }

        public int CompareUnitTestElements(IUnitTestElement x, IUnitTestElement y)
        {
            return shim.CompareUnitTestElements(x, y);
        }

    	public void SerializeElement(XmlElement parent, IUnitTestElement element)
    	{
    	    var gallioTestElement = element as GallioTestElement;
            if (gallioTestElement != null)
            {
                gallioTestElement.Serialize(parent);
            }
    	}

        public string ID
        {
            get { return ProviderId; }
        }

        public Image Icon
        {
            get { return shim.Icon; }
        }

    	public string Name
        {
            get { return shim.Name; }
        }

        public ISolution Solution { get; private set; }

        public PsiModuleManager PsiModuleManager { get; set; }

        private sealed class Shim
        {
            private static readonly MultiMap<string, string> IncompatibleProviders = new MultiMap<string, string>
                                                                                         {
                { "nUnit", new[] { "NUnitAdapter248.TestFramework", "NUnitAdapter25.TestFramework" } },
                { "MSTest", new[] { "MSTestAdapter.TestFramework" } }
            };

            private readonly GallioTestProvider provider;
            private readonly ITestFrameworkManager testFrameworkManager;
            private readonly ILogger logger;

        	/// <summary>
            /// Initializes the provider.
            /// </summary>
            public Shim(GallioTestProvider provider)
            {
                this.provider = provider;

                testFrameworkManager = RuntimeAccessor.ServiceLocator.Resolve<ITestFrameworkManager>();
                logger = new FacadeLoggerWrapper(new AdapterFacadeLogger());

                RuntimeAccessor.Instance.AddLogListener(logger);
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

            	try
            	{
#if RESHARPER_60
            		var reflectionPolicy = new MetadataReflectionPolicy(assembly, project);
#else
					var reflectionPolicy = new MetadataReflectionPolicy(assembly, project, provider.CacheManager);
#endif
					IAssemblyInfo assemblyInfo = reflectionPolicy.Wrap(assembly);

            		if (assemblyInfo != null)
            		{
            			var consumerAdapter = new ConsumerAdapter(provider, consumer);
            			Describe(reflectionPolicy, new ICodeElementInfo[] {assemblyInfo}, consumerAdapter);
            		}
            	}
            	catch (Exception ex)
            	{
            		HandleEmbeddedProcessCancelledException(ex);
            		throw;
            	}
            }

        	/// <summary>
            /// Explores given PSI file.
            /// </summary>
			public void ExploreFile(ITreeNode psiFile, UnitTestElementLocationConsumer consumer, CheckForInterrupt interrupted)
        	{
        		if (psiFile == null)
        			throw new ArgumentNullException("psiFile");
        		if (consumer == null)
        			throw new ArgumentNullException("consumer");

        		if (!psiFile.IsValid())
        			return;

        		try
        		{
#if RESHARPER_60
        			var reflectionPolicy = new PsiReflectionPolicy(psiFile.GetPsiServices().PsiManager);
#else
					var reflectionPolicy = new PsiReflectionPolicy(psiFile.GetPsiServices().PsiManager, provider.CacheManager);
#endif
        			var consumerAdapter = new ConsumerAdapter(provider, consumer, psiFile);

        			var codeElements = new List<ICodeElementInfo>();
        			psiFile.ProcessDescendants(new OneActionProcessorWithoutVisit(delegate(ITreeNode element)
					{
						var declaration = element as ITypeDeclaration;
						if (declaration != null)
							PopulateCodeElementsFromTypeDeclaration(codeElements, reflectionPolicy, declaration);
					}, delegate(ITreeNode element)
					{
						if (interrupted())
							throw new ProcessCancelledException();

						// Stop recursing at the first type declaration found.
						return element is ITypeDeclaration;
					}));

        			Describe(reflectionPolicy, codeElements, consumerAdapter);

        			ProjectFileState.SetFileState(psiFile.GetSourceFile().ToProjectFile(), consumerAdapter.CreateProjectFileState());
        		}
        		catch (Exception ex)
        		{
        			HandleEmbeddedProcessCancelledException(ex);
        			throw;
        		}
        	}

        	private static void PopulateCodeElementsFromTypeDeclaration(ICollection<ICodeElementInfo> codeElements, PsiReflectionPolicy reflectionPolicy, ITypeDeclaration declaration)
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

#if RESHARPER_60
				var unitTestManager = UnitTestManager.GetInstance(provider.Solution);
				var unitTestProviders = unitTestManager.GetEnabledProviders();
#else
            	var unitTestProviders = provider.UnitTestProvidersManager.GetEnabledProviders();
#endif
            	foreach (var testProvider in unitTestProviders)
                {
                    IList<string> frameworkIds;
                    if (IncompatibleProviders.TryGetValue(testProvider.ID, out frameworkIds))
                        excludedTestFrameworkIds.AddRange(frameworkIds);
                }

                var testFrameworkSelector = new TestFrameworkSelector
                                                {
                    Filter = testFrameworkHandle => !excludedTestFrameworkIds.Contains(testFrameworkHandle.Id),
                    FallbackMode = TestFrameworkFallbackMode.Approximate
                };
                return testFrameworkManager.GetTestDriver(testFrameworkSelector, logger);
            }

            private void Describe(IReflectionPolicy reflectionPolicy, IList<ICodeElementInfo> codeElements, IMessageSink consumerAdapter)
            {
                var testDriver = CreateTestDriver();
                var testExplorationOptions = new TestExplorationOptions();
                testDriver.Describe(reflectionPolicy, codeElements, testExplorationOptions, consumerAdapter,
                    NullProgressMonitor.CreateInstance());
            }

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

            public bool IsElementOfKind(IUnitTestElement element, UnitTestElementKind kind)
            {
                if (element == null)
                    throw new ArgumentNullException("element");

                var gallioTestElement = element as GallioTestElement;
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

			private bool EvalTestPartPredicate(IDeclaredElement element, Predicate<TestPart> predicate)
			{
				if (!element.IsValid())
					return false;

				try
				{
#if RESHARPER_60
					var reflectionPolicy = new PsiReflectionPolicy(element.GetPsiServices().PsiManager);
#else
					var reflectionPolicy = new PsiReflectionPolicy(element.GetPsiServices().PsiManager, provider.CacheManager);
#endif
					var elementInfo = reflectionPolicy.Wrap(element);
					if (elementInfo == null)
						return false;

					var driver = CreateTestDriver();
					var testParts = driver.GetTestParts(reflectionPolicy, elementInfo);
					return GenericCollectionUtils.Exists(testParts, predicate);
				}
				catch (Exception ex)
				{
					HandleEmbeddedProcessCancelledException(ex);
					throw;
				}
			}

            /// <summary>
            /// Gets instance of <see cref="T:JetBrains.ReSharper.TaskRunnerFramework.RemoteTaskRunnerInfo" />.
            /// </summary>
            public RemoteTaskRunnerInfo GetTaskRunnerInfo()
            {
                return new RemoteTaskRunnerInfo(typeof(FacadeTaskRunner));
            }

            /// <summary>
            /// Compares unit tests elements to determine relative sort order.
            /// </summary>
            public int CompareUnitTestElements(IUnitTestElement x, IUnitTestElement y)
            {
                if (x == null)
                    throw new ArgumentNullException("x");
                if (y == null)
                    throw new ArgumentNullException("y");

                var xe = (GallioTestElement)x;
                var ye = (GallioTestElement)y;

                return xe.CompareTo(ye);
            }

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
                private readonly GallioTestProvider provider;
                private readonly UnitTestElementConsumer consumer;
                private readonly MessageConsumer messageConsumer;
                private readonly Dictionary<string, GallioTestElement> tests = new Dictionary<string, GallioTestElement>();
                private readonly List<AnnotationData> annotations = new List<AnnotationData>();

                public ConsumerAdapter(GallioTestProvider provider, UnitTestElementConsumer consumer)
                {
                    this.provider = provider;
                    this.consumer = consumer;

                    messageConsumer = new MessageConsumer()
                        .Handle<TestDiscoveredMessage>(HandleTestDiscoveredMessage)
                        .Handle<AnnotationDiscoveredMessage>(HandleAnnotationDiscoveredMessage);
                }

                public ConsumerAdapter(GallioTestProvider provider, UnitTestElementLocationConsumer consumer, ITreeNode psiFile)
                    : this(provider, delegate(IUnitTestElement element)
                    {
                        var projectFile = psiFile.GetSourceFile().ToProjectFile();

                    	if (projectFile == null || !projectFile.IsValid()) 
							return;

                        consumer(element.GetDisposition());
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

        public IEnumerable<string> CategoryAttributes
        {
            get { yield return CategoryAttribute.FullName; }
        }
    }
}
