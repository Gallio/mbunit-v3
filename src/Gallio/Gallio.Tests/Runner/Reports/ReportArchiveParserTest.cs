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
    [TestsOn(typeof(ReportArchiveParser))]
    public class ReportArchiveParserTest
    {
        [Test]
        [Row(null, true, ReportArchive.Normal)]
        [Row("", true, ReportArchive.Normal)]
        [Row("Normal", true, ReportArchive.Normal)]
        [Row("normal", true, ReportArchive.Normal)]
        [Row("nOrMal", true, ReportArchive.Normal)]
        [Row("Zip", true, ReportArchive.Zip)]
        [Row("zip", true, ReportArchive.Zip)]
        [Row("ZiP", true, ReportArchive.Zip)]
        [Row("qwerty", false, ReportArchive.Normal)]
        public void TryParse(string value, bool expectedResult, ReportArchive expectedMode)
        {
            ReportArchive actualMode;
            bool actualResult = ReportArchiveParser.TryParse(value, out actualMode);
            Assert.AreEqual(expectedResult, actualResult);
            Assert.AreEqual(expectedMode, actualMode);
        }

        [Test]
        [Row(null, ReportArchive.Normal)]
        [Row("", ReportArchive.Normal)]
        [Row("Normal", ReportArchive.Normal)]
        [Row("normal", ReportArchive.Normal)]
        [Row("nOrMal", ReportArchive.Normal)]
        [Row("Zip", ReportArchive.Zip)]
        [Row("zip", ReportArchive.Zip)]
        [Row("ZiP", ReportArchive.Zip)]
        [Row("qwerty", ReportArchive.Normal, ExpectedException = typeof(ArgumentException))]
        public void Parse(string value, ReportArchive expectedMode)
        {
            ReportArchive actualMode = ReportArchiveParser.Parse(value);
            Assert.AreEqual(expectedMode, actualMode);
        }
    }
}
