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
using Gallio.Utilities;
using NDepend.Helpers.FileDirectoryPath;

namespace Gallio.Runner.Projects
{
    /// <summary>
    /// 
    /// </summary>
    public class ProjectUtils
    {
        /// <summary>
        /// Load a saved Gallio project file.
        /// </summary>
        /// <param name="projectLocation">The location of the project file.</param>
        /// <returns>A Gallio Project instance.</returns>
        /// <exception cref="ArgumentException">Thrown when the specified project location is invalid.</exception>
        public static Project LoadProject(string projectLocation)
        {
            // fail fast
            if (!File.Exists(projectLocation))
                throw new ArgumentException(String.Format("Project file {0} does not exist.", projectLocation));

            // deserialize project
            var project = XmlSerializationUtils.LoadFromXml<Project>(projectLocation);
            ConvertFromRelativePaths(project, Path.GetDirectoryName(projectLocation));
            return project;
        }

        private static void ConvertFromRelativePaths(Project project, string directory)
        {
            IList<string> assemblyList = new List<string>();
            foreach (string assembly in project.TestPackageConfig.AssemblyFiles)
            {
                string assemblyPath = assembly;
                if (!Path.IsPathRooted(assembly))
                {
                    try
                    {
                        FilePathRelative filePath = new FilePathRelative(assembly);
                        DirectoryPathAbsolute directoryPath = new DirectoryPathAbsolute(directory);
                        assemblyPath = filePath.GetAbsolutePathFrom(directoryPath).Path;
                    }
                    catch
                    {
                        assemblyPath = assembly;
                    }
                }
                if (File.Exists(assemblyPath))
                    assemblyList.Add(assemblyPath);
            }
            project.TestPackageConfig.AssemblyFiles.Clear();
            project.TestPackageConfig.AssemblyFiles.AddRange(assemblyList);
        }

        /// <summary>
        /// 
        /// </summary>
        public static void SaveProject(Project project, string projectLocation)
        {
            ConvertToRelativePaths(project, Path.GetDirectoryName(projectLocation));
            XmlSerializationUtils.SaveToXml(project, projectLocation);
            ConvertFromRelativePaths(project, Path.GetDirectoryName(projectLocation));
        }

        private static void ConvertToRelativePaths(Project project, string directory)
        {
            IList<string> assemblyList = new List<string>();
            foreach (string assembly in project.TestPackageConfig.AssemblyFiles)
            {
                if (Path.IsPathRooted(assembly))
                {
                    try
                    {
                        FilePathAbsolute filePath = new FilePathAbsolute(assembly);
                        DirectoryPathAbsolute directoryPath = new DirectoryPathAbsolute(directory);
                        assemblyList.Add(filePath.GetPathRelativeFrom(directoryPath).Path);
                    }
                    catch
                    {
                        assemblyList.Add(assembly);
                    }
                }
                else
                    assemblyList.Add(assembly);
            }
            project.TestPackageConfig.AssemblyFiles.Clear();
            project.TestPackageConfig.AssemblyFiles.AddRange(assemblyList);
        }
    }
}
