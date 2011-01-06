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
// WITHOUT WC:\Projects\Gallio\v3\src\Extensions\MbUnitCpp\Gallio.MbUnitCppAdapter.Tests\Model\Bridge\UnmanagedTestRepositoryTest.csARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gallio.MbUnitCppAdapter.Model.PE;
using Gallio.Model;
using Gallio.Common.Reflection;
using MbUnit.Framework;
using System.Reflection;

namespace Gallio.MbUnitCppAdapter.Tests.Model.PE
{
    [TestFixture]
    [TestsOn(typeof(PEImageReader))]
    public class PEImageReaderTest
    {
        private static PEImageInfo GetPEImageInfo(string file)
        {
            using (var reader = new PEImageReader(file))
            {
                return reader.Read();
            }
        }

        [Test]
        public void GetArchitecture32()
        {
            string file = Helper.GetTestResources("x86");
            PEImageInfo info = GetPEImageInfo(file);
            Assert.IsNotNull(info);
            Assert.AreEqual(ProcessorArchitecture.X86, info.Architecture);
            Assert.Contains(info.Imports.Select(x => x.ToUpper()), "KERNEL32.DLL");
            Assert.Contains(info.Exports, "MbUnitCpp_GetHeadTest");
            Assert.Contains(info.Exports, "MbUnitCpp_GetNextTest");
            Assert.Contains(info.Exports, "MbUnitCpp_GetString");
            Assert.Contains(info.Exports, "MbUnitCpp_ReleaseAllStrings");
            Assert.Contains(info.Exports, "MbUnitCpp_ReleaseString");
            Assert.Contains(info.Exports, "MbUnitCpp_RunTest");
        }

        [Test]
        public void GetArchitecture64()
        {
            string file = Helper.GetTestResources("x64");
            PEImageInfo info = GetPEImageInfo(file);
            Assert.IsNotNull(info);
            Assert.Contains(new[] { ProcessorArchitecture.IA64, ProcessorArchitecture.Amd64 }, info.Architecture);
            Assert.Contains(info.Imports.Select(x => x.ToUpper()), "KERNEL32.DLL");
            Assert.Contains(info.Exports, "MbUnitCpp_GetHeadTest");
            Assert.Contains(info.Exports, "MbUnitCpp_GetNextTest");
            Assert.Contains(info.Exports, "MbUnitCpp_GetString");
            Assert.Contains(info.Exports, "MbUnitCpp_ReleaseAllStrings");
            Assert.Contains(info.Exports, "MbUnitCpp_ReleaseString");
            Assert.Contains(info.Exports, "MbUnitCpp_RunTest");
        }
    }
}
