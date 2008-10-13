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
using System.Runtime.InteropServices;
using Gallio.Navigator.Native;

namespace Gallio.Navigator
{
    /// <summary>
    /// ActiveX control for navigation.
    /// Also usable as a general-purpose COM component.
    /// </summary>
    [ComVisible(true)]
    [Guid("0DAA4E63-51CB-4ddf-988D-F1CBCE74B3E3")]
    [ClassInterface(ClassInterfaceType.None)]
    public class GallioNavigator : GallioNavigatorClient, IGallioNavigator, IObjectSafety
    {
        private bool safeForScripting = true;
        private bool safeForInitialization = true;

        /// <inheritdoc />
        public bool NavigateTo(string path, int lineNumber, int columnNumber)
        {
            ThrowIfNotSafeForScripting();

            return Navigator.NavigateTo(path, lineNumber, columnNumber);
        }

        private void ThrowIfNotSafeForScripting()
        {
            if (! safeForScripting)
                throw new InvalidOperationException("Interface has not been set safe for scripting.");
        }

        #region IObjectSafety implementation

        int IObjectSafety.GetInterfaceSafetyOptions(ref Guid riid, ref int pdwSupportedOptions, ref int pdwEnabledOptions)
        {
            pdwSupportedOptions = NativeConstants.INTERFACESAFE_FOR_UNTRUSTED_CALLER | NativeConstants.INTERFACESAFE_FOR_UNTRUSTED_DATA;

            if (riid == NativeConstants.IID_IDispatch || riid == NativeConstants.IID_IDispatchEx)
            {
                pdwEnabledOptions = safeForScripting ? NativeConstants.INTERFACESAFE_FOR_UNTRUSTED_CALLER : 0;
                return NativeConstants.S_OK;
            }

            if (riid == NativeConstants.IID_IPersistStorage || riid == NativeConstants.IID_IPersistStream || riid == NativeConstants.IID_IPersistPropertyBag)
            {
                pdwEnabledOptions = safeForInitialization ? NativeConstants.INTERFACESAFE_FOR_UNTRUSTED_DATA : 0;
                return NativeConstants.S_OK;
            }

            pdwEnabledOptions = 0;
            return NativeConstants.E_NOINTERFACE;
        }

        int IObjectSafety.SetInterfaceSafetyOptions(ref Guid riid, int dwOptionSetMask, int dwEnabledOptions)
        {
            if (riid == NativeConstants.IID_IDispatch || riid == NativeConstants.IID_IDispatchEx)
            {
                if ((dwEnabledOptions & dwOptionSetMask & ~ NativeConstants.INTERFACESAFE_FOR_UNTRUSTED_CALLER) != 0)
                    return NativeConstants.E_FAIL;

                safeForScripting = (dwEnabledOptions & dwOptionSetMask) != 0;
                return NativeConstants.S_OK;
            }

            if (riid == NativeConstants.IID_IPersistStorage || riid == NativeConstants.IID_IPersistStream || riid == NativeConstants.IID_IPersistPropertyBag)
            {
                if ((dwEnabledOptions & dwOptionSetMask & ~NativeConstants.INTERFACESAFE_FOR_UNTRUSTED_DATA) != 0)
                    return NativeConstants.E_FAIL;

                safeForInitialization = (dwEnabledOptions & dwOptionSetMask) != 0;
                return NativeConstants.S_OK;
            }

            return NativeConstants.E_NOINTERFACE;
        }

        #endregion
    }
}
