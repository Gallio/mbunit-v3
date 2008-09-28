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
using Microsoft.Win32;

namespace Gallio.Navigator
{
    /// <summary>
    /// Gallio Asynchronous Pluggable Protocol Handler implementation.
    /// </summary>
    [ComVisible(true)]
    [Guid(ProtocolGuid)]
    [ClassInterface(ClassInterfaceType.None)]
    public class GallioProtocolHandler : GallioNavigatorClient, IInternetProtocol, IInternetProtocolRoot, IInternetProtocolInfo
    {
        private const string ProtocolGuid = "829B8F35-9874-49db-880F-142C98EB36A1";
        private const string ProtocolCLSID = "{" + ProtocolGuid + "}";
        private const string ProtocolDescription = "gallio: Asynchronous Pluggable Protocol Handler";
        private const string CLSIDKeyName = "CLSID";

        private IInternetProtocolSink protocolSink;

        #region Registration

        [ComRegisterFunction]
        internal static void Register(Type type)
        {
            // Async pluggable protocol handler.
            using (RegistryKey handlerKey = OpenProtocolHandlerKey())
            {
                using (RegistryKey gallioKey = handlerKey.CreateSubKey(ProtocolScheme))
                {
                    gallioKey.SetValue(null, ProtocolDescription);
                    gallioKey.SetValue(CLSIDKeyName, ProtocolCLSID);
                }
            }

            // Application url protocol handler.
            string appPath = new Uri(typeof(Program).Assembly.CodeBase).LocalPath;

            using (RegistryKey applicationKey = Registry.ClassesRoot.CreateSubKey(ProtocolScheme))
            {
                using (RegistryKey defaultIconKey = applicationKey.CreateSubKey("DefaultIcon"))
                {
                    defaultIconKey.SetValue(null, appPath);
                }

                using (RegistryKey shellKey = applicationKey.CreateSubKey("shell"))
                {
                    using (RegistryKey openKey = shellKey.CreateSubKey("open"))
                    {
                        using (RegistryKey commandKey = openKey.CreateSubKey("command"))
                        {
                            commandKey.SetValue(null, string.Concat("\"", appPath, "\" \"%1\""));
                        }
                    }
                }

                applicationKey.SetValue(null, ProtocolDescription);
                applicationKey.SetValue("URL Protocol", "");
            }
        }

        [ComUnregisterFunction]
        internal static void Unregister(Type type)
        {
            using (RegistryKey handlerKey = OpenProtocolHandlerKey())
            {
                DeleteSubKeyTree(handlerKey, ProtocolScheme);
            }

            DeleteSubKeyTree(Registry.ClassesRoot, ProtocolScheme);
        }

        private static RegistryKey OpenProtocolHandlerKey()
        {
            return Registry.ClassesRoot.OpenSubKey(@"PROTOCOLS\Handler", true);
        }

        private static void DeleteSubKeyTree(RegistryKey key, string subKey)
        {
            try
            {
                key.DeleteSubKeyTree(subKey);
            }
            catch (ArgumentException)
            {
                // Eat exception in case subkey does not exist.
            }
        }

        #endregion

        #region IInternetProtocol

        int IInternetProtocol.Read(IntPtr pv, uint cb, out uint pcbRead)
        {
            pcbRead = 0;
            return NativeConstants.S_FALSE;
        }

        int IInternetProtocol.Seek(long dlibMove, uint dwOrigin, out ulong plibNewPosition)
        {
            plibNewPosition = 0;
            return NativeConstants.E_FAIL;
        }

        int IInternetProtocol.LockRequest(uint dwOptions)
        {
            return NativeConstants.S_OK;
        }

        int IInternetProtocol.UnlockRequest()
        {
            return NativeConstants.S_OK;
        }

        #endregion
        #region IInternetProtocolRoot

        int IInternetProtocolRoot.Start(string szUrl, IInternetProtocolSink protocolSink, IInternetBindInfo bindInfo, uint grfPI, uint dwReserved)
        {
            return Start(szUrl, protocolSink, bindInfo, grfPI, dwReserved);
        }

