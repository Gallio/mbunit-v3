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
using Gallio.Model.Helpers;
using NUnit.Core;
using NUnit.Util;

namespace Gallio.NUnitAdapter.Model
{
    /// <summary>
    /// Driver for NUnit tests.
    /// </summary>
    internal class NUnitTestDriver : SimpleTestDriver
    {
        private static bool nunitInitialized;

        public NUnitTestDriver()
        {
            InitializeNUnit();
        }

        protected override string FrameworkName
        {
            get { return "NUnit"; }
        }

        protected override TestExplorer CreateTestExplorer()
        {
            return new NUnitTestExplorer();
        }

        protected override TestController CreateTestController()
        {
            return new DelegatingTestController(test =>
            {
                var topTest = test as NUnitAssemblyTest;
                return topTest != null ? new NUnitTestController(topTest.Runner) : null;
            });
        }

        private static void InitializeNUnit()
        {
            if (!nunitInitialized)
            {
                ServiceManager.Services.AddService(new DomainManager());
                ServiceManager.Services.AddService(new AddinRegistry());
                ServiceManager.Services.AddService(new AddinManager());
                ServiceManager.Services.AddService(new TestAgency());
                ServiceManager.Services.InitializeServices();

                AppDomain.CurrentDomain.SetData("AddinRegistry", Services.AddinRegistry);

                nunitInitialized = true;
            }
        }
    }
}
