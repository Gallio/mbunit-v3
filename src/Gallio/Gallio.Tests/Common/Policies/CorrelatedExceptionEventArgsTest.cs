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
using Gallio.Common.Policies;
using MbUnit.Framework;

namespace Gallio.Tests.Common.Policies
{
    [TestFixture]
    [TestsOn(typeof(CorrelatedExceptionEventArgs))]
    public class CorrelatedExceptionEventArgsTest
    {
        [Test, ExpectedArgumentNullException]
        public void ConstructorThrowsWhenMessageIsNull()
        {
            new CorrelatedExceptionEventArgs(null, new Exception(), "reported by", true);
        }

        [Test, ExpectedArgumentNullException]
        public void ConstructorThrowsWhenExceptionIsNull()
        {
            new CorrelatedExceptionEventArgs("foo", null, "reported by", true);
        }

        [Test, ExpectedArgumentNullException]
        public void AddCorrelationThrowsWhenMessageIsNull()
        {
            CorrelatedExceptionEventArgs e = new CorrelatedExceptionEventArgs("foo", new Exception(), "reported by", true);
            e.AddCorrelationMessage(null);
        }

        [Test]
        public void PropertiesContainSameValuesAsWereSetInTheConstructor()
        {
            Exception ex = new Exception();
            CorrelatedExceptionEventArgs e = new CorrelatedExceptionEventArgs("foo", ex, "reported by", true);

            Assert.AreEqual("foo", e.Message);
            Assert.AreEqual("reported by", e.ReporterStackTrace);
            Assert.AreSame(ex, e.Exception);
            Assert.IsTrue(e.IsRecursive);
        }

        [Test]
        public void AddCorrelationAppendsToTheMessage()
        {
            CorrelatedExceptionEventArgs e = new CorrelatedExceptionEventArgs("foo", new Exception(), "reported by", false);
            e.AddCorrelationMessage("bar");

            Assert.AreEqual("foo\nbar", e.Message);
        }
    }
}