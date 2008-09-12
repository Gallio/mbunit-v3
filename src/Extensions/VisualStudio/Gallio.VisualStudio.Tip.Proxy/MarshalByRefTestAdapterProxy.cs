using System;
using Microsoft.VisualStudio.TestTools.Common;
using Microsoft.VisualStudio.TestTools.Execution;
using Microsoft.VisualStudio.TestTools.TestAdapter;

namespace Gallio.VisualStudio.Tip
{
    /// <summary>
    /// Marshal-by-ref base <see cref="ITestAdapter" /> proxy implementation.
    /// </summary>
    public class MarshalByRefTestAdapterProxy : MarshalByRefObject, ITestAdapter
    {
        private readonly ITestAdapter target;

        public MarshalByRefTestAdapterProxy(ITestAdapter target)
        {
            this.target = target;
        }

        public void Run(ITestElement testElement, ITestContext testContext)
        {
            target.Run(testElement, testContext);
        }

        public void Cleanup()
        {
            target.Cleanup();
        }

        public void StopTestRun()
        {
            target.StopTestRun();
        }

        public void AbortTestRun()
        {
            target.AbortTestRun();
        }

        public void PauseTestRun()
        {
            target.PauseTestRun();
        }

        public void ResumeTestRun()
        {
            target.ResumeTestRun();
        }

        public void Initialize(IRunContext runContext)
        {
            target.Initialize(runContext);
        }

        public void ReceiveMessage(object obj)
        {
            target.ReceiveMessage(obj);
        }

        public void PreTestRunFinished(IRunContext runContext)
        {
            target.PreTestRunFinished(runContext);
        }

        public override object InitializeLifetimeService()
        {
            return null;
        }
    }
}
