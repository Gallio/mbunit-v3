using Gallio.Tests;
using MbUnit.Framework;
using Rhino.Mocks;

namespace Gallio.AutoCAD.Tests
{
    [TestsOn(typeof(AcadTestDriverFactory))]
    public class AcadTestDriverFactoryTest : BaseTestWithMocks
    {
        [Test, ExpectedArgumentNullException]
        public void ConstructorThrowsExceptionWhenAcadProcessFactoryArgumentIsNull()
        {
            new AcadTestDriverFactory(null);
        }

        [Test]
        public void CanCreateAcadTestDriver()
        {
            IAcadProcessFactory processFactory = Mocks.StrictMock<IAcadProcessFactory>();
            IAcadProcess process = Mocks.StrictMock<IAcadProcess>();
            using (Mocks.Record())
            {
                Expect.Call(processFactory.CreateProcess()).Return(process);
                Expect.Call(process.GetRemoteTestDriver()).Return(Mocks.Stub<IRemoteTestDriver>());
            }

            using (Mocks.Playback())
            {
                AcadTestDriverFactory driverFactory = new AcadTestDriverFactory(processFactory);
                Assert.IsNotNull(driverFactory.CreateTestDriver());
            }
        }
    }
}
