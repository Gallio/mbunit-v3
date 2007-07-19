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
using System.IO;
using MbUnit.Core.Runner.CommandLine;

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
			 CommandLineArgumentType.AtMostOnce,
			 ShortName = "fc",
			 LongName = "filter-category",
			 Description="Name of the filtered category"
			 )]
		public String FilterCategory = null;

		[CommandLineArgument(
			 CommandLineArgumentType.AtMostOnce,
			 ShortName = "fa",
			 LongName = "filter-author",
			 Description="Name of the filtered author name"
			 )]
		public String FilterAuthor = null;

		[CommandLineArgument(
			 CommandLineArgumentType.AtMostOnce,
			 ShortName = "ft",
			 LongName = "filter-type",
			 Description="Name of the filtered type"
			 )]
		public String FilterType = null;

		[CommandLineArgument(
			 CommandLineArgumentType.AtMostOnce,
			 ShortName = "fn",
			 LongName = "filter-namespace",
			 Description="Name of the filtered namespace"
			 )]
		public String FilterNamespace = null;

/*
		[CommandLineArgument(
			 CommandLineArgumentType.MultipleUnique,
			 ShortName = "fi",
			 LongName = "filter-importance",
			 Description="Name of the filtered importance"
			 )]
		public TestImportance[] FilterImportances = null;
*/
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
			 LongName = "verbose",
			 Description="Return a lot of information or not..."
			 )]
		public bool Verbose = false;

        [CommandLineArgument(
             CommandLineArgumentType.MultipleUnique,
             ShortName = "pd",
             LongName = "plugin-directory",
             Description = "Additional directories that contain MbUnit plugins"
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
			sw.WriteLine("Report types:");
			foreach(ReportType type in this.ReportTypes)
				sw.WriteLine("\t{0}",type);
            sw.WriteLine("Show reports: {0}", this.ShowReports);

            sw.WriteLine("Filter Category: {0}",this.FilterCategory);
			sw.WriteLine("Filter Author: {0}",this.FilterAuthor);
			sw.WriteLine("Filter Namespace: {0}",this.FilterNamespace);
			sw.WriteLine("Filter Type: {0}",this.FilterType);
			sw.WriteLine("Verbose: {0}",this.Verbose);
            sw.WriteLine("Save Template Tree: {0}", this.SaveTemplateTree);
            sw.WriteLine("Save Test Tree: {0}", this.SaveTestTree);
            sw.WriteLine("ShadowCopyFiles: {0}", this.ShadowCopyFiles);
            return sw.ToString();
		}

        /*
		public FixtureFilterBase GetFilter()
		{
			FixtureFilterBase filter = FixtureFilters.Any;

			if (this.FilterCategory!=null)
			{
				filter = FixtureFilters.And(
					filter,
					FixtureFilters.Category(this.FilterCategory)
					);
			}
			if (this.FilterAuthor!=null)
			{
				filter = FixtureFilters.And(
					filter,
					FixtureFilters.Author(this.FilterAuthor)
					);
			}
			if (this.FilterNamespace!=null)
			{
				filter = FixtureFilters.And(
					filter,
					FixtureFilters.Namespace(this.FilterNamespace)
					);
			}
			if (this.FilterType!=null)
			{
				filter = FixtureFilters.And(
					filter,
					FixtureFilters.Type(this.FilterType)
					);
			}
			return filter;
		}
         */
	}
}
