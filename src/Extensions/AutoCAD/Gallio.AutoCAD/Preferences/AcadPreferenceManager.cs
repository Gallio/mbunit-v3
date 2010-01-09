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
using Gallio.Common.Collections;
using Gallio.Runtime.Preferences;

namespace Gallio.AutoCAD.Preferences
{
    /// <summary>
    /// Default implementation of <see cref="IAcadPreferenceManager"/>.
    /// </summary>
    public class AcadPreferenceManager : IAcadPreferenceManager
    {
        private const string DefaultCommandLineArguments = null;
        private const StartupAction DefaultStartupAction = StartupAction.StartMostRecentlyUsed;
        private const string DefaultUserSpecifiedExecutable = null;
        private const string DefaultWorkingDirectory = null;

        private readonly IPreferenceManager preferenceManager;

        /// <summary>
        /// Creates a new <see cref="AcadPreferenceManager"/>.
        /// </summary>
        /// <param name="preferenceManager">A preference manager.</param>
        public AcadPreferenceManager(IPreferenceManager preferenceManager)
        {
            if (preferenceManager == null)
                throw new ArgumentNullException("preferenceManager");
            this.preferenceManager = preferenceManager;
        }

        private IPreferenceSet LocalUserPreferences
        {
            get { return preferenceManager.LocalUserPreferences["Gallio.AutoCAD"]; }
        }

        /// <inheritdoc/>
        public string CommandLineArguments
        {
            get
            {
                return LocalUserPreferences.Read(reader => reader.GetSetting(
                    new Key<string>("CommandLineArguments"), DefaultCommandLineArguments));
            }
            set
            {
                LocalUserPreferences.Write(writer => writer.SetSetting(
                    new Key<string>("CommandLineArguments"), value));
            }
        }

        /// <inheritdoc/>
        public StartupAction StartupAction
        {
            get
            {
                return LocalUserPreferences.Read(reader => reader.GetSetting(
                    new Key<StartupAction>("StartupAction"), DefaultStartupAction));
            }
            set
            {
                LocalUserPreferences.Write(writer => writer.SetSetting(
                    new Key<StartupAction>("StartupAction"), value));
            }
        }

        /// <inheritdoc/>
        public string UserSpecifiedExecutable
        {
            get
            {
                return LocalUserPreferences.Read(reader => reader.GetSetting(
                    new Key<string>("UserSpecifiedExecutable"), DefaultUserSpecifiedExecutable));
            }
            set
            {
                LocalUserPreferences.Write(writer => writer.SetSetting(
                    new Key<string>("UserSpecifiedExecutable"), value));
            }
        }

        /// <inheritdoc/>
        public string WorkingDirectory
        {
            get
            {
                return LocalUserPreferences.Read(reader => reader.GetSetting(
                    new Key<string>("WorkingDirectory"), DefaultWorkingDirectory));
            }
            set
            {
                LocalUserPreferences.Write(writer => writer.SetSetting(
                    new Key<string>("WorkingDirectory"), value));
            }
        }
    }
}
