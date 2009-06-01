using System;
using System.Windows.Forms;
using Gallio.Copy.Properties;
using Gallio.Runtime;
using Gallio.Runtime.ConsoleSupport;
using Gallio.Runtime.Logging;

namespace Gallio.Copy
{
    internal class CopyProgram : ConsoleProgram<CopyArguments>
    {
                /// <summary>
        /// Creates an instance of the program.
        /// </summary>
        private CopyProgram()
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

            var logger = new FilteredLogger(new RichConsoleLogger(Console), Verbosity.Normal);
            using (RuntimeBootstrap.Initialize(runtimeSetup, logger))
            {
                Application.Run(new CopyForm(new CopyController()));
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
            return new CopyProgram().Run(NativeConsole.Instance, args);
        }
    }
}
