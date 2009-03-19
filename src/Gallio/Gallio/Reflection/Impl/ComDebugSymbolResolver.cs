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
#define USE_ISTREAM

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.SymbolStore;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Security.Permissions;
using System.Text;
using System.Threading;
using Microsoft.Win32.SafeHandles;
using STATSTG=System.Runtime.InteropServices.ComTypes.STATSTG;

namespace Gallio.Reflection.Impl
{
    /// <summary>
    /// Resolves debug symbols via the CorMetaDataDispenser COM class.
    /// </summary>
    /// <remarks author="jeff">
    /// <para>
    /// This code has been adapted from the CLR Managed Debugger sample from
    /// http://www.microsoft.com/downloads/details.aspx?FamilyID=38449a42-6b7a-4e28-80ce-c55645ab1310
    /// and from Mike Stall's PDB2Xml program sample available on
    /// his blog at: http://blogs.msdn.com/jmstall/archive/2005/08/25/pdb2xml.aspx.
    /// Many thanks to you!
    /// </para>
    /// <para>
    /// We do not use the ISymWrapper implementation of the symbol store interfaces because
    /// they throw COMExceptions whenever method metadata is not available.  This causes
    /// confusion for users because the Visual Studio Debugger Exception Assistant window
    /// may pop up dozens of times while loading symbol information.
    /// </para>
    /// </remarks>
    public class ComDebugSymbolResolver : IDebugSymbolResolver
    {
        private const int S_OK = 0;
        private const int E_PDB_NOT_FOUND = unchecked((int)0x806D0005);
        private const int E_PDB_NO_DEBUG_INFO = unchecked((int)0x806D0014);
        private const int E_PDB_DEBUG_INFO_NOT_IN_PDB = unchecked((int)0x806D0017);
        private const int E_FILE_NOT_FOUND = unchecked((int)0x80070002);
        private const int E_FAIL = unchecked((int)0x80004005);
        private const int STG_E_INVALIDFUNCTION = unchecked((int)0x80030001);
        private const int STG_E_CANTSAVE = unchecked((int)0x80030103);

        private readonly bool avoidLocks;
        private readonly Dictionary<string, ISymUnmanagedReader> symbolReaders = new Dictionary<string, ISymUnmanagedReader>();

        /// <summary>
        /// Creates a COM debug symbol resolver.
        /// </summary>
        /// <param name="avoidLocks">If true, avoids taking a lock on the PDB files but may use more memory or storage</param>
        public ComDebugSymbolResolver(bool avoidLocks)
        {
            this.avoidLocks = avoidLocks;
        }

        /// <inheritdoc />
        public void Dispose()
        {
            foreach (var symbolReader in symbolReaders.Values)
                Marshal.ReleaseComObject(symbolReader);

            symbolReaders.Clear();
        }

        /// <inheritdoc />
        public CodeLocation GetSourceLocationForMethod(string assemblyPath, int methodToken)
        {
            if (assemblyPath == null)
                throw new ArgumentNullException("assemblyPath");

            // The CorDebug APIs can only be used from an MTA thread.
            if (Thread.CurrentThread.GetApartmentState() == ApartmentState.MTA)
            {
                return GetSourceLocationForMethodInMTAThread(assemblyPath, methodToken);
            }
            else
            {
                ManualResetEvent signal = new ManualResetEvent(false);
                CodeLocation result = CodeLocation.Unknown;
                Exception asyncException = null;

                ThreadPool.UnsafeQueueUserWorkItem(state =>
                {
                    try
                    {
                        Debug.Assert(Thread.CurrentThread.GetApartmentState() == ApartmentState.MTA,
                            "ThreadPool threads should always be MTA.");

                        result = GetSourceLocationForMethodInMTAThread(assemblyPath, methodToken);
                    }
                    catch (Exception ex)
                    {
                        asyncException = ex;
                    }
                    finally
                    {
                        signal.Set();
                    }
                }, null);

                signal.WaitOne(1000, false);

                if (asyncException != null)
                    throw new InvalidOperationException("An exception occurred while reading debug symbols.", asyncException);

                return result;
            }
        }

