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
using System.Collections;
using System.IO;
using Gallio.Common.Collections;
using Gallio.Common.Messaging;
using Gallio.Framework.Pattern;
using Gallio.Model;
using Gallio.Model.Messages.Exploration;
using Gallio.Model.Schema;
using Gallio.Model.Tree;
using Gallio.Runtime;
using Gallio.Common.Reflection;
using Gallio.Runtime.Loader;
using Gallio.Runtime.ProgressMonitoring;
using Gallio.VisualStudio.Shell.Core;
using Microsoft.VisualStudio.TestTools.Common;
using TestResult=Microsoft.VisualStudio.TestTools.Common.TestResult;

namespace Gallio.VisualStudio.Tip
{
    /// <summary>
    /// Integrates the Gallio test model as an extension for MSTest.
    /// This enables MSTest to run Gallio tests and to display them in the IDE.
    /// </summary>
    public class GallioTip : Microsoft.VisualStudio.TestTools.Common.Tip
    {
        private readonly ITmi tmi;

        public GallioTip(ITmi tmi)
        {
            if (tmi == null)
                throw new ArgumentNullException("tmi");

            this.tmi = tmi;
        }

        public override ICollection Load(string location, ProjectData projectData, IWarningHandler warningHandler)
        {
            // Skip loading if the extension is not fully initalized unless we are not
            // running in Visual Studio (because we are running in MSTest instead).
            if (!TipShellExtension.IsInitialized && ShellEnvironment.IsRunningInVisualStudio)
                return EmptyArray<TestElement>.Instance;

            // Explore the tests.
            ITestFrameworkManager testFrameworkManager = RuntimeAccessor.ServiceLocator.Resolve<ITestFrameworkManager>();
            WarningLogger logger = new WarningLogger(warningHandler);

            ReflectionOnlyAssemblyLoader loader = new ReflectionOnlyAssemblyLoader();
            loader.AddHintDirectory(Path.GetDirectoryName(location));

            IAssemblyInfo assembly = loader.ReflectionPolicy.LoadAssemblyFrom(location);

            var testFrameworkSelector = new TestFrameworkSelector()
            {
                Filter = testFrameworkHandle => testFrameworkHandle.Id != "MSTestAdapter.TestFramework",
                FallbackMode = TestFrameworkFallbackMode.Approximate
            };

            ITestDriver driver = testFrameworkManager.GetTestDriver(testFrameworkSelector, logger);
            TestExplorationOptions testExplorationOptions = new TestExplorationOptions();

            ArrayList tests = new ArrayList();
            MessageConsumer messageConsumer = new MessageConsumer()
                .Handle<TestDiscoveredMessage>(message =>
                {
                    if (message.Test.IsTestCase)
                        tests.Add(GallioTestElementFactory.CreateTestElement(message.Test, location, projectData));
                })
                .Handle<AnnotationDiscoveredMessage>(message =>
                {
                    message.Annotation.Log(logger, true);
                });

            driver.Describe(loader.ReflectionPolicy, new ICodeElementInfo[] { assembly },
                testExplorationOptions, messageConsumer, NullProgressMonitor.CreateInstance());

            return tests;
        }

        public override void Save(ITestElement[] tests, string location, ProjectData projectData)
        {
            throw new NotSupportedException();
        }

        public override TestResult MergeResults(TestResult inMemory, TestResultMessage fromTheWire)
        {
            // The only type of message we should receive from the wire is a GallioTestResult.
            // However, we can receive multiple results in the case where the test is data-driven
            // so we need to merge them.
            GallioTestResult gallioInMemory = inMemory as GallioTestResult;
            if (gallioInMemory == null && inMemory != null)
                gallioInMemory = new GallioTestResult(gallioInMemory);

            GallioTestResult gallioFromTheWire = fromTheWire as GallioTestResult;
            if (gallioFromTheWire == null && fromTheWire is TestResult)
                gallioFromTheWire = new GallioTestResult((TestResult)fromTheWire);

            return GallioTestResultFactory.Merge(gallioInMemory, gallioFromTheWire);
        }

        public override TestType TestType
        {
            get { return Guids.GallioTestType; }
        }
    }
}
