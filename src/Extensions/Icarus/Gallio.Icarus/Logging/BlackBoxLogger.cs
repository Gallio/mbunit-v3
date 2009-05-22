using System;
using System.IO;
using Gallio.Common.Concurrency;
using Gallio.Common.Policies;

namespace Gallio.Icarus.Logging
{
    internal class BlackBoxLogger
    {
        private static LockBox<BlackBoxLogger> logger = new LockBox<BlackBoxLogger>(new BlackBoxLogger());

        public static void Initialize()
        {
            try
            {
                if (File.Exists(Paths.BlackBoxLogFile))
                    File.Delete(Paths.BlackBoxLogFile);
            }
            catch
            {
                // eat any exceptions
            }
        }

        public static void Log(CorrelatedExceptionEventArgs e)
        {
            logger.Write(bl => bl.LogImpl(e));
        }

        private void LogImpl(CorrelatedExceptionEventArgs e)
        {
            try
            {
                using (var sw = new StreamWriter(Paths.BlackBoxLogFile, true))
                {
                    sw.WriteLine(string.Format("Unhandled exception reported at: {0}", DateTime.Now.ToString()));
                    sw.WriteLine(e.Message);
                    sw.WriteLine();
                    sw.WriteLine(e.GetDescription());
                    sw.WriteLine();
                }
            }
            catch
            { 
                // eat any exceptions
            }
        }
    }
}
