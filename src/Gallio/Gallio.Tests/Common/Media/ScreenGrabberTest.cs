using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using Gallio.Common.Markup;
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
                Bitmap bitmap = grabber.CaptureScreenshot(null);
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

        [Test]
        public void CaptureScreenshot_WhenBitmapIsNullAndZoomed_CapturesScaledScreenshotIntoNewBitmap()
        {
            var parameters = new CaptureParameters() { Zoom = 0.25 };
            using (var grabber = new ScreenGrabber(parameters))
            {
                Bitmap bitmap = grabber.CaptureScreenshot(null);
                TestLog.EmbedImage("Screenshot with 0.25x zoom", bitmap);

                Assert.Multiple(() =>
                {
                    Assert.AreApproximatelyEqual(ScreenGrabber.GetScreenSize().Width / 2, grabber.ScreenshotWidth, 1);
                    Assert.AreApproximatelyEqual(ScreenGrabber.GetScreenSize().Height / 2, grabber.ScreenshotHeight, 1);
                    Assert.AreEqual(grabber.ScreenshotWidth, bitmap.Width);
                    Assert.AreEqual(grabber.ScreenshotHeight, bitmap.Height);
                });
            }
        }

        [Test]
        public void CaptureScreenshot_WhenBitmapIsNotNull_CapturesScreenshotIntoProvidedBitmap()
        {
            var parameters = new CaptureParameters() { Zoom = 0.25 };
            using (var grabber = new ScreenGrabber(parameters))
            {
                Bitmap bitmap = new Bitmap(grabber.ScreenshotWidth, grabber.ScreenshotHeight);

                Bitmap returnedBitmap = grabber.CaptureScreenshot(bitmap);
                TestLog.EmbedImage("Screenshot with 0.25x zoom", bitmap);

                Assert.AreSame(bitmap, returnedBitmap);
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
