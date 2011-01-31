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

using Gallio.UI.Common.Synchronization;
using Gallio.UI.DataBinding;
using MbUnit.Framework;

namespace Gallio.UI.Tests.DataBinding
{
    [TestsOn(typeof(Observable<>))]
    public class ObservableTest
    {
        [Test]
        public void Value_passed_in_to_ctor_should_be_returned()
        {
            const string value = "test";

            var observable = new Observable<string>(value);

            Assert.AreEqual(value, observable.Value);
        }

        [Test]
        public void Value_set_should_be_returned()
        {
            const string value = "test";
            var observable = new Observable<string>();

            observable.Value = value;

            Assert.AreEqual(value, observable.Value);
        }

        [Test]
        public void Value_should_be_implicit()
        {
            const string value = "test";
            
            var observable = new Observable<string>(value);

            Assert.AreEqual(value, observable);
        }

        [Test]
        public void Setting_value_should_raise_property_changed()
        {
            var observable = new Observable<string>();
            bool propertyChangedRaised = false;
            observable.PropertyChanged += (s, e) => propertyChangedRaised = true;

            observable.Value = "test";

            Assert.IsTrue(propertyChangedRaised);
        }

        [Test]
        public void Setting_value_should_raise_property_changed_using_sync_context()
        {
            var observable = new Observable<string>();
            bool propertyChangedRaised = false;
            observable.PropertyChanged += (s, e) => propertyChangedRaised = true;
            SynchronizationContext.Current = new TestSynchronizationContext();

            observable.Value = "test";

            Assert.IsTrue(propertyChangedRaised);
        }
    }
}
