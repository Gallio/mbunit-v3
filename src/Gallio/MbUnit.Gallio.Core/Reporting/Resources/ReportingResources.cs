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
using System.Collections.ObjectModel;
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
        /// The name of the CSS stylesheet for HTML reports
        /// </summary>
        public const string StyleSheet = "MbUnit-Report.css";

        /// <summary>
        /// The name of the js file for HTML reports
        /// </summary>
        public const string ScriptFile = "MbUnit-Report.js";

        /// <summary>
        /// The names of the image resources.
        /// </summary>
        public static readonly IList<string> Images = new ReadOnlyCollection<string>(new string[]
        {
            @"Container.png", @"Fixture.png", @"Test.png", @"Logo.png", @"Plus.gif", @"Minus.gif", @"FullStop.gif"
        });

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
