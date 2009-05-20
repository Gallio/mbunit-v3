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
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Gallio.Common.Policies;
using MbUnit.Framework;

namespace Gallio.Tests.Common.Policies
{
    [TestsOn(typeof(SpecialPathPolicy))]
    public class SpecialPathPolicyTest
    {
        [Test]
        public void For_WhenPartitionIsNull_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => SpecialPathPolicy.For(null));
        }

        [Test]
        public void For_WhenPartitionIsValid_ReturnsPolicyWithNamedPartition()
        {
            var policy = SpecialPathPolicy.For("name");

            Assert.AreEqual("name", policy.Partition);
        }

        [Test]
        public void For_WhenPartitionIsAType_ReturnsPolicyWithNamedPartitionDerivedFromFullName()
        {
            var policy = SpecialPathPolicy.For<SpecialPathPolicyTest>();

            Assert.AreEqual(typeof(SpecialPathPolicyTest).FullName, policy.Partition);
        }

        [Test]
        public void GetTempDirectory_ReturnsTempDirectoryOfPartition()
        {
            var policy = SpecialPathPolicy.For("Test");

            DirectoryInfo result = policy.GetTempDirectory();

            Assert.Multiple(() =>
            {
                Assert.IsTrue(result.Exists);
                Assert.AreEqual(Path.Combine(Path.GetTempPath(), @"Gallio\Test"), result.ToString());
            });
        }

        [Test]
        public void GetTempDirectoryWithUniqueName_ReturnsUniqueTempSubDirectoryWithinPartition()
        {
            var policy = SpecialPathPolicy.For("Test");

            DirectoryInfo result = policy.CreateTempDirectoryWithUniqueName();

            Assert.Multiple(() =>
            {
                Assert.IsTrue(result.Exists);
                Assert.StartsWith(result.ToString(), Path.Combine(Path.GetTempPath(), @"Gallio\Test\"));
            });
        }

        [Test]
        public void GetTempFileWithUniqueName_ReturnsUniqueTempFileWithinPartition()
        {
            var policy = SpecialPathPolicy.For("Test");

            FileInfo result = policy.CreateTempFileWithUniqueName();

            Assert.Multiple(() =>
            {
                Assert.IsTrue(result.Exists);
                Assert.StartsWith(result.ToString(), Path.Combine(Path.GetTempPath(), @"Gallio\Test\"));
            });
        }

        [Test]
        public void GetLocalUserApplicationDataDirectory_ReturnsTempDirectoryOfPartition()
        {
            var policy = SpecialPathPolicy.For("Test");

            DirectoryInfo result = policy.GetLocalUserApplicationDataDirectory();

            Assert.Multiple(() =>
            {
                Assert.IsTrue(result.Exists);
                Assert.AreEqual(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), @"Gallio\Test"), result.ToString());
            });
        }

        [Test]
        public void GetRoamingUserApplicationDataDirectory_ReturnsTempDirectoryOfPartition()
        {
            var policy = SpecialPathPolicy.For("Test");

            DirectoryInfo result = policy.GetRoamingUserApplicationDataDirectory();

            Assert.Multiple(() =>
            {
                Assert.IsTrue(result.Exists);
                Assert.AreEqual(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), @"Gallio\Test"), result.ToString());
            });
        }

        [Test]
        public void GetCommonApplicationDataDirectory_ReturnsTempDirectoryOfPartition()
        {
            var policy = SpecialPathPolicy.For("Test");

            DirectoryInfo result = policy.GetCommonApplicationDataDirectory();

            Assert.Multiple(() =>
            {
                Assert.IsTrue(result.Exists);
                Assert.AreEqual(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), @"Gallio\Test"), result.ToString());
            });
        }
    }
}
