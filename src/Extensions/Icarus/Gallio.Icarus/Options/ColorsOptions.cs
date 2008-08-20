using System;
using System.Windows.Forms;
using Gallio.Icarus.Interfaces;

namespace Gallio.Icarus.Options
{
    internal partial class ColorsOptions : OptionsPanel
    {
        private readonly IOptionsController optionsController;

        public ColorsOptions(IOptionsController optionsController)
        {
            this.optionsController = optionsController;

            InitializeComponent();

            passedColor.DataBindings.Add("BackColor", optionsController, "PassedColor");
            failedColor.DataBindings.Add("BackColor", optionsController, "FailedColor");
            inconclusiveColor.DataBindings.Add("BackColor", optionsController, "InconclusiveColor");
            skippedColor.DataBindings.Add("BackColor", optionsController, "SkippedColor");
        }

        private void color_Click(object sender, EventArgs e)
        {
            using (ColorDialog colorDialog = new ColorDialog())
            {
                colorDialog.Color = ((Label) sender).BackColor;
                if (colorDialog.ShowDialog() != DialogResult.OK)
                    return;
                ((Label) sender).BackColor = colorDialog.Color;
                // two way databinding doesn't appear to work!
                switch (((Label)sender).Name)
                {
                    case "passedColor":
                        optionsController.PassedColor = colorDialog.Color;
                        break;
                    case "failedColor":
                        optionsController.FailedColor = colorDialog.Color;
                        break;
                    case "inconclusiveColor":
                        optionsController.InconclusiveColor = colorDialog.Color;
                        break;
                    case "skippedColor":
                        optionsController.SkippedColor = colorDialog.Color;
                        break;
                }
            }
        }
    }
}
