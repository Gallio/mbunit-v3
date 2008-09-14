using System;
using System.Drawing;
using Gallio.Icarus.Controllers.EventArgs;
using Gallio.Icarus.Controllers.Interfaces;
using Gallio.Runtime.Logging;
using Gallio.Utilities;

namespace Gallio.Icarus.Controllers
{
    public class RuntimeLogController : BaseLogger, IRuntimeLogController
    {
        public event EventHandler<RuntimeLogEventArgs> LogMessage;

        protected override void LogImpl(LogSeverity severity, string message, Exception exception)
        {
            Color color = Color.Black;
            switch (severity)
            {
                case LogSeverity.Error:
                    color = Color.Red;
                    break;

                case LogSeverity.Warning:
                    color = Color.Gold;
                    break;

                case LogSeverity.Info:
                    color = Color.Gray;
                    break;

                case LogSeverity.Debug:
                    color = Color.DarkGray;
                    break;
            }

            if (LogMessage == null)
                return;

            LogMessage(this, new RuntimeLogEventArgs(message, color));

            if (exception != null)
                LogMessage(this, new RuntimeLogEventArgs(ExceptionUtils.SafeToString(exception), color));
        }
    }
}
