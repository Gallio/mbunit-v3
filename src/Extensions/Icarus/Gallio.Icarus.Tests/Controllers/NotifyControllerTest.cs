using System.ComponentModel;
using System.Threading;
using Gallio.Icarus.Controllers;
using Gallio.Icarus.Utilities;
using MbUnit.Framework;
using Gallio.Icarus.Tests.Utilities;

namespace Gallio.Icarus.Tests.Controllers
{
    internal class NotifyControllerTest
    {
        private class TestController : NotifyController
        {
            public void Notify(string propertyName)
            {
                OnPropertyChanged(new PropertyChangedEventArgs(propertyName));
            }
        }

        [Test]
        public void OnPropertyChanged_without_synchronization_context()
        {
            var testController = new TestController();
            bool propertyChangedFlag = false;
            testController.PropertyChanged += delegate { propertyChangedFlag = true; };

            testController.Notify("test");

            Assert.AreEqual(false, propertyChangedFlag);
        }

        [Test]
        public void OnPropertyChanged_with_synchronization_context()
        {
            var testController = new TestController();
            bool propertyChangedFlag = false;
            string propertyName = "test";
            testController.PropertyChanged += delegate(object sender, PropertyChangedEventArgs e)
            {
                if (e.PropertyName == propertyName)
                    propertyChangedFlag = true;
            };
            testController.SynchronizationContext = new TestSynchronizationContext();

            testController.Notify(propertyName);

            Assert.AreEqual(true, propertyChangedFlag);
        }
    }
}
