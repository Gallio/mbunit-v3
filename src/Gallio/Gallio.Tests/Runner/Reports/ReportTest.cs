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
using System.IO;
using System.Xml.Serialization;
using Gallio.Model;
using Gallio.Reflection;
using Gallio.Runner.Reports;
using MbUnit.Framework;
using Gallio.Model.Serialization;

namespace Gallio.Tests.Runner.Reports
{
    [TestFixture]
    [TestsOn(typeof(Report))]
    public class ReportTests : BaseTestWithMocks
    {
        [Test]
        public void ReportTypeIsXmlSerializable()
        {
            Assert.IsXmlSerializableType(typeof(Report));
        }

        [Test]
        public void RoundTripXmlSerialization()
        {
            XmlSerializer serializer = new XmlSerializer(typeof(Report));
            StringWriter writer = new StringWriter();

            Report report = new Report();
            report.TestPackageRun = new TestPackageRun();
            report.TestPackageRun.RootTestStepRun = new TestStepRun(new TestStepData("456", "abc", "456:abc", "testId"));
            report.TestPackageRun.RootTestStepRun.Children.Add(new TestStepRun(new TestStepData("child", "child", "child", "child")));

            serializer.Serialize(writer, report);

            Report deserializedReport = (Report) serializer.Deserialize(new StringReader(writer.ToString()));
            ReportAssert.AreEqual(report, deserializedReport);
        }

        [Test]
        public void GetAndSetTestModel()
        {
            Report report = new Report();

            Assert.IsNull(report.TestModel);

            TestModelData value = MockTestDataFactory.CreateEmptyTestModel();
            report.TestModel = value;
            Assert.AreSame(value, report.TestModel);
        }

        [Test]
        public void GetAndSetPackageConfig()
        {
            Report report = new Report();

            Assert.IsNull(report.TestModel);

            TestPackageConfig value = new TestPackageConfig();

            report.TestPackageConfig = value;
            Assert.AreSame(value, report.TestPackageConfig);
        }

        [Test]
        public void GetAndSetPackageRun()
        {
            Report report = new Report();

            Assert.IsNull(report.TestPackageRun);

            TestPackageRun value = new TestPackageRun();
            report.TestPackageRun = value;
            Assert.AreSame(value, report.TestPackageRun);
        }
    }
}
