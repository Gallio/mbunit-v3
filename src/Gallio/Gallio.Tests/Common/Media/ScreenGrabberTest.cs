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
using Gallio.Common.Media;
using Gallio.Framework;
using MbUnit.Framework;

namespace Gallio.Tests.Common.Media
{
    [TestsOn(typeof(ScreenGrabber))]
    public class ScreenGrabberTest
    {
        [Test]
        public void Constructor_WhenCaptureParametersIsNull_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => new ScreenGrabber(null));
        }

        [Test]
        public void ScreenSize_ReturnsSensibleResult()
        {
            Size screenSize = ScreenGrabber.GetScreenSize();

            Assert.Multiple(() =>
            {
                Assert.GreaterThan(screenSize.Width, 0);
                Assert.GreaterThan(screenSize.Height, 0);
            });
        }

        [Test]
        public void Parameters_ReturnsParameters()
        {
            var parameters = new CaptureParameters();
            using (var grabber = new ScreenGrabber(parameters))
            {
                Assert.AreSame(parameters, grabber.Parameters);
            }
        }

        [Test]
        public void Parameters_WhenDisposed_Throws()
        {
            var parameters = new CaptureParameters();
            var grabber = new ScreenGrabber(parameters);
            grabber.Dispose();

            CaptureParameters x;
            Assert.Throws<ObjectDisposedException>(() => x = grabber.Parameters);
        }

        [Test]
        public void ScreenshotWidth_ReturnsScaledWidth()
        {
            var parameters = new CaptureParameters() { Zoom = 0.25 };
            using (var grabber = new ScreenGrabber(parameters))
            {
                Assert.AreApproximatelyEqual(ScreenGrabber.GetScreenSize().Width / 2, grabber.ScreenshotWidth, 1);
            }
        }

        [Test]
        public void ScreenshotWidth_WhenDisposed_Throws()
        {
            var parameters = new CaptureParameters();
            var grabber = new ScreenGrabber(parameters);
            grabber.Dispose();

            int x;
            Assert.Throws<ObjectDisposedException>(() => x = grabber.ScreenshotWidth);
        }

        [Test]
        public void ScreenshotHeight_ReturnsScaledHeight()
        {
            var parameters = new CaptureParameters() { Zoom = 0.25 };
            using (var grabber = new ScreenGrabber(parameters))
            {
                Assert.AreApproximatelyEqual(ScreenGrabber.GetScreenSize().Height / 2, grabber.ScreenshotHeight, 1);
            }
        }

        [Test]
        public void ScreenshotHeight_WhenDisposed_Throws()
        {
            var parameters = new CaptureParameters();
            var grabber = new ScreenGrabber(parameters);
            grabber.Dispose();

            int x;
            Assert.Throws<ObjectDisposedException>(() => x = grabber.ScreenshotHeight);
        }

        [Test]
        public void CaptureScreenshot_WhenBitmapIsNullAndNotZoomed_CapturesUnscaledScreenshotIntoNewBitmap()
        {
            var parameters = new CaptureParameters();
            using (var grabber = new ScreenGrabber(parameters))
            {
                if (ScreenGrabber.CanCaptureScreenshot())
                {
                    using (Bitmap bitmap = grabber.CaptureScreenshot(null))
                    {
                        TestLog.EmbedImage("Screenshot", bitmap);

                        Assert.Multiple(() =>
                        {
                            Assert.AreEqual(ScreenGrabber.GetScreenSize().Width, grabber.ScreenshotWidth);
                            Assert.AreEqual(ScreenGrabber.GetScreenSize().Height, grabber.ScreenshotHeight);
                            Assert.AreEqual(grabber.ScreenshotWidth, bitmap.Width);
                            Assert.AreEqual(grabber.ScreenshotHeight, bitmap.Height);
                        });
                    }
                }
                else
                {
                    Assert.Throws<ScreenshotNotAvailableException>(() => grabber.CaptureScreenshot(null),
                        "CanCaptureScreenshot returned false so expected an exception to be thrown.");
                }
            }
        }

        [Test]
        public void CaptureScreenshot_WhenBitmapIsNullAndZoomed_CapturesScaledScreenshotIntoNewBitmap()
        {
            var parameters = new CaptureParameters() { Zoom = 0.25 };
            using (var grabber = new ScreenGrabber(parameters))
            {
                if (ScreenGrabber.CanCaptureScreenshot())
                {
                    using (Bitmap bitmap = grabber.CaptureScreenshot(null))
                    {
                        TestLog.EmbedImage("Screenshot with 0.25x zoom", bitmap);

                        Assert.Multiple(() =>
                        {
                            Assert.AreApproximatelyEqual(ScreenGrabber.GetScreenSize().Width / 2,
                                grabber.ScreenshotWidth, 1);
                            Assert.AreApproximatelyEqual(ScreenGrabber.GetScreenSize().Height / 2,
                                grabber.ScreenshotHeight, 1);
                            Assert.AreEqual(grabber.ScreenshotWidth, bitmap.Width);
                            Assert.AreEqual(grabber.ScreenshotHeight, bitmap.Height);
                        });
                    }
                }
                else
                {
                    Assert.Throws<ScreenshotNotAvailableException>(() => grabber.CaptureScreenshot(null),
                        "CanCaptureScreenshot returned false so expected an exception to be thrown.");
                }
            }
        }

        [Test]
        public void CaptureScreenshot_WhenBitmapIsNotNull_CapturesScreenshotIntoProvidedBitmap()
        {
            var parameters = new CaptureParameters() { Zoom = 0.25 };
            using (var grabber = new ScreenGrabber(parameters))
            {
                using (Bitmap bitmap = new Bitmap(grabber.ScreenshotWidth, grabber.ScreenshotHeight))
                {
                    if (ScreenGrabber.CanCaptureScreenshot())
                    {
                        Bitmap returnedBitmap = grabber.CaptureScreenshot(bitmap);
                        TestLog.EmbedImage("Screenshot with 0.25x zoom", bitmap);

                        Assert.AreSame(bitmap, returnedBitmap);
                    }
                    else
                    {
                        Assert.Throws<ScreenshotNotAvailableException>(() => grabber.CaptureScreenshot(bitmap),
                            "CanCaptureScreenshot returned false so expected an exception to be thrown.");
                    }
                }
            }
        }

        [Test]
        public void CaptureScreenshot_WhenBitmapIsNotTheRightSize_Throws()
        {
            var parameters = new CaptureParameters() { Zoom = 0.25 };
            using (var grabber = new ScreenGrabber(parameters))
            {
                var ex = Assert.Throws<ArgumentException>(() => grabber.CaptureScreenshot(new Bitmap(1, grabber.ScreenshotHeight)));
                Assert.Contains(ex.Message, "The bitmap dimensions must exactly match the screenshot dimensions.");

                ex = Assert.Throws<ArgumentException>(() => grabber.CaptureScreenshot(new Bitmap(grabber.ScreenshotWidth, 1)));
                Assert.Contains(ex.Message, "The bitmap dimensions must exactly match the screenshot dimensions.");
            }
        }

        [Test]
        public void CaptureScreenshot_WhenDisposed_Throws()
        {
            var parameters = new CaptureParameters();
            var grabber = new ScreenGrabber(parameters);
            grabber.Dispose();

            Assert.Throws<ObjectDisposedException>(() => grabber.CaptureScreenshot(null));
        }
    }
}
