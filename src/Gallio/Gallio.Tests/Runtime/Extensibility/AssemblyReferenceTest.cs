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
using Gallio.Runtime.Extensibility;
using MbUnit.Framework;

namespace Gallio.Tests.Runtime.Extensibility
{
    [TestsOn(typeof(AssemblyReference))]
    public class AssemblyReferenceTest
    {
        [Test]
        public void Constructor_WhenAssemblyNameIsNull_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => new AssemblyReference(null, new Uri("file:///c:/foo.dll")));
        }

        [Test]
        public void Constructor_WhenCodeBaseIsNotAnAbsoluteUri_Throws()
        {
            Assert.Throws<ArgumentException>(() => new AssemblyReference(new AssemblyName("Gallio"),
                new Uri("bar/foo.dll", UriKind.Relative)));
        }

        [Test]
        public void Constructor_WhenAssemblyNameNotNullButCodeBaseNull_InitializesPropertiesWithNullCodeBase()
        {
            var assemblyName = new AssemblyName("Gallio");

            var assemblyReference = new AssemblyReference(assemblyName, null);
            Assert.AreSame(assemblyName, assemblyReference.AssemblyName);
            Assert.IsNull(assemblyReference.CodeBase);
        }

        [Test]
        public void Constructor_WhenAssemblyNameAndCodeBaseNotNull_InitializesPropertiesAsSpecified()
        {
            var assemblyName = new AssemblyName("Gallio");
            var codeBase = new Uri("file:///c:/foo.dll");

            var assemblyReference = new AssemblyReference(assemblyName, codeBase);
            Assert.AreSame(assemblyName, assemblyReference.AssemblyName);
            Assert.AreEqual(codeBase, assemblyReference.CodeBase);
        }
    }
}