        [SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.UnmanagedCode)]
        private CodeLocation GetSourceLocationForMethodInMTAThread(string assemblyPath, int methodToken)
        {
            ISymUnmanagedReader reader = GetSymbolReader(assemblyPath);
            if (reader == null)
                return CodeLocation.Unknown;

            ISymUnmanagedMethod method;
            int errorCode = reader.GetMethod(new SymbolToken(methodToken), out method);
            if (errorCode < 0)
            {
                if (errorCode == E_FAIL)
                    return CodeLocation.Unknown;
                Marshal.ThrowExceptionForHR(errorCode);
            }

            int seqPtCount;
            method.GetSequencePointCount(out seqPtCount);
            if (seqPtCount == 0)
                return CodeLocation.Unknown;

            ISymUnmanagedDocument[] docs = new ISymUnmanagedDocument[seqPtCount];
            int[] offsets = new int[seqPtCount];
            int[] lines = new int[seqPtCount];
            int[] columns = new int[seqPtCount];
            int[] endLines = new int[seqPtCount];
            int[] endColumns = new int[seqPtCount];

            int unused;
            method.GetSequencePoints(seqPtCount, out unused, offsets, docs, lines, columns, endLines, endColumns);

            // The first sequence point of a method sometimes refers to a very large invalid
            // line number and a column number of 0.  We ignore this sequence point and
            // look for the next one.
            for (int i = 0; i < seqPtCount; i++)
            {
                // Note: We omit the column number information because it is not useful.
                //       It will just refer to the column of the first line of the method,
                //       which already isn't sufficiently accurate to determine its point of definition.
                if (columns[i] != 0)
                    return new CodeLocation(GetDocumentUrl(docs[i]), lines[i], 0);
            }

            // Fallback on file information only.
            return new CodeLocation(GetDocumentUrl(docs[0]), 0, 0);
        }

        private ISymUnmanagedReader GetSymbolReader(string assemblyPath)
        {
            assemblyPath = Path.GetFullPath(assemblyPath);

            lock (symbolReaders)
            {
                ISymUnmanagedReader symbolReader;
                if (!symbolReaders.TryGetValue(assemblyPath, out symbolReader))
                {
                    symbolReader = CreateSymbolReader(assemblyPath, null, avoidLocks);
                    symbolReaders.Add(assemblyPath, symbolReader);
                }

                return symbolReader;
            }
        }

        private static string GetDocumentUrl(ISymUnmanagedDocument document)
        {
            int cchUrl;
            document.GetURL(0, out cchUrl, null);
            StringBuilder result = new StringBuilder(cchUrl);
            document.GetURL(cchUrl, out cchUrl, result);
            return result.ToString();
        }

