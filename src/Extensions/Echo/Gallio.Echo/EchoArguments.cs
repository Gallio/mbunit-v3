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
using Gallio.Common.Collections;
using Gallio.Runtime.ConsoleSupport;
using Gallio.Runner;
using Gallio.Runtime.Logging;

namespace Gallio.Echo
{
    public class EchoArguments
    {
        #region Files and Directories

        [DefaultCommandLineArgument(
            CommandLineArgumentFlags.MultipleUnique,
            Description = "The test assemblies to run.",
            ValueLabel = "assemblies"
            )]
        public string[] Assemblies = EmptyArray<string>.Instance;

        [CommandLineArgument(
            CommandLineArgumentFlags.MultipleUnique,
            ShortName = "hd",
            LongName = "hint-directory",
            Description = "Additional directories used for loading assemblies and other dependent resources.",
            ValueLabel = "dir"
            )]
        public string[] HintDirectories = EmptyArray<string>.Instance;

        [CommandLineArgument(
            CommandLineArgumentFlags.MultipleUnique,
            ShortName = "pd",
            LongName = "plugin-directory",
            Description = "Additional MbUnit plugin directories to search recursively",
            ValueLabel = "dir"
            )]
        public string[] PluginDirectories = EmptyArray<string>.Instance;

        [CommandLineArgument(
            CommandLineArgumentFlags.AtMostOnce,
            ShortName = "abd",
            LongName = "application-base-directory",
            Description = "The application base directory to use during test execution instead of the default.",
            ValueLabel = "dir"
            )]
        public string ApplicationBaseDirectory;

        [CommandLineArgument(
            CommandLineArgumentFlags.AtMostOnce,
            ShortName = "wd",
            LongName = "working-directory",
            Description = "The working directory to use during test execution instead of the default.",
            ValueLabel = "dir"
            )]
        public string WorkingDirectory;

        [CommandLineArgument(
             CommandLineArgumentFlags.AtMostOnce,
             ShortName = "sc",
             LongName = "shadow-copy",
             Description = "Enable shadow copying of the assemblies.  Shadow copying allows the original assemblies to be modified while the tests are running.  However, shadow copying may occasionally cause some tests to fail if they depend on their original location."
             )]
        public bool ShadowCopy;

        [CommandLineArgument(
             CommandLineArgumentFlags.AtMostOnce,
             ShortName = "d",
             LongName = "debug",
             Description = "Attach the debugger to the test process."
             )]
        public bool Debug;

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
        public string[] ReportTypes = EmptyArray<string>.Instance;

