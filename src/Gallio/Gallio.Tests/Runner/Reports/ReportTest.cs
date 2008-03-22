// Copyright 2005-2008 Gallio Project - http://www.gallio.org/
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

using System;
using System.IO;
using System.Xml.Serialization;
using Gallio.Model;
using Gallio.Runner.Reports;
using MbUnit.Framework;
using MbUnit.Framework.Xml;
using Gallio.Model.Serialization;

namespace Gallio.Tests.Runner.Reports
{
    [TestFixture]
    [TestsOn(typeof(Report))]
    [Author("Jeff", "jeff@ingenio.com")]
    public class ReportTests
    {
        [Test]
        public void ReportTypeIsXmlSerializable()
        {
            XmlSerializationAssert.IsXmlSerializable(typeof(Report));
        }

        [Test]
        public void RoundTripXmlSerialization()
        {
            XmlSerializer serializer = new XmlSerializer(typeof(Report));
            StringWriter writer = new StringWriter();

            Report report = new Report();
            report.PackageRun = new PackageRun();
            report.PackageRun.RootTestStepRun = new TestStepRun(new TestStepData("456", "abc", "456:abc", "testId"));
            report.PackageRun.RootTestStepRun.Children.Add(new TestStepRun(new TestStepData("child", "child", "child", "child")));

            serializer.Serialize(writer, report);

            Report deserializedReport = (Report) serializer.Deserialize(new StringReader(writer.ToString()));
            ReportAssert.AreEqual(report, deserializedReport);
        }

        [Test]
        public void GetAndSetTestModel()
        {
            Report report = new Report();

            Assert.IsNull(report.TestModelData);

            TestModelData value = MockTestDataFactory.CreateEmptyTestModel();
            report.TestModelData = value;
            Assert.AreSame(value, report.TestModelData);
        }

        [Test]
        public void GetAndSetPackage()
        {
            Report report = new Report();

            Assert.IsNull(report.TestModelData);

            TestPackageConfig value = new TestPackageConfig();
            report.PackageConfig = value;
            Assert.AreSame(value, report.PackageConfig);
        }

        [Test]
        public void GetAndSetPackageRun()
        {
            Report report = new Report();

            Assert.IsNull(report.PackageRun);

            PackageRun value = new PackageRun();
            report.PackageRun = value;
            Assert.AreSame(value, report.PackageRun);
        }
    }
}
