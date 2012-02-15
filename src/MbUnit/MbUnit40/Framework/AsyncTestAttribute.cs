using System.Threading.Tasks;
using Gallio.Framework.Pattern;

namespace MbUnit.Framework
{
    /// <summary>
    /// Test async methods.
    /// </summary>
    public class AsyncTestAttribute : TestAttribute
    {
        /// <inheritdoc />
        protected override void Execute(PatternTestInstanceState state)
        {
            var result = state.InvokeTestMethod();

            var task = result as Task;
            if (task != null)
                task.Wait();
        }
    }
}