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
