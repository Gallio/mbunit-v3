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
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System;
using System.Collections.Generic;
using Gallio;
using Gallio.Common;
using MbUnit.Framework;

namespace Gallio.Tests.Common
{
    [TestFixture]
    [TestsOn(typeof(FuncChain<,>))]
    public class FuncChainTest
    {
        private FuncChain<string, int> chain;

        private int LengthOf(string x)
        {
            return x.Length;
        }

        [SetUp]
        public void SetUp()
        {
            chain = new FuncChain<string, int>(LengthOf);
        }

        [Test]
        public void GetAndSetFunc()
        {
            Assert.AreEqual(LengthOf, chain.Func);

            Gallio.Common.Func<string, int> func = x => 1;
            chain.Func = func;
            Assert.AreEqual(func, chain.Func);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void SetFunc_ThrowsIfValueIfNull()
        {
            chain.Func = null;
        }

        [Test]
        public void Around()
        {
            chain.Around((str, inner) => inner(str + "x") * 3);
            Assert.AreEqual(15, chain.Func("test"));
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Around_ThrowsIfActionIsNull()
        {
            chain.Around(null);
        }
    }
}