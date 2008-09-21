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
using System.Reflection;
using Gallio.Runtime.Logging;
using Gallio.Reflection;
using Gallio.Reflection.Impl;
using MbUnit.Framework;

namespace Gallio.Tests.Reflection.Impl
{
    [TestFixture]
    [TestsOn(typeof(DebugSymbolUtils))]
    public class DebugSymbolUtilsTest : BaseUnitTest
    {
        [Test, ExpectedArgumentNullException]
        public void GetSourceLocation_Method_ThrowsIfMethodIsNull()
        {
            DebugSymbolUtils.GetSourceLocation((MethodBase)null);
        }

        [Test]
        public void GetSourceLocation_Method_ReturnsUnknownIfAssemblyIsDynamic()
        {
            ILogger mockLogger = Mocks.CreateMock<ILogger>();
            Mocks.ReplayAll();

            Assert.AreEqual(CodeLocation.Unknown, DebugSymbolUtils.GetSourceLocation(mockLogger.GetType().GetMethods()[0]));
        }

        [Test]
        public void GetSourceLocation_Method_ReturnsValidLocationForConcreteMethod()
        {
            CodeLocation codeLocation = DebugSymbolUtils.GetSourceLocation(typeof(Sample).GetMethod("ConcreteMethod"));

            Assert.EndsWith(codeLocation.Path, GetType().Name + ".cs");
            Assert.Between(codeLocation.Line, 1000, 1003);
            Assert.AreEqual(codeLocation.Column, 0, "No column information should be returned because it is inaccurate.");
        }

        [Test, ExpectedArgumentNullException]
        public void GetSourceLocation_Type_ThrowsIfTypeIsNull()
        {
            DebugSymbolUtils.GetSourceLocation((Type)null);
        }

        [Test]
        public void GetSourceLocation_Type_ReturnsUnknownIfAssemblyIsDynamic()
        {
            ILogger mockLogger = Mocks.CreateMock<ILogger>();
            Mocks.ReplayAll();

            Assert.AreEqual(CodeLocation.Unknown, DebugSymbolUtils.GetSourceLocation(mockLogger.GetType()));
        }

        [Test]
        public void GetSourceLocation_Type_ReturnsUnknownIfTypeHasNoMembers()
        {
            Assert.AreEqual(CodeLocation.Unknown, DebugSymbolUtils.GetSourceLocation(typeof(EmptyType)));
        }

        [Test]
        [Row(typeof(TypeWithPublicInstanceConstructor))]
        [Row(typeof(TypeWithNonPublicInstanceConstructor))]
        [Row(typeof(TypeWithNonPublicStaticConstructor))]
        [Row(typeof(TypeWithPublicInstanceMethod))]
        [Row(typeof(TypeWithNonPublicInstanceMethod))]
        [Row(typeof(TypeWithPublicStaticMethod))]
        [Row(typeof(TypeWithNonPublicStaticMethod))]
        [Row(typeof(TypeWithPublicInstanceProperty))]
        [Row(typeof(TypeWithNonPublicInstanceProperty))]
        [Row(typeof(TypeWithPublicStaticProperty))]
        [Row(typeof(TypeWithNonPublicStaticProperty))]
        public void GetSourceLocation_Type_ReturnsCorrectFileNameIfTypeHasMethodsConstructorsOrProperties(Type type)
        {
            CodeLocation codeLocation = DebugSymbolUtils.GetSourceLocation(type);

            Assert.EndsWith(codeLocation.Path, GetType().Name + ".cs");
            Assert.AreEqual(0, codeLocation.Line);
            Assert.AreEqual(0, codeLocation.Column);
        }

        private abstract class Sample
        {
#line 1000
            public void ConcreteMethod()
            {
                ConcreteMethod();
            }

            protected abstract void AbstractMethod();
        }

        private class EmptyType
        {
        }

        private class TypeWithPublicInstanceConstructor
        {
            public TypeWithPublicInstanceConstructor()
            {
            }
        }

        private class TypeWithNonPublicInstanceConstructor
        {
            protected TypeWithNonPublicInstanceConstructor()
            {
            }
        }

        private class TypeWithNonPublicStaticConstructor
        {
            static TypeWithNonPublicStaticConstructor()
            {
            }
        }

        private class TypeWithPublicInstanceMethod
        {
            public void Method()
            {
            }
        }

        private class TypeWithNonPublicInstanceMethod
        {
            protected void Method()
            {
            }
        }

        private class TypeWithPublicStaticMethod
        {
            public static void Method()
            {
            }
        }

        private class TypeWithNonPublicStaticMethod
        {
            protected static void Method()
            {
            }
        }

        private class TypeWithPublicInstanceProperty
        {
            public int Property { get { return 42; } }
        }

        private class TypeWithNonPublicInstanceProperty
        {
            protected int Property { get { return 42; } }
        }

        private class TypeWithPublicStaticProperty
        {
            public static int Property { get { return 42; } }
        }

        private class TypeWithNonPublicStaticProperty
        {
            protected static int Property { get { return 42; } }
        }
    }
}