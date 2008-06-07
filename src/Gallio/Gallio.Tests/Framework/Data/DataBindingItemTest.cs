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
using Gallio.Framework.Data;
using MbUnit.Framework;

namespace Gallio.Tests.Framework.Data
{
    [TestFixture]
    [TestsOn(typeof(DataBindingItem))]
    public class DataBindingItemTest : BaseUnitTest
    {
        [Test, ExpectedArgumentNullException]
        public void ConstructorThrowsIfRowIsNull()
        {
            new DataBindingItem(null);
        }

        [Test]
        public void GetRowReturnsSameRowAsWasSpecifiedInTheConstructor()
        {
            IDataRow row = Mocks.Stub<IDataRow>();
            DataBindingItem item = new DataBindingItem(row);
            Assert.AreSame(row, item.GetRow());
        }

        [Test]
        public void GetRowThrowsObjectDisposedExceptionWhenDisposed()
        {
            IDataRow row = Mocks.Stub<IDataRow>();
            DataBindingItem item = new DataBindingItem(row);
            item.Dispose();

            InterimAssert.Throws<ObjectDisposedException>(delegate { item.GetRow(); });
        }

        [Test]
        public void DisposeCallsEventHandlersInReverseOrderTheFirstTime()
        {
            int disposedRepetitions = 1;

            IDataRow row = Mocks.Stub<IDataRow>();
            DataBindingItem item = new DataBindingItem(row);
            item.Disposed += delegate { disposedRepetitions += 1; };
            item.Disposed += delegate { disposedRepetitions *= 3; };

            item.Dispose();
            item.Dispose();

            Assert.AreEqual(1 * 3 + 1, disposedRepetitions,
                "Should have called the dispose handlers exactly once in reverse order.");
        }

        [Test]
        public void DisposeDoesNotCallEventHandlerThatHasBeenRemoved()
        {
            bool disposed = false;
            EventHandler handler = delegate { disposed = true; };

            IDataRow row = Mocks.Stub<IDataRow>();
            DataBindingItem item = new DataBindingItem(row);
            item.Disposed += handler;
            item.Disposed -= handler;

            item.Dispose();

            Assert.IsFalse(disposed, "Should not call removed handler.");
        }
    }
}
