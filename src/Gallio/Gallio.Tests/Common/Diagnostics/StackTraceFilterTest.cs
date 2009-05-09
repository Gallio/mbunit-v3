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
using System.Runtime.CompilerServices;
using Gallio.Framework;
using Gallio.Common.Diagnostics;
using MbUnit.Framework;

namespace Gallio.Tests.Common.Diagnostics
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
                TestLog.WriteLine(trace);

                Assert.Contains(trace, "OmitsDebuggerHiddenMethods");
                Assert.IsFalse(trace.Contains("CaptureHidden"));
            }

            [Test]
            public void OmitsDebuggerHiddenConstructors()
            {
                string trace = new CaptureHiddenConstructor().Trace;
                TestLog.WriteLine(trace);

                Assert.Contains(trace, "OmitsDebuggerHiddenConstructors");
                Assert.IsFalse(trace.Contains("CaptureHiddenConstructor"));
            }

            [Test]
            public void OmitsInternalMethods()
            {
                string trace = CaptureInternal();
                TestLog.WriteLine(trace);

                Assert.Contains(trace, "OmitsInternalMethods");
                Assert.IsFalse(trace.Contains("CaptureInternal"));
            }

            [Test]
            public void OmitsOverloadedMethodsAppropriately()
            {
                string trace = CaptureOverload(1);
                TestLog.WriteLine(trace);

                Assert.Contains(trace, "OmitsOverloadedMethodsAppropriately");
                Assert.IsFalse(trace.Contains("CaptureOverload"), "Should exclude the overload that lacks a debugger hidden");

                trace = CaptureOverload(1, "abc");
                TestLog.WriteLine(trace);

                Assert.Contains(trace, "OmitsOverloadedMethodsAppropriately");
                Assert.IsTrue(trace.Contains("CaptureOverload"), "Should include the overload that lacks a debugger hidden");
            }

            [Test]
            public void OmitsGenericOverloadedMethodsAppropriately()
            {
                string trace = CaptureGenericOverload<int>();
                TestLog.WriteLine(trace);

                Assert.Contains(trace, "OmitsGenericOverloadedMethodsAppropriately");
                Assert.IsFalse(trace.Contains("CaptureGenericOverload"), "Should exclude the overload that lacks a debugger hidden");

                trace = CaptureGenericOverload<int, string>();
                TestLog.WriteLine(trace);

                Assert.Contains(trace, "OmitsGenericOverloadedMethodsAppropriately");
                Assert.IsTrue(trace.Contains("CaptureGenericOverload"), "Should include the overload that lacks a debugger hidden");
            }

            [Test, UserCodeEntryPoint]
            public void CutsOffAtTestEntryPoint()
            {
                string trace = CaptureProxy();
                TestLog.WriteLine(trace);

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
            [DebuggerHidden]
            private string CaptureOverload(int dummy)
            {
                return StackTraceFilter.CaptureFilteredStackTrace();
            }

            [MethodImpl(MethodImplOptions.NoInlining)]
            private string CaptureOverload(int dummy1, string dummy2)
            {
                return StackTraceFilter.CaptureFilteredStackTrace();
            }

            [MethodImpl(MethodImplOptions.NoInlining)]
            [DebuggerHidden]
            private string CaptureGenericOverload<T>()
            {
                return StackTraceFilter.CaptureFilteredStackTrace();
            }

            [MethodImpl(MethodImplOptions.NoInlining)]
            private string CaptureGenericOverload<S, T>()
            {
                return StackTraceFilter.CaptureFilteredStackTrace();
            }

            [MethodImpl(MethodImplOptions.NoInlining)]
            [SystemInternal]
            private string CaptureInternal()
            {
                return StackTraceFilter.CaptureFilteredStackTrace();
            }

            private class CaptureHiddenConstructor
            {
                public readonly string Trace;

                [MethodImpl(MethodImplOptions.NoInlining)]
                [DebuggerHidden]
                public CaptureHiddenConstructor()
                {
                    Trace = StackTraceFilter.CaptureFilteredStackTrace();
                }
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
                    TestLog.WriteLine(trace);
                }

                Assert.Contains(trace, "OmitsDebuggerHiddenMethods");
                Assert.IsFalse(trace.Contains("ThrowHidden"));
            }

            [Test]
            public void OmitsDebuggerHiddenConstructors()
            {
                string trace = null;
                try
                {
                    new ThrowHiddenConstructor();
                    Assert.Fail();
                }
                catch (InvalidOperationException ex)
                {
                    trace = StackTraceFilter.FilterException(ex).ToString();
                    TestLog.WriteLine(trace);
                }

                Assert.Contains(trace, "OmitsDebuggerHiddenConstructors");
                Assert.IsFalse(trace.Contains("ThrowHiddenConstructor"));
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
                    TestLog.WriteLine(trace);
                }

                Assert.Contains(trace, "OmitsInternalMethods");
                Assert.IsFalse(trace.Contains("ThrowInternal"));
            }

            [Test]
            public void OmitsOverloadedMethodsAppropriately()
            {
                string trace = null;
                try
                {
                    ThrowOverload(1);
                    Assert.Fail();
                }
                catch (InvalidOperationException ex)
                {
                    trace = StackTraceFilter.FilterException(ex).ToString();
                    TestLog.WriteLine(trace);
                }

                Assert.Contains(trace, "OmitsOverloadedMethodsAppropriately");
                Assert.IsFalse(trace.Contains("ThrowOverload"), "Should exclude the overload that lacks a debugger hidden");

                try
                {
                    ThrowOverload(1, "abc");
                    Assert.Fail();
                }
                catch (InvalidOperationException ex)
                {
                    trace = StackTraceFilter.FilterException(ex).ToString();
                    TestLog.WriteLine(trace);
                }

                Assert.Contains(trace, "OmitsOverloadedMethodsAppropriately");
                Assert.IsTrue(trace.Contains("ThrowOverload"), "Should include the overload that lacks a debugger hidden");
            }

            [Test]
            public void OmitsGenericOverloadedMethodsAppropriately()
            {
                string trace = null;
                try
                {
                    ThrowGenericOverload<int>();
                    Assert.Fail();
                }
                catch (InvalidOperationException ex)
                {
                    trace = StackTraceFilter.FilterException(ex).ToString();
                    TestLog.WriteLine(trace);
                }

                Assert.Contains(trace, "OmitsGenericOverloadedMethodsAppropriately");
                Assert.IsFalse(trace.Contains("ThrowGenericOverload"), "Should exclude the overload that lacks a debugger hidden");

                try
                {
                    ThrowGenericOverload<int, string>();
                    Assert.Fail();
                }
                catch (InvalidOperationException ex)
                {
                    trace = StackTraceFilter.FilterException(ex).ToString();
                    TestLog.WriteLine(trace);
                }

                Assert.Contains(trace, "OmitsGenericOverloadedMethodsAppropriately");
                Assert.IsTrue(trace.Contains("ThrowGenericOverload"), "Should include the overload that lacks a debugger hidden");
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
                    TestLog.WriteLine(trace);
                }

                Assert.Contains(trace, "OmitsInternalGenericMethods");
                Assert.IsFalse(trace.Contains("ThrowInternalGeneric"));
            }

            [Test, UserCodeEntryPoint]
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
                    TestLog.WriteLine(trace);
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
            [DebuggerHidden]
            private void ThrowOverload(int dummy)
            {
                throw new InvalidOperationException("Boom");
            }

            [MethodImpl(MethodImplOptions.NoInlining)]
            private void ThrowOverload(int dummy1, string dummy2)
            {
                throw new InvalidOperationException("Boom");
            }

            [MethodImpl(MethodImplOptions.NoInlining)]
            [DebuggerHidden]
            private void ThrowGenericOverload<T>()
            {
                throw new InvalidOperationException("Boom");
            }

            [MethodImpl(MethodImplOptions.NoInlining)]
            private void ThrowGenericOverload<T, S>()
            {
                throw new InvalidOperationException("Boom");
            }

            [MethodImpl(MethodImplOptions.NoInlining)]
            [SystemInternal]
            private void ThrowInternal()
            {
                throw new InvalidOperationException("Boom");
            }

            [MethodImpl(MethodImplOptions.NoInlining)]
            [SystemInternal]
            private void ThrowInternalGeneric<T>()
            {
                throw new InvalidOperationException("Boom");
            }

            private class ThrowHiddenConstructor
            {
                [MethodImpl(MethodImplOptions.NoInlining)]
                [DebuggerHidden]
                public ThrowHiddenConstructor()
                {
                    throw new InvalidOperationException("Boom");
                }
            }
        }
    }
}