        [CommandLineArgument(
             CommandLineArgumentFlags.Multiple,
             ShortName = "rfp",
             LongName = "report-formatter-property",
             ValueLabel = "key=value",
             Description = "Specifies a property key/value for the report formatters.  eg. \"AttachmentContentDisposition=Absent\""
             )]
        public string[] ReportFormatterProperties = EmptyArray<string>.Instance;

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
             ValueLabel = "expr",
             Description =
                 "Sets the filter set to apply, which consists of one or more inclusion "
                 + "or exclusion filter rules prefixed using 'include' (optional) or 'exclude'. "
                 + "A filter rule consists of zero or more filter expressions "
                 + "that may be combined using 'and', 'or', and 'not' and grouped with "
                 + "parentheses.  A filter expression consists of a filter key followed by one or "
                 + "more comma-delimited matching values in the form 'key: value, \"quoted value\", "
                 +" /regular expression/'.\n\n"
                 + "Examples:\n"
                 + "Type: Fixture1, Fixture2\n"
                 + "- Runs tests belonging to Fixture1 or Fixture2 including subclasses.\n"
                 + "ExactType: Fixture1 and Member: Test1\n"
                 + "- Runs Fixture1.Test1 excluding subclasses.\n"
                 + "include Type: Fixture1 exclude AuthorName: \"Albert Einstein\"\n"
                 + "- Run Fixture1 and all other tests except those with the AuthorName metadata value \"Albert Einstein\"."
             )]
        public string Filter;

        #endregion

        #region Runner options

        [CommandLineArgument(
             CommandLineArgumentFlags.AtMostOnce,
             ShortName = "r",
             LongName = "runner",
             ValueLabel = "type",
             Description = "Specifies the type of test runner to use.  See below for all supported types.  The default is '" + StandardTestRunnerFactoryNames.IsolatedProcess + "'"
             )]
        public string RunnerType = StandardTestRunnerFactoryNames.IsolatedProcess;

        [CommandLineArgument(
             CommandLineArgumentFlags.Multiple,
             ShortName = "re",
             LongName = "runner-extension",
             ValueLabel = "type;params",
             Description = "Specifies the type, assembly, and parameters of custom test runner extensions to use during the test run in the form:\n'[Namespace.]Type,Assembly[;Parameters]'.\neg. 'FancyLogger,MyExtensions.dll;ColorOutput,FancyIndenting'"
             )]
        public string[] RunnerExtensions = EmptyArray<string>.Instance;

        [CommandLineArgument(
             CommandLineArgumentFlags.Multiple,
             ShortName = "rp",
             LongName = "runner-property",
             ValueLabel = "key=value",
             Description = "Specifies an option property key/value for the test runner.  eg. \"NCoverArguments='//eas Gallio'\""
             )]
        public string[] RunnerProperties = EmptyArray<string>.Instance;

        #endregion

        #region Misc arguments

        [CommandLineArgument(
             CommandLineArgumentFlags.AtMostOnce,
             ShortName = "h",
             LongName = "help",
             Description = "Display this help text.",
             Synonyms = new[] { "?" }
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
             Description = "Do not echo results to the screen as tests finish.  If this option is specified, only the final summary statistics are displayed.  Otherwise test results are echoed to the console in varying detail depending on the current verbosity level."
             )]
        public bool NoEchoResults;

        [CommandLineArgument(
             CommandLineArgumentFlags.AtMostOnce,
             ShortName = "np",
             LongName = "no-progress",
             Description = "Do not display progress messages during execution."
             )]
        public bool NoProgress;

        [CommandLineArgument(
             CommandLineArgumentFlags.AtMostOnce,
             ShortName = "dnr",
             LongName = "do-not-run",
             Description = "Load the tests but does not run them.  This option may be used to produce a report that contains test metadata for consumption by other tools."
             )]
        public bool DoNotRun;

        [CommandLineArgument(
             CommandLineArgumentFlags.AtMostOnce,
             ShortName = "ia",
             LongName = "ignore-annotations",
             Description = "Ignore annotations when determining the result code.  When not specified error annotations, usually indicative of broken tests, will cause a failure result to be generated."
             )]
        public bool IgnoreAnnotations;

        [CommandLineArgument(
            CommandLineArgumentFlags.AtMostOnce,
            ShortName = "rtl",
            LongName = "run-time-limit",
            Description = "Maximum amount of time (in seconds) the tests can run before they are canceled. The default is an infinite time to run.",
            ValueLabel = "limit"
            )]
        public int RunTimeLimitInSeconds = -1;

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

            sw.WriteLine("Application Base Directory: {0}", ApplicationBaseDirectory);
            sw.WriteLine("Working Directory: {0}", WorkingDirectory);
            sw.WriteLine("Shadow Copy: {0}", ShadowCopy);
            sw.WriteLine("Debug: {0}", Debug);

            sw.WriteLine("Report Folder: {0}", ReportDirectory);
            sw.WriteLine("Report Name Format: {0}", ReportNameFormat);
            sw.WriteLine("Report Types: {0}", String.Join(", ", ReportTypes));
            sw.WriteLine("Report Formatter Properties: {0}", String.Join(", ", ReportFormatterProperties));
            sw.WriteLine("Show Reports: {0}", ShowReports);

            sw.WriteLine("Filter: {0}", Filter);

            sw.WriteLine("Runner Type: {0}", RunnerType);
            sw.WriteLine("Runner Extensions: {0}", String.Join(", ", RunnerExtensions));
            sw.WriteLine("Runner Properties: {0}", String.Join(", ", RunnerProperties));

            sw.WriteLine("Verbosity: {0}", Verbosity);
            sw.WriteLine("No Echo Results: {0}", NoEchoResults);
            sw.WriteLine("Save Test Model: {0}", DoNotRun);
            
            sw.WriteLine("RunTimeLimit: {0}", (RunTimeLimitInSeconds >= 0) ? RunTimeLimitInSeconds + " seconds" : "infinte");
            return sw.ToString();
        }
    }
}
