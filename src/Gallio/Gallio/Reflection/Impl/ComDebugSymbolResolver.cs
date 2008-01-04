using System;
using System.Collections.Generic;
using System.Diagnostics.SymbolStore;
using System.IO;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using Gallio.Reflection;

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
    /// </para>
    /// <para>
    /// Many thanks to you!
    /// </para>
    /// </remarks>
    public class ComDebugSymbolResolver : IDebugSymbolResolver
    {
        private const int E_PDB_NOT_FOUND = unchecked((int)0x806D0005);
        private const int E_PDB_NO_DEBUG_INFO = unchecked((int)0x806D0014);
        private const int E_PDB_DEBUG_INFO_NOT_IN_PDB = unchecked((int)0x806D0017);
        private const int E_FAIL = unchecked((int)0x80004005);

        private readonly Dictionary<string, ISymbolReader> symbolReaders = new Dictionary<string, ISymbolReader>();

        /// <inheritdoc />
        public SourceLocation GetSourceLocationForMethod(string assemblyFile, int methodToken)
        {
            if (assemblyFile == null)
                throw new ArgumentNullException("assemblyFile");

            try
            {
                ISymbolReader reader = GetSymbolReader(assemblyFile);
                if (reader == null)
                    return null;

                ISymbolMethod method = reader.GetMethod(new SymbolToken(methodToken));

                int seqPtCount = method.SequencePointCount;
                ISymbolDocument[] docs = new ISymbolDocument[seqPtCount];
                int[] offsets = new int[seqPtCount];
                int[] lines = new int[seqPtCount];
                int[] columns = new int[seqPtCount];
                int[] endLines = new int[seqPtCount];
                int[] endColumns = new int[seqPtCount];

                method.GetSequencePoints(offsets, docs, lines, columns, endLines, endColumns);

                return new SourceLocation(docs[0].URL, lines[0], columns[0]);
            }
            catch (COMException ex)
            {
                if (ex.ErrorCode == E_FAIL)
                    return null;
                throw;
            }
        }

        private ISymbolReader GetSymbolReader(string assemblyFile)
        {
            lock (symbolReaders)
            {
                ISymbolReader symbolReader;
                if (!symbolReaders.TryGetValue(assemblyFile, out symbolReader))
                {
                    symbolReader = CreateSymbolReader(assemblyFile, null);
                    symbolReaders.Add(assemblyFile, symbolReader);
                }

                return symbolReader;
            }
        }

        [SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.UnmanagedCode)]
        private static ISymbolReader CreateSymbolReader(string filename, string searchPath)
        {
            // Guids for imported metadata interfaces.
            Guid CLSID_CorMetaDataDispenser = new Guid(0xe5cb7a31, 0x7512, 0x11d2, 0x89, 0xce, 0x00, 0x80, 0xc7, 0x92, 0xe5, 0xd8);
            Guid IID_IMetaDataImport = new Guid(0x7dac8207, 0xd3ae, 0x4c75, 0x9b, 0x67, 0x92, 0x80, 0x1a, 0x49, 0x7d, 0x44);

            // First create the Metadata dispenser.
            IMetaDataDispenser dispenser = (IMetaDataDispenser)Activator.CreateInstance(Type.GetTypeFromCLSID(CLSID_CorMetaDataDispenser));

            IntPtr importer = IntPtr.Zero;
            try
            {
                dispenser.OpenScope(filename, 0, ref IID_IMetaDataImport, out importer);

                SymBinder binder = new SymBinder();
                return binder.GetReader(importer, filename, searchPath);
            }
            catch (FileNotFoundException)
            {
                return null;
            }
            catch (COMException ex)
            {
                if (ex.ErrorCode == E_PDB_NOT_FOUND || ex.ErrorCode == E_PDB_NO_DEBUG_INFO || ex.ErrorCode == E_PDB_DEBUG_INFO_NOT_IN_PDB)
                    return null;
                throw;
            }
            finally
            {
                if (importer != IntPtr.Zero)
                    Marshal.Release(importer);
            }
        }

        [Guid("809c652e-7396-11d2-9771-00a0c9b4d50c"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
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
            void OpenScope([In, MarshalAs(UnmanagedType.LPWStr)] String szScope,
                [In] Int32 dwOpenFlags,
                [In] ref Guid riid,
                [Out] out IntPtr punk);

            // Don't need any other methods.
        }
    }
}
