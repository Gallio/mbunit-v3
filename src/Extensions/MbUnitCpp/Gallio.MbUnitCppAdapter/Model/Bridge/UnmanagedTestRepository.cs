// Copyright 2005-2010 Gallio Project - http://www.gallio.org/
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
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using Gallio.Model;

namespace Gallio.MbUnitCppAdapter.Model.Bridge
{
    /// <summary>
    /// Wraps a MbUnit C++ unmanaged test executable library.
    /// </summary>
    public class UnmanagedTestRepository : IDisposable
    {
        private delegate int GetVersionDelegate();
        private delegate void GetHeadTestDelegate(out Position position);
        private delegate int GetNextTestDelegate(ref Position position, ref TestInfoData testStepInfo);
        private delegate void RunTestDelegate(ref Position position, out TestStepResult testResultInfo);
        private delegate IntPtr GetStringDelegate(int stringId);
        private delegate void ReleaseStringDelegate(int stringId);
        private delegate void ReleaseAllStringsDelegate();
        private IntPtr procGetVersion;
        private IntPtr procGetHeadTest;
        private IntPtr procGetNextTest;
        private IntPtr procRunTest;
        private IntPtr procGetString;
        private IntPtr procReleaseString;
        private IntPtr procReleaseAllStrings;
        private bool disposed;
        private bool isValid;
        private readonly IntPtr hModule;
        private readonly string fileName;

        /// <summary>
        /// Gets a value indicating whether the test library was successfully loaded.
        /// </summary>
        public bool IsValid
        {
            get
            {
                return isValid;
            }
        }

        /// <summary>
        /// Gets the file name.
        /// </summary>
        public string FileName
        {
            get
            {
                return fileName;
            }
        }

        /// <summary>
        /// Constructs a wrapper around a MbUnit C++ unmanaged test library.
        /// </summary>
        /// <param name="fileName"></param>
        public UnmanagedTestRepository(string fileName)
        {
            if (fileName == null)
                throw new ArgumentNullException("fileName");

            this.fileName = fileName;
            IntPtr hLibrary = IntPtr.Zero;

            try
            {
                hLibrary = NativeMethods.LoadLibrary(fileName);
                int error = Marshal.GetLastWin32Error();

                if (hLibrary != IntPtr.Zero)
                {
                    isValid = true;
                    hModule = hLibrary;
                    CollectProcAddresses();
                }
            }
            finally
            {
                if (!isValid && hLibrary != IntPtr.Zero)
                    NativeMethods.FreeLibrary(hModule);
            }
        }


        private void CollectProcAddresses()
        {
            procGetVersion = GetProcAddress("MbUnitCpp_GetVersion");
            procGetHeadTest = GetProcAddress("MbUnitCpp_GetHeadTest");
            procGetNextTest = GetProcAddress("MbUnitCpp_GetNextTest");
            procRunTest = GetProcAddress("MbUnitCpp_RunTest");
            procGetString = GetProcAddress("MbUnitCpp_GetString");
            procReleaseString = GetProcAddress("MbUnitCpp_ReleaseString");
            procReleaseAllStrings = GetProcAddress("MbUnitCpp_ReleaseAllStrings");
        }

        private IntPtr GetProcAddress(string procName)
        {
            IntPtr hFunction = NativeMethods.GetProcAddress(hModule, procName);
            isValid &= (hFunction != IntPtr.Zero);
            return hFunction;
        }

        /// <summary>
        /// Returns the version of the MbUnitCpp test framework version that was used to compile the test library.
        /// </summary>
        /// <returns></returns>
        public int GetVersion()
        {
            if (!IsValid)
                throw new InvalidOperationException("The target MbUnitCpp test library is not valid.");

            var function = (GetVersionDelegate)Marshal.GetDelegateForFunctionPointer(procGetVersion, typeof(GetVersionDelegate));
            return function();
        }

        public IEnumerable<TestInfoData> GetTests()
        {
            if (!IsValid)
                throw new InvalidOperationException("The target MbUnitCpp test library is not valid.");

            var getHeadTest = (GetHeadTestDelegate)Marshal.GetDelegateForFunctionPointer(procGetHeadTest, typeof(GetHeadTestDelegate));
            var getNextTest = (GetNextTestDelegate)Marshal.GetDelegateForFunctionPointer(procGetNextTest, typeof(GetNextTestDelegate));
            Position position;
            getHeadTest(out position);
            var data = new TestInfoData();
            var list = new List<TestInfoData>();

            while (getNextTest(ref position, ref data) > 0)
            {
                yield return data;
            }
        }

        public TestStepResult RunTest(TestInfoData testInfoData)
        {
            if (!IsValid)
                throw new InvalidOperationException("The target MbUnitCpp test library is not valid.");
            if (testInfoData.IsTestFixture)
                throw new InvalidOperationException("Not a valid test case.");

            TestStepResult testStepResult;
            var function = (RunTestDelegate)Marshal.GetDelegateForFunctionPointer(procRunTest, typeof(RunTestDelegate));
            function(ref testInfoData.Position, out testStepResult);
            return testStepResult;
        }

        public string GetString(int stringId)
        {
            if (!IsValid)
                throw new InvalidOperationException("The target MbUnitCpp test library is not valid.");
            if (stringId == 0)
                return String.Empty;

            var getString = (GetStringDelegate)Marshal.GetDelegateForFunctionPointer(procGetString, typeof(GetStringDelegate));
            var releaseString = (ReleaseStringDelegate)Marshal.GetDelegateForFunctionPointer(procReleaseString, typeof(ReleaseStringDelegate));
            string result = Marshal.PtrToStringAnsi(getString(stringId));
            releaseString(stringId);
            return result;
        }

        private void ReleaseAllStrings()
        {
            if (IsValid)
            {
                var releaseAllStrings = (ReleaseAllStringsDelegate)Marshal.GetDelegateForFunctionPointer(procReleaseAllStrings, typeof(ReleaseAllStringsDelegate));
                releaseAllStrings();
            }
        }

        /// <summary>
        /// Disposes the test library and releases the handle.
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            if (!disposed && (hModule != IntPtr.Zero))
            {
                NativeMethods.FreeLibrary(hModule);
            }

            disposed = true;
        }

        /// <inheritdoc />
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
