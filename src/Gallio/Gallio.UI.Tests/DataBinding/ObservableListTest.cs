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

using Gallio.UI.DataBinding;
using MbUnit.Framework;

namespace Gallio.UI.Tests.DataBinding
{
    [TestsOn(typeof(ObservableList<>))]
    public class ObservableListTest
    {
        [Test]
        public void Adding_an_item_to_the_list_raises_property_changed()
        {
            var observable = new ObservableList<string>();
            var propertyChangedRaised = false;
            observable.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == "Add") 
                    propertyChangedRaised = true;
            };

            observable.Add("test");

            Assert.IsTrue(propertyChangedRaised);
        }

        [Test]
        public void Clearing_the_list_raises_property_changed()
        {
            var observable = new ObservableList<string>();
            var propertyChangedRaised = false;
            observable.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == "Clear")
                    propertyChangedRaised = true;
            };

            observable.Clear();

            Assert.IsTrue(propertyChangedRaised);
        }

        [Test]
        public void Removing_an_item_from_the_list_raises_property_changed()
        {
            const string item = "test";
            var observable = new ObservableList<string> { item };
            var propertyChangedRaised = false;
            observable.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == "Remove")
                    propertyChangedRaised = true;
            };
            
            observable.Remove(item);

            Assert.IsTrue(propertyChangedRaised);
        }
    }
}
