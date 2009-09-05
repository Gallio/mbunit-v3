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
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using Gallio.Common.Markup;
using Gallio.Navigator.Native;
using Microsoft.Win32;

namespace Gallio.Navigator
{
    /// <summary>
    /// Gallio Asynchronous Pluggable Protocol Handler implementation.
    /// </summary>
    /// <remarks>
    /// <para>
    /// In addition to the other navigation commands offered by the navigator application,
    /// the protocol handler supports a command called "openAttachment" which is used to
    /// open a local file attachment within IE.  The command is specifically intended to
    /// circumvent the Local Machine Zone Lockdown that prevents HTML reports stored on
    /// the local filesystem that use the Mark of the Web from linking to attachments.
    /// As a security precaution, we verify that the path of the requested attachment file is
    /// within a Gallio report directory.
    /// </para>
    /// </remarks>
    [ComVisible(true)]
    [Guid(ProtocolGuid)]
    [ClassInterface(ClassInterfaceType.None)]
    public class GallioProtocolHandler : GallioNavigatorClient, IInternetProtocol, IInternetProtocolRoot, IInternetProtocolInfo, IInternetProtocolEx
    {
        private const string ProtocolGuid = "829B8F35-9874-49db-880F-142C98EB36A1";
        private const string ProtocolCLSID = "{" + ProtocolGuid + "}";
        private const string ProtocolDescription = "gallio: Asynchronous Pluggable Protocol Handler";
        private const string CLSIDKeyName = "CLSID";

        private IInternetProtocolSink currentProtocolSink;
        private string currentUrl;
        private BINDF currentBindFlags;

        private int dataLockCount;
        private string dataPath;
        private MemoryStream data;
        private string dataMimeType;

        #region Registration

        [ComRegisterFunction]
        internal static void Register(Type type)
        {
            // Async pluggable protocol handler.
            using (RegistryKey handlerKey = OpenProtocolHandlerKey())
            {
                using (RegistryKey gallioKey = handlerKey.CreateSubKey(GallioNavigatorCommand.ProtocolScheme))
                {
                    gallioKey.SetValue(null, ProtocolDescription);
                    gallioKey.SetValue(CLSIDKeyName, ProtocolCLSID);
                }
            }

            // Application url protocol handler.
            string appPath = new Uri(typeof(Program).Assembly.CodeBase).LocalPath;

            using (RegistryKey applicationKey = Registry.ClassesRoot.CreateSubKey(GallioNavigatorCommand.ProtocolScheme))
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
                DeleteSubKeyTree(handlerKey, GallioNavigatorCommand.ProtocolScheme);
            }

            DeleteSubKeyTree(Registry.ClassesRoot, GallioNavigatorCommand.ProtocolScheme);
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

        #region IInternetProtocolEx

        int IInternetProtocolEx.StartEx(IUri uri, IInternetProtocolSink protocolSink, IInternetBindInfo bindInfo, PI_FLAGS grfPI, uint dwReserved)
        {
            string szUrl;
            if (uri.GetAbsoluteUri(out szUrl) != NativeConstants.S_OK)
                return NativeConstants.INET_E_INVALID_URL;

            return Start(szUrl, protocolSink, bindInfo, grfPI, dwReserved);
        }

        #endregion

        #region IInternetProtocol

        int IInternetProtocol.Read(IntPtr pv, uint cb, out uint pcbRead)
        {
            return Read(pv, cb, out pcbRead);
        }

        int IInternetProtocolEx.Read(IntPtr pv, uint cb, out uint pcbRead)
        {
            return Read(pv, cb, out pcbRead);
        }

        private int Read(IntPtr pv, uint cb, out uint pcbRead)
        {
            if (data == null || data.Position == data.Length)
            {
                pcbRead = 0;
                return NativeConstants.S_FALSE;
            }

            int length = (int) Math.Min(data.Length - data.Position, cb);
            pcbRead = (uint)length;

            if (length != 0)
            {
                Marshal.Copy(data.GetBuffer(), (int) data.Position, pv, length);
                data.Position += length;
            }

            return NativeConstants.S_OK;
        }

        int IInternetProtocol.Seek(long dlibMove, uint dwOrigin, out ulong plibNewPosition)
        {
            return Seek(dlibMove, dwOrigin, out plibNewPosition);
        }

