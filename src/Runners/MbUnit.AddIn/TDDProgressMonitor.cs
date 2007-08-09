using MbUnit.Core.Runner;

namespace MbUnit.AddIn
{
    internal class TDDProgressMonitor : TextualProgressMonitor
    {
        private readonly TDDLogger tddLogger = null;

        public TDDProgressMonitor(TDDLogger logger)
        {            
            tddLogger = logger;
        }

        private string previousTaskName = string.Empty;
        protected override void UpdateDisplay()
        {
            // We can't show progress in a convenient way when running 
            // within Visual Studio, so just inform when a new task
            // has begun.
            if (previousTaskName.CompareTo(TaskName) != 0)
            {
                previousTaskName = TaskName;
                tddLogger.Info(TaskName);
            }
        }
    }
}
