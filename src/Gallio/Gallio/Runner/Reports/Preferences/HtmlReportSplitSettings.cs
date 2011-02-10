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
using System.Text;
using Gallio.Runtime.Preferences;
using Gallio.Common.Collections;

namespace Gallio.Runner.Reports.Preferences
{
    /// <summary>
    /// Settings for the HTML report automatic splitting.
    /// </summary>
    public sealed class HtmlReportSplitSettings
    {
        private const bool DefaultEnabled = true;
        private const int DefaultPageSize = 1000;
        private int pageSize;

        /// <summary>
        /// Indicates whether the splitting option is enabled.
        /// </summary>
        public bool Enabled
        { 
            get; 
            set; 
        }

        /// <summary>
        /// Gets or sets the number of test steps per report page.
        /// </summary>
        public int PageSize
        {
            get
            {
                return pageSize;
            }

            set
            {
                if (value < 100)
                    throw new ArgumentOutOfRangeException("pageSize", "Must be greater than or equal to 100.");

                pageSize = value;
            }
        }

        /// <summary>
        /// Constructs default settings.
        /// </summary>
        public HtmlReportSplitSettings()
        {
            Defaults();
        }

        /// <summary>
        /// Sets the default values.
        /// </summary>
        public void Defaults()
        {
            Enabled = DefaultEnabled;
            pageSize = DefaultPageSize;
        }

        /// <summary>
        /// Reads the settings from the specified preference set.
        /// </summary>
        /// <param name="preferenceSet">The preference set which contains the data to read.</param>
        /// <returns>The resulting report settings.</returns>
        public static HtmlReportSplitSettings ReadFrom(IPreferenceSet preferenceSet)
        {
            if (preferenceSet == null)
                throw new ArgumentNullException("preferenceSet");

            return new HtmlReportSplitSettings
            {
                Enabled = preferenceSet.Read(reader => reader.GetSetting(new Key<bool>("HtmlReportSplitEnabled"), DefaultEnabled)),
                PageSize = preferenceSet.Read(reader => reader.GetSetting(new Key<int>("HtmlReportSplitPageSize"), DefaultPageSize)),
            };
        }

        /// <summary>
        /// Writes the contents of the settings to the specified preference set.
        /// </summary>
        /// <param name="preferenceSet">The preference set to write data in.</param>
        public void WriteTo(IPreferenceSet preferenceSet)
        {
            if (preferenceSet == null)
                throw new ArgumentNullException("preferenceSet");

            preferenceSet.Write(writer => writer.SetSetting(new Key<bool>("HtmlReportSplitEnabled"), Enabled));
            preferenceSet.Write(writer => writer.SetSetting(new Key<int>("HtmlReportSplitPageSize"), pageSize));
        }
    }
}