        int IInternetProtocolEx.Seek(long dlibMove, uint dwOrigin, out ulong plibNewPosition)
        {
            return Seek(dlibMove, dwOrigin, out plibNewPosition);
        }

        private int Seek(long dlibMove, uint dwOrigin, out ulong plibNewPosition)
        {
            plibNewPosition = 0;
            return NativeConstants.E_FAIL;
        }

        int IInternetProtocol.LockRequest(uint dwOptions)
        {
            return LockRequest(dwOptions);
        }

        int IInternetProtocolEx.LockRequest(uint dwOptions)
        {
            return LockRequest(dwOptions);
        }

        private int LockRequest(uint dwOptions)
        {
            LockData();
            return NativeConstants.S_OK;
        }

        int IInternetProtocol.UnlockRequest()
        {
            return UnlockRequest();
        }

        int IInternetProtocolEx.UnlockRequest()
        {
            return UnlockRequest();
        }

        private int UnlockRequest()
        {
            UnlockData();
            return NativeConstants.S_OK;
        }

        #endregion
        #region IInternetProtocolRoot

        int IInternetProtocolRoot.Start(string szUrl, IInternetProtocolSink protocolSink, IInternetBindInfo bindInfo, PI_FLAGS grfPI, uint dwReserved)
        {
            return Start(szUrl, protocolSink, bindInfo, grfPI, dwReserved);
        }

        int IInternetProtocol.Start(string szUrl, IInternetProtocolSink protocolSink, IInternetBindInfo bindInfo, PI_FLAGS grfPI, uint dwReserved)
        {
            return Start(szUrl, protocolSink, bindInfo, grfPI, dwReserved);
        }

        int IInternetProtocolEx.Start(string szUrl, IInternetProtocolSink protocolSink, IInternetBindInfo bindInfo, PI_FLAGS grfPI, uint dwReserved)
        {
            return Start(szUrl, protocolSink, bindInfo, grfPI, dwReserved);
        }

