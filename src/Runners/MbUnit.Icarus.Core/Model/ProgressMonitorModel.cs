using System.Collections.Generic;
using MbUnit.Core.Runner;

namespace MbUnit.Icarus.Core.Model
{
    public class ProgressMonitorModel : TextualProgressMonitor
    {
        private readonly List<string> _infoList = new List<string>();
        private string previousTaskName = string.Empty;
        
        public ProgressMonitorModel(List<string> infoList)
        {
            _infoList = infoList;
        }

        public List<string> InfoList
        {
            get { return _infoList; }
        }

        protected override void UpdateDisplay()
        {
            if (previousTaskName.CompareTo(TaskName) != 0)
            {
                previousTaskName = TaskName;
                _infoList.Add(TaskName);
            }
        }
    }
}