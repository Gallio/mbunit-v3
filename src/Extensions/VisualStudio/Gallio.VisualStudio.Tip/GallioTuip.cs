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
            get { return null; }
        }

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

        public void InvokeResultViewer(TestResultMessage result)
        {
            GallioTestResult gallioResult = result as GallioTestResult;
            if (gallioResult != null)
            {
                // TODO: Should show a viewer with details about the test result.
                // This might require us to store additional details about the result, possibly in
                // a side-band channel with all of the report contents preserved and ready to
                // be rendered on demand.
            }
        }

        public void CloseResultViewer(TestResultMessage result)
        {
            // TODO
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
