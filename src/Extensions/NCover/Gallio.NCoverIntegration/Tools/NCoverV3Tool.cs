using System.Collections.Generic;
using Gallio.Common.Concurrency;

namespace Gallio.NCoverIntegration.Tools
{
    public class NCoverV3Tool : NCoverTool
    {
        public static readonly NCoverV3Tool Instance = new NCoverV3Tool();

        public override string Name
        {
            get { return "NCover v3"; }
        }

        public override string GetInstallDir()
        {
            return GetNCoverInstallDirFromRegistry("3.");
        }

        protected override bool RequiresDotNet20
        {
            get { return true; }
        }

        protected override ProcessTask CreateMergeTask(IList<string> sources, string destination)
        {
            return CreateNCoverReportingMergeTask(sources, destination);
        }
    }
}