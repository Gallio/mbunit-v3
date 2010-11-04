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
using System.IO;
using System.Text;
using Gallio.Common.Messaging;
using Gallio.MbUnitCppAdapter.Model.Bridge;
using Gallio.Model;
using Gallio.Model.Isolation;
using Gallio.Model.Messages;
using Gallio.Model.Tree;
using Gallio.Runtime.Logging;
using Gallio.Runtime.ProgressMonitoring;

namespace Gallio.MbUnitCppAdapter.Model.Tasks
{
    internal abstract class AbstractTask : IsolatedTask
    {
        protected TestPackage TestPackage
        {
            get;
            private set;
        }

        protected TestExplorationOptions TestExplorationOptions
        {
            get;
            private set;
        }

        protected TestExecutionOptions TestExecutionOptions
        {
            get;
            private set;
        }

        protected IMessageSink MessageSink
        {
            get;
            private set;
        }

        protected ILogger Logger
        {
            get;
            private set;
        }

        protected TestModel TestModel
        {
            get;
            private set;
        }

        protected sealed override object RunImpl(object[] args)
        {
            TestPackage = (TestPackage)args[0];
            TestExplorationOptions = (TestExplorationOptions)args[1];
            TestExecutionOptions = (TestExecutionOptions)args[2];
            MessageSink = (IMessageSink)args[3];
            var progressMonitor = (IProgressMonitor)args[4];
            Logger = (ILogger)args[5];
            var fileInfo = (FileInfo)args[6];
            TestModel = new TestModel();

            using (var subProgressMonitor = progressMonitor.CreateSubProgressMonitor(1))
            {
                if (!subProgressMonitor.IsCanceled)
                {
                    using (var repository = new UnmanagedTestRepository(fileInfo.FullName))
                    {
                        if (repository.IsValid)
                        {
                            Execute(repository, subProgressMonitor); 
                        }
                    }
                }
            }

            return null;
        }

        protected abstract void Execute(UnmanagedTestRepository repository, IProgressMonitor progressMonitor);

        protected void BuildTestModel(UnmanagedTestRepository repository, IProgressMonitor progressMonitor)
        {
            Test testFixture = TestModel.RootTest;

            foreach (var testInfoData in repository.GetTests())
            {
                if (progressMonitor.IsCanceled)
                    return;

                var test = new MbUnitCppTest(testInfoData);

                if (testInfoData.IsTestFixture)
                {
                    TestModel.RootTest.AddChild(test);
                    testFixture = test;
                }
                else
                {
                    testFixture.AddChild(test);
                }
            }

            TestModelSerializer.PublishTestModel(TestModel, MessageSink);
        }
    }
}