        int IInternetProtocol.Start(string szUrl, IInternetProtocolSink protocolSink, IInternetBindInfo bindInfo, uint grfPI, uint dwReserved)
        {
            return Start(szUrl, protocolSink, bindInfo, grfPI, dwReserved);
        }

        private int Start(string szUrl, IInternetProtocolSink protocolSink, IInternetBindInfo bindInfo, uint grfPI, uint dwReserved)
        {
            this.protocolSink = protocolSink;
            try
            {
                int hr;
                if (ProcessCommandUrl(szUrl))
                {
                    // Abort the request by indicating that there is no data.
                    hr = NativeConstants.INET_E_DATA_NOT_AVAILABLE;
                }
                else
                {
                    hr = NativeConstants.INET_E_INVALID_URL;
                }

                protocolSink.ReportResult(hr, 0, null);
                return hr;
            }
            finally
            {
                this.protocolSink = null;
            }
        }

        int IInternetProtocolRoot.Continue(ref PROTOCOLDATA protocolData)
        {
            return Continue(ref protocolData);
        }

        int IInternetProtocol.Continue(ref PROTOCOLDATA protocolData)
        {
            return Continue(ref protocolData);
        }

        private int Continue(ref PROTOCOLDATA protocolData)
        {
            return NativeConstants.S_OK;
        }

        int IInternetProtocolRoot.Abort(int hrReason, uint dwOptions)
        {
            return Abort(hrReason, dwOptions);
        }

        int IInternetProtocol.Abort(int hrReason, uint dwOptions)
        {
            return Abort(hrReason, dwOptions);
        }

        private int Abort(int hrReason, uint dwOptions)
        {
            if (protocolSink != null)
                protocolSink.ReportResult(hrReason, 0, null);
            return NativeConstants.S_OK;
        }

        int IInternetProtocolRoot.Terminate(uint dwOptions)
        {
            return Terminate(dwOptions);
        }

        int IInternetProtocol.Terminate(uint dwOptions)
        {
            return Terminate(dwOptions);
        }

        private int Terminate(uint dwOptions)
        {
            protocolSink = null;
            return NativeConstants.S_OK;
        }

        int IInternetProtocolRoot.Suspend()
        {
            return Suspend();
        }

        int IInternetProtocol.Suspend()
        {
            return Suspend();
        }

        private int Suspend()
        {
            return NativeConstants.E_NOTIMPL;
        }

        int IInternetProtocolRoot.Resume()
        {
            return Resume();
        }

        int IInternetProtocol.Resume()
        {
            return Resume();
        }

        private int Resume()
        {
            return NativeConstants.E_NOTIMPL;
        }

        #endregion
        #region IInternetProtocolInfo

        int IInternetProtocolInfo.CombineUrl(string pwzBaseUrl, string pwzRelativeUrl, uint dwCombineFlags, out string pwzResult, uint cchResult, out uint pcchResult, uint dwReserved)
        {
            pwzResult = null;
            pcchResult = 0;
            return NativeConstants.INET_E_DEFAULT_ACTION;
        }

        int IInternetProtocolInfo.CompareUrl(string pwzUrl1, string pwzUrl2, uint dwCompareFlags)
        {
            return NativeConstants.INET_E_DEFAULT_ACTION;
        }

        int IInternetProtocolInfo.ParseUrl(string pwzUrl, PARSEACTION ParseAction, uint dwParseFlags, out string pwzResult, uint cchResult, out uint pcchResult, uint dwReserved)
        {
            pwzResult = null;
            pcchResult = 0;
            return NativeConstants.INET_E_DEFAULT_ACTION;
        }

        int IInternetProtocolInfo.QueryInfo(string pwzUrl, QUERYOPTION OueryOption, uint dwQueryFlags, IntPtr pBuffer, uint cbBuffer, ref uint pcbBuf, uint dwReserved)
        {
            pcbBuf = 0;
            return NativeConstants.INET_E_DEFAULT_ACTION;
        }
        #endregion

        /// <inheritdoc />
        protected override bool HandleNavigateToCommand(string path, int lineNumber, int columnNumber)
        {
            protocolSink.ReportProgress(0, "Navigating to source file.");
            return base.HandleNavigateToCommand(path, lineNumber, columnNumber);
        }
    }
}
