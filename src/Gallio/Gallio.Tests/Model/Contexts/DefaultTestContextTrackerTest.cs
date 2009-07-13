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
using System.Text;
using System.Threading;
using Gallio.Framework;
using Gallio.Model.Contexts;
using MbUnit.Framework;

namespace Gallio.Tests.Model.Commands
{
    [TestFixture]
    [TestsOn(typeof(DefaultTestContextTracker))]
    public class DefaultTestContextTrackerTest
    {
        private DefaultTestContextTracker mgr;

        [SetUp]
        public void SetUp()
        {
            mgr = new DefaultTestContextTracker();
        }

        [Test]
        public void TheInitialContextShouldBeNull()
        {
            Assert.IsNull(mgr.CurrentContext);
            Assert.IsNull(mgr.GlobalContext);
        }

        [Test]
        public void TheCurrentThreadInheritsTheGlobalContext()
        {
            StubTestContext context = new StubTestContext();
            mgr.GlobalContext = context;

            Assert.AreSame(context, mgr.GlobalContext);
            Assert.AreSame(context, mgr.CurrentContext);

            mgr.GlobalContext = null;
            Assert.IsNull(mgr.GlobalContext);
            Assert.IsNull(mgr.CurrentContext);
        }

        [Test]
        public void TheCurrentThreadUsesTheThreadDefaultContextInsteadOfInheritingTheGlobalContext()
        {
            mgr.GlobalContext = new StubTestContext();

            StubTestContext context = new StubTestContext();
            mgr.SetThreadDefaultContext(Thread.CurrentThread, context);

            Assert.AreSame(context, mgr.GetThreadDefaultContext(Thread.CurrentThread));
            Assert.AreSame(context, mgr.CurrentContext);
            Assert.AreNotSame(context, mgr.GlobalContext);
        }

        [Test]
        public void TheCurrentThreadUsesTheEnteredContextInsteadOfItsDefaultContext()
        {
            StubTestContext enteredContext = new StubTestContext();
            mgr.EnterContext(enteredContext);

            StubTestContext context = new StubTestContext();
            mgr.SetThreadDefaultContext(Thread.CurrentThread, context);

            Assert.AreSame(context, mgr.GetThreadDefaultContext(Thread.CurrentThread));
            Assert.AreSame(enteredContext, mgr.CurrentContext);
            Assert.IsNull(mgr.GlobalContext);
        }

        [Test]
        public void TheEnteredContextPropagatesAcrossThreadsWithIndependentStacks()
        {
            StubTestContext rootContext = new StubTestContext();
            mgr.EnterContext(rootContext);
            Assert.AreSame(rootContext, mgr.CurrentContext);

            Tasks.StartThreadTask("A", delegate
            {
                Assert.AreSame(rootContext, mgr.CurrentContext);

                StubTestContext leafContext = new StubTestContext();
                mgr.EnterContext(leafContext);
                Assert.AreSame(leafContext, mgr.CurrentContext);

                Tasks.StartThreadTask("B", delegate
                {
                    Assert.AreSame(leafContext, mgr.CurrentContext);

                    StubTestContext leafContext2 = new StubTestContext();
                    mgr.EnterContext(leafContext2);
                    Assert.AreSame(leafContext2, mgr.CurrentContext);
                }).Join(new TimeSpan(0, 0, 1));

                Assert.AreSame(leafContext, mgr.CurrentContext);
            }).Join(new TimeSpan(0, 0, 1));

            Assert.AreSame(rootContext, mgr.CurrentContext);

            Tasks.JoinAndVerify(new TimeSpan(0, 0, 5));
        }

        [Test]
        public void TheEnteredContextIsExitedWhenTheCookieIsDisposed()
        {
            StubTestContext context1 = new StubTestContext();
            using (mgr.EnterContext(context1))
            {
                Assert.AreSame(context1, mgr.CurrentContext);

                StubTestContext context2 = new StubTestContext();
                using (mgr.EnterContext(context2))
                {
                    Assert.AreSame(context2, mgr.CurrentContext);
                }

                Assert.AreSame(context1, mgr.CurrentContext);
            }

            Assert.IsNull(mgr.CurrentContext);
        }

        [Test]
        public void TheEnteredContextCanBeNullWhichWillOverrideEvenTheGlobalContext()
        {
            StubTestContext rootContext = new StubTestContext();
            mgr.GlobalContext = rootContext;

            using (mgr.EnterContext(null))
            {
                Assert.IsNull(mgr.CurrentContext);

                StubTestContext context1 = new StubTestContext();
                using (mgr.EnterContext(context1))
                {
                    Assert.AreSame(context1, mgr.CurrentContext);

                    using (mgr.EnterContext(null))
                    {
                        Assert.IsNull(mgr.CurrentContext);

                        StubTestContext context2 = new StubTestContext();
                        using (mgr.EnterContext(context2))
                        {
                            Assert.AreSame(context2, mgr.CurrentContext);
                        }

                        Assert.IsNull(mgr.CurrentContext);
                    }

                    Assert.AreSame(context1, mgr.CurrentContext);
                }

                Assert.IsNull(mgr.CurrentContext);
            }

            Assert.AreSame(rootContext, mgr.CurrentContext);
        }

        [Test]
        public void AContextCannotBeExitedTwice()
        {
            IDisposable cookie = mgr.EnterContext(new StubTestContext());
            cookie.Dispose();

            Assert.Throws<InvalidOperationException>(delegate { cookie.Dispose(); });
        }

        [Test]
        public void AContextCannotBeExitedOnADifferentThreadFromTheOneThatEnteredIt()
        {
            IDisposable cookie = mgr.EnterContext(new StubTestContext());

            Tasks.StartThreadTask("A different thread.", delegate
            {
                Assert.Throws<InvalidOperationException>(delegate { cookie.Dispose(); });
            });

            Tasks.JoinAndVerify(new TimeSpan(0, 0, 5));
        }
    }
}
