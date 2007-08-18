using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace MbUnit.Core.Reporting.Resources
{
    /// <summary>
    /// Helper methods for accessing reporting resources.
    /// </summary>
    public static class ReportingResources
    {
        /// <summary>
        /// The name of the HTML XSL template resource.
        /// </summary>
        public const string HtmlTemplate = "MbUnit-Report.html.xsl";

        /// <summary>
        /// The name of the Text XSL template resource.
        /// </summary>
        public const string TextTemplate = "MbUnit-Report.txt.xsl";

        /// <summary>
        /// The names of the image resources.
        /// </summary>
        public static readonly string[] Images =
        {
            "Container.png", "Fixture.png", "Test.png", "Logo.png"
        };

        /// <summary>
        /// Gets the reporting resource with the specified name.
        /// </summary>
        /// <param name="name">The resource name</param>
        /// <returns>The stream</returns>
        public static Stream GetResource(string name)
        {
            return typeof(ReportingResources).Assembly.GetManifestResourceStream(typeof(ReportingResources), name);
        }
    }
}
