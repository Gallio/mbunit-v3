using MbUnit.Core.Runner;

namespace MbUnit.Tasks.MSBuild
{
    /// <summary>
    /// Implementation of a IProgressMonitor that logs messages to a MSBuildLogger
    /// instance.
    /// </summary>
    public class MSBuildProgressMonitor : TextualProgressMonitor
    {
        private readonly MSBuildLogger logger = null;

        /// <summary>
        /// Initializes a new instance of the MSBuildProgressMonitor class.
        /// </summary>
        /// <param name="logger">A MSBuildLogger instance where log messages will be
        /// channeled to.</param>
        public MSBuildProgressMonitor(MSBuildLogger logger)
        {            
            this.logger = logger;
        }

        private string previousTaskName = string.Empty;

        /// <summary>
        /// 
        /// </summary>
        protected override void UpdateDisplay()
        {
            // We can't show progress in a convenient way when running 
            // within Visual Studio, so just inform when a new task
            // has begun.
            if (previousTaskName.CompareTo(TaskName) != 0)
            {
                previousTaskName = TaskName;
                logger.Info(TaskName);
            }
        }
    }
}
