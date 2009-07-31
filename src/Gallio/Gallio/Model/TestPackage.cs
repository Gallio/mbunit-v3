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
using System.Collections.ObjectModel;
using System.IO;
using Gallio.Common.Collections;
using Gallio.Runtime.Debugging;
using Gallio.Runtime.Hosting;

namespace Gallio.Model
{
    /// <summary>
    /// A test package specifies the test files, assemblies, and configuration options
    /// that govern test execution.
    /// </summary>
    [Serializable]
    public class TestPackage
    {
        private readonly List<FileInfo> files;
        private readonly List<DirectoryInfo> hintDirectories;
        private readonly List<string> excludedFrameworkIds;
        private readonly PropertySet properties;

        private bool shadowCopy;
        private bool isShadowCopySpecified;
        private DebuggerSetup debuggerSetup;
        private bool isDebuggetSetupSpecified;
        private DirectoryInfo applicationBaseDirectory;
        private bool isApplicationBaseDirectorySpecified;
        private DirectoryInfo workingDirectory;
        private bool isWorkingDirectorySpecified;
        private string runtimeVersion;
        private bool isRuntimeVersionSpecified;

        /// <summary>
        /// Creates an empty test package.
        /// </summary>
        public TestPackage()
        {
            files = new List<FileInfo>();
            hintDirectories = new List<DirectoryInfo>();
            excludedFrameworkIds = new List<string>();
            properties = new PropertySet();
        }

        /// <summary>
        /// Gets the read-only list of test files.
        /// </summary>
        public IList<FileInfo> Files
        {
            get { return new ReadOnlyCollection<FileInfo>(files); }
        }

        /// <summary>
        /// Gets the read-only list of hint directories used to resolve test assemblies and other files.
        /// </summary>
        public IList<DirectoryInfo> HintDirectories
        {
            get { return new ReadOnlyCollection<DirectoryInfo>(hintDirectories); }
        }

        /// <summary>
        /// Gets the read-only list of test framework IDs that are to be excluded from the test
        /// exploration process.
        /// </summary>
        public IList<string> ExcludedFrameworkIds
        {
            get { return new ReadOnlyCollection<string>(excludedFrameworkIds); }
        }

        /// <summary>
        /// Gets a read-only collection of properties for the test runner.
        /// </summary>
        public PropertySet Properties
        {
            get { return properties.AsReadOnly(); }
        }

        /// <summary>
        /// Gets or sets whether assembly shadow copying is enabled.
        /// </summary>
        /// <value>True if shadow copying is enabled.  Default is <c>false</c>.</value>
        public bool ShadowCopy
        {
            get { return shadowCopy; }
            set
            {
                shadowCopy = value;
                isShadowCopySpecified = true;
            }
        }

        /// <summary>
        /// Returns true if <see cref="ShadowCopy" /> has been set explicitly.
        /// </summary>
        public bool IsShadowCopySpecified
        {
            get { return isShadowCopySpecified; }
        }

        /// <summary>
        /// Gets or sets the debugger setup options, or null if not debugging.
        /// </summary>
        /// <value>The debugger setup options.  Default is <c>null</c>.</value>
        public DebuggerSetup DebuggerSetup
        {
            get { return debuggerSetup; }
            set
            {
                debuggerSetup = value;
                isDebuggetSetupSpecified = true;
            }
        }

        /// <summary>
        /// Returns true if <see cref="DebuggerSetup" /> has been set explicitly.
        /// </summary>
        public bool IsDebuggerSetupSpecified
        {
            get { return isDebuggetSetupSpecified; }
        }

        /// <summary>
        /// Gets or sets the application base directory, or null to use the directory
        /// containing each group of test files or some other suitable default chosen by the test framework.
        /// </summary>
        /// <value>
        /// The application base directory.  Default is <c>null</c>.
        /// </value>
        public DirectoryInfo ApplicationBaseDirectory
        {
            get { return applicationBaseDirectory; }
            set
            {
                applicationBaseDirectory = value;
                isApplicationBaseDirectorySpecified = true;
            }
        }

        /// <summary>
        /// Returns true if <see cref="ApplicationBaseDirectory" /> has been set explicitly.
        /// </summary>
        public bool IsApplicationBaseDirectorySpecified
        {
            get { return isApplicationBaseDirectorySpecified; }
        }

        /// <summary>
        /// Gets or sets the working directory, or null to use the directory containing
        /// each group of test files or some other suitable default chosen by the test framework.
        /// </summary>
        /// <value>
        /// The working directory.  Default is <c>null</c>.
        /// </value>
        public DirectoryInfo WorkingDirectory
        {
            get { return workingDirectory; }
            set
            {
                workingDirectory = value;
                isWorkingDirectorySpecified = true;
            }
        }

        /// <summary>
        /// Returns true if <see cref="WorkingDirectory" /> has been set explicitly.
        /// </summary>
        public bool IsWorkingDirectorySpecified
        {
            get { return isWorkingDirectorySpecified; }
        }

