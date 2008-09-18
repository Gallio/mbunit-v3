using System;
using System.Runtime.InteropServices;
using Gallio.Navigator.Native;

namespace Gallio.Navigator
{
    /// <summary>
    /// ActiveX control for navigation.
    /// Also usable as a general-purpose COM control.
    /// </summary>
    /// <remarks>
    /// Be mindful of security!  This class could potentially be used by any web page
    /// or program that allows access to ActiveX.
    /// </remarks>
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
            pdwSupportedOptions = Constants.INTERFACESAFE_FOR_UNTRUSTED_CALLER | Constants.INTERFACESAFE_FOR_UNTRUSTED_DATA;

            if (riid == Constants.IID_IDispatch || riid == Constants.IID_IDispatchEx)
            {
                pdwEnabledOptions = safeForScripting ? Constants.INTERFACESAFE_FOR_UNTRUSTED_CALLER : 0;
                return Constants.S_OK;
            }

            if (riid == Constants.IID_IPersistStorage || riid == Constants.IID_IPersistStream || riid == Constants.IID_IPersistPropertyBag)
            {
                pdwEnabledOptions = safeForInitialization ? Constants.INTERFACESAFE_FOR_UNTRUSTED_DATA : 0;
                return Constants.S_OK;
            }

            pdwEnabledOptions = 0;
            return Constants.E_NOINTERFACE;
        }

        int IObjectSafety.SetInterfaceSafetyOptions(ref Guid riid, int dwOptionSetMask, int dwEnabledOptions)
        {
            if (riid == Constants.IID_IDispatch || riid == Constants.IID_IDispatchEx)
            {
                if ((dwEnabledOptions & dwOptionSetMask & ~ Constants.INTERFACESAFE_FOR_UNTRUSTED_CALLER) != 0)
                    return Constants.E_FAIL;

                safeForScripting = (dwEnabledOptions & dwOptionSetMask) != 0;
                return Constants.S_OK;
            }

            if (riid == Constants.IID_IPersistStorage || riid == Constants.IID_IPersistStream || riid == Constants.IID_IPersistPropertyBag)
            {
                if ((dwEnabledOptions & dwOptionSetMask & ~Constants.INTERFACESAFE_FOR_UNTRUSTED_DATA) != 0)
                    return Constants.E_FAIL;

                safeForInitialization = (dwEnabledOptions & dwOptionSetMask) != 0;
                return Constants.S_OK;
            }

            return Constants.E_NOINTERFACE;
        }

        #endregion
    }
}
