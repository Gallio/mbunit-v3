// Copyright 2005-2010 Gallio Project - http://www.gallio.org/
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
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Text;
using Gallio.Common.Text;
using Gallio.Model;
using Gallio.Runtime.Installer;
using Gallio.Runtime.ProgressMonitoring;

namespace Gallio.Reports
{
    /// <summary>
    /// Installs derived resources for reports such as test framework icons.
    /// </summary>
    public class ReportResourcesInstaller : BaseInstaller
    {
        private readonly ITestKindManager testKindManager;
        private readonly DirectoryInfo testKindImageDir;
        private readonly FileInfo generatedCssFile;

        /// <summary>
        /// Initializes the installer.
        /// </summary>
        /// <param name="testKindManager">The test kind manager, not null.</param>
        /// <param name="testKindImageDir">The test kind image directory, not null.</param>
        /// <param name="generatedCssFile">The generated CSS file, not null.</param>
        public ReportResourcesInstaller(ITestKindManager testKindManager, DirectoryInfo testKindImageDir,
            FileInfo generatedCssFile)
        {
            this.testKindManager = testKindManager;
            this.testKindImageDir = testKindImageDir;
            this.generatedCssFile = generatedCssFile;
        }

        /// <inheritdoc />
        public override void Install(IProgressMonitor progressMonitor)
        {
            using (progressMonitor.BeginTask("Installing report resources.", 1))
            {
                using (var cssWriter = new StreamWriter(generatedCssFile.Open(FileMode.Create, FileAccess.Write, FileShare.None)))
                {
                    GenerateTestKinds(cssWriter);
                }
            }
        }

        private void GenerateTestKinds(TextWriter cssWriter)
        {
            if (testKindImageDir.Exists)
            {
                foreach (FileInfo file in testKindImageDir.GetFiles("*.png"))
                    file.Delete();
            }
            else
            {
                testKindImageDir.Create();
            }

            foreach (var testKindHandle in testKindManager.TestKindHandles)
            {
                TestKindTraits testKindTraits = testKindHandle.GetTraits();
                string normalizedKindName = NormalizeTestKindName(testKindTraits.Name);

                cssWriter.WriteLine(".gallio-report .testKind-{0}", normalizedKindName);
                cssWriter.WriteLine("{");

                if (testKindTraits.Icon != null)
                {
                    string imageName = normalizedKindName + ".png";
                    string imagePath = Path.Combine(testKindImageDir.FullName, imageName);
                    Icon icon = new Icon(testKindTraits.Icon, 16, 16);
                    icon.ToBitmap().Save(imagePath, ImageFormat.Png);

                    cssWriter.WriteLine("    background-image: url(../img/testkinds/{0});", imageName);
                }

                // Need a workaround for alt text & title on these images that does not involve
                // spewing them all over the HTML.
                //cssWriter.WriteLine("    alt: {0};", StringUtils.ToStringLiteral(testKindTraits.Name));
                //cssWriter.WriteLine("    title: {0};", StringUtils.ToStringLiteral(testKindTraits.Description));

                cssWriter.WriteLine("}");
            }
        }

        private static string NormalizeTestKindName(string kind)
        {
            return kind.Replace(" ", "").Replace(".", "");
        }
    }
}
