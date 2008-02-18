// Copyright 2008 MbUnit Project - http://www.mbunit.com/
// Portions Copyright 2000-2004 Jonathan De Halleux, Jamie Cansdale
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System;
using System.IO;
using Gallio.Hosting;
using Gallio.Logging;
using Gallio.Model.Execution;
using MbUnit.Framework;

namespace Gallio.Tests.Integration
{
    /// <summary>
    /// This integration test verifies that <see cref="StubTestContextTracker" /> is
    /// used when the runtime is not initialized.  The trick to doing this is
    /// in running the test code in an isolated AppDomain where the runtime does
    /// not exist.
    /// </summary>
    [TestFixture]
    [TestsOn(typeof(StubTestContext))]
    [TestsOn(typeof(StubTestContextTracker))]
    public class StubContextTest
    {
        [Test]
        public void StubContextIsUsedWhenRuntimeIsNotInitialized()
        {
            Type remoteCodeType = typeof(RemoteCode);

            AppDomainSetup appDomainSetup = new AppDomainSetup();
            appDomainSetup.ApplicationBase = Path.GetDirectoryName(Loader.GetFriendlyAssemblyLocation(remoteCodeType.Assembly));

            AppDomain appDomain = AppDomain.CreateDomain("Test", null, appDomainSetup);
            try
            {
                RemoteCode remoteCode = (RemoteCode)appDomain.CreateInstanceAndUnwrap(remoteCodeType.Assembly.FullName, remoteCodeType.FullName);

                string output = remoteCode.Run();

                Assert.AreEqual("[Attach 'Attachment1': text/plain]\n"
                    + "[Begin Section 'Test Section']\n"
                    + "Foo\n"
                    + "[Embed 'Attachment2': text/plain]\n"
                    + "[Embed 'Attachment1']\n"
                    + "[End Section]\n",
                    output);
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

                Assert.IsFalse(Runtime.IsInitialized);

                Log.AttachPlainText("Attachment1", "Text");
                using (Log.BeginSection("Test Section"))
                {
                    Log.WriteLine("Foo");
                    Log.EmbedPlainText("Attachment2", "Text");
                    Log.EmbedExisting("Attachment1");
                }

                return textWriter.ToString();
            }
        }
    }
}
