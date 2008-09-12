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
using System.ComponentModel;
using Microsoft.VisualStudio.TestTools.Common;
using Microsoft.VisualStudio.TestTools.Vsip;

namespace Gallio.VisualStudio.Tip
{
    /// <summary>
    /// Proxies the <see cref="ITuip" /> interface over to the actual implementation
    /// after initializing the loader.
    /// </summary>
    public class GallioTuipProxy : ITuip, SGallioTestService
    {
        private readonly ITuip target;

        public GallioTuipProxy(IServiceProvider serviceProvider)
        {
            target = ProxyHelper.GetTargetFactory().CreateTuip(serviceProvider);
        }

        public void InvokeEditor(UIBlob uiBlob, ITestElement test)
        {
            target.InvokeEditor(uiBlob, test);
        }

        public void InvokeResultViewer(TestResultMessage result)
        {
            target.InvokeResultViewer(result);
        }

        public void CloseResultViewer(TestResultMessage result)
        {
            target.CloseResultViewer(result);
        }

        public void UpdateTestProperty(ITestElement test, PropertyDescriptor propertyToChange)
        {
            target.UpdateTestProperty(test, propertyToChange);
        }

        public void UpdateTestCustomProperty(ITestElement test, string propertyToChange)
        {
            target.UpdateTestCustomProperty(test, propertyToChange);
        }

        public bool IsTestPropertiesReadOnly(ITestElement test)
        {
            return target.IsTestPropertiesReadOnly(test);
        }

        public IRunConfigurationCustomEditor RunConfigurationEditor
        {
            get { return target.RunConfigurationEditor; }
        }
    }
}
