using System;
using Gallio.AutoCAD;
using Gallio.Tests;
using MbUnit.Framework;
using Rhino.Mocks;

namespace Gallio.AutoCAD.Tests
{
    [TestsOn(typeof(AcadTestDriver))]
    public class AcadTestDriverTest : BaseTestWithMocks
    {
        [Test, ExpectedArgumentNullException]
        public void ConstructorThrowsExceptionWhenAcadProcessArgumentIsNull()
        {
            new AcadTestDriver(null, Mocks.Stub<IRemoteTestDriver>());
        }

        [Test, ExpectedArgumentNullException]
        public void ConstructorThrowsExceptionWhenRemoteTestDriverArgumentIsNull()
        {
            new AcadTestDriver(Mocks.Stub<IAcadProcess>(), null);
        }

        [Test]
        public void DisposingTestDriverDisposesAcadProcess()
        {
            IAcadProcess process = Mocks.StrictMock<IAcadProcess>();
            IRemoteTestDriver testDriver = Mocks.Stub<IRemoteTestDriver>();

            using (Mocks.Record())
            {
                Expect.Call(process.Dispose);
            }

            using (Mocks.Playback())
            {
                new AcadTestDriver(process, testDriver).Dispose();
            }
        }
    }
}
