using System.Threading.Tasks;

namespace MbUnit.Framework.Tests
{
    public class AsyncTests
    {
        [AsyncTest]
        public Task Pass()
        {
            return Task.Factory.StartNew(() => { });
        }
    }
}