        private int Start(string szUrl, IInternetProtocolSink protocolSink, IInternetBindInfo bindInfo, PI_FLAGS grfPI, uint dwReserved)
        {
            ClearData();

            currentProtocolSink = protocolSink;
            currentUrl = szUrl;

            BINDINFO bindInfoData = new BINDINFO();
            bindInfoData.cbSize = (uint)Marshal.SizeOf(bindInfoData);
            bindInfo.GetBindInfo(out currentBindFlags, ref bindInfoData);

            if ((grfPI & PI_FLAGS.PI_FORCE_ASYNC) != 0)
            {
                PROTOCOLDATA protocolData = new PROTOCOLDATA();
                protocolData.grfFlags = PI_FLAGS.PI_FORCE_ASYNC;
                protocolSink.Switch(ref protocolData);
                return NativeConstants.E_PENDING;
            }
            else
            {
                return DoBind();
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

        int IInternetProtocolEx.Continue(ref PROTOCOLDATA protocolData)
        {
            return Continue(ref protocolData);
        }

        private int Continue(ref PROTOCOLDATA protocolData)
        {
            return DoBind();
        }

        private int DoBind()
        {
            currentProtocolSink.ReportProgress(BINDSTATUS.BINDSTATUS_FINDINGRESOURCE, currentUrl);
            currentProtocolSink.ReportProgress(BINDSTATUS.BINDSTATUS_CONNECTING, "Gallio Navigator.");

            bool success = false;
            try
            {
                GallioNavigatorCommand command = GallioNavigatorCommand.ParseUri(currentUrl);
                if (command != null)
                {

                    if (command.Name == "openAttachment")
                    {
                        string path = command.Arguments["path"];
                        if (!string.IsNullOrEmpty(path))
                        {
                            currentProtocolSink.ReportProgress(BINDSTATUS.BINDSTATUS_SENDINGREQUEST, Path.GetFileName(path));
                            success = StartOpenAttachmentCommand(path);
                        }
                    }
                    else
                    {
                        currentProtocolSink.ReportProgress(BINDSTATUS.BINDSTATUS_SENDINGREQUEST, command.Name);
                        success = command.Execute(Navigator);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(String.Format("Failed to process Uri '{0}' due to an exception: {1}.", currentUrl, ex));
            }

            LockData();

            int hr;
            if (success)
            {
                if (data != null && (currentBindFlags & BINDF.BINDF_NOWRITECACHE) == 0)
                {
                    uint dataLength = (uint)data.Length;

                    // Provide mime type.
                    if (dataMimeType != null)
                    {
                        currentProtocolSink.ReportProgress(BINDSTATUS.BINDSTATUS_VERIFIEDMIMETYPEAVAILABLE, dataMimeType);
                    }

                    // Determine whether we need to provide the path to the local file.
                    if ((currentBindFlags & BINDF.BINDF_NEEDFILE) != 0 && dataPath != null)
                    {
#if false // Not needed at the moment since the file we provide does not need to be in the cache.
          // Keeping this code in case we decide to provide dynamic content later.
                        StringBuilder cacheFilePathBuilder = new StringBuilder(NativeConstants.MAX_PATH);
                        string extension = Path.GetExtension(dataPath);
                        if (extension.Length != 0)
                            extension = extension.Substring(1); // strip leading '.'

                        NativeMethods.CreateUrlCacheEntry(currentUrl, dataLength, extension, cacheFilePathBuilder, 0);
                        string cacheFilePath = cacheFilePathBuilder.ToString();

                        DateTime now = DateTime.Now;
                        var nowTime = new System.Runtime.InteropServices.ComTypes.FILETIME()
                        {
                            dwHighDateTime = (int) (now.Ticks >> 32),
                            dwLowDateTime = (int) now.Ticks
                        };

                        const string headers = "HTTP/1.0 200 OK\r\n\r\n";
                        File.Copy(dataPath, cacheFilePath, true);
                        NativeMethods.CommitUrlCacheEntry(currentUrl, cacheFilePath,
                            nowTime,
                            nowTime,
                            NativeConstants.NORMAL_CACHE_ENTRY,
                            headers, (uint)headers.Length,
                            extension, null);

                        currentProtocolSink.ReportProgress(BINDSTATUS.BINDSTATUS_CACHEFILENAMEAVAILABLE, cacheFilePath);
#else
                        currentProtocolSink.ReportProgress(BINDSTATUS.BINDSTATUS_CACHEFILENAMEAVAILABLE, dataPath);
#endif
                    }

                    // Report all data available.
                    currentProtocolSink.ReportData(BSCF.BSCF_FIRSTDATANOTIFICATION | BSCF.BSCF_LASTDATANOTIFICATION | BSCF.BSCF_DATAFULLYAVAILABLE, dataLength, dataLength);

                    hr = NativeConstants.S_OK;
                }
                else
                {
                    // Aborts the navigation.
                    hr = NativeConstants.INET_E_DATA_NOT_AVAILABLE;
                }
            }
            else
            {
                // Reports an invalid Url.
                hr = NativeConstants.INET_E_INVALID_URL;
            }

            currentProtocolSink.ReportResult(hr, 0, null);
            currentProtocolSink = null;

            UnlockData();
            return hr;
        }

        int IInternetProtocolRoot.Abort(int hrReason, uint dwOptions)
        {
            return Abort(hrReason, dwOptions);
        }

        int IInternetProtocol.Abort(int hrReason, uint dwOptions)
        {
            return Abort(hrReason, dwOptions);
        }

        int IInternetProtocolEx.Abort(int hrReason, uint dwOptions)
        {
            return Abort(hrReason, dwOptions);
        }

        private int Abort(int hrReason, uint dwOptions)
        {
            if (currentProtocolSink != null)
                currentProtocolSink.ReportResult(hrReason, 0, null);

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

        int IInternetProtocolEx.Terminate(uint dwOptions)
        {
            return Terminate(dwOptions);
        }

        private int Terminate(uint dwOptions)
        {
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

        int IInternetProtocolEx.Suspend()
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

        int IInternetProtocolEx.Resume()
        {
            return Resume();
        }

        private int Resume()
        {
            return NativeConstants.E_NOTIMPL;
        }

        #endregion
        #region IInternetProtocolInfo

        int IInternetProtocolInfo.CombineUrl(string pwzBaseUrl, string pwzRelativeUrl, uint dwCombineFlags, IntPtr pwzResult, uint cchResult, out uint pcchResult, uint dwReserved)
        {
            pcchResult = (uint) pwzRelativeUrl.Length;

            if (pwzRelativeUrl.Length > cchResult)
                return NativeConstants.S_FALSE; // buffer too small

            WriteLPWStr(pwzRelativeUrl, pwzResult);
            return NativeConstants.S_OK;
        }

        int IInternetProtocolInfo.CompareUrl(string pwzUrl1, string pwzUrl2, uint dwCompareFlags)
        {
            return pwzUrl1 == pwzUrl2 ? NativeConstants.S_OK : NativeConstants.S_FALSE;
        }

        int IInternetProtocolInfo.ParseUrl(string pwzUrl, PARSEACTION ParseAction, uint dwParseFlags, IntPtr pwzResult, uint cchResult, out uint pcchResult, uint dwReserved)
        {
            string result = DoParseUrl(pwzUrl, ParseAction);
            if (result != null)
            {
                pcchResult = (uint) result.Length;

                if (result.Length > cchResult)
                    return NativeConstants.S_FALSE; // buffer too small

                WriteLPWStr(result, pwzResult);
                return NativeConstants.S_OK;
            }

            pcchResult = 0;
            return NativeConstants.INET_E_DEFAULT_ACTION;
        }

        private string DoParseUrl(string pwzUrl, PARSEACTION ParseAction)
        {
            switch (ParseAction)
            {
                case PARSEACTION.PARSE_CANONICALIZE:
                    return pwzUrl;
                case PARSEACTION.PARSE_SECURITY_URL:
                    return "gallio:localhost+" + (int) URLZONE.URLZONE_INTERNET;
                case PARSEACTION.PARSE_SECURITY_DOMAIN:
                    return "gallio:localhost";
                default:
                    return null;
            }
        }

        int IInternetProtocolInfo.QueryInfo(string pwzUrl, QUERYOPTION QueryOption, uint dwQueryFlags, IntPtr pBuffer, uint cbBuffer, ref uint pcbBuf, uint dwReserved)
        {
            return NativeConstants.INET_E_DEFAULT_ACTION;
        }

        private static void WriteLPWStr(string value, IntPtr targetPtr)
        {
            Marshal.Copy(value.ToCharArray(), 0, targetPtr, value.Length);
            Marshal.WriteInt16(targetPtr, value.Length * 2, 0);
        }

        #endregion

        private bool StartOpenAttachmentCommand(string path)
        {
            if (! IsValidAttachmentPath(path))
                return false;

            SetDataPath(path);
            return true;
        }

        private static bool IsValidAttachmentPath(string path)
        {
            if (! Path.IsPathRooted(path))
                return false;

            path = Path.GetFullPath(path);
            if (! File.Exists(path))
                return false;

            string directory = Path.GetDirectoryName(path);
            if (directory == null)
                return false;

            for (;;)
            {
                string parentDirectory = Path.GetDirectoryName(directory);
                if (parentDirectory == null)
                    return false;

                string possibleReportName = Path.GetFileName(directory);
                string possibleReportFileNamePattern = possibleReportName + ".*";

                // Ensure that the path is relative to a likely Gallio report file.
                string[] possibleReportFilePaths = Directory.GetFiles(parentDirectory, possibleReportFileNamePattern);
                foreach (string possibleReportFilePath in possibleReportFilePaths)
                {
                    if (File.ReadAllText(possibleReportFilePath).IndexOf("Gallio", StringComparison.OrdinalIgnoreCase) >= 0)
                        return true;
                }

                directory = parentDirectory;
            }
        }

        private void SetDataPath(string path)
        {
            byte[] bytes = File.ReadAllBytes(path);
            data = new MemoryStream(bytes, 0, bytes.Length, false, true);
            dataPath = path;
            dataMimeType = MimeTypes.GetMimeTypeByExtension(Path.GetExtension(path));
        }

        private void LockData()
        {
            dataLockCount += 1;
        }

        private void UnlockData()
        {
            dataLockCount -= 1;
            if (dataLockCount == 0)
                ClearData();
        }

        private void ClearData()
        {
            dataLockCount = 0;
            data = null;
            dataPath = null;
            dataMimeType = null;
        }
    }
}
