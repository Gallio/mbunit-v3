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
using System.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Gallio.MSTestAdapter.TestResources
{
    /// <summary>
    /// A fixture that verifies that emits some debug output to help ensure the
    /// testing environment is being set up correctly.
    /// </summary>
    [TestClass]
    public class EnvironmentSetupSample
    {
        public TestContext TestContext { get; set; }

        [TestMethod]
        public void WriteDiagnostics()
        {
            Console.WriteLine("CodeBase: " + GetType().Assembly.CodeBase);
            Console.WriteLine("AppBase: " + AppDomain.CurrentDomain.BaseDirectory);
            Console.WriteLine("ConfigurationFile: " + AppDomain.CurrentDomain.SetupInformation.ConfigurationFile);
            Console.WriteLine("CurrentDir: " + Environment.CurrentDirectory);
            Console.WriteLine("TestDir: " + TestContext.TestDir);
            Console.WriteLine("TestLogsDir: " + TestContext.TestLogsDir);
            Console.WriteLine("TestDeploymentDir: " + TestContext.TestDeploymentDir);
        }
    }
}
