using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Gallio.Common.Splash.Demo.Properties;

namespace Gallio.Common.Splash.Demo
{
    public partial class DemoForm : Form
    {
        public DemoForm()
        {
            InitializeComponent();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            Style defaultStyle = Style.DefaultStyle;
            StyleBuilder styleBuilder;
            SplashDocument document = splashView.Document;
            
            styleBuilder = new StyleBuilder();
            document.AppendText(styleBuilder.ToStyle(defaultStyle), "Text...\nText 2... ");

            styleBuilder = new StyleBuilder()
            {
                Color = Color.Green,
                Font = new Font(FontFamily.GenericSerif, 16)
            };
            document.AppendText(styleBuilder.ToStyle(defaultStyle), "Text 3... ");

            styleBuilder = new StyleBuilder()
            {
                Color = Color.Gold
            };
            document.AppendText(styleBuilder.ToStyle(defaultStyle), "Text 4...\n");
            document.AppendText(styleBuilder.ToStyle(defaultStyle), "\nMore \tText...\n");

            styleBuilder = new StyleBuilder();
            document.AppendText(styleBuilder.ToStyle(defaultStyle), "Why Hello مرحبا العالمي World?  How are you?");
            document.AppendLine(styleBuilder.ToStyle(defaultStyle));
            document.AppendText(styleBuilder.ToStyle(defaultStyle), "Tab1\tTab2\tTab3\tTab4\tTab5\tTab6\tTab7\tTab8\n");
            document.AppendText(styleBuilder.ToStyle(defaultStyle), "Tab.1\tTab.2\tTab.3\tTab.4\tTab.5\tTab.6\tTab.7\tTab.8\n");

            styleBuilder = new StyleBuilder()
            {
                Font = new Font(defaultStyle.Font, FontStyle.Italic),
                LeftMargin = 30,
                RightMargin = 30,
                FirstLineIndent = 40
            };
            document.AppendText(styleBuilder.ToStyle(defaultStyle), "This paragraph has a first line indent, left margin and right margin.  Lorem ipsum dolor sit amet, consectetur adipisicing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum.\n");

            styleBuilder = new StyleBuilder()
            {
                WordWrap = false,
                Color = Color.Blue,
                Font = new Font(defaultStyle.Font, FontStyle.Bold)
            };
            document.AppendText(styleBuilder.ToStyle(defaultStyle), "Word wrap disabled.  Word wrap disabled.  Word wrap disabled.  Word wrap disabled.  Word wrap disabled.  Word wrap disabled.  Word wrap disabled.  Word wrap disabled.  Word wrap disabled.  Word wrap disabled.  Word wrap disabled.  Word wrap disabled.  Word wrap disabled.\n");

            styleBuilder = new StyleBuilder()
            {
                Color = Color.Red,
                Font = new Font(FontFamily.GenericSerif, 16)
            };
            document.AppendLine(styleBuilder.ToStyle(defaultStyle));
            document.AppendText(styleBuilder.ToStyle(defaultStyle), "القرآن تمتلك المظهر الخارجي وعمق خفي ، وهو المعنى الظاهر والمعنى الباطني. هذا المعنى الباطني بدوره يخفي معنى باطني (هذا العمق تمتلك العمق ، بعد صورة من الكرات السماوية التي هي المغلقة داخل بعضها البعض). غني عن ذلك لمدة سبعة المعاني الباطنية (سبعة من عمق أعماق المخفية).");
            document.AppendLine(styleBuilder.ToStyle(defaultStyle));

            styleBuilder = new StyleBuilder()
            {
                LeftMargin = 40,
                RightMargin = 40
            };
            document.AppendLine(defaultStyle);
            document.AppendText(defaultStyle, "Sample image. ");
            document.AppendObject(defaultStyle, new EmbeddedImage(Resources.SampleImage) { Baseline = 10 });
            document.AppendText(defaultStyle, " (with baseline adjusted by 10 px).");
            document.AppendLine(defaultStyle);

            document.AppendLine(defaultStyle);
            document.AppendText(defaultStyle, "How many lambs did Mary have?");
            document.AppendObject(defaultStyle, new EmbeddedControl(new TextBox()) { Margin = new Padding(3, 3, 3, 3), Baseline = 6 });

            document.AppendLine(defaultStyle);
            document.AppendObject(defaultStyle, new EmbeddedImage(Resources.Passed) { Margin = new Padding(3, 3, 3, 3) });
            document.AppendText(defaultStyle, "Passed 5 ");
            document.AppendObject(defaultStyle, new EmbeddedImage(Resources.Failed) { Margin = new Padding(3, 3, 3, 3) });
            document.AppendText(defaultStyle, "Failed 1");
            document.AppendObject(defaultStyle, new EmbeddedImage(Resources.Ignored) { Margin = new Padding(3, 3, 3, 3) });
            document.AppendText(defaultStyle, "Inconclusive 0");
            document.AppendLine(defaultStyle);

            document.AppendLine(defaultStyle);
            document.AppendText(defaultStyle, "What's the magic word?");
            document.AppendObject(defaultStyle, new EmbeddedControl(new TextBox()) { Margin = new Padding(3, 3, 3, 3), Baseline = 6 });
            document.AppendLine(defaultStyle);

            for (int i = 0; i < 1000; i++)
            {
                document.AppendText(defaultStyle, string.Format("Line #{0}\n", i));
            }
        }

        private void leftToRightButton_Click(object sender, EventArgs e)
        {
            splashView.RightToLeft = RightToLeft.No;
        }

        private void rightToLeftButton_Click(object sender, EventArgs e)
        {
            splashView.RightToLeft = RightToLeft.Yes;
        }

        private void splashView_MouseMove(object sender, MouseEventArgs e)
        {
            SnapPosition snapPosition = splashView.GetSnapPositionAtPoint(splashView.MousePositionToLayout(e.Location));
            snapPositionStatusLabel.Text = string.Format("Snap Position: {0} at {1}.", snapPosition.Kind, snapPosition.CharIndex);
        }

        private void splashView_SelectionChanged(object sender, EventArgs e)
        {
            selectionStatusLabel.Text = string.Format("Selection: {0} - {1}.", splashView.SelectionStart,
                splashView.SelectionStart + splashView.SelectionLength);
        }
    }
}
