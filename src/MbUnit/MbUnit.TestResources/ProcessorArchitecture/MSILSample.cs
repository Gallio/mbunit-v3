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
using System.Diagnostics;
using System.IO;
using System.Text;
using Gallio.Framework;
using Gallio.Runner;
using MbUnit.Framework;

namespace MbUnit.TestResources.ProcessorArchitecture
{
    /// <summary>
    /// Verifies that the test is being run using the right host process assuming it is
    /// being run using the <see cref="StandardTestRunnerFactoryNames.IsolatedProcess" /> test runner.
    /// </summary>
    public class MSILSample
    {
        [Test]
        public void VerifyHostProcessName()
        {
            Assert.Contains(Process.GetCurrentProcess().MainModule.FileName, "Gallio.Host.exe");
        }
    }
}
