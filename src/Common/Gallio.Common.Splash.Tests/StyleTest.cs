using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using MbUnit.Framework;
using MbUnit.Framework.ContractVerifiers;

namespace Gallio.Common.Splash.Tests
{
    public class StyleTest
    {
        [Test]
        public void CreateDefaultStyle_ReturnsStyleGeneratedFromSystemProperties()
        {
            Style style = Style.DefaultStyle;

            Assert.Multiple(() =>
            {
                Assert.AreEqual(SystemFonts.DefaultFont, style.Font);
                Assert.AreEqual(SystemColors.WindowText, style.Color);
                Assert.AreEqual(60, ((PixelTabStopRuler)style.TabStopRuler).PixelsPerTabStop);
                Assert.AreEqual(10, ((PixelTabStopRuler)style.TabStopRuler).MinimumTabWidth);
                Assert.IsTrue(style.WordWrap);
                Assert.AreEqual(0, style.LeftMargin);
                Assert.AreEqual(0, style.RightMargin);
                Assert.AreEqual(0, style.FirstLineIndent);
            });
        }

        [VerifyContract]
        public readonly IContract EqualityAndHashCode = new EqualityContract<Style>()
        {
            ImplementsOperatorOverloads = false,
            EquivalenceClasses =
            {
                { Style.DefaultStyle, new StyleBuilder().ToStyle() },
                { new StyleBuilder() { Color = Color.Red }.ToStyle(), new StyleBuilder() { Color = Color.Red }.ToStyle() },
                { new StyleBuilder() { Font = SystemFonts.SmallCaptionFont }.ToStyle(), new StyleBuilder() { Font = SystemFonts.SmallCaptionFont }.ToStyle() },
                { new StyleBuilder() { TabStopRuler = new PixelTabStopRuler(30, 10) }.ToStyle(), new StyleBuilder() { TabStopRuler = new PixelTabStopRuler(30, 10) }.ToStyle() },
                { new StyleBuilder() { WordWrap = false }.ToStyle(), new StyleBuilder() { WordWrap = false }.ToStyle() },
                { new StyleBuilder() { LeftMargin = 5 }.ToStyle(), new StyleBuilder() { LeftMargin = 5 }.ToStyle() },
                { new StyleBuilder() { RightMargin = 10 }.ToStyle(), new StyleBuilder() { RightMargin = 10 }.ToStyle() },
                { new StyleBuilder() { FirstLineIndent = 15 }.ToStyle(), new StyleBuilder() { FirstLineIndent = 15 }.ToStyle() }
            }
        };
    }
}
