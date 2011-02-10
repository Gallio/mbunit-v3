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
using Gallio.Common.Collections;
using Gallio.Runtime.Preferences;

namespace Gallio.Runner.Reports.Preferences
{
    /// <summary>
    /// Manager for user preferences about test reporting.
    /// </summary>
    public class ReportPreferenceManager
    {
        private readonly IPreferenceManager preferenceManager;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="preferenceManager">General-purpose preference manager.</param>
        public ReportPreferenceManager(IPreferenceManager preferenceManager)
        {
            this.preferenceManager = preferenceManager;
        }

        private IPreferenceSet PreferenceSet
        {
            get
            {
                return preferenceManager.LocalUserPreferences["Gallio.Reports"];
            }
        }

        /// <summary>
        /// Reads or writes the HTML report split settings.
        /// </summary>
        public HtmlReportSplitSettings HtmlReportSplitSettings
        {
            get
            {
                return HtmlReportSplitSettings.ReadFrom(PreferenceSet);
            }

            set
            {
                value.WriteTo(PreferenceSet);
            }
        }
    }
}
