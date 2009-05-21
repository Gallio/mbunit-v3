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
using System.IO;
using System.Windows.Forms;
using Gallio.Common.Collections;
using Gallio.Common.Reflection;
using Gallio.ControlPanel.Properties;
using Gallio.Runtime;
using Gallio.Runtime.ConsoleSupport;
using Gallio.Runtime.Logging;
using Gallio.UI.ControlPanel;

namespace Gallio.ControlPanel
{
    /// <summary>
    /// The Control Panel program.
    /// </summary>
    public class ControlPanelProgram : ConsoleProgram<ControlPanelArguments>
    {
        /// <summary>
        /// Creates an instance of the program.
        /// </summary>
        private ControlPanelProgram()
        {
            ApplicationName = Resources.ApplicationName;
        }

        /// <inheritdoc />
        protected override int RunImpl(string[] args)
        {
            if (!ParseArguments(args))
            {
                ShowHelp();
                return 1;
            }

            if (Arguments.Help)
            {
                ShowHelp();
                return 0;
            }

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            var runtimeSetup = new RuntimeSetup();
            runtimeSetup.PluginDirectories.AddRange(Arguments.PluginDirectories);

            ILogger logger = new FilteredLogger(new RichConsoleLogger(Console), Verbosity.Normal);
            using (RuntimeBootstrap.Initialize(runtimeSetup, logger))
            {
                IControlPanelPresenter presenter = RuntimeAccessor.Instance.ServiceLocator.Resolve<IControlPanelPresenter>();
                presenter.Show(null);
            }

            return 0;
        }

        protected override void ShowHelp()
        {
            ArgumentParser.ShowUsageInMessageBox(ApplicationTitle);
        }

        [STAThread]
        internal static int Main(string[] args)
        {
            return new ControlPanelProgram().Run(NativeConsole.Instance, args);
        }
    }
}
