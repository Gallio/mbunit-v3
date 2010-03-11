using Gallio.Runtime.ProgressMonitoring;
using Gallio.UI.ProgressMonitoring;
using MbUnit.Framework;

namespace Gallio.UI.Tests.ProgressMonitoring
{
    [TestsOn(typeof(ToolStripStatusLabel))]
    public class ToolStripStatusLabelTest
    {
        private ToolStripStatusLabel toolStripStatusLabel;
        private ObservableProgressMonitor progressMonitor;

        [SetUp]
        public void SetUp()
        {
            toolStripStatusLabel = new ToolStripStatusLabel();
            progressMonitor = new ObservableProgressMonitor();
        }

        [Test]
        public void Text_should_start_with_task_name()
        {
            const string taskName = "blahblahblah";
            progressMonitor.BeginTask(taskName, 5);

            toolStripStatusLabel.ProgressChanged(progressMonitor);

            Assert.IsTrue(toolStripStatusLabel.Text.StartsWith(taskName));
        }

        [Test]
        public void Text_should_contain_with_sub_task_name()
        {
            const string subTaskName = "blahblahblah";
            progressMonitor.BeginTask("jfdkhfkj", 5);
            var subProgressMonitor = progressMonitor.CreateSubProgressMonitor(1);
            subProgressMonitor.BeginTask(subTaskName, 5);

            toolStripStatusLabel.ProgressChanged(progressMonitor);

            Assert.IsTrue(toolStripStatusLabel.Text.Contains(subTaskName));
        }

        [Test]
        public void Text_should_contain_progress_as_a_percentage()
        {
            progressMonitor.BeginTask("jfdkhfkj", 5);
            progressMonitor.Worked(1);

            toolStripStatusLabel.ProgressChanged(progressMonitor);

            Assert.IsTrue(toolStripStatusLabel.Text.Contains("20 %"), 
                "Expected text ({0}) to contain \"20 %\"", toolStripStatusLabel.Text);
        }

        [Test]
        public void Text_should_be_empty_if_work_is_done()
        {
            using (progressMonitor.BeginTask("jfdkhfkj", 1)) { }

            toolStripStatusLabel.ProgressChanged(progressMonitor);

            Assert.AreEqual("", toolStripStatusLabel.Text);
        }
    }
}
