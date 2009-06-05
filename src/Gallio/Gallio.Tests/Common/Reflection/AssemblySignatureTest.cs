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
using System.Linq;
using System.Reflection;
using System.Text;
using Gallio.Common.Reflection;
using MbUnit.Framework;

namespace Gallio.Tests.Common.Reflection
{
    [TestsOn(typeof(AssemblySignature))]
    public class AssemblySignatureTest
    {
        [Test]
        public void Constructor_WhenNameIsNull_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => new AssemblySignature(null));
        }

        [Test]
        public void Constructor_WhenNameIsValid_ReturnsInitializedInstance()
        {
            var sig = new AssemblySignature("name");

            Assert.Multiple(() =>
            {
                Assert.AreEqual("name", sig.Name);
                Assert.IsNull(sig.MinVersion);
                Assert.IsNull(sig.MaxVersion);
            });
        }

        [Test]
        public void SetVersion_SetsBothMinAndMaxVersion()
        {
            var sig = new AssemblySignature("name");

            sig.SetVersion(new Version(1, 2, 3, 4));
            Assert.AreEqual(new Version(1, 2, 3, 4), sig.MinVersion);
            Assert.AreEqual(new Version(1, 2, 3, 4), sig.MaxVersion);

            sig.SetVersion(null);
            Assert.IsNull(sig.MinVersion);
            Assert.IsNull(sig.MaxVersion);
        }

        [Test]
        public void SetVersionRange_WhenBothArgumentsNull_SetsBothMinAndMaxVersion()
        {
            var sig = new AssemblySignature("name");
            sig.SetVersion(new Version(1, 2, 3, 4));

            sig.SetVersionRange(null, null);
            Assert.IsNull(sig.MinVersion);
            Assert.IsNull(sig.MaxVersion);
        }

        [Test]
        public void SetVersionRange_WhenBothArgumentsNotNull_SetsBothMinAndMaxVersion()
        {
            var sig = new AssemblySignature("name");

            sig.SetVersionRange(new Version(1, 0, 0, 0), new Version(1, 2, 65535, 65535));
            Assert.AreEqual(new Version(1, 0, 0, 0), sig.MinVersion);
            Assert.AreEqual(new Version(1, 2, 65535, 65535), sig.MaxVersion);
        }

        [Test]
        public void SetVersionRange_WhenOneArgumentNullButNotTheOther_Throws()
        {
            var sig = new AssemblySignature("name");

            var ex = Assert.Throws<ArgumentException>(() => sig.SetVersionRange(null, new Version(1, 2, 3, 4)));
            Assert.Contains(ex.Message, "Min and max version must either both be non-null or both be null.");

            ex = Assert.Throws<ArgumentException>(() => sig.SetVersionRange(new Version(1, 2, 3, 4), null));
            Assert.Contains(ex.Message, "Min and max version must either both be non-null or both be null.");
        }

        [Test]
        public void SetVersionRange_WhenMinGreaterThanMax_Throws()
        {
            var sig = new AssemblySignature("name");

            var ex = Assert.Throws<ArgumentException>(() => sig.SetVersionRange(new Version(2, 0, 0, 0), new Version(1, 2, 3, 4)));
            Assert.Contains(ex.Message, "Min version must be less than or equal to max version.");
        }

        [Test]
        public void IsMatch_WhenAssemblyNameIsNull_Throws()
        {
            var sig = new AssemblySignature("name");

            Assert.Throws<ArgumentNullException>(() => sig.IsMatch(null));
        }

        [Test]
        [Row("Name", "Name", true)]
        [Row("Name", "DifferentName", false)]
        [Row("Name, Version=1.2.3.4", "Name", true)]
        [Row("Name, Version=1.2.3.4", "Name, Version=1.2.3.4", true)]
        [Row("Name, Version=1.2.3.4", "Name, Version=2.3.4.5", false)]
        [Row("Name, Version=1.0.0.0-1.2.65535.65535", "Name, Version=1.2.3.4", true)]
        [Row("Name, Version=1.0.0.0-1.2.65535.65535", "Name, Version=1.0.0.0", true)]
        [Row("Name, Version=1.0.0.0-1.2.65535.65535", "Name, Version=1.2.65535.65535", true)]
        [Row("Name, Version=1.0.0.0-1.2.65535.65535", "Name, Version=0.0.0.0", false)]
        [Row("Name, Version=1.0.0.0-1.2.65535.65535", "Name, Version=2.0.0.0", false)]
        public void IsMatch_WhenAssemblyNameIsValid_ReturnsExpectedResult(string signature, string name, bool expectedResult)
        {
            var sig = AssemblySignature.Parse(signature);

            Assert.AreEqual(expectedResult, sig.IsMatch(new AssemblyName(name)));
        }

        [Test]
        public void Parse_WhenStringIsNull_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => AssemblySignature.Parse(null));
        }

        [Test]
        public void Parse_WhenStringIsMalformed_Throws()
        {
            var ex = Assert.Throws<ArgumentException>(() => AssemblySignature.Parse("Abc-def"));
            Assert.Contains(ex.Message, "The specified assembly signature is not valid.");
        }

        [Test]
        [Row("Name", "Name", null, null)]
        [Row("Name,Version=1.2.3.4", "Name", "1.2.3.4", "1.2.3.4")]
        [Row("Name,Version=1.0.0.0-1.2.65535.65535", "Name", "1.0.0.0", "1.2.65535.65535")]
        [Row("Name , Version = 1.0.0.0 - 1.2.65535.65535", "Name", "1.0.0.0", "1.2.65535.65535", Description="Whitespace ignored")]
        public void Parse_WhenStringIsWellFormed_ReturnsParsedSignature(string signature,
            string expectedName, string expectedMinVersion, string expectedMaxVersion)
        {
            var sig = AssemblySignature.Parse(signature);

            Assert.Multiple(() =>
            {
                Assert.AreEqual(expectedName, sig.Name);
                Assert.AreEqual(expectedMinVersion, sig.MinVersion != null ? sig.MinVersion.ToString() : null);
                Assert.AreEqual(expectedMaxVersion, sig.MaxVersion != null ? sig.MaxVersion.ToString() : null);
            });
        }

        [Test]
        [Row("Name")]
        [Row("Name, Version=1.2.3.4")]
        [Row("Name, Version=1.0.0.0-1.2.65535.65535")]
        public void ToString_ReturnsFormattedSignture(string signature)
        {
            var sig = AssemblySignature.Parse(signature);

            Assert.AreEqual(signature, sig.ToString());
        }
    }
}
