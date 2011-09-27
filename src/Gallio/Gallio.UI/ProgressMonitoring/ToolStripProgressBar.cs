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
using System.Windows.Forms;

namespace Gallio.UI.ProgressMonitoring
{
    ///<summary>
    /// Impl of a tool strip progress bar for Gallio progress monitoring.
    ///</summary>
    public class ToolStripProgressBar : System.Windows.Forms.ToolStripProgressBar
    {
        ///<summary>
        /// Total work to do.
        ///</summary>
        public double TotalWork 
        {
            set 
            {
                if (ProgressBar == null)
                    return;

                if (value == 0) 
                {
                    Maximum = 0;
                    return;
                }

                if (!double.IsNaN(value))
                {
                    Style = ProgressBarStyle.Continuous;
                    Maximum = Convert.ToInt32(value);
                } 
                else 
                {
                    Style = ProgressBarStyle.Marquee;
                }
            }
        }

        ///<summary>
        /// Total work to do.
        ///</summary>
        public double CompletedWork
        {
            set 
            {
                if (ProgressBar == null)
                    return;

                Value =  Convert.ToInt32(value);
            }
        }
    }
}