        [SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.UnmanagedCode)]
        private static ISymUnmanagedReader CreateSymbolReader(string filename, string searchPath, bool avoidLocks)
        {
            // Guids for imported metadata interfaces.
            Guid CLSID_CorMetaDataDispenser = new Guid(0xe5cb7a31, 0x7512, 0x11d2, 0x89, 0xce, 0x00, 0x80, 0xc7, 0x92, 0xe5, 0xd8);
            Guid IID_IMetaDataImport = new Guid(0x7dac8207, 0xd3ae, 0x4c75, 0x9b, 0x67, 0x92, 0x80, 0x1a, 0x49, 0x7d, 0x44);
            Guid CLSID_CorSymBinder = new Guid("0A29FF9E-7F9C-4437-8B11-F424491E3931");

            // First create the Metadata dispenser.
            IMetaDataDispenser dispenser = (IMetaDataDispenser)Activator.CreateInstance(Type.GetTypeFromCLSID(CLSID_CorMetaDataDispenser));

            IntPtr importer = IntPtr.Zero;
            try
            {
                int errorCode = dispenser.OpenScope(filename, 0, ref IID_IMetaDataImport, out importer);
                if (errorCode < 0)
                {
                    if (errorCode == E_FAIL
                        || errorCode == E_FILE_NOT_FOUND)
                        return null;
                    Marshal.ThrowExceptionForHR(errorCode);
                }

                ISymUnmanagedBinder3 binder = (ISymUnmanagedBinder3)Activator.CreateInstance(Type.GetTypeFromCLSID(CLSID_CorSymBinder));
                ISymUnmanagedReader reader;

                errorCode = CreateSymbolReader(binder, importer, filename, searchPath, avoidLocks, out reader);

                if (errorCode < 0)
                {
                    if (errorCode == E_FAIL
                        || errorCode == E_PDB_DEBUG_INFO_NOT_IN_PDB
                        || errorCode == E_PDB_NO_DEBUG_INFO
                        || errorCode == E_PDB_NOT_FOUND)
                        return null;
                    Marshal.ThrowExceptionForHR(errorCode);
                }

                return reader;
            }
            finally
            {
                if (importer != IntPtr.Zero)
                    Marshal.Release(importer);
            }
        }

        private static int CreateSymbolReader(ISymUnmanagedBinder3 binder, IntPtr importer,
            string filename, string searchPath, bool avoidLocks, out ISymUnmanagedReader reader)
        {
            if (! avoidLocks)
                return binder.GetReaderForFile(importer, filename, searchPath, out reader);

            reader = null;

            string pdbFilename = Path.ChangeExtension(filename, ".pdb");
            if (!File.Exists(pdbFilename))
                return E_PDB_NOT_FOUND;

            try
            {
                byte[] bytes = File.ReadAllBytes(pdbFilename);

#if USE_ISTREAM
                IStream stream = new ComStream(new MemoryStream(bytes, 0, bytes.Length, false, true));
                return binder.GetReaderFromStream(importer, stream, out reader);
#else
                IDiaReadExeAtOffsetCallback callback = new DiaReadExeAtOffsetCallback(bytes);
                return binder.GetReaderFromCallback(importer, filename, searchPath,
                    CorSymSearchPolicyAttributes.AllowReferencePathAccess, callback, out reader);
#endif
            }
            catch (IOException)
            {
                return E_FAIL;
            }
        }

#if USE_ISTREAM
        private sealed class ComStream : IStream
        {
            private readonly MemoryStream stream;

            public ComStream(MemoryStream stream)
            {
                this.stream = stream;
            }

            int IStream.Read(IntPtr pv, int cb, out int pcbRead)
            {
                int length = (int) stream.Length;
                int position = (int) stream.Position;
                int count = Math.Min(length - position, cb);

                Marshal.Copy(stream.GetBuffer(), position, pv, count);
                stream.Position += count;
                pcbRead = count;

                return S_OK;
            }

            int IStream.Write(IntPtr pv, int cb, out int pcbWritten)
            {
                pcbWritten = 0;
                return STG_E_CANTSAVE;
            }

            int IStream.Seek(long dlibMove, STREAM_SEEK dwOrigin, IntPtr plibNewPosition)
            {
                SeekOrigin origin;
                switch (dwOrigin)
                {
                    case STREAM_SEEK.STREAM_SEEK_SET:
                        origin = SeekOrigin.Begin;
                        break;
                    case STREAM_SEEK.STREAM_SEEK_CUR:
                        origin = SeekOrigin.Current;
                        break;
                    case STREAM_SEEK.STREAM_SEEK_END:
                        origin = SeekOrigin.End;
                        break;
                    default:
                        return STG_E_INVALIDFUNCTION;
                }

                if (plibNewPosition != IntPtr.Zero)
                    Marshal.WriteInt64(plibNewPosition, stream.Seek(dlibMove, origin));
                return S_OK;
            }

            int IStream.SetSize(long libNewSize)
            {
                return STG_E_INVALIDFUNCTION;
            }

            int IStream.CopyTo(IStream pstm, long cb, IntPtr pcbRead, IntPtr pcbWritten)
            {
                return STG_E_INVALIDFUNCTION;
            }

            int IStream.Commit(int grfCommitFlags)
            {
                return S_OK;
            }

            int IStream.Revert()
            {
                return S_OK;
            }

            int IStream.LockRegion(long libOffset, long cb, int dwLockType)
            {
                return STG_E_INVALIDFUNCTION;
            }

            int IStream.UnlockRegion(long libOffset, long cb, int dwLockType)
            {
                return STG_E_INVALIDFUNCTION;
            }

            int IStream.Stat(out STATSTG pstatstg, int grfStatFlag)
            {
                pstatstg = new STATSTG();
                pstatstg.type = 2;
                pstatstg.cbSize = stream.Length;
                pstatstg.grfLocksSupported = 2;
                return S_OK;
            }

            int IStream.Clone(out IStream ppstm)
            {
                ppstm = new ComStream(new MemoryStream(stream.GetBuffer()));
                return S_OK;
            }
        }
