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
using System.Drawing;
using System.Threading;
using System.Linq;
using Gallio.Common.Markup;
using Gallio.Common.Media;
using Gallio.Common.Reflection;
using Gallio.Framework;
using Gallio.Runner.Reports.Schema;
using MbUnit.Framework;

namespace Gallio.Tests.Framework
{
    [TestFixture]
    [TestsOn(typeof(Capture))]
    [RunSample(typeof(AutoEmbedScreenshotSamples))]
    [RunSample(typeof(AutoEmbedRecordingSamples))]
    public class CaptureTest : BaseTestWithSampleRunner
    {
        [Test]
        public void ScreenSize_ReturnsSensibleResult()
        {
            Size screenSize = Capture.GetScreenSize();

            Assert.Multiple(() =>
            {
                Assert.GreaterThan(screenSize.Width, 0);
                Assert.GreaterThan(screenSize.Height, 0);
            });
        }

        [Test]
        [Row("A caption.")]
        [Row(null)]
        public void Screenshot_CapturesScreenshot(string caption)
        {
            Size screenSize = Capture.GetScreenSize();

            if (Capture.CanCaptureScreenshot())
            {
                if (caption != null)
                    Capture.SetCaption(caption);

                using (Bitmap bitmap = Capture.Screenshot())
                {
                    TestLog.EmbedImage("Screenshot", bitmap);

                    Assert.Multiple(() =>
                    {
                        Assert.AreEqual(screenSize.Width, bitmap.Width);
                        Assert.AreEqual(screenSize.Height, bitmap.Height);
                    });
                }
            }
            else
            {
                Assert.Throws<ScreenshotNotAvailableException>(() => Capture.Screenshot(),
                    "CanCaptureScreenshot returned false so expected an exception to be thrown.");
            }
        }

