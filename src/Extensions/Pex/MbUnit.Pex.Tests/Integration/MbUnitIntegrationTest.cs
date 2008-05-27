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

// ==++==
// 
//   Copyright (c) Pex Extensions Project (http://www.codeplex.com/pex). All rights reserved.
//   License: MS-Pl 
//
// ==--==

using System;
using Gallio.Concurrency;
using Gallio.Framework;
using Gallio.Reflection;
using MbUnit.Framework;
using MbUnit.Pex.TestResources;
using Microsoft.Pex.Engine;
using Microsoft.Pex.Engine.Utilities;
using Microsoft.ExtendedReflection.Monitoring;

namespace MbUnit.Pex.Tests.Integration
{
    [TestFixture]
    public class MbUnitIntegrationTest
    {
        private readonly string pexConsole = PexInstallHelper.GetPexConsole(Bitness.Any);

        [Test]
        public void ExecutePex()
        {
            PexEngineOptions options = PexEngineOptions.CreateDefault();
            options.DoNotOpenReport = true;
            options.CompileTestProject = true;
            options.TestNoPartialClasses = true;
            options.Assembly = AssemblyUtils.GetFriendlyAssemblyLocation(typeof(MbUnitTestSampleForPex).Assembly);

            string arguments = String.Join(" ", options.GetCommandLineArguments(true));

            ProcessTask task = Tasks.StartProcessTask(pexConsole, arguments, Environment.CurrentDirectory);
            Tasks.WatchTask(task);
            Tasks.JoinAndVerify(new TimeSpan(0, 2, 0));

            Log.WriteLine(PexExitCodes.Describe(task.ExitCode));
            Assert.AreEqual(PexExitCodes.Success, task.ExitCode);
        }
    }
}
