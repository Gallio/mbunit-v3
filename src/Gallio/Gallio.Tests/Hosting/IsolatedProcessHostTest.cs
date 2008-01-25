using System;
using System.Collections.Generic;
using System.Text;
using Gallio.Hosting;
using MbUnit.Framework;

namespace Gallio.Tests.Hosting
{
    [TestFixture]
    [TestsOn(typeof(IsolatedProcessHostFactory))]
    public class IsolatedProcessHostTest
    {
        [Test, ExpectedArgumentNullException]
        public void CreateHostThrowsIfHostSetupIsNull()
        {
            IsolatedProcessHostFactory factory = new IsolatedProcessHostFactory();
            factory.CreateHost(null);
        }

        [Test]
        public void CreateInstanceFromCreatesAValidObjectHandle()
        {
            IsolatedProcessHostFactory factory = new IsolatedProcessHostFactory();

            HostSetup hostSetup = new HostSetup();
            using (IHost host = factory.CreateHost(hostSetup))
            {
                Type serviceType = typeof (TestService);
                TestService serviceProxy = (TestService) host.CreateInstanceFrom(
                    Loader.GetAssemblyLocalPath(serviceType.Assembly), serviceType.FullName).Unwrap();

                Assert.AreEqual(42, serviceProxy.Add(23, 19));
            }
        }

        public class TestService : MarshalByRefObject
        {
            public int Add(int x, int y)
            {
                return x + y;
            }
        }
    }
}