        /// <summary>
        /// Gets or sets the .Net runtime version, or null to auto-detect.
        /// </summary>
        /// <value>The runtime version, eg. "v2.0.50727".  Default is <c>null</c>.</value>
        public string RuntimeVersion
        {
            get { return runtimeVersion; }
            set
            {
                runtimeVersion = value;
                isRuntimeVersionSpecified = true;
            }
        }

        /// <summary>
        /// Returns true if <see cref="RuntimeVersion" /> has been set explicitly.
        /// </summary>
        public bool IsRuntimeVersionSpecified
        {
            get { return isRuntimeVersionSpecified; }
        }

        /// <summary>
        /// Creates a copy of the test package.
        /// </summary>
        /// <returns>The new copy.</returns>
        public TestPackage Copy()
        {
            TestPackage copy = new TestPackage()
            {
                applicationBaseDirectory = applicationBaseDirectory,
                isApplicationBaseDirectorySpecified = isApplicationBaseDirectorySpecified,
                debuggerSetup = debuggerSetup,
                isDebuggetSetupSpecified = isDebuggetSetupSpecified,
                runtimeVersion = runtimeVersion,
                isRuntimeVersionSpecified = isRuntimeVersionSpecified,
                shadowCopy = shadowCopy,
                isShadowCopySpecified = isShadowCopySpecified,
                workingDirectory = workingDirectory,
                isWorkingDirectorySpecified = isWorkingDirectorySpecified
            };

            copy.excludedFrameworkIds.AddRange(excludedFrameworkIds);
            copy.files.AddRange(files);
            copy.hintDirectories.AddRange(hintDirectories);
            copy.properties.AddAll(properties);
            return copy;
        }

        /// <summary>
        /// Resets <see cref="ApplicationBaseDirectory"/> to its default value and sets <see cref="IsApplicationBaseDirectorySpecified" /> to false.
        /// </summary>
        public void ResetApplicationBaseDirectory()
        {
            applicationBaseDirectory = null;
            isApplicationBaseDirectorySpecified = false;
        }

        /// <summary>
        /// Resets <see cref="DebuggerSetup"/> to its default value and sets <see cref="IsDebuggerSetupSpecified" /> to false.
        /// </summary>
        public void ResetDebug()
        {
            debuggerSetup = null;
            isDebuggetSetupSpecified = false;
        }

        /// <summary>
        /// Resets <see cref="RuntimeVersion"/> to its default value and sets <see cref="IsRuntimeVersionSpecified" /> to false.
        /// </summary>
        public void ResetRuntimeVersion()
        {
            runtimeVersion = null;
            isRuntimeVersionSpecified = false;
        }

        /// <summary>
        /// Resets <see cref="ShadowCopy"/> to its default value and sets <see cref="IsShadowCopySpecified" /> to false.
        /// </summary>
        public void ResetShadowCopy()
        {
            shadowCopy = false;
            isShadowCopySpecified = false;
        }

        /// <summary>
        /// Resets <see cref="WorkingDirectory"/> to its default value and sets <see cref="IsWorkingDirectorySpecified" /> to false.
        /// </summary>
        public void ResetReportDirectory()
        {
            workingDirectory = null;
            isWorkingDirectorySpecified = false;
        }

        /// <summary>
        /// Clears the list of test files.
        /// </summary>
        public void ClearFiles()
        {
            files.Clear();
        }

        /// <summary>
        /// Adds a test file if it is not already in the test package.
        /// </summary>
        /// <param name="file">The file to add.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="file"/> is null.</exception>
        public void AddFile(FileInfo file)
        {
            if (file == null)
                throw new ArgumentNullException("file");

            if (FindFile(file) == null)
                files.Add(file);
        }

        /// <summary>
        /// Removes a test file.
        /// </summary>
        /// <param name="file">The file to remove.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="file"/> is null.</exception>
        public void RemoveFile(FileInfo file)
        {
            if (file == null)
                throw new ArgumentNullException("file");

            var existingFile = FindFile(file);
            if (existingFile != null)
                files.Remove(existingFile);
        }

        private FileInfo FindFile(FileInfo file)
        {
            return GenericCollectionUtils.Find(files, x => string.Compare(x.ToString(), file.ToString(), StringComparison.OrdinalIgnoreCase) == 0);
        }

        /// <summary>
        /// Clears the list of hint directories.
        /// </summary>
        public void ClearHintDirectories()
        {
            hintDirectories.Clear();
        }

        /// <summary>
        /// Adds a hint directory if it is not already in the test package.
        /// </summary>
        /// <param name="directory">The directory to add.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="directory"/> is null.</exception>
        public void AddHintDirectory(DirectoryInfo directory)
        {
            if (directory == null)
                throw new ArgumentNullException("directory");

            if (! GenericCollectionUtils.Exists(hintDirectories, x => x.ToString() == directory.ToString()))
                hintDirectories.Add(directory);
        }

