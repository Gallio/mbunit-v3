// Copyright 2005-2009 Gallio Project - http://www.gallio.org/
// Portions Copyright 2000-2004 Jonathan de Halleux
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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gallio.Common.Markup;
using Gallio.Framework;
using Gallio.Model.Schema;
using Gallio.Common.Reflection;
using Gallio.Runner.Reports.Schema;
using Gallio.Tests;
using MbUnit.Framework;
using MbUnit.TestResources;

namespace MbUnit.Tests.Framework
{
    [Ignore("Can't run this on the build server.")]
    [TestFixture]
    [TestsOn(typeof(AutoEmbedScreenshotAttribute))]
    [RunSample(typeof(ScreenshotSample))]
    public class AutoEmbedScreenshotAttributeTest : BaseTestWithSampleRunner
    {
        [Test]
        [Row("Triggered", true)]
        [Row("NotTriggered", false)]
        public void AutoEmbedScreenshot(string testName, bool triggered)
        {
            TestStepRun run = Runner.GetPrimaryTestStepRun(typeof(ScreenshotSample), testName);

            if (Capture.CanCaptureScreenshot())
            {
                if (triggered)
                {
                    Assert.AreEqual(1, run.TestLog.Attachments.Count);
                    Assert.AreEqual(MimeTypes.Png, run.TestLog.Attachments[0].ContentType);
                }
                else
                {
                    Assert.AreEqual(0, run.TestLog.Attachments.Count);
                }
            }
            else
            {
                if (triggered)
                {
                    Assert.Contains(run.TestLog.ToString(), "Screenshot not available.");
                }
                else
                {
                    Assert.DoesNotContain(run.TestLog.ToString(), "Screenshot not available.");
                }
            }
        }

        [TestFixture, Explicit]
        internal class ScreenshotSample
        {
            [Test]
            [AutoEmbedScreenshot(TriggerEvent.TestPassed, Zoom = 0.25)]
            public void Triggered()
            {
            }

            [Test]
            [AutoEmbedScreenshot(TriggerEvent.TestFailed, Zoom = 0.25)]
            public void NotTriggered()
            {
            }
        }
    }
}
