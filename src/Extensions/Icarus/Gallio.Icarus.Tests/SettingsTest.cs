using Gallio.Runtime.Logging;
using MbUnit.Framework;

namespace Gallio.Icarus.Tests
{
    [TestsOn(typeof(Settings))]
    public class SettingsTest
    {
        [Test]
        public void MinLogSeverity_should_default_to_info()
        {
            var settings = new Settings();

            Assert.AreEqual(LogSeverity.Info, settings.MinLogSeverity);
        }

        [Test]
        public void TreeViewCategories_should_be_an_empty_list()
        {
            var settings = new Settings();

            Assert.IsNotNull(settings.TreeViewCategories);
        }
    }
}
