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
using System.Linq;
using System.Text;
using MbUnit.Framework;
using Gallio.TDNetRunner.Core;

namespace Gallio.TDNetRunner.Tests
{
    [TestFixture]
    [TestsOn(typeof(ReportSettings))]
    public class ReportSettingsTest
    {
        [Test]
        [ExpectedArgumentNullException]
        public void Constructs_with_null_reportType_should_throw_exception()
        {
            new ReportSettings(null, 123);
        }

        [Test]
        [ExpectedArgumentOutOfRangeException]
        public void Constructs_with_negative_autoCondenseThreshold_should_throw_exception()
        {
            new ReportSettings("Html", -123);
        }

        [Test]
        [Row(0, false)]
        [Row(123, true)]
        public void Constructs_ok(int threshold, bool expectedEnabled)
        {
            var settings = new ReportSettings("Html", threshold);
            Assert.AreEqual("Html", settings.ReportType);
            Assert.AreEqual(threshold, settings.AutoCondenseThreshold);
            Assert.AreEqual(expectedEnabled, settings.AutoCondenseEnabled);
        }

        [Test]
        [ExpectedArgumentNullException]
        public void SupportsAutoCondense_with_null_allReportTypes_should_throw_exception()
        {
            var settings = new ReportSettings("Html", 123);
            settings.SupportsAutoCondense(null);
        }

        [Test]
        [Row("A", true)]
        [Row("A-Condensed", false)]
        [Row("B", false)]
        public void SupportsAutoCondense(string reportType, bool expectedSupport)
        {
            var settings = new ReportSettings(reportType, 123);
            var actualSupport = settings.SupportsAutoCondense(new List<string>{ "A", "A-Condensed", "B" });
            Assert.AreEqual(expectedSupport, actualSupport);
        }
    }
}
