using System;
using System.Collections.Generic;

namespace MbUnit.Framework.Kernel.Model.Reflection
{
    /// <summary>
    /// Wraps a list of <see cref="ITest" /> for reflection.
    /// </summary>
    public sealed class TestInfoList : BaseInfoList<ITest, TestInfo>
    {
        /// <summary>
        /// Creates a wrapper for the specified input list of model objects.
        /// </summary>
        /// <param name="inputList">The input list</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="inputList"/> is null</exception>
        public TestInfoList(IList<ITest> inputList)
            : base(inputList)
        {
        }

        /// <inheritdoc />
        protected override TestInfo Wrap(ITest inputItem)
        {
            return new TestInfo(inputItem);
        }
    }
}