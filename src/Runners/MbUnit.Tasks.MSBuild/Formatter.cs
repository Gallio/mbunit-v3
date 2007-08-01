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