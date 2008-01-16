// Copyright 2008 MbUnit Project - http://www.mbunit.com/
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
using Gallio.Concurrency;
using Gallio.Hosting;
using MbUnit.Framework;

namespace Gallio.PowerShellCommands.Tests
{
    /// <summary>
    /// Simple integration tests that ensures that the Cmdlet can be used from PowerShell.
    /// </summary>
    /// <remarks>
    /// This test will fail unless PowerShell is configured to allow executing scripts.
    /// The easiest way to do it is by changing the execution policy to remote signed,
    /// so local scripts can be run:
    /// 
    ///     Set-ExecutionPolicy RemoteSigned
    /// 
    /// </remarks>
    [TestFixture]
    [Author("Julian Hidalgo")]
    [TestsOn(typeof(RunGallioCommand))]
    [Category("IntegrationTests")]
    public class RunGallioCommandIntegrationTest
    {
        private string executablePath;
        private string workingDirectory;

        private Hashtable state;

        [SetUp]
        public void InstallSnapIn()
        {
            Hashtable state = new Hashtable();
            new GallioSnapIn().Install(state);
        }

        [TearDown]
        public void UninstallSnapIn()
        {
            if (state != null)
            {
                new GallioSnapIn().Uninstall(state);
                state = null;
            }
        }

        [Test]
        public void RunPowerShell()
        {
            executablePath = Path.Combine
                (
                Environment.GetFolderPath(Environment.SpecialFolder.System),
                @"windowspowershell\v1.0\powershell.exe"
                );

            workingDirectory = Path.GetDirectoryName((Loader.GetAssemblyLocalPath(GetType().Assembly)));

            ProcessTask task = new ProcessTask(executablePath,
               "\"& Add-PSSnapIn Gallio; Run-Gallio 'MbUnit.TestResources.dll' -pd '" +
               Loader.InstallationPath + "' -verbose -filter Type:SimpleTest \"");
            task.WorkingDirectory = workingDirectory;

            Assert.IsTrue(task.Run(TimeSpan.FromSeconds(60)), "A timeout occurred.");

            Assert.Contains(task.ConsoleOutput, "Run: 2, Passed: 1, Failed: 1");
            Assert.AreEqual(task.ExitCode, 1, "Unexpected exit code.");
        }
    }
}
