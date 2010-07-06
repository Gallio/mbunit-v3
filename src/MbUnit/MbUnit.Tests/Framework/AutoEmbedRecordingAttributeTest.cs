// Copyright 2005-2010 Gallio Project - http://www.gallio.org/
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
using System.Threading;
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
    [TestFixture]
    [TestsOn(typeof(AutoEmbedRecordingAttribute))]
    [RunSample(typeof(RecordingSample))]
    public class AutoEmbedRecordingAttributeTest : BaseTestWithSampleRunner
    {
        [Test]
        [Row("Triggered", true)]
        [Row("NotTriggered", false)]
        public void AutoEmbedScreenshot(string testName, bool triggered)
        {
            TestStepRun run = Runner.GetPrimaryTestStepRun(typeof(RecordingSample), testName);

            if (Capture.CanCaptureScreenshot())
            {
                if (triggered)
                {
                    Assert.Count(1, run.TestLog.Attachments);
                    Assert.AreEqual(MimeTypes.FlashVideo, run.TestLog.Attachments[0].ContentType);
                }
                else
                {
                    Assert.Count(0, run.TestLog.Attachments);
                }
            }
            else
            {
                if (triggered)
                {
                    Assert.Contains(run.TestLog.ToString(), "Recording not available.");
                }
                else
                {
                    Assert.DoesNotContain(run.TestLog.ToString(), "Recording not available.");
                }
            }
        }

        [TestFixture, Explicit]
        internal class RecordingSample
        {
            [Test]
            [AutoEmbedRecording(TriggerEvent.TestPassed, Zoom = 0.25, FramesPerSecond = 5)]
            public void Triggered()
            {
                Thread.Sleep(500);
            }

            [Test]
            [AutoEmbedRecording(TriggerEvent.TestFailed, Zoom = 0.25, FramesPerSecond = 3)]
            public void NotTriggered()
            {
            }
        }
    }
}
