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

using System;
using System.Collections;
using System.IO;
using Gallio.Loader;
using Gallio.Model;
using Gallio.Model.Serialization;
using Gallio.MSTestRunner.Runtime;
using Gallio.Reflection;
using Microsoft.VisualStudio.TestTools.Common;
using TestResult=Microsoft.VisualStudio.TestTools.Common.TestResult;

namespace Gallio.MSTestRunner
{
    /// <summary>
    /// Integrates the Gallio test model as an extension for MSTest.
    /// This enables MSTest to run Gallio tests and to display them in the IDE.
    /// </summary>
    internal class GallioTip : Tip
    {
        private readonly ITmi tmi;

        static GallioTip()
        {
            GallioAssemblyResolver.Install(typeof(GallioPackage).Assembly);
        }

        public GallioTip(ITmi tmi)
        {
            if (tmi == null)
                throw new ArgumentNullException("tmi");

            this.tmi = tmi;
        }

        public override ICollection Load(string location, ProjectData projectData, IWarningHandler warningHandler)
        {
            ITestPackageExplorerFactory explorerFactory = RuntimeProvider.GetRuntime().Resolve<ITestPackageExplorerFactory>();
            WarningLogger logger = new WarningLogger(warningHandler);

            ArrayList tests = new ArrayList();
            TestPackageConfig testPackageConfig = new TestPackageConfig();
            testPackageConfig.AssemblyFiles.Add(location);

            ReflectionOnlyAssemblyLoader loader = new ReflectionOnlyAssemblyLoader();
            loader.AddHintDirectory(Path.GetDirectoryName(location));

            ITestExplorer explorer = explorerFactory.CreateTestExplorer(testPackageConfig, loader.ReflectionPolicy);
            IAssemblyInfo assembly = loader.ReflectionPolicy.LoadAssemblyFrom(location);
            explorer.ExploreAssembly(assembly, null);

            foreach (ITest test in explorer.TestModel.AllTests)
            {
                if (test.IsTestCase)
                    tests.Add(new GallioTestElement(new TestData(test), location, projectData));
            }

            foreach (Annotation annotation in explorer.TestModel.Annotations)
                new AnnotationData(annotation).Log(logger);

            return tests;
        }

        public override void Save(ITestElement[] tests, string location, ProjectData projectData)
        {
            throw new NotSupportedException();
        }

        public override TestResult MergeResults(TestResult inMemory, TestResultMessage fromTheWire)
        {
            // Use the base code for merging results.
            TestResult testResult = base.MergeResults(inMemory, fromTheWire);

            // If the base code did not handle our result type, then do extra work.
            GallioTestResult gallioTestResult = testResult as GallioTestResult;
            if (gallioTestResult == null)
            {
                gallioTestResult = new GallioTestResult(testResult);

                GallioTestResult source = inMemory as GallioTestResult;
                if (source != null)
                    gallioTestResult.MergeFrom(source);
            }

            return gallioTestResult;
        }

        public override TestType TestType
        {
            get { return Guids.GallioTestType; }
        }
    }
}
