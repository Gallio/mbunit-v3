using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Gallio.Framework;
using Gallio.Framework.Formatting;
using Gallio.Reflection;
using Gallio.Runtime;
using MbUnit.Framework;

namespace Gallio.Tests.Integration
{
    /// <summary>
    /// This integration test verifies that <see cref="StubFormatter" /> is
    /// used when the runtime is not initialized.  The trick to doing this is
    /// in running the test code in an isolated AppDomain where the runtime does
    /// not exist.
    /// </summary>
    [TestFixture]
    [TestsOn(typeof(Formatter))]
    [TestsOn(typeof(StubFormatter))]
    public class StubFormatterIntegrationTest
    {
        [Test]
        public void StubFormatterIsUsedWhenRuntimeIsNotInitialized()
        {
            Type remoteCodeType = typeof(RemoteCode);

            AppDomainSetup appDomainSetup = new AppDomainSetup();
            appDomainSetup.ApplicationBase = Path.GetDirectoryName(AssemblyUtils.GetFriendlyAssemblyLocation(remoteCodeType.Assembly));

            AppDomain appDomain = AppDomain.CreateDomain("Test", null, appDomainSetup);
            try
            {
                RemoteCode remoteCode = (RemoteCode)appDomain.CreateInstanceAndUnwrap(remoteCodeType.Assembly.FullName, remoteCodeType.FullName);

                string output = remoteCode.Run();

                Assert.AreEqual("\"abc\"", output);
            }
            finally
            {
                AppDomain.Unload(appDomain);
            }
        }

        public class RemoteCode : MarshalByRefObject
        {
            public string Run()
            {
                StringWriter textWriter = new StringWriter();
                textWriter.NewLine = "\n";
                Console.SetOut(textWriter);

                Assert.IsFalse(RuntimeAccessor.IsInitialized);
                Assert.IsInstanceOfType(typeof(StubFormatter), Formatter.Instance);

                textWriter.Write(Formatter.Instance.Format("abc"));

                return textWriter.ToString();
            }
        }
    }
}