#else
        private sealed class DiaReadExeAtOffsetCallback : IDiaReadExeAtOffsetCallback
        {
            private readonly byte[] bytes;

            public DiaReadExeAtOffsetCallback(byte[] bytes)
            {
                this.bytes = bytes;
            }

            int IDiaReadExeAtOffsetCallback.ReadExecutableAt(long fileOffset, int cbData, out int pcbData, IntPtr data)
            {
                if (fileOffset < 0 || cbData < 0 || fileOffset > bytes.Length)
                {
                    pcbData = 0;
                    return E_FAIL;
                }

                int offset = (int)fileOffset;
                int count = Math.Min(bytes.Length - offset, cbData);
                Marshal.Copy(bytes, offset, data, count);
                pcbData = count;

                return S_OK;
            }
        }
#endif

        [ComImport, Guid("809c652e-7396-11d2-9771-00a0c9b4d50c"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        private interface IMetaDataDispenser
        {
            // We need to be able to call OpenScope, which is the 2nd vtable slot.
            // Thus we need this one placeholder here to occupy the first slot..
            void DefineScope_Placeholder();

            // STDMETHOD(OpenScope)(                   // Return code.
            //     LPCWSTR     szScope,                // [in] The scope to open.
            //     DWORD       dwOpenFlags,            // [in] Open mode flags.
            //     REFIID      riid,                   // [in] The interface desired.
            //     IUnknown    **ppIUnk) PURE;         // [out] Return interface on success.
            [PreserveSig]
            int OpenScope([In, MarshalAs(UnmanagedType.LPWStr)] String szScope,
                [In] Int32 dwOpenFlags,
                [In] ref Guid riid,
                [Out] out IntPtr punk);

            // Don't need any other methods.
        }

        [ComImport, Guid("28AD3D43-B601-4d26-8A1B-25F9165AF9D7"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        private interface ISymUnmanagedBinder3
        {
            [PreserveSig]
            int GetReaderForFile(IntPtr importer,
                                      [MarshalAs(UnmanagedType.LPWStr)] String filename,
                                      [MarshalAs(UnmanagedType.LPWStr)] String SearchPath,
                                      [MarshalAs(UnmanagedType.Interface)] out ISymUnmanagedReader retVal);

            [PreserveSig]
            int GetReaderFromStream(IntPtr importer,
                                            [MarshalAs(UnmanagedType.Interface)] IStream stream,
                                            [MarshalAs(UnmanagedType.Interface)] out ISymUnmanagedReader retVal);

            [PreserveSig]
            int GetReaderForFile2(IntPtr importer,
                                      [MarshalAs(UnmanagedType.LPWStr)] String filename,
                                      [MarshalAs(UnmanagedType.LPWStr)] String SearchPath,
                                      [MarshalAs(UnmanagedType.U4)] CorSymSearchPolicyAttributes searchPolicy,
                                      [MarshalAs(UnmanagedType.Interface)] out ISymUnmanagedReader retVal);

            [PreserveSig]
            int GetReaderFromCallback(IntPtr importer,
                                      [MarshalAs(UnmanagedType.LPWStr)] String filename,
                                      [MarshalAs(UnmanagedType.LPWStr)] String SearchPath,
                                      [MarshalAs(UnmanagedType.U4)] CorSymSearchPolicyAttributes searchPolicy,
                                      [MarshalAs(UnmanagedType.Interface)] IDiaReadExeAtOffsetCallback callback,
                                      [MarshalAs(UnmanagedType.Interface)] out ISymUnmanagedReader retVal);
        }

        [ComImport, Guid("B4CE6286-2A6B-3712-A3B7-1EE1DAD467B5"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        private interface ISymUnmanagedReader
        {
            void GetDocument([MarshalAs(UnmanagedType.LPWStr)] String url,
                                  Guid language,
                                  Guid languageVendor,
                                  Guid documentType,
                                  [MarshalAs(UnmanagedType.Interface)] out ISymUnmanagedDocument retVal);

            void GetDocuments(int cDocs,
                                   out int pcDocs,
                                   [In, Out, MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 0)] ISymUnmanagedDocument[] pDocs);

            [PreserveSig]
            int GetUserEntryPoint(out SymbolToken EntryPoint);

            [PreserveSig]
            int GetMethod(SymbolToken methodToken,
                              [MarshalAs(UnmanagedType.Interface)] out ISymUnmanagedMethod retVal);

            [PreserveSig]
            int GetMethodByVersion(SymbolToken methodToken,
                                          int version,
                                          [MarshalAs(UnmanagedType.Interface)] out ISymUnmanagedMethod retVal);

            void GetVariables(SymbolToken parent,
                                int cVars,
                                out int pcVars,
                                [In, Out, MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 1)] ISymUnmanagedVariable[] vars);

            void GetGlobalVariables(int cVars,
                                        out int pcVars,
                                        [In, Out, MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 0)] ISymUnmanagedVariable[] vars);


            void GetMethodFromDocumentPosition(ISymUnmanagedDocument document,
                                                  int line,
                                                  int column,
                                                  [MarshalAs(UnmanagedType.Interface)] out ISymUnmanagedMethod retVal);

            void GetSymAttribute(SymbolToken parent,
                                    [MarshalAs(UnmanagedType.LPWStr)] String name,
                                    int sizeBuffer,
                                    out int lengthBuffer,
                                    [In, Out, MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 2)] byte[] buffer);

            void GetNamespaces(int cNameSpaces,
                                    out int pcNameSpaces,
                                    [In, Out, MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 0)] ISymUnmanagedNamespace[] namespaces);

            void Initialize(IntPtr importer,
                           [MarshalAs(UnmanagedType.LPWStr)] String filename,
                           [MarshalAs(UnmanagedType.LPWStr)] String searchPath,
                           IStream stream);

            void UpdateSymbolStore([MarshalAs(UnmanagedType.LPWStr)] String filename,
                                         IStream stream);

            void ReplaceSymbolStore([MarshalAs(UnmanagedType.LPWStr)] String filename,
                                          IStream stream);

            void GetSymbolStoreFileName(int cchName,
                                               out int pcchName,
                                               [MarshalAs(UnmanagedType.LPWStr)] StringBuilder szName);

            void GetMethodsFromDocumentPosition(ISymUnmanagedDocument document,
                                                          int line,
                                                          int column,
                                                          int cMethod,
                                                          out int pcMethod,
                                                          [In, Out, MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 3)] ISymUnmanagedMethod[] pRetVal);

            void GetDocumentVersion(ISymUnmanagedDocument pDoc,
                                          out int version,
                                          out Boolean pbCurrent);

            void GetMethodVersion(ISymUnmanagedMethod pMethod,
                                       out int version);
        }

        [ComImport, Guid("40DE4037-7C81-3E1E-B022-AE1ABFF2CA08"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        private interface ISymUnmanagedDocument
        {
            void GetURL(int cchUrl,
                           out int pcchUrl,
                           [MarshalAs(UnmanagedType.LPWStr)] StringBuilder szUrl);

            void GetDocumentType(ref Guid pRetVal);

            void GetLanguage(ref Guid pRetVal);

            void GetLanguageVendor(ref Guid pRetVal);

            void GetCheckSumAlgorithmId(ref Guid pRetVal);

            void GetCheckSum(int cData,
                                  out int pcData,
                                  [In, Out, MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 0)] byte[] data);

            void FindClosestLine(int line,
                                    out int pRetVal);

            void HasEmbeddedSource(out Boolean pRetVal);

            void GetSourceLength(out int pRetVal);

            void GetSourceRange(int startLine,
                                     int startColumn,
                                     int endLine,
                                     int endColumn,
                                     int cSourceBytes,
                                     out int pcSourceBytes,
                                     [In, Out, MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 4)] byte[] source);
        }

        [ComImport, Guid("B62B923C-B500-3158-A543-24F307A8B7E1"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        private interface ISymUnmanagedMethod
        {
            void GetToken(out SymbolToken pToken);
            void GetSequencePointCount(out int retVal);
            void GetRootScope([MarshalAs(UnmanagedType.Interface)] out ISymUnmanagedScope retVal);
            void GetScopeFromOffset(int offset, [MarshalAs(UnmanagedType.Interface)] out ISymUnmanagedScope retVal);
            void GetOffset(ISymUnmanagedDocument document,
                             int line,
                             int column,
                             out int retVal);
            void GetRanges(ISymUnmanagedDocument document,
                              int line,
                              int column,
                              int cRanges,
                              out int pcRanges,
                              [In, Out, MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 3)] int[] ranges);
            void GetParameters(int cParams,
                                  out int pcParams,
                                  [In, Out, MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 0)] ISymUnmanagedVariable[] parms);
            void GetNamespace([MarshalAs(UnmanagedType.Interface)] out ISymUnmanagedNamespace retVal);
            void GetSourceStartEnd(ISymUnmanagedDocument[] docs,
                                      [In, Out, MarshalAs(UnmanagedType.LPArray)] int[] lines,
                                      [In, Out, MarshalAs(UnmanagedType.LPArray)] int[] columns,
                                      out Boolean retVal);
            void GetSequencePoints(int cPoints,
                                      out int pcPoints,
                                      [In, Out, MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 0)] int[] offsets,
                                      [In, Out, MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 0)] ISymUnmanagedDocument[] documents,
                                      [In, Out, MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 0)] int[] lines,
                                      [In, Out, MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 0)] int[] columns,
                                      [In, Out, MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 0)] int[] endLines,
                                      [In, Out, MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 0)] int[] endColumns);
        }

        [ComImport, Guid("0DFF7289-54F8-11d3-BD28-0000F80849BD"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        private interface ISymUnmanagedNamespace
        {
            // Omitted for brevity
        }

        [ComImport, Guid("9F60EEBE-2D9A-3F7C-BF58-80BC991C60BB"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        private interface ISymUnmanagedVariable
        {
            // Omitted for brevity
        }

        [ComImport, Guid("68005D0F-B8E0-3B01-84D5-A11A94154942"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        private interface ISymUnmanagedScope
        {
            // Omitted for brevity
        }

        [ComImport, Guid("587A461C-B80B-4f54-9194-5032589A6319"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        private interface IDiaReadExeAtOffsetCallback
        {
            [PreserveSig]
            int ReadExecutableAt(long fileOffset, int cbData, out int pcbData, IntPtr data);
        }

        [ComImport, Guid("0000000c-0000-0000-C000-000000000046"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        private interface IStream
        {
            [PreserveSig]
            int Read(IntPtr pv, int cb, out int pcbRead);

            [PreserveSig]
            int Write(IntPtr pv, int cb, out int pcbWritten);

            [PreserveSig]
            int Seek([In, MarshalAs(UnmanagedType.I8)] long dlibMove,
                [MarshalAs(UnmanagedType.I4)] STREAM_SEEK dwOrigin,
                IntPtr plibNewPosition);

            [PreserveSig]
            int SetSize([In, MarshalAs(UnmanagedType.I8)] long libNewSize);

            [PreserveSig]
            int CopyTo([In, MarshalAs(UnmanagedType.Interface)] IStream pstm,
                [In, MarshalAs(UnmanagedType.I8)] long cb,
                IntPtr pcbRead,
                IntPtr pcbWritten);

            [PreserveSig]
            int Commit(int grfCommitFlags);

            [PreserveSig]
            int Revert();

            [PreserveSig]
            int LockRegion(long libOffset, long cb, int dwLockType);

            [PreserveSig]
            int UnlockRegion(long libOffset, long cb, int dwLockType);

            [PreserveSig]
            int Stat([Out] out STATSTG pstatstg, int grfStatFlag);

            [PreserveSig]
            int Clone([MarshalAs(UnmanagedType.Interface)] out IStream ppstm);
        }

        private enum CorSymSearchPolicyAttributes
        {
            None = 0,
            AllowRegistryAccess = 0x1,
	        AllowSymbolServerAccess = 0x2,
	        AllowOriginalPathAccess = 0x4,
	        AllowReferencePathAccess = 0x8
        }

        private enum STREAM_SEEK
        {
            STREAM_SEEK_SET = 0,
            STREAM_SEEK_CUR = 1,
            STREAM_SEEK_END = 2
        }
    }
}
