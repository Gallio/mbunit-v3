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
using Gallio;
using MbUnit.Framework;

namespace Gallio.Tests
{
    [TestFixture]
    [TestsOn(typeof(ActionChain<>))]
    public class ActionChainTest
    {
        private ActionChain<string> chain;
        private List<string> trace;

        [SetUp]
        public void SetUp()
        {
            chain = new ActionChain<string>();
            trace = new List<string>();
        }

        [Test]
        public void GetAndSetAction()
        {
            Assert.AreEqual(ActionChain<string>.NoOp, chain.Action);

            Action<string> action = CreateAction("");
            chain.Action = action;
            Assert.AreEqual(action, chain.Action);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void SetAction_ThrowsIfValueIfNull()
        {
            chain.Action = null;
        }

        [Test]
        public void Before()
        {
            chain.Before(CreateAction("abc"));
            chain.Before(CreateAction("def"));
            chain.Before(CreateAction("ghi"));
            chain.Action("key");

            AssertTraceEquals("key: ghi", "key: def", "key: abc");
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Before_ThrowsIfActionIsNull()
        {
            chain.Before(null);
        }

        [Test]
        public void After()
        {
            chain.After(CreateAction("abc"));
            chain.After(CreateAction("def"));
            chain.After(CreateAction("ghi"));
            chain.Action("key");

            AssertTraceEquals("key: abc", "key: def", "key: ghi");
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void After_ThrowsIfActionIsNull()
        {
            chain.After(null);
        }

        [Test]
        public void Around()
        {
            chain.Around(CreateActionDecorator("abc", "xyz"));
            chain.Around(CreateActionDecorator("def", "uvw"));
            chain.Around(CreateActionDecorator("ghi", "rst"));
            chain.Action("key");

            AssertTraceEquals("key: ghi", "key: def", "key: abc", "key: xyz", "key: uvw", "key: rst");
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Around_ThrowsIfActionIsNull()
        {
            chain.Around(null);
        }

        [Test]
        public void ClearSetsTheActionBackToNoOp()
        {
            chain.Before(CreateAction("abc"));
            Assert.AreNotEqual(ActionChain<string>.NoOp, chain.Action);

            chain.Clear();
            Assert.AreEqual(ActionChain<string>.NoOp, chain.Action);
        }

        private void AssertTraceEquals(params string[] expectedTrace)
        {
            Assert.AreElementsEqual(expectedTrace, trace);
        }

        private void Trace(string obj, string token)
        {
            trace.Add(obj + ": " + token);

        }

        private Action<string> CreateAction(string token)
        {
            return delegate(string obj)
            {
                Trace(obj, token);
            };
        }

        private ActionDecorator<string> CreateActionDecorator(string beforeToken, string afterToken)
        {
            return delegate(string obj, Action<string> action)
            {
                Trace(obj, beforeToken);
                action(obj);
                Trace(obj, afterToken);
            };
        }
    }
}
