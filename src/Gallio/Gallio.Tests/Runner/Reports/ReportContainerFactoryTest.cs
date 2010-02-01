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
using Gallio.Runner.Reports;
using MbUnit.Framework;

namespace Gallio.Tests.Runner.Reports
{
    [TestFixture]
    [TestsOn(typeof(ReportContainerFactory))]
    public class ReportContainerFactoryTest
    {
        [Test]
        public void MakeForSaving_with_file_system()
        {
            var factory = new ReportContainerFactory(@"C:\Directory", "name");
            var container = (AbstractReportContainer)factory.MakeForSaving(false);
            Assert.IsInstanceOfType<FileSystemReportContainer>(container);
            Assert.AreEqual("name", container.ReportName);
            Assert.AreEqual(@"C:\Directory\", container.ReportDirectory);
        }

        [Test]
        public void MakeForSaving_with_archive()
        {
            var factory = new ReportContainerFactory(@"C:\Directory", "name");
            var container = (AbstractReportContainer)factory.MakeForSaving(true);
            Assert.IsInstanceOfType<ArchiveReportContainer>(container);
            Assert.AreEqual("name", container.ReportName);
            Assert.AreEqual(@"C:\Directory\", container.ReportDirectory);
        }

        [Test]
        [ExpectedException(typeof(NotImplementedException))]
        public void MakeForReading()
        {
            var factory = new ReportContainerFactory("directory", "name");
            factory.MakeForReading();
        }
    }
}
