using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MbUnit.Framework;
using MbUnit.Framework.ContractVerifiers;

namespace Gallio.Common.Splash.Tests
{
    public class PixelTabStopRulerTest
    {
        [Test]
        public void Constructor_WhenPixelsPerTabStopIsNegative_Throws()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new PixelTabStopRuler(-1, 10));
        }

        [Test]
        public void Constructor_WhenMinimumTabWidthIsNegative_Throws()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new PixelTabStopRuler(10, -1));
        }

        [Test]
        [Row(60, 10)]
        [Row(60, 0)]
        [Row(0, 10)]
        public void Constructor_WhenArgumentsValid_SetsProperties(int pixelsPerTabStop, int minimumTabWidth)
        {
            var ruler = new PixelTabStopRuler(pixelsPerTabStop, minimumTabWidth);
            Assert.AreEqual(pixelsPerTabStop, ruler.PixelsPerTabStop);
            Assert.AreEqual(minimumTabWidth, ruler.MinimumTabWidth);
        }

        [Test]
        public void AdvanceToNextTabStop_WhenPixelsPerTabStopIsZero_AdvancesMinimumWidth()
        {
            var ruler = new PixelTabStopRuler(0, 10);
            Assert.AreEqual(52, ruler.AdvanceToNextTabStop(42));
        }

        [Test]
        [Row(true)]
        [Row(false)]
        public void AdvanceToNextTabStop_WhenPixelsPerTabStopIsPositiveAndAtTabStop_AdvancesWholeWidth(bool negativeX)
        {
            int xShift = negativeX ? -120 : 0;
            var ruler = new PixelTabStopRuler(30, 10);
            Assert.AreEqual(xShift + 60, ruler.AdvanceToNextTabStop(xShift + 30));
        }

        [Test]
        [Row(true)]
        [Row(false)]
        public void AdvanceToNextTabStop_WhenPixelsPerTabStopIsPositiveAndPastTabStopAndMoreThanMinimumWidthAway_AdvancesOneTabStop(bool negativeX)
        {
            int xShift = negativeX ? -120 : 0;
            var ruler = new PixelTabStopRuler(30, 10);
            Assert.AreEqual(xShift + 60, ruler.AdvanceToNextTabStop(xShift + 35));
        }

        [Test]
        [Row(true)]
        [Row(false)]
        public void AdvanceToNextTabStop_WhenPixelsPerTabStopIsPositiveAndPastTabStopAndExactlyMinimumWidthAway_AdvancesOneTabStop(bool negativeX)
        {
            int xShift = negativeX ? -120 : 0;
            var ruler = new PixelTabStopRuler(30, 10);
            Assert.AreEqual(xShift + 60, ruler.AdvanceToNextTabStop(xShift + 50));
        }

        [Test]
        [Row(true)]
        [Row(false)]
        public void AdvanceToNextTabStop_WhenPixelsPerTabStopIsPositiveAndPastTabStopButLessThanMinimumWidthAway_AdvancesAnExtraTabStop(bool negativeX)
        {
            int xShift = negativeX ? -120 : 0;
            var ruler = new PixelTabStopRuler(30, 10);
            Assert.AreEqual(xShift + 90, ruler.AdvanceToNextTabStop(xShift + 55));
        }

        [VerifyContract]
        public readonly IContract EqualityAndHashCode = new EqualityContract<PixelTabStopRuler>()
        {
            ImplementsOperatorOverloads = false,
            EquivalenceClasses =
            {
                { new PixelTabStopRuler(0, 10), new PixelTabStopRuler(0, 10) },
                { new PixelTabStopRuler(30, 0), new PixelTabStopRuler(30, 0) },
                { new PixelTabStopRuler(30, 10), new PixelTabStopRuler(30, 10) }
            }
        };
    }
}
