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
using Gallio.Reflection;
using MbUnit.Framework;
using MbUnit.Framework.ContractVerifiers;

namespace Gallio.Tests.Reflection
{
    [TestsOn(typeof(TypeName))]
    public class TypeNameTest
    {
        [Test, ExpectedArgumentNullException]
        public void Constructor_WhenCalledWithNullType_Throws()
        {
            new TypeName((Type) null);
        }

        [Test, ExpectedArgumentNullException]
        public void Constructor_WhenCalledWithNullTypeInfo_Throws()
        {
            new TypeName((ITypeInfo)null);
        }

        [Test, ExpectedArgumentNullException]
        public void Constructor_WhenCalledWithNullAssemblyQualifiedName_Throws()
        {
            new TypeName((string)null);
        }

        [Test, ExpectedArgumentNullException]
        public void Constructor_WhenCalledWithNullFullName_Throws()
        {
            new TypeName(null, new AssemblyName("System"));
        }

        [Test, ExpectedArgumentNullException]
        public void Constructor_WhenCalledWithNullAssemblyName_Throws()
        {
            new TypeName("TypeFullName", null);
        }

        [Test]
        public void Constructor_WhenCalledWithGenericParameterType_Throws()
        {
            Type genericParam = typeof(Dummy<>).GetGenericArguments()[0];

            Assert.Throws<ArgumentException>(() => new TypeName(genericParam));
        }

        [Test]
        public void Constructor_WhenCalledWithGenericParameterTypeInfo_Throws()
        {
            Type genericParam = typeof(Dummy<>).GetGenericArguments()[0];
            ITypeInfo genericParamInfo = Reflector.Wrap(genericParam);

            Assert.Throws<ArgumentException>(() => new TypeName(genericParamInfo));
        }

        [Test]
        public void Constructor_WhenCalledWithValidType_InitializesProperties()
        {
            Type type = typeof(Dummy<>);

            TypeName typeName = new TypeName(type);

            Assert.AreEqual(type.FullName, typeName.FullName);
            Assert.AreEqual(type.Assembly.GetName().FullName, typeName.AssemblyName.FullName);
            Assert.AreEqual(type.AssemblyQualifiedName, typeName.AssemblyQualifiedName);
        }

        [Test]
        public void Constructor_WhenCalledWithValidTypeInfo_InitializesProperties()
        {
            Type type = typeof(Dummy<>);
            ITypeInfo typeInfo = Reflector.Wrap(type);

            TypeName typeName = new TypeName(typeInfo);

            Assert.AreEqual(typeInfo.FullName, typeName.FullName);
            Assert.AreEqual(typeInfo.Assembly.GetName().FullName, typeName.AssemblyName.FullName);
            Assert.AreEqual(typeInfo.AssemblyQualifiedName, typeName.AssemblyQualifiedName);
        }

        [Test]
        public void Constructor_WhenCalledWithValidAssemblyQualifiedName_InitializesProperties()
        {
            Type type = typeof(Dummy<>);
            string assemblyQualifiedName = type.AssemblyQualifiedName;

            TypeName typeName = new TypeName(assemblyQualifiedName);

            Assert.AreEqual(type.FullName, typeName.FullName);
            Assert.AreEqual(type.Assembly.GetName().FullName, typeName.AssemblyName.FullName);
            Assert.AreEqual(type.AssemblyQualifiedName, typeName.AssemblyQualifiedName);
        }

        [Test]
        public void Constructor_WhenCalledWithValidAssemblyQualifiedNameIncludingTypeArguments_InitializesProperties()
        {
            Type type = typeof(Dummy<int>);
            string assemblyQualifiedName = type.AssemblyQualifiedName;

            TypeName typeName = new TypeName(assemblyQualifiedName);

            Assert.AreEqual(type.FullName, typeName.FullName);
            Assert.AreEqual(type.Assembly.GetName().FullName, typeName.AssemblyName.FullName);
            Assert.AreEqual(type.AssemblyQualifiedName, typeName.AssemblyQualifiedName);
        }

        [Test]
        public void Constructor_WhenCalledWithValidFullNameAndAssemblyName_InitializesProperties()
        {
            Type type = typeof(Dummy<>);

            TypeName typeName = new TypeName(type.FullName, type.Assembly.GetName());

            Assert.AreEqual(type.FullName, typeName.FullName);
            Assert.AreEqual(type.Assembly.GetName().FullName, typeName.AssemblyName.FullName);
            Assert.AreEqual(type.AssemblyQualifiedName, typeName.AssemblyQualifiedName);
        }

        [Test]
        public void ToString_Always_ReturnsAssemblyQualifiedName()
        {
            Type type = typeof(Dummy<>);

            TypeName typeName = new TypeName(type);

            Assert.AreEqual(type.AssemblyQualifiedName, typeName.ToString());
        }

        [Test]
        public void Resolve_WhenValidTypeName_ReturnsTheNamedType()
        {
            Type type = typeof(Dummy<>);

            TypeName typeName = new TypeName(type);

            Assert.AreEqual(type, typeName.Resolve());
        }

        [Test]
        public void Resolve_WhenInvalidTypeName_ThrowsResolveException()
        {
            TypeName typeName = new TypeName("SomeUndefinedType", new AssemblyName("UnknownAssembly"));

            var ex = Assert.Throws<ReflectionResolveException>(() => typeName.Resolve());
            Assert.Contains(ex.Message, "SomeUndefinedType");
        }

        [Test]
        [Row("PartialName", true)]
        [Row("FullName, Version=1.2.3.4", false)]
        public void HasPartialAssemblyName_WhenTheAssemblyNameIsPartial_ReturnsTrueOtherwiseFalse(string assemblyName, bool isPartial)
        {
            TypeName typeName = new TypeName("Type", new AssemblyName(assemblyName));

            AssertEx.That(() => isPartial == typeName.HasPartialAssemblyName);
        }

        [Test]
        public void ConvertToPartialAssemblyName_WhenTheAssemblyNameIsPartial_ReturnsTheSameInstance()
        {
            TypeName typeName = new TypeName("Type", new AssemblyName("PartialName"));

            Assert.AreSame(typeName, typeName.ConvertToPartialAssemblyName());
        }

        [Test]
        public void ConvertToPartialAssemblyName_WhenTheAssemblyNameIsFull_ReturnsANewInstanceWithThePartialName()
        {
            TypeName typeName = new TypeName("Type", new AssemblyName("FullName, Version=1.2.3.4"));

            TypeName result = typeName.ConvertToPartialAssemblyName();

            Assert.IsTrue(result.HasPartialAssemblyName);
            Assert.AreEqual("Type, FullName", result.AssemblyQualifiedName);
        }

        [VerifyContract]
        public readonly IContract EqualityAndHashCode = new EqualityContract<TypeName>()
        {
            ImplementsOperatorOverloads = false,
            EquivalenceClasses =
            {
                { new TypeName("Type, Assembly"), new TypeName("Type,Assembly") },
                { new TypeName("DifferentType, Assembly") },
                { new TypeName("Type, DifferentAssembly") },
                { new TypeName("Type, Assembly, Version=1.2.3.4") }
            }
        };

        private class Dummy<T>
        {
        }
    }
}
