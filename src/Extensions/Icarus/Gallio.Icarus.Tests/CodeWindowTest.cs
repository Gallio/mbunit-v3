using Gallio.Reflection;
using MbUnit.Framework;

namespace Gallio.Icarus.Tests
{
    [Category("Views")]
    class CodeWindowTest
    {
        [Test]
        public void Constructor_Test()
        {
            CodeWindow codeWindow = new CodeWindow(CodeLocation.Unknown);
        }
    }
}
