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
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using Gallio.Icarus.Controllers.Interfaces;
using Gallio.Icarus.Models;
using Gallio.Icarus.Reports;
using Gallio.Model.Logging;
using Gallio.Runner.Events;
using Gallio.Runtime;
using Gallio.Utilities;

namespace Gallio.Icarus.Controllers
{
    class ExecutionLogController : IExecutionLogController
    {
        private readonly ITestController testController;
        private MemoryStream executionLog;
        private readonly string executionLogFolder;
        private readonly TaskManager taskManager = new TaskManager();

        public event EventHandler<System.EventArgs> ExecutionLogUpdated;

        public Stream ExecutionLog
        {
            get { return executionLog; }
        }

        public ExecutionLogController(ITestController testController, string executionLogFolder)
        {
            this.testController = testController;
            this.executionLogFolder = executionLogFolder;

            testController.SelectedTests.ListChanged += delegate { UpdateExecutionLog(); };
            testController.TestStepFinished += testController_TestStepFinished;

            SetupExecutionLog();
        }

        void testController_TestStepFinished(object sender, TestStepFinishedEventArgs e)
        {
            // store attachments as we go along
            string attachmentDirectory = string.Empty;
            if (e.TestStepRun.TestLog.Attachments.Count > 0)
            {
                attachmentDirectory = Path.Combine(executionLogFolder, FileUtils.EncodeFileName(e.TestStepRun.Step.Id));
                if (!Directory.Exists(attachmentDirectory))
                    Directory.CreateDirectory(attachmentDirectory);
            }

            foreach (AttachmentData attachmentData in e.TestStepRun.TestLog.Attachments)
            {
                string fileName = Path.Combine(attachmentDirectory, FileUtils.EncodeFileName(attachmentData.Name));
                using (FileStream fs = File.Open(fileName, FileMode.Create, FileAccess.Write))
                    attachmentData.SaveContents(fs, Encoding.Default);
            }

            UpdateExecutionLog();
        }

        void SetupExecutionLog()
        {
            if (!Directory.Exists(executionLogFolder))
                Directory.CreateDirectory(executionLogFolder);

            // clear old attachments
            DirectoryInfo di = new DirectoryInfo(executionLogFolder);
            foreach (DirectoryInfo attachmentFolder in di.GetDirectories())
                attachmentFolder.Delete(true);

            // copy resources
            string contentLocalPath = RuntimeAccessor.Instance.MapUriToLocalPath(new Uri("plugin://Gallio.Reports/"));
            foreach (string resourcePath in new[] { "css", "img" })
            {
                string sourceContentPath = Path.Combine(contentLocalPath, resourcePath);
                string destContentPath = Path.GetFullPath(Path.Combine(executionLogFolder, resourcePath));
                if (!Directory.Exists(destContentPath))
                    Directory.CreateDirectory(destContentPath);
                FileUtils.CopyAllIndirect(sourceContentPath, destContentPath, null,
                    delegate(string sourceFilePath, string destFilePath)
                    {
                        using (Stream sourceStream = File.OpenRead(sourceFilePath))
                        using (Stream destStream = File.Open(destFilePath, FileMode.CreateNew, FileAccess.Write))
                        {
                            FileUtils.CopyStreamContents(sourceStream, destStream);
                        }
                    });
            }
        }

        void UpdateExecutionLog()
        {
            taskManager.StartTask(delegate
            {
                if (testController.Report.TestPackageRun == null)
                    executionLog = null;
                else
                {
                    MemoryStream memoryStream = new MemoryStream();
                    XmlTextWriter xmlTextWriter = new XmlTextWriter(memoryStream, Encoding.UTF8);
                    TestStepReportWriter testStepReportWriter = new TestStepReportWriter(xmlTextWriter,
                        executionLogFolder,
                        testController.Report.TestModel);
                    List<string> testIds = new List<string>();
                    foreach (TestTreeNode testTreeNode in testController.SelectedTests)
                        testIds.Add(testTreeNode.Name);
                    if (testIds.Count == 0 && testController.Model.Root != null)
                        testIds.Add(testController.Model.Root.Name);
                    testStepReportWriter.RenderReport(testIds, testController.Report.TestPackageRun);
                    memoryStream.Position = 0;
                    executionLog = memoryStream;
                }
                EventHandlerUtils.SafeInvoke(ExecutionLogUpdated, this, System.EventArgs.Empty);
            });
        }
    }
}
