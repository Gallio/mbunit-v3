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
using MbUnit.Core.ConsoleSupport;
using MbUnit.Framework;
using MbUnit.Model.Filters;
using MbUnit.Model;

namespace MbUnit.Runner
{
    public class MainArguments
    {
        #region Files and Directories

        [DefaultCommandLineArgument(
            CommandLineArgumentFlags.MultipleUnique,
            Description = "List of assemblies containing the tests.",
            ValueLabel = "assemblies"
            )]
        public string[] Assemblies;

        [CommandLineArgument(
            CommandLineArgumentFlags.MultipleUnique,
            ShortName = "hd",
            LongName = "hint-directories",
            Description = "The list of directories used for loading assemblies and other dependent resources.",
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
             Description = "Format string for the report name. {0} is replaced by the date, {1} by the time.  Default = mbunit-{0}-{1}.",
             ValueLabel = "pattern"
             )]
        public string ReportNameFormat = "mbunit-result-{0}-{1}";

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
             CommandLineArgumentFlags.MultipleUnique,
             ShortName = "fc",
             LongName = "filter-category",
             Description = "Name of the filtered category.",
             ValueLabel = "category"
             )]
        public string[] FilterCategories;

        [CommandLineArgument(
             CommandLineArgumentFlags.MultipleUnique,
             ShortName = "fa",
             LongName = "filter-author",
             Description = "Name of the filtered author name.",
             ValueLabel = "author"
             )]
        public string[] FilterAuthors;

        [CommandLineArgument(
             CommandLineArgumentFlags.MultipleUnique,
             ShortName = "ft",
             LongName = "filter-type",
             Description = "Full or partial name of the filtered type.",
             ValueLabel = "type"
             )]
        public string[] FilterTypes;

        [CommandLineArgument(
             CommandLineArgumentFlags.MultipleUnique,
             ShortName = "fn",
             LongName = "filter-namespace",
             Description = "Name of the filtered namespace.",
             ValueLabel = "namespace"
             )]
        public string[] FilterNamespaces;

        [CommandLineArgument(
             CommandLineArgumentFlags.MultipleUnique,
             ShortName = "fi",
             LongName = "filter-importance",
             Description = "Name of the filtered importance.",
             ValueLabel = "importance"
             )]
        public TestImportance[] FilterImportances;

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

            sw.WriteLine("Filter Category: {0}", String.Join(", ", FilterCategories));
            sw.WriteLine("Filter Author: {0}", String.Join(", ", FilterAuthors));
            sw.WriteLine("Filter Namespace: {0}", String.Join(", ", FilterNamespaces));
            sw.WriteLine("Filter Type: {0}", String.Join(", ", FilterTypes));

            sw.WriteLine("Verbosity: {0}", Verbosity);
            sw.WriteLine("No Echo Results: {0}", NoEchoResults);
            sw.WriteLine("Save Template Tree: {0}", SaveTemplateTree);
            sw.WriteLine("Save Test Tree: {0}", SaveTestTree);
            sw.WriteLine("Shadow Copy Files: {0}", ShadowCopyFiles);
            return sw.ToString();
        }

        public Filter<ITest> GetFilter()
        {
            List<Filter<ITest>> filters = new List<Filter<ITest>>();

            if (FilterCategories.Length != 0)
            {
                List<Filter<ITest>> categoryFilters = new List<Filter<ITest>>();

                foreach (string category in FilterCategories)
                    categoryFilters.Add(new MetadataFilter<ITest>(MetadataKeys.CategoryName, category));

                filters.Add(new OrFilter<ITest>(categoryFilters.ToArray()));
            }

            if (FilterAuthors.Length != 0)
            {
                List<Filter<ITest>> authorFilters = new List<Filter<ITest>>();

                foreach (string author in FilterAuthors)
                    authorFilters.Add(new MetadataFilter<ITest>(MetadataKeys.AuthorName, author));

                filters.Add(new OrFilter<ITest>(authorFilters.ToArray()));
            }

            if (FilterImportances.Length != 0)
            {
                List<Filter<ITest>> importanceFilters = new List<Filter<ITest>>();

                foreach (TestImportance importance in FilterImportances)
                    importanceFilters.Add(new MetadataFilter<ITest>(MetadataKeys.Importance, importance.ToString()));

                filters.Add(new OrFilter<ITest>(importanceFilters.ToArray()));
            }

            if (FilterNamespaces.Length != 0)
            {
                List<Filter<ITest>> namespaceFilters = new List<Filter<ITest>>();

                foreach (string @namespace in FilterNamespaces)
                    namespaceFilters.Add(new NamespaceFilter<ITest>(@namespace));

                filters.Add(new OrFilter<ITest>(namespaceFilters.ToArray()));
            }

            if (FilterTypes.Length != 0)
            {
                List<Filter<ITest>> typeFilters = new List<Filter<ITest>>();

                // FIXME: Should we always include derived types?
                foreach (string type in FilterTypes)
                    typeFilters.Add(new TypeFilter<ITest>(type, true));

                filters.Add(new OrFilter<ITest>(typeFilters.ToArray()));
            }

            return new AndFilter<ITest>(filters.ToArray());
        }
    }

    /// <summary>
    /// Specifies the verbosity of the output.
    /// </summary>
    public enum Verbosity
    {
        Quiet = 0,
        Normal,
        Verbose,
        Debug
    }
}
