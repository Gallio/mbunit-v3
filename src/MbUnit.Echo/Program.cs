using System;
using System.Collections.Generic;
using System.Text;

namespace MbUnit.Echo
{
    /// <summary>
    /// The MbUnit console test runner program.
    /// </summary>
    public class Program
    {
        [STAThread]
        [LoaderOptimization(LoaderOptimization.MultiDomain)]
        public static int Main(string[] args)
        {
            try
            {
                using (MainClass main = new MainClass())
                {
                    return main.Run(args);
                }
            }
            finally
            {
                Console.ResetColor();
            }
        }
    }
}
