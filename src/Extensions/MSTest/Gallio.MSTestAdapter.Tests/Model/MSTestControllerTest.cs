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

using Gallio.Model;
using Gallio.Model.Execution;
using Gallio.MSTestAdapter.Model;
using Gallio.Reflection;
using Gallio.Runtime.ProgressMonitoring;
using Gallio.Tests;
using MbUnit.Framework;
using Rhino.Mocks;
using System;
using System.Collections.Generic;

namespace Gallio.MSTestAdapter.Tests.Model
{
    [TestsOn(typeof(MSTestController))]
    [Author("Julian", "julian.hidalgo@gallio.org")]
    public class MSTestControllerTest : BaseTestWithMocks
    {
        //IMSTestProcess stubbedProcess = null;
        //ITestCommand rootCommand = null;
        //ITest rootTest = null;
        //ITestStep parentStep = null;
        //IProgressMonitor progressMonitor = null;
        //MSTestAssembly assemblyTest = null;
        //IAssemblyInfo assemblyCodeElement = null;
        //ITestContext parentContext = null;
        //ITestLogWriter logWriter = null;
        //List<ITestCommand> commands = new List<ITestCommand>();

        [SetUp]
        public override void SetUp()
        {
            //stubbedProcess = Mocks.CreateMock<IMSTestProcess>();
            //rootCommand = Mocks.CreateMock<ITestCommand>();
            //rootTest = Mocks.CreateMock<ITest>();
            //parentStep = Mocks.CreateMock<ITestStep>();
            //progressMonitor = Mocks.Stub<IProgressMonitor>();
            //assemblyCodeElement = Mocks.Stub<IAssemblyInfo>();
            //SetupResult.For(assemblyCodeElement.Name).Throw(new Exception());//.Return("FakeAssemly.dll");
            //assemblyTest = new MSTestAssembly("Fake Assembly", assemblyCodeElement);
            //parentContext = Mocks.Stub<ITestContext>();
            //logWriter = Mocks.Stub<ITestLogWriter>();
        }

        [Test]
        public void Test()
        {
            //using (Mocks.Record())
            //{
            //    Expect.Call(rootCommand.Test).Return(assemblyTest).Repeat.Any();
            //    Expect.Call(rootCommand.TestCount).Return(1);
            //    Expect.Call(rootCommand.StartPrimaryChildStep(null)).IgnoreArguments().Return(parentContext);
            //    Expect.Call(rootCommand.GetAllCommands()).Return(commands);
            //    SetupResult.For(parentContext.LogWriter).Return(logWriter);
            //}

            //using (Mocks.Playback())
            //{
            //    StubbedMSTestController controller = new StubbedMSTestController(stubbedProcess);
            //    controller.RunTestsImpl(rootCommand, parentStep, new TestExecutionOptions(), progressMonitor);
            //}            
        }
    }
}
