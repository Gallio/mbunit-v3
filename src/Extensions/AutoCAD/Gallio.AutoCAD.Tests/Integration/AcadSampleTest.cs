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

using System.Diagnostics;
using Autodesk.AutoCAD.ApplicationServices;
using Gallio.Framework;
using MbUnit.Framework;

namespace Gallio.AutoCAD.Tests.Integration
{
    /// <summary>
    /// A sample test to run within AutoCAD.
    /// </summary>
    [Explicit("Sample test.")]
    public class AcadSampleTest
    {
        [Test]
        public void Test()
        {
            TestLog.WriteLine("Running with AutoCAD version {0}.", Application.Version);
            TestLog.WriteLine("Process name {0}.", Process.GetCurrentProcess().ProcessName);

            Assert.IsNotNull(Application.DocumentManager);
        }
    }
}
