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
using System.IO;
using System.Text;
using Gallio.Common.IO;

namespace Gallio.Common.Policies
{
    /// <summary>
    /// Describes a policy for obtaining paths of special resources such as
    /// temporary directories and user profile settings files.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The policy is broken down into partitions based on their common purpose.  The idea
    /// is that Gallio will try to keep related resources together within a directory
    /// named after the partition.  As an additional safeguard, all paths returned by this
    /// policy will be grouped within a folder called "Gallio".
    /// </para>
    /// </remarks>
    public class SpecialPathPolicy
    {
        private const int MaxTries = 10;
        private readonly string partition;
        private readonly string partitionPath;

        private SpecialPathPolicy(string partition)
        {
            this.partition = partition;

            partitionPath = Path.Combine("Gallio", FileUtils.EncodeFileName(partition));
        }

        /// <summary>
        /// Gets the name of the partition this policy describes.
        /// </summary>
        public string Partition
        {
            get { return partition; }
        }

        /// <summary>
        /// Gets the special path policy for a partition whose name is derived from the type's full name.
        /// </summary>
        /// <typeparam name="TPartition">The type used to define the name of the partition.</typeparam>
        /// <returns>The special path policy.</returns>
        public static SpecialPathPolicy For<TPartition>()
        {
            return new SpecialPathPolicy(typeof(TPartition).FullName);
        }

        /// <summary>
        /// Gets the special path policy for a specified partition.
        /// </summary>
        /// <param name="partition">The partition name.</param>
        /// <returns>The special path policy.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="partition"/> is null.</exception>
        public static SpecialPathPolicy For(string partition)
        {
            if (partition == null)
                throw new ArgumentNullException("partition");

            return new SpecialPathPolicy(partition);
        }

        /// <summary>
        /// Gets the temporary directory.
        /// </summary>
        /// <returns>The temporary directory.</returns>
        public DirectoryInfo GetTempDirectory()
        {
            return CreateDirectoryIfAbsent(Path.Combine(Path.GetTempPath(), partitionPath));
        }

        /// <summary>
        /// Creates a temporary directory with a unique name.
        /// </summary>
        /// <returns>The temporary directory.</returns>
        public DirectoryInfo CreateTempDirectoryWithUniqueName()
        {
            foreach (string path in GetTempPathsWithRandomNames(MaxTries))
            {
                DirectoryInfo directory = new DirectoryInfo(path);
                if (!directory.Exists)
                {
                    try
                    {
                        directory.Create();
                        directory.Refresh();
                        return directory;
                    }
                    catch
                    {
                        // Could be I/O error, access error, or other transient condition.
                        // We retry up to MaxTries times.
                    }
                }
            }

            throw new IOException(string.Format("Failed to create a temporary directory with a unique name."));
        }

        /// <summary>
        /// Creates a zero-byte temporary file with a unique name.
        /// </summary>
        /// <returns>The temporary file.</returns>
        public FileInfo CreateTempFileWithUniqueName()
        {
            foreach (string path in GetTempPathsWithRandomNames(MaxTries))
            {
                FileInfo file = new FileInfo(path);
                if (!file.Exists)
                {
                    try
                    {
                        file.Create().Close();
                        file.Refresh();
                        return file;
                    }
                    catch
                    {
                        // Could be I/O error, access error, or other transient condition.
                        // We retry up to MaxTries times.
                    }
                }
            }

            throw new IOException(string.Format("Failed to create a temporary file with a unique name."));
        }

        /// <summary>
        /// Gets the application data directory for the roaming user.
        /// </summary>
        /// <returns>The roaming user application data directory.</returns>
        public DirectoryInfo GetRoamingUserApplicationDataDirectory()
        {
            return CreateDirectoryIfAbsent(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), partitionPath));
        }

        /// <summary>
        /// Gets the application data directory for the local non-roaming user.
        /// </summary>
        /// <returns>The local non-roaming user application data directory.</returns>
        public DirectoryInfo GetLocalUserApplicationDataDirectory()
        {
            return CreateDirectoryIfAbsent(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), partitionPath));
        }

        /// <summary>
        /// Gets the application data directory shared by all users.
        /// </summary>
        /// <returns>The common application data directory.</returns>
        public DirectoryInfo GetCommonApplicationDataDirectory()
        {
            return CreateDirectoryIfAbsent(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), partitionPath));
        }

        private IEnumerable<string> GetTempPathsWithRandomNames(int maxCount)
        {
            DirectoryInfo tempDirectory = GetTempDirectory();
            while (maxCount-- > 0)
            {
                yield return Path.Combine(tempDirectory.FullName, Path.GetRandomFileName());
            }
        }

        private static DirectoryInfo CreateDirectoryIfAbsent(string path)
        {
            DirectoryInfo directory = new DirectoryInfo(path);
            if (!directory.Exists)
            {
                directory.Create();
                directory.Refresh();
            }
            return directory;
        }
    }
}
