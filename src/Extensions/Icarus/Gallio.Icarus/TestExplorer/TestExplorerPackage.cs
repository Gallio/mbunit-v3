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

using Gallio.Icarus.Commands;
using Gallio.Icarus.WindowManager;
using Gallio.UI.Menus;

namespace Gallio.Icarus.TestExplorer
{
    public class TestExplorerPackage : IPackage
    {
    	private readonly IWindowManager windowManager;
    	private readonly ITestExplorerController testExplorerController;
    	private readonly ITestExplorerModel testExplorerModel;

    	public const string WindowId = "Gallio.Icarus.TestExplorer";

    	public TestExplorerPackage(IWindowManager windowManager, ITestExplorerController testExplorerController, ITestExplorerModel testExplorerModel)
    	{
    		this.windowManager = windowManager;
    		this.testExplorerController = testExplorerController;
    		this.testExplorerModel = testExplorerModel;
    	}

    	public void Load()
		{
			RegisterWindow();
			AddMenuItem();
		}

		private void RegisterWindow()
		{
			windowManager.Register(WindowId, () =>
			{
				var view = new TestExplorerView(testExplorerController, testExplorerModel);
				windowManager.Add(WindowId, view, TestExplorerResources.Test_Explorer, TestExplorerResources.TestExplorerIcon);
			}, Location.Left);
		}

		private void AddMenuItem()
		{
			windowManager.MenuManager.Add("View", () => new MenuCommand
			{
				Command = new DelegateCommand(pm => windowManager.Show(WindowId)),
				Text = TestExplorerResources.Test_Explorer,
				Image = TestExplorerResources.TestExplorerIcon.ToBitmap(),
			});
		}

        public void Dispose() { }
    }
}
