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
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Gallio.Framework;
using Gallio.Model.Diagnostics;
using MbUnit.Framework;

namespace Gallio.Tests.Model.Diagnostics
{
    [TestsOn(typeof(StackTraceFilter))]
    public class StackTraceFilterTest
    {
        [Test, ExpectedArgumentNullException]
        public void FilterStackTraceThrowsIfNull()
        {
            StackTraceFilter.FilterStackTrace(null);
        }

        [Test, ExpectedArgumentNullException]
        public void FilterStackTraceToStringThrowsIfNull()
        {
            StackTraceFilter.FilterStackTraceToString(null);
        }

        [Test, ExpectedArgumentNullException]
        public void FilterExceptionThrowsIfNull()
        {
            StackTraceFilter.FilterException((Exception) null);
        }

        [Test, ExpectedArgumentNullException]
        public void FilterExceptionDataThrowsIfNull()
        {
            StackTraceFilter.FilterException((ExceptionData)null);
        }

        public class WhenStackTracesIsFiltered
        {
            [Test]
            public void OmitsDebuggerHiddenMethods()
            {
                string trace = CaptureHidden();
                Log.WriteLine(trace);

                Assert.Contains(trace, "OmitsDebuggerHiddenMethods");
                Assert.IsFalse(trace.Contains("CaptureHidden"));
            }

            [Test]
            public void OmitsInternalMethods()
            {
                string trace = CaptureInternal();
                Log.WriteLine(trace);

                Assert.Contains(trace, "OmitsInternalMethods");
                Assert.IsFalse(trace.Contains("CaptureInternal"));
            }

            [Test, TestEntryPoint]
            public void CutsOffAtTestEntryPoint()
            {
                string trace = CaptureProxy();
                Log.WriteLine(trace);

                Assert.IsFalse(trace.Contains("CutsOffAtTestEntryPoint"));
                Assert.Contains(trace, "CaptureProxy");
                Assert.IsFalse(trace.Contains("CaptureInternal"));
            }

            [MethodImpl(MethodImplOptions.NoInlining)]
            private string CaptureProxy()
            {
                return CaptureInternal();
            }

            [MethodImpl(MethodImplOptions.NoInlining)]
            [DebuggerHidden]
            private string CaptureHidden()
            {
                return StackTraceFilter.CaptureFilteredStackTrace();
            }

            [MethodImpl(MethodImplOptions.NoInlining)]
            [TestFrameworkInternal]
            private string CaptureInternal()
            {
                return StackTraceFilter.CaptureFilteredStackTrace();
            }
        }

        public class WhenExceptionIsFiltered
        {
            [Test]
            public void OmitsDebuggerHiddenMethods()
            {
                string trace = null;
                try
                {
                    ThrowHidden();
                    Assert.Fail();
                }
                catch (InvalidOperationException ex)
                {
                    trace = StackTraceFilter.FilterException(ex).ToString();
                    Log.WriteLine(trace);
                }

                Assert.Contains(trace, "OmitsDebuggerHiddenMethods");
                Assert.IsFalse(trace.Contains("ThrowHidden"));
            }

            [Test]
            public void OmitsInternalMethods()
            {
                string trace = null;
                try
                {
                    ThrowInternal();
                    Assert.Fail();
                }
                catch (InvalidOperationException ex)
                {
                    trace = StackTraceFilter.FilterException(ex).ToString();
                    Log.WriteLine(trace);
                }

                Assert.Contains(trace, "OmitsInternalMethods");
                Assert.IsFalse(trace.Contains("ThrowInternal"));
            }

            [Test]
            public void OmitsInternalGenericMethods()
            {
                string trace = null;
                try
                {
                    ThrowInternalGeneric<int>();
                    Assert.Fail();
                }
                catch (InvalidOperationException ex)
                {
                    trace = StackTraceFilter.FilterException(ex).ToString();
                    Log.WriteLine(trace);
                }

                Assert.Contains(trace, "OmitsInternalGenericMethods");
                Assert.IsFalse(trace.Contains("ThrowInternalGeneric"));
            }

            [Test, TestEntryPoint]
            public void CutsOffAtTestEntryPoint()
            {
                string trace = null;
                try
                {
                    ThrowProxy();
                    Assert.Fail();
                }
                catch (InvalidOperationException ex)
                {
                    trace = StackTraceFilter.FilterException(ex).ToString();
                    Log.WriteLine(trace);
                }

                Assert.IsFalse(trace.Contains("CutsOffAtTestEntryPoint"));
                Assert.Contains(trace, "ThrowProxy");
                Assert.IsFalse(trace.Contains("ThrowInternal"));
            }

            [MethodImpl(MethodImplOptions.NoInlining)]
            private void ThrowProxy()
            {
                ThrowInternal();
            }

            [MethodImpl(MethodImplOptions.NoInlining)]
            [DebuggerHidden]
            private void ThrowHidden()
            {
                throw new InvalidOperationException("Boom");
            }

            [MethodImpl(MethodImplOptions.NoInlining)]
            [TestFrameworkInternal]
            private void ThrowInternal()
            {
                throw new InvalidOperationException("Boom");
            }

            [MethodImpl(MethodImplOptions.NoInlining)]
            [TestFrameworkInternal]
            private void ThrowInternalGeneric<T>()
            {
                throw new InvalidOperationException("Boom");
            }
        }
    }
}
