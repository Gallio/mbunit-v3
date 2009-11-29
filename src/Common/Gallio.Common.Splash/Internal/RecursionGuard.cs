using System;
using System.Collections.Generic;
using System.Text;

namespace Gallio.Common.Splash.Internal
{
    internal delegate void ProtectedAction();

    internal delegate T ProtectedFunc<T>();

    internal class RecursionGuard
    {
        private bool entered;

        public void Do(ProtectedAction action)
        {
            try
            {
                if (entered)
                    ThrowRecursionDetected();

                entered = true;
                action();
            }
            finally
            {
                entered = false;
            }
        }

        public T Do<T>(ProtectedFunc<T> func)
        {
            try
            {
                if (entered)
                    ThrowRecursionDetected();

                return func();
            }
            finally
            {
                entered = false;
            }
        }

        private static void ThrowRecursionDetected()
        {
            throw new InvalidOperationException("Detected inappropriate recursion!");
        }
    }
}
