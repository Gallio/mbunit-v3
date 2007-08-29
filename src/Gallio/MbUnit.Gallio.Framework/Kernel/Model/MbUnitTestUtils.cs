using System;
using System.Reflection;

namespace MbUnit.Framework.Kernel.Model
{
    /// <summary>
    /// Utility functions to help build MbUnit tests.
    /// </summary>
    public static class MbUnitTestUtils
    {
        /// <summary>
        /// Creates an action that invokes a method on the fixture without parameters.
        /// </summary>
        /// <param name="method">The method to invoke</param>
        /// <returns>The action</returns>
        public static Action<MbUnitTestState> CreateFixtureMethodInvoker(MethodInfo method)
        {
            if (method == null)
                throw new ArgumentNullException(@"method");

            return delegate(MbUnitTestState state)
            {
                if (method.IsStatic)
                {
                    method.Invoke(null, null);
                }
                else
                {
                    object instance = state.FixtureInstance;
                    if (instance == null)
                        throw new ModelException(Resources.MbUnitTestUtils_ExpectedToInvokeAnInstanceMethod);

                    method.Invoke(instance, null);
                }
            };
        }
    }
}
