using System;
using Gallio.ReSharperRunner.Hosting;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Daemon;
using JetBrains.ReSharper.UnitTestExplorer;

namespace Gallio.ReSharperRunner
{
    /// <summary>
    /// This daemon stage adds support for displaying annotations produced by
    /// the test exploration process.
    /// </summary>
    [DaemonStage(StagesBefore=new Type[] { typeof(UnitTestDaemonStage)})]
    public class GallioDaemonStage : IDaemonStage
    {
        public IDaemonStageProcess CreateProcess(IDaemonProcess process)
        {
            if (!RuntimeProxy.TryInitializeWithPrompt())
                return null;

            return new GallioDaemonStageProcess(process);
        }

        public ErrorStripeRequest NeedsErrorStripe(IProjectFile projectFile)
        {
            return ErrorStripeRequest.STRIPE_AND_ERRORS;
        }
    }
}
