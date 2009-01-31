using Gallio.Runtime.ProgressMonitoring;
using Rhino.Mocks;

namespace Gallio.Icarus.Tests
{
    class MockProgressMonitor
    {
        public static IProgressMonitor GetMockProgressMonitor()
        {
            var progressMonitor = MockRepository.GenerateStub<IProgressMonitor>();
            progressMonitor.Stub(x => x.BeginTask(Arg<string>.Is.Anything,
                Arg<double>.Is.Anything)).Return(new ProgressMonitorTaskCookie(progressMonitor));
            progressMonitor.Stub(x => x.CreateSubProgressMonitor(
                Arg<double>.Is.Anything)).Return(progressMonitor).Repeat.Any();
            return progressMonitor;
        }
    }
}
