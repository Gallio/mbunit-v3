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
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using Castle.Core.Logging;
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
            SourceLocation sourceLocation = GetSourceLocationForMethod("ConcreteMethod");

            StringAssert.EndsWith(sourceLocation.Filename, GetType().Name + ".cs");
            Assert.Between(sourceLocation.Line, 1000, 1003);
            Assert.GreaterEqualThan(sourceLocation.Column, 1);
        }

        [Test]
        public void GetSourceLocationForMethod_ReturnsNullIfMethodIsAbstract()
        {
            SourceLocation sourceLocation = GetSourceLocationForMethod("AbstractMethod");
            Assert.IsNull(sourceLocation);
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

        private SourceLocation GetSourceLocationForMethod(string methodName)
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
