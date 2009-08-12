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
using Gallio.Common.IO;
using Gallio.Common.Xml;
using Gallio.Runner.Projects.Schema;

namespace Gallio.Runner.Projects
{
    /// <summary>
    /// Default implementation of a test project manager.
    /// </summary>
    public class DefaultTestProjectManager : ITestProjectManager
    {
        private readonly IFileSystem fileSystem;
        private readonly IXmlSerializer xmlSerializer;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="fileSystem">The file system.</param>
        /// <param name="xmlSerializer">The Xml serializer.</param>
        public DefaultTestProjectManager(IFileSystem fileSystem, IXmlSerializer xmlSerializer)
        {
            if (fileSystem == null)
                throw new ArgumentNullException("fileSystem");
            if (xmlSerializer == null)
                throw new ArgumentNullException("xmlSerializer");

            this.fileSystem = fileSystem;
            this.xmlSerializer = xmlSerializer;
        }

        /// <inheritdoc />
        public TestProject LoadProject(FileInfo testProjectFile)
        {
            if (testProjectFile == null)
                throw new ArgumentNullException("testProjectFile");

            if (!fileSystem.FileExists(testProjectFile.FullName))
                throw new ArgumentException(String.Format("Project file {0} does not exist.", testProjectFile.FullName));

            var testProjectData = xmlSerializer.LoadFromXml<TestProjectData>(testProjectFile.FullName);
            return LoadProject(testProjectFile.DirectoryName, testProjectData);
        }

        private static TestProject LoadProject(string directoryName, TestProjectData testProjectData)
        {
            testProjectData.Validate(); // sanity check
            testProjectData.MakeAbsolutePaths(directoryName);

            var testProject = testProjectData.ToTestProject();
            return testProject;
        }

        /// <inheritdoc />
        public TestProject NewProject(string projectName)
        {
            if (string.IsNullOrEmpty(projectName))
                throw new ArgumentException("projectName");

            var testProjectData = new TestProjectData();
            return LoadProject(Path.GetDirectoryName(projectName), testProjectData);
        }

        /// <inheritdoc />
        public void SaveProject(TestProject testProject, FileInfo testProjectFile)
        {
            if (testProject == null)
                throw new ArgumentNullException("testProject");
            if (testProjectFile == null)
                throw new ArgumentNullException("testProjectFile");

            var testProjectData = new TestProjectData(testProject);
            testProjectData.Validate(); // sanity check
            testProjectData.MakeRelativePaths(testProjectFile.DirectoryName);
            xmlSerializer.SaveToXml(testProjectData, testProjectFile.FullName);
        }
    }
}
