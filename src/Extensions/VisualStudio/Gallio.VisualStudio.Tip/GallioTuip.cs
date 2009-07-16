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
using System.Runtime.InteropServices;
using System.Windows.Forms;
using EnvDTE;
using Gallio.UI.ErrorReporting;
using Gallio.VisualStudio.Tip.UI;
using Microsoft.VisualStudio.TestTools.Common;
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
            if (!TipShellExtension.IsInitialized)
                return;

            GallioTestElement gallioTest = test as GallioTestElement;

            if (gallioTest != null)
            {
                if (gallioTest.Path == null)
                {
                    ErrorDialog.Show(NativeWindow.FromHandle((IntPtr) ext.Shell.DTE.MainWindow.HWnd),
                        Properties.Resources.UnknownTestCodeLocationCaption, 
                        Properties.Resources.UnknownTestCodeLocation,
                        "");
                }
                else
                {
                    Window window = ext.Shell.DTE.OpenFile(EnvDTE.Constants.vsViewKindCode, gallioTest.Path);

                    TextSelection selection = window.Selection as TextSelection;
                    if (gallioTest.Line != 0)
                    {
                        if (selection != null)
                            selection.MoveToLineAndOffset(gallioTest.Line, Math.Max(1, gallioTest.Column), false);
                    }

                    window.Activate();
                }
            }
        }

        /// <summary>
        /// Invokes the test result viewer for the specified Gallio test.
        /// </summary>
        /// <param name="result">The result of the unit test.</param>
        public void InvokeResultViewer(TestResultMessage result)
        {
            if (!TipShellExtension.IsInitialized)
                return;

            GallioTestResult gallioResult = result as GallioTestResult;
            if (gallioResult != null)
            {
                TestResultWindow window = new TestResultWindow(gallioResult);
                ext.Shell.WindowManager.OpenToolWindow(GetWindowId(gallioResult), window);
            }
        }

        /// <summary>
        /// Closes the test result viewer for the specified Gallio result.
        /// </summary>
        /// <param name="result">The test result message.</param>
        public void CloseResultViewer(TestResultMessage result)
        {
            if (!TipShellExtension.IsInitialized)
                return;

            GallioTestResult gallioResult = result as GallioTestResult;
            if (gallioResult != null)
            {
                ext.Shell.WindowManager.CloseToolWindow(GetWindowId(gallioResult));
            }
        }

        private static string GetWindowId(GallioTestResult result)
        {
            return "Gallio.VisualStudio.Tip.TestResult:" + result.Id.TestId;
        }

        /// <summary>
        /// Returns true if the properties of the test are read only.
        /// </summary>
        /// <param name="test">The test.</param>
        /// <returns>True if the properties are read-only.</returns>
        public bool IsTestPropertiesReadOnly(ITestElement test)
        {
            return true;
        }
        
        /// <summary>
        /// Updates a custom test property.
        /// </summary>
        /// <param name="test">The test.</param>
        /// <param name="propertyToChange">The property to change.</param>
        public void UpdateTestCustomProperty(ITestElement test, string propertyToChange)
        {
        }

        /// <summary>
        /// Updates a test property.
        /// </summary>
        /// <param name="test">The test.</param>
        /// <param name="propertyToChange">The property to change.</param>
        public void UpdateTestProperty(ITestElement test, System.ComponentModel.PropertyDescriptor propertyToChange)
        {
        }
    }
}
