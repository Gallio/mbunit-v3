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
using System.Reflection;
using Gallio.Common.Reflection;
using MbUnit.Framework;
using MbUnit.TestResources;
using MbUnit.TestResources.ProcessorArchitecture;

namespace Gallio.Tests.Common.Reflection
{
    [TestsOn(typeof(AssemblyUtils))]
    public class AssemblyUtilsTest
    {
        [Test]
        public void IsAssembly_WhenStreamIsNull_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => AssemblyUtils.IsAssembly((Stream) null));
        }

        [Test]
        public void IsAssembly_WhenFilePathIsNull_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => AssemblyUtils.IsAssembly((string)null));
        }

        [Test]
        public void IsAssembly_WhenStreamIsEmpty_ReturnsFalse()
        {
            var stream = new MemoryStream();

            Assert.IsFalse(AssemblyUtils.IsAssembly(stream));
        }

        [Test]
        public void IsAssembly_WhenStreamDoesNotContainPEHeaderSignature_ReturnsFalse()
        {
            var stream = new MemoryStream();
            stream.SetLength(1024); // only contains nulls

            Assert.IsFalse(AssemblyUtils.IsAssembly(stream));
        }

        [Test]
        public void IsAssembly_WhenStreamIsACLRAssembly_ReturnsTrue()
        {
            var path = Assembly.GetExecutingAssembly().Location;

            Assert.IsTrue(AssemblyUtils.IsAssembly(path));
        }

        [Test]
        public void IsAssembly_WhenStreamIsAPEFileButNotAnAssembly_ReturnsFalse()
        {
            var path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.System), @"kernel32.dll");

            Assert.IsFalse(AssemblyUtils.IsAssembly(path));
        }
        
        [Test]
        public void GetAssemblyMetadata_WhenStreamIsNull_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => AssemblyUtils.GetAssemblyMetadata((Stream)null, AssemblyMetadataFields.Default));
        }

        [Test]
        public void GetAssemblyMetadata_WhenFilePathIsNull_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => AssemblyUtils.GetAssemblyMetadata((string)null, AssemblyMetadataFields.Default));
        }

        [Test]
        public void GetAssemblyMetadata_WhenStreamIsEmpty_ReturnsNull()
        {
            var stream = new MemoryStream();

            Assert.IsNull(AssemblyUtils.GetAssemblyMetadata(stream, AssemblyMetadataFields.Default));
        }

        [Test]
        public void GetAssemblyMetadata_WhenStreamDoesNotContainPEHeaderSignature_ReturnsNull()
        {
            var stream = new MemoryStream();
            stream.SetLength(1024); // only contains nulls

            Assert.IsNull(AssemblyUtils.GetAssemblyMetadata(stream, AssemblyMetadataFields.Default));
        }

        [Test]
        public void GetAssemblyMetadata_WhenStreamIsAnMSILAssembly_ReturnsMetadata()
        {
            var path = "MbUnit.TestResources.dll";

            AssemblyMetadata metadata = AssemblyUtils.GetAssemblyMetadata(path, AssemblyMetadataFields.Default);

            Assert.Multiple(() =>
            {
                Assert.IsNotNull(metadata);
                Assert.AreEqual(2, metadata.MajorRuntimeVersion);
                Assert.AreEqual(5, metadata.MinorRuntimeVersion);
                Assert.AreEqual(ProcessorArchitecture.MSIL, metadata.ProcessorArchitecture);
            });
        }

        [Test]
        public void GetAssemblyMetadata_WhenStreamIsAnx86Assembly_ReturnsMetadata()
        {
            var path = "MbUnit.TestResources.x86.dll";

            AssemblyMetadata metadata = AssemblyUtils.GetAssemblyMetadata(path, AssemblyMetadataFields.Default);

            Assert.Multiple(() =>
            {
                Assert.IsNotNull(metadata);
                Assert.AreEqual(2, metadata.MajorRuntimeVersion);
                Assert.AreEqual(5, metadata.MinorRuntimeVersion);
                Assert.AreEqual(ProcessorArchitecture.X86, metadata.ProcessorArchitecture);
            });
        }

        [Test]
        public void GetAssemblyMetadata_WhenStreamIsAnx64Assembly_ReturnsMetadata()
        {
            var path = "MbUnit.TestResources.x64.dll";

            AssemblyMetadata metadata = AssemblyUtils.GetAssemblyMetadata(path, AssemblyMetadataFields.Default);

            Assert.Multiple(() =>
            {
                Assert.IsNotNull(metadata);
                Assert.AreEqual(2, metadata.MajorRuntimeVersion);
                Assert.AreEqual(5, metadata.MinorRuntimeVersion);
                Assert.AreEqual(ProcessorArchitecture.Amd64, metadata.ProcessorArchitecture);
            });
        }

        [Test]
        public void GetAssemblyMetadata_WhenStreamIsAPEFileButNotAnAssembly_ReturnsNull()
        {
            var path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.System), @"kernel32.dll");

            Assert.IsNull(AssemblyUtils.GetAssemblyMetadata(path, AssemblyMetadataFields.Default));
        }
        
        [Test]
        public void GetAssemblyMetadata_WhenAttemptingToAccessAssemblyNameButNotRead_Throws()
        {
            var path = "MbUnit.TestResources.dll";

            AssemblyMetadata metadata = AssemblyUtils.GetAssemblyMetadata(path, AssemblyMetadataFields.Default);

            AssemblyName x;
            Assert.Throws<InvalidOperationException>(() => x = metadata.AssemblyName);
        }

        [Test]
        public void GetAssemblyMetadata_WhenAttemptingToAccessAssemblyReferencesButNotRead_Throws()
        {
            var path = "MbUnit.TestResources.dll";

            AssemblyMetadata metadata = AssemblyUtils.GetAssemblyMetadata(path, AssemblyMetadataFields.Default);

            IList<AssemblyName> x;
            Assert.Throws<InvalidOperationException>(() => x = metadata.AssemblyReferences);
        }

        [Test]
        public void GetAssemblyMetadata_WhenAssemblyNameFieldSpecified_ReturnsMetadataIncludingAssemblyName()
        {
            var path = "MbUnit.TestResources.dll";

            AssemblyMetadata metadata = AssemblyUtils.GetAssemblyMetadata(path, AssemblyMetadataFields.AssemblyName);

            Assembly assembly = typeof(SimpleTest).Assembly;
            Assert.AreEqual(assembly.GetName(), metadata.AssemblyName);
        }

        [Test]
        public void GetAssemblyMetadata_WhenAssemblyReferencesFieldSpecified_ReturnsMetadataIncludingAssemblyReferences()
        {
            var path = "MbUnit.TestResources.dll";

            AssemblyMetadata metadata = AssemblyUtils.GetAssemblyMetadata(path, AssemblyMetadataFields.AssemblyReferences);

            Assembly assembly = typeof(SimpleTest).Assembly;
            Assert.AreElementsEqual(assembly.GetReferencedAssemblies(), metadata.AssemblyReferences);
        }

        [Test]
        public void GetAssemblyMetadata_WhenRuntimeVersionFieldSpecified_ReturnsMetadataIncludingRuntimeVersion()
        {
            var path = "MbUnit.TestResources.dll";

            AssemblyMetadata metadata = AssemblyUtils.GetAssemblyMetadata(path, AssemblyMetadataFields.RuntimeVersion);

            Assembly assembly = typeof(SimpleTest).Assembly;
            Assert.AreEqual("v2.0.50727", metadata.RuntimeVersion);
        }
    }
}