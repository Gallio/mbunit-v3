using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using Gallio.Common.Media;
using MbUnit.Framework;

namespace Gallio.Tests.Common.Media
{
    [TestsOn(typeof(OverlayManager))]
    public class OverlayManagerTest
    {
        [Test]
        public void AddOverlay_WhenOverlayIsNull_Throws()
        {
            var overlayManager = new OverlayManager();

            Assert.Throws<ArgumentNullException>(() => overlayManager.AddOverlay(null));
        }

        [Test]
        public void AddOverlay_WhenOverlayIsNotNull_AddsTheOverlay()
        {
            var overlayManager = new OverlayManager();
            var overlay = new StubOverlay();

            overlayManager.AddOverlay(overlay);

            Assert.AreElementsEqual(new[] { overlay }, overlayManager.Overlays);
        }

        [Test]
        public void RemoveOverlay_WhenOverlayIsNull_Throws()
        {
            var overlayManager = new OverlayManager();

            Assert.Throws<ArgumentNullException>(() => overlayManager.RemoveOverlay(null));
        }

        [Test]
        public void AddOverlay_WhenOverlayIsNotNullAndIsNotPresent_DoesNothing()
        {
            var overlayManager = new OverlayManager();
            var overlay = new StubOverlay();

            overlayManager.RemoveOverlay(overlay);

            Assert.AreEqual(0, overlayManager.Overlays.Count);
        }

        [Test]
        public void AddOverlay_WhenOverlayIsNotNullAndIsPresent_RemovesTheOverlay()
        {
            var overlayManager = new OverlayManager();
            var overlay = new StubOverlay();
            overlayManager.AddOverlay(overlay);

            overlayManager.RemoveOverlay(overlay);

            Assert.AreEqual(0, overlayManager.Overlays.Count);
        }

        [Test]
        public void PaintOverlays_WhenRequestIsNull_Throws()
        {
            var overlayManager = new OverlayManager();

            Assert.Throws<ArgumentNullException>(() => overlayManager.PaintOverlays(null));
        }

        [Test]
        public void PaintOverlays_PaintsAllOverlaysAndRestoresGraphicsContextForEachOne()
        {
            var overlayManager = new OverlayManager();
            var overlay1 = new OverlayThatChangesInterpolationMode();
            var overlay2 = new OverlayThatChangesInterpolationMode();
            overlayManager.AddOverlay(overlay1);
            overlayManager.AddOverlay(overlay2);

            using (Bitmap bitmap = new Bitmap(32, 32))
            {
                using (Graphics graphics = Graphics.FromImage(bitmap))
                {
                    graphics.InterpolationMode = InterpolationMode.HighQualityBilinear;
                    var request = new OverlayPaintRequest(graphics, new Rectangle(0, 0, 32, 32), 0, 0);

                    overlayManager.PaintOverlays(request);

                    Assert.Multiple(() =>
                    {
                        Assert.AreEqual(InterpolationMode.HighQualityBilinear, graphics.InterpolationMode);

                        Assert.IsTrue(overlay1.WasPainted);
                        Assert.AreEqual(InterpolationMode.HighQualityBilinear, overlay1.OldInterpolationMode);

                        Assert.IsTrue(overlay2.WasPainted);
                        Assert.AreEqual(InterpolationMode.HighQualityBilinear, overlay2.OldInterpolationMode);
                    });
                }
            }
        }

        [Test]
        public void PaintOverlaysOnImage_WhenImageIsNull_Throws()
        {
            var overlayManager = new OverlayManager();

            Assert.Throws<ArgumentNullException>(() => overlayManager.PaintOverlaysOnImage(null, 0, 0));
        }

        [Test]
        public void ToOverlay_ReturnsACompositeOverlay()
        {
            var overlayManager = new OverlayManager();
            var overlay = new OverlayThatChangesInterpolationMode();

            Overlay compositeOverlay = overlayManager.ToOverlay();

            var compositeOverlayManager = new OverlayManager();
            compositeOverlayManager.AddOverlay(compositeOverlay);
            using (Bitmap bitmap = new Bitmap(32, 32))
                compositeOverlayManager.PaintOverlaysOnImage(bitmap, 0, 0);
            Assert.IsTrue(overlay.WasPainted);
        }

        private class StubOverlay : Overlay
        {
            protected override void PaintImpl(OverlayPaintRequest request)
            {
                throw new NotImplementedException();
            }
        }

        private class OverlayThatChangesInterpolationMode : Overlay
        {
            public bool WasPainted { get; private set; }

            public InterpolationMode OldInterpolationMode { get; private set; }

            protected override void PaintImpl(OverlayPaintRequest request)
            {
                OldInterpolationMode = request.Graphics.InterpolationMode;
                WasPainted = true;

                request.Graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
            }
        }
    }
}
