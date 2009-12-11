using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using MbUnit.Framework;

namespace Gallio.Common.Splash.Tests
{
    public class StyleBuilderTest
    {
        [Test]
        public void DefaultConstructor_InheritsAllProperties()
        {
            var builder = new StyleBuilder();

            Assert.Multiple(() =>
            {
                Assert.IsTrue(builder.Font.Inherited);
                Assert.IsTrue(builder.Color.Inherited);
                Assert.IsTrue(builder.TabStopRuler.Inherited);
                Assert.IsTrue(builder.WordWrap.Inherited);
                Assert.IsTrue(builder.LeftMargin.Inherited);
                Assert.IsTrue(builder.RightMargin.Inherited);
                Assert.IsTrue(builder.FirstLineIndent.Inherited);
            });
        }

        [Test]
        public void StyleCopyConstructor_WhenStyleIsNull_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => new StyleBuilder((Style) null));
        }

        [Test]
        public void StyleCopyConstructor_WhenStyleIsNotNull_CopiesTheStyle()
        {
            var style = Style.DefaultStyle;
            var builder = new StyleBuilder(style);

            Assert.Multiple(() =>
            {
                Assert.AreEqual(style.Font, builder.Font);
                Assert.AreEqual(style.Color, builder.Color);
                Assert.AreEqual(style.TabStopRuler, builder.TabStopRuler);
                Assert.AreEqual(style.WordWrap, builder.WordWrap);
                Assert.AreEqual(style.LeftMargin, builder.LeftMargin);
                Assert.AreEqual(style.RightMargin, builder.RightMargin);
                Assert.AreEqual(style.FirstLineIndent, builder.FirstLineIndent);
            });
        }

        [Test]
        public void StyleBuilderCopyConstructor_WhenStyleIsNull_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => new StyleBuilder((StyleBuilder)null));
        }

        [Test]
        public void StyleBuilderCopyConstructor_WhenStyleIsNotNull_CopiesTheStyle()
        {
            var builder1 = new StyleBuilder(Style.DefaultStyle);
            var builder2 = new StyleBuilder(builder1);

            Assert.AreEqual(builder1, builder2, new StructuralEqualityComparer<StyleBuilder>()
            {
                x => x.Font,
                x => x.Color,
                x => x.TabStopRuler,
                x => x.WordWrap,
                x => x.LeftMargin,
                x => x.RightMargin,
                x => x.FirstLineIndent
            });
        }

        [Test]
        public void Font_SetGet()
        {
            var builder = new StyleBuilder();
            builder.Font = SystemFonts.SmallCaptionFont;
            Assert.AreEqual(SystemFonts.SmallCaptionFont, builder.Font.Value);

            Assert.Throws<ArgumentNullException>(() => builder.Font = null);
        }

        [Test]
        public void Color_SetGet()
        {
            var builder = new StyleBuilder();
            builder.Color = Color.Red;
            Assert.AreEqual(Color.Red, builder.Color.Value);
        }

        [Test]
        public void TabStopRuler_SetGet()
        {
            var builder = new StyleBuilder();
            var ruler = new PixelTabStopRuler(30, 10);
            builder.TabStopRuler = ruler;
            Assert.AreEqual(ruler, builder.TabStopRuler.Value);

            Assert.Throws<ArgumentNullException>(() => builder.TabStopRuler = null);
        }

        [Test]
        public void WordWrap_SetGet()
        {
            var builder = new StyleBuilder();
            builder.WordWrap = false;
            Assert.AreEqual(false, builder.WordWrap.Value);
        }

        [Test]
        public void LeftMargin_SetGet()
        {
            var builder = new StyleBuilder();
            builder.LeftMargin = 10;
            Assert.AreEqual(10, builder.LeftMargin.Value);
        }

        [Test]
        public void RightMargin_SetGet()
        {
            var builder = new StyleBuilder();
            builder.RightMargin = 10;
            Assert.AreEqual(10, builder.RightMargin.Value);
        }

        [Test]
        public void FirstLineIndent_SetGet()
        {
            var builder = new StyleBuilder();
            builder.FirstLineIndent = 10;
            Assert.AreEqual(10, builder.FirstLineIndent.Value);
        }

        [Test]
        public void ToStyle_WhenInheritedStyleIsNull_Throws()
        {
            var builder = new StyleBuilder();
            Assert.Throws<ArgumentNullException>(() => builder.ToStyle(null));
        }

        [Test]
        public void ToStyle_WhenInheritedStyleIsNotNullAndAllPropertiesAreInherited_ReturnsStyleEqualToInherited()
        {
            var builder = new StyleBuilder();
            Assert.AreEqual(Style.DefaultStyle, builder.ToStyle(Style.DefaultStyle));
        }

        [Test]
        public void ToStyleNoArguments_WhenAllPropertiesAreInherited_ReturnsStyleEqualToDefault()
        {
            var builder = new StyleBuilder();
            Assert.AreEqual(Style.DefaultStyle, builder.ToStyle());
        }

        [Test]
        public void ToStyle_WhenInheritedStyleIsNotNullAndNoPropertiesAreInherited_ReturnsStyleGeneratedFromProperties()
        {
            var builder = new StyleBuilder()
            {
                Font = SystemFonts.SmallCaptionFont,
                Color = Color.Red,
                TabStopRuler = new PixelTabStopRuler(30, 10),
                WordWrap = false,
                LeftMargin = 10,
                RightMargin = 20,
                FirstLineIndent = 15
            };

            var style = builder.ToStyle(Style.DefaultStyle);

            Assert.Multiple(() =>
            {
                Assert.AreEqual(SystemFonts.SmallCaptionFont, style.Font);
                Assert.AreEqual(Color.Red, style.Color);
                Assert.AreEqual(new PixelTabStopRuler(30, 10), style.TabStopRuler);
                Assert.AreEqual(false, style.WordWrap);
                Assert.AreEqual(10, style.LeftMargin);
                Assert.AreEqual(20, style.RightMargin);
                Assert.AreEqual(15, style.FirstLineIndent);
            });
        }

        [Test]
        public void ToStyleNoArguments_WhenNoPropertiesAreInherited_ReturnsStyleGeneratedFromProperties()
        {
            var builder = new StyleBuilder()
            {
                Font = SystemFonts.SmallCaptionFont,
                Color = Color.Red,
                TabStopRuler = new PixelTabStopRuler(30, 10),
                WordWrap = false,
                LeftMargin = 10,
                RightMargin = 20,
                FirstLineIndent = 15
            };

            var style = builder.ToStyle();

            Assert.Multiple(() =>
            {
                Assert.AreEqual(SystemFonts.SmallCaptionFont, style.Font);
                Assert.AreEqual(Color.Red, style.Color);
                Assert.AreEqual(new PixelTabStopRuler(30, 10), style.TabStopRuler);
                Assert.AreEqual(false, style.WordWrap);
                Assert.AreEqual(10, style.LeftMargin);
                Assert.AreEqual(20, style.RightMargin);
                Assert.AreEqual(15, style.FirstLineIndent);
            });
        }
    }
}
