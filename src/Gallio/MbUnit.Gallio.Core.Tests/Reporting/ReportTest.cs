// Copyright 2007 MbUnit Project - http://www.mbunit.com/
// Portions Copyright 2000-2004 Jonathan De Halleux, Jamie Cansdale
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

extern alias MbUnit2;
using System;
using System.IO;
using System.Xml.Serialization;
using MbUnit.Core.Reporting;
using MbUnit2::MbUnit.Framework;

namespace MbUnit.Core.Tests.Reporting
{
    [TestFixture]
    [TestsOn(typeof(Report))]
    [Author("Jeff", "jeff@ingenio.com")]
    public class ReportTests
    {
        [Test]
        public void RoundTripXmlSerialization()
        {
            XmlSerializer serializer = new XmlSerializer(typeof(Report));
            StringWriter writer = new StringWriter();

            Report report = new Report();
            report.PackageRun = new PackageRun();
            report.PackageRun.TestRuns.Add(new TestRun("abc"));

            serializer.Serialize(writer, report);

            Report deserializedReport = (Report) serializer.Deserialize(new StringReader(writer.ToString()));
            AreEqual(report, deserializedReport);
        }

        public static void AreEqual(Report expected, Report actual)
        {
            if (expected == null)
            {
                Assert.IsNull(actual);
                return;
            }
        }
    }
}
