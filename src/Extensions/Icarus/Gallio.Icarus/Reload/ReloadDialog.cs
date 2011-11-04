// Copyright 2005-2010 Gallio Project - http://www.gallio.org/
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
using Gallio.Icarus.Properties;

namespace Gallio.Icarus.Reload
{
	public partial class ReloadDialog : Form
	{
		private readonly IOptionsController optionsController;

		public ReloadDialog(IOptionsController optionsController)
		{
			this.optionsController = optionsController;
			
			InitializeComponent();
		}

		public void SetFilename(string filename)
		{
			fileModifiedLabel.Text = string.Format(Resources.FileModified, filename);
		}

		protected override void OnLoad(EventArgs e)
		{
			alwaysReload.DataBindings.Add("Checked", optionsController, "AlwaysReloadFiles",
				false, DataSourceUpdateMode.OnPropertyChanged);

			base.OnLoad(e);
		}
	}
}
