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
        private static readonly Key<string> HRefKey = new Key<string>("href");

        public DemoForm()
        {
            InitializeComponent();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            SplashDocument document = splashView.Document;
            
            document.AppendText("Text...\nText 2... ");

            using (document.BeginStyle(new StyleBuilder()
            {
                Color = Color.Green,
                Font = new Font(FontFamily.GenericSerif, 16)
            }.ToStyle()))
            {
                document.AppendText("Text 3... ");

                using (document.BeginStyle(new StyleBuilder()
                {
                    Color = Color.Gold
                }.ToStyle(document.CurrentStyle)))
                {
                    document.AppendText("Text 4...\n");
                }

                document.AppendText("\nMore \tText...\n");
            }

            document.AppendText("Why Hello مرحبا العالمي World?  How are you?");
            document.AppendLine();
            document.AppendText("Tab1\tTab2\tTab3\tTab4\tTab5\tTab6\tTab7\tTab8\n");
            document.AppendText("Tab.1\tTab.2\tTab.3\tTab.4\tTab.5\tTab.6\tTab.7\tTab.8\n");

            using (document.BeginStyle(new StyleBuilder()
            {
                Font = new Font(Style.Default.Font, FontStyle.Underline),
                Color = Color.DarkBlue
            }.ToStyle()))
            {
                using (document.BeginAnnotation(HRefKey, "http://www.gallio.org/"))
                    document.AppendText("Gallio.org\n");
            }

            using (document.BeginStyle(new StyleBuilder()
            {
                Font = new Font(Style.Default.Font, FontStyle.Italic),
                LeftMargin = 30,
                RightMargin = 30,
                FirstLineIndent = 40
            }.ToStyle()))
            {
                document.AppendText(
                    "This paragraph has a first line indent, left margin and right margin.  Lorem ipsum dolor sit amet, consectetur adipisicing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum.\n");
            }

            using (document.BeginStyle(new StyleBuilder()
            {
                WordWrap = false,
                Color = Color.Blue,
                Font = new Font(Style.Default.Font, FontStyle.Bold)
            }.ToStyle()))
            {
                document.AppendText("Word wrap disabled.  Word wrap disabled.  Word wrap disabled.  Word wrap disabled.  Word wrap disabled.  Word wrap disabled.  Word wrap disabled.  Word wrap disabled.  Word wrap disabled.  Word wrap disabled.  Word wrap disabled.  Word wrap disabled.  Word wrap disabled.\n");
            }

            using (document.BeginStyle(new StyleBuilder()
            {
                Color = Color.Red,
                Font = new Font(FontFamily.GenericSerif, 16)
            }.ToStyle()))
            {
                document.AppendLine();
                document.AppendText("القرآن تمتلك المظهر الخارجي وعمق خفي ، وهو المعنى الظاهر والمعنى الباطني. هذا المعنى الباطني بدوره يخفي معنى باطني (هذا العمق تمتلك العمق ، بعد صورة من الكرات السماوية التي هي المغلقة داخل بعضها البعض). غني عن ذلك لمدة سبعة المعاني الباطنية (سبعة من عمق أعماق المخفية).");
                document.AppendLine();
            }

            document.AppendLine();
            document.AppendText("Sample image. ");
            document.AppendObject(new EmbeddedImage(Resources.SampleImage) { Baseline = 10 });
            document.AppendText(" (with baseline adjusted by 10 px).");
            document.AppendLine();

            document.AppendLine();
            document.AppendText("How many lambs did Mary have?");
            document.AppendObject(new EmbeddedControl(new TextBox()) { Margin = new Padding(3, 3, 3, 3), Baseline = 6 });

            document.AppendLine();
            document.AppendObject(new EmbeddedImage(Resources.Passed) { Margin = new Padding(3, 3, 3, 3) });
            document.AppendText("Passed 5 ");
            document.AppendObject(new EmbeddedImage(Resources.Failed) { Margin = new Padding(3, 3, 3, 3) });
            document.AppendText("Failed 1");
            document.AppendObject(new EmbeddedImage(Resources.Ignored) { Margin = new Padding(3, 3, 3, 3) });
            document.AppendText("Inconclusive 0");
            document.AppendLine();

            document.AppendLine();
            document.AppendText("What's the magic word?");
            document.AppendObject(new EmbeddedControl(new TextBox()) { Margin = new Padding(3, 3, 3, 3), Baseline = 6 });
            document.AppendLine();

            for (int i = 0; i < 1000; i++)
            {
                document.AppendText(string.Format("Line #{0}\n", i));
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

            if (snapPosition.Kind == SnapKind.Exact)
            {
                string href;
                if (splashView.Document.TryGetAnnotationAtIndex(HRefKey, snapPosition.CharIndex, out href))
                {
                    splashView.Cursor = Cursors.Hand;
                    return;
                }
            }

            splashView.Cursor = Cursors.IBeam;
        }

        private void splashView_SelectionChanged(object sender, EventArgs e)
        {
            selectionStatusLabel.Text = string.Format("Selection: {0} - {1}.", splashView.SelectionStart,
                splashView.SelectionStart + splashView.SelectionLength);
        }

        private void splashView_MouseClick(object sender, MouseEventArgs e)
        {
            SnapPosition snapPosition = splashView.GetSnapPositionAtPoint(splashView.MousePositionToLayout(e.Location));
            if (snapPosition.Kind == SnapKind.Exact)
            {
                string href;
                if (splashView.Document.TryGetAnnotationAtIndex(HRefKey, snapPosition.CharIndex, out href))
                {
                    MessageBox.Show(string.Format("Clicked on link to '{0}'.", href), "Link");
                }
            }
        }
    }
}
