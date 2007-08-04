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
using MbUnit.Core.Runner.CommandLine;
using MbUnit.Framework;
using MbUnit.Framework.Kernel.Filters;
using MbUnit.Framework.Kernel.Metadata;
using MbUnit.Framework.Kernel.Model;
using MbUnit.Framework.Kernel.Utilities;

namespace MbUnit.Echo
{
	public class MainArguments
	{
		[DefaultCommandLineArgument(
			 CommandLineArgumentType.MultipleUnique,
			 Description = "List of assemblies containing the tests"
			 )]
		public string[] Files = null;

        [CommandLineArgument(
             CommandLineArgumentType.MultipleUnique,
             ShortName = "ap",
             LongName = "assembly-path",
             Description = "Path where assembly can be loaded"
             )]
        public string[] AssemblyPath = null;

		#region Report Arguments
		[CommandLineArgument(
			 CommandLineArgumentType.AtMostOnce,
			 ShortName = "rf",
			 LongName = "report-folder",
			 Description="Target output folder for the reports"
			 )]
		public String ReportFolder = "";

        [CommandLineArgument(
             CommandLineArgumentType.AtMostOnce,
             ShortName = "rnf",
             LongName = "report-name-format",
             Description = "Format string for the report name. {0} is replaced by the date, {1} by the time.Default = mbunit-{0}{1}"
             )]
        public string ReportNameFormat = "mbunit-{0}{1}";

        [CommandLineArgument(
			 CommandLineArgumentType.MultipleUnique,
			 ShortName = "rt",
			 LongName = "report-type",
			Description="Report types supported: Xml, Html, Text"
		)]
		public ReportType[] ReportTypes = null;

        [CommandLineArgument(
             CommandLineArgumentType.AtMostOnce,
             ShortName = "sr",
             LongName = "show-reports",
            Description = "Show generated reports in a window"
        )]
        public bool ShowReports = false;
		#endregion

		#region Filter Arguments
		[CommandLineArgument(
			 CommandLineArgumentType.MultipleUnique,
			 ShortName = "fc",
			 LongName = "filter-category",
			 Description="Name of the filtered category"
			 )]
		public string[] FilterCategories = null;

		[CommandLineArgument(
             CommandLineArgumentType.MultipleUnique,
			 ShortName = "fa",
			 LongName = "filter-author",
			 Description="Name of the filtered author name"
			 )]
        public string[] FilterAuthors = null;

		[CommandLineArgument(
			 CommandLineArgumentType.MultipleUnique,
			 ShortName = "ft",
			 LongName = "filter-type",
			 Description="Name of the filtered type"
			 )]
        public string[] FilterTypes = null;

		[CommandLineArgument(
             CommandLineArgumentType.MultipleUnique,
			 ShortName = "fn",
			 LongName = "filter-namespace",
			 Description="Name of the filtered namespace"
			 )]
        public string[] FilterNamespaces = null;

		[CommandLineArgument(
			 CommandLineArgumentType.MultipleUnique,
			 ShortName = "fi",
			 LongName = "filter-importance",
			 Description="Name of the filtered importance"
			 )]
		public TestImportance[] FilterImportances = null;
		#endregion

        #region Misc arguments
        [CommandLineArgument(
             CommandLineArgumentType.AtMostOnce,
             ShortName = "h",
             LongName = "help",
             Description = "Display this help text"
             )]
        public bool Help = false;

        [CommandLineArgument(
			 CommandLineArgumentType.AtMostOnce,
			 ShortName = "v",
			 LongName = "verbosity",
			 Description="Return a lot of information or not..."
			 )]
		public Verbosity Verbosity = Verbosity.Normal;

        [CommandLineArgument(
             CommandLineArgumentType.MultipleUnique,
             ShortName = "pd",
             LongName = "plugin-directory",
             Description = "Additional MbUnit plugin directories to search recursively"
             )]
        public string[] PluginDirectories;

        [CommandLineArgument(
             CommandLineArgumentType.AtMostOnce,
             LongName = "save-template-tree",
             Description = "Saves the template tree to a file as XML"
             )]
        public string SaveTemplateTree;

        [CommandLineArgument(
             CommandLineArgumentType.AtMostOnce,
             LongName = "save-test-tree",
             Description = "Saves the test tree to a file as XML"
             )]
        public string SaveTestTree;

        [CommandLineArgument(
             CommandLineArgumentType.AtMostOnce,
             ShortName = "sc",
             LongName = "shadow-copy-files",
             Description = "Enabled/disable shadow copying of the assemblies"
             )]
        public bool ShadowCopyFiles = false;
        #endregion

        public override string ToString()
		{
			StringWriter sw = new StringWriter();
			sw.WriteLine("-- Parsed Arguments");
			sw.WriteLine("Files:");
			foreach(string file in this.Files)
				sw.WriteLine("\t{0}",file);

            sw.WriteLine("Assembly paths:");
            foreach (string assemblyPath in this.AssemblyPath)
            {
                sw.WriteLine("\t{0}", assemblyPath);
            }

            sw.WriteLine("Report folder: {0}",this.ReportFolder);
            sw.WriteLine("Report Name Format: {0}",this.ReportNameFormat);
			sw.WriteLine("Report types: {0}", String.Join(", ",
                ListUtils.ConvertAllToArray<ReportType, string>(this.ReportTypes, delegate(ReportType reportType) { return reportType.ToString(); })));
            sw.WriteLine("Show reports: {0}", this.ShowReports);

            sw.WriteLine("Filter Category: {0}", String.Join(", ", this.FilterCategories));
			sw.WriteLine("Filter Author: {0}", String.Join(", ", this.FilterAuthors));
			sw.WriteLine("Filter Namespace: {0}", String.Join(", ", this.FilterNamespaces));
			sw.WriteLine("Filter Type: {0}", String.Join(", ", this.FilterTypes));

			sw.WriteLine("Verbosity: {0}", this.Verbosity);
            sw.WriteLine("Save Template Tree: {0}", this.SaveTemplateTree);
            sw.WriteLine("Save Test Tree: {0}", this.SaveTestTree);
            sw.WriteLine("ShadowCopyFiles: {0}", this.ShadowCopyFiles);
            return sw.ToString();
		}

		public Filter<ITest> GetFilter()
		{
            List<Filter<ITest>> filters = new List<Filter<ITest>>();

			if (FilterCategories.Length != 0)
			{
                List<Filter<ITest>> categoryFilters = new List<Filter<ITest>>();

                foreach (string category in FilterCategories)
                    categoryFilters.Add(new MetadataFilter<ITest>(MetadataConstants.CategoryNameKey, category));

                filters.Add(new OrFilter<ITest>(categoryFilters.ToArray()));
			}

			if (FilterAuthors.Length != 0)
			{
                List<Filter<ITest>> authorFilters = new List<Filter<ITest>>();

                foreach (string author in FilterAuthors)
                    authorFilters.Add(new MetadataFilter<ITest>(MetadataConstants.AuthorNameKey, author));

                filters.Add(new OrFilter<ITest>(authorFilters.ToArray()));
			}

            if (FilterImportances.Length != 0)
            {
                List<Filter<ITest>> importanceFilters = new List<Filter<ITest>>();

                foreach (TestImportance importance in FilterImportances)
                    importanceFilters.Add(new MetadataFilter<ITest>(MetadataConstants.ImportanceKey, importance.ToString()));

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
}
