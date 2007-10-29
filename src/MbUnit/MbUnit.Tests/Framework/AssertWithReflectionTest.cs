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
using System.Reflection;
using MbUnit.Framework;

namespace MbUnit.Tests.Framework
{
    [TestFixture]
    [TestsOn(typeof(Assert))]
    public class AssertWithReflectionTest
    {
        #region IsSealed
        [Test]
        public void IsSealed()
        {
            ReflectionAssert.IsSealed(typeof(SuccessClass));
        }
        [Test]
        [ExpectedException(typeof(AssertionException))]
        public void IsSealedFail()
        {
            ReflectionAssert.IsSealed(typeof(FailClass));
        }
        #endregion

        #region HasConstructor
        [Test]
        public void HasConstructorNoParamter()
        {
            ReflectionAssert.HasConstructor(typeof(SuccessClass));
        }

        [Test]
        [ExpectedException(typeof(AssertionException))]
        public void HasConstructorNoParamterFail()
        {
            ReflectionAssert.HasConstructor(typeof(FailClass));
        }
        [Test]
        public void HasConstructorStringParamter()
        {
            ReflectionAssert.HasConstructor(typeof(SuccessClass),typeof(string));
        }

        [Test]
        [ExpectedException(typeof(AssertionException))]
        public void HasConstructorStringParameterFail()
        {
            ReflectionAssert.HasConstructor(typeof(FailClass),typeof(string));
        }
        [Test]
        public void HasConstructorPrivate()
        {
            ReflectionAssert.HasConstructor(typeof(NoPublicConstructor),
                BindingFlags.Instance | BindingFlags.NonPublic);
        }
        #endregion

        #region HasDefaultConstructor
        [Test]
        public void HasDefaultConstructor()
        {
            ReflectionAssert.HasDefaultConstructor(typeof(SuccessClass));
        }
        [Test]
        [ExpectedException(typeof(AssertionException))]
        public void HasDefaultConstructorFail()
        {
            ReflectionAssert.HasDefaultConstructor(typeof(FailClass));
        }
        #endregion

        #region NotCreatable
        [Test]
        public void NotCreatable()
        {
            ReflectionAssert.NotCreatable(typeof(NoPublicConstructor));
        }
        [Test]
        [ExpectedException(typeof(AssertionException))]
        public void NotCreatableFail()
        {
            ReflectionAssert.NotCreatable(typeof(SuccessClass));
        }
        #endregion 

        #region HasField
        [Test]
        public void HasField()
        {
            ReflectionAssert.HasField(typeof(SuccessClass),"PublicField");
        }
        [Test]
        [ExpectedException(typeof(AssertionException))]
        public void HasFieldFail()
        {
            ReflectionAssert.HasField(typeof(FailClass), "PublicField");
        }
        [Test]
        [ExpectedException(typeof(AssertionException))]
        public void HasPrivateFieldFail()
        {
            ReflectionAssert.HasField(typeof(SuccessClass), "privateField");
        }
        #endregion

        #region HasMethod
        [Test]
        public void HasMethod()
        {
            ReflectionAssert.HasMethod(typeof(SuccessClass), "Method");
        }
        [Test]
        public void HasMethodOneParameter()
        {
            ReflectionAssert.HasMethod(typeof(SuccessClass), "Method",typeof(string));
        }
        [Test]
        [ExpectedException(typeof(AssertionException))]
        public void HasMethodFail()
        {
            ReflectionAssert.HasMethod(typeof(FailClass), "Method");
        }
        [Test]
        [ExpectedException(typeof(AssertionException))]
        public void HasMethodOneParameterFail()
        {
            ReflectionAssert.HasMethod(typeof(FailClass), "Method",typeof(string));
        }
        #endregion 

        #region IsAssignableFrom
        [Test]
        public void ObjectIsAssignableFromString()
        {
            ReflectionAssert.IsAssignableFrom(typeof(Object), typeof(string));            
        }
        [Test]
        [ExpectedException(typeof(AssertionException))]
        public void StringIsNotAssignableFromObject()
        {
            ReflectionAssert.IsAssignableFrom(typeof(string),typeof(Object));
        }
        #endregion

        #region IsInstanceOf
        [Test]
        public void IsInstanceOf()
        {
            ReflectionAssert.IsInstanceOf(typeof(string), "hello");
        }
        [Test]
        [ExpectedException(typeof(AssertionException))]
        public void IsInstanceOfFail()
        {
            ReflectionAssert.IsInstanceOf(typeof(string), 1);
        }
        #endregion

        #region ReadOnlyProperty
        [Test]
        public void ReadOnlyProperty()
        {
            ReflectionAssert.ReadOnlyProperty(typeof(string), "Length");
        }
        [Test]
        [ExpectedException(typeof(AssertionException))]
        public void ReadOnlyPropertyFail()
        {
            ReflectionAssert.ReadOnlyProperty(typeof(SuccessClass), "Prop");
        }
        #endregion        

        #region dummy test classes
        internal class FailClass
        {
            public FailClass(int i)
            { }
        }

        internal class NoPublicConstructor
        {
            private NoPublicConstructor() { }
        }
        internal sealed class SuccessClass
        {
            public string PublicField = "public";
            private string privateField = "private";

            public SuccessClass()
            {
            }

            public SuccessClass(string s)
            { }

            public string Prop
            {
                get
                {
                    return privateField;
                }
                set
                { }
            }

            public void Method()
            { }
            public void Method(string s)
            { }
        }
        #endregion    
    }
}