        [Test]
        public void Screenshot_WhenCaptureParametersIsNull_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => Capture.Screenshot(null));
        }

        [Test]
        [Row("A caption.")]
        [Row(null)]
        public void Screenshot_WithCaptureParametersIncludingZoomFactor_CapturesZoomedScreenshot(string caption)
        {
            Size screenSize = Capture.GetScreenSize();

            if (Capture.CanCaptureScreenshot())
            {
                if (caption != null)
                    Capture.SetCaption(caption);

                using (Bitmap bitmap = Capture.Screenshot(new CaptureParameters() {Zoom = 0.25}))
                {
                    TestLog.EmbedImage("Screenshot with 0.25x zoom", bitmap);

                    Assert.Multiple(() =>
                    {
                        Assert.AreApproximatelyEqual(screenSize.Width / 2, bitmap.Width, 1);
                        Assert.AreApproximatelyEqual(screenSize.Height / 2, bitmap.Height, 1);
                    });
                }
            }
            else
            {
                Assert.Throws<ScreenshotNotAvailableException>(() => Capture.Screenshot(new CaptureParameters() { Zoom = 0.25 }),
                    "CanCaptureScreenshot returned false so expected an exception to be thrown.");
            }
        }

        [Test]
        [Row("A caption.")]
        [Row(null)]
        public void StartRecording_CapturesVideo(string caption)
        {
            Size screenSize = Capture.GetScreenSize();

            if (Capture.CanCaptureScreenshot())
            {
                if (caption != null)
                    Capture.SetCaption(caption);

                using (ScreenRecorder recorder = Capture.StartRecording())
                {
                    Thread.Sleep(2000);
                    recorder.Stop();

                    TestLog.EmbedVideo("Video", recorder.Video);

                    Assert.Multiple(() =>
                    {
                        Assert.AreEqual(screenSize.Width, recorder.Video.Parameters.Width);
                        Assert.AreEqual(screenSize.Height, recorder.Video.Parameters.Height);
                    });
                }
            }
            else
            {
                Assert.Throws<ScreenshotNotAvailableException>(() => Capture.StartRecording(),
                    "CanCaptureScreenshot returned false so expected an exception to be thrown.");
            }
        }

        [Test]
        public void StartRecording_WhenCaptureParametersIsNull_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => Capture.StartRecording(null, 5.0));
        }

        [Test]
        [Row("A caption.")]
        [Row(null)]
        public void StartRecording_WithCaptureParametersIncludingZoomFactor_CapturesZoomedVideo(string caption)
        {
            Size screenSize = Capture.GetScreenSize();

            if (Capture.CanCaptureScreenshot())
            {
                if (caption != null)
                    Capture.SetCaption(caption);

                using (ScreenRecorder recorder = Capture.StartRecording(new CaptureParameters() {Zoom = 0.25}, 5))
                {
                    Thread.Sleep(2000);
                    recorder.Stop();

                    TestLog.EmbedVideo("Video", recorder.Video);

                    Assert.Multiple(() =>
                    {
                        Assert.AreApproximatelyEqual(screenSize.Width / 2, recorder.Video.Parameters.Width, 1);
                        Assert.AreApproximatelyEqual(screenSize.Height / 2, recorder.Video.Parameters.Height, 1);
                    });
                }
            }
            else
            {
                Assert.Throws<ScreenshotNotAvailableException>(() => Capture.StartRecording(new CaptureParameters() { Zoom = 0.25 }, 5),
                    "CanCaptureScreenshot returned false so expected an exception to be thrown.");
            }
        }

        [Test]
        public void AutoEmbedScreenshot_WhenCaptureParametersIsNull_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => Capture.AutoEmbedScreenshot(TriggerEvent.TestFinished, "name", null));
        }

        [Test]
        [Row("Triggered", true)]
        [Row("NotTriggered", false)]
        public void AutoEmbedScreenshot_EmbedsImageWhenTriggered(string testName, bool triggered)
        {
            TestStepRun run = Runner.GetPrimaryTestStepRun(CodeReference.CreateFromMember(typeof(AutoEmbedScreenshotSamples).GetMethod(testName)));

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

        [Test]
        public void AutoEmbedRecording_WhenCaptureParametersIsNull_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => Capture.AutoEmbedRecording(TriggerEvent.TestFinished, "name", null, 5));
        }

        [Test]
        [Row("Triggered", true)]
        [Row("NotTriggered", false)]
        public void AutoEmbedRecording_EmbedsVideoWhenTriggered(string testName, bool triggered)
        {
            TestStepRun run = Runner.GetPrimaryTestStepRun(CodeReference.CreateFromMember(typeof(AutoEmbedRecordingSamples).GetMethod(testName)));

            if (Capture.CanCaptureScreenshot())
            {
                if (triggered)
                {
                    Assert.AreEqual(1, run.TestLog.Attachments.Count);
                    Assert.AreEqual(MimeTypes.FlashVideo, run.TestLog.Attachments[0].ContentType);
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
                    Assert.Contains(run.TestLog.ToString(), "Recording not available.");
                }
                else
                {
                    Assert.DoesNotContain(run.TestLog.ToString(), "Recording not available.");
                }
            }
        }

        [Test]
        public void GetOverlayManager_ReturnsSameOverlayManagerEachTime()
        {
            var first = Capture.GetOverlayManager();
            var second = Capture.GetOverlayManager();

            Assert.Multiple(() =>
            {
                Assert.IsNotNull(first);
                Assert.AreSame(first, second);
            });
        }

        [Test]
        public void GetOverlayManager_WhenContextIsNull_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => Capture.GetOverlayManager(null));
        }

        [Test]
        public void AddOverlay_WhenOverlayIsNull_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => Capture.AddOverlay(null));
        }

        [Test]
        public void AddOverlay_WhenOverlayIsValid_AddsTheOverlayToTheOverlayManager()
        {
            var overlay = new CaptionOverlay();

            Capture.AddOverlay(overlay);

            Assert.Contains(Capture.GetOverlayManager().Overlays, overlay);
        }

        [Test]
        public void SetCaption_WhenTextIsNull_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => Capture.SetCaption(null));
        }

        [Test]
        public void SetCaption_WhenTextIsNotNullAndNotEmpty_AddsACaptionOverlay()
        {
            Capture.SetCaption("A caption.");

            var captionOverlay = Capture.GetCaptionOverlay();

            Assert.Multiple(() =>
            {
                Assert.IsNotNull(captionOverlay);
                Assert.AreEqual("A caption.", captionOverlay.Text);
            });
        }

        [Test]
        public void SetCaption_WhenTextIsNotNullAndNotEmptyAndThereIsAPreviousCaption_ReplacesExistingCaptionOverlay()
        {
            Capture.SetCaption("A caption.");
            Capture.SetCaption("Another caption.");

            var overlays = Capture.GetOverlayManager().Overlays.Where(x => x is CaptionOverlay).ToArray();

            Assert.Multiple(() =>
            {
                Assert.AreEqual(1, overlays.Length);

                var captionOverlay = (CaptionOverlay)overlays[0];
                Assert.AreEqual("Another caption.", captionOverlay.Text);

                Assert.AreSame(captionOverlay, Capture.GetCaptionOverlay());
            });
        }

        [Test]
        public void SetCaption_WhenTextIsEmptyAndThereIsNoPreviousCaption_CaptionRemainsEmpty()
        {
            Capture.SetCaption("");

            var captionOverlay = Capture.GetCaptionOverlay();
            Assert.AreEqual("", captionOverlay.Text);
        }

        [Test]
        public void SetCaption_WhenTextIsEmptyAndThereIsAPreviousCaption_CaptionBecomesEmpty()
        {
            Capture.SetCaption("A caption.");
            Capture.SetCaption("");

            var captionOverlay = Capture.GetCaptionOverlay();
            Assert.AreEqual("", captionOverlay.Text);
        }

        [Test]
        public void SetCaptionFontSize_WhenFontSizeIsLessThan1_Throws()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => Capture.SetCaptionFontSize(0));
            Assert.Throws<ArgumentOutOfRangeException>(() => Capture.SetCaptionFontSize(-1));
        }

        [Test]
        public void SetCaptionFontSize_WhenFontSizeIsValid_SetsTheCaptionFontSize()
        {
            Capture.SetCaptionFontSize(5);

            Assert.AreEqual(5, Capture.GetCaptionOverlay().FontSize);
        }

        [Test]
        public void SetCaptionAlignment_SetsTheCaptionHoriontalAndVerticalAlignment()
        {
            Capture.SetCaptionAlignment(HorizontalAlignment.Right, VerticalAlignment.Middle);

            Assert.Multiple(() =>
            {
                var captionOverlay = Capture.GetCaptionOverlay();
                Assert.AreEqual(HorizontalAlignment.Right, captionOverlay.HorizontalAlignment);
                Assert.AreEqual(VerticalAlignment.Middle, captionOverlay.VerticalAlignment);
            });
        }

        [Explicit("Sample")]
        public class AutoEmbedScreenshotSamples
        {
            [Test]
            public void Triggered()
            {
                Register(TriggerEvent.TestPassed);
            }

            [Test]
            public void NotTriggered()
            {
                Register(TriggerEvent.TestPassed);
                Assert.Fail();
            }

            private static void Register(TriggerEvent triggerEvent)
            {
                Capture.AutoEmbedScreenshot(triggerEvent, null, new CaptureParameters() { Zoom = 0.25 });
            }
        }

        [Explicit("Sample")]
        public class AutoEmbedRecordingSamples
        {
            [Test]
            public void Triggered()
            {
                Register(TriggerEvent.TestPassed);
                Thread.Sleep(1000);
            }

            [Test]
            public void NotTriggered()
            {
                Register(TriggerEvent.TestPassed);
                Thread.Sleep(1000);
                Assert.Fail();
            }

            private static void Register(TriggerEvent triggerEvent)
            {
                Capture.AutoEmbedRecording(triggerEvent, null, new CaptureParameters() { Zoom = 0.25 }, 5);
            }
        }
    }
}
