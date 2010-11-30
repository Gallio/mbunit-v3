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
    /// <summary>
    /// Abstract isolated base task for the MbUnitCpp test adapter.
    /// </summary>
    internal abstract class AbstractTask : IsolatedTask
    {
        /// <summary>
        /// Gets the test package.
        /// </summary>
        protected TestPackage TestPackage
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the test exploration options.
        /// </summary>
        protected TestExplorationOptions TestExplorationOptions
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the test execution options.
        /// </summary>
        protected TestExecutionOptions TestExecutionOptions
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the message sink.
        /// </summary>
        protected IMessageSink MessageSink
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the logging service.
        /// </summary>
        protected ILogger Logger
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the test model.
        /// </summary>
        protected TestModel TestModel
        {
            get;
            private set;
        }

        /// <inheritdoc />
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

        /// <summary>
        /// Executes the tasks.
        /// </summary>
        /// <param name="repository">The MbUnitCpp unmanaged test repository.</param>
        /// <param name="progressMonitor">The active progress monitor for the task.</param>
        protected abstract void Execute(UnmanagedTestRepository repository, IProgressMonitor progressMonitor);

        /// <summary>
        /// Builds the test model.
        /// </summary>
        /// <param name="repository">The MbUnitCpp unmanaged test repository.</param>
        /// <param name="progressMonitor">The active progress monitor for the task.</param>
        protected void BuildTestModel(UnmanagedTestRepository repository, IProgressMonitor progressMonitor)
        {
            Test fixture = null;
            Test group = null;
            var root = repository.CreateRootTest();
            TestModel.RootTest.AddChild(root);

            foreach (var testInfoData in repository.GetTests())
            {
                if (progressMonitor.IsCanceled)
                    return;

                var test = new MbUnitCppTest(testInfoData, repository);

                switch (testInfoData.Kind)
                {
                    case NativeTestKind.Fixture:
                        fixture = test;
                        root.AddChild(fixture);
                        break;
                
                    case NativeTestKind.Test:
                        fixture.AddChild(test);
                        break;

                    case NativeTestKind.Group:
                        group = test;
                        fixture.AddChild(group);
                        break;

                    case NativeTestKind.RowTest:
                        group.AddChild(test);
                        break;
             
                    default:
                        throw new ModelException(String.Format("Unexpected or invalid MbUnitCpp test kind '{0}'.", testInfoData.Kind));
                }
            }

            TestModelSerializer.PublishTestModel(TestModel, MessageSink);
        }
    }
}