        /// <summary>
        /// Removes a hint directory.
        /// </summary>
        /// <param name="directory">The directory to remove.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="directory"/> is null.</exception>
        public void RemoveHintDirectory(DirectoryInfo directory)
        {
            if (directory == null)
                throw new ArgumentNullException("directory");

            var existingDirectory = GenericCollectionUtils.Find(hintDirectories, x => x.ToString() == directory.ToString());
            hintDirectories.Remove(existingDirectory);
        }

        /// <summary>
        /// Clears the list of excluded framework ids.
        /// </summary>
        public void ClearExcludedFrameworkIds()
        {
            excludedFrameworkIds.Clear();
        }

        /// <summary>
        /// Adds an excluded framework id if it is not already in the test package.
        /// </summary>
        /// <param name="frameworkId">The framework id to add.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="frameworkId"/> is null.</exception>
        public void AddExcludedFrameworkId(string frameworkId)
        {
            if (frameworkId == null)
                throw new ArgumentNullException("frameworkId");

            if (!excludedFrameworkIds.Contains(frameworkId))
                excludedFrameworkIds.Add(frameworkId);
        }

        /// <summary>
        /// Removes an excluded framework id.
        /// </summary>
        /// <param name="frameworkId">The framework id to remove.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="frameworkId"/> is null.</exception>
        public void RemoveExcludedFrameworkId(string frameworkId)
        {
            if (frameworkId == null)
                throw new ArgumentNullException("frameworkId");

            excludedFrameworkIds.Remove(frameworkId);
        }

        /// <summary>
        /// Clears the collection of properties.
        /// </summary>
        public void ClearProperties()
        {
            properties.Clear();
        }

        /// <summary>
        /// Adds a property key/value pair.
        /// </summary>
        /// <param name="key">The property key.</param>
        /// <param name="value">The property value.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="key"/> or <paramref name="value"/> is null.</exception>
        /// <exception cref="InvalidOperationException">Thrown if <paramref name="key"/> is already in the property set.</exception>
        public void AddProperty(string key, string value)
        {
            properties.Add(key, value); // note: implicitly checks arguments
        }

        /// <summary>
        /// Removes a property key/value pair.
        /// </summary>
        /// <param name="key">The property key.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="key"/> is null.</exception>
        public void RemoveProperty(string key)
        {
            properties.Remove(key); // note: implicitly checks arguments
        }

        /// <summary>
        /// Creates a host setup based on the package properties.
        /// </summary>
        /// <returns>The host setup.</returns>
        public HostSetup CreateHostSetup()
        {
            var hostSetup = new HostSetup
            {
                DebuggerSetup = DebuggerSetup,
                ShadowCopy = ShadowCopy,
                ApplicationBaseDirectory = ApplicationBaseDirectory != null ? ApplicationBaseDirectory.FullName : null,
                WorkingDirectory = WorkingDirectory != null ? WorkingDirectory.FullName : null,
                RuntimeVersion = RuntimeVersion
            };

            return hostSetup;
        }

        /// <summary>
        /// Returns true if the framework with the specified id should be used to explore
        /// the contents of the test package.
        /// </summary>
        /// <param name="frameworkId">The framework id.</param>
        /// <returns>True if the framework is requested.</returns>
        public bool IsFrameworkRequested(string frameworkId)
        {
            return !excludedFrameworkIds.Contains(frameworkId);
        }

        /// <summary>
        /// Applies the settings of another test package as an overlay on top of this one.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Overrides scalar settings (such as <see cref="ApplicationBaseDirectory"/>) with those of
        /// the overlay when they are specified (such as when <see cref="IsApplicationBaseDirectorySpecified" /> is true).
        /// Merges aggregate settings (such as lists of files).
        /// </para>
        /// </remarks>
        /// <param name="overlay">The test package to overlay on top of this one.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="overlay"/> is null.</exception>
        public void ApplyOverlay(TestPackage overlay)
        {
            if (overlay == null)
                throw new ArgumentNullException("overlay");

            if (overlay.IsApplicationBaseDirectorySpecified)
                ApplicationBaseDirectory = overlay.ApplicationBaseDirectory;
            if (overlay.IsDebuggerSetupSpecified)
                DebuggerSetup = overlay.DebuggerSetup;
            if (overlay.IsRuntimeVersionSpecified)
                RuntimeVersion = overlay.RuntimeVersion;
            if (overlay.IsShadowCopySpecified)
                ShadowCopy = overlay.ShadowCopy;
            if (overlay.IsWorkingDirectorySpecified)
                WorkingDirectory = overlay.WorkingDirectory;

            GenericCollectionUtils.ForEach(overlay.Files, x => AddFile(x));
            GenericCollectionUtils.ForEach(overlay.HintDirectories, x => AddHintDirectory(x));
            GenericCollectionUtils.ForEach(overlay.ExcludedFrameworkIds, x => AddExcludedFrameworkId(x));
            GenericCollectionUtils.ForEach(overlay.Properties, x => properties[x.Key] = x.Value);
        }
    }
}
