using MbUnit.Framework;

namespace Gallio.Icarus.Tests
{
    [Category("Views")]
    class ReloadDialogTest
    {
        [Test]
        public void Constructor_Test()
        {
            ReloadDialog reloadDialog = new ReloadDialog("assembly");
            Assert.IsFalse(reloadDialog.AlwaysReloadTests);
        }
    }
}
