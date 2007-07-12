// MbUnit Test Framework
// 
// Copyright (c) 2004 Jonathan de Halleux
//
// This software is provided 'as-is', without any express or implied warranty. 
// 
// In no event will the authors be held liable for any damages arising from 
// the use of this software.
// Permission is granted to anyone to use this software for any purpose, 
// including commercial applications, and to alter it and redistribute it 
// freely, subject to the following restrictions:
//
//		1. The origin of this software must not be misrepresented; 
//		you must not claim that you wrote the original software. 
//		If you use this software in a product, an acknowledgment in the product 
//		documentation would be appreciated but is not required.
//
//		2. Altered source versions must be plainly marked as such, and must 
//		not be misrepresented as being the original software.
//
//		3. This notice may not be removed or altered from any source 
//		distribution.
//		
//		MbUnit HomePage: http://www.mbunit.org
//		Author: Jonathan de Halleux

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
