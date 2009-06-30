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
using System.Reflection;
using System.Text;
using Gallio.Common.Policies;
using MbUnit.Framework;

namespace Gallio.Tests.Common.Policies
{
    [TestsOn(typeof(VersionPolicy))]
    public class VersionPolicyTest
    {
        [Test]
        public void GetVersionLabel_WhenVersionIsNull_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => VersionPolicy.GetVersionLabel((Version)null));
        }

        [Test]
        public void GetVersionLabel_WhenVersionIsProvided_ReturnsFormattedLabel()
        {
            var version = new Version(1, 2, 3, 4);

            string label = VersionPolicy.GetVersionLabel(version);

            Assert.AreEqual("1.2 build 3", label);
        }

        [Test]
        public void GetVersionLabel_WhenAssemblyIsNull_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => VersionPolicy.GetVersionLabel((Assembly)null));
        }

        [Test]
        public void GetVersionLabel_WhenAssemblyIsProvided_ReturnsFormattedLabel()
        {
            var assembly = typeof(VersionPolicy).Assembly;
            var version = VersionPolicy.GetVersionNumber(assembly);

            string label = VersionPolicy.GetVersionLabel(version);

            Assert.AreEqual(string.Format("{0}.{1} build {2}", version.Major, version.Minor, version.Build), label);
        }

        [Test]
        public void GetVersionNumber_WhenAssemblyIsNull_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => VersionPolicy.GetVersionNumber(null));
        }

        [Test]
        public void GetVersionNumber_WhenAssemblyIsProvided_ReturnsFileVersionWhenAvailableOrAssemblyVersionOtherwise()
        {
            var assembly = typeof(VersionPolicy).Assembly;

            var version = VersionPolicy.GetVersionNumber(typeof(VersionPolicy).Assembly);

            var attribs = (AssemblyFileVersionAttribute[])assembly.GetCustomAttributes(typeof(AssemblyFileVersionAttribute), false);
            if (attribs.Length != 0)
                Assert.AreEqual(version.ToString(), attribs[0].Version);
            else
                Assert.AreEqual(version, assembly.GetName().Version);
        }
    }
}
