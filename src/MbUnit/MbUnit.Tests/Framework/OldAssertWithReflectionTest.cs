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
using Gallio.Framework.Assertions;
using MbUnit.Framework;

namespace MbUnit.Tests.Framework
{
    [TestFixture]
    [TestsOn(typeof(Assert))]
    public class OldAssertWithReflectionTest
    {
        #region IsSealed
        [Test]
        public void IsSealed()
        {
            OldReflectionAssert.IsSealed(typeof(SuccessClass));
        }
        [Test]
        [ExpectedException(typeof(AssertionException))]
        public void IsSealedFail()
        {
            OldReflectionAssert.IsSealed(typeof(FailClass));
        }
        #endregion

        #region HasConstructor
        [Test]
        public void HasConstructorNoParamter()
        {
            OldReflectionAssert.HasConstructor(typeof(SuccessClass));
        }

        [Test]
        [ExpectedException(typeof(AssertionException))]
        public void HasConstructorNoParamterFail()
        {
            OldReflectionAssert.HasConstructor(typeof(FailClass));
        }
        [Test]
        public void HasConstructorStringParamter()
        {
            OldReflectionAssert.HasConstructor(typeof(SuccessClass),typeof(string));
        }

        [Test]
        [ExpectedException(typeof(AssertionException))]
        public void HasConstructorStringParameterFail()
        {
            OldReflectionAssert.HasConstructor(typeof(FailClass),typeof(string));
        }
        [Test]
        public void HasConstructorPrivate()
        {
            OldReflectionAssert.HasConstructor(typeof(NoPublicConstructor),
                BindingFlags.Instance | BindingFlags.NonPublic);
        }
        #endregion

        #region HasDefaultConstructor
        [Test]
        public void HasDefaultConstructor()
        {
            OldReflectionAssert.HasDefaultConstructor(typeof(SuccessClass));
        }
        [Test]
        [ExpectedException(typeof(AssertionException))]
        public void HasDefaultConstructorFail()
        {
            OldReflectionAssert.HasDefaultConstructor(typeof(FailClass));
        }
        #endregion

        #region NotCreatable
        [Test]
        public void NotCreatable()
        {
            OldReflectionAssert.NotCreatable(typeof(NoPublicConstructor));
        }
        [Test]
        [ExpectedException(typeof(AssertionException))]
        public void NotCreatableFail()
        {
            OldReflectionAssert.NotCreatable(typeof(SuccessClass));
        }
        #endregion 

        #region HasField
        [Test]
        public void HasField()
        {
            OldReflectionAssert.HasField(typeof(SuccessClass),"PublicField");
        }
        [Test]
        [ExpectedException(typeof(AssertionException))]
        public void HasFieldFail()
        {
            OldReflectionAssert.HasField(typeof(FailClass), "PublicField");
        }
        [Test]
        [ExpectedException(typeof(AssertionException))]
        public void HasPrivateFieldFail()
        {
            OldReflectionAssert.HasField(typeof(SuccessClass), "privateField");
        }
        #endregion

        #region HasMethod
        [Test]
        public void HasMethod()
        {
            OldReflectionAssert.HasMethod(typeof(SuccessClass), "Method");
        }
        [Test]
        public void HasMethodOneParameter()
        {
            OldReflectionAssert.HasMethod(typeof(SuccessClass), "Method",typeof(string));
        }
        [Test]
        [ExpectedException(typeof(AssertionException))]
        public void HasMethodFail()
        {
            OldReflectionAssert.HasMethod(typeof(FailClass), "Method");
        }
        [Test]
        [ExpectedException(typeof(AssertionException))]
        public void HasMethodOneParameterFail()
        {
            OldReflectionAssert.HasMethod(typeof(FailClass), "Method",typeof(string));
        }
        #endregion 

        #region IsAssignableFrom
        [Test]
        public void ObjectIsAssignableFromString()
        {
            OldReflectionAssert.IsAssignableFrom(typeof(Object), typeof(string));            
        }
        [Test]
        [ExpectedException(typeof(AssertionException))]
        public void StringIsNotAssignableFromObject()
        {
            OldReflectionAssert.IsAssignableFrom(typeof(string),typeof(Object));
        }
        #endregion

        #region IsInstanceOf
        [Test]
        public void IsInstanceOf()
        {
            OldReflectionAssert.IsInstanceOf(typeof(string), "hello");
        }
        [Test]
        [ExpectedException(typeof(AssertionException))]
        public void IsInstanceOfFail()
        {
            OldReflectionAssert.IsInstanceOf(typeof(string), 1);
        }
        #endregion

        #region ReadOnlyProperty
        [Test]
        public void ReadOnlyProperty()
        {
            OldReflectionAssert.ReadOnlyProperty(typeof(string), "Length");
        }
        [Test]
        [ExpectedException(typeof(AssertionException))]
        public void ReadOnlyPropertyFail()
        {
            OldReflectionAssert.ReadOnlyProperty(typeof(SuccessClass), "Prop");
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
