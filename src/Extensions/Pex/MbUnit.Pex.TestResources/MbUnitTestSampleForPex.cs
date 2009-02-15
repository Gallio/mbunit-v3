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

// ==++==
// 
//   Copyright (c) Pex Extensions Project (http://www.codeplex.com/pex). All rights reserved.
//   License: MS-Pl 
//
// ==--==

using System;
using Microsoft.Pex.Framework;
using Microsoft.Pex.Engine;
using Microsoft.Pex.Engine.ComponentModel;
using Microsoft.Pex.Framework.Packages;
using Microsoft.Pex.Framework.Validation;
using MbUnit.Framework;

namespace MbUnit.Pex.TestResources
{
    /// <summary>
    /// This fixture should be run through Pex to ensure
    /// that MbUnit is properly supported.
    /// </summary>
    [TestFixture, PexClass(Suite = "integration")]
    public partial class MbUnitTestSampleForPex
    {
        internal static bool ClassInitializeTouched;
        internal static bool ClassCleanupTouched;
        internal static bool TestInitializeTouched;
        internal static bool TestCleanupTouched;

        [FixtureSetUp]
        public void ClassInitialize()
        {
            ClassInitializeTouched = true;
            Console.WriteLine("ClassInitialize");
        }

        [FixtureTearDown]
        public void ClassCleanup()
        {
            ClassCleanupTouched = true;
            Console.WriteLine("ClassCleanup");
        }

        [SetUp]
        public void TestInitialize()
        {
            TestInitializeTouched = true;
            Console.WriteLine("TestInitialize");
        }

        [TearDown]
        public void TestCleanup()
        {
            TestCleanupTouched = true;
            Console.WriteLine("TestCleanup");
        }

        [PexMethod]
        [SetUpTearDownRecorder]
        public void VerifyAssemblySetupTearDown()
        {
            Assert.IsTrue(true);
        }

        [PexMethod, PexExpectedTests(FailureCount = 1)]
        public void FailedAssertion()
        {
            Assert.IsTrue(false);
        }

        private class SetUpTearDownRecorderAttribute : 
            PexExplorationPackageAttributeBase
        {
            protected override object BeforeExploration(IPexExplorationComponent host)
            {
                if (!AssemblyFixture.SetUpCalled)
                    host.Log.LogError(null, PexLogCategories.Execution, "AssemblySetUp was not run");
                if (!ClassInitializeTouched)
                    host.Log.LogError(null, PexLogCategories.Execution, "ClassInitialize was not run");
                host.Log.LogMessage(PexLogCategories.Execution, "assembly setup verified");
                return null;
            }

            protected override void AfterExploration(IPexExplorationComponent host, object data)
            {
                if (!TestInitializeTouched)
                    host.Log.LogError(null, PexLogCategories.Execution, "TestInitialize was not run");
                if (!TestCleanupTouched)
                    host.Log.LogError(null, PexLogCategories.Execution, "TestCleanup was not run");
                host.Log.LogMessage(PexLogCategories.Execution, "setup teardown verified");
            }
        }
    }
}
