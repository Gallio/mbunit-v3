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
using System.Windows.Forms;
using Gallio.Icarus.Controllers.Interfaces;

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
