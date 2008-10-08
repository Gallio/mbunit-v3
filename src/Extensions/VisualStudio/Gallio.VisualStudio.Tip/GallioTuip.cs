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
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using EnvDTE;
using Gallio.Reflection;
using Gallio.VisualStudio.Shell;
using Gallio.VisualStudio.Shell.UI;
using Gallio.VisualStudio.Tip.UI;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.TestTools.Common;
using Microsoft.VisualStudio.TestTools.Exceptions;
using Microsoft.VisualStudio.TestTools.Vsip;

namespace Gallio.VisualStudio.Tip
{
    /// <summary>
    /// The Gallio test UI provider.
    /// </summary>
    [ComVisible(true)]
    public class GallioTuip : ITuip, SGallioTestService
    {
        private readonly TipShellExtension ext;
        
        /// <summary>
        /// Constructs a Gallio test UI provider.
        /// </summary>
        /// <param name="ext">The Gallio shell extension.</param>
        public GallioTuip(TipShellExtension ext)
        {
            if (ext == null)
                throw new ArgumentNullException("ext");

            this.ext = ext;
        }
        
        /// <summary>
        /// User Control to show in the Run Config dialog tab for this Test Type. 
        /// Returning null signifies this no special editor.
        /// </summary>
        public IRunConfigurationCustomEditor RunConfigurationEditor
        {
            get
            {
                return null;
            }
        }

        /// <summary>
        /// Invokes the editor for the specified Gallio test.
        /// </summary>
        /// <param name="uiBlob">Identifies the Project/Item blob to be displayed.</param>
        /// <param name="test">The test that the editor is being invoked for.</param>
        public void InvokeEditor(UIBlob uiBlob, ITestElement test)
        {
            GallioTestElement gallioTest = test as GallioTestElement;

            if (gallioTest != null)
            {
                if (gallioTest.CodeLocation == CodeLocation.Unknown)
                {
                    MessageBox.Show(
                        Properties.Resources.UnknownTestCodeLocation, 
                        Properties.Resources.UnknownTestCodeLocationCaption, 
                        MessageBoxButtons.OK, 
                        MessageBoxIcon.Error);
                }
                else
                {
                    ext.Shell.DTE.ItemOperations.OpenFile(
                        gallioTest.CodeLocation.Path, 
                        EnvDTE.Constants.vsDocumentKindText);
                }
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
        /// <param name="result">The result of the unit test.</param>
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
                window.SetControl(new ResultViewer(gallioResult, ext.Shell));
                window.Caption = String.Format("{0} [{1}]", result.TestName, Properties.Resources.Results);
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="test"></param>
        /// <returns></returns>
        public bool IsTestPropertiesReadOnly(ITestElement test)
        {
            return true;
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="test"></param>
        /// <param name="propertyToChange"></param>
        public void UpdateTestCustomProperty(ITestElement test, string propertyToChange)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="test"></param>
        /// <param name="propertyToChange"></param>
        public void UpdateTestProperty(ITestElement test, System.ComponentModel.PropertyDescriptor propertyToChange)
        {
        }
    }
}
