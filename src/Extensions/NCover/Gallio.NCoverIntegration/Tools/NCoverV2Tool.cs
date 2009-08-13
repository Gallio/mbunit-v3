using System.Collections.Generic;
using Gallio.Common.Concurrency;

namespace Gallio.NCoverIntegration.Tools
{
    public class NCoverV2Tool : NCoverTool
    {
        public static readonly NCoverV2Tool Instance = new NCoverV2Tool();

        public override string Name
        {
            get { return "NCover v2"; }
        }

        public override string GetInstallDir()
        {
            return GetNCoverInstallDirFromRegistry("2.");
        }

        protected override bool RequiresDotNet20
        {
            get { return true; }
        }

        protected override ProcessTask CreateMergeTask(IList<string> sources, string destination)
        {
            return CreateNCoverExplorerConsoleMergeTask("", sources, destination);
        }
    }
}