using System;
using System.IO;
using System.Reflection;
using System.Text;
using System.Xml;
using Gallio.Icarus.Controllers.Interfaces;
using Gallio.Icarus.Core.Reports;
using Gallio.Model.Logging;
using Gallio.Runner.Events;
using Gallio.Utilities;

namespace Gallio.Icarus.Controllers
{
    class ExecutionLogController : IExecutionLogController
    {
        private readonly ITestController testController;
        private MemoryStream executionLog;
        // TODO: Move this to optionsController
        private string executionLogFolder = Path.Combine(Paths.IcarusAppDataFolder, @"ExecutionLog");

        public event EventHandler<System.EventArgs> ExecutionLogUpdated;

        public Stream ExecutionLog
        {
            get { return executionLog; }
        }

        public string ExecutionLogFolder
        {
            get { return executionLogFolder; }
            set { executionLogFolder = value; }
        }

        public ExecutionLogController(ITestController testController)
        {
            this.testController = testController;

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

            // output css file
            string cssFile = Path.Combine(executionLogFolder, "ExecutionLog.css");
            if (File.Exists(cssFile))
                File.Delete(cssFile);

            using (Stream css = Assembly.GetExecutingAssembly().GetManifestResourceStream("Gallio.Icarus.Reports.ExecutionLog.css"))
            {
                if (css == null)
                    return;

                using (FileStream fs = File.Open(cssFile, FileMode.Create, FileAccess.Write))
                {
                    const int size = 4096;
                    byte[] bytes = new byte[size];
                    int numBytes;
                    while ((numBytes = css.Read(bytes, 0, size)) > 0)
                        fs.Write(bytes, 0, numBytes);
                }
            }
        }

        void UpdateExecutionLog()
        {
            if (testController.Report.TestPackageRun == null)
                return;

            MemoryStream memoryStream = new MemoryStream();
            XmlTextWriter xmlTextWriter = new XmlTextWriter(memoryStream, Encoding.UTF8);
            TestStepReportWriter testStepReportWriter = new TestStepReportWriter(xmlTextWriter, executionLogFolder, testController.Report.TestModel);
            testStepReportWriter.RenderReport(testController.SelectedTests, testController.Report.TestPackageRun.AllTestStepRuns);
            memoryStream.Position = 0;
            executionLog = memoryStream;

            if (ExecutionLogUpdated != null)
                ExecutionLogUpdated(this, System.EventArgs.Empty);
        }
    }
}
