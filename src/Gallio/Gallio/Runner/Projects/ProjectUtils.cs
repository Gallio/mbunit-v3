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
using System.Collections.Generic;
using System.IO;
using Gallio.Common.IO;
using Gallio.Common.Xml;
using NDepend.Helpers.FileDirectoryPath;

namespace Gallio.Runner.Projects
{
    /// <summary>
    /// Utilities for working with Gallio projects.
    /// </summary>
    public class ProjectUtils
    {
        private readonly IFileSystem fileSystem;
        private readonly IXmlSerializer xmlSerializer;

        ///<summary>
        /// Constructor.
        ///</summary>
        ///<param name="fileSystem">The file system to use (allows mocking).</param>
        ///<param name="xmlSerializer">The xml serializer to use (allows mocking).</param>
        public ProjectUtils(IFileSystem fileSystem, IXmlSerializer xmlSerializer)
        {
            this.fileSystem = fileSystem;
            this.xmlSerializer = xmlSerializer;
        }

        /// <summary>
        /// Load a saved Gallio project file.
        /// </summary>
        /// <param name="projectLocation">The location of the project file.</param>
        /// <returns>A Gallio Project instance.</returns>
        /// <exception cref="ArgumentException">Thrown when the specified project location is invalid.</exception>
        public Project LoadProject(string projectLocation)
        {
            // fail fast
            if (!fileSystem.FileExists(projectLocation))
                throw new ArgumentException(String.Format("Project file {0} does not exist.", projectLocation));

            // deserialize project
            var project = xmlSerializer.LoadFromXml<Project>(projectLocation);
            ConvertFromRelativePaths(project, Path.GetDirectoryName(projectLocation));
            return project;
        }

        private void ConvertFromRelativePaths(Project project, string directory)
        {
            var assemblyList = new List<string>();
            foreach (string assembly in project.TestPackageConfig.AssemblyFiles)
            {
                string assemblyPath = ConvertFromRelativePath(directory, assembly);
                if (fileSystem.FileExists(assemblyPath))
                    assemblyList.Add(assemblyPath);
            }
            project.TestPackageConfig.AssemblyFiles.Clear();
            project.TestPackageConfig.AssemblyFiles.AddRange(assemblyList);

            project.ReportDirectory = ConvertFromRelativePath(directory, project.ReportDirectory);
        }

        private string ConvertFromRelativePath(string directory, string file)
        {
            if (!fileSystem.IsPathRooted(file))
            {
                try
                {
                    var filePath = new FilePathRelative(file);
                    var directoryPath = new DirectoryPathAbsolute(directory);
                    return filePath.GetAbsolutePathFrom(directoryPath).Path;
                }
                catch
                { }
            }
            return file;
        }

        /// <summary>
        /// Save a Gallio project to disk.
        /// </summary>
        /// <param name="project">The Project instance to save.</param>
        /// <param name="projectLocation">The location to save it to.</param>
        public void SaveProject(Project project, string projectLocation)
        {
            ConvertToRelativePaths(project, Path.GetDirectoryName(projectLocation));
            xmlSerializer.SaveToXml(project, projectLocation);
            ConvertFromRelativePaths(project, Path.GetDirectoryName(projectLocation));
        }

        private void ConvertToRelativePaths(Project project, string directory)
        {
            var assemblyList = new List<string>();
            foreach (string assembly in project.TestPackageConfig.AssemblyFiles)
                assemblyList.Add(ConvertToRelativePath(directory, assembly));

            project.TestPackageConfig.AssemblyFiles.Clear();
            project.TestPackageConfig.AssemblyFiles.AddRange(assemblyList);

            project.ReportDirectory = ConvertToRelativePath(directory, project.ReportDirectory);
        }

        private string ConvertToRelativePath(string directory, string file)
        {
            if (fileSystem.IsPathRooted(file))
            {
                try
                {
                    var filePath = new FilePathAbsolute(file);
                    var directoryPath = new DirectoryPathAbsolute(directory);
                    return filePath.GetPathRelativeFrom(directoryPath).Path;
                }
                catch
                { }
            }
            return file;
        }
    }
}
