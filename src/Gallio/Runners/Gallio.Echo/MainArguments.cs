// Copyright 2007 MbUnit Project - http://www.mbunit.com/
// Portions Copyright 2000-2004 Jonathan De Halleux, Jamie Cansdale
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
using System.Collections.Generic;
using System.IO;
using Gallio.Core.ConsoleSupport;
using Gallio.Model.Filters;
using Gallio.Model;

namespace Gallio.Echo
{
    public class MainArguments
    {
        #region Files and Directories

        [DefaultCommandLineArgument(
            CommandLineArgumentFlags.MultipleUnique,
            Description = "The test assemblies to run.",
            ValueLabel = "assemblies"
            )]
        public string[] Assemblies;

        [CommandLineArgument(
            CommandLineArgumentFlags.MultipleUnique,
            ShortName = "hd",
            LongName = "hint-directories",
            Description = "The directories used for loading assemblies and other dependent resources.",
            ValueLabel = "dir"
            )]
        public string[] HintDirectories;

        [CommandLineArgument(
            CommandLineArgumentFlags.MultipleUnique,
            ShortName = "pd",
            LongName = "plugin-directory",
            Description = "Additional MbUnit plugin directories to search recursively",
            ValueLabel = "dir"
            )]
        public string[] PluginDirectories;

        [CommandLineArgument(
            CommandLineArgumentFlags.AtMostOnce,
            ShortName = "abd",
            LongName = "application-base-directory",
            Description = "The application base directory to set during test execution.",
            ValueLabel = "dir"
            )]
        public string AppBaseDirectory = "";

        #endregion

        #region Report Arguments
        [CommandLineArgument(
             CommandLineArgumentFlags.AtMostOnce,
             ShortName = "rd",
             LongName = "report-directory",
             Description = "Target output directory for the reports.",
             ValueLabel = "dir"
             )]
        public string ReportDirectory = "";

        [CommandLineArgument(
             CommandLineArgumentFlags.AtMostOnce,
             ShortName = "rnf",
             LongName = "report-name-format",
             Description = "Format string for the report name. {0} is replaced by the date, {1} by the time.  Default = test-report-{0}-{1}.",
             ValueLabel = "pattern"
             )]
        public string ReportNameFormat = "test-report-{0}-{1}";

        [CommandLineArgument(
             CommandLineArgumentFlags.MultipleUnique,
             ShortName = "rt",
             LongName = "report-type",
             Description = "Report types to generate.  See below for all supported types.",
             ValueLabel = "type"
        )]
        public string[] ReportTypes;

        [CommandLineArgument(
             CommandLineArgumentFlags.AtMostOnce,
             ShortName = "sr",
             LongName = "show-reports",
             Description = "Show generated reports in a window using the default system application registered to the report file type."
        )]
        public bool ShowReports;
        #endregion

        #region Filter Arguments

        [CommandLineArgument(
             CommandLineArgumentFlags.AtMostOnce,
             ShortName = "f",
             LongName = "filter",
             Description = "The test filter in the format \"filterkey1=value1,value2; filterkey2=value3;...\".\n"
                + "The filter key may be Id, Assembly, Namespace, Type, Member, or any metadata key that is associated with tests.\n  Examples:\n"
                + "  Type=Fixture1,Fixture2\n"
                + "    Runs tests in Fixture1 and Fixture2.\n"
                + "  Type=Fixture1,Member=Test1\n"
                + "    Runs Fixture1.Test1.\n"
                + "  AuthorName=AlbertEinstein\n"
                + "    Runs tests with the AuthorName metadata value AlbertEinstein."
             )]
        public string Filter;

        #endregion

        #region Misc arguments

        [CommandLineArgument(
             CommandLineArgumentFlags.AtMostOnce,
             ShortName = "h",
             LongName = "help",
             Description = "Display this help text.",
             Synonyms = new string[] { "?" }
             )]
        public bool Help;

        [CommandLineArgument(
             CommandLineArgumentFlags.AtMostOnce,
             ShortName = "v",
             LongName = "verbosity",
             Description = "Controls the level of detail of the information to display.",
             ValueLabel = "level"
             )]
        public Verbosity Verbosity = Verbosity.Normal;

        [CommandLineArgument(
             CommandLineArgumentFlags.AtMostOnce,
             ShortName = "ne",
             LongName = "no-echo-results",
             Description = "Do not echo results to the screen as tests finish.  Unless this option is specified, test results are echoed to the console in varying detail depending on the current verbosity level."
             )]
        public bool NoEchoResults;

        [CommandLineArgument(
             CommandLineArgumentFlags.AtMostOnce,
             ShortName= "",
             LongName = "save-template-tree",
             Description = "Save the template tree to a file as XML.",
             ValueLabel = "file"
             )]
        public string SaveTemplateTree;

        [CommandLineArgument(
             CommandLineArgumentFlags.AtMostOnce,
             ShortName = "",
             LongName = "save-test-tree",
             Description = "Save the test tree to a file as XML.",
             ValueLabel = "file"
             )]
        public string SaveTestTree;

        [CommandLineArgument(
             CommandLineArgumentFlags.AtMostOnce,
             ShortName = "sc",
             LongName = "shadow-copy-files",
             Description = "Enable shadow copying of the assemblies.  Shadow copying allows the original assemblies to be modified while the tests are running.  However, shadow copying may occasionally some tests to fail if they depend on their original location."
             )]
        public bool ShadowCopyFiles;

        #endregion

        public override string ToString()
        {
            StringWriter sw = new StringWriter();
            sw.WriteLine("-- Parsed Arguments");
            sw.WriteLine("Assemblies:");
            foreach (string file in Assemblies)
                sw.WriteLine("\t{0}", file);

            sw.WriteLine("Hint Directories:");
            foreach (string hintDirectory in HintDirectories)
            {
                sw.WriteLine("\t{0}", hintDirectory);
            }

            sw.WriteLine("Plugin Directories:");
            foreach (string pluginDirectory in PluginDirectories)
            {
                sw.WriteLine("\t{0}", pluginDirectory);
            }

            sw.WriteLine("Report folder: {0}", ReportDirectory);
            sw.WriteLine("Report Name Format: {0}", ReportNameFormat);
            sw.WriteLine("Report types: {0}", String.Join(", ", ReportTypes));
            sw.WriteLine("Show reports: {0}", ShowReports);

            sw.WriteLine("Filter: {0}", Filter);

            sw.WriteLine("Verbosity: {0}", Verbosity);
            sw.WriteLine("No Echo Results: {0}", NoEchoResults);
            sw.WriteLine("Save Template Tree: {0}", SaveTemplateTree);
            sw.WriteLine("Save Test Tree: {0}", SaveTestTree);
            sw.WriteLine("Shadow Copy Files: {0}", ShadowCopyFiles);
            return sw.ToString();
        }
    }
}
