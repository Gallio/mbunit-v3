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
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using EnvDTE;
using System.Runtime.InteropServices;
using Gallio.Navigator.Native;
using Gallio.Runtime.Logging;
using Gallio.UI.ErrorReporting;
using Gallio.VisualStudio.Interop;

namespace Gallio.Navigator
{
    /// <summary>
    /// Gallio navigator implementation.
    /// </summary>
    public class GallioNavigatorImpl : IGallioNavigator
    {
        private readonly IVisualStudioManager visualStudioManager;

        /// <summary>
        /// Creates a navigator.
        /// </summary>
        /// <param name="visualStudioManager">The visual studio manager.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="visualStudioManager"/> is null.</exception>
        public GallioNavigatorImpl(IVisualStudioManager visualStudioManager)
        {
            if (visualStudioManager == null)
                throw new ArgumentNullException("visualStudioManager");

            this.visualStudioManager = visualStudioManager;
        }

        /// <summary>
        /// Creates a navigator.
        /// </summary>
        public GallioNavigatorImpl()
            : this(VisualStudioManager.Instance)
        {
        }

        /// <inheritdoc />
        public bool NavigateTo(string path, int lineNumber, int columnNumber)
        {
            if (path == null)
                throw new ArgumentNullException("path");
            if (path.Length == 0)
                throw new ArgumentException("Path must not be empty.", "path");
            if (!Path.IsPathRooted(path))
                throw new ArgumentException("Path must be rooted.", "path");
            if (lineNumber < 0)
                throw new ArgumentOutOfRangeException("lineNumber");
            if (columnNumber < 0)
                throw new ArgumentOutOfRangeException("columnNumber");

            try
            {
                return NavigateToImpl(path, lineNumber, columnNumber);
            }
            catch (Exception ex)
            {
                ErrorDialog.Show(null, "Gallio Navigator", String.Format(
                    "Gallio could not navigate to: {0} ({1},{2}) because the file was not found or Visual Studio could not be controlled.\nPlease try again after launching Visual Studio manually and opening the appropriate solution.",
                    path, lineNumber, columnNumber), ex.ToString());
                return false;
            }
        }

        private bool NavigateToImpl(string path, int lineNumber, int columnNumber)
        {
            path = Path.GetFullPath(path);

            var logger = NullLogger.Instance;
            IVisualStudio visualStudio = visualStudioManager.GetVisualStudio(VisualStudioVersion.Any, true, logger);
            if (visualStudio == null)
                return false;

            visualStudio.Call(dte =>
            {
                Window window = OpenFile(dte, path);
                if (window == null)
                    window = FindFileInSolution(dte, path);

                TextSelection selection = window.Selection as TextSelection;
                if (lineNumber != 0)
                {
                    if (selection != null)
                        selection.MoveToLineAndOffset(lineNumber, Math.Max(1, columnNumber), false);
                }

                window.Activate();
                window.Visible = true;
            });

            visualStudio.BringToFront();

            return true;
        }

        private static Window OpenFile(DTE dte, string path)
        {
            try
            {
                if (!File.Exists(path))
                    return null;

                return dte.OpenFile(Constants.vsViewKindCode, path);
            }
            catch (COMException ex)
            {
                if (ex.ErrorCode != NativeConstants.STG_E_FILENOTFOUND)
                    throw;

                return null;
            }
        }

        private static Window FindFileInSolution(DTE dte, string path)
        {
            Solution solution = dte.Solution;

            if (! solution.IsOpen)
                    throw new ApplicationException("File not found and no solution is open to be searched.");

            List<string> searchPaths = new List<string>();
            searchPaths.Add(Path.GetDirectoryName(solution.FileName));

            foreach (Project project in FindAllProjects(solution))
            {
                try
                {
                    string projectFile = project.FileName;
                    if (! string.IsNullOrEmpty(projectFile))
                        searchPaths.Add(Path.GetDirectoryName(projectFile));
                }
                catch (COMException)
                {
                }
            }

            return FindFileInSearchPaths(dte, searchPaths, path);
        }

        private static IEnumerable<Project> FindAllProjects(Solution solution)
        {
            var projects = new List<Project>();

            foreach (Project project in solution.Projects)
            {
                if (project.Kind != Constants.vsProjectItemKindSolutionItems)
                    projects.Add(project);

                FindAllProjects(projects, project.ProjectItems);
            }

            return projects;
        }

        private static void FindAllProjects(IList<Project> projects, ProjectItems parent)
        {
            foreach (ProjectItem projectItem in parent)
            {
                Project project = projectItem.SubProject;
                if (project != null)
                {
                    if (project.Kind != EnvDTE.Constants.vsProjectItemKindSolutionItems)
                        projects.Add(project);

                    FindAllProjects(projects, project.ProjectItems);
                }
            }
        }

        private static Window FindFileInSearchPaths(DTE dte, IEnumerable<string> searchPaths, string path)
        {
            for (; ; )
            {
                path = RemoveLeadingSegment(path);
                if (path.Length == 0)
                    return null;

                foreach (string searchPath in searchPaths)
                {
                    Window window = OpenFile(dte, Path.Combine(searchPath, path));
                    if (window != null)
                        return window;
                }
            }
        }

        private static string RemoveLeadingSegment(string path)
        {
            int slash = path.IndexOfAny(new[] {Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar});
            if (slash < 0)
                return "";

            return path.Substring(slash + 1);
        }
    }
}
