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
using System.Drawing;
using System.Threading;
using Gallio.Common.Media;
using Gallio.Framework;
using MbUnit.Framework;

namespace Gallio.Tests.Framework
{
    [TestFixture]
    [TestsOn(typeof(Capture))]
    public class CaptureTest
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
        public void Screenshot_CapturesScreenshot()
        {
            Size screenSize = Capture.GetScreenSize();

            Bitmap bitmap = Capture.Screenshot();
            TestLog.EmbedImage("Screenshot", bitmap);

            Assert.Multiple(() =>
            {
                Assert.AreEqual(screenSize.Width, bitmap.Width);
                Assert.AreEqual(screenSize.Height, bitmap.Height);
            });
        }

        [Test]
        public void Screenshot_WhenCaptureParametersIsNull_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => Capture.Screenshot(null));
        }

        [Test]
        public void Screenshot_WithCaptureParametersIncludingZoomFactor_CapturesZoomedScreenshot()
        {
            Size screenSize = Capture.GetScreenSize();

            Bitmap bitmap = Capture.Screenshot(new CaptureParameters() { Zoom = 0.25 });
            TestLog.EmbedImage("Screenshot with 0.25x zoom", bitmap);

            Assert.Multiple(() =>
            {
                Assert.AreApproximatelyEqual(screenSize.Width / 2, bitmap.Width, 1);
                Assert.AreApproximatelyEqual(screenSize.Height / 2, bitmap.Height, 1);
            });
        }

        [Test]
        public void StartRecording_CapturesVideo()
        {
            Size screenSize = Capture.GetScreenSize();

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

        [Test]
        public void StartRecording_WhenCaptureParametersIsNull_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => Capture.StartRecording(null, 5.0));
        }

        [Test]
        public void StartRecording_WithCaptureParametersIncludingZoomFactor_CapturesZoomedVideo()
        {
            Size screenSize = Capture.GetScreenSize();

            using (ScreenRecorder recorder = Capture.StartRecording(new CaptureParameters() { Zoom = 0.25 }, 5))
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
    }
}
