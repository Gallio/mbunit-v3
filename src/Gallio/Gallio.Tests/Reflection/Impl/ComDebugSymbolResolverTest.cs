// Copyright 2005-2008 Gallio Project - http://www.gallio.org/
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
using System.Text;
using Gallio.Runtime.Logging;
using Gallio.Reflection;
using Gallio.Reflection.Impl;
using MbUnit.Framework;

namespace Gallio.Tests.Reflection.Impl
{
    [TestFixture]
    [TestsOn(typeof(ComDebugSymbolResolver))]
    public class ComDebugSymbolResolverTest
    {
        [Test, ExpectedArgumentNullException]
        public void GetSourceLocationForMethod_ThrowsIfAssemblyFileIsNull()
        {
            new ComDebugSymbolResolver().GetSourceLocationForMethod(null, 0);
        }

        [Test]
        public void GetSourceLocationForMethod_ReturnsValidLocationForConcreteMethod()
        {
            CodeLocation codeLocation = GetSourceLocationForMethod("ConcreteMethod");

            OldStringAssert.EndsWith(codeLocation.Path, GetType().Name + ".cs");
            Assert.Between(codeLocation.Line, 1000, 1003);
            Assert.AreEqual(codeLocation.Column, 0, "No column information should be returned because it is inaccurate.");
        }

        [Test]
        public void GetSourceLocationForMethod_ReturnsUnknownIfMethodIsAbstract()
        {
            CodeLocation codeLocation = GetSourceLocationForMethod("AbstractMethod");
            Assert.AreEqual(CodeLocation.Unknown, codeLocation);
        }

        [Test]
        public void GetSourceLocationForMethod_ThrowsIfAssemblyFileDoesNotExist()
        {
            new ComDebugSymbolResolver().GetSourceLocationForMethod("NoSuchAssembly", 0);
        }

        [Test]
        public void GetSourceLocationForMethod_ThrowsIfAssemblyExistsButThereIsNoPDB()
        {
            new ComDebugSymbolResolver().GetSourceLocationForMethod(typeof(ILogger).Assembly.Location, 0);
        }

        [Test]
        public void GetSourceLocationForMethod_ThrowsIfMethodTokenNotValid()
        {
            new ComDebugSymbolResolver().GetSourceLocationForMethod(GetType().Assembly.Location, 0);
        }

        private CodeLocation GetSourceLocationForMethod(string methodName)
        {
            ComDebugSymbolResolver resolver = new ComDebugSymbolResolver();

            MethodInfo method = typeof(Sample).GetMethod(methodName, BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
            return resolver.GetSourceLocationForMethod(method.DeclaringType.Assembly.Location, method.MetadataToken);
        }

        private abstract class Sample
        {
#line 1000
            private void ConcreteMethod()
            {
                ConcreteMethod();
            }

            protected abstract void AbstractMethod();
        }
    }
}
