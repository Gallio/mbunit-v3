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
using System.Xml;
using Microsoft.Build.Framework;

#if false // Needs to be updated for Gallio.
namespace MbUnit.Tasks.MSBuild
{
    /// <summary>
    /// Summary description for FormatterElement.
    /// </summary>
    public class Formatter
    {
        private FormatterType type = FormatterType.Text;
        private string outputDirectory;
        private string fileNameFormat;

        public Formatter(ITaskItem item)
        {
            this.type = (FormatterType)Enum.Parse(typeof(FormatterType), item.GetAttribute("Type"), true);
            this.outputDirectory =item.GetAttribute("OutputDirectory");
            this.fileNameFormat = item.GetAttribute("FileNameFormat");
        }

        [Required]
        public FormatterType Type
        {
            get { return type; }
            set { type = value; }
        }

        public string FileNameFormat
        {
            get { return fileNameFormat; }
            set { fileNameFormat = value; }
        }

        public string OutputDirectory
        {
            get { return outputDirectory; }
            set { outputDirectory = value;}
        }

        public void Render(MbUnit task, ReportResult result)
        {
            string nameFormat = "mbunit-{0}{1}";
            string outputPath = "";

            if(this.fileNameFormat.Length!=0)
                nameFormat = fileNameFormat;
            if (this.OutputDirectory.Length!=0)
                outputPath = OutputDirectory;

            string outputFileName = null;
            switch (this.Type)
            {
                case FormatterType.Text:
                    outputFileName=TextReport.RenderToText(result, outputPath, nameFormat);
                    break;
                case FormatterType.Html:
                    outputFileName=HtmlReport.RenderToHtml(result, outputPath, nameFormat);
                    break;
                case FormatterType.Xml:
                    outputFileName=XmlReport.RenderToXml(result, outputPath, nameFormat);
                    break;
                case FormatterType.Dox:
                    outputFileName=DoxReport.RenderToDox(result, outputPath, nameFormat);
                    break;
            }

            task.Log.LogMessage("Generated {0} report at {1}",
                this.Type,
                outputFileName);
        }
    }
}
#endif