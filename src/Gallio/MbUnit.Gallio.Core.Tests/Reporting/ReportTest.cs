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
using MbUnit.Core.Harness;
using MbUnit.Core.Reporting;
using MbUnit.Core.Model;
using MbUnit.Framework.Xml;
using MbUnit2::MbUnit.Framework;

namespace MbUnit.Core.Tests.Reporting
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
            report.PackageRun.TestRuns.Add(new TestRun("123", new StepRun("456", "abc", "456:abc")));

            serializer.Serialize(writer, report);

            Report deserializedReport = (Report) serializer.Deserialize(new StringReader(writer.ToString()));
            CoreAssert.AreEqual(report, deserializedReport);
        }

        [Test]
        public void GetAndSetTemplateModel()
        {
            Report report = new Report();

            Assert.IsNull(report.TemplateModel);

            TemplateModel value = MockTestDataFactory.CreateEmptyTemplateModel();
            report.TemplateModel = value;
            Assert.AreSame(value, report.TemplateModel);
        }

        [Test]
        public void GetAndSetTestModel()
        {
            Report report = new Report();

            Assert.IsNull(report.TestModel);

            TestModel value = MockTestDataFactory.CreateEmptyTestModel();
            report.TestModel = value;
            Assert.AreSame(value, report.TestModel);
        }

        [Test]
        public void GetAndSetPackage()
        {
            Report report = new Report();

            Assert.IsNull(report.TestModel);

            TestPackage value = new TestPackage();
            report.Package = value;
            Assert.AreSame(value, report.Package);
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
