// Copyright 2005-2008 Gallio Project - http://www.gallio.org/
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
using Microsoft.VisualStudio.TestTools.Common;
using Microsoft.VisualStudio.TestTools.Vsip;
using Gallio.VisualStudio.Shell;
using EnvDTE;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.Shell;
using Gallio.VisualStudio.Shell.ToolWindows;
using Microsoft.VisualStudio.Shell.Interop;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.Exceptions;
using Gallio.VisualStudio.Tip.UI;

namespace Gallio.VisualStudio.Tip
{
    /// <summary>
    /// The Gallio test UI provider.
    /// </summary>
    [ComVisible(true)]
    public class GallioTuip : ITuip, SGallioTestService
    {
        private readonly TipShellExtension ext;

        public GallioTuip(TipShellExtension ext)
        {
            if (ext == null)
                throw new ArgumentNullException("ext");

            this.ext = ext;
        }

        public IRunConfigurationCustomEditor RunConfigurationEditor
        {
            get
            {
                return null;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="uiBlob"></param>
        /// <param name="test"></param>
        public void InvokeEditor(UIBlob uiBlob, ITestElement test)
        {
            GallioTestElement gallioTest = test as GallioTestElement;
            if (gallioTest != null)
            {
                // TODO: Use the test location information to navigate to the appropriate
                // source file and line number.
                // eg. ext.Shell.DTE.ItemOperations.OpenFile(...);
            }
        }

        /// <summary>
        /// The next window ID.
        /// </summary>
        private int nextWindowId = 1;

        /// <summary>
        /// Map of window ID by test ID.
        /// </summary>
        private Dictionary<TestResultId, int> windowMap = new Dictionary<TestResultId, int>();

        /// <summary>
        /// Invokes the test result viewer for the specified Gallio test.
        /// </summary>
        /// <param name="result"></param>
        public void InvokeResultViewer(TestResultMessage result)
        {
            GallioTestResult gallioResult = result as GallioTestResult;
            
            if (gallioResult != null)
            {
                int windowId;

                if (!windowMap.TryGetValue(gallioResult.Id, out windowId))
                {
                    windowId = nextWindowId++;
                }

                GallioToolWindow window = FindToolWindow(windowId, true);

                if ((window == null) || (window.Frame == null))
                {
                    throw new EqtException(Properties.Resources.ResultWindowCouldNotBeCreated);
                }

                windowMap[result.Id] = windowId;
                window.SetContent(new ResultViewer(gallioResult));
                window.Caption = result.TestName;
                window.Show();
            }
        }

        /// <summary>
        /// Closes the test result viewer for the specified Gallio result.
        /// </summary>
        /// <param name="result"></param>
        public void CloseResultViewer(TestResultMessage result)
        {
            GallioTestResult gallioResult = result as GallioTestResult;

            if ((gallioResult != null) &&
                windowMap.ContainsKey(gallioResult.Id))
            {
                int windowId = windowMap[gallioResult.Id];
                GallioToolWindow window = FindToolWindow(windowId, false);

                if (window != null)
                {
                    window.Close();
                }
            }
        }

        /// <summary>
        /// Gets the tool window corresponding to the specified type and ID.
        /// </summary>
        /// <param name="windowId">The tool window ID.</param>
        /// <param name="createIfNotFound">If true, the tool window is created if it does not exist.</param>
        /// <returns></returns>
        private GallioToolWindow FindToolWindow(int windowId, bool createIfNotFound)
        {
            return ext.Shell.Package.FindToolWindow(typeof(GallioToolWindow), windowId, createIfNotFound) as GallioToolWindow;
        }




        public bool IsTestPropertiesReadOnly(ITestElement test)
        {
            return true;
        }

        public void UpdateTestCustomProperty(ITestElement test, string propertyToChange)
        {
        }

        public void UpdateTestProperty(ITestElement test, System.ComponentModel.PropertyDescriptor propertyToChange)
        {
        }
    }
}
