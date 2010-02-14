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
using System.IO;
using Gallio.Common.IO;
using Gallio.Runner.Reports;
using MbUnit.Framework;
using Rhino.Mocks;

namespace Gallio.Tests.Runner.Reports
{
    [TestFixture]
    [TestsOn(typeof(ReportArchive))]
    public class ReportArchiveTest
    {
        [Test]
        [Row(null, true, ReportArchive.ReportArchiveValue.Normal)]
        [Row("", true, ReportArchive.ReportArchiveValue.Normal)]
        [Row("Normal", true, ReportArchive.ReportArchiveValue.Normal)]
        [Row("normal", true, ReportArchive.ReportArchiveValue.Normal)]
        [Row("nOrMal", true, ReportArchive.ReportArchiveValue.Normal)]
        [Row("Zip", true, ReportArchive.ReportArchiveValue.Zip)]
        [Row("zip", true, ReportArchive.ReportArchiveValue.Zip)]
        [Row("ZiP", true, ReportArchive.ReportArchiveValue.Zip)]
        [Row("qwerty", false, ReportArchive.ReportArchiveValue.Normal)]
        internal void TryParse(string value, bool expectedResult, ReportArchive.ReportArchiveValue expectedMode)
        {
            ReportArchive actualMode;
            bool actualResult = ReportArchive.TryParse(value, out actualMode);
            Assert.AreEqual(expectedResult, actualResult);
            Assert.AreEqual(expectedMode, actualMode.Value);
        }

        [Test]
        [Row(null, ReportArchive.ReportArchiveValue.Normal)]
        [Row("", ReportArchive.ReportArchiveValue.Normal)]
        [Row("Normal", ReportArchive.ReportArchiveValue.Normal)]
        [Row("normal", ReportArchive.ReportArchiveValue.Normal)]
        [Row("nOrMal", ReportArchive.ReportArchiveValue.Normal)]
        [Row("Zip", ReportArchive.ReportArchiveValue.Zip)]
        [Row("zip", ReportArchive.ReportArchiveValue.Zip)]
        [Row("ZiP", ReportArchive.ReportArchiveValue.Zip)]
        [Row("qwerty", ReportArchive.ReportArchiveValue.Normal, ExpectedException = typeof(ArgumentException))]
        internal void Parse(string value, ReportArchive.ReportArchiveValue expectedMode)
        {
            ReportArchive actualMode = ReportArchive.Parse(value);
            Assert.AreEqual(expectedMode, actualMode.Value);
        }
    }
}
