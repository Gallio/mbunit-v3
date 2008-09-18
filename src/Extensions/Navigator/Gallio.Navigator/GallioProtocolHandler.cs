using System;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Web;
using Gallio.Navigator.Native;
using Microsoft.Win32;

namespace Gallio.Navigator
{
    [ComVisible(true)]
    [Guid(ProtocolGuid)]
    [ClassInterface(ClassInterfaceType.None)]
    public class GallioProtocolHandler : GallioNavigatorClient, IInternetProtocol, IInternetProtocolRoot, IInternetProtocolInfo
    {
        private const string ProtocolGuid = "829B8F35-9874-49db-880F-142C98EB36A1";
        private const string ProtocolCLSID = "{" + ProtocolGuid + "}";
        private const string ProtocolScheme = "gallio";
        private const string ProtocolDescription = "gallio: Asynchronous Pluggable Protocol Handler";
        private const string CLSIDKeyName = "CLSID";

        private const string NavigateToCommandName = "navigateTo";

        private IInternetProtocolSink protocolSink;

        #region Registration

        [ComRegisterFunction]
        internal static void Register(Type type)
        {
            using (RegistryKey handlerKey = OpenProtocolHandlerKey())
            {
                RegistryKey gallioKey = handlerKey.CreateSubKey(ProtocolScheme);
                gallioKey.SetValue(null, ProtocolDescription);
                gallioKey.SetValue(CLSIDKeyName, ProtocolCLSID);
            }
        }

        [ComUnregisterFunction]
        internal static void Unregister(Type type)
        {
            using (RegistryKey handlerKey = OpenProtocolHandlerKey())
            {
                try
                {
                    handlerKey.DeleteSubKeyTree(ProtocolScheme);
                }
                catch (ArgumentException)
                {
                    // Eat exception in case subkey does not exist.
                }
            }
        }

        private static RegistryKey OpenProtocolHandlerKey()
        {
            return Registry.ClassesRoot.OpenSubKey(@"PROTOCOLS\Handler", true);
        }

        #endregion

        #region IInternetProtocol

        public int Read(IntPtr pv, uint cb, out uint pcbRead)
        {
            pcbRead = 0;
            return Constants.S_FALSE;
        }

        public int Seek(long dlibMove, uint dwOrigin, out ulong plibNewPosition)
        {
            plibNewPosition = 0;
            return Constants.E_FAIL;
        }

        public int LockRequest(uint dwOptions)
        {
            return Constants.S_OK;
        }

        public int UnlockRequest()
        {
            return Constants.S_OK;
        }

        #endregion
        #region IInternetProtocolRoot

        public int Start(string szUrl, IInternetProtocolSink protocolSink, IInternetBindInfo bindInfo, uint grfPI, uint dwReserved)
        {
            int hr = Constants.INET_E_INVALID_URL;
            try
            {
                Uri uri = new Uri(szUrl);
                if (uri.Scheme == ProtocolScheme)
                {
                    string commandName = uri.AbsolutePath;
                    NameValueCollection args = HttpUtility.ParseQueryString(uri.Query);

                    switch (commandName)
                    {
                        case NavigateToCommandName:
                            string path = args["path"];
                            int lineNumber = GetValueOrDefault(args, "line", 0);
                            int columnNumber = GetValueOrDefault(args, "column", 0);

                            protocolSink.ReportProgress(0, "Navigating to source file.");
                            Navigator.NavigateTo(path, lineNumber, columnNumber);

                            // Abort the request by indicating that there is no data.
                            hr = Constants.INET_E_DATA_NOT_AVAILABLE;
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                // We want to report a result no matter what, even if a serious exception occurs.
                // Otherwise IE hangs.
                Debug.WriteLine(String.Format("Gallio could not perform requested service.", ex));
            }

            protocolSink.ReportResult(hr, 0, null);
            return hr;
        }

        public int Continue(ref PROTOCOLDATA protocolData)
        {
            return Constants.S_OK;
        }

        public int Abort(int hrReason, uint dwOptions)
        {
            if (protocolSink != null)
                protocolSink.ReportResult(hrReason, 0, null);
            return Constants.S_OK;
        }

        public int Terminate(uint dwOptions)
        {
            protocolSink = null;
            return Constants.S_OK;
        }

        public int Suspend()
        {
            return Constants.E_NOTIMPL;
        }

        public int Resume()
        {
            return Constants.E_NOTIMPL;
        }

        #endregion
        #region IInternetProtocolInfo

        public int CombineUrl(string pwzBaseUrl, string pwzRelativeUrl, uint dwCombineFlags, out string pwzResult, uint cchResult, out uint pcchResult, uint dwReserved)
        {
            pwzResult = null;
            pcchResult = 0;
            return Constants.INET_E_DEFAULT_ACTION;
        }

        public int CompareUrl(string pwzUrl1, string pwzUrl2, uint dwCompareFlags)
        {
            return Constants.INET_E_DEFAULT_ACTION;
        }

        public int ParseUrl(string pwzUrl, PARSEACTION ParseAction, uint dwParseFlags, out string pwzResult, uint cchResult, out uint pcchResult, uint dwReserved)
        {
            pwzResult = null;
            pcchResult = 0;
            return Constants.INET_E_DEFAULT_ACTION;
        }

        public int QueryInfo(string pwzUrl, QUERYOPTION OueryOption, uint dwQueryFlags, IntPtr pBuffer, uint cbBuffer, ref uint pcbBuf, uint dwReserved)
        {
            pcbBuf = 0;
            return Constants.INET_E_DEFAULT_ACTION;
        }
        #endregion

        private static int GetValueOrDefault(NameValueCollection collection, string key, int defaultValue)
        {
            string value = collection[key];
            if (value == null)
                return defaultValue;

            return int.Parse(value, NumberStyles.None, CultureInfo.InvariantCulture);
        }
    }
}
