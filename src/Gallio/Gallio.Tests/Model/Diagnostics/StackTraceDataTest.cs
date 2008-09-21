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
using Gallio.Model.Diagnostics;
using Gallio.Model.Logging;
using MbUnit.Framework;

namespace Gallio.Tests.Model.Diagnostics
{
    [TestsOn(typeof(StackTraceData))]
    public class StackTraceDataTest
    {
        [Test, ExpectedArgumentNullException]
        public void ConstructorThrowsIfArgumentIsNull()
        {
            new StackTraceData(null);
        }

        [Test]
        public void ToStringReturnsStackTrace()
        {
            Assert.AreEqual("foo", new StackTraceData("foo").ToString());
        }

        [Test]
        public void IsEmptyReturnsTrueIfAndOnlyIfStackTraceIsEmpty()
        {
            Assert.IsTrue(new StackTraceData("").IsEmpty);
            Assert.IsFalse(new StackTraceData("foo").IsEmpty);
        }

        [Test]
        public void ConstructorStripsTrailingNewLineAutomatically()
        {
            Assert.AreEqual("bar\nfoo", new StackTraceData("bar\nfoo\n").ToString());
        }

        [Test]
        public void WriteToThrowsIfArgumentIsNull()
        {
            StackTraceData data = new StackTraceData("foo");
            Assert.Throws<ArgumentNullException>(() => data.WriteTo(null));
        }

        [Test]
        public void WriteTo()
        {
            StackTraceData data = new StackTraceData("   at SomeMethod\r\n   at Gallio.Tests.Model.Diagnostics.StackTraceDataTest.WriteTo() in C:\\Source\\MbUnit\\v3\\src\\Gallio\\Gallio.Tests\\Model\\Diagnostics\\StackTraceDataTest.cs:line 70\r\n   at Gallio.Tests.Model.Diagnostics.StackTraceDataTest.Blah() in C:\\Source\\MbUnit\\v3\\src\\Gallio\\Gallio.Tests\\Model\\Diagnostics\\StackTraceDataTest.cs:line 72\r\n");

            StringTestLogWriter writer = new StringTestLogWriter(true);
            data.WriteTo(writer.Failures);

            Assert.AreEqual("[Marker \'StackTrace\']   at SomeMethod\n   at Gallio.Tests.Model.Diagnostics.StackTraceDataTest.WriteTo() in [Marker \'CodeLocation\']C:\\Source\\MbUnit\\v3\\src\\Gallio\\Gallio.Tests\\Model\\Diagnostics\\StackTraceDataTest.cs:line 70[End]\n   at Gallio.Tests.Model.Diagnostics.StackTraceDataTest.Blah() in [Marker \'CodeLocation\']C:\\Source\\MbUnit\\v3\\src\\Gallio\\Gallio.Tests\\Model\\Diagnostics\\StackTraceDataTest.cs:line 72[End][End]", writer.ToString());
        }
    }
}
