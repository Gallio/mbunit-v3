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

using System.Windows.Forms;
using Gallio.Icarus.Controllers.Interfaces;
using Gallio.Icarus.Properties;

namespace Gallio.Icarus
{
	public partial class ReloadDialog : Form
	{
		/// <summary>
		/// Set to true while the dialog is opened to prevent showing more then one instance of the dialog.
		/// </summary>
		private static bool isOpened;

		public ReloadDialog(string fileName, IOptionsController optionsController)
		{
			InitializeComponent();

			fileModifiedLabel.Text = string.Format(Resources.FileModified, fileName);

			alwaysReload.DataBindings.Add("Checked", optionsController, "AlwaysReloadFiles",
				false, DataSourceUpdateMode.OnPropertyChanged);
		}

		/// <summary>
		/// Shows the Reload dialog if it is not visible already.
		/// </summary>
		/// <param name="owner">The window that owns the dialog.</param>
		/// <returns>One of the <see cref="DialogResult"/> values (<see cref="DialogResult.Cancel"/>
		/// if another instance of the dialog is already visible).</returns>
		public DialogResult ShowDialogIfNotVisible(IWin32Window owner)
		{
			if (isOpened)
			{
				return DialogResult.Cancel;
			}

			isOpened = true;
			try
			{
				return ShowDialog(owner);
			}
			finally
			{
				isOpened = false;
			}
		}
	}
}
