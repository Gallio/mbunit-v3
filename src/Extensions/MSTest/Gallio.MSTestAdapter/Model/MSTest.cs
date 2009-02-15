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
using System.Security.Cryptography;
using System.Text;
using Gallio.Model;
using Gallio.Reflection;
using Gallio.Model.Execution;

namespace Gallio.MSTestAdapter.Model
{
    internal class MSTest : BaseTest
    {
        private static readonly SHA1CryptoServiceProvider provider = new SHA1CryptoServiceProvider();
        private string testName;
        private string guid;

        /// <summary>
        /// Initializes a test initially without a parent.
        /// </summary>
        /// <param name="name">The name of the component</param>
        /// <param name="codeElement">The point of definition, or null if none</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="name"/> is null</exception>
        public MSTest(string name, ICodeElementInfo codeElement)
            : base(name, codeElement)
        {
            PopulateTestName();
            PopulateGuid();
        }

        /// <summary>
        /// Gets the name MSTest uses to refer to the test.
        /// </summary>
        internal string TestName
        {
            get { return testName; }
        }

        /// <summary>
        /// Gets the GUID MSTest uses to refer to the test.
        /// </summary>
        internal string Guid
        {
            get { return guid; }
        }

        private void PopulateTestName()
        {
            CodeReference codeReference = CodeElement.CodeReference;
            testName = String.Empty;
            if (String.IsNullOrEmpty(codeReference.NamespaceName))
                testName += codeReference.NamespaceName + ".";
            testName += codeReference.TypeName + "." + codeReference.MemberName;
        }

        private void PopulateGuid()
        {
            guid = GuidFromString(testName).ToString();
        }

        internal static Guid GuidFromString(string fullMethodName)
        {
            byte[] sourceArray = provider.ComputeHash(Encoding.Unicode.GetBytes(fullMethodName));
            byte[] destinationArray = new byte[16];
            Array.Copy(sourceArray, destinationArray, 16);
            return new Guid(destinationArray);
        }
    }
}
