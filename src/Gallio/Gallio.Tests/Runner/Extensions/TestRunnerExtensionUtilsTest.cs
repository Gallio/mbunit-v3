// Copyright 2005-2008 Gallio Project - http://www.gallio.org/
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
using Gallio.Runner;
using Gallio.Runner.Extensions;
using Gallio.Runtime;
using Gallio.Utilities;
using MbUnit.Framework;

namespace Gallio.Tests.Runner.Extensions
{
    [TestFixture]
    [TestsOn(typeof(TestRunnerExtensionUtils))]
    public class TestRunnerExtensionUtilsTest
    {
        [Test]
        [Row("Gallio.Runner.Extensions.DebugExtension,Gallio", typeof(DebugExtension), "",
            Description = "Full type name and assembly name with no parameters.")]
        [Row("Gallio.Runner.Extensions.DebugExtension,Gallio;", typeof(DebugExtension), "",
            Description = "Full type name and assembly name with empty parameters.")]
        [Row("Gallio.Runner.Extensions.DebugExtension,Gallio;SomeParameters,AnotherParameter;Foo,;", typeof(DebugExtension), "SomeParameters,AnotherParameter;Foo,;",
            Description = "Full type name and assembly name with full parameters including uninterpreted comma and semicolon.")]
        [Row("DebugExtension,Gallio", typeof(DebugExtension), "",
            Description = "Partial type name and assembly name with no parameters.")]
        [Row("DebugExtension,Gallio.dll", typeof(DebugExtension), "",
            Description = "Partial type name and assembly file with no parameters.")]
        [Row("Extensions.DebugExtension,Gallio", null, "",
            ExpectedException = typeof(RunnerException),
            Description = "Unresolved full type name.")]
        [Row("NoSuchExtension,Gallio", null, "",
            ExpectedException = typeof(RunnerException),
            Description = "Unresolved partial type name.")]
        [Row("DebugExtension,Gallio.NoSuchAssembly", null, "",
            ExpectedException = typeof(RunnerException),
            Description = "Unresolved assembly name.")]
        [Row("DebugExtension,Gallio.NoSuchAssembly.dll", null, "",
            ExpectedException = typeof(RunnerException),
            Description = "Unresolved assembly file.")]
        public void CreateTestRunnerExtension(string extensionSpecification, Type expectedExtensionType,
            string expectedParameters)
        {
            using (new CurrentDirectorySwitcher(RuntimeAccessor.InstallationPath))
            {
                ITestRunnerExtension extension = TestRunnerExtensionUtils.CreateExtensionFromSpecification(extensionSpecification);
                Assert.IsNotNull(extension);
                Assert.IsInstanceOfType(expectedExtensionType, extension);
                Assert.AreEqual(expectedParameters, extension.Parameters);
            }
        }
    }
}
