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

using System;
using System.IO;
using System.Runtime.InteropServices;
using MbUnit.Tools.VsIntegration;

namespace MbUnit.Tools.CodeGenerator
{
    [Guid("42d1dbed-4997-40b5-bbb9-4ff7ea2d09b8")]
    [VsCustomToolRegistration("MbUnit.AssertGenerator",
        "Generates a partial class containing variations on Assert methods with different message format parameters.",
        new string[] { VsCategoryGuid.CSharp },
        new string[] { "8.0" })]
    public class MbUnitAssertGenerator : VsCustomTool
    {
        public override string DefaultExtension
        {
            get { return @".cs"; }
        }

        public override byte[] GenerateCode(string fileName, string fileContents, string defaultNamespace)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                using (TextWriter writer = new StreamWriter(stream))
                    GenerateCode(fileContents, writer);

                return stream.ToArray();
            }
        }

        private void GenerateCode(string fileContents, TextWriter writer)
        {

        }
    }
}
